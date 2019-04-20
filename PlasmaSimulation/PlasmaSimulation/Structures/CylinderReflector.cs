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
        public Atom.ReflectionPattern ReflectionPattern { get; }

        public double? ReflectionCoefficient { get; }

        public Collision Collision { get; }

        public CylinderReflector(int id, Vector position, Vector direction, double length, double radius, Atom.ReflectionPattern reflectionPattern = Atom.ReflectionPattern.Specularly, double? reflectionCoefficient = null)
        {
            Length = length;
            Radius = radius;
            Position = position;
            Direction = direction.Normal;
            ReflectionCoefficient = reflectionCoefficient;
            ReflectionPattern = reflectionPattern;
            ID = id;
            Collision = new Collision(ID, reflectionPattern);
        }

        public Structure Copy()
        {
            return new CylinderReflector(ID, Position, Direction, Length, Radius, ReflectionPattern, ReflectionCoefficient);
        }

        public bool SetCollision(Atom atom)
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
            if(double.IsNaN(t1))
            {
                Collision.Disable();
                return false;
            }

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
                var dx = x - Position;
                //移動した先が円筒に含まれていた場合
                if (Dot(dx, Direction) > 0 && Dot(dx - Length * Direction, Direction) < 0)
                {
                    var d = Direction * Dot(Direction, dx);
                    var n = (d - dx) * (1 / Radius);

                    Collision.Set(x, n, t);
                    return true;
                }
                //それ以外
                else
                {
                    Collision.Disable();
                    return false;
                }
            }
            //t1 > 0 && t2 > 0 つまり円筒の外部におり、円筒を突っ切るとき
            else if (t1 > 0 && t2 > 0)
            {
                var x1 = atom.Position + atom.Velocity * Min(t1, t2);
                var x2 = atom.Position + atom.Velocity * Max(t1, t2);
                var dx1 = x1 - Position;
                var dx2 = x2 - Position;

                //円筒に外側から当たったとき
                //これも考慮すべきかも
                if (Dot(dx1, Direction) > 0 && Dot(x1 - (Position + Length * Direction), Direction) < 0)
                {
                    var d = Direction * Dot(Direction, dx1);
                    var n = (d - dx1) * (1 / Radius);
                    Collision.Set(x1, n, Min(t1, t2));
                    return true;
                }
                //円筒が切れているところから内部に入り、円筒に内部から当たるとき
                else if (Dot(dx2, Direction) > 0 && Dot(dx2 - Length * Direction, Direction) < 0)
                {
                    var d = Direction * Dot(Direction, dx2);
                    var n = -1 * (dx2 - d) * (1 / Radius);

                    if (Dot(atom.Velocity, n) < 0)
                    {
                        Collision.Set(x2, n, Max(t1, t2));
                        return true;
                    }
                    Collision.Disable();
                    return false;
                }
                //それ以外
                else
                {
                    Collision.Disable();
                    return false;
                }
            }
            //それ以外
            else
            {
                Collision.Disable();
                return false;
            }
        }
    }

}
