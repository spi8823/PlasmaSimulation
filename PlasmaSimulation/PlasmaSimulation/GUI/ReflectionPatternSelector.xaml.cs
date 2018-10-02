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
    /// ReflectionPatternComboBox.xaml の相互作用ロジック
    /// </summary>
    public partial class ReflectionPatternSelector : UserControl
    {
        public Atom.ReflectionPattern ReflectionPattern
        {
            get
            {
                return (Atom.ReflectionPattern)ReflectionPatternComboBox.SelectedIndex;
            }

            set
            {
                ReflectionPatternComboBox.SelectedIndex = (int)value;
            }
        }

        public ReflectionPatternSelector()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReflectionPatternComboBox.Items.Add("鏡面反射");
            ReflectionPatternComboBox.Items.Add("ランダム反射");
            ReflectionPatternComboBox.Items.Add("コサイン分布散乱");
            ReflectionPatternComboBox.Items.Add("コサイン反射");
        }
    }
}
