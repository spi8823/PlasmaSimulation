using System;
using static PlasmaSimulation.Extensions;

namespace PlasmaSimulation.Structures
{
    public struct Plate : Structure
    {
        public int ID { get; }

        public Vector Position { get; }

        public Vector Direction { get; }

        public double? ReflectionCoefficient { get; }
        public Atom.ReflectionPattern ReflectionPattern { get; }

        public Collision Collision { get; }

        public Vector Normal => Direction;
        public Vector HorizontalVector { get; }
        public Vector VerticalVector { get; }

        public Plate(int id, Vector position, Vector horizontalVector, Vector verticalVector, Atom.ReflectionPattern reflectionPattern, double? reflectionCoefficient = null)
        {
            ID = id;
            Position = position;
            Direction = Cross(horizontalVector, verticalVector).Normal;
            HorizontalVector = horizontalVector;
            VerticalVector = verticalVector;
            ReflectionCoefficient = reflectionCoefficient;
            ReflectionPattern = reflectionPattern;
            Collision = new Collision(id, reflectionPattern);
        }

        public Structure Copy()
        {
            return new Plate(ID, Position, HorizontalVector, VerticalVector, ReflectionPattern, ReflectionCoefficient);
        }

        public bool SetCollision(Atom atom)
        {
            var t = Dot(Normal, (Position - atom.Position)) / Dot(Normal, atom.Velocity);
            if (t <= RoundingValue || double.IsNaN(t))
            {
                Collision.Disable();
                return false;
            }

            var p = atom.Position + atom.Velocity * t - Position;

            var h = Dot(p, HorizontalVector) / Math.Pow(HorizontalVector.Length, 2);
            if(h < -1 || 1 < h)
            {
                Collision.Disable();
                return false;
            }

            var v = Dot(p, VerticalVector) / Math.Pow(VerticalVector.Length, 2);
            if (v < -1 || 1 < v)
            {
                Collision.Disable();
                return false;
            }

            if (Dot(Normal, atom.Velocity) > 0)
                Collision.Set(atom.Position + atom.Velocity * t, -1 * Normal, t);
            else
                Collision.Set(atom.Position + atom.Velocity * t, Normal, t);

            return true;
        }
    }
}
