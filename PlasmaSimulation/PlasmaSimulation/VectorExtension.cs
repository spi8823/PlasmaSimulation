using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation
{
    public struct Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static Vector operator+(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator*(Vector a, double d)
        {
            return new Vector(a.X * d, a.Y * d, a.Z * d);
        }

        public static Vector operator*(double d, Vector a)
        {
            return a * d;
        }

        public static Vector operator-(Vector a, Vector b)
        {
            return a + (-1 * b);
        }
    }

    public static class Extensions
    {
        public static Vector Cross(Vector a, Vector b)
        {
            var x = a.Y * b.Z - a.Z * b.Y;
            var y = a.Z * b.X - a.X * b.Z;
            var z = a.X * b.Y - a.Y * b.X;

            return new Vector(x, y, z);
        }

        public static double Dot(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public const double RoundingValue = 0.00000001;
    }
}
