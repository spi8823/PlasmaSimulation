using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PlasmaSimulation.Geometries;
using PlasmaSimulation.Structures;

namespace PlasmaSimulation.GUI
{
    /// <summary>
    /// HoleGeometryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class HoleGeometryWindow : Window
    {
        private CancellationTokenSource CalculationCancelationTokenSource { get; set; }

        public HoleGeometryWindow()
        {
            InitializeComponent();
        }

        public HoleGeometry GetGeometry()
        {
            var nozzle = NozzleSettingPanel.CylinderReflector;
            var hole = HoleSettingPanel.Hole;
            var detector = DetectorSettingPanel.Shield;
            var subnozzle = SubNozzleSettingPanel.CylinderReflector;
            
            var limit = ReflectionLimitUpDown.Value ?? 100;
            var coefficient = MinimumReflectionCoefficientUpDown.Value ?? 1;
            var pattern = ReflectionPatternSelector.ReflectionPattern;

            return new HoleGeometry(limit, coefficient, pattern, nozzle, subnozzle, hole, detector);
        }

        private async void CalculateButton_Clicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;
            InterruptButton.IsEnabled = true;

            //経過時間表示
            var elapsedTime = new TimeSpan();
            void tick(object s, System.Timers.ElapsedEventArgs ee)
            {
                Dispatcher.Invoke(() =>
                {
                    elapsedTime += TimeSpan.FromSeconds(1);
                    ElapsedTimeLabel.Content = "経過時間：" + (int)elapsedTime.TotalMinutes + "分" + elapsedTime.Seconds + "秒";
                });
            }

            using (var timer = new System.Timers.Timer(1000))
            {
                timer.Elapsed += tick;
                timer.Start();

                //計算本体
                var setting = GetSetting();
                CalculationCancelationTokenSource = new CancellationTokenSource();
                var token = CalculationCancelationTokenSource.Token;
                var steps = await Task.Run(() => Calculate(setting, token), token);

                if (!token.IsCancellationRequested)
                    new Graph.SimulationResultWindow(new SimulationResults.HoleGeometrySimulationResult(steps, setting)).Show();

                timer.Stop();
            }

            button.IsEnabled = true;
            InterruptButton.IsEnabled = false;
        }

        private void InterruptButton_Clicked(object sender, RoutedEventArgs e)
        {
            CalculationCancelationTokenSource.Cancel();
        }

        private List<SimulationResults.HoleGeometrySimulationResult.Step> Calculate(HoleGeometrySetting setting, CancellationToken cancellationToken)
        {
            var steps = new List<SimulationResults.HoleGeometrySimulationResult.Step>();

            int progressMaximum = (int)((setting.MaximumReflectionCoefficient - setting.MinimumReflectionCoefficient) / setting.ReflectionCoefficientInterval + 1) * (int)((setting.MaximumRadius - setting.MinimumRadius) / setting.RadiusInterval + 1);
            int progress = 0;

            //プログレスバー初期化
            ShowProgress(progress, progressMaximum);

            //ジオメトリ初期化
            var geometry = setting.HoleGeometry;
            geometry.ReflectionPattern = setting.ReflectionPattern;
            geometry.ReflectionLimit = setting.ReflectionLimit;

            //計算を回す
            for (var coefficient = setting.MinimumReflectionCoefficient; coefficient <= setting.MaximumReflectionCoefficient; coefficient += setting.ReflectionCoefficientInterval)
            {
                var results = new List<(double Radius, long Count)>();
                geometry.ReflectionCoefficient = coefficient;

                for (var radius = setting.MinimumRadius; radius <= setting.MaximumRadius; radius += setting.RadiusInterval)
                {
                    if(cancellationToken.IsCancellationRequested)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ProgressLabel.Content = "進捗：中断";
                            System.Windows.Forms.Application.DoEvents();
                        });
                        return steps;
                    }

                    geometry.Hole = new Hole(geometry.Hole.ID, geometry.Hole.Position, geometry.Hole.Direction, radius);
                    var atoms = geometry.ProcessAsParallel(setting.SimulationCount);
                    //GetAverageReflectionCount(atoms, 0, 2);
                    //GetAverageReflectionCount(atoms, 8, 10);

                    var count = atoms.Count(atom => atom.IsValid);
                    results.Add((radius, count));

