#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using System.Text;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// `MessageListener` base class for all
    /// <see cref="ARTrackableManager{TSubsystem,TSubsystemDescriptor,TProvider,TSessionRelativeData,TTrackable}"/> types.
    /// </summary>
    /// <typeparam name="TManager">The `ARTrackableManager` type.</typeparam>
    /// <typeparam name="TTrackable">The trackable type.</typeparam>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public abstract class ARTrackableManagerListener<TManager, TTrackable> : MessageListener
        where TManager : MonoBehaviour, ITrackablesChanged<TTrackable>
        where TTrackable : ARTrackable
    {
        TManager m_Manager;
        StringBuilder m_LogBuilder = new();

        static readonly Type k_HookNameKey = typeof(ARTrackablesChangedEventArgs<TTrackable>);

        void OnEnable()
        {
            m_Manager = GetComponent<TManager>();
            if (m_Manager == null)
            {
                m_Manager = FindAnyObjectByType<TManager>();
                if (m_Manager == null)
                    throw new InvalidOperationException(
                        $"{GetType().Name} requires an active and enabled {typeof(TManager).Name} in the scene, but none was found.");

                m_LogBuilder.Append($"{GetType().Name} requires an active and enabled {typeof(TManager).Name} in the scene,");
                m_LogBuilder.Append($" but but there was no such component on target GameObject {name}.");
                m_LogBuilder.Append($" A runtime search was able to locate the component on GameObject {m_Manager.gameObject.name}.");
                m_LogBuilder.Append($" For better performance, you should use {m_Manager.gameObject.name} instead of {name}");
                m_LogBuilder.Append(" as your Target input in visual scripting graphs.");
                Debug.LogWarning(m_LogBuilder.ToString());
                m_LogBuilder.Clear();
            }

            m_Manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }

        void OnDisable()
        {
            m_Manager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }

        void OnTrackablesChanged(ARTrackablesChangedEventArgs<TTrackable> args)
        {
            Debug.Log(typeof(ARTrackablesChangedEventArgs<TTrackable>));
            EventBus.Trigger(Constants.EventHookNames[k_HookNameKey], gameObject, args);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
