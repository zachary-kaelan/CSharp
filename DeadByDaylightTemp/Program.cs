using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightTemp
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] portraits = Directory.GetFiles(@"E:\Dead By Daylight\Icons\Portraits");
            Rectangle cropRect = new Rectangle(45, 0, 164, 243);
            Rectangle targetRect = new Rectangle(0, 0, 164, 243);

            foreach(string portrait in portraits)
            {
                using (Bitmap src = Image.FromFile(portrait, true) as Bitmap)
                {
                    using (Bitmap target = new Bitmap(cropRect.Width, cropRect.Height))
                    {
                        using (Graphics gfx = Graphics.FromImage(target))
                        {
                            gfx.DrawImage(
                                src,
                                targetRect,
                                cropRect,
                                GraphicsUnit.Pixel
                            );
                        }
                        target.Save(@"E:\Dead By Daylight\Icons\Portraits - Copy\" + Path.GetFileName(portrait));
                    }
                }
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
