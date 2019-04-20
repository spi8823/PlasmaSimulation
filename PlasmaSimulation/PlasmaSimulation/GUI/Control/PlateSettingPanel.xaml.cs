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

namespace PlasmaSimulation.GUI.Control
{
    /// <summary>
    /// PlateSettingPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class PlateSettingPanel : UserControl
    {
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(int), typeof(PlateSettingPanel));
        public static readonly DependencyProperty ReflectionCoefficientProperty = DependencyProperty.Register("ReflectionCoefficient", typeof(double?), typeof(PlateSettingPanel));

        public int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public double? ReflectionCoefficient
        {
            get { return (double?)GetValue(ReflectionCoefficientProperty); }
            set { SetValue(ReflectionCoefficientProperty, value); }
        }

        public Plate Plate => new Plate(ID, PositionUpDown.Vector, HorizontalVectorUpDown.Vector, VerticalVectorUpDown.Vector, Atom.ReflectionPattern.Specularly, ReflectionCoefficient);

        public PlateSettingPanel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Set(Vector position, Vector horizontalVector, Vector verticalVector)
        {
            PositionUpDown.XUpDown.Value = position.X;
            PositionUpDown.YUpDown.Value = position.Y;
            PositionUpDown.ZUpDown.Value = position.Z;
            HorizontalVectorUpDown.XUpDown.Value = horizontalVector.X;
            HorizontalVectorUpDown.YUpDown.Value = horizontalVector.Y;
            HorizontalVectorUpDown.ZUpDown.Value = horizontalVector.Z;
            VerticalVectorUpDown.XUpDown.Value = verticalVector.X;
            VerticalVectorUpDown.YUpDown.Value = verticalVector.Y;
            VerticalVectorUpDown.ZUpDown.Value = verticalVector.Z;
        }

        public void Set(Plate plate)
        {
            Set(plate.Position, plate.HorizontalVector, plate.VerticalVector);
            ReflectionCoefficient = plate.ReflectionCoefficient;
        }
    }
}
