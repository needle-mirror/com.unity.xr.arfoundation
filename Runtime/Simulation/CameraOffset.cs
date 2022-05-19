using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Defines the API for applying Camera Offsets
    /// </summary>
    class CameraOffset
    {
        Vector3 m_PositionOffset;
        Quaternion m_RotationOffset;
        Vector3 m_ScaleOffset;
        Matrix4x4 m_OffsetMatrix;

        Quaternion m_InverseRotationOffset;
        Vector3 m_InverseScaleOffset;
        Matrix4x4 m_InverseOffsetMatrix;

        public CameraOffset(Camera camera)
        {
            var cameraTransform = camera.transform;

            m_PositionOffset = cameraTransform.position;
            m_RotationOffset = cameraTransform.rotation;
            m_ScaleOffset = cameraTransform.lossyScale;
            m_OffsetMatrix = Matrix4x4.TRS(m_PositionOffset, m_RotationOffset, m_ScaleOffset);

            m_InverseRotationOffset = Quaternion.Inverse(m_RotationOffset);
            m_InverseScaleOffset = m_ScaleOffset.Inverse();
            m_InverseOffsetMatrix = m_OffsetMatrix.inverse;
        }

        /// <summary>
        /// Apply the camera offset to a position and return the modified position
        /// </summary>
        /// <param name="position">The position to which the offset will be applied</param>
        /// <returns>The modified position</returns>
        public Vector3 ApplyToPosition(Vector3 position)
        {
            var newPosition = m_RotationOffset * position;
            newPosition.Scale(m_ScaleOffset);

            return newPosition + m_PositionOffset;
        }

        /// <summary>
        /// Apply the camera offset to a rotation and return the modified rotation
        /// </summary>
        /// <param name="rotation">The rotation to which the offset will be applied</param>
        /// <returns>The modified rotation</returns>
        public Quaternion ApplyToRotation(Quaternion rotation)
        {
            return m_RotationOffset * rotation;
        }

        /// <summary>
        /// Apply the camera offset to a direction and return the modified direction
        /// </summary>
        /// <param name="direction">The direction to which the offset will be applied</param>
        /// <returns>The modified direction</returns>
        public Vector3 ApplyToDirection(Vector3 direction)
        {
            return m_RotationOffset * direction;
        }

        /// <summary>
        /// Apply the inverse of the camera offset to a position and return the modified position
        /// </summary>
        /// <param name="position">The position to which the offset will be applied</param>
        /// <returns>The modified position</returns>
        public Vector3 ApplyInverseToPosition(Vector3 position)
        {
            var newPosition = m_InverseRotationOffset * (position - m_PositionOffset);
            newPosition.Scale(m_InverseScaleOffset);

            return newPosition;
        }

        /// <summary>
        /// Apply the inverse of the camera offset to a rotation and return the modified rotation
        /// </summary>
        /// <param name="rotation">The rotation to which the offset will be applied</param>
        /// <returns>The modified rotation</returns>
        public Quaternion ApplyInverseToRotation(Quaternion rotation)
        {
            return m_InverseRotationOffset * rotation;
        }

        /// <summary>
        /// Apply the inverse of the camera offset to a direction and return the modified direction
        /// </summary>
        /// <param name="direction">The direction to which the offset will be applied</param>
        /// <returns>The modified direction</returns>
        public Vector3 ApplyInverseToDirection(Vector3 direction)
        {
            return m_InverseRotationOffset * direction;
        }
    }
}
