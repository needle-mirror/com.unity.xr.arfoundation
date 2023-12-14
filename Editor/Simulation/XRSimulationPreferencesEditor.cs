using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomEditor(typeof(XRSimulationPreferences))]
    class XRSimulationPreferencesEditor : Editor
    {
        SerializedObject m_SerializedObject;

        public override VisualElement CreateInspectorGUI()
        {
            var rootElement = new VisualElement();

            m_SerializedObject = new SerializedObject(XRSimulationPreferences.Instance);

            var envPrefabProperty = m_SerializedObject.FindProperty("m_EnvironmentPrefab");
            var envPrefabElement = new PropertyField(envPrefabProperty);
            envPrefabElement.RegisterValueChangeCallback(PropertyChanged);

            StyleLength marginSize = new Length(5);
            rootElement.Add(envPrefabElement);

            var navigationActionsSettingsLabel = new Label("Navigation Input Action References");
            navigationActionsSettingsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            navigationActionsSettingsLabel.style.marginTop = marginSize;
            navigationActionsSettingsLabel.style.marginBottom = marginSize;
            rootElement.Add(navigationActionsSettingsLabel);

            var unlockActionProperty = m_SerializedObject.FindProperty("m_UnlockInputActionReference");
            var unlockActionElement = new PropertyField(unlockActionProperty);
            unlockActionElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(unlockActionElement);

            var moveActionProperty = m_SerializedObject.FindProperty("m_MoveInputActionReference");
            var moveActionElement = new PropertyField(moveActionProperty);
            moveActionElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(moveActionElement);

            var lookActionProperty = m_SerializedObject.FindProperty("m_LookInputActionReference");
            var lookActionElement = new PropertyField(lookActionProperty);
            lookActionElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(lookActionElement);

            var sprintActionProperty = m_SerializedObject.FindProperty("m_SprintInputActionReference");
            var sprintActionElement = new PropertyField(sprintActionProperty);
            sprintActionElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(sprintActionElement);

            var navigationSettingsLabel = new Label("Navigation Settings");
            navigationSettingsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            navigationSettingsLabel.style.marginTop = marginSize;
            navigationSettingsLabel.style.marginBottom = marginSize;
            rootElement.Add(navigationSettingsLabel);

            var lookSpeedProperty = m_SerializedObject.FindProperty("m_LookSpeed");
            var lookSpeedElement = new PropertyField(lookSpeedProperty);
            lookSpeedElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(lookSpeedElement);

            var moveSpeedProperty = m_SerializedObject.FindProperty("m_MoveSpeed");
            var moveSpeedElement = new PropertyField(moveSpeedProperty);
            moveSpeedElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(moveSpeedElement);

            var navSpeedModifierProperty = m_SerializedObject.FindProperty("m_MoveSpeedModifier");
            var navSpeedModifierElement = new PropertyField(navSpeedModifierProperty);
            navSpeedModifierElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(navSpeedModifierElement);

            rootElement.Bind(m_SerializedObject);
            return rootElement;
        }

        void PropertyChanged(SerializedPropertyChangeEvent evt)
        {
            m_SerializedObject.ApplyModifiedProperties();
        }
    }
}
