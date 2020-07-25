using PEPlugin.Pmx;
using PEPlugin.SDX;
using SlimDX;
using System;
using System.Drawing;

namespace PEPExtensions
{
    /// <summary>
    /// PEPlugin.SDXのベクトル系クラス用拡張メソッド
    /// </summary>
    public static class VectorExtensions
    {

        //
        // ------- ToArray -------
        //

        /// <summary>
        /// ベクトルの座標を配列<c>float[]</c>に変換する
        /// </summary>
        /// <param name="v">変換対象ベクトル</param>
        /// <returns>座標値の配列</returns>
        public static float[] ToArray(this V3 v)
        {
            return new float[] { v.X, v.Y, v.Z };
        }

        /// <summary>
        /// ベクトルの座標を配列<c>float[]</c>に変換する
        /// </summary>
        /// <param name="v">変換対象ベクトル</param>
        /// <returns>座標値の配列</returns>
        public static float[] ToArray(this V2 v)
        {
            return new float[] { v.X, v.Y };
        }

        //
        // ------- ToPoint -------
        //

        /// <summary>
        /// ベクトルの座標を<c>PointF</c>に変換する
        /// </summary>
        /// <param name="vertex">変換対象ベクトル</param>
        /// <returns>点</returns>
        public static PointF ToPointF(this V2 vertex)
        {
            return new PointF(vertex.X, vertex.Y);
        }

        /// <summary>
        /// ベクトルの座標を拡縮して<c>PointF</c>に変換する
        /// </summary>
        /// <param name="vertex">変換対象ベクトル</param>
        /// <param name="Width">横倍率</param>
        /// <param name="Height">縦倍率</param>
        /// <returns></returns>
        public static PointF ToPointF(this V2 vertex, int Width, int Height)
        {
            return new PointF(vertex.X * Width, vertex.Y * Height);
        }

