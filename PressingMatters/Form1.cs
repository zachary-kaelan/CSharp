using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InstagramLib;
using InstagramLib.API_Models;

namespace PressingMatters
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            splitMain.SplitterDistance = splitMain.Size.Width - 475;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            API.
        }
    }
}
