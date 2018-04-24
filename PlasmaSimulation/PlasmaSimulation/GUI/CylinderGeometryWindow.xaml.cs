using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlasmaSimulation.GUI
{
    /// <summary>
    /// CylinderGeometryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CylinderGeometryWindow : Window
    {
        public CylinderGeometryWindow()
        {
            InitializeComponent();
        }

        private CylinderGeometry GetGeometry()
        {
            var radius = RadiusUpDown.Value ?? 5;
            var length = LengthUpDown.Value ?? 100;
            var resolution = ResolutionUpDown.Value ?? 5;

            var span = TimeUpDown.Value ?? 100;
            var interval = TimeIntervalUpDown.Value ?? 10;

            var flux = (FluxUpDown.Value ?? 1) * Math.Pow(10, 15);
            var count = (long)(TrialCountUpDown.Value ?? 100000);

            var cylinder = new CylinderReflector(0, Vector.Zero, Vector.Forward, length, radius);
            var bottom = new Shield(1, Vector.Forward * length, Vector.Forward, radius);

            var geometry = new CylinderGeometry(cylinder, bottom, null, span, interval, flux, resolution, count, 100, Atom.ReflectionPattern.Specularly);
            return geometry;
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Cursor = Cursors.Wait;
            ExecuteButton.IsEnabled = false;

            var geometry = GetGeometry();
            var results = geometry.GetDistribution();

            var r1 = results[results.Count / 100];
            var datas1 = new List<OxyPlot.DataPoint>();
            for (var i = 0; i < r1.Length; i++)
            {
                var point = new OxyPlot.DataPoint(i * geometry.Resolution, r1[i]);
                datas1.Add(point);
            }

            var r2 = results[results.Count / 10];
            var datas2 = new List<OxyPlot.DataPoint>();
            for (var i = 0; i < r2.Length; i++)
            {
                var point = new OxyPlot.DataPoint(i * geometry.Resolution, r2[i]);
                datas2.Add(point);
            }

            var r3 = results.Last();
            var datas3 = new List<OxyPlot.DataPoint>();
            for (var i = 0; i < r2.Length; i++)
            {
                var point = new OxyPlot.DataPoint(i * geometry.Resolution, r3[i]);
                datas3.Add(point);
            }

            DistributionPlot.Series[0].ItemsSource = datas1;
            DistributionPlot.Series[1].ItemsSource = datas2;
            DistributionPlot.Series[2].ItemsSource = datas3;

            DistributionPlot.Axes[0].Maximum = ((CylinderReflector)geometry.Structures[0]).Length;
            DistributionPlot.Axes[1].Maximum = r3.Max();

            ExecuteButton.IsEnabled = true;
            Cursor = Cursors.Arrow;
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds.ToString() + "m");
        }
    }
}
