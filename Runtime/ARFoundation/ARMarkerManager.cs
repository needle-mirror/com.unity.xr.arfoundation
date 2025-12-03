using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers) that enables you to detect
    /// and track AR markers. Add this component to your XR Origin GameObject to enable marker tracking in your app.
    /// </summary>
    /// <remarks>
    /// An AR Marker is a known visual pattern in the physical environment, such as a QR code or an ArUco tag,
    /// that a device can recognize and track. This manager detects these markers and creates GameObjects
    /// with <see cref="ARMarker"/> components to represent them.
    /// </remarks>
    [DefaultExecutionOrder(ARUpdateOrder.k_MarkerManager)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XROrigin))]
    [HelpURL(typeof(ARMarkerManager))]
    public class ARMarkerManager : ARTrackableManager<
        XRMarkerSubsystem,
        XRMarkerSubsystemDescriptor,
        XRMarkerSubsystem.Provider,
        XRMarker,
        ARMarker>, IRaycaster
    {
        [SerializeField]
        [Tooltip(
            "If not null, this prefab is instantiated for each detected AR marker. " +
            "If the prefab does not contain an ARMarker component, ARMarkerManager will add one.")]
        GameObject m_MarkerPrefab;

        /// <summary>
        /// Get or set the prefab to instantiate for each detected AR marker. Can be `null`.
        /// </summary>
        public GameObject markerPrefab
        {
            get => m_MarkerPrefab;
            set => m_MarkerPrefab = value;
        }

        /// <summary>
        /// The name to assign to the GameObject instantiated for each <see cref="ARMarker"/>.
        /// </summary>
        protected override string gameObjectName => "ARMarker";

        /// <summary>
        /// Gets the <see cref="ARMarker"/> with the given <paramref name="trackableId"/>, or `null` if no such marker exists.
        /// </summary>
        /// <param name="trackableId">The <see cref="TrackableId"/> of the <see cref="ARMarker"/> to retrieve.</param>
        /// <returns>The <see cref="ARMarker"/> or `null`.</returns>
        public ARMarker GetMarker(TrackableId trackableId)
        {
            return m_Trackables.GetValueOrDefault(trackableId);
        }

        /// <summary>
        /// Get the prefab to instantiate for each <see cref="ARMarker"/>.
        /// </summary>
        /// <returns>The prefab to instantiate for each <see cref="ARMarker"/>.</returns>
        protected override GameObject GetPrefab() => m_MarkerPrefab;

        /// <summary>
        /// Attempts to get the encoded data of a specific marker, decoded as a string.
        /// </summary>
        /// <remarks>
        /// To determine if a marker's encoded data is available as a string, you can check its
        /// <see cref="ARMarker.dataBuffer"/> property. If the <see cref="XRSpatialBuffer.bufferType"/> is
        /// <see cref="XRSpatialBufferType.String"/>, this method should be used to retrieve the encoded data.
        ///
        /// Alternatively, if you expect string data (for example, you are scanning QR codes with URLs), you can
        /// call this method directly and check the <see cref="Result{T}.status"/> to see if it was successful.
        /// </remarks>
        /// <param name="marker">The <see cref="ARMarker"/> for which to retrieve the encoded data.</param>
        /// <returns>
        /// A `Result` which contains the decoded string if successful.
        /// </returns>
        public Result<string> TryGetStringData(ARMarker marker)
        {
            if (subsystem == null)
            {
                var resultStatus = new XRResultStatus(XRResultStatus.StatusCode.ProviderUninitialized);
                return new Result<string>(resultStatus, string.Empty);
            }

            return subsystem.TryGetStringData(marker.trackableId, marker.dataBuffer);
        }

        /// <summary>
        /// Attempts to get the encoded data of a specific marker, decoded as a byte array.
        /// </summary>
        /// <remarks>
        /// To determine if a marker's encoded data is available as binary data, you can check its
        /// <see cref="ARMarker.dataBuffer"/> property. If the <see cref="XRSpatialBuffer.bufferType"/> is
        /// <see cref="XRSpatialBufferType.Uint8"/>, this method should be used to retrieve the encoded data.
        ///
        /// This method allocates a new managed `byte[]` on each successful call. For performance critical applications
        /// that need to avoid garbage collection, consider using (<see cref="TryGetBytesData(ARMarker, Allocator)"/>).
        /// </remarks>
        /// <param name="marker">The <see cref="ARMarker"/> for which to retrieve the encoded data.</param>
        /// <returns>
        /// A `Result` which contains the raw byte array if successful.
        /// </returns>
        public Result<byte[]> TryGetBytesData(ARMarker marker)
        {
            if (subsystem == null)
            {
                var resultStatus = new XRResultStatus(XRResultStatus.StatusCode.ProviderUninitialized);
                return new Result<byte[]>(resultStatus, null);
            }

            var result = subsystem.TryGetBytesData(marker.trackableId, marker.dataBuffer, Allocator.Temp);
            var bytesData = result.value;
            result = new Result<NativeArray<byte>>(result.status, bytesData);

            var byteArray = result.value.ToArray();
            return new Result<byte[]>(result.status, byteArray);
        }

        /// <summary>
        /// Attempts to get the encoded data of a specific marker, decoded as a <see cref="NativeArray{Byte}"/>.
        /// </summary>
        /// <remarks>
        /// This method is useful for performance sensitive applications as it avoids managed memory allocations.
        /// The caller is responsible for disposing the returned <see cref="NativeArray{Byte}"/> for persistent allocators.
        /// </remarks>
        /// <param name="marker">The marker with the encoded data to retrieve.</param>
        /// <param name="allocator">The allocator to use for the returned <see cref="NativeArray{Byte}"/>.</param>
        /// <returns>A `Result` containing the raw bytes in a <see cref="NativeArray{Byte}"/> if the operation is
        /// successful.</returns>
        public Result<NativeArray<byte>> TryGetBytesData(ARMarker marker, Allocator allocator)
        {
            if (subsystem == null)
            {
                var resultStatus = new XRResultStatus(XRResultStatus.StatusCode.ProviderUninitialized);
                return new Result<NativeArray<byte>>(resultStatus, default);
            }

            return subsystem.TryGetBytesData(marker.trackableId, marker.dataBuffer, allocator);
        }

        /// <summary>
        /// Performs a raycast against all tracked markers.
        /// </summary>
        /// <remarks>
        /// This method checks for intersections between the given <paramref name="ray"/> and the rectangular plane
        /// of each active <see cref="ARMarker"/>. It is an implementation of the <see cref="IRaycaster.Raycast"/> method.
        /// </remarks>
        /// <param name="ray">The ray, in session space, to cast against the markers.</param>
        /// <param name="trackableTypeMask">A mask of trackable types to raycast against. If this mask does not
        /// include <see cref="TrackableType.Marker"/>, the method returns an empty array.</param>
        /// <param name="allocator">The allocator to use for the returned <see cref="NativeArray{XRRaycastHit}"/> of hits.
        /// The caller is responsible for disposing this array.</param>
        /// <returns>A <see cref="NativeArray{XRRaycastHit}"/> containing the raycast hit data for each hit marker.
        /// The caller must dispose of this array for persistent allocators.</returns>
        public NativeArray<XRRaycastHit> Raycast(Ray ray, TrackableType trackableTypeMask, Allocator allocator)
        {
            if ((trackableTypeMask & TrackableType.Marker) == TrackableType.None)
                return new NativeArray<XRRaycastHit>(0, allocator);

            var hitBuffer = new NativeArray<XRRaycastHit>(trackables.count, Allocator.Temp);

            var count = 0;
            foreach (var marker in trackables)
            {
                var markerTransform = marker.transform;
                var normal = markerTransform.localRotation * Vector3.up;
                var infinitePlane = new Plane(normal, markerTransform.localPosition);
                if (!infinitePlane.Raycast(ray, out var distance))
                    continue;

                var pose = new Pose(
                    ray.origin + ray.direction * distance,
                    marker.transform.localRotation);

                var inverseLocalRotation = Quaternion.Inverse(marker.transform.localRotation);
                var localHitPosition = inverseLocalRotation * (pose.position - marker.transform.localPosition);

                if (Mathf.Abs(localHitPosition.x) <= marker.size.x * 0.5f &&
                    Mathf.Abs(localHitPosition.z) <= marker.size.y * 0.5f)
                {
                    hitBuffer[count] = new XRRaycastHit(
                        marker.trackableId,
                        pose,
                        distance,
                        TrackableType.Marker);

                    count += 1;
                }
            }

            var hitResults = new NativeArray<XRRaycastHit>(count, allocator);
            NativeArray<XRRaycastHit>.Copy(hitBuffer, hitResults, count);
            return hitResults;
        }
    }
}
