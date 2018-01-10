using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Jil;
using System.Text;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;
using PPOffline.Properties;

namespace PPOffline
{
    public partial class Form1 : Form
    {
        public Lookup<string, Dictionary<string, string>> customers = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //var stream = Resources.ResourceManager.GetStream("LookupTable");
            //customers = (Lookup<string, Dictionary<string, string>>)(new BinaryFormatter().Deserialize(stream));
            try
            {
                using (var ms = new MemoryStream())
                {
                    byte[] data = (byte[])Resources.ResourceManager.GetObject("LookupTableBIN");
                    ms.Write(data, 0, data.Length);
                    customers = Jil.JSON.Deserialize<Lookup<string, Dictionary<string, string>>>((string)(new BinaryFormatter().Deserialize(ms)));
                    ms.Dispose();
                    ms.Close();
                }
            }
            catch
            {
                List<string> lines = new List<string>(File.ReadAllLines(@"C:\Users\ZACH-GAMING\Documents\InsightLocations.txt"));
                string[] labels = lines[0].Split('\t');
                lines.RemoveAt(0);

                customers = (Lookup<string, Dictionary<string, string>>)lines.ToLookup(
                    l => l.Split('\t')[5],
                    l => l.Split('\t').Select(
                        (x, i) => new { Item = x, Index = i }
                        ).ToDictionary(
                        k => labels[k.Index], k => k.Item
                    )
                );

                using (var ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(
                        ms,
                        Jil.JSON.Serialize<Lookup<string, Dictionary<string, string>>>(
                            customers
                        )
                    );
                    var data = ms.ToArray();
                    ResourceWriter rw = new ResourceWriter("Resources.LookupTableBIN");
                    rw.AddResourceData("LookupTableBIN", "ByteArray", data);
                    rw.Generate();
                    rw.Dispose();
                    rw.Close();
                    //ms.Read((byte[])Resources.LookupTableBIN.SyncRoot, 0, data.Length);
                    ms.Dispose();
                    ms.Close();
                }
            }
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            
        }
    }
}
