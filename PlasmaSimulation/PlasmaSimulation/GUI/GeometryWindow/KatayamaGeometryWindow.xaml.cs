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

namespace PlasmaSimulation.GUI.GeometryWindow
{
    /// <summary>
    /// KatayamaGeometryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class KatayamaGeometryWindow : Window
    {
        private Settings Settings { get; set; }
        public KatayamaGeometryWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new GraphicalGeometryWindow(GetGeometry());
            window.ShowDialog();
        }

        private KatayamaGeometry GetGeometry()
        {
            var nozzle = NozzleSettingPanel.CylinderReflector;
            var reflector = ReflectorSettingPanel.CylinderReflector;
            var shield = ShieldSettingPanel.Shield;
            var target = TargetSettingPanel.Shield;
            var chamber = ChamberSettingPanel.CylinderReflector;
            var chamberTop = ChamberTopSettingPanel.Shield;
            var chamberBottom = ChamberBottomSettingPanel.Shield;

            var limit = ReflectionLimitUpDown.Value ?? 100;
            var coefficient = ReflectionCoefficientUpDown.Value ?? 1;
            var pattern = ReflectionPatternSelector.ReflectionPattern;

            return new KatayamaGeometry(nozzle, reflector, shield, target, chamber, chamberTop, chamberBottom, limit, coefficient, pattern);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings = Settings.Load();
            var geometry = Settings.KatayamaGeometry;
            ChamberSettingPanel.Set((CylinderReflector)geometry.Structures[0]);
            ChamberTopSettingPanel.Set((Shield)geometry.Structures[1]);
            ChamberBottomSettingPanel.Set((Shield)geometry.Structures[2]);
            NozzleSettingPanel.Set((CylinderReflector)geometry.Structures[3]);
            ReflectorSettingPanel.Set((CylinderReflector)geometry.Structures[4]);
            ShieldSettingPanel.Set((Shield)geometry.Structures[5]);
            TargetSettingPanel.Set((Shield)geometry.Structures[6]);
            ReflectionLimitUpDown.Value = geometry.ReflectionLimit;
            ReflectionCoefficientUpDown.Value = geometry.ReflectionCoefficient;
            ReflectionPatternSelector.ReflectionPattern = geometry.ReflectionPattern;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.KatayamaGeometry = GetGeometry();
            Settings.Save(Settings);
        }
    }
}
