using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBDLib;
using ZachLib;
using DBDLootCharts.Properties;

namespace DBDLootCharts
{
    public partial class Form1 : Form
    {
        public const string MAIN_PATH = @"H:\Notes\Games\Dead By Daylight\Loot Charts\";

        private static readonly string[] LuckOfferings = new string[]
        {
            "None",
            "Slight",
            "Moderate",
            "Considerable"
        };

        private GroupBox current = null;

        public Form1()
        {
            InitializeComponent();
            this.MouseWheel += Form1_MouseWheel;

            Application.ApplicationExit += Application_ApplicationExit;
            if (!String.IsNullOrWhiteSpace(Settings.Default.FormState))
            {
                Loot loot = new Loot();
                loot.LoadCondensed(Settings.Default.FormState);
            }
            
            //var controls = grpAnteSlots.Controls;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {

        }

        private void btnAddOffering_Click(object sender, EventArgs e)
        {
            if (cboOfferings.Items.Contains(cboOfferings.Text))
            {
                if (lstOfferings.Items.Count > 3)
                    lstOfferings.Items.RemoveAt(0);
                if (cboOfferings.Text != "None")
                {
                    lstOfferings.Items.Add(cboOfferings.Text);
                    cboOfferings.ResetText();
                }
                else
                    lstOfferings.Items.Add("");
            }
        }

