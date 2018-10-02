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
using PlasmaSimulation.Structures;
namespace PlasmaSimulation.GUI
{
    /// <summary>
    /// ShieldSettingPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class HoleSettingPanel : UserControl
    {
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(int), typeof(HoleSettingPanel));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(HoleSettingPanel));

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

        public Hole Hole => new Hole(ID, PositionUpDown.Vector, DirectionUpDown.Vector, Radius);

        public HoleSettingPanel()
        {
            InitializeComponent();
            DataContext = this;
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

        public void Set(Vector position, Vector direction, double radius)
        {
            SetPosition(position);
            SetDirection(direction);
            Radius = radius;
        }

        public void Set(Hole hole)
        {
            Set(hole.Position, hole.Direction, hole.Radius);
        }
    }
}
