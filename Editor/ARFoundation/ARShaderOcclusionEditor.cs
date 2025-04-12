using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEditor.XR.ARFoundation
{
    [CustomEditor(typeof(ARShaderOcclusion))]
    class ARShaderOcclusionEditor : Editor
    {
        internal const string softOcclusionPreprocessShaderName = "Occlusion/Soft/DepthPreprocessing";

        SerializedProperty m_ScriptProp;
        SerializedProperty m_OcclusionShaderModeProp;
        SerializedProperty m_SoftOcclusionPreprocessShaderProp;
        SerializedProperty m_AROcclusionSources;
        SerializedProperty m_HandsOcclusionMaterialProp;

        void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");
            m_OcclusionShaderModeProp = serializedObject.FindProperty("m_OcclusionShaderMode");
            m_SoftOcclusionPreprocessShaderProp = serializedObject.FindProperty("m_SoftOcclusionPreprocessShader");
            m_HandsOcclusionMaterialProp = serializedObject.FindProperty("m_HandsOcclusionMaterial");
            m_AROcclusionSources = serializedObject.FindProperty("m_AROcclusionSources");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_AROcclusionSources);

            var occlusionSources = (AROcclusionSources)m_AROcclusionSources.intValue;

            if (EditorGUI.EndChangeCheck())
            {
                if (occlusionSources == AROcclusionSources.None)
                {
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            if ((occlusionSources & AROcclusionSources.HandMesh) == AROcclusionSources.HandMesh)
            {
                EditorGUILayout.PropertyField(m_HandsOcclusionMaterialProp);
            }

            if ((occlusionSources & AROcclusionSources.EnvironmentDepth) != AROcclusionSources.EnvironmentDepth)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_OcclusionShaderModeProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_OcclusionShaderModeProp.intValue == (int) AROcclusionShaderMode.SoftOcclusion)
                {
                    m_SoftOcclusionPreprocessShaderProp.objectReferenceValue =
                        Shader.Find(softOcclusionPreprocessShaderName);

                    EditorGUILayout.PropertyField(m_SoftOcclusionPreprocessShaderProp);
                }
                else
                {
                    m_SoftOcclusionPreprocessShaderProp.objectReferenceValue = null;
                }
            }

            if (m_OcclusionShaderModeProp.intValue == (int) AROcclusionShaderMode.SoftOcclusion)
            {
                EditorGUILayout.PropertyField(m_SoftOcclusionPreprocessShaderProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
