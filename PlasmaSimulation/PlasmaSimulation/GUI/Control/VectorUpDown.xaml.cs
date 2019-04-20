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

namespace PlasmaSimulation.GUI.Control
{
    /// <summary>
    /// VectorUpDown.xaml の相互作用ロジック
    /// </summary>
    public partial class VectorUpDown : UserControl
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(VectorUpDown));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(VectorUpDown));
        public static readonly DependencyProperty ZProperty = DependencyProperty.Register("Z", typeof(double), typeof(VectorUpDown));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public double Z
        {
            get { return (double)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }

        public Vector Vector => new Vector(XUpDown.Value ?? 0, YUpDown.Value ?? 0, ZUpDown.Value ?? 0);

        public VectorUpDown()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
