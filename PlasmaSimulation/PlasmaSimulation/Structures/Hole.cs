using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasmaSimulation.Extensions;

namespace PlasmaSimulation.Structures
{
    public struct Hole : Structure
    {
        public int ID { get; }

        public Vector Position { get; }

        public Vector Direction { get; }

        public double? Radius { get; }

        public double? ReflectionCoefficient { get; }
        public Atom.ReflectionPattern ReflectionPattern { get; }

        public Collision Collision { get; }

        public Hole(int id, Vector position, Vector direction, double? radius, Atom.ReflectionPattern reflectionPattern = Atom.ReflectionPattern.Specularly, double? reflectionCoefficient = null)
        {
            Radius = radius;
            Position = position;
            Direction = direction.Normal;
            ReflectionCoefficient = reflectionCoefficient;
            ReflectionPattern = reflectionPattern;
            ID = id;
            Collision = new Collision(ID, reflectionPattern);
        }

        public Structure Copy()
        {
            return new Hole(ID, Position, Direction, Radius, ReflectionPattern, ReflectionCoefficient);
        }

        public bool SetCollision(Atom atom)
        {
            if(Radius == null)
            {
                Collision.Disable();
                return false;
            }

            //Direction * (x - Position) = 0
            //x = atom.Position + atom.Velocity * t
            //賢い！！！！！
            var t = Dot(Direction, (Position - atom.Position)) / Dot(Direction, atom.Velocity);
            if (t <= RoundingValue || double.IsNaN(t))
            {
                Collision.Disable();
                return false;
            }
            var x = atom.Position + atom.Velocity * t;
            if ((x - Position).Length < Radius)
            {
                Collision.Disable();
                return false;
            }

            var normal = Direction;
            if (Dot(atom.Velocity, normal) > 0)
                normal = -1 * normal;

            Collision.Set(x, normal, t);
            return true;
        }
    }
}
