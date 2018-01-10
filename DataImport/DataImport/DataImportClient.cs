using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

using CsvHelper;
using CsvHelper.Configuration;
using System.Collections;

namespace DataImport
{
    class DataImportClient
    {
        public const string MainPath = @"E:\Data Import\";
        public const string ExportsPath = MainPath + @"Exports\";
        public const string FileFormatPath = MainPath + @"FileFormatSkeleton.txt";
        public readonly string[] fields = File.ReadAllText(FileFormatPath).Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
        //public Group[] groups { get; set; }

        public List<FileFormatSkeleton> records { get; set; }

        private CsvConfiguration cfg { get; set; }
        private DefaultCsvClassMap<FileFormatSkeleton> fileFormatMap { get; set; }
        private Dictionary<string, FieldInfo> ffsFields { get; set; }
        //private static TypedReference ffsRef = 
        public Lookup<string, Lookup<string, string>> csvMapping { get; set; }

        public DataImportClient()
        {
            /*
            groups = fields.GroupBy(
                f => f.Substring(
                    0, f.IndexOf('.')
                ), f => f.Remove(
                    0, f.IndexOf('.') + 1
                )
            ).Select(g => new Group(
                g.Key, g.ToArray()
            )).OrderBy(g => g.prefix).ToArray();*/

            cfg = new CsvConfiguration();
            cfg.Delimiter = "\t";
            cfg.DetectColumnCountChanges = true;
            cfg.TrimFields = true;
            //cfg.RegisterClassMap(typeof(Group));

            csvMapping = (Lookup<string, Lookup<string, string>>)fields.ToLookup(
                f => f.Substring(0, f.IndexOf('.')),
                f => (Lookup<string, string>)Enumerable
                    .Empty<string>().ToLookup(
                        p => f.Remove(
                            0, f.IndexOf('.'))));

           /* var csvMap = new DefaultCsvClassMap<Lookup<string, string>>();
            foreach(var group in csvMapping)
            {
                foreach(var subgroup in group)
                {
                    foreach(var item in subgroup)
                    {
                        var newMap = new CsvPropertyMap(
                            typeof(IGrouping<string, string>).GetType().GetProperty(item.Key)
                        );
                    }
                }
            }*/

            fileFormatMap = new DefaultCsvClassMap<FileFormatSkeleton>();
            Type ffs = typeof(FileFormatSkeleton);

            ffsFields = ffs.GetFields(BindingFlags.SetProperty).ToDictionary(f => f.Name, f => f);

            var fieldNames = fields.Select(f => new KeyValuePair<string, string>(f, f.Replace('.', '_')));
            foreach (var field in fieldNames)
            {
                var newMap = new CsvPropertyMap(
                    ffs.GetProperty(field.Value));
                newMap.Name(field.Key);
                fileFormatMap.PropertyMaps.Add(newMap);
            }

            cfg.RegisterClassMap(fileFormatMap);
        }

        /*
        public string GroupsToString()
        {
            return String.Join("\r\n",
                groups.Select(g => g.ToString())
            );
        }
        */

        public void AddRecord(Dictionary<string, string> rec)
        {
            FileFormatSkeleton record = new FileFormatSkeleton();
            TypedReference ffsRef = TypedReference.MakeTypedReference(record, ffsFields.Values.ToArray());
            foreach(var field in ffsFields)
            {
                field.Value.SetValueDirect(ffsRef,
                    rec.TryGetValue(
                        field.Key.Replace('_', '.'), 
                        out string prop
                    ) ? prop : ""
                );
            }

            records.Add(record);
        }

        public void Export(string fileName)
        {
            FileStream fs = new FileStream(ExportsPath + fileName + ".txt", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);
            CsvWriter writer = new CsvWriter(sw, cfg);

            writer.WriteRecords(records);
            //writer.Dispose();
            sw.Close();
            fs.Close();
        }
    }

    public struct Group : IEnumerable<Field>
    {
        public string prefix { get; set; }
        public Field[] fields { get; set; }

        public Group(string key, string[] items)
        {
            prefix = key;
            fields = items.Select(
                i => new Field(i)
            ).ToArray();//.OrderBy(f => f.label).ToArray();
        }

        public string ToString(bool pretty)
        {
            StringBuilder sb = new StringBuilder(prefix);
            if (pretty)
            {
                sb.Append("\r\n\t");
                sb.Append(String.Join("\r\n\t", fields));
            }
            else
            {
                sb.Append("\r\n");
                sb.Append(String.Join("\r\n", fields));
            }
            return sb.ToString();
        }

