using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Used for raycasting against 3D bounding boxes.
    /// </summary>
    internal class BoundingBoxRaycaster : IRaycaster
    {
        const int k_MaxNumberOfHitFaces = 2;

        TrackableCollection<ARBoundingBox> m_BoundingBoxTrackables;

        /// <summary>
        /// Constructs a new instance, initializing the collection of <see cref="XRBoundingBox"> trackables to test raycasts against.
        /// </summary>
        /// <param name="boundingBoxTrackables">The collection of bounding boxes to test raycasts again.</param>
        public BoundingBoxRaycaster(TrackableCollection<ARBoundingBox> boundingBoxTrackables)
        {
            m_BoundingBoxTrackables = boundingBoxTrackables;
        }

        /// <summary>
        /// Performs a raycast against all 3D bounding boxes passed in during construction.
        /// </summary>
        /// <param name="ray">The ray, in Unity world space, to cast.</param>
        /// <param name="trackableTypeMask">A mask of raycast types to perform.</param>
        /// <param name="allocator">The <c>Allocator</c> to use when creating the returned <c>NativeArray</c>.</param>
        /// <returns>
        /// A new <c>NativeArray</c> of raycast results allocated with <paramref name="allocator"/>.
        /// The caller owns the memory and is responsible for calling <c>Dispose</c> on the <c>NativeArray</c>.
        /// </returns>
        /// <seealso cref="ARRaycastManager.Raycast(Ray, List{ARRaycastHit}, TrackableType)"/>
        public NativeArray<XRRaycastHit> Raycast(
            Ray ray,
            TrackableType trackableTypeMask,
            Allocator allocator)
        {
            // if the bounding box flag is not set for the mask to search for, return early
            if ((trackableTypeMask & TrackableType.BoundingBox) != TrackableType.BoundingBox)
                return new NativeArray<XRRaycastHit>(0, allocator);

            var hitBuffer = new NativeArray<XRRaycastHit>(m_BoundingBoxTrackables.count, Allocator.Temp);
            var boundingBoxHitCount = 0;
            foreach (var boundingBox in m_BoundingBoxTrackables)
            {
                var closestHitDistance = float.MaxValue;
                var numFacesHit = 0;

                // forward face
                var wasForwardFaceHit = TryRaycastZAxisFace(
                    boundingBox,
                    ray,
                    1,
                    hitBuffer,
                    boundingBoxHitCount,
                    ref closestHitDistance);

                numFacesHit += wasForwardFaceHit ? 1 : 0;

                // back face
                if (numFacesHit < k_MaxNumberOfHitFaces)
                {
                    var wasBackFaceHit = TryRaycastZAxisFace(
                        boundingBox,
                        ray,
                        -1,
                        hitBuffer,
                        boundingBoxHitCount,
                        ref closestHitDistance);

                    numFacesHit += wasBackFaceHit ? 1 : 0;
                }

                // right face
                if (numFacesHit < k_MaxNumberOfHitFaces)
                {
                    var wasRightFaceHit = TryRaycastXAxisFace(
                        boundingBox,
                        ray,
                        1,
                        hitBuffer,
                        boundingBoxHitCount,
                        ref closestHitDistance);

                    numFacesHit += wasRightFaceHit ? 1 : 0;
                }

                // left face
                if (numFacesHit < k_MaxNumberOfHitFaces)
                {
                    var wasLeftFaceHit = TryRaycastXAxisFace(
                        boundingBox,
                        ray,
                        -1,
                        hitBuffer,
                        boundingBoxHitCount,
                        ref closestHitDistance);

                    numFacesHit += wasLeftFaceHit ? 1 : 0;
                }

                // top face
                if (numFacesHit < k_MaxNumberOfHitFaces)
                {
                    var wasTopFaceHit = TryRaycastYAxisFace(
                        boundingBox,
                        ray,
                        1,
                        hitBuffer,
                        boundingBoxHitCount,
                        ref closestHitDistance);

                    numFacesHit += wasTopFaceHit ? 1 : 0;
                }

                // bottom face
                if (numFacesHit < k_MaxNumberOfHitFaces)
                {
                    var wasBottomFaceHit = TryRaycastYAxisFace(
                        boundingBox,
                        ray,
                        -1,
                        hitBuffer,
                        boundingBoxHitCount,
                        ref closestHitDistance);

                    numFacesHit += wasBottomFaceHit ? 1 : 0;
                }

                if (numFacesHit > 0)
                    boundingBoxHitCount += 1;
            }

            var hitResults = new NativeArray<XRRaycastHit>(boundingBoxHitCount, allocator);
            NativeArray<XRRaycastHit>.Copy(hitBuffer, hitResults, boundingBoxHitCount);
            hitBuffer.Dispose();
            return hitResults;
        }

        bool TryRaycastZAxisFace(
            ARBoundingBox boundingBox,
            Ray ray,
            int sign,
            NativeArray<XRRaycastHit> hitBuffer,
            int hitBufferIndex,
            ref float closestHitDistance)
        {
            var boundingBoxTransform = boundingBox.transform;

            var faceNormal = sign * boundingBoxTransform.forward;
            var faceOffset = faceNormal * (boundingBox.size.z * 0.5f);
            var facePosition = boundingBox.pose.position + faceOffset;
            var infiniteFacePlane = new Plane(faceNormal, facePosition);

            // return early if the ray does not intersect the infinite plane
            // of the face
            if (!infiniteFacePlane.Raycast(ray, out var distance))
                return false;

            // rotate the hit pose so the up vector aligns with the normal of the plane
            // and the forward vector faces up
            var hitPose = new Pose(
                ray.origin + ray.direction * distance,
                boundingBox.transform.localRotation * Quaternion.Euler(-90, (1 + sign) * 90, 0));

            var localHitPosition = boundingBox.transform.InverseTransformPoint(hitPose.position);

            if (Mathf.Abs(localHitPosition.x) <= boundingBox.size.x * 0.5f &&
                Mathf.Abs(localHitPosition.y) <= boundingBox.size.y * 0.5f)
            {
                // we return early because the distance to this face is further than
                // the closest distance and we only want to update the data in the hitbuffer
                // for the closest hits. We return true to notify the caller that the face
                // was hit.
                if (distance > closestHitDistance)
                    return true;

                closestHitDistance = distance;
                hitBuffer[hitBufferIndex] = new(
                    boundingBox.trackableId,
                    hitPose,
                    distance,
                    TrackableType.BoundingBox);

                return true;
            }

            return false;
        }

        bool TryRaycastXAxisFace(
            ARBoundingBox boundingBox,
            Ray ray,
            int sign,
            NativeArray<XRRaycastHit> hitBuffer,
            int hitBufferIndex,
            ref float closestHitDistance)
        {
            var boundingBoxTransform = boundingBox.transform;

            var faceNormal = sign * boundingBoxTransform.right;
            var faceOffset = faceNormal * (boundingBox.size.x * 0.5f);
            var facePosition = boundingBox.pose.position + faceOffset;
            var infiniteFacePlane = new Plane(faceNormal, facePosition);

            // return early if the ray does not intersect the infinite plane
            // of the face
            if (!infiniteFacePlane.Raycast(ray, out var distance))
                return false;

            // rotate the hit pose so the up vector aligns with the normal of the plane
            // and the forward vector faces up
            var hitPose = new Pose(
                ray.origin + ray.direction * distance,
                boundingBox.transform.localRotation * Quaternion.Euler(-90, 0, -sign * 90));

            var localHitPosition = boundingBox.transform.InverseTransformPoint(hitPose.position);

            if (Mathf.Abs(localHitPosition.z) <= boundingBox.size.z * 0.5f &&
                Mathf.Abs(localHitPosition.y) <= boundingBox.size.y * 0.5f)
            {
                // we return early because the distance to this face is further than
                // the closest distance and we only want to update the data in the hitbuffer
                // for the closest hits. We return true to notify the caller that the face
                // was hit.
                if (distance > closestHitDistance)
                    return true;

                closestHitDistance = distance;
                hitBuffer[hitBufferIndex] = new(
                    boundingBox.trackableId,
                    hitPose,
                    distance,
                    TrackableType.BoundingBox);

                return true;
            }

            return false;
        }

        bool TryRaycastYAxisFace(
            ARBoundingBox boundingBox,
            Ray ray,
            int sign,
            NativeArray<XRRaycastHit> hitBuffer,
            int hitBufferIndex,
            ref float closestHitDistance)
        {
            var boundingBoxTransform = boundingBox.transform;

            var faceNormal = sign * boundingBoxTransform.up;
            var faceOffset = faceNormal * (boundingBox.size.y * 0.5f);
            var facePosition = boundingBox.pose.position + faceOffset;
            var infiniteFacePlane = new Plane(faceNormal, facePosition);

            // return early if the ray does not intersect the infinite plane
            // of the face
            if (!infiniteFacePlane.Raycast(ray, out var distance))
                return false;

            // rotate the hit pose so the up vector aligns with the normal of the plane
            // and the forward vector matches the bounding box forward for the top face or
            // mirrors it for the bottom face.
            var hitPose = new Pose(
                ray.origin + ray.direction * distance,
                boundingBox.transform.localRotation * Quaternion.Euler(0, 0, (1 - sign) * 90));

            var localHitPosition = boundingBox.transform.InverseTransformPoint(hitPose.position);

            if (Mathf.Abs(localHitPosition.x) <= boundingBox.size.x * 0.5f &&
                Mathf.Abs(localHitPosition.z) <= boundingBox.size.z * 0.5f)
            {
                // we return early because the distance to this face is further than
                // the closest distance and we only want to update the data in the hitbuffer
                // for the closest hits. We return true to notify the caller that the face
                // was hit.
                if (distance > closestHitDistance)
                    return true;

                closestHitDistance = distance;
                hitBuffer[hitBufferIndex] = new(
                    boundingBox.trackableId,
                    hitPose,
                    distance,
                    TrackableType.BoundingBox);

                return true;
            }

            return false;
        }
    }
}
