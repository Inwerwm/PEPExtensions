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
        /// 頂点インデックス配列に変換
        /// </summary>
        /// <param name="face">変換元面</param>
        /// <param name="pmx">頂点を内包したPMXデータ</param>
        /// <returns>面を構成する頂点のインデックス配列</returns>
        public static int[] ToVertexIndices(this IPXFace face, IPXPmx pmx) => new int[] { pmx.Vertex.IndexOf(face.Vertex1), pmx.Vertex.IndexOf(face.Vertex2), pmx.Vertex.IndexOf(face.Vertex3) };
    }
}
