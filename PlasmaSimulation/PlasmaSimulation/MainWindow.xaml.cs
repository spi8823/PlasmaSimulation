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
            var from = new Vector(1, 0, 0);
            var to = new Vector(0, 1, 0);
            var rotation = new Rotation(from, to);
            var result = rotation.Rotate(new Vector(0, 0, 1));
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void StartKatayamaGeometry(object sender, RoutedEventArgs e)
        {
            new GUI.KatayamaGeometryWindow().ShowDialog();
        }

        private void StartCylinderGeometry(object sender, RoutedEventArgs e)
        {
            new GUI.CylinderGeometryWindow().ShowDialog();
        }

        private void StartHoleGeometry(object sender, RoutedEventArgs e)
        {
            new GUI.HoleGeometryWindow().ShowDialog();
        }
    }
}