        private void btnLogItem_Click(object sender, EventArgs e)
        {
            if (TryGetLoot(out Loot loot))
            {
                File.AppendAllLines(@"C:\Users\ZACH-GAMING\AppData\Local\DBDLootCharts\LootLog.txt", new string[] { loot.Condense() });
                Clear();
            }
            else if (MessageBox.Show("A field is incomplete or incorrect. Would you like to clear the form?", "DBDLootCharts", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Clear();
        }

        private bool TryGetLoot(out Loot loot)
        {
            this.UseWaitCursor = true;
            loot = new Loot();
            try
            {
                loot.ItemType = (ItemType)grpItemType.GetChecked();
                loot.ItemRarity = (Rarity)grpItemRarity.GetChecked();
                loot.AceInTheHole = grpAceInTheHole.GetChecked();
                if (loot.AceInTheHole > 0)
                {
                    loot.AddonRarity = (Rarity)grpAddonRarity.GetChecked();
                    loot.Addon2Rarity = (Rarity)grpAddon2Rarity.GetChecked();
                }
                else
                {
                    loot.AddonRarity = Rarity.None;
                    loot.Addon2Rarity = Rarity.None;
                }
                loot.Plunderers = grpPlunderers.GetChecked();
                loot.Luck = 0;
                int ante = grpAnteSlots.GetChecked();
                for (int i = 1; i <= ante; ++i)
                {
                    switch(i)
                    {
                        case 1:
                            loot.Luck += grpAnteTier1.GetChecked();
                            break;

                        case 2:
                            loot.Luck += grpAnteTier2.GetChecked();
                            break;

                        case 3:
                            loot.Luck += grpAnteTier3.GetChecked();
                            break;

                        case 4:
                            loot.Luck += grpAnteTier4.GetChecked();
                            break;
                    }
                }
                loot.Luck *= grpAlive.GetChecked() - 1;
                loot.Luck += lstOfferings.Items.Cast<string>().Sum(o => Array.IndexOf(LuckOfferings, o));

                this.UseWaitCursor = false;
                return true;
            }
            catch
            {
                this.UseWaitCursor = false;
                return false;
            }

        }

        private void Clear()
        {
            radItemType6.Checked = true;
            radItemRarity1.Checked = true;
            radAddonRarity1.Checked = true;
            radAddon2Rarity0.Checked = true;
            /*radAnteTier10.Checked = true;
            radAnteTier20.Checked = true;
            radAnteTier30.Checked = true;
            radAnteTier40.Checked = true;*/
            //lstOfferings.Items.Clear();
            //cboOfferings.ResetText();
            Settings.Default.FormState = null;
        }

        private void btnResetForm_Click(object sender, EventArgs e)
        {
            Clear();
            lstOfferings.Items.Clear();
            cboOfferings.ResetText();
            radPlunderers0.Checked = true;
            radAce0.Checked = true;
            radAnteSlots0.Checked = true;
            radAlive4.Checked = true;
        }

        private void cboOfferings_Enter(object sender, EventArgs e)
        {
            Form1.ActiveForm.AcceptButton = btnAddOffering;
        }

        private void cboOfferings_Leave(object sender, EventArgs e)
        {
            Form1.ActiveForm.AcceptButton = btnLogItem;
        }

        private void grpAceInTheHole_CheckedChanged(object sender, EventArgs e)
        {
            var rad = ((RadioButton)sender).GetControlIndex();

            if (rad > 0)
            {
                grpAddonRarity.Visible = true;
                grpAddonRarity.Enabled = true;

                grpAddon2Rarity.Visible = true;
                grpAddon2Rarity.Enabled = true;
                radAddon2Rarity0.Checked = true;

                /*if (rad > 1)
                {
                    radAddonRarity3.Visible = true;
                    radAddonRarity3.Enabled = true;

                    if (rad == 3 && !radItemType6.Checked)
                    {
                        radAddonRarity4.Visible = true;
                        radAddonRarity4.Enabled = true;
                    }
                    else
                    {
                        radAddonRarity4.Visible = false;
                        radAddonRarity4.Enabled = false;
                    }
                }
                else
                {
                    radAddonRarity4.Visible = false;
                    radAddonRarity4.Enabled = false;
                    radAddonRarity3.Visible = false;
                    radAddonRarity3.Enabled = false;
                }*/
            }
            else
            {
                /*radAddonRarity4.Visible = false;
                radAddonRarity4.Enabled = false;
                radAddonRarity3.Visible = false;
                radAddonRarity3.Enabled = false;*/
                grpAddonRarity.Visible = false;
                grpAddonRarity.Enabled = false;
                grpAddon2Rarity.Visible = false;
                grpAddon2Rarity.Enabled = false;
            }

            radAddonRarity1.Checked = true;
        }

        private void grpAnteSlots_CheckedChanged(object sender, EventArgs e)
        {
            var rad = ((RadioButton)sender).GetControlIndex();
            //grpAlive.Check(Math.Min(rad + 1, 4).ToString()[0]);

            if (rad > 0)
            {
                grpAlive.Enabled = true;
                grpAlive.Visible = true;

                grpAnteTier1.Visible = true;
                grpAnteTier1.Enabled = true;

                if (rad > 1)
                {
                    grpAnteTier2.Visible = true;
                    grpAnteTier2.Enabled = true;

                    if (rad > 2)
                    {
                        grpAnteTier3.Visible = true;
                        grpAnteTier3.Enabled = true;

                        if (rad == 4)
                        {
                            grpAnteTier4.Visible = true;
                            grpAnteTier4.Enabled = true;
                        }
                        else if (grpAnteTier4.Enabled)
                        {
                            grpAnteTier4.Visible = false;
                            grpAnteTier4.Enabled = false;
                        }
                    }
                    else if (grpAnteTier3.Enabled)
                    {
                        grpAnteTier4.Visible = false;
                        grpAnteTier4.Enabled = false;
                        grpAnteTier3.Visible = false;
                        grpAnteTier3.Enabled = false;
                    }
                }
                else if (grpAnteTier2.Enabled)
                {
                    grpAnteTier4.Visible = false;
                    grpAnteTier4.Enabled = false;
                    grpAnteTier3.Visible = false;
                    grpAnteTier3.Enabled = false;
                    grpAnteTier2.Visible = false;
                    grpAnteTier2.Enabled = false;
                }
            }
            else if (grpAnteTier1.Enabled)
            {
                grpAnteTier4.Visible = false;
                grpAnteTier4.Enabled = false;
                grpAnteTier3.Visible = false;
                grpAnteTier3.Enabled = false;
                grpAnteTier2.Visible = false;
                grpAnteTier2.Enabled = false;
                grpAnteTier1.Visible = false;
                grpAnteTier1.Enabled = false;
                grpAlive.Enabled = false;
                grpAlive.Visible = false;
            }
        }

        private static readonly int[][] AVAILABLE_ITEM_RARITIES = new int[][] {
            new int[] { 1, 2, 3, 4, 5 },
            new int[] { 1, 3 },
            new int[] { 2, 3, 4 },
            new int[] { 3, 4, 5 },
            new int[] { 3, 5 },
            new int[] { 1, 2, 3, 4 },
            new int[] { 1, 2, 3, 4 }
        };

        private void grpAlive_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode.TryGetNumber(out char num) && num >= '1' && num <= '4')
                grpAlive.Check(num);
        }

