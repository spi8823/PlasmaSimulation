using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaSimulation
{
    public abstract class Geometry
    {
        /// <summary>
        /// 反射回数の上限
        /// </summary>
        public int ReflectionLimit { get; set; }

        public double ReflectionCoefficient { get; set; }
        
        [Newtonsoft.Json.JsonIgnore()]
        /// <summary>
        /// 構造体の配列
        /// </summary>
        public Structure[] Structures { get; }

        /// <summary>
        /// 反射のパターン
        /// Structureに持たせたほうがいいかも？
        /// </summary>
        public Atom.ReflectionPattern ReflectionPattern { get; set; }

        /// <summary>
        /// コンパイラ
        /// </summary>
        /// <param name="limit"></param>
        protected Geometry(int limit, double reflectionCoefficient, Atom.ReflectionPattern pattern, Structure[] structures)
        {
            ReflectionLimit = limit;
            ReflectionCoefficient = reflectionCoefficient;
            ReflectionPattern = pattern;
            Structures = structures;
        }

        /// <summary>
        /// しかるべき初期化をしたAtomを生成
        /// </summary>
        /// <param name="random"></param>
        /// <returns>初期化したAtom</returns>
        public abstract Atom CreateAtomRandomly(Random random);

        public Atom CreateAtomRandomly() => CreateAtomRandomly(new Random());

        public List<Vector?> GetTrack() => GetTrack(CreateAtomRandomly(), new Random());

        /// <summary>
        /// ジオメトリの中でAtomに対して一連の処理をした結果を返す
        /// </summary>
        /// <param name="atom"></param>
        /// <returns>処理結果</returns>
        public virtual List<Vector?> GetTrack(Atom atom, Random random)
        {
            var track = new List<Vector?>();
            //反射回数が上限に達するまで回す
            var count = 0;
            while (count++ < ReflectionLimit)
            {
                track.Add(atom.Position);
                //構造物とAtomの衝突情報
                var collisions = (from s
                                 in Structures
                                 let c = s.GetCollision(atom)
                                 where c != null
                                 select c.Value).ToList();

                //衝突しなかったらどっかに行く
                if (!collisions.Any())
                {
                    track.Add(atom.Position + atom.Velocity * 10);
                    track.Add(null);
                    return track;
                }

                var minTime = collisions.Min(c => c.Time);
                var collision = collisions.First(c => c.Time == minTime);

                //衝突する
                atom.Update(collision.Time);
                atom.Reflect(collision.Normal, ReflectionPattern);

                //Targetと衝突したらおしまい
                if (ShouldTerminate(collision) || random.NextDouble() > ReflectionCoefficient)
                {
                    track.Add(collision.Position);
                    return track;
                }
            }
            return track;
        }

        /// <summary>
        /// ジオメトリの中でAtomに対して一連の処理をした結果を返す
        /// </summary>
        /// <param name="atom"></param>
        /// <returns>処理結果</returns>
        protected virtual Vector? GetResult(Atom atom, Random random)
        {
            //反射回数が上限に達するまで回す
            var count = 0;
            while (count++ < ReflectionLimit)
            {
                //構造物とAtomの衝突情報
                var collisions = (from s
                                 in Structures
                                 let c = s.GetCollision(atom)
                                 where c != null
                                 select c.Value).ToList();

                //衝突しなかったらどっかに行く
                if (!collisions.Any())
                    return null;

                var minTime = collisions.Min(c => c.Time);
                var collision = collisions.First(c => c.Time == minTime);


                //衝突する
                atom.Update(collision.Time);
                atom.Reflect(collision.Normal, ReflectionPattern);

                if (ShouldTerminate(collision) || (random.NextDouble() > ReflectionCoefficient))
                    return atom.Position;
            }
            return null;
        }

        protected abstract bool ShouldTerminate(Collision collision);

        /// <summary>
        /// ジオメトリの複製
        /// 並列処理のため
        /// </summary>
        /// <returns>複製したもの</returns>
        protected abstract Geometry Copy();

        /// <summary>
        /// 並列に処理する
        /// </summary>
        /// <param name="geometry">処理するジオメトリ</param>
        /// <param name="count">回数</param>
        /// <returns>処理結果</returns>
        public virtual List<Vector?> ProcessAsParallel(long count)
        {
            var random = new Random();
            var array = new(Geometry geometry, Atom atom, Random random)[count];
            for(var i = 0;i < count;i++)
            {
                array[i].geometry = Copy();
                array[i].atom = CreateAtomRandomly(random);
                array[i].random = new Random(random.Next());
            }

            var result = from item
                         in array.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                         select item.geometry.GetResult(item.atom, item.random);

            return result.ToList();
        }

        /// <summary>
        /// 並列じゃない
        /// 遅いぞ！！！
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual List<Vector?> Process(long count)
        {
            var random = new Random();
            var result = new List<Vector?>();

            for(var i = 0;i < count;i++)
            {
                var atom = CreateAtomRandomly(random);
                result.Add(GetResult(atom, random));
            }

            return result;
        }
    }
}
