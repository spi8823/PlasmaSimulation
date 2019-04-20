using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using OxyPlot.Wpf;

namespace PlasmaSimulation.SimulationResults
{
    public class HoleGeometrySimulationResult : SimulationResult
    {
        public HoleGeometrySetting GeometrySetting { get; }
        public List<Step> Steps { get; set; }
        public override string Discription { get; }
        public override string HorizontalAxisTitle { get; } = "面積(㎟)";
        public override string VerticalAxisTitle { get; } = "入射水素原子数";

        public HoleGeometrySimulationResult(List<Step> steps, HoleGeometrySetting setting)
        {
            Steps = steps;
            GeometrySetting = setting;

            var subnozzle = GeometrySetting.HoleGeometry.SubNozzle;
            var builder = new StringBuilder();
            builder.Append("サブノズル　");
            builder.Append("半径:" + subnozzle.Radius + ",");
            builder.Append("長さ:" + subnozzle.Length + ",");
            builder.Append("距離:" + (subnozzle.Position.Z - 40));
            builder.Append("ホール板:" + GeometrySetting.HoleGeometry.Hole.Position.Z);
            Discription = builder.ToString();
        }

        public override void OutputCSV(string filename)
        {
            var steps = Steps.OrderBy(step => step.ReflectionCoefficient).ToList();
            var radiusList = (from step in steps from data in step.Datas select data.Radius).Distinct().OrderBy(r => r).ToList();

            var book = new XLWorkbook();
            var sheet = book.AddWorksheet("Main");

            //説明
            sheet.Cell(1, 1).Value = "説明";
            sheet.Cell(2, 1).Value = Discription;

            //データの記述
            const int dataStartRow = 4;
            sheet.Cell(dataStartRow, 1).Value = "半径/反射率";
            for (var i = 0; i < radiusList.Count; i++)
            {
                var radius = radiusList[i];
                sheet.Cell(dataStartRow + 1 + i, 1).Value = radius;
            }

            for (var i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                sheet.Cell(dataStartRow, 2 + i).Value = step.ReflectionCoefficient.ToString("0.00");
            }

            for (var i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var max = step.Datas.Max(data => data.Count);

                for (var j = 0; j < radiusList.Count; j++)
                {
                    var radius = radiusList[j];
                    if (step.Datas.Any(data => data.Radius == radius))
                        sheet.Cell(dataStartRow + 1 + j, 2 + i).Value = step.Datas.First(data => data.Radius == radius).Count / (double)max;
                }
            }

            book.SaveAs(filename);
        }

        public override List<Series> GetSeries()
        {
            var series = new List<Series>();

            foreach (var step in Steps)
            {
                var s = new LineSeries();
                s.Title = "反射率:" + step.ReflectionCoefficient.ToString("0.0");
                var max = step.Datas.Max(data => data.Count);
                var points = new List<OxyPlot.DataPoint>();
                foreach (var data in step.Datas)
                    points.Add(new OxyPlot.DataPoint(Math.Pow(data.Radius, 2) * Math.PI, data.Count / (double)max));

                s.ItemsSource = points;
                series.Add(s);
            }

            return series;
        }

        public class Step
        {
            public double ReflectionCoefficient;
            public List<(double Radius, long Count)> Datas;
        }
    }
}
