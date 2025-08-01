using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Manages an <c>XRRaycastSubsystem</c>, exposing raycast functionality in AR Foundation. Use this component
    /// to raycast against trackables (that is, detected features in the physical environment) when they do not have
    /// a presence in the Physics world.
    /// </summary>
    /// <remarks>
    /// Related information: <a href="xref:arfoundation-raycasts">AR Raycast Manager component</a>
    /// </remarks>
    [DefaultExecutionOrder(ARUpdateOrder.k_RaycastManager)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XROrigin))]
    [AddComponentMenu("XR/AR Foundation/AR Raycast Manager")]
    [HelpURL("features/raycasts")]
    public sealed class ARRaycastManager : ARTrackableManager<
        XRRaycastSubsystem, XRRaycastSubsystemDescriptor,
        XRRaycastSubsystem.Provider,
        XRRaycast, ARRaycast>
    {
        [SerializeField]
        [Tooltip("If not null, instantiates this prefab for each raycast.")]
        GameObject m_RaycastPrefab;

        static Comparison<ARRaycastHit> s_RaycastHitComparer = CompareRaycastHit;
        static List<NativeArray<XRRaycastHit>> s_NativeRaycastHits = new();

        HashSet<IRaycaster> m_Raycasters = new();
        ARPlaneManager m_CachedPlaneManager;
        ARBoundingBoxManager m_CachedBoundingBoxManager;

        /// <summary>
        /// The name of the `GameObject` for each instantiated <see cref="ARRaycast"/>.
        /// </summary>
        protected override string gameObjectName => "ARRaycast";

        /// <summary>
        /// If not null, this prefab will be instantiated for each <see cref="ARRaycast"/>.
        /// </summary>
        public GameObject raycastPrefab
        {
            get => m_RaycastPrefab;
            set => m_RaycastPrefab = value;
        }

        /// <summary>
        /// Cast a ray from a point in screen space against trackables, that is, detected features such as planes.
        /// </summary>
        /// <param name="screenPoint">The point, in device screen pixels, from which to cast.</param>
        /// <param name="hitResults">Contents are replaced with the raycast results, if successful.
        /// Results are sorted by distance in closest-first order.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <returns><see langword="true"/> if the raycast hit a trackable in the <paramref name="trackableTypes"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        #region ARRaycastManager_Raycast_screenPoint
        public bool Raycast(
            Vector2 screenPoint,
            List<ARRaycastHit> hitResults,
            TrackableType trackableTypes = TrackableType.AllTypes)
        #endregion
        {
            if (hitResults == null)
                throw new ArgumentNullException("hitResults");

            var allHits = RaycastViewPortImpl(screenPoint, trackableTypes, Allocator.Temp);
            var originTransform = origin.Camera != null ? origin.Camera.transform : origin.TrackablesParent;

            return TransformAndDisposeNativeHitResults(allHits, hitResults, originTransform.position);
        }

        /// <summary>
        /// Cast a <c>Ray</c> against trackables, that is, detected features such as planes.
        /// </summary>
        /// <param name="ray">The <c>Ray</c>, in Unity world space, to cast.</param>
        /// <param name="hitResults">Contents are replaced with the raycast results, if successful.
        /// Results are sorted by distance in closest-first order.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <returns><see langword="true"/> if the raycast hit a trackable in the <paramref name="trackableTypes"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        #region ARRaycastManager_Raycast_ray
        public bool Raycast(
            Ray ray,
            List<ARRaycastHit> hitResults,
            TrackableType trackableTypes = TrackableType.AllTypes)
        #endregion
        {
            if (hitResults == null)
                throw new ArgumentNullException(nameof(hitResults));

            var sessionSpaceRay = origin.TrackablesParent.InverseTransformRay(ray);
            var allHits = RaycastWorldSpace(sessionSpaceRay, trackableTypes, Allocator.Temp);

            return TransformAndDisposeNativeHitResults(allHits,hitResults,ray.origin);
        }

        /// <summary>
        /// Creates an <see cref="ARRaycast"/> that updates automatically. <see cref="ARRaycast"/>s will
        /// continue to update until you remove them with <see cref="RemoveRaycast(ARRaycast)"/> or disable
        /// this component.
        /// </summary>
        /// <param name="screenPoint">A point on the screen, in pixels.</param>
        /// <param name="estimatedDistance">The estimated distance to the intersection point.
        /// This can be used to determine a potential intersection before the environment has been fully mapped.</param>
        /// <returns>A new <see cref="ARRaycast"/> if successful; otherwise `null`.</returns>
        #region ARRaycastManager_AddRaycast_screenPoint
        public ARRaycast AddRaycast(Vector2 screenPoint, float estimatedDistance)
        #endregion
        {
            if (subsystem == null)
                return null;

            var normalizedScreenPoint = new Vector2(
                Mathf.Clamp01(screenPoint.x / Screen.width),
                Mathf.Clamp01(screenPoint.y / Screen.height));

            if (subsystem.TryAddRaycast(normalizedScreenPoint, estimatedDistance, out XRRaycast sessionRelativeData))
            {
                return CreateTrackableImmediate(sessionRelativeData);
            }

            if (origin.Camera && subsystem.TryAddRaycast(ScreenPointToSessionSpaceRay(screenPoint), estimatedDistance, out sessionRelativeData))
            {
                return CreateTrackableImmediate(sessionRelativeData);
            }

            return null;
        }

        /// <summary>
        /// Removes an existing <see cref="ARRaycast"/>.
        /// </summary>
        /// <param name="raycast">The <see cref="ARRaycast"/> to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="raycast"/> is `null`.</exception>
        public void RemoveRaycast(ARRaycast raycast)
        {
            if (raycast == null)
                throw new ArgumentNullException(nameof(raycast));

            subsystem?.RemoveRaycast(raycast.trackableId);
        }

        /// <summary>
        /// Gets the Prefab that should be instantiated for each <see cref="ARRaycast"/>. Can be `null`.
        /// </summary>
        /// <returns>The Prefab that should be instantiated for each <see cref="ARRaycast"/>.</returns>
        protected override GameObject GetPrefab() => m_RaycastPrefab;

        /// <summary>
        /// Allows AR managers to register themselves as a raycaster.
        /// Raycasters can be used as a fallback method if the AR platform does
        /// not support raycasting using arbitrary <c>Ray</c>s.
        /// </summary>
        /// <param name="raycaster">A raycaster implementing the IRaycast interface.</param>
        internal void RegisterRaycaster(IRaycaster raycaster)
        {
            var raycasterType = raycaster.GetType();
            if (raycasterType == typeof(ARPlaneManager))
                m_CachedPlaneManager = raycaster as ARPlaneManager;
            else if (raycasterType == typeof(ARBoundingBoxManager))
                m_CachedBoundingBoxManager = raycaster as ARBoundingBoxManager;

            m_Raycasters.Add(raycaster);
        }

        /// <summary>
        /// Unregisters a raycaster previously registered with <see cref="RegisterRaycaster(IRaycaster)"/>.
        /// </summary>
        /// <param name="raycaster">A raycaster to use as a fallback, if needed.</param>
        internal void UnregisterRaycaster(IRaycaster raycaster)
        {
            if (m_Raycasters != null)
                m_Raycasters.Remove(raycaster);
        }

        /// <summary>
        /// Invoked just after the subsystem has been `Start`ed. Used to set raycast delegates internally.
        /// </summary>
        protected override void OnAfterStart() {}

        NativeArray<XRRaycastHit> RaycastWorldSpace(Ray ray, TrackableType typeMask, Allocator allocator)
        {
            TrackableType fallbackTypeMask = typeMask;
            bool doNativeRaycast =
                subsystem != null
                && subsystem.subsystemDescriptor.supportsWorldBasedRaycast
                && (typeMask & subsystem.subsystemDescriptor.supportedTrackableTypes) != TrackableType.None;
            if (doNativeRaycast)
                fallbackTypeMask = typeMask & ~subsystem.subsystemDescriptor.supportedTrackableTypes;

            bool doFallbackRaycast = fallbackTypeMask != TrackableType.None;

            if (doNativeRaycast && doFallbackRaycast)
            {
                var nativeHits = RaycastRay(ray, typeMask, Allocator.Temp);
                var fallbackHits = RaycastRayFallback(ray, fallbackTypeMask, Allocator.Temp);
                return CombineNativeArraysAndDispose(nativeHits, fallbackHits, allocator);
            }
            else if (doNativeRaycast)
                return RaycastRay(ray, typeMask, allocator);
            else if (doFallbackRaycast)
                return RaycastRayFallback(ray, fallbackTypeMask, allocator);
            else
                return new NativeArray<XRRaycastHit>(0, allocator);
        }

        NativeArray<XRRaycastHit> RaycastViewPortImpl(Vector2 screenPoint, TrackableType typeMask, Allocator allocator)
        {
            TrackableType fallbackTypeMask = typeMask;
            bool doNativeRaycast =
                subsystem != null
                && subsystem.subsystemDescriptor.supportsViewportBasedRaycast
                && (typeMask & subsystem.subsystemDescriptor.supportedTrackableTypes) != TrackableType.None;
            if (doNativeRaycast)
                fallbackTypeMask = typeMask & ~subsystem.subsystemDescriptor.supportedTrackableTypes;

            bool doFallbackRaycast = fallbackTypeMask != TrackableType.None;

            if (doNativeRaycast && doFallbackRaycast)
            {
                var nativeHits = RaycastViewport(screenPoint, typeMask, Allocator.Temp);
                var fallbackHits = RaycastWorldSpace(ScreenPointToSessionSpaceRay(screenPoint), fallbackTypeMask, Allocator.Temp);
                return CombineNativeArraysAndDispose(nativeHits, fallbackHits, allocator);
            }
            else if (doNativeRaycast)
                return RaycastViewport(screenPoint, typeMask, allocator);
            else if (doFallbackRaycast)
                return RaycastWorldSpace(ScreenPointToSessionSpaceRay(screenPoint), fallbackTypeMask, allocator);
            else
                return new NativeArray<XRRaycastHit>(0, allocator);
        }

        static int CompareRaycastHit(ARRaycastHit lhs, ARRaycastHit rhs)
        {
            return lhs.CompareTo(rhs);
        }

        Ray ScreenPointToSessionSpaceRay(Vector2 screenPoint)
        {
            var worldSpaceRay = origin.Camera.ScreenPointToRay(screenPoint);
            return origin.TrackablesParent.InverseTransformRay(worldSpaceRay);
        }

        NativeArray<XRRaycastHit> RaycastViewport(Vector2 screenPoint, TrackableType typeMask, Allocator allocator)
        {
            screenPoint.x = Mathf.Clamp01(screenPoint.x / Screen.width);
            screenPoint.y = Mathf.Clamp01(screenPoint.y / Screen.height);
            return subsystem.Raycast(screenPoint, typeMask, allocator);
        }

        NativeArray<XRRaycastHit> RaycastRay(
            Ray ray,
            TrackableType trackableTypeMask,
            Allocator allocator)
        {
            return subsystem.Raycast(ray, trackableTypeMask, allocator);
        }

        NativeArray<XRRaycastHit> RaycastRayFallback(Ray ray, TrackableType typeMask, Allocator allocator)
        {
            s_NativeRaycastHits.Clear();
            int count = 0;
            foreach (var raycaster in m_Raycasters)
            {
                var hits = raycaster.Raycast(ray, typeMask, Allocator.Temp);
                if (hits.IsCreated)
                {
                    s_NativeRaycastHits.Add(hits);
                    count += hits.Length;
                }
            }

            var allHits = new NativeArray<XRRaycastHit>(count, allocator);
            int dstIndex = 0;
            foreach (var hitArray in s_NativeRaycastHits)
            {
                if (hitArray.Length > 0)
                {
                    NativeArray<XRRaycastHit>.Copy(hitArray, 0, allHits, dstIndex, hitArray.Length);
                    dstIndex += hitArray.Length;
                }

                if (hitArray.IsCreated)
                {
                    hitArray.Dispose();
                }
            }

            return allHits;
        }

        bool TransformAndDisposeNativeHitResults(
            NativeArray<XRRaycastHit> nativeHits,
            List<ARRaycastHit> managedHits,
            Vector3 rayOrigin)
        {
            managedHits.Clear();
            if (!nativeHits.IsCreated)
                return false;

            using (nativeHits)
            {
                bool cachedPlaneManagerFound = m_CachedPlaneManager != null;
                bool cachedBoxManagerFound = m_CachedBoundingBoxManager != null;

                // Results are in "trackables space", so transform results back into world space
                foreach (var nativeHit in nativeHits)
                {
                    float distanceInWorldSpace = (nativeHit.pose.position - rayOrigin).magnitude;

                    // Attempt to look up the trackable
                    ARTrackable trackable = null;

                    // Planes
                    if ((nativeHit.hitType & TrackableType.Planes) != 0 && cachedPlaneManagerFound)
                    {
                        trackable = m_CachedPlaneManager.GetPlane(nativeHit.trackableId);
                    }
                    // Bounding Boxes
                    if ((nativeHit.hitType & TrackableType.BoundingBox) != 0 && cachedBoxManagerFound)
                    {
                        ARBoundingBox boundingBox;
                        if (m_CachedBoundingBoxManager.TryGetBoundingBox(nativeHit.trackableId, out boundingBox))
                            trackable = boundingBox;
                    }

                    managedHits.Add(new ARRaycastHit(nativeHit, distanceInWorldSpace, origin.TrackablesParent, trackable));
                }

                managedHits.Sort(s_RaycastHitComparer);
                return managedHits.Count > 0;
            }
        }

        NativeArray<T> CombineNativeArraysAndDispose<T>(
            NativeArray<T> first,
            NativeArray<T> second,
            Allocator allocator) where T : struct
        {
            NativeArray<T> combinedArray = new NativeArray<T>(first.Length + second.Length, allocator);
            NativeArray<T>.Copy(first, 0, combinedArray, 0, first.Length);
            if (second.Length > 0)
                NativeArray<T>.Copy(second, 0, combinedArray, first.Length, second.Length);
            first.Dispose();
            second.Dispose();
            return combinedArray;
        }

        /// <summary>
        /// Invoked just after a <see cref="ARRaycast"/> has been updated.
        /// </summary>
        /// <param name="raycast">The <see cref="ARRaycast"/> being updated.</param>
        /// <param name="sessionRelativeData">The new data associated with the raycast. All spatial
        /// data is is session-relative space.</param>
        protected override void OnAfterSetSessionRelativeData(ARRaycast raycast, XRRaycast sessionRelativeData)
        {
            raycast.plane = m_CachedPlaneManager != null ? m_CachedPlaneManager.GetPlane(sessionRelativeData.hitTrackableId) : null;
        }
    }
}
