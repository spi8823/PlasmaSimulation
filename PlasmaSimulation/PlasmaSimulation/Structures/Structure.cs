using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation
{
    public interface Structure
    {
        int ID { get; }
        Vector Position { get; }
        Vector Direction { get; }
        double? ReflectionCoefficient { get; }
        Collision Collision { get; }
        void SetCollision(Atom atom);
        Structure Copy();
    }
}
