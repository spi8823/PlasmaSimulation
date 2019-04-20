using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PlasmaSimulation.Geometries;
using PlasmaSimulation.Structures;

namespace PlasmaSimulation.GUI.GeometryWindow
{
    /// <summary>
    /// ProbeRotationGeometryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ProbeRotationGeometryWindow : Window
    {
        private CancellationTokenSource CalculationCancelationTokenSource { get; set; }

        public ProbeRotationGeometryWindow()
        {
            InitializeComponent();
        }

        public ProbeRotationGeometry GetGeometry()
        {
            var nozzle = NozzleSettingPanel.CylinderReflector;
            var plate = PlateSettingPanel.Plate;
            var probe = ProbeSettingPanel.Shield;
            var slit1 = Slit1SettingPanel.Hole;
            var slit2 = Slit2SettingPanel.Hole;

            var pattern = ReflectionPatternSelector.ReflectionPattern;
            plate = new Plate(plate.ID, plate.Position, plate.HorizontalVector, plate.VerticalVector, pattern, plate.ReflectionCoefficient);

            return new ProbeRotationGeometry(nozzle, plate, probe, slit1, slit2, pattern);
        }

        private void LoadSettings()
        {
            var setting = Settings.Load().ProbeRotationGeometrySetting;
            var geometry = setting.Geometry;
            NozzleSettingPanel.Set(geometry.Nozzle);
            PlateSettingPanel.Set(geometry.Plate);
            ProbeSettingPanel.Set(geometry.Probe);
            Slit1SettingPanel.Set(geometry.Slit1);
            Slit2SettingPanel.Set(geometry.Slit2);

            ProbeDistanceUpDown.Value = setting.ProbeDistance;
            MinimumAngleUpDown.Value = setting.MinimumAngle;
            MaximumAngleUpDown.Value = setting.MaximumAngle;
            AngleIntervalUpDown.Value = setting.AngleInterval;
            SimulationCountUpDown.Value = setting.SimulationCount;
            PlateAngleUpDown.Value = setting.PlateAngle;
            ReflectionPatternSelector.ReflectionPattern = setting.ReflectionPattern;
        }

        private void SaveSettings()
        {
            var settings = Settings.Load();
            settings.ProbeRotationGeometrySetting = GetSetting();
            Settings.Save(settings);
        }

        private ProbeRotationGeometrySetting GetSetting()
        {
            var setting = new ProbeRotationGeometrySetting();
            setting.Geometry = GetGeometry();
            setting.ProbeDistance = Math.Round(ProbeDistanceUpDown.Value.Value, 2);
            setting.MinimumAngle = Math.Round(MinimumAngleUpDown.Value.Value, 2);
            setting.MaximumAngle = Math.Round(MaximumAngleUpDown.Value.Value, 2);
            setting.AngleInterval = Math.Round(AngleIntervalUpDown.Value.Value, 2);
            setting.SimulationCount = SimulationCountUpDown.Value.Value;
            setting.PlateAngle = PlateAngleUpDown.Value.Value;
            setting.ReflectionPattern = ReflectionPatternSelector.ReflectionPattern;
            return setting;
        }

        private void SetPlateAngle(ProbeRotationGeometrySetting setting)
        {
            var angle = setting.PlateAngle;
            var geometry = setting.Geometry;
            var rotation = new Rotation(Vector.Up, angle / -180 * Math.PI);
            var plate = geometry.Plate;
            geometry.Plate = new Plate(plate.ID, plate.Position, rotation.Rotate(plate.HorizontalVector), rotation.Rotate(plate.VerticalVector), plate.ReflectionPattern, plate.ReflectionCoefficient);
        }

        private void SetProbeAngle(ProbeRotationGeometrySetting setting, double angle)
        {
            var geometry = setting.Geometry;
            var rotation = new Rotation(Vector.Up, angle / -180 * Math.PI);
            var plate_nozzle = (geometry.Nozzle.Position - geometry.Plate.Position).Normal;
            var position = rotation.Rotate(plate_nozzle * setting.ProbeDistance);
            var normal = rotation.Rotate(plate_nozzle).Normal;
            geometry.Probe = new Shield(geometry.Probe.ID, position, normal, geometry.Probe.Radius, Atom.ReflectionPattern.Specularly, geometry.Probe.ReflectionCoefficient);

            var gap_h = rotation.Rotate(new Vector(1, 0, 0));
            var gap_v = new Vector(0, 1, 0);
            var gap1 = new Vector(0, 0, 0) + gap_h * 0;
            var gap2 = new Vector(0, 0, 0) + gap_h * 0.3;
            var slit1 = geometry.Slit1;
            geometry.Slit1 = new Hole(slit1.ID, position - 15 * rotation.Rotate(plate_nozzle) + gap1, normal, 1, Atom.ReflectionPattern.Specularly, 0);
            var slit2 = geometry.Slit2;
            geometry.Slit2 = new Hole(slit2.ID, position - 20 * rotation.Rotate(plate_nozzle) + gap2, normal, 1, Atom.ReflectionPattern.Specularly, 0);
        }

        private List<SimulationResults.ProbeRotationGeometrySimulationResult.DataPoint> Simulate(ProbeRotationGeometrySetting setting, CancellationToken token)
        {
            SetPlateAngle(setting);
            int progressMaximum = (int)Math.Round((setting.MaximumAngle - setting.MinimumAngle) / setting.AngleInterval + 1);
            int progress = 0;

            //プログレスバー初期化
            ShowProgress(progress, progressMaximum);

            var geometry = setting.Geometry;
            var list = new List<SimulationResults.ProbeRotationGeometrySimulationResult.DataPoint>();

            for(var angle = setting.MinimumAngle;angle <= setting.MaximumAngle;angle += setting.AngleInterval)
            {
                if (token.IsCancellationRequested)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ProgressLabel.Content = "進捗：中断";
                        System.Windows.Forms.Application.DoEvents();
                    });
                    return list;
                }

