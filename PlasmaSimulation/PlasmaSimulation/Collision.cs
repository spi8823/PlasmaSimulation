using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation
{
    public class Collision
    {
        public bool IsValid { get; set; }
        public double Time { get; set; }
        public Vector Position { get; set; }
        public Vector Normal { get; set; }
        public int StructureID { get; set; }

        public Collision(int id)
        {
            IsValid = false;
            Time = double.NaN;
            Position = Vector.Zero;
            Normal = Vector.Zero;
            StructureID = id;
        }

        public void Disable()
        {
            IsValid = false;
        }

        public void Set(Vector position, Vector normal, double time)
        {
            IsValid = true;
            Position = position;
            Normal = normal;
            Time = time;
        }
    }
}
