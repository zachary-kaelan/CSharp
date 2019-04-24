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
using DatamuseLib;
using DatamuseLib.Models;
using DatamuseLib.Models.Songs;
using ParodyWriter.Properties;
using ZachLib;

namespace ParodyWriter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string PATH_MAIN { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Settings.Default.TempPath))
            {
                PATH_MAIN = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Appdata\Local\Temp\";
                Settings.Default.TempPath = PATH_MAIN;
                Settings.Default.Save();
            }
            else
                PATH_MAIN = Settings.Default.TempPath;
        }
    }
}
