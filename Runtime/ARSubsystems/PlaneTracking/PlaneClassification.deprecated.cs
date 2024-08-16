using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Indicates the semantic classification of the plane.
    /// </summary>
    /// <seealso cref="BoundedPlane.classification"/>
    [Obsolete("PlaneClassification has been deprecated in AR Foundation version 6.0. Use PlaneClassifications instead.")]
    public enum PlaneClassification
    {
        /// <summary>
        /// The plane does not match any available classification.
        /// </summary>
        None = 0,

        /// <summary>
        /// The plane is classified as a wall.
        /// </summary>
        Wall,

        /// <summary>
        /// The plane is classified as the floor.
        /// </summary>
        Floor,

        /// <summary>
        /// The plane is classified as the ceiling.
        /// </summary>
        Ceiling,

        /// <summary>
        /// The plane is classified as a table.
        /// </summary>
        Table,

        /// <summary>
        /// The plane is classified as a seat.
        /// </summary>
        Seat,

        /// <summary>
        /// The plane is classified as a door.
        /// </summary>
        Door,

        /// <summary>
        /// The plane is classified as a window.
        /// </summary>
        Window,

        /// <summary>
        /// The plane is classified as other.
        /// </summary>
        Other = int.MaxValue,
    }

    /// <summary>
    /// Utility methods for converting between <see cref="PlaneClassification"/> and <see cref="PlaneClassifications"/> values. These methods will be removed once the deprecated `PlaneClassification` enumeration is removed.
    /// </summary>
    [Obsolete("PlaneClassification has been deprecated in AR Foundation version 6.0. Use PlaneClassifications instead.")]
    public static class PlaneClassificationExtensions
    {
        /// <summary>
        /// Converts from <see cref="PlaneClassifications"/> to <see cref="PlaneClassification"/> .
        /// </summary>
        /// <param name="self">The <see cref="PlaneClassification"/> being extended.</param>
        /// <param name="classifications">The <see cref="PlaneClassifications"/> to be converted.</param>
        public static void ConvertFromPlaneClassifications(this PlaneClassification self, PlaneClassifications classifications)
        {
            if (classifications.HasFlag(PlaneClassifications.None))
            {
                self = PlaneClassification.None;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.WallFace))
            {
                self = PlaneClassification.Wall;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.Floor))
            {
                self = PlaneClassification.Floor;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.Ceiling))
            {
                self = PlaneClassification.Ceiling;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.Table))
            {
                self = PlaneClassification.Table;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.Couch))
            {
                self = PlaneClassification.Seat;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.DoorFrame))
            {
                self = PlaneClassification.Door;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.WindowFrame))
            {
                self = PlaneClassification.Window;
                return;
            }
            if (classifications.HasFlag(PlaneClassifications.Other))
            {
                self = PlaneClassification.Other;
                return;
            }

            self = PlaneClassification.Other;
        }

        /// <summary>
        /// Converts from <see cref="PlaneClassification"/> to <see cref="PlaneClassifications"/> .
        /// </summary>
        /// <param name="self">The <see cref="PlaneClassification"/> being extended.</param>
        /// <returns>The converted value <see cref="PlaneClassifications"/> flags.</returns>
        public static PlaneClassifications ConvertToPlaneClassifications(this PlaneClassification self)
        {
            switch (self)
            {
                case PlaneClassification.None:
                    return PlaneClassifications.None;
                case PlaneClassification.Wall:
                    return PlaneClassifications.WallFace;
                case PlaneClassification.Floor:
                    return PlaneClassifications.Floor;
                case PlaneClassification.Ceiling:
                    return PlaneClassifications.Ceiling;
                case PlaneClassification.Table:
                    return PlaneClassifications.Table;
                case PlaneClassification.Seat:
                    return PlaneClassifications.Seat;
                case PlaneClassification.Door:
                    return PlaneClassifications.DoorFrame;
                case PlaneClassification.Window:
                    return PlaneClassifications.WindowFrame;
                case PlaneClassification.Other:
                    return PlaneClassifications.Other;
                default:
                    return PlaneClassifications.Other;
            }
        }
    }
}
