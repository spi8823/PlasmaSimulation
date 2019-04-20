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
        /// <param name="reflectionLimit"></param>
        protected Geometry(int reflectionLimit, double reflectionCoefficient, Atom.ReflectionPattern pattern, Structure[] structures)
        {
            ReflectionLimit = reflectionLimit;
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
            var structureCount = Structures.Length;
            while (count++ < ReflectionLimit)
            {
                track.Add(atom.Position);

                Collision collision = null;
                for (var i = 0; i < structureCount;i++)
                {
                    Structures[i].SetCollision(atom);
                    var c = Structures[i].Collision;
                    if (!c.IsValid || double.IsNaN(c.Time))
                        continue;
                    if (collision == null)
                    {
                        collision = c;
                        continue;
                    }
                    if (c.Time < collision.Time)
                        collision = c;
                }
                if(collision == null)
                {
                    track.Add(atom.Position + atom.Velocity * 10);
                    track.Add(null);
                    return track;
                }

                atom.Update(collision.Time);
                atom.Reflect(collision.Normal, collision.ReflectionPattern, random);

                //Targetと衝突したらおしまい
                if (OnCollision(atom, collision) || random.NextDouble() > (Structures[collision.StructureID].ReflectionCoefficient ?? ReflectionCoefficient))
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
        protected virtual bool GetResult(Atom atom, Random random)
        {
            atom.ReflectionCount = 0;
            while (atom.ReflectionCount < ReflectionLimit)
            {
                Collision collision = null;
                for(var i = 0;i < Structures.Length;i++)
                {
                    if (!Structures[i].SetCollision(atom))
                        continue;
                    var c = Structures[i].Collision;

                    if (collision == null)
                    {
                        collision = c;
                        continue;
                    }
                    if (c.Time < collision.Time)
                    {
                        collision = c;
                    }
                }

                if (collision == null || double.IsNaN(collision.Time))
                {
                    atom.IsValid = false;
                    return false;
                }

                atom.Update(collision.Time);
                atom.Reflect(collision.Normal, collision.ReflectionPattern, random);

                if(OnCollision(atom, collision))
                {
                    atom.IsValid = true;
                    return true;
                }

                if(random.NextDouble() > (Structures[collision.StructureID].ReflectionCoefficient ?? ReflectionCoefficient))
                {
                    atom.IsValid = false;
                    return false;
                }
            }
            return false;
        }

        protected abstract bool OnCollision(Atom atom, Collision collision);

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
        public virtual List<Atom> ProcessAsParallel(int count)
        {
            var seed = (int)DateTime.Now.Ticks;
            var list = (from i
                        in ParallelEnumerable.Range(0, count).WithDegreeOfParallelism(Environment.ProcessorCount - 1)
                        let geometry = Copy()
                        let random = new Random(i + seed)
                        let atom = geometry.CreateAtomRandomly(random)
                        let result = geometry.GetResult(atom, random)
                        select atom).ToList();
            
            return list;
        }

        /// <summary>
        /// 並列じゃない
        /// 遅いぞ！！！
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual List<Atom> Process(int count)
        {
            var random = new Random();
            return (from i in Enumerable.Range(0, count)
                    let atom = CreateAtomRandomly(random)
                    let result = GetResult(atom, random)
                    select atom).ToList();
        }
    }
}