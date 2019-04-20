using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static PlasmaSimulation.Extensions;

namespace PlasmaSimulation
{
    public class Atom
    {
        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public int History { get; set; } = 0;
        public bool IsValid { get; set; }
        public int ReflectionCount { get; set; } = 0;

        public Atom() { }

        public Atom(Vector position, Vector velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        /// <summary>
        /// 指定された時間分進ませる
        /// </summary>
        /// <param name="t">進ませる時間</param>
        public void Update(double t)
        {
            Position += Velocity * t;
        }

        /// <summary>
        /// 鏡面反射後の速度にする
        /// </summary>
        /// <param name="normal">反射面法線</param>
        public void ReflectSpecularly(Vector normal)
        {
            ReflectionCount++;
            Velocity = Vector.GetSpecularlyReflectionVector(Velocity, normal);
        }

        /// <summary>
        /// ランダム反射後の速度にする
        /// </summary>
        /// <param name="normal">反射面法線</param>
        public void ReflectRandomly(Vector normal, Random random)
        {
            ReflectionCount++;

            var r = random.NextDouble();
            var theta = 2 * PI * random.NextDouble();

            var x = Sqrt(1 - r * r) * Cos(theta);
            var y = Sqrt(1 - r * r) * Sin(theta);
            var z = r;
            var v = new Vector(x, y, z);
            var rotation = new Rotation(Vector.Forward, normal);
            Velocity = rotation.Rotate(v);
        }

        public void ReflectCosineRandomly(Vector normal, Random random)
        {
            ReflectionCount++;

            var v = Vector.GetRandomCosineDistributionVector(random);
            Velocity = new Rotation(Vector.Forward, normal).Rotate(v);
        }

        public void ReflectCosineSpecularly(Vector normal, Random random)
        {
            ReflectionCount++;
            while (true)
            {
                var rotation = new Rotation(Vector.Forward, Vector.GetSpecularlyReflectionVector(Velocity, normal));
                var v = rotation.Rotate(Vector.GetRandomCosineDistributionVector(random));
                if (Dot(v, normal) > 0)
                {
                    Velocity = v;
                    return;
                }
            }
        }

        public void Reflect(Vector normal, ReflectionPattern pattern, Random random)
        {
            switch (pattern)
            {
                case ReflectionPattern.Specularly:
                    ReflectSpecularly(normal);
                    break;
                case ReflectionPattern.Randomly:
                    ReflectRandomly(normal, random);
                    break;
                case ReflectionPattern.CosineRandomly:
                    ReflectCosineRandomly(normal, random);
                    break;
                case ReflectionPattern.CosineSpecularly:
                    ReflectCosineSpecularly(normal, random);
                    break;
                default:
                    ReflectSpecularly(normal);
                    break;
            }
        }

        public enum ReflectionPattern
        {
            Specularly = 0, Randomly = 1, CosineRandomly = 2, CosineSpecularly = 3
        }
    }
}
