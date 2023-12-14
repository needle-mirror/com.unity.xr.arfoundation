using UnityEngine;
using UnityEngine.XR.ARFoundation.InternalUtils;

namespace UnityEditor.XR.ARFoundation.InternalUtils
{
    /// <summary>
    /// The implementation of <see cref="ShowInDebugInspectorOnlyAttribute"/>, which adds nothing in OnGUI
    /// and a height of 0 for the inspector. These get ignored when the Inspector is set to debug mode, revealing the property.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowInDebugInspectorOnlyAttribute))]
    class ShowInDebugInspectorOnlyAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {}
    }
}
