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

namespace PlasmaSimulation.GUI
{
    /// <summary>
    /// CylinderReflectorSettingPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class CylinderReflectorSettingPanel : UserControl
    {
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(int), typeof(CylinderReflectorSettingPanel));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(CylinderReflectorSettingPanel));
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register("Length", typeof(double), typeof(CylinderReflectorSettingPanel));

        public int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public CylinderReflector CylinderReflector => new CylinderReflector(ID, PositionUpDown.Vector, DirectionUpDown.Vector, Length, Radius);

        public CylinderReflectorSettingPanel()
        {
            DataContext = this;
            InitializeComponent();
        }

        public void SetPosition(Vector position)
        {
            PositionUpDown.XUpDown.Value = position.X;
            PositionUpDown.YUpDown.Value = position.Y;
            PositionUpDown.ZUpDown.Value = position.Z;
        }

        public void SetDirection(Vector direction)
        {
            DirectionUpDown.XUpDown.Value = direction.X;
            DirectionUpDown.YUpDown.Value = direction.Y;
            DirectionUpDown.ZUpDown.Value = direction.Z;
        }

        public void Set(Vector position, Vector direction, double radius, double length)
        {
            SetPosition(position);
            SetDirection(direction);
            Radius = radius;
            Length = length;
        }

        public void Set(CylinderReflector cylinderReflector)
        {
            Set(cylinderReflector.Position, cylinderReflector.Direction, cylinderReflector.Radius, cylinderReflector.Length);
        }
    }
}
