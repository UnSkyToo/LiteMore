using System.Collections.Generic;
using LiteFramework.Core.Log;
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.Extend
{
    [AddComponentMenu("UI/Lite/Letter Spacing")]
    public class LetterSpacing : BaseMeshEffect
    {
        [SerializeField] private float Spacing_ = 0f;

        public override void ModifyMesh(VertexHelper VH)
        {
            if (!IsActive())
            {
                return;
            }

            var VertextCount = VH.currentVertCount;
            if (VertextCount == 0)
            {
                return;
            }

            var Vertexs = new List<UIVertex>();
            for (var Index = 0; Index < VertextCount; ++Index)
            {
                var Vertex = new UIVertex();
                VH.PopulateUIVertex(ref Vertex, Index);
                Vertexs.Add(Vertex);
            }

            var Text = GetComponent<Text>();
            if (Text == null)
            {
                LLogger.LWarning("LetterSpacing : Missing Text component");
                return;
            }

            var Lines = Text.text.Split('\n');
            var LetterOffset = Spacing_ * (float)Text.fontSize / 100f;
            var AlignmentFactor = 0.0f;
            var GlyphIndex = 0;
            var Pos = Vector3.zero;

            switch (Text.alignment)
            {
                case TextAnchor.LowerLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft:
                    AlignmentFactor = 0f;
                    break;
                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                    AlignmentFactor = 0.5f;
                    break;
                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    AlignmentFactor = 1f;
                    break;
            }

            for (var LineIndex = 0; LineIndex < Lines.Length; ++LineIndex)
            {
                var Line = Lines[LineIndex];
                var LineOffset = (Line.Length - 1) * LetterOffset * AlignmentFactor;

                for (var CharIndex = 0; CharIndex < Line.Length; ++CharIndex)
                {
                    var Vert1 = Vertexs[GlyphIndex * 4 + 0];
                    var Vert2 = Vertexs[GlyphIndex * 4 + 1];
                    var Vert3 = Vertexs[GlyphIndex * 4 + 2];
                    var Vert4 = Vertexs[GlyphIndex * 4 + 3];

                    Pos = Vector3.right * (LetterOffset * CharIndex - LineOffset);

                    Vert1.position += Pos;
                    Vert2.position += Pos;
                    Vert3.position += Pos;
                    Vert4.position += Pos;


                    Vertexs[GlyphIndex * 4 + 0] = Vert1;
                    Vertexs[GlyphIndex * 4 + 1] = Vert2;
                    Vertexs[GlyphIndex * 4 + 2] = Vert3;
                    Vertexs[GlyphIndex * 4 + 3] = Vert4;
                    GlyphIndex++;
                }

                GlyphIndex++;
            }

            for (var Index = 0; Index < Vertexs.Count; ++Index)
            {
                VH.SetUIVertex(Vertexs[Index], Index);
            }
        }
    }
}