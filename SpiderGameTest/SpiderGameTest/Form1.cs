using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Configuration;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderGameTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Graphics gfx = null;
        public Web web = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            gfx = this.CreateGraphics();
            web = new Web(gfx, new PointF(gfx.DpiX / 2, gfx.DpiY / 2), new WebSpiral(gfx.DpiX, gfx.DpiY, penLength: 2), 5);
        }
    }
}
