using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace OpenCV
{
    class Program
    {
        static void Main(string[] args)
        {
            CvInvoke.NamedWindow("Test Window");
            Mat img = new Mat(200, 400, DepthType.Cv8U, 3);
            img.SetTo(new Bgr(255, 0, 0).MCvScalar);

            CvInvoke.PutText(
                img,
                "Hello, world!",
                new Point(10, 80),
                FontFace.HersheyComplex,
                1.0,
                new Bgr(0, 255, 0).MCvScalar
                );

            CvInvoke.Imshow("Test Window", img);
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyWindow("Test Window");
        }
    }
}
