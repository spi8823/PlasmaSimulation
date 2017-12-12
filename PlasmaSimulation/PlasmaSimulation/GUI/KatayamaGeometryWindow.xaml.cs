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
    /// KatayamaGeometryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class KatayamaGeometryWindow : Window
    {
        public KatayamaGeometryWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var nozzle = NozzleSettingPanel.CylinderReflector;
            var reflector = ReflectorSettingPanel.CylinderReflector;
            var shield = ShieldSettingPanel.Shield;
            var target = TargetSettingPanel.Shield;

            var window = new GraphicalGeometryWindow(new KatayamaGeometry(nozzle, reflector, shield, target, 100, Atom.ReflectionPattern.Specularly));
            window.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NozzleSettingPanel.Set(Vector.Zero, Vector.Forward, 2.5, 10);
            ReflectorSettingPanel.Set(Vector.Forward * 10, Vector.Forward, 5, 10);
            ShieldSettingPanel.Set(Vector.Forward * 15, Vector.Forward, 2.5);
            TargetSettingPanel.Set(Vector.Forward * 20, Vector.Forward, 5);
        }
    }
}
