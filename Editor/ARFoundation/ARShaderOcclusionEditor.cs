using System;
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

        void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");
            m_OcclusionShaderModeProp = serializedObject.FindProperty("m_OcclusionShaderMode");
            m_SoftOcclusionPreprocessShaderProp = serializedObject.FindProperty("m_SoftOcclusionPreprocessShader");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_OcclusionShaderModeProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_OcclusionShaderModeProp.intValue == 2) // AROcclusionShaderMode.SoftOcclusion
                {
                    m_SoftOcclusionPreprocessShaderProp.objectReferenceValue =
                        Shader.Find(softOcclusionPreprocessShaderName);
                }
                else
                {
                    m_SoftOcclusionPreprocessShaderProp.objectReferenceValue = null;
                }
            }

            if (m_OcclusionShaderModeProp.intValue == 2) // AROcclusionShaderMode.SoftOcclusion
                EditorGUILayout.PropertyField(m_SoftOcclusionPreprocessShaderProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
