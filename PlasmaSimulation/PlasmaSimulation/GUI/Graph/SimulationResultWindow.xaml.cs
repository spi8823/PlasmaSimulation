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
using Microsoft.WindowsAPICodePack.Dialogs;
using PlasmaSimulation.SimulationResults;

namespace PlasmaSimulation.GUI.Graph
{
    /// <summary>
    /// HoleGeometryResultGraphWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SimulationResultWindow : Window
    {
        private SimulationResult SimulationResult { get; set; }

        public SimulationResultWindow(SimulationResult result)
        {
            InitializeComponent();
            SimulationResult = result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowResult();
        }

        private void ShowResult()
        {
            HorizontalAxis.Title = SimulationResult.HorizontalAxisTitle;
            if (SimulationResult.HorizontalAxisMinimum.HasValue)
                HorizontalAxis.Minimum = SimulationResult.HorizontalAxisMinimum.Value;
            if (SimulationResult.HorizontalAxisMaximum.HasValue)
                HorizontalAxis.Maximum = SimulationResult.HorizontalAxisMaximum.Value;
            if (SimulationResult.HorizontalAxisStep.HasValue)
                HorizontalAxis.MajorStep = SimulationResult.HorizontalAxisStep.Value;
            if (SimulationResult.VerticalAxisMinimum.HasValue)
                VerticalAxis.Minimum = SimulationResult.VerticalAxisMinimum.Value;
            if (SimulationResult.VerticalAxisMaximum.HasValue)
                VerticalAxis.Maximum = SimulationResult.VerticalAxisMaximum.Value;
            if (SimulationResult.VerticalAxisStep.HasValue)
                VerticalAxis.MajorStep = SimulationResult.VerticalAxisStep.Value;
            VerticalAxis.Title = SimulationResult.VerticalAxisTitle;
            var series = SimulationResult.GetSeries();
            foreach (var s in series)
                GraphPlot.Series.Add(s);
            GraphPlot.LegendPosition = OxyPlot.LegendPosition.RightMiddle;
            GraphPlot.LegendFontSize = 24;
            GraphPlot.InvalidateVisual();

            DiscriptionLabel.Content = SimulationResult.Discription;
        }

        private void OutputImage(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonSaveFileDialog()
            {
                DefaultExtension = "png",
            };

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
            }
        }

        private void OutputCSV(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonSaveFileDialog()
            {
                DefaultExtension = "xlsx",
            };

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SimulationResult.OutputCSV(dialog.FileName);
            }
        }
    }
}
