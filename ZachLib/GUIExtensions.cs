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
        #region TryGetPath
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
#endregion

        public static bool TryGetNumber(this Keys key, out int number)
        {
            if((key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                number = Convert.ToInt32(Char.GetNumericValue(key.ToString().Last()));
                return true;
            }
            number = -1;
            return false;
        }

        public static bool TryGetNumber(this Keys key, out char number)
        {
            if ((key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                number = key.ToString().Last();
                return true;
            }
            number = Char.MinValue;
            return false;
        }

        public static void Check(this GroupBox grp, char num)
        {
            var rad = grp.Controls.OfType<RadioButton>().Single(r => r.Name.Last() == num);
            if (rad.Enabled)
                rad.Checked = true;
        }

        public static int GetChecked(this GroupBox grp)
        {
            return grp.Controls.OfType<RadioButton>().Single(r => r.Checked).GetControlIndex();
        }

        public static int GetControlIndex(this Control control)
        {
            return Convert.ToInt32(Char.GetNumericValue(control.Name.Last()));
        }
    }
}
