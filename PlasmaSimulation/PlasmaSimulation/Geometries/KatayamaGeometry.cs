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
            :base(limit, pattern, new Structure[] { nozzle, reflector, shield, target })
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

        protected override bool ShouldTerminate(Collision collision)
        {
            return collision.StructureID == Target.ID;
        }

        protected override Geometry Copy()
        {
            return new KatayamaGeometry(Nozzle, Reflector, Shield, Target, Length, ReflectionLimit, ReflectionPattern);
        }
    }
}
