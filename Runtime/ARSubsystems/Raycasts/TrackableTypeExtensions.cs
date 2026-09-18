namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Extension methods for <see cref="TrackableType"/>.
    /// </summary>
    public static class TrackableTypeExtensions
    {
        const TrackableType k_AllPlaneTypes =
            TrackableType.PlaneWithinPolygon |
            TrackableType.PlaneWithinBounds |
            TrackableType.PlaneWithinInfinity |
            TrackableType.PlaneEstimated;

        /// <summary>
        /// Determines whether <paramref name="trackableType"/> refers to, or includes, any plane type.
        /// </summary>
        /// <param name="trackableType">The trackable type or type mask to test.</param>
        /// <returns><see langword="true"/> if <paramref name="trackableType"/> contains any plane
        /// sub-type flag; otherwise, <see langword="false"/>.</returns>
        public static bool IsPlane(this TrackableType trackableType) => (trackableType & k_AllPlaneTypes) != 0;

        /// <summary>
        /// The bitwise combination of every plane sub-type flag (for example,
        /// <see cref="TrackableType.PlaneWithinPolygon"/> and <see cref="TrackableType.PlaneWithinBounds"/>, etc).
        /// </summary>
        public static TrackableType allPlaneTypes => k_AllPlaneTypes;
    }
}
