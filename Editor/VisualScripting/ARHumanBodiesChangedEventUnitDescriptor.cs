#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Text;
using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="ARHumanBodiesChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(ARHumanBodiesChangedEventUnit))]
    public sealed class ARHumanBodiesChangedEventUnitDescriptor : ARTrackablesChangedEventUnitDescriptor<
        ARHumanBodyManager, ARHumanBody, ARHumanBodiesChangedListener>
    {
        static StringBuilder s_Builder = new();

        /// <inheritdoc/>
        protected override string unitDescription => s_UnitDescription;
        static string s_UnitDescription;

        /// <inheritdoc/>
        protected override string addedPortDescription => "AR Human Bodies that have been added.";

        /// <inheritdoc/>
        protected override string updatedPortDescription => "AR Human Bodies that have been updated.";

        /// <inheritdoc/>
        protected override string removedPortDescription => "AR Human Bodies that have been removed.";

        /// <inheritdoc/>
        public ARHumanBodiesChangedEventUnitDescriptor(ARHumanBodiesChangedEventUnit target) : base(target)
        {
            if (s_UnitDescription != null)
                return;

            s_Builder.AppendLine("Triggers when AR Human Bodies have changed.");
            s_Builder.Append("AR Human Bodies can be added, updated, and/or removed every frame if there is ");
            s_Builder.Append($"an enabled {nameof(ARHumanBodyManager)} in the scene.");
            s_UnitDescription = s_Builder.ToString();
            s_Builder.Clear();
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