                SetProbeAngle(setting, angle);
                var atoms = geometry.ProcessAsParallel(setting.SimulationCount);
                list.Add(new SimulationResults.ProbeRotationGeometrySimulationResult.DataPoint(angle, atoms.Count(atom => atom.IsValid), atoms.Count(atom => atom.IsValid && atom.History == 0)));

                ShowProgress(++progress, progressMaximum);
            }

            return list;
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

        private async void CalculateButton_Clicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;
            InterruptButton.IsEnabled = true;

            var elapsedTime = new TimeSpan();
            void tick(object s, System.Timers.ElapsedEventArgs _)
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

                var setting = GetSetting();
                CalculationCancelationTokenSource = new CancellationTokenSource();
                var token = CalculationCancelationTokenSource.Token;
                var result = await Task.Run(() => Simulate(setting, token), token);

                if(!token.IsCancellationRequested)
                {
                    new Graph.SimulationResultWindow(new SimulationResults.ProbeRotationGeometrySimulationResult(result, setting)).Show();
                }

                timer.Stop();
            }

            button.IsEnabled = true;
            InterruptButton.IsEnabled = false;
        }

        private async void ExecuteScriptButton_Clicked(object sender, RoutedEventArgs e)
        {
            PlateSettingPanel.PositionUpDown.Z = 0;
            CalculateButton_Clicked(sender, e);
            PlateSettingPanel.PositionUpDown.Z = 2.5;
            CalculateButton_Clicked(sender, e);
            PlateSettingPanel.PositionUpDown.Z = 5;
            CalculateButton_Clicked(sender, e);
        }

        private void InterruptButton_Clicked(object sender, RoutedEventArgs e)
        {
            CalculationCancelationTokenSource.Cancel();
        }

        private void ShowGeometry(object sender, RoutedEventArgs e)
        {
            var setting = GetSetting();
            SetPlateAngle(setting);
            SetProbeAngle(setting, MinimumAngleUpDown.Value.Value);
            var window = new GraphicalGeometryWindow(setting.Geometry);
            window.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }
    }
}
