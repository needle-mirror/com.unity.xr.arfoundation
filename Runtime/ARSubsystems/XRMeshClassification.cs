namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The semantic classification of an XR mesh or mesh component.
    /// </summary>
    public enum XRMeshClassification : uint
    {
        /// <summary>
        /// Unknown/no mesh classification
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Generic "other" mesh classification
        /// </summary>
        Other = 1,

        /// <summary>
        /// Mesh classification as a floor
        /// </summary>
        Floor = 2,

        /// <summary>
        /// Mesh classification as a ceiling
        /// </summary>
        Ceiling = 3,

        /// <summary>
        /// Mesh classification as a wall
        /// </summary>
        Wall = 4,

        /// <summary>
        /// Mesh classification as a table
        /// </summary>
        Table = 5,

        /// <summary>
        /// Mesh classification as a seat/chair
        /// </summary>
        Seat = 6,

        /// <summary>
        /// Mesh classification as a window
        /// </summary>
        Window = 7,

        /// <summary>
        /// Mesh classification as a door
        /// </summary>
        Door = 8,
    }
}
