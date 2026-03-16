using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation
{
    internal static class FallbackRaycastRegistry
    {
        internal static HashSet<IRaycaster> Raycasters => m_Raycasters;

        internal static ARPlaneManager CachedPlaneManager => m_CachedPlaneManager;

        internal static ARBoundingBoxManager CachedBoundingBoxManager => m_CachedBoundingBoxManager;

        static HashSet<IRaycaster> m_Raycasters = new();
        static ARPlaneManager m_CachedPlaneManager;
        static ARBoundingBoxManager m_CachedBoundingBoxManager;

        /// <summary>
        /// Allows AR managers to register themselves as a raycaster.
        /// Raycasters can be used as a fallback method if the AR platform does
        /// not support raycasting using arbitrary <c>Ray</c>s.
        /// </summary>
        /// <param name="raycaster">A raycaster implementing the IRaycast interface.</param>
        internal static void RegisterRaycaster(IRaycaster raycaster)
        {
            var raycasterType = raycaster.GetType();
            if (raycasterType == typeof(ARPlaneManager))
                m_CachedPlaneManager = raycaster as ARPlaneManager;
            else if (raycasterType == typeof(ARBoundingBoxManager))
                m_CachedBoundingBoxManager = raycaster as ARBoundingBoxManager;

            m_Raycasters.Add(raycaster);
        }

        /// <summary>
        /// Unregisters a raycaster previously registered with <see cref="RegisterRaycaster(IRaycaster)"/>.
        /// </summary>
        /// <param name="raycaster">A raycaster to use as a fallback, if needed.</param>
        internal static void UnregisterRaycaster(IRaycaster raycaster)
        {
            if (m_Raycasters != null)
                m_Raycasters.Remove(raycaster);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearStaticState()
        {
            m_Raycasters?.Clear();
            m_CachedBoundingBoxManager = null;
            m_CachedPlaneManager = null;
        }
    }
}
