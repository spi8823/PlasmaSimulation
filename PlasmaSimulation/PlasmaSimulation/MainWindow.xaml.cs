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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PlasmaSimulation.Extensions;

namespace PlasmaSimulation
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var nozzle = 
                new CylinderReflector(0, Vector.Zero, Vector.Forward, 20, 2.5);
            var reflector = 
                new CylinderReflector(1, Vector.Forward * 20, Vector.Forward, 20, 5);
            var shield = 
                new Shield(2, Vector.Forward * 30, Vector.Forward, 0);
            var target = 
                new Shield(3, Vector.Forward * 40, Vector.Forward, 5);

            var geometry = new KatayamaGeometry(nozzle, reflector, shield, target, 50, 50, Atom.ReflectionPattern.Specularly);
            var window = new GUI.GraphicalGeometryWindow(geometry);
            window.ShowDialog();
            Close();
            return;

            var count = 10000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = geometry.ProcessAsParallel(count);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            Console.WriteLine(result.Count(r => r != null) + " / " + count.ToString());
        }
    }
}
