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
using TinderAPI;
using TinderAPI.Models;

namespace TinderPlus
{
    public partial class Form1 : Form
    {
        public static readonly string PATH_TEMP = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Temp\Tinder\";
        public static readonly string PATH_TEMP_CURR_PHOTOS = PATH_TEMP + @"Current Photos\";

        public Image[] CurrentImages { get; private set; }
        public Photo[] CurrentPhotos { get; private set; }

        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists(PATH_TEMP))
                Directory.CreateDirectory(PATH_TEMP);
            if (!Directory.Exists(PATH_TEMP_CURR_PHOTOS))
                Directory.CreateDirectory(PATH_TEMP_CURR_PHOTOS);
        }

        private void lblBio_Click(object sender, EventArgs e)
        {

        }

        public void DisplayProfile(PublicProfile profile)
        {
            int photoIndex = 0;
            int photosCount = profile.Photos.Length;
            CurrentImages = new Image[photosCount];
            CurrentPhotos = profile.Photos;
            Directory.Delete(PATH_TEMP_CURR_PHOTOS);
            Directory.CreateDirectory(PATH_TEMP_CURR_PHOTOS);

            foreach(var photo in profile.Photos)
            {
                string path = PATH_TEMP_CURR_PHOTOS + String.Format("Photo{0}.jpg", photoIndex);
                
            }
            
        }
    }
}
