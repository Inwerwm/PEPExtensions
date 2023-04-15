using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPExtensions
{
    /// <summary>
    /// IEnumerableの拡張メソッド
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 対象のコレクションの全要素を自身も格納しているかを調べる
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="basis">比較元コレクション</param>
        /// <param name="collection">比較先コレクション</param>
        /// <returns>すべての要素を含んでいるか</returns>
        public static bool Contains<T>(this IEnumerable<T> basis, IEnumerable<T> collection) => collection.Aggregate(true, (sum, elm) => sum && basis.Contains(elm));

        /// <summary>
        /// 指定したコレクションの要素を全て加える
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="basis">元リスト</param>
        /// <param name="collection">追加コレクション</param>
        public static void AddRange<T>(this IList<T> basis, IEnumerable<T> collection)
        {
            foreach (var elm in collection)
            {
                basis.Add(elm);
            }
        }
    }
}
