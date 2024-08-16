#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Base unit descriptor for units derived from
    /// <see cref="ARTrackablesChangedEventUnit{TManager, TTrackable, TListener}"/>.
    /// </summary>
    /// <typeparam name="TManager">The manager type.</typeparam>
    /// <typeparam name="TTrackable">The type of trackable for which we are listening.</typeparam>
    /// <typeparam name="TListener">The message listener type.</typeparam>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    public abstract class ARTrackablesChangedEventUnitDescriptor<TManager, TTrackable, TListener>
        : UnitDescriptor<ARTrackablesChangedEventUnit<TManager, TTrackable, TListener>>
        where TManager : MonoBehaviour, ITrackablesChanged<TTrackable>
        where TTrackable : ARTrackable
        where TListener : MessageListener
    {
        static readonly StringBuilder s_Builder = new();

        /// <summary>
        /// The unit description.
        /// </summary>
        protected virtual string unitDescription => s_UnitDescription;
        static string s_UnitDescription;

        /// <summary>
        /// The Target port description.
        /// </summary>
        protected virtual string targetPortDescription => s_TargetPortDescription;
        static string s_TargetPortDescription;

        /// <summary>
        /// The Added port description.
        /// </summary>
        protected virtual string addedPortDescription => s_AddedPortDescription;
        static string s_AddedPortDescription = $"{typeof(TTrackable).DisplayName()}s that have been added.";

        /// <summary>
        /// The Updated port description.
        /// </summary>
        protected virtual string updatedPortDescription => s_UpdatedPortDescription;
        static string s_UpdatedPortDescription = $"{typeof(TTrackable).DisplayName()}s that have been updated.";

        /// <summary>
        /// The Removed port description.
        /// </summary>
        protected virtual string removedPortDescription => s_RemovedPortDescription;
        static string s_RemovedPortDescription = $"{typeof(TTrackable).DisplayName()}s that have been removed.";

        /// <inheritdoc/>
        protected ARTrackablesChangedEventUnitDescriptor(
            ARTrackablesChangedEventUnit<TManager, TTrackable, TListener> target)
            : base(target)
        {
            s_Builder.AppendLine($"Triggers when {typeof(TTrackable).DisplayName()}s have changed.");
            s_Builder.Append($"{typeof(TTrackable).DisplayName()}s can be added, updated, and/or removed every ");
            s_Builder.Append($"frame if there is an enabled {typeof(TManager).DisplayName()} in the scene.");
            s_UnitDescription = s_Builder.ToString();
            s_Builder.Clear();

            s_Builder.AppendLine($"Target GameObject should have an enabled {typeof(TManager).DisplayName()} component.");
            s_Builder.Append("If you do not connect this port, this node searches for an enabled ");
            s_Builder.Append($"{typeof(TManager).DisplayName()} instead, and throws an exception if none is found.");
            s_TargetPortDescription = s_Builder.ToString();
            s_Builder.Clear();
        }

        /// <summary>
        /// Render a summary under the unit title in the Graph Inspector.
        /// </summary>
        /// <returns>The summary.</returns>
        protected override string DefinedSummary() => unitDescription;

        /// <summary>
        /// Render summaries of each port in the Graph Inspector.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="portDescription">The port description to write to.</param>
        protected override void DefinedPort(IUnitPort port, UnitPortDescription portDescription)
        {
            base.DefinedPort(port, portDescription);

            switch (port.key)
            {
                case nameof(target.target):
                    portDescription.summary = targetPortDescription;
                    break;
                case nameof(target.added):
                    portDescription.summary = addedPortDescription;
                    break;
                case nameof(target.updated):
                    portDescription.summary = updatedPortDescription;
                    break;
                case nameof(target.removed):
                    portDescription.summary = removedPortDescription;
                    break;
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
