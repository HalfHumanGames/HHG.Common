using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class UILineRenderer : MaskableGraphic
    {
        public float Thickness
        {
            get => thickness;
            set
            {
                thickness = value;
                SetVerticesDirty();
            }
        }

        public int PositionCount => points.Count;

        public override Texture mainTexture => material != null && material.mainTexture != null ? material.mainTexture : s_WhiteTexture;

        public LineTextureMode TextureMode
        {
            get => textureMode;
            set
            {
                textureMode = value;
                SetVerticesDirty();
            }
        }

        public Vector2 TextureScale
        {
            get => textureScale;
            set
            {
                textureScale = value;
                SetVerticesDirty();
            }
        }

        [SerializeField] private float thickness = 4f;
        [SerializeField] private LineTextureMode textureMode = LineTextureMode.Stretch;
        [SerializeField] private Vector2 textureScale = Vector2.one;

        private readonly List<Vector2> points = new List<Vector2>();
        private readonly List<float> segmentLengths = new List<float>();

        public void SetPositions(IReadOnlyList<Vector2> positions)
        {
            points.Clear();
            points.AddRange(positions);
            SetVerticesDirty();
        }

        public void Clear()
        {
            points.Clear();
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            if (points.Count < 2)
            {
                return;
            }

            int segmentCount = points.Count - 1;
            segmentLengths.Clear();
            float totalLength = 0f;

            for (int i = 0; i < segmentCount; i++)
            {
                float segmentLength = Vector2.Distance(points[i], points[i + 1]);
                segmentLengths.Add(segmentLength);
                totalLength += segmentLength;
            }

            float distance = 0f;

            for (int i = 0; i < segmentCount; i++)
            {
                float segmentLength = segmentLengths[i];
                GetSegmentUVRange(i, segmentCount, segmentLength, distance, totalLength, out float startU, out float endU);
                AddSegment(vertexHelper, points[i], points[i + 1], startU, endU, i * 4);
                distance += segmentLength;
            }
        }

        private void GetSegmentUVRange(int segmentIndex, int segmentCount, float segmentLength, float distance, float totalLength, out float startU, out float endU)
        {
            switch (textureMode)
            {
                case LineTextureMode.Tile:
                    startU = distance * textureScale.x;
                    endU = (distance + segmentLength) * textureScale.x;
                    break;
                case LineTextureMode.RepeatPerSegment:
                    startU = 0f;
                    endU = textureScale.x;
                    break;
                case LineTextureMode.DistributePerSegment:
                    startU = (float)segmentIndex / segmentCount;
                    endU = (float)(segmentIndex + 1) / segmentCount;
                    break;
                case LineTextureMode.Stretch:
                default:
                    startU = totalLength > 0f ? distance / totalLength : 0f;
                    endU = totalLength > 0f ? (distance + segmentLength) / totalLength : 1f;
                    break;
            }
        }

        private void AddSegment(VertexHelper vertexHelper, Vector2 start, Vector2 end, float startU, float endU, int vertexIndex)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * (thickness * 0.5f);

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = start - normal;
            vertex.uv0 = new Vector2(startU, 0f);
            vertexHelper.AddVert(vertex);

            vertex.position = start + normal;
            vertex.uv0 = new Vector2(startU, textureScale.y);
            vertexHelper.AddVert(vertex);

            vertex.position = end + normal;
            vertex.uv0 = new Vector2(endU, textureScale.y);
            vertexHelper.AddVert(vertex);

            vertex.position = end - normal;
            vertex.uv0 = new Vector2(endU, 0f);
            vertexHelper.AddVert(vertex);

            vertexHelper.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
            vertexHelper.AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex);
        }
    }
}
