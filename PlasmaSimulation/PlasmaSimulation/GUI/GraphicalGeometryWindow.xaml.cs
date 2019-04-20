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
using static PlasmaSimulation.Extensions;
using PlasmaSimulation.Structures;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace PlasmaSimulation.GUI
{
    /// <summary>
    /// GraphicalGeometryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class GraphicalGeometryWindow : Window
    {
        private Geometry Geometry { get; }
        public Point3D CameraPosition
        {
            get { return Viewport.Camera.Position; }
            set { Viewport.Camera.Position = value; }
        }

        public GraphicalGeometryWindow(Geometry geometry)
        {
            InitializeComponent();

            Geometry = geometry;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var count = 10000;
            var result = Geometry.ProcessAsParallel(count);
            ProcessResultLabel.Content = result.LongCount(r => r.IsValid) + " / " + count;
            ShowGeometry();
            ShowAtomTrack();
        }

        /// <summary>
        /// Geometryを見せる
        /// </summary>
        private void ShowGeometry()
        {
            foreach (var structure in Geometry.Structures.Reverse())
            {
                var model = GetModel(structure);
                if (model == null)
                    continue;
                var mv = new ModelVisual3D();
                mv.Content = model;
                Viewport.Children.Add(mv);
            }
        }

        /// <summary>
        /// 構造体を表す3Dモデルを取得する
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        private Model3D GetModel(Structure structure)
        {
            var importer = new ModelImporter();
            Model3D model = null;
            Vector scale;

            if (structure is CylinderReflector cylinder)
            {
                model = importer.Load(@"Models\Cylinder.obj");
                scale = new Vector(cylinder.Radius, cylinder.Radius, cylinder.Length);
            }
            else if (structure is Shield shield)
            {
                model = importer.Load(@"Models\Shield.obj");
                scale = new Vector(shield.Radius, shield.Radius, 1);
            }
            else if (structure is Hole hole)
            {
                model = importer.Load(@"Models\Hole.obj");
                scale = new Vector(hole.Radius ?? 0, hole.Radius ?? 0, 1);
            }
            else if(structure is Plate plate)
            {
                model = importer.Load(@"Models\Plate.obj");
                scale = new Vector(plate.HorizontalVector.Length, plate.VerticalVector.Length, 1);
            }
            else
                throw new NotImplementedException();

            var transform = GetTransform(structure.Position, structure.Direction, scale);
            model.Transform = transform;
            return model;
        }

        /// <summary>
        /// Atomの飛跡を見せる
        /// </summary>
        private void ShowAtomTrack()
        {
            AtomTrack.Children.Clear();
            var track = Geometry.GetTrack();

            for (var i = 0; i < track.Count; i++)
            {
                if (track.Last() == null && i == track.Count - 2)
                {
                    break;
                }

                AtomTrack.Children.Add(GetSphere(track[i].Value));
            }

            AtomTrack.Children.Add(
                GetLines(
                    (
                        from p
                        in track
                        where p != null
                        select p.Value
                    ).ToList()
                )
            );
        }

        /// <summary>
        /// 描画用の球を取得する
        /// </summary>
        /// <param name="position">位置</param>
        /// <returns></returns>
        private SphereVisual3D GetSphere(Vector position)
        {
            var sphere = new SphereVisual3D();
            sphere.Material = new DiffuseMaterial(Brushes.Red);
            sphere.Transform = GetTransform(position, Vector.Forward, Vector.One * 0.25);
            return sphere;
        }

        /// <summary>
        /// 描画用の線を取得する
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        private LinesVisual3D GetLines(List<Vector> track)
        {
            var lines = new LinesVisual3D();

            for(var i = 0;i < track.Count - 1;i++)
            {
                var position1 = track[i];
                var point1 = new Point3D()
                {
                    X = position1.X,
                    Y = position1.Y,
                    Z = position1.Z,
                };
                lines.Points.Add(point1);

                var position2 = track[i + 1];
                var point2 = new Point3D()
                {
                    X = position2.X,
                    Y = position2.Y,
                    Z = position2.Z,
                };
                lines.Points.Add(point2);
            }

            lines.Transform = GetTransform(Vector.Zero, Vector.Forward, Vector.One);

            return lines;
        }

        /// <summary>
        /// 適切なTransformを取得する
        /// Transformを使うときはこれを噛ませる
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private MatrixTransform3D GetTransform(Vector position, Vector direction, Vector scale)
        {
            var matrix = Matrix3D.Identity;
            matrix.Scale(scale.Vector3D);

            var cross = Cross(new Vector(0, 0, 1), direction);
            var sign = Math.Sign(Dot(new Vector(0, 0, 1), direction));
            var angle = Math.Asin(cross.Length) * 180 / Math.PI;
            angle = sign > 0 ? angle : (180 - angle);
            if (cross.Length != 0)
                matrix.Rotate(new Quaternion(cross.Normal.Vector3D, angle));

            matrix.Translate(position.Vector3D);

            //Viewportの座標系に合わせるために座標変換
            matrix = FixCoordinate(matrix);

            return new MatrixTransform3D(matrix);
        }

        /// <summary>
        /// 自前の座標系をViewport3Dの座標系に直すための変換
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private Matrix3D FixCoordinate(Matrix3D matrix)
        {
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), 90));
            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), -90));
            return matrix;
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAtomTrack();
        }
    }
}
