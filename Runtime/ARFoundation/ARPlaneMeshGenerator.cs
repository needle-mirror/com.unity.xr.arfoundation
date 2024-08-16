using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Generator method for <see cref="ARPlane"/> mesh geometry.
    /// </summary>
    public static class ARPlaneMeshGenerator
    {
        static List<Vector3> s_Vertices = new();
        static List<int> s_TriangleIndices = new();
        static List<Vector3> s_VertexNormals = new();
        static Bounds s_Bounds;

        static LinkedList<int> s_UnusedVertices = new();
        static LinkedListNode<int> s_CurrentUnusedVertexNode;
        static HashSet<int> s_ReflexVertices = new();

        /// <summary>
        /// Generates a <c>Mesh</c> from the given parameters. The <paramref name="boundaryVertices"/> is assumed to be a simple polygon. The algorithm uses the ear clipping algorithm with O(n) time complexity for convex polygons and O(n * m) for concave polygons where n is the total number vertices and m is the number of reflex vertices. The space complexity is O(n), scaling linearly with the number of boundary vertices.
        /// </summary>
        /// <param name="mesh">The <c>Mesh</c> to write results to.</param>
        /// <param name="boundaryVertices">The vertices of the planes boundary, in plane-space.</param>
        /// <returns><c>True</c> if the <paramref name="mesh"/> was generated, <c>False</c> otherwise.</returns>
        public static bool TryGenerateMesh(Mesh mesh, NativeArray<Vector2> boundaryVertices)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            if (boundaryVertices.Length < 3)
                return false;

            s_Vertices.Clear();
            s_TriangleIndices.Clear();
            s_VertexNormals.Clear();
            s_Bounds = new();
            s_UnusedVertices.Clear();
            s_CurrentUnusedVertexNode = null;
            s_ReflexVertices.Clear();

            Initialize(boundaryVertices);
            Triangulate();

            mesh.Clear();
            mesh.SetVertices(s_Vertices);
            mesh.SetTriangles(s_TriangleIndices, 0, true);
            mesh.SetNormals(s_VertexNormals);

            var uvs = new List<Vector2>();
            GenerateUVs(uvs, s_Vertices);
            mesh.SetUVs(0, uvs);
            return true;
        }

        static void GenerateUVs(ICollection<Vector2> uvs, IEnumerable<Vector3> vertices)
        {
            uvs.Clear();
            foreach (var vertex in vertices)
            {
                var uv = new Vector2(vertex.x, vertex.z);
                uvs.Add(uv);
            }
        }

        static bool IsVertexReflex(Vector3 vertex, Vector3 nextVertex, Vector3 previousVertex)
        {
            var nextVector = nextVertex - vertex;
            var previousVector = previousVertex - vertex;

            var determinant = Determinant(nextVector, previousVector);
            return determinant >= 0;
        }

        static void Initialize(NativeArray<Vector2> vertices)
        {
            var numVertices = vertices.Length;
            for (int i = 0; i < numVertices; i += 1)
            {
                s_UnusedVertices.AddLast(i);
                s_Bounds.Encapsulate(vertices[i]);
                s_Vertices.Add(new(vertices[i].x, 0, vertices[i].y));
                s_VertexNormals.Add(Vector3.up);

                var nextIndex = (i + 1) % numVertices;
                var previousIndex = (i < 1 ? numVertices : i) - 1;

                var vertex = new Vector3(vertices[i].x, 0, vertices[i].y);
                var nextVertex = new Vector3(vertices[nextIndex].x, 0, vertices[nextIndex].y);
                var previousVertex = new Vector3(vertices[previousIndex].x, 0, vertices[previousIndex].y);

                if (IsVertexReflex(vertex, nextVertex, previousVertex))
                    s_ReflexVertices.Add(i);
            }
        }

        static void Triangulate()
        {
            s_CurrentUnusedVertexNode = s_UnusedVertices.Last;

            while (s_UnusedVertices.Count > 2)
            {
                s_CurrentUnusedVertexNode = GetNextUnusedIndexNode(s_CurrentUnusedVertexNode);
                var nextUnusedVertexNode = GetNextUnusedIndexNode(s_CurrentUnusedVertexNode);
                var previousUnusedVertexNode = GetPreviousUnusedIndexNode(s_CurrentUnusedVertexNode);

                if (s_ReflexVertices.Contains(s_CurrentUnusedVertexNode.Value))
                    continue;

                var isAnyReflexVertexInsideTriangle = IsAnyReflexVertexInsideTriangle(
                    s_CurrentUnusedVertexNode.Value,
                    nextUnusedVertexNode.Value,
                    previousUnusedVertexNode.Value);

                if (isAnyReflexVertexInsideTriangle)
                    continue;

                s_TriangleIndices.Add(s_CurrentUnusedVertexNode.Value);
                s_TriangleIndices.Add(nextUnusedVertexNode.Value);
                s_TriangleIndices.Add(previousUnusedVertexNode.Value);

                s_UnusedVertices.Remove(s_CurrentUnusedVertexNode);

                UpdateReflexVertex(nextUnusedVertexNode);
                UpdateReflexVertex(previousUnusedVertexNode);
            }
        }

        static bool IsAnyReflexVertexInsideTriangle(
            int currentIndex,
            int nextIndex,
            int previousIndex)
        {
            foreach (var index in s_ReflexVertices)
            {
                if (index == nextIndex || index == previousIndex)
                    continue;

                var triangleSideA = s_Vertices[nextIndex] - s_Vertices[currentIndex];
                var triangleSideB = s_Vertices[previousIndex] - s_Vertices[nextIndex];
                var triangleSideC = s_Vertices[currentIndex] - s_Vertices[previousIndex];

                var reflexPointVectorA = s_Vertices[index] - s_Vertices[currentIndex];
                var reflexPointVectorB = s_Vertices[index] - s_Vertices[nextIndex];
                var reflexPointVectorC = s_Vertices[index] - s_Vertices[previousIndex];

                var triangleSideADeterminant = Determinant(triangleSideA, reflexPointVectorA);
                var triangleSideBDeterminant = Determinant(triangleSideB, reflexPointVectorB);
                var triangleSideCDeterminant = Determinant(triangleSideC, reflexPointVectorC);

                var isADeterminantNegative = triangleSideADeterminant < 0;
                var isBDeterminantNegative = triangleSideBDeterminant < 0;
                var isCDeterminantNegative = triangleSideCDeterminant < 0;

                if (isADeterminantNegative == isBDeterminantNegative &&
                    isADeterminantNegative == isCDeterminantNegative)
                    return true;
            }

            return false;
        }

        static void UpdateReflexVertex(LinkedListNode<int> node)
        {
            if (!s_ReflexVertices.Contains(node.Value))
                return;

            var nextIndex = GetNextUnusedIndexNode(node).Value;
            var previousIndex = GetPreviousUnusedIndexNode(node).Value;

            var vertex = s_Vertices[node.Value];
            var nextVertex = s_Vertices[nextIndex];
            var previousVertex = s_Vertices[previousIndex];

            if (IsVertexReflex(vertex, nextVertex, previousVertex))
                return;

            s_ReflexVertices.Remove(node.Value);
        }

        static LinkedListNode<int> GetNextUnusedIndexNode(LinkedListNode<int> node)
        {
            return node.Next ?? s_UnusedVertices.First;
        }

        static LinkedListNode<int> GetPreviousUnusedIndexNode(LinkedListNode<int> node)
        {
            return node.Previous ?? s_UnusedVertices.Last;
        }

        static float Determinant(Vector3 vectorA, Vector3 vectorB)
        {
            return vectorA.x * vectorB.z - vectorA.z * vectorB.x;
        }
    }
}
