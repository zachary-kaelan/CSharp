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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void cxtCategories_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void treCategories_AfterCheck(object sender, TreeViewEventArgs e)
        {
            e.Node.Nodes.Cast<TreeNode>().ToList().ForEach(n => n.Checked = e.Node.Checked);
        }

        private void mnuExpandAll_Click(object sender, EventArgs e)
        {
            treCategories.ExpandAll();
        }

        private void mnuCollapseAll_Click(object sender, EventArgs e)
        {
            treCategories.CollapseAll();
        }

        private void mnuAddCategory_Click(object sender, EventArgs e)
        {
            New_Category frmNewCategory = new New_Category();
            
        }
    }
}
