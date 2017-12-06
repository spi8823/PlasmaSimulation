using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static PlasmaSimulation.Extensions;

namespace PlasmaSimulation
{
    public class KatayamaGeometry : Geometry
    {
        private CylinderReflector Nozzle { get; }
        private CylinderReflector Reflector { get; }
        private Shield Shield { get; }
        private Shield Target { get; }
        private double Length { get; }

        public KatayamaGeometry(CylinderReflector nozzle, CylinderReflector reflector, Shield shield, Shield target, double length, int limit, Atom.ReflectionPattern pattern)
            :base(limit, pattern)
        {
            Nozzle = nozzle;
            Reflector = reflector;
            Shield = shield;
            Target = target;
            Length = length;
        }

        protected override Atom CreateAtomRandomly(Random random)
        {
            //位置
            var r = random.NextDouble();
            var theta = 2 * PI * random.NextDouble();

            var x = Nozzle.Radius * Pow(r, 0.5) * Cos(theta);
            var y = Nozzle.Radius * Pow(r, 0.5) * Sin(theta);
            var z = 0.0;

            var position = new Vector(x, y, z );

            //速度
            r = random.NextDouble();
            theta = 2 * PI * random.NextDouble();

            x = Sqrt(1 - r * r) * Cos(theta);
            y = Sqrt(1 - r * r) * Sin(theta);
            z = r;

            var velocity = new Vector(x, y, z);

            return new Atom(position, velocity);
        }

        protected override Vector? GetResult(Atom atom)
        {
            //ジオメトリ内の構造物の配列
            var structures = new Structure[] { Nozzle, Reflector, Shield, Target };

            //反射回数が上限に達するまで回す
            var count = 0;
            while (count++ < ReflectionLimit)
            {
                //構造物とAtomの衝突情報
                var collisions = from s
                                 in structures
                                 let c = s.GetCollision(atom)
                                 where c != null
                                 select c.Value;

                //衝突しなかったらどっかに行く
                if (!collisions.Any())
                {
                    Console.WriteLine("Far away");                    
                    return null;
                }
                var minTime = collisions.Min(c => c.Time);
                var collision = collisions.First(c => c.Time == minTime);

                //Targetと衝突したらおしまい
                if (collision.StructureID == Target.ID)
                {
                    if((collision.Position - Target.Position).Length() > 10)
                    {
                        var r = new Vector(atom.Position.X, atom.Position.Y, 0).Length();
                    }
                    return collision.Position;
                }

                //衝突する
                atom.Update(collision.Time);
                atom.ReflectSpecularly(collision.Normal);
            }

            Console.WriteLine("Too many");
            return null;
        }

        protected override Geometry Copy()
        {
            return new KatayamaGeometry(Nozzle, Reflector, Shield, Target, Length, ReflectionLimit, ReflectionPattern);
        }
    }
}