        IEnumerator<Field> IEnumerable<Field>.GetEnumerator()
        {
            return ((IEnumerable<Field>)fields).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Field>)fields).GetEnumerator();
        }
    }

    public struct Field : IEnumerable<string>
    {
        public string label { get; set; }
        public List<string> values { get; set; }

        public Field(string name)
        {
            label = name;
            values = new List<string>();
        }

        public string ToString(bool pretty)
        {
            StringBuilder sb = new StringBuilder(label);
            if (pretty)
            {
                sb.Append("\r\n\t");
                sb.Append(String.Join("\r\n\t", values));
            }
            else
            {
                sb.Append("\r\n");
                sb.Append(String.Join("\r\n", values));
            }
            return sb.ToString();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)values).GetEnumerator();
        }
    }

    public sealed class FileFormatMap : CsvClassMap<FileFormatSkeleton>
    {
        public FileFormatMap(int index)
        {
            
        }
    }

    public struct FileFormatSkeleton
    {
        public string L_LocationCode { get; set; }
        public string L_BillToCode { get; set; }
        public string L_Company { get; set; }
        public string L_LName { get; set; }
        public string L_FName { get; set; }
        public string L_Title { get; set; }
        public string L_Address { get; set; }
        public string L_Address2 { get; set; }
        public string L_Zip { get; set; }
        public string L_City { get; set; }
        public string L_State { get; set; }
        public string L_Salutation { get; set; }
        public string L_SalutationName { get; set; }
        public string L_Phone { get; set; }
        public string L_PhoneExt { get; set; }
        public string L_AltPhone { get; set; }
        public string L_AltPhoneExt { get; set; }
        public string L_Fax { get; set; }
        public string L_FaxExt { get; set; }
        public string L_Mobile { get; set; }
        public string L_MobileExt { get; set; }
        public string L_EMail { get; set; }
        public string L_URL { get; set; }
        public string L_MapCode { get; set; }
        public string L_Division { get; set; }
        public string L_Type { get; set; }
        public string L_Builder { get; set; }
        public string L_Source { get; set; }
        public string L_County { get; set; }
        public string L_Subdivision { get; set; }
        public string L_ContactDate { get; set; }
        public string L_ContactCode { get; set; }
        public string L_Comment { get; set; }
        public string L_Directions { get; set; }
        public string L_Latitude { get; set; }
        public string L_Longitude { get; set; }
        public string L_Branch { get; set; }
        public string L_TaxCode { get; set; }
        public string L_Liaison1 { get; set; }
        public string L_Liaison2 { get; set; }
        public string SS_ServiceCode { get; set; }
        public string SS_Description { get; set; }
        public string SS_Quantity { get; set; }
        public string SS_UnitPrice { get; set; }
        public string SS_Schedule { get; set; }
        public string SS_WorkTime { get; set; }
        public string SS_TimeRange { get; set; }
        public string SS_Duration { get; set; }
        public string SS_StartDate { get; set; }
        public string SS_CancelDate { get; set; }
        public string SS_CancelReason { get; set; }
        public string SS_PriceIncrDate { get; set; }
        public string SS_ExpirationDate { get; set; }
        public string SS_PONumber { get; set; }
        public string SS_POExpirationDate { get; set; }
        public string SS_Source { get; set; }
        public string SS_Route { get; set; }
        public string SS_Division { get; set; }
        public string SS_Measurement { get; set; }
        public string SS_MeasurementType { get; set; }
        public string SS_CommissionStartDate { get; set; }
        public string SS_CommissionEndDate { get; set; }
        public string SS_Tech1 { get; set; }
        public string SS_Tech2 { get; set; }
        public string SS_Tech3 { get; set; }
        public string SS_Tech4 { get; set; }
        public string SS_Comment { get; set; }
        public string SS_NotificationDays { get; set; }
        public string SS_Terms { get; set; }
        public string SS_LastGeneratedDate { get; set; }
        public string SO_InitialServiceCode { get; set; }
        public string SO_Price { get; set; }
        public string SO_WorkDate { get; set; }
        public string SO_WorkTime { get; set; }
        public string SO_Duration { get; set; }
        public string SO_GLCode { get; set; }
        public string SO_Taxable { get; set; }
        public string SO_Tech1 { get; set; }
        public string SO_Route { get; set; }
        public string SO_Lock { get; set; }
    }
}
