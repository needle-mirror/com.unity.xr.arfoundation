using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines a fluent API for constructing an <see cref="XRReferenceImage"/>.
    /// </summary>
    public class XRReferenceImageBuilder
    {
        SerializableGuid m_Guid;
        SerializableGuid m_TextureGuid;
        Vector2? m_Size;
        string m_Name;
        Texture2D m_Texture;
        MovingImageType m_MovingImageType;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public XRReferenceImageBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Initialize all fields using data from the given reference image.
        /// </summary>
        /// <param name="referenceImage">The reference image.</param>
        /// <returns>This instance.</returns>
        public XRReferenceImageBuilder FromReferenceImage(XRReferenceImage referenceImage)
        {
            m_Guid = referenceImage.m_SerializedGuid;
            m_TextureGuid = referenceImage.m_SerializedTextureGuid;
            m_Size = referenceImage.specifySize ? referenceImage.size : (Vector2?)null;
            m_Name = referenceImage.name;
            m_Texture = referenceImage.texture;
            m_MovingImageType = referenceImage.movingImageType;
            return this;
        }

        /// <summary>
        /// Reset the instance to default values.
        /// </summary>
        public void Reset()
        {
            m_Guid = default;
            m_TextureGuid = default;
            m_Size = default;
            m_Name = null;
            m_Texture = null;
            m_MovingImageType = MovingImageType.Unspecified;
        }

        /// <summary>
        /// Set the <see cref="SerializableGuid"/> for built `XRReferenceImage` instances.
        /// </summary>
        /// <param name="guid">The <see cref="SerializableGuid"/> associated with the reference image.</param>
        /// <returns>This instance.</returns>
        public XRReferenceImageBuilder WithGuid(SerializableGuid guid)
        {
            m_Guid = guid;
            return this;
        }

        /// <summary>
        /// Set the texture <see cref="SerializableGuid"/> for built `XRReferenceImage` instances.
        /// </summary>
        /// <param name="textureGuid">The <see cref="SerializableGuid"/> of the source texture as it appears in the
        /// [AssetDatabase](https://docs.unity3d.com/ScriptReference/AssetDatabase.html) in the Editor.</param>
        /// <returns>This instance.</returns>
        public XRReferenceImageBuilder WithTextureGuid(SerializableGuid textureGuid)
        {
            m_TextureGuid = textureGuid;
            return this;
        }

        /// <summary>
        /// Set the physical size, in meters, for built `XRReferenceImage` instances.
        /// </summary>
        /// <param name="size">The size of the image, in meters, or `null` to leave the size unspecified.
        /// Specifying a size can improve image detection, and might be required by some platforms.</param>
        /// <returns>This instance.</returns>
        public XRReferenceImageBuilder WithSize(Vector2? size)
        {
            m_Size = size;
            return this;
        }

        /// <summary>
        /// Set the name for built `XRReferenceImage` instances.
        /// </summary>
        /// <param name="name">A name associated with the reference image.</param>
        /// <returns>This instance.</returns>
        public XRReferenceImageBuilder WithName(string name)
        {
            m_Name = name;
            return this;
        }

        /// <summary>
        /// Set the source texture for built `XRReferenceImage` instances.
        /// </summary>
        /// <param name="texture">The source texture which the reference image represents. This can be `null`
        /// to avoid including the texture in the Player build.</param>
        /// <returns>This instance.</returns>
        public XRReferenceImageBuilder WithTexture(Texture2D texture)
        {
            m_Texture = texture;
            return this;
        }

        /// <summary>
        /// Set the moving image type for built `XRReferenceImage` instances.
        /// </summary>
        /// <param name="movingImageType">Indicates whether the image is expected to move in the real world.
        /// This overrides the moving image setting on the image library that contains it.</param>
        /// <returns>This instance.</returns>
        public XRReferenceImageBuilder WithMovingImageType(MovingImageType movingImageType)
        {
            m_MovingImageType = movingImageType;
            return this;
        }

        /// <summary>
        /// Build an output `XRReferenceImage` using all this builder's parameter values.
        /// </summary>
        /// <returns>The built `XRReferenceImage`.</returns>
        public XRReferenceImage Build()
        {
            var image = new XRReferenceImage(
                m_Guid,
                m_TextureGuid,
                m_Size,
                m_Name,
                m_Texture,
                m_MovingImageType);

            if (image.guid == Guid.Empty)
            {
                Debug.LogWarning(
                    "XRReferenceImageBuilder.Build: built a reference image with an empty GUID. Call WithGuid to " +
                    "assign a unique GUID, or image identity and lookup will collide.");
            }

            return image;
        }
    }
}
