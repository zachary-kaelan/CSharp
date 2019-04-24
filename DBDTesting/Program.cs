using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using ZachLib;

namespace DBDTesting
{
    class Program
    {
        public static SortedDictionary<byte, PixelTreeNode> PointTree = new SortedDictionary<byte, PixelTreeNode>();
        public static List<Bitmap> Images = null;
        public static int ONotationMax = 0;
        public static string[] files = null;

        public const int PERK_PX = 16;

        static void Main(string[] args)
        {
            float min = 200;
            float minPerc = 0;
            for (float i = 0; i <= 100; ++i)
            {
                // person1 heals person2 to i
                float person1 = i / 2;
                float person2 = i;

                // person2 heals person1 to full
                var temp = Math.Min((100 - person1) / 2, 100 - person2);
                person2 += temp;
                
                float total = i + (100 - person1) + (100 - person2);
                if (total < min)
                {
                    min = total;
                    minPerc = i;
                }
                Console.WriteLine("Heal to {0}%, ending with person2 at {1}%, with {2}% total.", i, person2, total);
            }
            Console.WriteLine();
            Console.WriteLine("Heal to {0}%; {1}% total.", minPerc, min);
            Console.ReadLine();

            //GenProgression();

            //SAM.Picker.Program.Main();
            //SAM.Game.Program.Main(new string[] { "381210" });
            DBDStatsViewer statsViewer = new DBDStatsViewer();
            statsViewer.GetGameStats();

            while (true)
            {
                string line = Console.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                    return;
                statsViewer.RefreshGameStats();
            }

            files = Directory.GetFiles(@"E:\Programming\Dead By Daylight\Icons\Perks - Scaled");
            Images = files.Select(
                f => new Bitmap(Image.FromFile(f))
            ).ToList();

            //ScaleImages();
            //BuildPointTree();
            PointTree = new SortedDictionary<byte, PixelTreeNode>(Utils.LoadJSON<SortedDictionary<byte, PixelTreeNode>>("PointTree.txt"));
            var testImage = new Bitmap(Image.FromFile(@"E:\Programming\Dead By Daylight\Programming Stuff\Perk.png"));
            MakeGrayScale(testImage).Save(@"E:\Programming\Dead By Daylight\Programming Stuff\Binary.png");
            TestPointTree(testImage);

            /*CropImages();

            FileStream stream = File.Open(@"E:\Programming\Dead By Daylight\Icons\Perks\iconPerks_adrenaline.png", FileMode.Open, FileAccess.Read, FileShare.None);
            var bitmapDecoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = bitmapDecoder.Frames[0];
            ImageMetadata metadata = bitmapSource.Metadata;
            BitmapMetadata bitmapMetadata = (BitmapMetadata)bitmapSource.Metadata;*/

            //BuildPointTree();

            //var files = Directory.GetFiles(@"E:\Programming\Dead By Daylight\Icons\Perks - Cropped");

            Bitmap src = new Bitmap(Image.FromFile(@"E:\Programming\Dead By Daylight\Programming Stuff\Table.png"));
            int cropX = 120;
            int checkBorderX = 107;
            var borderColor = Color.FromArgb(16, 16, 24);
            for (int i = 0; i < 4; ++i)
            {
                var borderPixel = src.GetPixel(checkBorderX, 66);
                if (borderPixel != borderColor)
                    break;
                Bitmap target = new Bitmap(PERK_PX, PERK_PX);
                Graphics gfx = Graphics.FromImage(target);
                gfx.DrawImage(src, new Rectangle(0, 0, PERK_PX, PERK_PX), new Rectangle(cropX, 56, PERK_PX, PERK_PX), GraphicsUnit.Pixel);
                target.Save(@"E:\Programming\Dead By Daylight\Programming Stuff\Perk" + i.ToString() + ".png");
                gfx.Dispose();

                cropX += 56;
                checkBorderX += 56;
            }

            //gfx.DrawImage(src, new Rectangle(0, 0, 204, 204), );
            src.Dispose();

            /*src = new Bitmap(Image.FromFile(@"E:\Programming\Dead By Daylight\Icons\Perks - Cropped\iconPerks_adrenaline.png"));
            var otherTarget = new Bitmap(PERK_PX, PERK_PX);
            gfx = Graphics.FromImage(otherTarget);
            gfx.DrawImage(src, new Rectangle(0, 0, PERK_PX, PERK_PX), new Rectangle(0, 0, 102, 102), GraphicsUnit.Pixel);

            otherTarget.Save(@"E:\Programming\Dead By Daylight\Programming Stuff\Original.png");
            gfx.Dispose();
            otherTarget.Dispose();
            src.Dispose();*/

            Console.WriteLine("FINISHED");
            Console.ReadLine();

            //Bitmap target = new Bitmap(Image.FromFile(@"E:\Programming\Dead By Daylight\Programming Stuff\Perk.png"));

            //MakeGrayScale(target);

            

            

            /*for (int i = 255; i != 0; --i)
            {
                if (img.GetPixel(128, i).A != 0)
                {
                    Console.WriteLine(i);
                    Console.WriteLine(img.GetPixel(128, i));
                    break;
                }
            }
            img.Dispose();
            Console.ReadLine();*/

            Console.WriteLine(
                Encoding.UTF8.GetString(
                    Convert.FromBase64String(
                        Console.ReadLine()
                    )
                )
            );
            Console.ReadLine();

            for (double i = 0; i < 52; ++i)
            {
                double escapeChance = 1.0 - (0.04 + (i / 100));
                Console.WriteLine(
                    String.Join(
                        "\t",
                        Enumerable.Range(0, 4).Select(
                            j => (1.0 - Math.Pow(
                                escapeChance,
                                3.0 + Convert.ToDouble(j)
                            )).ToString("##.0%")
                        )
                    )
                );

                if ((i + 1) % 13 == 0)
                    Console.WriteLine(" - - - ");
            }

            Console.ReadLine();
        }

