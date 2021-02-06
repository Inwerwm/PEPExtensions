using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PEPlugin.Pmx;

namespace PEPExtensions
{
    /// <summary>
    /// IPX系の拡張メソッド
    /// </summary>
    public static class IPXExtensions
    {
        /// <summary>
        /// 頂点配列に変換
        /// </summary>
        /// <param name="face">変換元面</param>
        /// <returns>面を構成する頂点の配列</returns>
        public static IPXVertex[] ToVertices(this IPXFace face) => new IPXVertex[] { face.Vertex1, face.Vertex2, face.Vertex3 };
        /// <summary>
        /// <para>頂点インデックス配列に変換</para>
        /// <para>使用数が多いようなら材質のFaceToVertexIndices拡張メソッドの使用を検討すること</para>
        /// <para>頂点インデックス辞書の作成分のオーバーヘッドが発生するため</para>
        /// </summary>
        /// <param name="face">変換元面</param>
        /// <param name="pmx">頂点を内包したPMXデータ</param>
        /// <returns>面を構成する頂点のインデックス配列</returns>
        public static int[] ToVertexIndices(this IPXFace face, IPXPmx pmx)
        {
            var vtxIds = pmx.Vertex.Select((v, i) => (v, i)).ToDictionary(p => p.v, p => p.i);
            return new int[] { vtxIds[face.Vertex1], vtxIds[face.Vertex2], vtxIds[face.Vertex3] };
        }
        /// <summary>
        /// 材質内のすべての面を頂点インデックス配列に変換
        /// </summary>
        /// <param name="material">変換する諸面の材質</param>
        /// <param name="pmx">頂点を内包したPMXデータ</param>
        /// <returns>材質内の全ての面を構成する頂点のインデックス配列</returns>
        public static IEnumerable<int[]> FaceToVertexIndices(this IPXMaterial material, IPXPmx pmx)
        {
            var vtxIds = pmx.Vertex.Select((v, i) => (v, i)).ToDictionary(p => p.v, p => p.i);
            foreach (var face in material.Faces)
            {
                yield return new int[] { vtxIds[face.Vertex1], vtxIds[face.Vertex2], vtxIds[face.Vertex3] };
            }
        }
    }
}
