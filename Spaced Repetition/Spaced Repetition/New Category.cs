using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spaced_Repetition
{
    public partial class New_Category : Form
    {
        public New_Category()
        {
            InitializeComponent();
        }

        public New_Category(string location) : this()
        {
            txtLocation.Text = location;
        }

        private void txtLocation_Enter(object sender, EventArgs e)
        {
            
        }

        private void New_Category_Load(object sender, EventArgs e)
        {

        }
    }
}
