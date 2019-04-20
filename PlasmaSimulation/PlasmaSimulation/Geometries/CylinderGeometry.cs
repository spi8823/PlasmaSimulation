using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace PlasmaSimulation
{
    public class CylinderGeometry : Geometry
    {
        public readonly double Time;
        public readonly double Interval;
        public readonly double Flux;
        public readonly double Resolution;
        public readonly double DefaultReflectionCoefficient = 0.1;
        public readonly double AtomCrossSection = 1 / Pow(10, 15);
        public readonly int Count;

        public double[] Distribution { get; private set; }

        public CylinderReflector Cylinder
        {
            get { return (CylinderReflector)Structures[0]; }
            set { Structures[0] = value; }
        }

        public Shield Bottom
        {
            get { return (Shield)Structures[1]; }
            set { Structures[1] = value; }
        }

        public CylinderGeometry(CylinderReflector cylinder, Shield bottom, double[] distribution, double time, double interval, double flux, double resolution, int count, int limit, Atom.ReflectionPattern pattern)
            : base(limit, 1, pattern, new Structure[2])
        {
            Cylinder = cylinder;
            Bottom = bottom;

            Time = time;
            Interval = interval;
            Flux = flux;
            Count = count;
            Resolution = resolution;
            if (distribution != null)
                Distribution = distribution;
            else
                Distribution = new double[(int)(Cylinder.Length / Resolution)];
        }

        public override Atom CreateAtomRandomly(Random random)
        {
            //位置
            var r = random.NextDouble();
            var theta = 2 * PI * random.NextDouble();

            var x = Cylinder.Radius * Pow(r, 0.5) * Cos(theta);
            var y = Cylinder.Radius * Pow(r, 0.5) * Sin(theta);
            var z = Cylinder.Position.Z;

            var position = new Vector(x, y, z);

            //速度
            r = random.NextDouble();
            theta = 2 * PI * random.NextDouble();

            x = Sqrt(1 - r * r) * Cos(theta);
            y = Sqrt(1 - r * r) * Sin(theta);
            z = r;

            var velocity = new Vector(x, y, z);

            return new Atom(position, velocity);
        }

        /// <summary>
        /// ジオメトリの中でAtomに対して一連の処理をした結果を返す
        /// </summary>
        /// <param name="atom"></param>
        /// <returns>処理結果</returns>
        protected override bool GetResult(Atom atom, Random random)
        {
            //反射回数が上限に達するまで回す
            var count = 0;
            while (count++ < ReflectionLimit)
            {
                Collision collision = null;
                for (var i = 0; i < Structures.Length; i++)
                {
                    Structures[i].SetCollision(atom);
                    var c = Structures[i].Collision;
                    if (!c.IsValid || double.IsNaN(c.Time))
                        continue;
                    if (collision == null)
                    {
                        collision = c;
                        continue;
                    }
                    if (c.Time < collision.Time)
                    {
                        collision = c;
                    }
                }
                if (collision == null)
                {
                    atom.IsValid = false;
                    return false;
                }

                atom.Update(collision.Time);
                atom.Reflect(collision.Normal, ReflectionPattern, random);
                
                if (OnCollision(atom, collision))
                {
                    atom.IsValid = true;
                    return true;
                }
                if (random.NextDouble() > GetReflectionCoefficient(collision.Position.Z))
                {
                    atom.IsValid = true;
                    return true;
                }
            }
            return false;
        }

        public double GetReflectionCoefficient(double z)
        {
            var index = z / Resolution;
            if (index > Distribution.Length)
                return 0;

            return DefaultReflectionCoefficient + Distribution[(int)index] * AtomCrossSection;
        }

        public List<double[]> GetDistribution()
        {
            var time = 0.0;
            var fluence = Flux * Interval * PI * Pow(Cylinder.Radius / 10, 2);

            var result = new List<double[]>();
            result.Add(Distribution.ToArray());

            while(time < Time)
            {
                foreach (var atom in ProcessAsParallel(Count))
                {
                    if(!atom.IsValid)
                        continue;
                    var p = atom.Position;

                    var index = p.Z / Resolution;
                    if (index > Distribution.Length - 1)
                        continue;

                    Distribution[(int)index] += 1 * (fluence / Count) / (Resolution * 2 * PI * Cylinder.Radius);
                }
                result.Add(Distribution.ToArray());
                time += Interval;
            }

            return result;
        }

        protected override bool OnCollision(Atom atom, Collision collision)
        {
            return collision.StructureID == Bottom.ID;
        }

        protected override Geometry Copy()
        {
            return new CylinderGeometry((CylinderReflector)Cylinder.Copy(), (Shield)Bottom.Copy(), Distribution.ToArray(), Time, Interval, Flux, Resolution, Count, ReflectionLimit, ReflectionPattern);
        }
    }
}