        private void ZeroToThree_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode.TryGetNumber(out char num) && num >= '0' && num <= '3')
                ((GroupBox)sender).Check(num);
        }

        private void OneToFive_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode.TryGetNumber(out char num) && num >= '1' && num <= '5')
                ((GroupBox)sender).Check(num);
        }

        private void ZeroToFour_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode.TryGetNumber(out char num) && num >= '0' && num <= '4')
                ((GroupBox)sender).Check(num);
        }
        

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode.HasFlag(Keys.R))
                    btnResetForm.PerformClick();
                else if (e.KeyCode.HasFlag(Keys.L))
                    btnLogItem.PerformClick();
            }
            else if (current != null && e.KeyCode.TryGetNumber(out int num) && num >= 0 && num <= 5)
            {
                if (current.Name == "grpItemType")
                {
                    if (++num != 0)
                        ((RadioButton)current.Controls[num - 1]).Checked = true;
                }
                else
                {
                    var controls = current.Controls.Cast<Control>().OrderBy(c => c.GetControlIndex());
                    int firstIndex = controls.First().GetControlIndex();
                    if (firstIndex == 1)
                    {
                        if (num != 0 && num <= controls.Count())
                            ((RadioButton)controls.ElementAt(num - 1)).Checked = true;
                    }
                    else
                    {
                        if (num + 1 <= controls.Count())
                            ((RadioButton)controls.ElementAt(num)).Checked = true;
                    }
                }
            }
        }

        private void cboOfferings_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode.TryGetNumber(out int num) && num >= 0 && num <= 3)
            {
                lstOfferings.SelectedItem = LuckOfferings[num];
            }
        }

        private void GroupBox_MouseHover(object sender, EventArgs e)
        {
            var grp = (GroupBox)sender;

            if (grp.Visible)
                current = grp;
            else
                current = null;
        }

        private void GroupBox_Leave(object sender, EventArgs e)
        {
            current = null;
        }

        private void cboOfferings_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                int num = Convert.ToInt32(Char.GetNumericValue(e.KeyChar));
                if (num >= 0 && num <= 3)
                    lstOfferings.SelectedItem = LuckOfferings[num];
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'l')
                btnLogItem.PerformClick();
            else if (e.KeyChar == 'r')
                btnResetForm.PerformClick();
            else if (current != null && Char.IsDigit(e.KeyChar))
            {
                int num = Convert.ToInt32(Char.GetNumericValue(e.KeyChar));
                if (current.Name == "grpItemType")
                {
                    if (num != 0)
                        ((RadioButton)current.Controls[num - 1]).Checked = true;
                }
                else
                {
                    var controls = current.Controls.Cast<Control>().OrderBy(c => c.GetControlIndex());
                    int firstIndex = controls.First().GetControlIndex();
                    if (firstIndex == 1)
                    {
                        if (num != 0 && num <= controls.Count())
                            ((RadioButton)controls.ElementAt(num - 1)).Checked = true;
                    }
                    else if (num + 1 <= controls.Count())
                        ((RadioButton)controls.ElementAt(num)).Checked = true;
                }
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            int increase = e.Delta / 120;
            if (increase != 0)
            {
                if (current.Name == "grpItemType")
                {
                    int oldIndex = current.Controls.IndexOfKey("radItemType" + current.GetChecked().ToString());
                    int newIndex = Math.Max(0, Math.Min(4, oldIndex - increase));
                    if (newIndex != oldIndex)
                        ((RadioButton)current.Controls[newIndex]).Checked = true;
                }
                else
                {
                    var controls = current.Controls.OfType<RadioButton>().OrderBy(c => c.GetControlIndex()).ToList();
                    int oldIndex = controls.FindIndex(r => r.Checked);
                    int newIndex = Math.Max(0, Math.Min(controls.Count - 1, current.Name.Contains("AnteTier") || current.Name.Contains("Rarity") ? oldIndex - increase : oldIndex + increase));
                    if (newIndex != oldIndex)
                        controls[newIndex].Checked = true;
                }
            }
            
        }

        private void RadioButton_MouseHover(object sender, EventArgs e)
        {
            GroupBox grp = (GroupBox)((RadioButton)sender).Parent;
            if (grp.Visible)
                current = grp;
            else
                current = null;
        }

        private void btnAddOffering_Click_1(object sender, EventArgs e)
        {
            btnAddOffering_Click(sender, e);
        }
    }
}
