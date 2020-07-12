using PEPlugin.Pmx;
using PEPlugin.SDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPExtensions
{
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

        public enum Interval
        {
            Open,
            Close
        }

        /// <summary>
        /// 指定した区間の範囲内であるかを判断します。
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
        /// <param name="name">名前</param>
        public static string ToStringName(this object value, string name)
        {
            return $"{name} = {value}";
        }

        /// <summary>
        /// 「名前 = 値↲」の形で出力する
        /// </summary>
        /// <param name="name">名前</param>
        public static string ToStringNL(this object value, string name)
        {
            return $"{name} = {value.ToStringLine()}";
        }

        public static float[] ToArray(this V3 v)
        {
            return new float[] { v.X, v.Y, v.Z };
        }

        public static float[] ToArray(this V2 v)
        {
            return new float[] { v.X, v.Y };
        }

        public static PointF ToPointF(this V2 vertex)
        {
            return new PointF(vertex.X, vertex.Y);
        }

        public static PointF ToPointF(this V2 vertex, int Width, int Height)
        {
            return new PointF(vertex.X * Width, vertex.Y * Height);
        }

        public static PointF[] ToPointF(this IPXFace face)
        {
            return new PointF[3] { face.Vertex1.UV.ToPointF(), face.Vertex2.UV.ToPointF(), face.Vertex3.UV.ToPointF() };
        }

        public static PointF[] ToPointF(this IPXFace face, int Width, int Height)
        {
            return new PointF[3] { face.Vertex1.UV.ToPointF(Width, Height), face.Vertex2.UV.ToPointF(Width, Height), face.Vertex3.UV.ToPointF(Width, Height) };
        }

        public static Point ToPoint(this V2 vertex)
        {
            return new Point((int)Math.Round(vertex.X, MidpointRounding.AwayFromZero), (int)Math.Round(vertex.Y, MidpointRounding.AwayFromZero));
        }

        public static Point ToPoint(this V2 vertex, int Width, int Height)
        {
            return new Point((int)Math.Round(vertex.X * Width, MidpointRounding.AwayFromZero), (int)Math.Round(vertex.Y * Height, MidpointRounding.AwayFromZero));
        }

        public static Point[] ToPoint(this IPXFace face)
        {
            return new Point[3] { face.Vertex1.UV.ToPoint(), face.Vertex2.UV.ToPoint(), face.Vertex3.UV.ToPoint() };
        }

        public static Point[] ToPoint(this IPXFace face, int Width, int Height)
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

        public static string Print(this Point point) => $"({point.X}, {point.Y})";
        public static string Print(this PointF point) => $"({point.X}, {point.Y})";

        public static V2[] ExtructUV(this IPXFace face)
        {
            return new V2[]
            {
                face.Vertex1.UV,
                face.Vertex2.UV,
                face.Vertex3.UV
            };
        }

        public static string Print(this V2 v) => $"({v.X}, {v.Y})";
        public static string Print(this V3 v) => $"({v.X}, {v.Y}), {v.Z}";
        public static string PrintUV(this IPXFace f) => $"{f.Vertex1.UV.Print()}, {f.Vertex2.UV.Print()}, {f.Vertex3.UV.Print()}";
    }
}
