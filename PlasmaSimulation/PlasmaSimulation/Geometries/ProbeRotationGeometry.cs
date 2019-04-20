using System;
using PlasmaSimulation.Structures;
using static System.Math;

namespace PlasmaSimulation.Geometries
{
    public class ProbeRotationGeometry : Geometry
    {
        public CylinderReflector Nozzle
        {
            get { return (CylinderReflector)Structures[0]; }
            set { Structures[0] = value; }
        }

        public Plate Plate
        {
            get { return (Plate)Structures[1]; }
            set { Structures[1] = value; }
        }

        public Shield Probe
        {
            get { return (Shield)Structures[2]; }
            set { Structures[2] = value; }
        }

        public Hole Slit1
        {
            get { return (Hole)Structures[3]; }
            set { Structures[3] = value; }
        }

        public Hole Slit2
        {
            get { return (Hole)Structures[4]; }
            set { Structures[4] = value; }
        }

        private Rotation NozzleRotation { get; }

        public ProbeRotationGeometry(CylinderReflector nozzle, Plate plate, Shield probe, Hole slit1, Hole slit2, Atom.ReflectionPattern pattern) : base(100, 1, pattern, new Structure[] { nozzle, plate, probe, slit1, slit2 })
        {
            NozzleRotation = new Rotation(Vector.Forward, nozzle.Direction);
        }

        public override Atom CreateAtomRandomly(Random random)
        {
            var atom = new Atom();
            //位置
            var r = random.NextDouble();
            var theta = 2 * PI * random.NextDouble();

            var x = Nozzle.Radius * Pow(r, 0.5) * Cos(theta);
            var y = Nozzle.Radius * Pow(r, 0.5) * Sin(theta);
            var z = 0.0;

            atom.Position = Nozzle.Position + NozzleRotation.Rotate(new Vector(x, y, z));

            //速度
            r = random.NextDouble();
            theta = 2 * PI * random.NextDouble();

            x = Sqrt(1 - r * r) * Cos(theta);
            y = Sqrt(1 - r * r) * Sin(theta);
            z = r;

            atom.Velocity = NozzleRotation.Rotate(new Vector(x, y, z));

            return atom;
        }

        protected override Geometry Copy()
        {
            return new ProbeRotationGeometry((CylinderReflector)Nozzle.Copy(), (Plate)Plate.Copy(), (Shield)Probe.Copy(), (Hole)Slit1.Copy(), (Hole)Slit2.Copy(), ReflectionPattern);
        }

        protected override bool OnCollision(Atom atom, Collision collision)
        {
            if (collision.StructureID == Plate.ID)
                atom.History++;

            return collision.StructureID == Probe.ID;
        }
    }
}
