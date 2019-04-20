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
    /// ShieldSettingPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class ShieldSettingPanel : UserControl
    {
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(int), typeof(ShieldSettingPanel));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(ShieldSettingPanel));

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

        public Shield Shield => new Shield(ID, PositionUpDown.Vector, DirectionUpDown.Vector, Radius);

        public ShieldSettingPanel()
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

        public void Set(Shield shield)
        {
            Set(shield.Position, shield.Direction, shield.Radius);
        }
    }
}
