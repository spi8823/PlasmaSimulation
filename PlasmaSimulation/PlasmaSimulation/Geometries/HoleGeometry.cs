using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using PlasmaSimulation.Structures;

namespace PlasmaSimulation.Geometries
{
    public class HoleGeometry : Geometry
    {
        public CylinderReflector Nozzle
        {
            get { return (CylinderReflector)Structures[0]; }
            set { Structures[0] = value; }
        }

        public CylinderReflector SubNozzle
        {
            get { return (CylinderReflector)Structures[1]; }
            set { Structures[1] = value; }
        }

        public Hole Hole
        {
            get { return (Hole)Structures[2]; }
            set { Structures[2] = value; }
        }

        public Shield Detector
        {
            get { return (Shield)Structures[3]; }
            set { Structures[3] = value; }
        }

        public HoleGeometry(int limit, double reflectionCoefficient, Atom.ReflectionPattern pattern, CylinderReflector nozzle, CylinderReflector subNozzle, Hole hole, Shield detector) : base(limit, reflectionCoefficient, pattern, new Structure[] { nozzle, subNozzle , hole, detector})
        {
        }

        public override Atom CreateAtomRandomly(Random random)
        {
            var atom = new Atom();
            //位置
            var r = random.NextDouble();
            var theta = 2 * PI * random.NextDouble();

            var x = Nozzle.Radius * Pow(r, 0.5) * Cos(theta);
            var y = Nozzle.Radius * Pow(r, 0.5) * Sin(theta);
            var z = Nozzle.Position.Z;

            atom.Position = new Vector(x, y, z);

            //速度
            r = random.NextDouble();
            theta = 2 * PI * random.NextDouble();

            x = Sqrt(1 - r * r) * Cos(theta);
            y = Sqrt(1 - r * r) * Sin(theta);
            z = r;

            atom.Velocity = new Vector(x, y, z);

            return atom;
        }

        protected override Geometry Copy()
        {
            return new HoleGeometry(ReflectionLimit, ReflectionCoefficient, ReflectionPattern, (CylinderReflector)Nozzle.Copy(), (CylinderReflector)SubNozzle.Copy(), (Hole)Hole.Copy(), (Shield)Detector.Copy());
        }

        protected override bool ShouldTerminate(Collision collision)
        {
            return collision.StructureID == Detector.ID;
        }
    }
}
