#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using System.Text;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARCameraManager.frameReceived"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="CameraFrameReceivedEventUnit"/>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public sealed class ARCameraManagerListener : MessageListener
    {
        ARCameraManager m_Manager;
        StringBuilder m_LogBuilder = new();

        static readonly Type k_HookNameKey = typeof(ARCameraFrameEventArgs);

        void OnEnable()
        {
            m_Manager = GetComponent<ARCameraManager>();
            if (m_Manager == null)
            {
                m_Manager = FindAnyObjectByType<ARCameraManager>();
                if (m_Manager == null)
                    throw new InvalidOperationException(
                        $"{GetType().Name} requires an active and enabled {nameof(ARCameraManager)} in the scene, but none was found.");

                m_LogBuilder.Append($"{GetType().Name} requires an active and enabled {nameof(ARCameraManager)} in the scene,");
                m_LogBuilder.Append($" but but there was no such component on target GameObject {name}.");
                m_LogBuilder.Append($" A runtime search was able to locate the component on GameObject {m_Manager.gameObject.name}.");
                m_LogBuilder.Append($" For better performance, you should use {m_Manager.gameObject.name} instead of {name}");
                m_LogBuilder.Append(" as your Target input in Visual Scripting graphs.");
                Debug.LogWarning(m_LogBuilder.ToString());
                m_LogBuilder.Clear();
            }

            m_Manager.frameReceived += OnFrameReceived;
        }

        void OnDisable()
        {
            if (m_Manager != null)
                m_Manager.frameReceived -= OnFrameReceived;
        }

        void OnFrameReceived(ARCameraFrameEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames[k_HookNameKey], gameObject, args);
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
