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
        public int ReflectionCount { get; set; } = 0;

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
        public bool ReflectSpecularly(Vector normal)
        {
            ReflectionCount++;
            Velocity = Velocity - 2 * Dot(normal, Velocity) * normal;

            return true;
        }

        /// <summary>
        /// ランダム反射後の速度にする
        /// </summary>
        /// <param name="normal">反射面法線</param>
        public bool ReflectRandomly(Vector normal)
        {
            ReflectionCount++;
            throw new NotImplementedException();
        }

        public bool Reflect(Vector normal, ReflectionPattern pattern)
        {
            switch (pattern)
            {
                case ReflectionPattern.Specularly:
                    return ReflectSpecularly(normal);
                case ReflectionPattern.Randomly:
                    return ReflectRandomly(normal);
                default:
                    return ReflectSpecularly(normal);
            }
        }

        public enum ReflectionPattern
        {
            Specularly = 0, Randomly = 1
        }
    }
}
