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

namespace DataImportUtility
{
    class DataImportClient
    {
        public const string MainPath = @"E:\Data Import\";
        public const string ExportsPath = MainPath + @"Exports\";
        public const string FileFormatPath = MainPath + @"FileFormatSkeleton.txt";
        public List<string> fields = File.ReadAllText(FileFormatPath).Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //public Group[] groups { get; set; }

        public List<FileFormatSkeleton> records { get; set; }
        public List<Dictionary<string, string>> recordDict { get; set; }

        private CsvConfiguration cfg { get; set; }
        private DefaultCsvClassMap<FileFormatSkeleton> fileFormatMap { get; set; }
        private readonly Dictionary<string, string> copy = FileFormatSkeleton.ReturnTemplate();
        private Dictionary<string, FieldInfo> ffsFields { get; set; }
        //private static TypedReference ffsRef = 
        public Lookup<string, Lookup<string, string>> csvMapping { get; set; }

        public DataImportClient()
        {
            fields.InsertRange(
                fields.IndexOf(
                    fields.Last(
                        f => f.StartsWith("L.")
                    )
                ) + 1, Enumerable.Range(1, 31).Select(
                    n => "L.UserDef" + n.ToString()
                )
            );
            fields.RemoveRange(fields.IndexOf("L.UserDef14"), 10);
            fields.Remove("L.UserDef4");
            fields.Remove("L.UserDef6");
            fields.Remove("L.UserDef7");
            fields.Remove("L.UserDef9");

            recordDict = new List<Dictionary<string, string>>();
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

            var grps = fields.GroupBy(f => f.Substring(0, f.IndexOf('.')));
            csvMapping = (
                Lookup<string, Lookup<string, string>>
                )grps.ToLookup(
                    f => f.Key, f => (
                    Lookup<string, string>
                    )f.ToLookup(
                        g => g.Remove(
                            0, g.IndexOf('.') + 1)));

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

            ffsFields = ffs.GetRuntimeFields()/*ffs.GetFields(BindingFlags.SetProperty)*/.ToDictionary(f => f.Name, f => f);

            var fieldNames = fields.Select(f => new KeyValuePair<string, string>(f, f.Replace('.', '_')));
            foreach (var field in fieldNames)
            {
                try
                {
                    var propInfo = ffs.GetProperty(field.Value);
                    var newMap = new CsvPropertyMap(propInfo);
                    newMap.Name(field.Key);
                    fileFormatMap.PropertyMaps.Add(newMap);
                }
                catch
                {
                    break;
                }
            }

            //cfg.RegisterClassMap(fileFormatMap);

            /*FileStream fs = new FileStream(
                @"C:\DocUploads\logs\Export.txt",
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.Read
            );
            StreamWriter sw = new StreamWriter(fs);
            CsvWriter writer = new CsvWriter(sw, cfg);*/
            
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
            /*FileFormatSkeleton record = new FileFormatSkeleton();
            //TypedReference ffsRef = TypedReference.MakeTypedReference(record, ffsFields.Values.ToArray());
            foreach (var field in ffsFields)
            {
                field.Value.SetValue(
                    record,
                    rec.TryGetValue(
                        field.Key,
                        out string prop
                    ) ? prop : ""
                );
            }

            records.Add(record);*/

            Dictionary<string, string> record = FileFormatSkeleton.ReturnTemplate();
            foreach(string field in rec.Keys)
            {
                record[field] = rec[field];
            }

            recordDict.Add(record);
        }

        public void Export(string fileName)
        {
            FileStream fs = new FileStream(ExportsPath + fileName + ".txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);

            /*sw.WriteLine(String.Join("\t", copy.Keys));
            foreach(var record in recordDict)
            {
                sw.WriteLine(String.Join("\t", record.Values));
            }*/

            CsvWriter writer = new CsvWriter(sw, cfg);
            writer.Configuration.Delimiter = "\t";
            //var classmap = cfg.AutoMap<Dictionary<string, string>>();
            //cfg.DetectColumnCountChanges = true;
            //cfg.HasHeaderRecord = false;
            //cfg.IgnorePrivateAccessor = true;
            

            /*
            foreach(string header in copy.Keys)
            {
                writer.WriteHeader<string>();
            }
            writer.NextRecord();
            */
            foreach(var header in fields)
            {
                writer.WriteField(header);
            }
            writer.NextRecord();

            foreach (var record in recordDict)
            {
                foreach(var value in record.Values)
                {
                    writer.WriteField(value);
                }
                writer.NextRecord();
            }

            //writer.WriteRecords(recordDict);
            //writer.Dispose();
            sw.Close();
            fs.Close();
        }

        public KeyValuePair<string, List<string>>[] LoadFile(Stream file)
        {
            StreamReader sr = new StreamReader(file);
            CsvReader reader = new CsvReader(sr, cfg);
            reader.Configuration.Delimiter = ",";

            reader.Read();//Header();
            string[] headers = reader.FieldHeaders;
            int headerCount = headers.Length;

            var lookup = headers.Select(
                h => new KeyValuePair<string, List<string>>(
                    h, new List<string>()
                )
            ).ToArray();

            while (reader.Read())
            {
                for (int i = 0; i < headerCount; ++i)
                {
                    lookup[i].Value.Add(reader.GetField(i));
                }
            }

            return lookup;

            /*int count = 0;
            return (Lookup<string, string>)
                lookup*//*.Select(
                h => new KeyValuePair<KeyValuePair<string,List<string>.Enumerator>, List<string>>( 
                    new KeyValuePair<string, List<string>.Enumerator>(
                        h.Key, h.Value.GetEnumerator()),
                    h.Value
                    ))*//*.ToLookup(
                h => h.Key,
                h => h.Value.ElementAt(count++)
            );*/
        }
    }