        /// <summary>
        /// ベクトルの座標を<c>Point</c>に変換する
        /// </summary>
        /// <param name="vertex">変換対象ベクトル</param>
        /// <returns>点</returns>
        public static Point ToPoint(this V2 vertex)
        {
            return new Point((int)Math.Round(vertex.X, MidpointRounding.AwayFromZero), (int)Math.Round(vertex.Y, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// ベクトルの座標を拡縮して<c>Point</c>に変換する
        /// </summary>
        /// <param name="vertex">変換対象ベクトル</param>
        /// <param name="Width">横倍率</param>
        /// <param name="Height">縦倍率</param>
        /// <returns></returns>
        public static Point ToPoint(this V2 vertex, int Width, int Height)
        {
            return new Point((int)Math.Round(vertex.X * Width, MidpointRounding.AwayFromZero), (int)Math.Round(vertex.Y * Height, MidpointRounding.AwayFromZero));
        }

        //
        // ------- 計算 -------
        //

        /// <summary>
        /// ベクトルにスカラーをかける
        /// </summary>
        /// <typeparam name="T">!! *演算子が実装されていること!! dynamicにキャストされる</typeparam>
        /// <param name="v">ベクトル</param>
        /// <param name="num">数値 *演算子が実装されている必要がある</param>
        public static void Times<T>(this V2 v, T num)
        {
            v.X *= (dynamic)num;
            v.Y *= (dynamic)num;
        }

        /// <summary>
        /// ベクトルにスカラーをかける
        /// </summary>
        /// <typeparam name="T">!! *演算子が実装されていること!! dynamicにキャストされる</typeparam>
        /// <param name="v">ベクトル</param>
        /// <param name="num">数値 *演算子が実装されている必要がある</param>
        public static void Times<T>(this V3 v, T num)
        {
            v.X *= (dynamic)num;
            v.Y *= (dynamic)num;
            v.Z *= (dynamic)num;
        }

        /// <summary>
        /// 内積
        /// </summary>
        public static float Dot(this V3 left, V3 right) => Vector3.Dot(left, right);

        /// <summary>
        /// 内積
        /// </summary>
        public static float Dot(this V2 left, V2 right) => Vector2.Dot(left, right);

        /// <summary>
        /// 外積
        /// </summary>
        public static V3 Cross(this V3 left, V3 right) => Vector3.Cross(left, right);

        /// <summary>
        /// 外積
        /// 3次元に変換される
        /// </summary>
        public static V3 Cross(this V2 left, V2 right) => Vector3.Cross(new Vector3(left.ToVector2(), 0.0f), new Vector3(right.ToVector2(), 0.0f));

        //
        // ------- 判定 -------
        //

        /// <summary>
        /// 指定座標が面のUV座標内であるかどうかを判定する
        /// </summary>
        public static bool UVIsInclude(this IPXFace face, V2 point)
        {
            // 外積の符号が全て等しいかの変数
            bool allUpper0 = true;
            bool allLower0 = true;

            var uv = face.ExtructUV();

            // 辺ベクトルを得る
            var V1V2 = uv[1] - uv[0];
            var V2V3 = uv[2] - uv[1];
            var V3V1 = uv[0] - uv[2];

            // 外積の方向を調査
            allUpper0 &= V1V2.Cross(point - uv[1]).Z >= 0.0f;
            allLower0 &= V1V2.Cross(point - uv[1]).Z <= 0.0f;

            allUpper0 &= V2V3.Cross(point - uv[2]).Z >= 0.0f;
            allLower0 &= V2V3.Cross(point - uv[2]).Z <= 0.0f;

            allUpper0 &= V3V1.Cross(point - uv[0]).Z >= 0.0f;
            allLower0 &= V3V1.Cross(point - uv[0]).Z <= 0.0f;

            // 外積のZ座標の符号が全て等しければ内側
            return allUpper0 | allLower0;
        }

        //
        // ------- 変換 -------
        //

        /// <summary>
        /// 面を構成する頂点のUV座標を返す
        /// </summary>
        /// <param name="face">対象面</param>
        /// <returns>2次元ベクトル配列</returns>
        public static V2[] ExtructUV(this IPXFace face)
        {
            return new V2[]
            {
                face.Vertex1.UV,
                face.Vertex2.UV,
                face.Vertex3.UV
            };
        }

        /// <summary>
        /// UV座標の境界領域を計算する
        /// </summary>
        /// <param name="face">対象面</param>
        /// <returns>UV座標の境界領域</returns>
        public static (V2 min, V2 max) ComputeUVBoundingBox(this IPXFace face)
        {
            (V2 min, V2 max) bb = (new V2(float.MaxValue, float.MaxValue), new V2(float.MinValue, float.MinValue));

            foreach (var uv in face.ExtructUV())
            {
                bb.min.X = bb.min.X < uv.X ? bb.min.X : uv.X;
                bb.min.Y = bb.min.Y < uv.Y ? bb.min.Y : uv.Y;

                bb.max.X = bb.max.X > uv.X ? bb.max.X : uv.X;
                bb.max.Y = bb.max.Y > uv.Y ? bb.max.Y : uv.Y;
            }

            return bb;
        }

        /// <summary>
        /// V2のタプルから四角形構造体に変換する
        /// </summary>
        /// <param name="tuple">変換対象タプル</param>
        /// <returns>四角形</returns>
        public static RectangleF ToRectangle(this (V2 min, V2 max) tuple) => new RectangleF(tuple.min.X, tuple.min.Y, tuple.max.X - tuple.min.X, tuple.max.Y - tuple.min.Y);

        //
        // ------- 表示 -------
        //

        /// <summary>
        /// ベクトルの座標を表示する文字列を返す
        /// </summary>
        /// <param name="v">表示する点</param>
        /// <param name="format">書式</param>
        /// <returns>"(X座標, Y座標)"</returns>
        public static string Print(this V2 v, string format = "") => $"({v.X.ToString(format)}, {v.Y.ToString(format)})";
        /// <summary>
        /// ベクトルの座標を表示する文字列を返す
        /// </summary>
        /// <param name="v">表示する点</param>
        /// <param name="format">書式</param>
        /// <returns>"(X座標, Y座標, Z座標)"</returns>
        public static string Print(this V3 v, string format = "") => $"({v.X.ToString(format)}, {v.Y.ToString(format)}), {v.Z.ToString(format)}";
    }
}
