using PEPlugin;
using PEPlugin.Pmx;
using PEPlugin.SDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PEPExtensions
{
    /// <summary>
    /// PMXEditorプラグイン用便利メソッド
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// モデル・フォーム・ビューを一括更新する
        /// </summary>
        /// <param name="connector">ホストへのコネクタ</param>
        /// <param name="pmx">更新用プラグインPMX</param>
        /// <param name="option">更新対象</param>
        /// <param name="index">任意の対象Index</param>
        public static void Update(IPEConnector connector, IPXPmx pmx, PmxUpdateObject option = PmxUpdateObject.All, int index = -1)
        {
            connector.Pmx.Update(pmx, option, index);
            connector.Form.UpdateList(ConvUObjrct_DtoX(option));
            connector.View.PmxView.UpdateModel();
        }

        /// <summary>
        /// PmxUpdateObjectからPmd.UpdateObjectに変換する
        /// </summary>
        /// <param name="input">変換するPmxUpdateObject</param>
        /// <returns></returns>
        public static PEPlugin.Pmd.UpdateObject ConvUObjrct_DtoX(PmxUpdateObject input)
        {
            PEPlugin.Pmd.UpdateObject output;
            switch (input)
            {
                case PmxUpdateObject.None:
                    output = PEPlugin.Pmd.UpdateObject.None;
                    break;
                case PmxUpdateObject.All:
                    output = PEPlugin.Pmd.UpdateObject.All;
                    break;
                case PmxUpdateObject.Header:
                    output = PEPlugin.Pmd.UpdateObject.Header;
                    break;
                case PmxUpdateObject.ModelInfo:
                    output = PEPlugin.Pmd.UpdateObject.All;
                    break;
                case PmxUpdateObject.Vertex:
                    output = PEPlugin.Pmd.UpdateObject.Vertex;
                    break;
                case PmxUpdateObject.Face:
                    output = PEPlugin.Pmd.UpdateObject.Face;
                    break;
                case PmxUpdateObject.Material:
                    output = PEPlugin.Pmd.UpdateObject.Material;
                    break;
                case PmxUpdateObject.Bone:
                    output = PEPlugin.Pmd.UpdateObject.Bone;
                    break;
                case PmxUpdateObject.Morph:
                    output = PEPlugin.Pmd.UpdateObject.Morph;
                    break;
                case PmxUpdateObject.Node:
                    output = PEPlugin.Pmd.UpdateObject.Node;
                    break;
                case PmxUpdateObject.Body:
                    output = PEPlugin.Pmd.UpdateObject.Body;
                    break;
                case PmxUpdateObject.Joint:
                    output = PEPlugin.Pmd.UpdateObject.Joint;
                    break;
                case PmxUpdateObject.SoftBody:
                    output = PEPlugin.Pmd.UpdateObject.All;
                    break;
                default:
                    output = PEPlugin.Pmd.UpdateObject.All;
                    break;
            }

            return output;
        }

        /// <summary>
        /// 入力頂点のボーン・ウェイトをタプルリストに変換する
        /// </summary>
        /// <param name="vertex">入力頂点</param>
        /// <returns>(ボーン,ウェイト)のタプルリスト</returns>
        public static List<(IPXBone bone, float weight)> GetWeights(IPXVertex vertex)
        {
            var weights = new List<(IPXBone bone, float weight)>();
            weights.Add((vertex.Bone1, vertex.Weight1));
            weights.Add((vertex.Bone2, vertex.Weight2));
            weights.Add((vertex.Bone3, vertex.Weight3));
            weights.Add((vertex.Bone4, vertex.Weight4));
            weights.RemoveAll(w => w.bone == null);
            weights.RemoveAll(w => w.weight == 0.0);
            return weights;
        }

        /// <summary>
        /// タプルリストから頂点にボーン・ウェイト情報を正規化して格納する
        /// </summary>
        /// <param name="weights">(ボーン,ウェイト)タプルのリスト</param>
        /// <param name="vertex">格納する頂点</param>
        public static void SetVertexWeights(List<(IPXBone bone, float weight)> weights, ref IPXVertex vertex)
        {
            //正規化
            var weight = NormalizeWeights(weights);
            ClearVertexWeight(ref vertex);

            //頂点に入力
            vertex.Bone1 = weight[0].bone;
            vertex.Weight1 = weight[0].weight;
            if (weight.Count > 1)
            {
                vertex.Bone2 = weight[1].bone;
                vertex.Weight2 = weight[1].weight;
            }
            if (weight.Count > 2)
            {
                vertex.Bone3 = weight[2].bone;
                vertex.Weight3 = weight[2].weight;
            }
            if (weight.Count > 3)
            {
                vertex.Bone4 = weight[3].bone;
                vertex.Weight4 = weight[3].weight;
            }
        }

        /// <summary>
        /// ウェイトを正規化する
        /// </summary>
        /// <param name="weights">正規化する(ボーン,ウェイト)タプルのリスト</param>
        /// <param name="basis">正規化後の合計</param>
        /// <returns>正規化された(ボーン,ウェイト)タプルのリスト</returns>
        public static List<(IPXBone bone, float weight)> NormalizeWeights(List<(IPXBone bone, float weight)> weights, float basis = 1)
        {
            //weightを基準に降順でソート
            weights.Sort((v, w) => w.weight.CompareTo(v.weight));

            //大きい方から4つまでを残し正規化
            var sum = weights.Take(4).Select(w => w.weight).Sum();
            return weights.Take(4).Select(w => (w.bone, basis * w.weight / sum)).ToList();
        }

        /// <summary>
        /// ウェイトを正規化する
        /// </summary>
        /// <param name="vertex">正規化する頂点</param>
        public static void NormalizeWeights(ref IPXVertex vertex)
        {
            SetVertexWeights(GetWeights(vertex), ref vertex);
        }

        /// <summary>
        /// 頂点のウェイトを除去する
        /// </summary>
        /// <param name="vertex">対象頂点</param>
        public static void ClearVertexWeight(ref IPXVertex vertex)
        {
            vertex.Bone1 = null;
            vertex.Weight1 = 0;
            vertex.Bone2 = null;
            vertex.Weight2 = 0;
            vertex.Bone3 = null;
            vertex.Weight3 = 0;
            vertex.Bone4 = null;
            vertex.Weight4 = 0;
        }

        /// <summary>
        /// 面に含まれる頂点を返す
        /// </summary>
        /// <param name="face">対象面</param>
        /// <returns>包含頂点リスト</returns>
        public static List<IPXVertex> GetFaceVertices(IPXFace face)
        {
            var vertices = new List<IPXVertex>();
            vertices.Add(face.Vertex1);
            vertices.Add(face.Vertex2);
            vertices.Add(face.Vertex3);
            return vertices;
        }


        /// <summary>
        /// 材質に属する面を構成する全ての頂点を取得する
        /// </summary>
        /// <param name="material">対象材質</param>
        /// <returns>頂点コレクション</returns>
        public static IEnumerable<IPXVertex> GetMaterialVertices(IPXMaterial material)
        {
            return material.Faces.SelectMany(f => GetFaceVertices(f)).Distinct();
        }

        /// <summary>
        /// 例外表示用複合ダイアログボックスを表示する
        /// </summary>
        /// <param name="ex">表示対象例外オブジェクト</param>
        /// <returns>押下されたボタン種類</returns>
        public static DialogResult ShowException(Exception ex)
        {
            // Get reference to the dialog type. 
            var dialogTypeName = "System.Windows.Forms.PropertyGridInternal.GridErrorDlg";
            var dialogType = typeof(Form).Assembly.GetType(dialogTypeName);

            // Create dialog instance. 
            var dialog = (Form)Activator.CreateInstance(dialogType, new PropertyGrid());

            // Populate relevant properties on the dialog instance. 
            dialog.Text = "例外発生";
            dialogType.GetProperty("Details").SetValue(dialog, ex.StackTrace, null);
            dialogType.GetProperty("Message").SetValue(dialog, ex.Message, null);

            // Display dialog. 
            return dialog.ShowDialog();
        }

        /// <summary>
        /// 指定された頂点コレクションの空間的範囲を返す
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static (V3 min, V3 max) GetBoundingBox(IEnumerable<IPXVertex> vertices)
        {
            if (!vertices.Any())
                throw new ArgumentOutOfRangeException("頂点が入っていないリストが境界箱生成メソッドに渡されました。");

            (V3 min, V3 max) bound = (vertices.First().Position, vertices.First().Position);

            // たぶん集計系Linqは即時評価されるので一括ループで集計せねば遅い
            foreach (var v in vertices)
            {
                bound.min.X = bound.min.X > v.Position.X ? v.Position.X : bound.min.X;
                bound.min.Y = bound.min.Y > v.Position.Y ? v.Position.Y : bound.min.Y;
                bound.min.Z = bound.min.Z > v.Position.Z ? v.Position.Z : bound.min.Z;

                bound.max.X = bound.max.X < v.Position.X ? v.Position.X : bound.max.X;
                bound.max.Y = bound.max.Y < v.Position.Y ? v.Position.Y : bound.max.Y;
                bound.max.Z = bound.max.Z < v.Position.Z ? v.Position.Z : bound.max.Z;
            }

            return bound;
        }
    }
}