                    ShowProgress(++progress, progressMaximum);
                }

                var step = new SimulationResults.HoleGeometrySimulationResult.Step();
                step.ReflectionCoefficient = coefficient;
                step.Datas = results;
                steps.Add(step);
            }

            ShowProgress(progressMaximum, progressMaximum);
            return steps;
        }

        private void ShowProgress(int value, int maximum)
        {
            Dispatcher.Invoke(() =>
            {
                CalculationProgressBar.Minimum = 0;
                CalculationProgressBar.Maximum = maximum;
                CalculationProgressBar.Value = value;
                ProgressLabel.Content = "進捗：" + (int)CalculationProgressBar.Value + " / " + (int)CalculationProgressBar.Maximum;
                System.Windows.Forms.Application.DoEvents();
            });
        }

        public double? GetAverageReflectionCount(List<Atom> atoms, double minRadius, double maxRadius)
        {
            double getRadius(Atom atom) => Math.Sqrt(Math.Pow(atom.Position.X, 2) + Math.Pow(atom.Position.Y, 2));
            if (!atoms.Any(atom => atom.IsValid && minRadius <= getRadius(atom) && getRadius(atom) <= maxRadius))
            {
                Console.WriteLine(minRadius + "～" + maxRadius + ":null");
                return null;
            }
            var rangeAtoms = from atom in atoms
                             let r = getRadius(atom)
                             where atom.IsValid
                             where minRadius <= r && r <= maxRadius
                             select atom;
            var average = rangeAtoms.Average(atom => atom.ReflectionCount);
            Console.WriteLine(minRadius + "～" + maxRadius + ":" + average);
            return average;
        }

        private void ShowGeometry(object sender, RoutedEventArgs e)
        {
            var window = new GraphicalGeometryWindow(GetGeometry());
            var setting = Settings.Load();
            window.CameraPosition = setting.HoleGeometrySetting.CameraPosition;
            window.Show();
            setting.HoleGeometrySetting.CameraPosition = window.CameraPosition;
            Settings.Save(setting);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            var setting = Settings.Load().HoleGeometrySetting;

            var geometry = setting.HoleGeometry;
            NozzleSettingPanel.Set(geometry.Nozzle);
            SubNozzleSettingPanel.Set(geometry.SubNozzle);
            HoleSettingPanel.Set(geometry.Hole);
            DetectorSettingPanel.Set(geometry.Detector);

            MinimumReflectionCoefficientUpDown.Value = setting.MinimumReflectionCoefficient;
            MaximumReflectionCoefficientUpDown.Value = setting.MaximumReflectionCoefficient;
            ReflectionCoefficientIntervalUpDown.Value = setting.ReflectionCoefficientInterval;

            MinimumRadiusUpDown.Value = setting.MinimumRadius;
            MaximumRadiusUpDown.Value = setting.MaximumRadius;
            RadiusIntervalUpDown.Value = setting.RadiusInterval;

            ReflectionLimitUpDown.Value = setting.ReflectionLimit;
            ReflectionPatternSelector.ReflectionPattern = setting.ReflectionPattern;
            SimulationCountUpDown.Value = setting.SimulationCount;
        }

        private void SaveSettings()
        {
            var settings = Settings.Load();
            settings.HoleGeometrySetting = GetSetting();
            Settings.Save(settings);
        }

        private HoleGeometrySetting GetSetting()
        {
            var setting = new HoleGeometrySetting();
            setting.HoleGeometry = GetGeometry();
            setting.MinimumReflectionCoefficient = MinimumReflectionCoefficientUpDown.Value.Value;
            setting.MaximumReflectionCoefficient = MaximumReflectionCoefficientUpDown.Value.Value;
            setting.ReflectionCoefficientInterval = ReflectionCoefficientIntervalUpDown.Value.Value;
            setting.MinimumRadius = MinimumRadiusUpDown.Value.Value;
            setting.MaximumRadius = MaximumRadiusUpDown.Value.Value;
            setting.RadiusInterval = RadiusIntervalUpDown.Value.Value;
            setting.SimulationCount = SimulationCountUpDown.Value.Value;
            setting.ReflectionPattern = ReflectionPatternSelector.ReflectionPattern;
            setting.ReflectionLimit = ReflectionLimitUpDown.Value.Value;
            return setting;
        }

        private void ShowDataSetWindow(object sender, RoutedEventArgs e)
        {
            var dataset = new HoleExperimentDataSet();
        }
    }
}
