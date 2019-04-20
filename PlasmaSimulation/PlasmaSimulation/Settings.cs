using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlasmaSimulation.Geometries;
using PlasmaSimulation.Structures;

namespace PlasmaSimulation
{
    public class Settings
    {
        private const string SaveFileName = "Settings.json";
        public KatayamaGeometry KatayamaGeometry { get; set; }
        public HoleGeometrySetting HoleGeometrySetting { get; set; }
        public ProbeRotationGeometrySetting ProbeRotationGeometrySetting { get; set; }

        public Settings()
        {
            KatayamaGeometry = new KatayamaGeometry(
                new CylinderReflector(0, Vector.Zero, Vector.Forward, 10, 2.5),
                new CylinderReflector(1, Vector.Forward * 10, Vector.Forward, 10, 5),
                new Shield(2, Vector.Forward * 15, Vector.Forward, 2.5),
                new Shield(3, Vector.Forward * 20, Vector.Forward, 5),
                new CylinderReflector(4, Vector.Forward * 150 + Vector.Up * -150, Vector.Up, 300, 150),
                new Shield(5, Vector.Forward * 150 + Vector.Up * 150, Vector.Up, 150),
                new Shield(6, Vector.Forward * 150 + Vector.Up * -150, Vector.Up, 150),
                100, 
                1,
                Atom.ReflectionPattern.Specularly);

            HoleGeometrySetting = new HoleGeometrySetting();
            ProbeRotationGeometrySetting = new ProbeRotationGeometrySetting();
        }

        public static Settings Load()
        {
            if (!File.Exists(SaveFileName))
                return new Settings();

            var json = File.ReadAllText(SaveFileName);
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            return settings;
        }

        public static void Save(Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SaveFileName, json);
        }
    }

    public class HoleGeometrySetting
    {
        public HoleGeometry HoleGeometry { get; set; }

        public double MinimumReflectionCoefficient { get; set; }
        public double MaximumReflectionCoefficient { get; set; }
        public double ReflectionCoefficientInterval { get; set; }

        public double MinimumRadius { get; set; }
        public double MaximumRadius { get; set; }
        public double RadiusInterval { get; set; }

        public int SimulationCount { get; set; }
        public int ReflectionLimit { get; set; }
        public Atom.ReflectionPattern ReflectionPattern { get; set; }

        public System.Windows.Media.Media3D.Point3D CameraPosition { get; set; }
        public HoleExperimentDataSet DataSet { get; set; }

        public HoleGeometrySetting()
        {
            HoleGeometry = new HoleGeometry(
                100, 0.8, Atom.ReflectionPattern.Specularly,
                new CylinderReflector(0, Vector.Zero, Vector.Forward, 40, 2.5),
                new CylinderReflector(3, Vector.Forward * 40, Vector.Forward, 10, 2.5),
                new Hole(1, Vector.Forward * 50, Vector.Forward, 5),
                new Shield(2, Vector.Forward * 60, Vector.Forward, 16)
                );

            MinimumReflectionCoefficient = 0;
            MaximumReflectionCoefficient = 1;
            ReflectionCoefficientInterval = 0.1;

            MinimumRadius = 0;
            MaximumRadius = 7.5;
            RadiusInterval = 0.5;

            SimulationCount = 100000;
            ReflectionLimit = 100;
            ReflectionPattern = Atom.ReflectionPattern.Specularly;

            CameraPosition = new System.Windows.Media.Media3D.Point3D();
        }
    }

    public class ProbeRotationGeometrySetting
    {
        public ProbeRotationGeometry Geometry { get; set; }
        public double ProbeDistance { get; set; }
        public double MinimumAngle { get; set; }
        public double MaximumAngle { get; set; }
        public double AngleInterval { get; set; }
        public int SimulationCount { get; set; }
        public double PlateAngle { get; set; }
        public Atom.ReflectionPattern ReflectionPattern { get; set; }

        public ProbeRotationGeometrySetting()
        {
            Geometry = new ProbeRotationGeometry
                (
                new CylinderReflector(0, 52.5 * Vector.Back, Vector.Forward, 40, 2.5, Atom.ReflectionPattern.Specularly, 0.93),
                new Plate(1, Vector.Zero, (10 / 1.414213) * (Vector.Right + Vector.Forward), 20 * Vector.Up, Atom.ReflectionPattern.Specularly, 1),
                new Shield(2, Vector.Zero, Vector.Right, 16, 0),
                new Hole(3, Vector.Zero, Vector.Forward, null, Atom.ReflectionPattern.Specularly, 0),
                new Hole(4, Vector.Zero, Vector.Forward, null, Atom.ReflectionPattern.Specularly, 0),
                Atom.ReflectionPattern.Specularly
                );

            ProbeDistance = 50;
            MinimumAngle = 45;
            MaximumAngle = 135;
            SimulationCount = 1000000;
            PlateAngle = 45;
            ReflectionPattern = Atom.ReflectionPattern.Specularly;
        }
    }
}
