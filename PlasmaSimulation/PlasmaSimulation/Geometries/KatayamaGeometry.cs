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
        public CylinderReflector Nozzle
        {
            get { return (CylinderReflector)Structures[0]; }
            set { Structures[0] = value; }
        }

        public CylinderReflector Reflector
        {
            get { return (CylinderReflector)Structures[1]; }
            set { Structures[1] = value; }
        }

        public Shield Shield
        {
            get { return (Shield)Structures[2]; }
            set { Structures[2] = value; }
        }

        public Shield Target
        {
            get { return (Shield)Structures[3]; }
            set { Structures[3] = value; }
        }

        public KatayamaGeometry(CylinderReflector nozzle, CylinderReflector reflector, Shield shield, Shield target, int limit, double reflectionCoefficient, Atom.ReflectionPattern pattern)
            : base(limit, reflectionCoefficient, pattern, new Structure[4])
        {
            Nozzle = nozzle;
            Reflector = reflector;
            Shield = shield;
            Target = target;
        }

        public override Atom CreateAtomRandomly(Random random)
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
            return new KatayamaGeometry(Nozzle, Reflector, Shield, Target, ReflectionLimit, ReflectionCoefficient, ReflectionPattern);
        }
    }
}
