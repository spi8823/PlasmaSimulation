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
            get { return (CylinderReflector)Structures[3]; }
            set { Structures[3] = value; }
        }

        public CylinderReflector Reflector
        {
            get { return (CylinderReflector)Structures[4]; }
            set { Structures[4] = value; }
        }

        public Shield Shield
        {
            get { return (Shield)Structures[5]; }
            set { Structures[5] = value; }
        }

        public Shield Target
        {
            get { return (Shield)Structures[6]; }
            set { Structures[6] = value; }
        }

        public CylinderReflector Chamber
        {
            get { return (CylinderReflector)Structures[0]; }
            set { Structures[0] = value; }
        }

        public Shield ChamberTop
        {
            get { return (Shield)Structures[1]; }
            set { Structures[1] = value; }
        }

        public Shield ChamberBottom
        {
            get { return (Shield)Structures[2]; }
            set { Structures[2] = value; }
        }

        public KatayamaGeometry(CylinderReflector nozzle, CylinderReflector reflector, Shield shield, Shield target, CylinderReflector chamber, Shield chamberTop, Shield chamberBottom, int limit, double reflectionCoefficient, Atom.ReflectionPattern pattern)
            : base(limit, reflectionCoefficient, pattern, new Structure[7])
        {
            Nozzle = nozzle;
            Reflector = reflector;
            Shield = shield;
            Target = target;
            Chamber = chamber;
            ChamberTop = chamberTop;
            ChamberBottom = chamberBottom;
        }

        public override Atom CreateAtomRandomly(Random random)
        {
            //位置
            var r = random.NextDouble();
            var theta = 2 * PI * random.NextDouble();

            var x = Nozzle.Radius * Pow(r, 0.5) * Cos(theta);
            var y = Nozzle.Radius * Pow(r, 0.5) * Sin(theta);
            var z = Nozzle.Position.Z;

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

        protected override bool OnCollision(Atom atom, Collision collision)
        {
            return collision.StructureID == Target.ID;
        }

        protected override Geometry Copy()
        {
            return new KatayamaGeometry((CylinderReflector)Nozzle.Copy(), 
                (CylinderReflector)Reflector.Copy(), 
                (Shield)Shield.Copy(), 
                (Shield)Target.Copy(), 
                (CylinderReflector)Chamber.Copy(),
                (Shield)ChamberTop.Copy(),
                (Shield)ChamberBottom.Copy(),
                ReflectionLimit, ReflectionCoefficient, ReflectionPattern);
        }
    }
}
