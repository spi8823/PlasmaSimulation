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
    /// DataSetWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DataSetWindow : Window
    {
        public DataSetWindow(List<object> dataSet)
        {
            InitializeComponent();
            DataSetGrid.ItemsSource = dataSet;
        }
    }
}
