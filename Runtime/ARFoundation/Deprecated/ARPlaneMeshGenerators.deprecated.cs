using System;
using System.Collections.Generic;
using Unity.Collections;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Generator methods for <see cref="ARPlane"/> mesh geometry.
    /// </summary>
    /// <remarks>
    /// This static class provides ways to generate different parts of plane geometry, such as vertices, indices, normals, and UVs.
    /// You can use these methods to build an ARPlane Mesh object.
    /// </remarks>
    [Obsolete("ARPlaneMeshGenerators has been deprecated in AR Foundation version 6.0. Use ARPlaneMeshGenerator instead.", true)]
    public static class ARPlaneMeshGenerators
    {
        /// <summary>
        /// Generates a <c>Mesh</c> from the given parameters. The <paramref name="convexPolygon"/> is assumed to be convex.
        /// </summary>
        /// <remarks>
        /// <paramref name="convexPolygon"/> is not checked for its convexness. Concave polygons will produce incorrect results.
        /// </remarks>
        /// <param name="mesh">The <c>Mesh</c> to write results to.</param>
        /// <param name="pose">The session-space pose of the mesh.</param>
        /// <param name="convexPolygon">The vertices of the plane's boundary, in plane-space.</param>
        /// <param name="areaTolerance">If any triangle in the generated mesh is less than this, then the entire mesh is ignored.
        /// This handles an edge case which prevents degenerate or very small triangles. Units are meters-squared.</param>
        /// <returns><c>True</c> if the <paramref name="mesh"/> was generated, <c>False</c> otherwise. The <paramref name="mesh"/> might
        /// fail to generate if it is not a valid polygon (too few vertices) or if it contains degenerate triangles (area smaller than <paramref name="areaTolerance"/>).</returns>
        [Obsolete("GenerateMesh has been deprecated in AR Foundation version 6.0. Use ARPlaneMeshGenerator.TryGenerateMesh instead.", true)]
        public static bool GenerateMesh(Mesh mesh, Pose pose, NativeArray<Vector2> convexPolygon, float areaTolerance = 1e-6f) => false;

        /// <summary>
        /// Generates a `List` of UVs from the given parameters.
        /// </summary>
        /// <param name="Uvs">The `List` to write results to.</param>
        /// <param name="pose">The session-space pose of the mesh.</param>
        /// <param name="vertices">The vertices of the plane's boundary, in plane-space.</param>
        [Obsolete("GenerateUvs has been deprecated in AR Foundation version 6.0.", true)]
        public static void GenerateUvs(List<Vector2> Uvs, Pose pose, List<Vector3> vertices) { }

        /// <summary>
        /// Generates a `List` of indices from the given parameters, forming a triangle fan.
        /// The <paramref name="convexPolygon"/> is assumed to be convex.
        /// </summary>
        /// <remarks>
        /// <paramref name="convexPolygon"/> is not checked for its convexness. Concave polygons will produce incorrect results.
        /// </remarks>
        /// <param name="indices">The `List` to write results to.</param>
        /// <param name="convexPolygon">The vertices of the plane's boundary, in plane-space.</param>
        /// <param name="areaTolerance">If any triangle in the generated mesh is less than this, then the entire mesh is ignored.</param>
        /// <returns><c>True</c> if the indices were generated, <c>False</c> if a triangle whose area is less than <paramref name="areaTolerance"/> is found.</returns>
        [Obsolete("GenerateIndices has been deprecated in AR Foundation version 6.0.", true)]
        public static bool GenerateIndices(List<int> indices, List<Vector3> convexPolygon, float areaTolerance = 1e-6f) => false;
    }
}