    public struct Iterator
    {
        int index { get; set; }
        List<string> list { get; set; }

        public Iterator(List<string> lst, int i = 0)
        {
            index = i;
            list = lst;
        }

        public string GetNext()
        {
            string output = list[index];
            index += 1;
            return output;
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

        public static Dictionary<string, string> ReturnTemplate()
        {
            return new Dictionary<string, string>()
            {
                {"L_LocationCode", "" },
                {"L_BillToCode", "" },
                {"L_Company", "" },
                {"L_LName", "" },
                {"L_FName", "" },
                {"L_Title", "" },
                {"L_Address", "" },
                {"L_Address2", "" },
                {"L_Zip", "" },
                {"L_City", "" },
                {"L_State", "" },
                {"L_Salutation", "" },
                {"L_SalutationName", "" },
                {"L_Phone", "" },
                {"L_PhoneExt", "" },
                {"L_AltPhone", "" },
                {"L_AltPhoneExt", "" },
                {"L_Fax", "" },
                {"L_FaxExt", "" },
                {"L_Mobile", "" },
                {"L_MobileExt", "" },
                {"L_EMail", "" },
                {"L_URL", "" },
                {"L_MapCode", "" },
                {"L_Division", "" },
                {"L_Type", "" },
                {"L_Builder", "" },
                {"L_Source", "" },
                {"L_County", "" },
                {"L_Subdivision", "" },
                {"L_ContactDate", "" },
                {"L_ContactCode", "" },
                {"L_Comment", "" },
                {"L_Directions", "" },
                {"L_Latitude", "" },
                {"L_Longitude", "" },
                {"L_Branch", "" },
                {"L_TaxCode", "" },
                {"L_Liaison1", "" },
                {"L_UserDef1", "" },
                {"L_UserDef2", "" },
                {"L_UserDef3", "" },
                {"L_UserDef5", "" },
                {"L_UserDef8", "" },
                {"L_UserDef10", "" },
                {"L_UserDef11", "" },
                {"L_UserDef12", "" },
                {"L_UserDef13", "" },
                {"L_UserDef24", "" },
                {"L_UserDef25", "" },
                {"L_UserDef26", "" },
                {"L_UserDef27", "" },
                {"L_UserDef28", "" },
                {"L_UserDef29", "" },
                {"L_UserDef30", "" },
                {"L_UserDef31", "" },
                {"SS_ServiceCode", "" },
                {"SS_Description", "" },
                {"SS_Quantity", "" },
                {"SS_UnitPrice", "" },
                {"SS_Schedule", "" },
                {"SS_WorkTime", "" },
                {"SS_TimeRange", "" },
                {"SS_Duration", "" },
                {"SS_StartDate", "" },
                {"SS_CancelDate", "" },
                {"SS_CancelReason", "" },
                {"SS_PriceIncrDate", "" },
                {"SS_ExpirationDate", "" },
                {"SS_PONumber", "" },
                {"SS_POExpirationDate", "" },
                {"SS_Source", "" },
                {"SS_Route", "" },
                {"SS_Division", "" },
                {"SS_Measurement", "" },
                {"SS_MeasurementType", "" },
                {"SS_CommissionStartDate", "" },
                {"SS_CommissionEndDate", "" },
                {"SS_Tech1", "" },
                {"SS_Tech2", "" },
                {"SS_Tech3", "" },
                {"SS_Tech4", "" },
                {"SS_Comment", "" },
                {"SS_NotificationDays", "" },
                {"SS_Terms", "" },
                {"SS_LastGeneratedDate", "" },
                {"SO_InitialServiceCode", "" },
                {"SO_Price", "" },
                {"SO_WorkDate", "" },
                {"SO_WorkTime", "" },
                {"SO_Duration", "" },
                {"SO_GLCode", "" },
                {"SO_Taxable", "" },
                {"SO_Tech1", "" },
                {"SO_Route", "" },
                {"SO_Lock", "" }
            };
        }
    }
}

