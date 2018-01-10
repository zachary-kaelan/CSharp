using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeadByDaylightMatchLog
{
    public partial class frmLogMatch : Form
    {
        public const string ICONS_PATH = @"E:\Dead By Daylight\Icons\";

        public frmLogMatch()
        {
            InitializeComponent();
        }

        private void cboKiller_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboSurvivor.Enabled = false;
            picPortrait.ImageLocation = ICONS_PATH + @"Portraits - Cropped\" + cboKiller.Text + ".png";
        }

        private void cboSurvivor_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboKiller.Enabled = false;
            picPortrait.ImageLocation = ICONS_PATH + @"Portraits - Cropped\" + cboSurvivor.Text + ".png";
        }

        private void cboOfferings_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstOfferings.Items.Add(cboOfferings.Text);
            cboOfferings.Items.RemoveAt(cboOfferings.SelectedIndex);
        }

        private void lstOfferings_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            cboOfferings.Items.Add(lstOfferings.Text);
            lstOfferings.Items.RemoveAt(lstOfferings.SelectedIndex);
            
        }

        private void lstOfferings_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
