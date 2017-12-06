using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasmaSimulation.Extensions;

namespace PlasmaSimulation
{
    public struct Shield : Structure
    {
        public int ID { get; }
        public double Radius { get; }
        public Vector Position { get; }
        public Vector Normal { get; }

        public Shield(double radius, Vector position, Vector normal, int id)
        {
            Radius = radius;
            Position = position;
            Normal = normal;
            ID = id;
        }

        public Collision? GetCollision(Atom atom)
        {
            
            //Normal * (x - Position) = 0
            //x = atom.Position + atom.Velocity * t
            //賢い！！！！！
            var t = Dot(Normal, (Position - atom.Position)) / Dot(Normal, atom.Velocity);
            if (t <= 0)
                return null;

            var x = atom.Position + atom.Velocity * t;
            if ((x - Position).Length() > Radius)
                return null;

            var normal = Normal;
            if (Dot(atom.Velocity,normal) > 0)
                normal = -1 * normal;
            return new Collision(x, normal, t, ID);
        }
    }
}
