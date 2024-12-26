using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents a field of view as angles in radians from the center point.
    /// </summary>
    /// <remarks>
    /// This struct is a C# representation of the OpenXR XrFovf struct.
    /// Refer to <a href="https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html#fundamentals-angles">the Open XR specification</a>.
    /// </remarks>
    public readonly struct XRFov : IEquatable<XRFov>
    {
        /// <summary>
        /// Gets the angle in radians between the left edge and center of the frame.
        /// </summary>
        /// <value>The angle in radians between the left edge and center of the frame.</value>
        public float angleLeft => m_AngleLeft;
        readonly float m_AngleLeft;

        /// <summary>
        /// Gets the angle in radians between the right edge and center of the frame.
        /// </summary>
        /// <value>The angle in radians between the right edge and center of the frame.</value>
        public float angleRight => m_AngleRight;
        readonly float m_AngleRight;

        /// <summary>
        /// Gets the angle in radians between the upper edge and center of the frame.
        /// </summary>
        /// <value>The angle in radians between the upper edge and center of the frame.</value>
        public float angleUp => m_AngleUp;
        readonly float m_AngleUp;

        /// <summary>
        /// Gets the angle in radians between the lower edge and center of the frame.
        /// </summary>
        /// <value>The angle in radians between the lower edge and center of the frame.</value>
        public float angleDown => m_AngleDown;
        readonly float m_AngleDown;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="left">The angle in radians between the left edge and center of the frame.</param>
        /// <param name="right">The angle in radians between the right edge and center of the frame.</param>
        /// <param name="up">The angle in radians between the upper edge and center of the frame.</param>
        /// <param name="down">The angle in radians between the lower edge and center of the frame.</param>
        public XRFov(float left, float right, float up, float down)
        {
            m_AngleLeft = left;
            m_AngleRight = right;
            m_AngleUp = up;
            m_AngleDown = down;
        }

        /// <summary>
        /// Returns this instance as a <see cref="Vector4"/> with the following field mappings:
        /// x | angleLeft
        /// y | angleRight
        /// z | angleUp
        /// w | angleDown
        /// </summary>
        /// <returns>This <see cref="XRFov"/> as a <see cref="Vector4"/>.</returns>
        public Vector4 AsVector4() => new(angleLeft, angleRight, angleUp, angleDown);

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRFov"/> to compare against.</param>
        /// <returns><see langword="true"/> if the <see cref="XRFov"/> represents the same object.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRFov other)
        {
            return angleLeft == other.angleLeft
                && angleRight == other.angleRight
                && angleUp == other.angleUp
                && angleDown == other.angleDown;
        }

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="obj">An <c>object</c> to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an <see cref="XRFov"/> and
        /// <see cref="Equals(XRFov)"/> is also <see langword="true"/>. Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(System.Object obj) => obj is XRFov fov && Equals(fov);

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for equality using <see cref="Equals(XRFov)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XRFov"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XRFov"/> of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XRFov lhs, XRFov rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for inequality using <see cref="Equals(XRFov)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XRFov"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XRFov"/> of the comparison.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="true"/>.</returns>
        public static bool operator !=(XRFov lhs, XRFov rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Generates a hash code suitable for use in <c>HashSet</c> and <c>Dictionary</c>.
        /// </summary>
        /// <returns>A hash of this <see cref="XRFov"/>.</returns>
        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = (hashCode * 486187739) + angleLeft.GetHashCode();
                hashCode = (hashCode * 486187739) + angleRight.GetHashCode();
                hashCode = (hashCode * 486187739) + angleUp.GetHashCode();
                hashCode = (hashCode * 486187739) + angleDown.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
            => $"{{ left: {m_AngleLeft}, right: {m_AngleRight}, up: {m_AngleUp}, down: {m_AngleDown} }}";
    }
}
