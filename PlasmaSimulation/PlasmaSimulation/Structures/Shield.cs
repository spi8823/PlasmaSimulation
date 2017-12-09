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

        /// <summary>
        /// 面の法線
        /// </summary>
        public Vector Direction { get; }

        public Shield(int id, Vector position, Vector direction, double radius)
        {
            Radius = radius;
            Position = position;
            Direction = direction;
            ID = id;
        }

        public Collision? GetCollision(Atom atom)
        {
            
            //Direction * (x - Position) = 0
            //x = atom.Position + atom.Velocity * t
            //賢い！！！！！
            var t = Dot(Direction, (Position - atom.Position)) / Dot(Direction, atom.Velocity);
            if (t <= RoundingValue)
                return null;

            var x = atom.Position + atom.Velocity * t;
            if ((x - Position).Length > Radius)
                return null;

            var normal = Direction;
            if (Dot(atom.Velocity, normal) > 0)
                normal = -1 * normal;
            return new Collision(x, normal, t, ID);
        }
    }
}