        public static void CropScreenshot()
        {
            var screenshot = new Bitmap(Image.FromFile(@"E:\Programming\Dead By Daylight\Programming Stuff\Killer Postgame Screen\20190107153529_1.jpg"));
            var table = new Bitmap(828, 550);
            var gfx = Graphics.FromImage(table);
            gfx.DrawImage(screenshot, new Rectangle(0, 0, 828, 550), new Rectangle(82, 256, 828, 550), GraphicsUnit.Pixel);
            table.Save(@"E:\Programming\Dead By Daylight\Programming Stuff\Table.png");
            gfx.Dispose();
            table.Dispose();
            screenshot.Dispose();
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        public static void ScaleImages()
        {
            //var files = Directory.GetFiles(@"E:\Programming\Dead By Daylight\Icons\Perks - Cropped");
            var srcRect = new Rectangle(0, 0, 102, 102);
            var newRect = new Rectangle(0, 0, PERK_PX, PERK_PX);
            foreach (var file in files)
            {
                Bitmap img = new Bitmap(Image.FromFile(file));

                var newImg = new Bitmap(PERK_PX, PERK_PX);
                Graphics gfx = Graphics.FromImage(newImg);
                gfx.DrawImage(img, newRect, srcRect, GraphicsUnit.Pixel);
                newImg.Save(@"E:\Programming\Dead By Daylight\Icons\Perks - Scaled\" + Path.GetFileName(file));
                newImg.Dispose();
                gfx.Dispose();
                img.Dispose();
            }
        }

        public static void CropImages()
        {
            var files = Directory.GetFiles(@"E:\Programming\Dead By Daylight\Icons\Perks");
            var cropRect = new Rectangle(26 + 51, 26 + 51, 204 - 51, 204 - 51);
            var newRect = new Rectangle(0, 0, 204, 204);
            foreach (var file in files)
            {
                Bitmap img = new Bitmap(Image.FromFile(file));

                var newImg = new Bitmap(204 - 102, 204 - 102);
                Graphics gfx = Graphics.FromImage(newImg);
                gfx.DrawImage(img, newRect, cropRect, GraphicsUnit.Pixel);
                newImg.Save(@"E:\Programming\Dead By Daylight\Icons\Perks - Cropped\" + Path.GetFileName(file));
                newImg.Dispose();
                gfx.Dispose();
                img.Dispose();
            }
        }

        public static void TestPointTree(Bitmap image)
        {
            byte next = 112;
            while (!PointTree[next].GetNext(image, out next)) { }
            Console.WriteLine(Path.GetFileNameWithoutExtension(files[next]));
            Console.ReadLine();
        }

        public static void BuildPointTree()
        {
            byte rootID = GetNext(Images);
            Console.WriteLine("FINISHED\r\n");
            Console.WriteLine(rootID);
            Console.WriteLine(ONotationMax);
            Console.WriteLine(PointTree.Count);
            PointTree.SaveAs("PointTree.txt");
            Console.ReadLine();
        }

        public static Bitmap MakeGrayScale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            //Bitmap newBitmapAvg = new Bitmap(original.Width, original.Height);
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            var gray65 = Color.FromArgb(166, 166, 166);
            var white = Color.FromArgb(100, Color.White);
            for (int x = 0; x < PERK_PX; ++x)
            {
                for (int y = 0; y < PERK_PX; ++y)
                {
                    Color color = original.GetPixel(x, y);

                    if (color.R < 128 || color.G < 128 || color.B < 128)
                        newBitmap.SetPixel(x, y, gray65);
                    else
                        newBitmap.SetPixel(x, y, white);

                    /*float[] distance = new float[3];
                    distance[0] = Math.Abs(color.R - color.G);
                    distance[1] = Math.Abs(color.G - color.B);
                    distance[2] = Math.Abs(color.B - color.R);

                    float max = distance.Max() / 256f;
                    float min = distance.Min() / 256f;
                    float avg = distance.Average() / 256f;

                    float averagePercent = 1f - avg;
                    float percent = 1f - ((max - min) / 2);
                    if (averagePercent > )
                    newBitmapAvg.SetPixel(x, y, Color.FromArgb((int)(255 * avg), Color.White));
                    newBitmap.SetPixel(x, y, Color.FromArgb((int)(255 * (1f - percent)), Color.White));*/
                }
            }

            newBitmap.Save(@"E:\Programming\Dead By Daylight\Programming Stuff\Binary.png");

            //newBitmapAvg.Save(@"E:\Programming\Dead By Daylight\Programming Stuff\AveragePercent.png");
            //newBitmap.Save(@"E:\Programming\Dead By Daylight\Programming Stuff\Percent.png");

            Console.WriteLine("FINISHED");
            Console.ReadLine();

            //create the grayscale ColorMatrix
            /*ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {0, 0, 0, .3f, 0},
                 new float[] {0, 0, 0, .59f, 0},
                 new float[] {0, 0, 0, .11f, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, -1, 1}
               });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();*/
            return newBitmap;
        }

        public static void HexGenProgression()
        {
            float skillCheckChance = 0;
            float hexSuccessChance = 0;
            float genProgressSeconds = 0;
            int totalSeconds = 0;
            //int noProgressSeconds = 0;
            while (genProgressSeconds < 80)
            {
                ++totalSeconds;
                /*if (noProgressSeconds > 0)
                    --noProgressSeconds;
                else*/
                    ++genProgressSeconds;
                skillCheckChance += 0.08f;
                if (skillCheckChance >= 1)
                {
                    hexSuccessChance += 0.2f;
                    --skillCheckChance;
                    if (hexSuccessChance >= 1)
                        --hexSuccessChance;
                    else
                    {
                        genProgressSeconds = Math.Max(0, genProgressSeconds - 4);
                        //noProgressSeconds = 3;
                        //totalSeconds += 3;
                    }
                }
            }
            Console.WriteLine(skillCheckChance);
            Console.WriteLine(genProgressSeconds);
            Console.WriteLine(totalSeconds);
            Console.ReadLine();
        }

        public static void GenProgression()
        {
            for (int i = 1; i <= 4; ++i)
            {
                float chargesPerSecond = (1f - (0.1f * (i - 1))) * i;

                float skillCheckChance = 0;
                float genProgressSeconds = 0;
                int totalSeconds = 0;
                while (genProgressSeconds < 80)
                {
                    ++totalSeconds;
                    genProgressSeconds += chargesPerSecond;
                    skillCheckChance += 0.08f;
                    if (skillCheckChance >= 1)
                    {
                        --skillCheckChance;
                        genProgressSeconds += 1.6f * i;
                    }
                }
                //Console.WriteLine(skillCheckChance);
                Console.WriteLine(totalSeconds);
            }

            Console.ReadLine();
        }

        public static byte GetNext(IEnumerable<Bitmap> images, int depth = 1, byte startingIndex = 0)
        {
            /*if (depth == 7 && startingIndex == 35)
            {
                var colors = images.Select(img => img.GetPixel(100, 100));
            }*/

            int bestScore = Int32.MaxValue;
            Point bestScorePoint = new Point(1, 1);

            for (int x = 0; x < PERK_PX; ++x)
            {
                for (int y = 0; y < PERK_PX; ++y)
                {
                    int yes = 0;
                    int no = 0;
                    foreach (var image in images)
                    {
                        var pixel = image.GetPixel(x, y);
                        if (pixel.A == 0)
                            ++no;
                        else
                            ++yes;
                    }
                    int score = Math.Abs(yes - no);
                    if (score < bestScore && yes > 0 && no > 0)
                    {
                        bestScore = score;
                        bestScorePoint = new Point(x, y);
                    }
                }
            }

            bool useAlpha = true;

            List<Bitmap> yeses = new List<Bitmap>();
            List<Bitmap> nos = new List<Bitmap>();

            if (bestScore == Int32.MaxValue)
            {
                useAlpha = false;
                for (int x = 0; x < PERK_PX; ++x)
                {
                    for (int y = 0; y < PERK_PX; ++y)
                    {
                        int yes = 0;
                        int no = 0;
                        foreach (var image in images)
                        {
                            var pixel = image.GetPixel(x, y);
                            if (pixel.R != 0)
                                ++no;
                            else
                                ++yes;
                        }
                        int score = Math.Abs(yes - no);
                        if (score < bestScore && yes > 0 && no > 0)
                        {
                            bestScore = score;
                            bestScorePoint = new Point(x, y);
                        }
                    }
                }

                foreach (var image in images)
                {
                    if (image.GetPixel(bestScorePoint.X, bestScorePoint.Y).R != 0)
                        nos.Add(image);
                    else
                        yeses.Add(image);
                }
            }
            else
            {
                foreach (var image in images)
                {
                    if (image.GetPixel(bestScorePoint.X, bestScorePoint.Y).A == 0)
                        nos.Add(image);
                    else
                        yeses.Add(image);
                }
            }

            if (bestScore == Int32.MaxValue)
            {
                int index = 0;
                foreach(var image in images)
                {
                    image.Save(@"E:\Programming\Dead By Daylight\Icons\Problem Icons\Problem" + index.ToString() + ".png");
                    ++index;
                }
            }
            Debug.Assert(bestScore != Int32.MaxValue);

            Console.WriteLine(bestScorePoint.ToString() + " - " + bestScore.ToString());

            YesNoFinality finality = YesNoFinality.None;
            byte yesID = 0;
            byte noID = 0;

            if (nos.Count == 1)
            {
                finality |= YesNoFinality.NoFinal;
                noID = (byte)Images.IndexOf(nos[0]);
                if (depth > ONotationMax)
                    ONotationMax = depth;
            }
            else
                noID = GetNext(nos, depth + 1, startingIndex);
            if (yeses.Count == 1)
            {
                finality |= YesNoFinality.YesFinal;
                yesID = (byte)Images.IndexOf(yeses[0]);
                if (depth > ONotationMax)
                    ONotationMax = depth;
            }
            else
                yesID = GetNext(yeses, depth + 1, (byte)(startingIndex + nos.Count));

            var node = new PixelTreeNode(
                (byte)bestScorePoint.X,
                (byte)bestScorePoint.Y,
                yesID,
                noID,
                finality,
                useAlpha
            );
            PointTree.Add(node.ID, node);
            return node.ID;
        }

        /*public float sign(PointF p1, PointF p2, PointF p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        bool PointInTriangle(PointF pt, PointF v1, PointF v2, PointF v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }*/
    }

    [Flags]
    public enum YesNoFinality
    {
        None,
        NoFinal,
        YesFinal
    }

    public struct PixelTreeNode
    {
        private static byte ID_COUNTER = 0;
        public byte ID { get; private set; }
        public byte X { get; private set; }
        public byte Y { get; private set; }
        public byte IfYes { get; private set; }
        public byte IfNo { get; private set; }
        public YesNoFinality Finality { get; private set; }
        public bool UseAlpha { get; private set; }

        public PixelTreeNode(byte x, byte y, byte ifYes, byte ifNo, YesNoFinality finality, bool useAlpha = true)
        {
            ID = ID_COUNTER;
            ++ID_COUNTER;
            X = x;
            Y = y;
            IfYes = ifYes;
            IfNo = ifNo;
            Finality = finality;
            UseAlpha = useAlpha;
        }

        public bool GetNext(Bitmap bitmap, out byte next)
        {
            bool no = UseAlpha ? bitmap.GetPixel(X, Y).A == 0 : bitmap.GetPixel(X, Y).R != 0;
            next = no ? IfNo : IfYes;
            if (Finality != YesNoFinality.None)
            {
                if (no)
                {
                    if (Finality.HasFlag(YesNoFinality.NoFinal))
                        return true;
                }
                else
                {
                    if (Finality.HasFlag(YesNoFinality.YesFinal))
                        return true;
                }
            }

            return false;
        }                
    }
}
