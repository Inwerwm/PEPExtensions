using PEPlugin.Pmx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPExtensions
{
    /// <summary>
    /// 拡張メソッド
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 指定された閉区間の範囲内であるかを判断します。
        /// </summary>
        /// <param name="i"></param>
        /// <param name="lower">下限（自身も含む）</param>
        /// <param name="upper">上限（自身も含む）</param>
        /// <returns></returns>
        public static bool IsWithin<T>(this T i, T lower, T upper) where T : IComparable
        {
            if (upper.CompareTo(lower) < 0)
                throw new ArgumentOutOfRangeException("IsWithin<T>:下限値が上限値よりも大きいです。");
            return i.CompareTo(lower) * upper.CompareTo(i) >= 0;
        }

        /// <summary>
        /// 指定された開区間の範囲内であるかを判断します。
        /// </summary>
        /// <param name="i"></param>
        /// <param name="lower">下限（自身は含まない）</param>
        /// <param name="upper">上限（自身は含まない）</param>
        /// <returns></returns>
        public static bool IsInside<T>(this T i, T lower, T upper) where T : IComparable
        {
            if (upper.CompareTo(lower) < 0)
                throw new ArgumentOutOfRangeException("IsInside<T>:下限値が上限値よりも大きいです。");
            return i.CompareTo(lower) * upper.CompareTo(i) > 0;
        }

        /// <summary>
        /// 区間種別
        /// </summary>
        public enum Interval
        {
            /// <summary>
            /// 開区間
            /// </summary>
            Open,
            /// <summary>
            /// 閉区間
            /// </summary>
            Close
        }

        /// <summary>
        /// 指定した区間の範囲内であるかを判断します。
        /// 閉区間：自身を含む
        /// 開区間：自身を含まない
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="lower">下限</param>
        /// <param name="lowerInterval">左側の開閉</param>
        /// <param name="upper">上限</param>
        /// <param name="upperInterval">右側の開閉</param>
        /// <returns></returns>
        public static bool IsInRangeOf<T>(this T i, T lower, Interval lowerInterval, T upper, Interval upperInterval) where T : IComparable
        {
            if (upper.CompareTo(lower) < 0)
                throw new ArgumentOutOfRangeException("IsInRangeOf<T>:下限値が上限値よりも大きいです。");

            var l = i.CompareTo(lower);
            bool isInRangeLower = lowerInterval == Interval.Close ? l >= 0 : l > 0;
            var u = upper.CompareTo(i);
            bool isInRangeUpper = upperInterval == Interval.Close ? u >= 0 : u > 0;

            return isInRangeLower && isInRangeUpper;
        }

        /// <summary>
        /// stringに変換して末尾に改行文字を加える
        /// </summary>
        public static string ToStringLine(this object value)
        {
            return value.ToString() + Environment.NewLine;
        }

        /// <summary>
        /// 「名前 = 値」の形で出力する
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">名前</param>
        public static string ToStringName(this object value, string name)
        {
            return $"{name} = {value}";
        }

        /// <summary>
        /// 「名前 = 値↲」の形で出力する
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">名前</param>
        public static string ToStringNL(this object value, string name)
        {
            return $"{name} = {value.ToStringLine()}";
        }

        /// <summary>
        /// 真偽値を○✕に変換
        /// </summary>
        /// <param name="val">真偽値</param>
        /// <returns>true:○, false:✕</returns>
        public static string ToSign(this bool val) => val ? "○" : "✕";

        /// <summary>
        /// 面を構成する頂点のUV座標の配列に変換する
        /// </summary>
        /// <param name="face">変換対象面</param>
        /// <returns>点の配列</returns>
        public static PointF[] UVToPointF(this IPXFace face)
        {
            return new PointF[3] { face.Vertex1.UV.ToPointF(), face.Vertex2.UV.ToPointF(), face.Vertex3.UV.ToPointF() };
        }

        /// <summary>
        /// 面を構成する頂点のUV座標の配列に拡縮して変換する
        /// </summary>
        /// <param name="face">変換対象面</param>
        /// <param name="Width">横倍率</param>
        /// <param name="Height">縦倍率</param>
        /// <returns>点の配列</returns>
        public static PointF[] UVToPointF(this IPXFace face, int Width, int Height)
        {
            return new PointF[3] { face.Vertex1.UV.ToPointF(Width, Height), face.Vertex2.UV.ToPointF(Width, Height), face.Vertex3.UV.ToPointF(Width, Height) };
        }

        /// <summary>
        /// 面を構成する頂点のUV座標の配列に変換する
        /// </summary>
        /// <param name="face">変換対象面</param>
        /// <returns>点の配列</returns>
        public static Point[] UVToPoint(this IPXFace face)
        {
            return new Point[3] { face.Vertex1.UV.ToPoint(), face.Vertex2.UV.ToPoint(), face.Vertex3.UV.ToPoint() };
        }

        /// <summary>
        /// 面を構成する頂点のUV座標の配列に拡縮して変換する
        /// </summary>
        /// <param name="face">変換対象面</param>
        /// <param name="Width">横倍率</param>
        /// <param name="Height">縦倍率</param>
        /// <returns>点の配列</returns>
        public static Point[] UVToPoint(this IPXFace face, int Width, int Height)
        {
            return new Point[3] { face.Vertex1.UV.ToPoint(Width, Height), face.Vertex2.UV.ToPoint(Width, Height), face.Vertex3.UV.ToPoint(Width, Height) };
        }

        /// <summary>
        /// 四捨五入で整数化
        /// </summary>
        public static int Round(this float value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);

        /// <summary>
        /// 四捨五入で整数化
        /// </summary>
        public static int Round(this double value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);

        /// <summary>
        /// 四捨五入で整数化
        /// </summary>
        public static int Round(this decimal value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);

        /// <summary>
        /// 点の座標を表示する文字列を返す
        /// </summary>
        /// <param name="point">表示する点</param>
        /// <param name="format">書式</param>
        /// <returns>"(X座標, Y座標)"</returns>
        public static string Print(this Point point, string format = "") => $"({point.X.ToString(format)}, {point.Y.ToString(format)})";
        /// <summary>
        /// 点の座標を表示する文字列を返す
        /// </summary>
        /// <param name="point">表示する点</param>
        /// <param name="format">書式</param>
        /// <returns>"(X座標, Y座標)"</returns>
        public static string Print(this PointF point, string format = "") => $"({point.X.ToString(format)}, {point.Y.ToString(format)})";

        /// <summary>
        /// 面を構成する頂点のUV座標を表示する文字列を返す
        /// </summary>
        /// <param name="f">表示する面</param>
        /// <param name="format">書式</param>
        /// <returns>"(U座標, V座標), (U座標, V座標), (U座標, V座標)"</returns>
        public static string PrintUV(this IPXFace f, string format = "") => $"{f.Vertex1.UV.Print(format)}, {f.Vertex2.UV.Print(format)}, {f.Vertex3.UV.Print(format)}";
    }
}
