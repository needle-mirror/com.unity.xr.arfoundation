namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents an external texture and its property name ID.
    /// </summary>
    public readonly struct ARGpuTexture
    {
        /// <summary>
        /// An external texture that exists only on the GPU. To use the texture on the CPU, you must read it back from
        /// the GPU using [Texture2D.ReadPixels](xref:UnityEngine.Texture2D.ReadPixels(UnityEngine.Rect,System.Int32,System.Int32,System.Boolean)).
        /// </summary>
        public Texture texture { get; }

        /// <summary>
        /// ID of the shader property associated with this texture.
        /// </summary>
        /// <seealso href="xref:UnityEngine.Shader.PropertyToID(System.String)"/>
        public int propertyId { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="propertyId">The texture's property name ID.</param>
        public ARGpuTexture(Texture texture, int propertyId)
        {
            this.texture = texture;
            this.propertyId = propertyId;
        }
    }
}
