using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Wpf;
using PlasmaSimulation.Geometries;
using ClosedXML.Excel;

namespace PlasmaSimulation.SimulationResults
{
    class ProbeRotationGeometrySimulationResult : SimulationResult
    {
        public List<DataPoint> DataPoints { get; }
        public ProbeRotationGeometrySetting Setting { get; }
        public override string Discription { get; }
        public override string HorizontalAxisTitle { get; } = "プローブ角度(degree)";
        public override double? HorizontalAxisMinimum { get; }
        public override double? HorizontalAxisStep { get; } = 15.0;
        public override string VerticalAxisTitle { get; } = "入射水素原子数";

        public ProbeRotationGeometrySimulationResult(List<DataPoint> dataPoints, ProbeRotationGeometrySetting setting)
        {
            DataPoints = dataPoints;
            HorizontalAxisMinimum = DataPoints.Min(p => p.Angle);
            Setting = setting;

            var builder = new StringBuilder();
            builder.Append("板:");
            builder.Append((Setting.Geometry.Plate.HorizontalVector.Length * 2).ToString("0.0"));
            builder.Append("×");
            builder.Append((Setting.Geometry.Plate.VerticalVector.Length * 2).ToString("0.0"));
            builder.Append(", 反射パターン:");
            builder.Append(Extensions.ToString(Setting.ReflectionPattern));
            builder.Append(", 最大入射数:");
            builder.Append(DataPoints.Max(point => point.Count));
            builder.Append("/");
            builder.Append(Setting.SimulationCount);
            Discription = builder.ToString();
        }

        public override List<Series> GetSeries()
        {
            var series = new LineSeries();
            double max = DataPoints.Max(p => p.Count);
            var points = new List<OxyPlot.DataPoint>();
            foreach (var p in DataPoints)
                points.Add(new OxyPlot.DataPoint(p.Angle, p.Count / max));
            series.ItemsSource = points;
            series.Title = "全体";

            var directSeries = new LineSeries();
            var directPoints = new List<OxyPlot.DataPoint>();
            foreach (var p in DataPoints)
                directPoints.Add(new OxyPlot.DataPoint(p.Angle, p.DirectCount / max));
            directSeries.ItemsSource = directPoints;
            directSeries.Title = "直接入射";

            var reflectSeries = new LineSeries();
            var reflectPoints = new List<OxyPlot.DataPoint>();
            foreach (var p in DataPoints)
                reflectPoints.Add(new OxyPlot.DataPoint(p.Angle, p.ReflectCount / max));
            reflectSeries.ItemsSource = reflectPoints;
            reflectSeries.Title = "反射後入射";

            return new List<Series>() { series, directSeries, reflectSeries };
        }

        public override void OutputCSV(string filename)
        {
            var book = new XLWorkbook();
            var sheet = book.AddWorksheet("Main");

            //説明
            sheet.Cell(1, 1).Value = "説明";
            sheet.Cell(2, 1).Value = Discription;

            //データ
            const int dataStartRow = 4;
            sheet.Cell(dataStartRow, 1).Value = "角度";
            sheet.Cell(dataStartRow, 1).Value = "入射粒子数";
            for(var i = 0;i < DataPoints.Count;i++)
            {
                sheet.Cell(dataStartRow + 1 + i, 1).Value = DataPoints[i].Angle;
                sheet.Cell(dataStartRow + 1 + i, 2).Value = DataPoints[i].Count;
            }

            book.SaveAs(filename);
        }

        public struct DataPoint
        {
            public double Angle;
            public int Count;
            public int DirectCount;
            public int ReflectCount => Count - DirectCount;

            public DataPoint(double angle, int count, int direct)
            {
                Angle = angle;
                Count = count;
                DirectCount = direct;
            }
        }
    }
}
