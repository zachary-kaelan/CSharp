using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZachLib
{
    public static class GUIExtensions
    {
        public static bool TryGetPath(this FileDialog dialog, out string path)
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.FileName;
                dialog.Dispose();
                return true;
            }
            else
            {
                dialog.Dispose();
                path = null;
                return false;
            }
        }

        public static bool TryGetPath(this FolderBrowserDialog dialog, out string path)
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;
                dialog.Dispose();
                return true;
            }
            else
            {
                dialog.Dispose();
                path = null;
                return false;
            }
        }
    }
}
