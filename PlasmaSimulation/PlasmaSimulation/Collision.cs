using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation
{
    public struct Collision
    {
        public double Time { get; }
        public Vector Position { get; }
        public Vector Normal { get; }
        public int StructureID { get; }

        public Collision(Vector position, Vector normal, double time, int id)
        {
            Position = position;
            Normal = normal;
            Time = time;
            StructureID = id;
        }
    }
}
