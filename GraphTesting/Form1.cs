using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Graphics gfx = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            gfx = picDisplay.CreateGraphics();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            Pen pen = new Pen(Color.Red);

            gfx.TranslateTransform(picDisplay.Size.Width / 2f, picDisplay.Size.Height / 2f);
            gfx.FillEllipse(Brushes.Black, -1.5f, -1.5f, 3f, 3f);
            gfx.DrawArc(pen, 0f, -32f, 64f, 64f, 180f, 60f);

            string str = "0111110";
            float x = 0;
            float y = 0;
            var rads = Convert.ToSingle(60.0 * (Math.PI / 180.0));
            var sin = Convert.ToSingle(Math.Sin(rads));
            var cos = Convert.ToSingle(1.0 - Math.Cos(rads));
            RectangleF ellipseCW = new RectangleF(0f, -32f, 64f, 64f);
            RectangleF ellipseCCW = new RectangleF(-64f, -32f, 64f, 64f);
            char lstChar = ' ';
            float angle = 0;

            //string str = Convert.ToString(i, 2).PadLeft(m, '0');
            for (int j = 0; j < str.Length; ++j)
            {
                if (str[j] == '1')
                {
                    y += sin;
                    x += cos;
                }
                else
                {
                    x += sin;
                    y += cos;
                }

                gfx.DrawArc(pen, ,)
            }
        }
    }
}
