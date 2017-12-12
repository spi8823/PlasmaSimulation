﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation
{
    public class CylinderGeometry : Geometry
    {
        protected CylinderGeometry(int limit, double reflectionCoefficient, Atom.ReflectionPattern pattern)
            : base(limit, reflectionCoefficient, pattern, null)
        {
            throw new NotImplementedException();
        }

        public override Atom CreateAtomRandomly(Random random)
        {
            throw new NotImplementedException();
        }

        public override List<Vector?> GetTrack(Atom atom, Random random)
        {
            throw new NotImplementedException();
        }

        protected override bool ShouldTerminate(Collision collision)
        {
            throw new NotImplementedException();
        }

        protected override Geometry Copy()
        {
            throw new NotImplementedException();
        }
    }
}
