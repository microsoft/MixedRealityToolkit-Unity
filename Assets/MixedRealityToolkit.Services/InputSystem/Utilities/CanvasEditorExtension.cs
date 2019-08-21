// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// Helper class to get CanvasUtility onto Canvas objects.
    /// </summary>
    [CustomEditor(typeof(Canvas))]
    public class CanvasEditorExtension : Editor
    {
        SerializedProperty m_Camera;
        SerializedProperty m_SortingLayerID;
        SerializedProperty m_SortingOrder;
        SerializedProperty m_ShaderChannels;

        private MethodInfo sortingLayerField;
        private Editor editor;

        private static class Styles
        {
            public static GUIContent eventCamera = EditorGUIUtility.TrTextContent("Event Camera", "The Camera which the events are triggered through. This is used to determine clicking and hover positions if the Canvas is in World Space render mode.");
            public static GUIContent m_SortingLayerStyle = EditorGUIUtility.TrTextContent("Sorting Layer", "Name of the Renderer's sorting layer");
            public static GUIContent m_SortingOrderStyle = EditorGUIUtility.TrTextContent("Order in Layer", "Renderer's order within a sorting layer");
            public static GUIContent m_ShaderChannel = EditorGUIUtility.TrTextContent("Additional Shader Channels");
        }

        private string[] shaderChannelOptions = { "TexCoord1", "TexCoord2", "TexCoord3", "Normal", "Tangent" };

        void OnEnable()
        {
            m_Camera = serializedObject.FindProperty("m_Camera");
            m_SortingLayerID = serializedObject.FindProperty("m_SortingLayerID");
            m_SortingOrder = serializedObject.FindProperty("m_SortingOrder");
            m_ShaderChannels = serializedObject.FindProperty("m_AdditionalShaderChannelsFlag");

            sortingLayerField = typeof(EditorGUILayout).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, CallingConventions.Standard, new System.Type[] { typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle) }, null);
            System.Type canvasEditorType = typeof(TransformUtils).Assembly.GetType("UnityEditor.CanvasEditor");
            if (canvasEditorType != null)
            {
                CreateCachedEditor(target, canvasEditorType, ref editor);
            }
        }

        public override void OnInspectorGUI()
        {
            Canvas canvas = (Canvas)target;

            if (CanvasUtility.CanSupportMrtkInput(canvas))
            {
                bool hasCanvasUtility = canvas.GetComponent<CanvasUtility>() != null;

                if (!hasCanvasUtility)
                {
                    EditorGUILayout.HelpBox("Canvases must have the CanvasUtility on them to support the Mixed Reality Toolkit", MessageType.Error);
                    if (GUILayout.Button("Add CanvasUtility"))
                    {
                        Undo.AddComponent<CanvasUtility>(canvas.gameObject);
                    }

                    DrawInspectorGUI();
                }
                else
                {
                    DrawMrtkInspectorGUI(hasCanvasUtility);
                }
            }
            else
            {
                DrawInspectorGUI();
            }
        }

        private void DrawInspectorGUI()
        {
            if (editor != null)
            {
                editor.OnInspectorGUI();
            }
            else
            {
                base.OnInspectorGUI();
            }
        }

        private void DrawMrtkInspectorGUI(bool hasCanvasUtility)
        {
            serializedObject.Update();

            if (!hasCanvasUtility || m_Camera.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_Camera, Styles.eventCamera);
            }

            if (sortingLayerField != null)
            {
                sortingLayerField.Invoke(null, new object[] { Styles.m_SortingLayerStyle, m_SortingLayerID, EditorStyles.popup });
            }
            else
            {
                EditorGUILayout.PropertyField(m_SortingLayerID);
            }

            EditorGUILayout.PropertyField(m_SortingOrder, Styles.m_SortingOrderStyle);

            int newShaderChannelValue = 0;

            EditorGUI.BeginChangeCheck();

            newShaderChannelValue = EditorGUILayout.MaskField(Styles.m_ShaderChannel, m_ShaderChannels.intValue, shaderChannelOptions);

            if (EditorGUI.EndChangeCheck())
            {
                m_ShaderChannels.intValue = newShaderChannelValue;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif