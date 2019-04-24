using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderGameTest
{
    [Flags]
    public enum TriangleDescriptors
    {
        None,
        Acute = 1,
        Equilateral = 2,
        Isoscoles = 4,
        Obtuse = 8,
        Right = 16
    }

    public struct Triangle
    {
        public PointF PointA { get; private set; }
        public PointF PointB { get; private set; }
        public PointF PointC { get; private set; }
        public Angle AngleA { get; private set; }
        public Angle AngleB { get; private set; }
        public Angle AngleC { get; private set; }
        public double SideA { get; private set; }
        public double SideB { get; private set; }
        public double SideC { get; private set; }
        public double Area { get; private set; }
        public double CircumcircleDiameter { get; private set; }
        public TriangleDescriptors Descriptors { get; private set; }

        /*[Flags]
        private enum Sides
        {
            None,
            SideA = 1,
            SideB = 2,
            SideC = 4
        };

        [Flags]
        private enum Angles
        {
            None,
            AngleA = 1,
            AngleB = 2,
            AngleC = 4
        };

        public Triangle(
            Tuple<double, double, double> sides, 
            Tuple<Angle, Angle, Angle> angles, 
            TriangleDescriptors descriptors
        ) {
            Sides sidesUnavailable = Sides.None;
            Angles anglesUnavailable = Angles.None;
            int numSidesUnavailable = 0;
            int numAnglesUnavailable = 0;
            if (sides.Item1 <= 0)
            {
                sidesUnavailable |= Sides.SideA;
                ++numSidesUnavailable;
            }
            if (sides.Item2 <= 0)
            {
                sidesUnavailable |= Sides.SideB;
                ++numSidesUnavailable;
            }
            if (sides.Item3 <= 0)
            {
                sidesUnavailable |= Sides.SideC;
                ++numSidesUnavailable;
            }

            if (angles.Item1.IsNull)
            {
                anglesUnavailable |= Angles.AngleA;
                ++numAnglesUnavailable;
            }
            if (angles.Item2.IsNull)
            {
                anglesUnavailable |= Angles.AngleB;
                ++numAnglesUnavailable;
            }
            if (angles.Item3.IsNull)
            {
                anglesUnavailable |= Angles.AngleC;
                ++numAnglesUnavailable;
            }

            if (numSidesUnavailable == 0)
            {
                if (numAnglesUnavailable == )
            }
        }*/

        public Triangle(double side1, double side2, double angle3, TriangleDescriptors descriptors) : 
            this(
                side1, side2,
                descriptors == TriangleDescriptors.Right ?
                    Math.Sqrt((side1 * side1) + (side2 * side2)) :
                    LawOfCosines(side1, side2, angle3),
                angle3,
                descriptors
            )
        { }

        public Triangle(double side1, double side2, double side3) : this(side1, side2, side3, LawOfCosines(side1, side2, side3, false))
        { }

        public Triangle(double side1, double side2, double side3, double angle3, TriangleDescriptors descriptors = TriangleDescriptors.None) : this()
        {
            Descriptors = descriptors;
            (AngleA, AngleB) = LawOfSines(SideC, AngleC, SideA, SideB, out double d);
            CircumcircleDiameter = d;
            Initialize();
        }

        public Triangle(double side1, double side2, double side3, double angle1, double angle2, double angle3) : this()
        {
            SideA = side1;
            SideB = side2;
            SideC = side3;
            AngleA = angle1;
            AngleB = angle2;
            AngleC = angle3;
            Initialize();
        }

        public Triangle(PointF point1, PointF point2, PointF point3) : 
            this(
                PythagoreonTheorm(point1.X, point1.Y),
                PythagoreonTheorm(point2.X, point2.Y),
                PythagoreonTheorm(point3.X, point3.Y)
            )
        {
            PointA = point1;
            PointB = point2;
            PointC = point3;
        }

        private void Initialize()
        {
            if (Descriptors == TriangleDescriptors.None)
            {
                bool acute = true;
                foreach (var angle in new Angle[] { AngleA, AngleB, AngleC })
                {
                    if (angle.Degrees > 90)
                    {
                        acute = false;
                        Descriptors |= TriangleDescriptors.Obtuse;
                    }
                    else if (angle.Degrees == 90)
                    {
                        acute = false;
                        Descriptors |= TriangleDescriptors.Right;
                    }
                }
                if (acute)
                    Descriptors |= TriangleDescriptors.Acute;

                if (AngleA.Degrees == AngleB.Degrees || AngleB.Degrees == AngleC.Degrees || AngleC.Degrees == AngleA.Degrees)
                {
                    if (AngleA.Degrees == AngleB.Degrees && AngleB.Degrees == AngleC.Degrees)
                        Descriptors |= TriangleDescriptors.Equilateral;
                    else
                        Descriptors |= TriangleDescriptors.Isoscoles;
                }
            }

            Area = GetArea(SideA, SideB, SideC);
            CircumcircleDiameter = SideA / Math.Sin(AngleA.Radians);
        }

        #region TriangleFormulas
        public static double PythagoreonTheorm(double side1, double side2, bool side3Known = false)
        {
            if (side1 == 0 || side2 == 0)
                throw new IncompleteInformationException(
                    new Dictionary<string, object>()
                    {
                        { "Side1", side1 },
                        { side3Known ? "Side3" : "Side2", side2 }
                    }
                );
            return side3Known?
                Math.Sqrt(Math.Abs((side2 * side2) - (side1 * side1))) :
                Math.Sqrt((side1 * side1) + (side2 * side2));
        }

        #region LawOfSines
        public static double LawOfSines(double side1, double angle1, double sideOrAngle2, bool angleGiven = false) => LawOfSines(side1, angle1, sideOrAngle2, out _, angleGiven);

        private const double ACUTE_CUTOFF = Math.PI / 2.0;
        public static double LawOfSines(double side1, double angle1, double sideOrAngle2, out double sinFactor, bool angleGiven = false)
        {
            if (side1 == 0 || angle1 == 0 || sideOrAngle2 == 0)
                throw new IncompleteInformationException(
                    new Dictionary<string, object>()
                    {
                        { "Side1", side1 },
                        { "Angle11", angle1 },
                        { angleGiven ? "Angle2" : "Side2", sideOrAngle2 }
                    }
                );

            var sinA = Math.Sin(angle1);
            sinFactor = side1 / sinA;
            if (!angleGiven)
            {
                var angle2 = Math.Asin(sideOrAngle2 / sinFactor);
                if (angle2 <= angle1)
                    return angle2;
                else
                    throw new IncompleteInformationException(
                        new Dictionary<string, object>()
                        {
                            { "Side1", side1 },
                            { "Angle1", angle1 },
                            { "Angle2", angle2 },
                            { "Angle2Alt", (2.0 * Math.PI) - angle2 }
                        }
                    );
            }
            else
                return sinFactor * Math.Sin(sideOrAngle2);
        }

        public static (double, double) LawOfSines(double side1, double angle1, double sideOrAngle2, double sideOrAngle3, bool anglesGiven = false) => LawOfSines(side1, angle1, sideOrAngle2, sideOrAngle3, out _, anglesGiven);

        public static (double, double) LawOfSines(double side1, double angle1, double sideOrAngle2, double sideOrAngle3, out double sinFactor, bool anglesGiven = false)
        {
            if (side1 == 0 || angle1 == 0 || sideOrAngle2 == 0 || sideOrAngle3 == 0)
                throw new IncompleteInformationException(
                    new Dictionary<string, object>()
                    {
                        { "Side1", side1 },
                        { "Angle11", angle1 },
                        { anglesGiven ? "Angle2" : "Side2", sideOrAngle2},
                        { anglesGiven ? "Angle3" : "Side3", sideOrAngle3 }
                    }
                );

            var sinA = Math.Sin(angle1);
            sinFactor = side1 / sinA;
            if (!anglesGiven)
            {
                var angle2 = Math.Asin(sideOrAngle2 / sinFactor);
                var angle3 = Math.Asin(sideOrAngle3 / sinFactor);
                if (angle2 <= angle1 && angle3 <= angle1)
                    return (angle2, angle3);
                else
                    throw new IncompleteInformationException(
                        new Dictionary<string, object>()
                        {
                            { "Side1", side1 },
                            { "Angle1", angle1 },
                            { "Angle2", angle2 },
                            { "Angle2Alt", (2.0 * Math.PI) - angle2 },
                            { "Angle3", angle3 },
                            { "Angle3Alt", (2.0 * Math.PI) - angle3 }
                        }
                    );
            }
            else
                return (
                    sinFactor * Math.Sin(sideOrAngle2), 
                    sinFactor * Math.Sin(sideOrAngle3)
                );
        }
        #endregion

        public static double LawOfCosines(double side1, double side2, double sideOrAngle3, bool angleGiven = true)
        {
            if (side1 == 0 || side2 == 0 || sideOrAngle3 == 0)
                throw new IncompleteInformationException(
                    new Dictionary<string, object>()
                    {
                        { "Side1", side1 },
                        { "Side2", side2 },
                        { angleGiven ? "Angle3" : "Side3", sideOrAngle3 }
                    }
                );
            var cosFactor = 2 * side1 * side2;
            if (side1 == side2)
                return angleGiven ?
                    Math.Sqrt(
                        cosFactor * (1 - Math.Cos(sideOrAngle3))
                    ) : Math.Acos(
                        1.0 - ((sideOrAngle3 * sideOrAngle3) / cosFactor)
                    );

            var pythagoreonFactor = (side1 * side1) + (side2 * side2);
            return angleGiven ?
                Math.Sqrt(
                    pythagoreonFactor - (cosFactor * Math.Cos(sideOrAngle3))
                ) : Math.Acos(
                    (pythagoreonFactor - (sideOrAngle3 * sideOrAngle3)) / cosFactor
                );
        }

        public static double GetArea(double triBase, double triHeight)
        {
            return triBase * triHeight * 0.5;
        }

        public static double GetArea(double side1, double side2, double side3)
        {
            var S = (side1 + side2 + side3) / 2;
            return Math.Sqrt(
                S * 
                (S - side1) *
                (S - side2) *
                (S - side3)
            );
        }
        #endregion
    }

    public class IncompleteInformationException : Exception
    {
        public IncompleteInformationException() : base("Incomplete information required for formula.") { }
        public IncompleteInformationException(Dictionary<string, object> data) : this()
        {
            foreach(var kv in data)
            {
                Data.Add(kv.Key, kv.Value);
            }
        }
    }

    public struct Angle
    {
        public double Radians { get; private set; }
        public double Degrees { get; private set; }
        public bool IsNull { get; private set; }

        public Angle(double angle, bool isRadians = true)
        {
            if (isRadians)
            {
                Radians = angle;
                Degrees = (angle * 180.0) / Math.PI;
            }
            else
            {
                Degrees = angle;
                Radians = (angle * Math.PI) / 180.0;
            }
            IsNull = false;
        }

        public override string ToString() => Degrees.ToString("#.00°");

        public string ToString(string format) => Degrees.ToString(format);

        public static implicit operator double(Angle angle) => angle.Radians;
        public static implicit operator Angle(double angle) => new Angle(angle);

        public static readonly Angle NULL = new Angle() { Degrees = -1, Radians = -1, IsNull = true };
    }
}
