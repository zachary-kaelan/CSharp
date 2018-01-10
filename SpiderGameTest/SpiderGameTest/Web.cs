using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderGameTest
{
    public class Web
    {
        private static Pen Silk = new Pen(Color.FloralWhite, 2);
        public Graphics gfxPallete { get; set; }
        private PointF Centre { get; set; }
        private Strand[] RadialThreads { get; set; }
        private WebSpiral Spiral { get; set; }


        public Web(Graphics gfx, PointF centre, WebSpiral spiral, params PointF[] strands)
        {
            this.gfxPallete = gfx;
            this.Centre = centre;
            this.Spiral = spiral;
            this.RadialThreads = strands.Select(s => new Strand(this.Centre, s)).ToArray();

            foreach(PointF strand in strands)
            {
                gfx.DrawLine(Silk, this.Centre, strand);
            }

            PointF lastPoint = new PointF(centre.X, centre.Y);
            float distancePerStrand = this.Spiral.LoopInterval / Convert.ToSingle(this.RadialThreads.Length);
            float length = distancePerStrand;
            for (int i = 0; i < this.Spiral.LoopCount; ++i)
            {
                for (int j = 0; j < this.RadialThreads.Length; ++j)
                {
                    PointF nextPoint = this.RadialThreads[j].PointToLength(length);
                    length += distancePerStrand;
                    gfx.DrawLine(Silk, lastPoint, nextPoint);
                    lastPoint = nextPoint;
                }
            }
        }

        public Web(Graphics gfx, PointF centre, WebSpiral spiral, int strandsCount)
        {
            PointF northPoint = new PointF(gfx.DpiX / 2, 0);
            PointF eastPoint = new PointF(gfx.DpiX, gfx.DpiY / 2);
            PointF southPoint = new PointF(gfx.DpiX / 2, gfx.DpiY);
            PointF westPoint = new PointF(0, gfx.DpiY / 2);

            List<PointF> strands = new List<PointF>();

            double angleInterval = 360.0 / (double)strandsCount;
            double targetAngle = 360.0 - (angleInterval / 2.0);
            for (double currentAngle = angleInterval / 2.0; currentAngle != targetAngle; currentAngle += angleInterval)
            {
                PointF point = new PointF();
                float x = 0;
                float y = 0;
                double adjustedAngle = currentAngle % 45.0;

                if (currentAngle < 45 || currentAngle >= 315)
                    x = gfx.DpiX;
                else if (currentAngle >= 135 && currentAngle < 225)
                    x = 0;
                else
                    x = ((float)Math.Tan(adjustedAngle) * gfx.DpiY);
            }


        }
    }

    public struct WebSpiral
    {
        public float AngleStart { get; set; }
        public float LoopInterval { get; set; }
        public float LoopCount { get; set; }

        public WebSpiral(float start, float interval, float count)
        {
            AngleStart = start;
            LoopInterval = interval;
            LoopCount = count;
        }

        public WebSpiral(float dpiX, float dpiY, int penLength)
        {
            AngleStart = 0;
            LoopInterval = (float)penLength * (float)2.0;
            LoopCount = (float)(Math.Min(dpiX, dpiY) / 2.0) / (LoopInterval + (float)penLength);
        }
    }

    public struct Strand
    {
        public PointF Start { get; set; }
        public PointF End { get; set; }
        public float Length { get; set; }
        public float Slope { get; set; }
        public float YIntercept { get; set; }
        public float Rise { get; set; }
        public float Run { get; set; }

        public Strand (PointF start, PointF end)
        {
            Start = start;
            End = end;
            Length = 0;
            Rise = (End.Y - Start.Y);
            Run = (End.X - Start.X);
            YIntercept = start.Y;
            Slope = Rise / Run;
            Length = LengthToPoint(end);
        }

        public float LengthToPoint (PointF intersectingPoint)
        {
            return Convert.ToSingle(
                Math.Sqrt(
                    Math.Pow(Math.Abs(intersectingPoint.X - Start.X), 2.0) +
                    Math.Pow(Math.Abs(intersectingPoint.Y - Start.Y), 2.0)
                )
            );
        }

        public bool PointOnLine(PointF intersectingPoint)
        {
            return Slope * intersectingPoint.X + YIntercept == intersectingPoint.Y;
        }

        public PointF PointToLength (float length, bool fromStart = true)
        {
            float lengthRatio = length / Length;
            return fromStart ?
                new PointF(
                    (lengthRatio * Run) + Start.X,
                    (lengthRatio * Rise) + Start.Y
                ) : new PointF(
                    End.X - (lengthRatio * Run),
                    End.Y - (lengthRatio * Rise)
                );
        }
    }
}
