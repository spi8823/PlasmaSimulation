using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlasmaSimulation
{
    public class Settings
    {
        private const string SaveFileName = "Settings.json";
        public KatayamaGeometry KatayamaGeometry { get; set; }

        public Settings()
        {
            KatayamaGeometry = new KatayamaGeometry(
                new CylinderReflector(0, Vector.Zero, Vector.Forward, 10, 2.5),
                new CylinderReflector(1, Vector.Forward * 10, Vector.Forward, 10, 5),
                new Shield(2, Vector.Forward * 15, Vector.Forward, 2.5),
                new Shield(3, Vector.Forward * 20, Vector.Forward, 5),
                100, 
                Atom.ReflectionPattern.Specularly);
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
}
