using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the semantic classifications of a plane.
    /// </summary>
    /// <remarks>
    /// An XR provider might not support plane classification. You can check
    /// <see cref='XRPlaneSubsystemDescriptor.supportsClassification'/> to determine whether
    /// the current provider can classify planes.
    ///
    /// An XR provider might not support all of the classifications in this list.
    ///
    /// Different XR providers might assign different semantic classifications in the same situation.
    /// </remarks>
    [Flags]
    public enum PlaneClassifications : uint
    {
        /// <summary>
        /// No classification is assigned.
        /// </summary>
        None = 0,

        /// <summary>
        /// The plane is classified as the ceiling.
        /// </summary>
        Ceiling = 1u << 0,

        /// <summary>
        /// The plane is classified as a door frame.
        /// </summary>
        DoorFrame = 1u << 1,

        /// <summary>
        /// The plane is classified as the floor.
        /// </summary>
        Floor = 1u << 2,

        /// <summary>
        /// The plane is classified as wall art.
        /// </summary>
        WallArt = 1u << 3,

        /// <summary>
        /// The plane is classified as a wall face.
        /// </summary>
        WallFace = 1u << 4,

        /// <summary>
        /// The plane is classified as a window frame.
        /// </summary>
        WindowFrame = 1u << 5,

        /// <summary>
        /// The plane is classified as a couch.
        /// </summary>
        Couch = 1u << 6,

        /// <summary>
        /// The plane is classified as a seat.
        /// </summary>
        Seat = 1u << 7,

        /// <summary>
        /// The plane is classified as any type of seat.
        /// </summary>
        SeatOfAnyType = Couch | Seat,

        /// <summary>
        /// The plane is classified as a table.
        /// </summary>
        Table = 1u << 8,

        /// <summary>
        /// The plane is classified as an invisible wall face .
        /// </summary>
        InvisibleWallFace = 1u << 9,

        /// <summary>
        /// The plane is classified as other.
        /// </summary>
        Other = 1u << 31
    }
}
