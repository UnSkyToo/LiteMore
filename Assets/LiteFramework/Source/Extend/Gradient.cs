using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.Extend
{
    [AddComponentMenu("UI/Lite/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField] private Color32 TopColor_ = Color.white;
        [SerializeField] private Color32 BottomColor_ = Color.black;

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

            var TopY = Vertexs[0].position.y;
            var BottomY = Vertexs[0].position.y;

            for (var Index = 1; Index < VertextCount; ++Index)
            {
                var CurrentY = Vertexs[Index].position.y;
                if (CurrentY > TopY)
                {
                    TopY = CurrentY;
                }
                else if (CurrentY < BottomY)
                {
                    BottomY = CurrentY;
                }
            }

            var Height = TopY - BottomY;
            for (var Index = 0; Index < VertextCount; ++Index)
            {
                var Vertex = Vertexs[Index];
                var Color = Color32.Lerp(BottomColor_, TopColor_, (Vertex.position.y - BottomY) / Height);
                Vertex.color = Color;
                VH.SetUIVertex(Vertex, Index);
            }
        }
    }
}