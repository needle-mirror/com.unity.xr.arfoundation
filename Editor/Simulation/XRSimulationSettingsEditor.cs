using System;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomEditor(typeof(XRSimulationSettings))]
    class XRSimulationSettingsEditor : Editor
    {
        Editor m_Editor;

        void OnEnable()
        {
            CreateCachedEditor(XRSimulationRuntimeSettings.Instance, typeof(XRSimulationRuntimeSettingsEditor), ref m_Editor);
        }

        public override void OnInspectorGUI()
        {
            m_Editor.OnInspectorGUI();
        }
    }
}
