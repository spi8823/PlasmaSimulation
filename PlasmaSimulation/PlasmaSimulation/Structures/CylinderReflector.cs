using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static PlasmaSimulation.Extensions;

namespace PlasmaSimulation
{
    /// <summary>
    /// 円筒型の構造物
    /// </summary>
    public struct CylinderReflector : Structure
    {
        public int ID { get; }
        public double Length { get; }
        public double Radius { get; }
        public Vector Position { get; }
        public Vector Direction { get; }

        public CylinderReflector(double length, double radius, Vector position, Vector direction, int id)
        {
            Length = length;
            Radius = radius;
            Position = position;
            Direction = direction;
            ID = id;
        }

        public Collision? GetCollision(Atom atom)
        {
            //Direction x (atom.Position + atom.Velocity * t - Position) = r
            //賢い！！！
            var dxv = Cross(Direction, atom.Velocity);
            var dxp = Cross(Direction, atom.Position);
            var dxq = Cross(Direction, Position);

            var a = Dot(dxv, dxv);
            var b = 2 * (Dot(dxv, dxp) - Dot(dxv, dxq));
            var c = Dot(dxp, dxp) + Dot(dxq, dxq) - 2 * Dot(dxp, dxq) - Pow(Radius, 2);

            //2次方程式の解
            var e = Sqrt(b * b - 4 * a * c);
            var t1 = (-b + e) / (2 * a);
            var t2 = (-b - e) / (2 * a);

            if (Abs(t1) < RoundingValue)
                t1 = 0;
            if (Abs(t2) < RoundingValue)
                t2 = 0;

            //t1 * t2 < 0 つまり円筒の内部にいた場合
            //いずれかがちょうど0のときもここ
            if (t1 * t2 < 0 || (t1 * t2 == 0 && Max(t1, t2) > 0))
            {
                var t = Max(t1, t2);
                var x = atom.Position + atom.Velocity * t;

                //移動した先が円筒に含まれていた場合
                if (Dot(x - Position, Direction) > 0 && Dot(x - (Position + Length * Direction), Direction) < 0)
                {
                    var dx = x - Position;
                    var d = Direction * Dot(Direction, x - Position);
                    var n = (Position + d - x) * (1 / Radius);

                    return new Collision(x, n, t, ID);
                }
                //それ以外
                else
                    return null;
            }
            //t1 > 0 && t2 > 0 つまり円筒の外部におり、円筒を突っ切るとき
            else if (t1 > 0 && t2 > 0)
            {
                var x1 = atom.Position + atom.Velocity * Min(t1, t2);
                var x2 = atom.Position + atom.Velocity * Max(t1, t2);

                //円筒に外側から当たったとき
                //これも考慮すべきかも
                if (Dot(x1 - Position, Direction) > 0 && Dot(x1 - (Position + Length * Direction), Direction) < 0)
                    return null;
                //円筒が切れているところから内部に入り、円筒に内部から当たるとき
                else if (Dot(x2 - Position, Direction) > 0 && Dot(x2 - (Position + Length * Direction), Direction) < 0)
                {
                    var dx = x2 - Position;
                    var d = Direction * Dot(Direction, dx);
                    var n = -1 * (dx - d) * (1 / Radius);

                    return new Collision(x2, n, t2, ID);
                }
                //それ以外
                else
                    return null;
            }
            //それ以外
            else
                return null;
        }
    }

}
