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
        /// The bounding box is classified as a bathtub.
        /// </summary>
        Bathtub = 1u << 7,

        /// <summary>
        /// The bounding box is classified as a chair.
        /// </summary>
        Chair = 1u << 8,

        /// <summary>
        /// The bounding box is classified as a dishwasher.
        /// </summary>
        Dishwasher = 1u << 9,

        /// <summary>
        /// The bounding box is classified as a fireplace.
        /// </summary>
        Fireplace = 1u << 10,

        /// <summary>
        /// The bounding box is classified as an oven.
        /// </summary>
        Oven = 1u << 11,

        /// <summary>
        /// The bounding box is classified as a refrigerator.
        /// </summary>
        Refrigerator = 1u << 12,

        /// <summary>
        /// The bounding box is classified as a sink.
        /// </summary>
        Sink = 1u << 13,

        /// <summary>
        /// The bounding box is classified as stairs.
        /// </summary>
        Stairs = 1u << 14,

        /// <summary>
        /// The bounding box is classified as a stove.
        /// </summary>
        Stove = 1u << 15,

        /// <summary>
        /// The bounding box is classified as a toilet.
        /// </summary>
        Toilet = 1u << 16,

        /// <summary>
        /// The bounding box is classified as a washer or dryer.
        /// </summary>
        WasherDryer = 1u << 17,

        /// <summary>
        /// The bounding box is classified as other.
        /// </summary>
        Other = 1u << 31
    }
}
