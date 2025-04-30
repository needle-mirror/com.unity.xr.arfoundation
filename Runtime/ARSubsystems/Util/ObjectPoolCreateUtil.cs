using UnityEngine.Pool;

namespace UnityEngine.XR.ARSubsystems
{
    interface IReleasable
    {
        public void Release();
    }

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

        internal static ObjectPool<TReleasable> CreateWithReleaseTrigger<TReleasable>(
            int defaultCapacity = 8, int maxSize = 1024)
            where TReleasable : class, IReleasable, new()
        {
            return new ObjectPool<TReleasable>(
                createFunc: () => new TReleasable(),
                actionOnGet: null,
                actionOnRelease: x => x.Release(),
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);
        }
    }
}
