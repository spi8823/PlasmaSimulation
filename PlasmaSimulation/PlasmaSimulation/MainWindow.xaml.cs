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
                new CylinderReflector(10, 5, new Vector(0, 0, 0), new Vector(0, 0, 1), 0);
            var reflector = 
                new CylinderReflector(10, 10, new Vector(0, 0, 10), new Vector(0, 0, 1), 1);
            var shield = 
                new Shield(5, new Vector(0, 0, 15), new Vector(0, 0, 1), 2);
            var target = 
                new Shield(10, new Vector(0, 0, 20), new Vector(0, 0, 1), 3);

            var geometry = new KatayamaGeometry(nozzle, reflector, shield, target, 50, 100, Atom.ReflectionPattern.Specularly);

            var count = 100000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = geometry.ProcessAsParallel(count);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            Console.WriteLine(result.Count(r => r != null) + " / " + count.ToString());
        }
    }
}
