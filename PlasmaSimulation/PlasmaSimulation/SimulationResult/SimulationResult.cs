using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation.SimulationResults
{
    public abstract class SimulationResult
    {
        public abstract void OutputCSV(string filename);
        public abstract List<OxyPlot.Wpf.Series> GetSeries();
        public abstract string Discription { get; }
        public abstract string HorizontalAxisTitle { get; }
        public abstract string VerticalAxisTitle { get; }
    }
}
