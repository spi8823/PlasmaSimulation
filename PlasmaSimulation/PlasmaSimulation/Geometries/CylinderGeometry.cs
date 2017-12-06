using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation
{
    public class CylinderGeometry : Geometry
    {
        protected CylinderGeometry(int limit, Atom.ReflectionPattern pattern)
            : base(limit, pattern)
        {
        }

        protected override Atom CreateAtomRandomly(Random random)
        {
            throw new NotImplementedException();
        }

        protected override Vector? GetResult(Atom atom)
        {
            throw new NotImplementedException();
        }

        protected override Geometry Copy()
        {
            throw new NotImplementedException();
        }
    }
}
