using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

using Microsoft.VisualBasic;

namespace DataImportUtility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private readonly Dictionary<string, string> tooltips = new Dictionary<string, string>()
        {
            {"L", "Locations"},
            {"SS", "Service Setups"},
            {"SO", "Service Orders"}
        };
        private KeyValuePair<string, List<string>>[] records;
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private DataImportClient client = new DataImportClient();
        //private SoundPlayer player = new SoundPlayer();

        private void Form1_Load(object sender, EventArgs e)
        {
            //lstLinked.Items.AddRange(new string[] { "", "", "" });

            var mnu = new MenuItem[] {
                new MenuItem("Collapse All", new EventHandler((arg0, arg1) => trvLookup.CollapseAll())),
                new MenuItem("Expand All", new EventHandler((arg0, arg1) => trvLookup.ExpandAll()))
            };

            trvLookup.BeforeCheck += TrvLookup_BeforeCheck;

            var nodes = trvLookup.Nodes;
            foreach(var map in client.csvMapping)
            {
                var curNode = nodes.Add(map.Key);
                curNode.ContextMenu = new ContextMenu(mnu);
                curNode.ToolTipText = tooltips[map.Key];
                var curNodes = curNode.Nodes;

                foreach(var group in map)
                {
                    foreach(var subgroup in group)
                    {
                        var curNode2 = curNodes.Add(map.Key + "_" + subgroup.Key, subgroup.Key);
                        curNode2.ToolTipText = map.Key + "_" + subgroup.Key;
                        //mnu.MenuItems[0].Name = map.Key + "_" + subgroup.Key;
                        curNode2.ContextMenu = new ContextMenu(
                            new MenuItem[] {
                                new MenuItem(
                                    "Link with selected CSV Header", 
                                    new EventHandler(mnu_OnClick)
                                ) {Name = map.Key + "_" + subgroup.Key}
                            }.Concat(mnu).ToArray());
                    }
                }
            }
        }

        private void TrvLookup_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            var node = e.Node;
            var nodes = node.Nodes;
            if (nodes.Count > 0)
            {
                foreach (var n in nodes)
                {
                    TreeNode _node = (TreeNode)n;
                    _node.Checked = !node.Checked;
                }
            }
            else if (!node.Checked)
                lstLinked.Items.Add(node.Parent.Text + "_" + node.Text);//AddHeader(node.Text, node.Parent.Text + "_" + node.Text);
            else
                lstLinked.Items.Remove(node.Parent.Text + "_" + node.Text);//headers.Remove(node.Text);
        }

        private void AddHeader(string key, string value)
        {
            if (!headers.TryGetValue(key, out _))
                headers.Add(key, value);
            else
                headers[key] = value;

            int index = lstMapping.Items.IndexOf(key);
            while (lstLinked.Items.Count < index)
                lstLinked.Items.Add("");
            lstLinked.Items.Insert(
                index, 
                value);
        }

        private void mnu_OnClick(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            /*string key = Interaction.InputBox(
                    "What is the header's name in the csv file?",
                    "Date Import Utility",
                    item.Name.Replace('_', '.')
                );
            AddHeader(key, item.Name);*/
            AddHeader(lstMapping.SelectedItem.ToString(), item.Name);

            //item.Checked = !item.Checked;
            //Debug.WriteLine(item.Name);
        }

        private void trvLookup_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void ofdInput_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (ofdInput.ShowDialog() == DialogResult.OK)
            {
                records = client.LoadFile(
                    ofdInput.OpenFile());
                lstMapping.Items.AddRange(
                    records.Select(
                        r => r.Key
                    ).ToArray()
                );

                /*foreach(var record in records)
                {
                    lstMapping.Items.Add(record.Key);

                    //string key = null;*/
                    /*
                    if (!headers.TryGetValue(record.Key, out key))
                        continue;
                    TreeNode node = trvLookup.Nodes[key];
                    node.Nodes.AddRange(
                        record.Select(
                            r => new TreeNode(r) /*{
                                ToolTipText = key.Replace(
                                    '_', '.'
                                ) + "_" + r }*/
                        /*).ToArray()
                    );*/
                //}
            }
        }

        private void lstMapping_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lstLinked_DragOver(object sender, DragEventArgs e)
        {
            
        }

        private void lstLinked_KeyUp(object sender, KeyEventArgs e)
        {
            /*
            try
            {
                int index = lstLinked.SelectedIndex;
                if (e.KeyCode == Keys.Up)
                {
                    lstLinked.SelectedIndex++;
                    if (e.Modifiers == Keys.Shift)
                    {
                        if (index == 0)
                            lstLinked.Items.Insert(1, "");
                        else
                        {
                            ++index;
                            string item = lstLinked.Items[index].ToString();
                            lstLinked.Items.RemoveAt(index);
                            lstLinked.Items.Insert(index - 1, item);
                            lstLinked.SelectedIndex = index - 1;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    lstLinked.SelectedIndex--;
                    if (e.Modifiers == Keys.Shift)
                    {
                        string item = lstLinked.Items[index].ToString();
                        if (index + 1 == lstLinked.Items.Count)
                        {
                            lstLinked.Items.Add("");
                            lstLinked.Items.RemoveAt(index);
                            lstLinked.Items.Add(item);
                            lstLinked.SelectedIndex = index + 1;
                        }
                        else
                            lstLinked.Items.Insert(--index, item);
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (e.Modifiers == Keys.Shift)
                        lstLinked.Items.Clear();
                    else
                    {
                        lstLinked.Items.RemoveAt(index);
                        if (lstLinked.Items.Count > 0)
                            lstLinked.SelectedIndex = index;
                    }
                }
            }
            catch
            {
                SystemSounds.Exclamation.Play();
            }*/
            
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstMapping.Items.Count; ++i)
            {
                if (lstLinked.Items[i].ToString() == "")
                    continue;
                headers.Add(
                    lstMapping.Items[i].ToString(),
                    lstLinked.Items[i].ToString()
                );
            }

            int count = records[0].Value.Count;
            var keys = headers.Keys.Select(
                (k, n) => new { Key = k, Index = n }
            ).ToArray();
            int headerCount = keys.Length;
            var items = keys.Select(k =>
                records.Single(kv => kv.Key == k.Key).Value.ToArray()
            ).ToArray();

            for (int i = 0; i < count; ++i)
            {
                client.AddRecord(
                    keys.ToDictionary(
                        kn => headers[kn.Key],
                        kn => items[kn.Index][i]
                    )
                );
                /*
                for(int j = 0; j < headerCount; ++j)
                {
                    record.Add(
                        headers[keys[j]],
                        items[j][i]
                    );
                }*/
            }

            client.Export("Export");
        }
    }

    public class NavListBox : ListBox
    {
        protected override void OnKeyUp(KeyEventArgs e)
        {
            e.Handled = true;
            try
            {
                if (this.SelectedItem == null || this.SelectedIndex < 0)
                    return;

                int index = this.SelectedIndex;
                if (e.KeyCode == Keys.Up)
                    --index;
                else if (e.KeyCode == Keys.Down)
                    ++index;
                else if (e.KeyCode == Keys.Delete)
                {
                    if (e.Shift)
                        this.Items.Clear();
                    else
                    {
                        this.Items.RemoveAt(index);
                        this.SetSelected(index, true);
                    }
                    return;    
                }

                if (index < 0)
                    return;

                if (index >= this.Items.Count)
                    this.Items.Add("");

                if (e.Shift)
                {
                    object item = this.SelectedItem;
                    this.Items.Remove(item);
                    this.Items.Insert(index, item);
                }

                this.SetSelected(index, true);
            }
            catch (Exception err)
            {
                SystemSounds.Exclamation.Play();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }

}
