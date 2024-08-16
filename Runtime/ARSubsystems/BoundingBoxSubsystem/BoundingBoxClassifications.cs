using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the semantic classifications of a 3D bounding box.
    /// </summary>
    /// <remarks>
    /// An XR provider might not support 3D bounding box classifications. You can check
    /// <see cref='XRBoundingBoxSubsystemDescriptor.supportsClassifications'/> to determine whether
    /// the current provider can classify 3D bounding boxes.
    /// </remarks>
    [Flags]
    public enum BoundingBoxClassifications : uint
    {
        /// <summary>
        /// No classification is assigned.
        /// </summary>
        None = 0,

        /// <summary>
        /// The bounding box is classified as a couch.
        /// </summary>
        Couch = 1u << 0,

        /// <summary>
        /// The bounding box is classified as a table.
        /// </summary>
        Table = 1u << 1,

        /// <summary>
        /// The bounding box is classified as a bed.
        /// </summary>
        Bed = 1u << 2,

        /// <summary>
        /// The bounding box is classified as a lamp.
        /// </summary>
        Lamp = 1u << 3,

        /// <summary>
        /// The bounding box is classified as a plant.
        /// </summary>
        Plant = 1u << 4,

        /// <summary>
        /// The bounding box is classified as a screen.
        /// </summary>
        Screen = 1u << 5,

        /// <summary>
        /// The bounding box is classified as storage.
        /// </summary>
        Storage = 1u << 6,

        /// <summary>
        /// The bounding box is classified as other.
        /// </summary>
        Other = 1u << 31
    }
}
