using UnityEngine.Pool;

namespace UnityEngine.XR.ARSubsystems
{
    static class ObjectPoolCreateUtil
    {
        internal static ObjectPool<T> Create<T>(int defaultCapacity = 8, int maxSize = 1024) where T : class, new()
        {
            return new ObjectPool<T>(
                createFunc: () => new T(),
                actionOnGet: null,
                actionOnRelease: null,
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);
        }
    }
}
