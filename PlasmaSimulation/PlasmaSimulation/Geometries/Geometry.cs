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
        protected int ReflectionLimit { get; }

        /// <summary>
        /// 反射のパターン
        /// Structureに持たせたほうがいいかも？
        /// </summary>
        protected Atom.ReflectionPattern ReflectionPattern { get; }

        /// <summary>
        /// コンパイラ
        /// </summary>
        /// <param name="limit"></param>
        protected Geometry(int limit, Atom.ReflectionPattern pattern)
        {
            ReflectionLimit = limit;
            ReflectionPattern = pattern;
        }

        /// <summary>
        /// しかるべき初期化をしたAtomを生成
        /// </summary>
        /// <param name="random"></param>
        /// <returns>初期化したAtom</returns>
        protected abstract Atom CreateAtomRandomly(Random random);

        /// <summary>
        /// ジオメトリの中でAtomに対して一連の処理をした結果を返す
        /// </summary>
        /// <param name="atom"></param>
        /// <returns>処理結果</returns>
        protected abstract Vector? GetResult(Atom atom);

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
        public static List<Vector?> ProcessAsParallel(Geometry geometry, long count)
        {
            var random = new Random();
            var array = new(Geometry geometry, Atom atom)[count];
            for(var i = 0;i < count;i++)
            {
                array[i].geometry = geometry.Copy();
                array[i].atom = geometry.CreateAtomRandomly(random);
            }

            var result = from item
                         in array.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                         select item.geometry.GetResult(item.atom);

            return result.ToList();
        }

        /// <summary>
        /// 並列じゃない
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<Vector?> Process(Geometry geometry, long count)
        {
            var random = new Random();
            var result = new List<Vector?>();

            for(var i = 0;i < count;i++)
            {
                var atom = geometry.CreateAtomRandomly(random);
                result.Add(geometry.GetResult(atom));
            }

            return result;
        }
    }
}
