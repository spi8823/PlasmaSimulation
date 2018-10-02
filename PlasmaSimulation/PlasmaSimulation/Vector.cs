using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasmaSimulation.Extensions;
using static System.Math;

namespace PlasmaSimulation
{
    public struct Vector
    {
        public static readonly Vector Right = new Vector(1, 0, 0);
        public static readonly Vector Left = new Vector(-1, 0, 0);
        public static readonly Vector Up = new Vector(0, 1, 0);
        public static readonly Vector Down = new Vector(0, -1, 0);
        public static readonly Vector Forward = new Vector(0, 0, 1);
        public static readonly Vector Back = new Vector(0, 0, -1);

        public static readonly Vector Zero = new Vector(0, 0, 0);
        public static readonly Vector One = new Vector(1, 1, 1);

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Round()
        {
            double round(double d)
            {
                var n = Math.Round(d);
                if (Abs(n - d) < RoundingValue)
                    return n;
                else return d;
            };

            X = round(X);
            Y = round(Y);
            Z = round(Z);
        }

        [Newtonsoft.Json.JsonIgnore()]
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        [Newtonsoft.Json.JsonIgnore()]
        public Vector Normal => this / Length;

        [Newtonsoft.Json.JsonIgnore()]
        public System.Windows.Media.Media3D.Vector3D Vector3D => new System.Windows.Media.Media3D.Vector3D(X, Y, Z);

        public static Vector operator+(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator*(Vector a, double d)
        {
            return new Vector(a.X * d, a.Y * d, a.Z * d);
        }

        public static Vector operator/(Vector a, double d)
        {
            return a * (1 / d);
        }

        public static Vector operator*(double d, Vector a)
        {
            return a * d;
        }

        public static Vector operator-(Vector a, Vector b)
        {
            return a + (-1 * b);
        }

        public static Vector GetRandomCosineDistributionVector(Random random)
        {
            var r = random.NextDouble();
            var theta = Acos(r);
            r = random.NextDouble();
            var phi = 2 * PI * r;

            var x = Cos(phi) * Sin(theta);
            var y = Sin(phi) * Sin(theta);
            var z = Cos(theta);
            var v = new Vector(x, y, z);

            return v;
        }

        public static Vector GetSpecularlyReflectionVector(Vector v, Vector normal)
        {
            return v - 2 * Dot(normal, v) * normal;
        }
    }

    public struct Rotation
    {
        public Vector Normal { get; set; }
        public double Theta { get; set; }

        public Rotation(Vector n, double theta)
        {
            Normal = n;
            Theta = theta;
        }

        public Rotation(Vector from, Vector to)
        {
            Normal = Cross(from, to).Normal;
            Theta = Acos(Dot(from, to) / (from.Length * to.Length));
        }

        public Vector Rotate(Vector v)
        {
            var sin = Sin(Theta);
            var cos = Cos(Theta);

            var parallel = Dot(v, Normal) * Normal;
            var orthogonal = v - parallel;

            var result = parallel + cos * orthogonal + sin * Cross(Normal, orthogonal);
            result.Round();
            return result;
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
