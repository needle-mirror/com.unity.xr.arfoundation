using UnityEngine;

namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    /// <summary>
    /// A property attribute to hide a value in the Inspector, similar to <see cref="HideInInspector"/>,
    /// but the property will be shown in the debug mode of the Inspector.
    /// </summary>
    class ShowInDebugInspectorOnlyAttribute : PropertyAttribute
    {
    }
}
