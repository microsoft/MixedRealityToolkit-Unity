// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Editor class used to edit UI Canvases.
    /// </summary>

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Canvas))]
    public class MixedRealityCanvasInspector : UnityEditor.Editor
    {
        private static readonly GUIContent makeMRTKCanvas = new GUIContent("Convert to MRTK Canvas", "Configures the GameObject for MRKT use:\n1. Switches Canvas to world space\n2. Removes world space Camera\n3. Ensures GraphicRaycaster component\n4. Ensures CanvasUtility component");
        private static readonly GUIContent removeMRTKCanvas = new GUIContent("Convert to Unity Canvas", "Configures the GameObject for regular use:\n1. Removes CanvasUtility component\n2. Removes NearInteractionTouchableUnityUI component");

        private MethodInfo sortingLayerField;
        private MethodInfo getDisplayNames;
        private MethodInfo getDisplayIndices;

        SerializedProperty m_RenderMode;
        SerializedProperty m_Camera;
        SerializedProperty m_PixelPerfect;
        SerializedProperty m_PixelPerfectOverride;
        SerializedProperty m_PlaneDistance;
        SerializedProperty m_SortingLayerID;
        SerializedProperty m_SortingOrder;
        SerializedProperty m_TargetDisplay;
        SerializedProperty m_OverrideSorting;
        SerializedProperty m_ShaderChannels;

        AnimBool m_OverlayMode;
        AnimBool m_CameraMode;
        AnimBool m_WorldMode;

        AnimBool m_SortingOverride;

        private static class Styles
        {
            public const string s_RootAndNestedMessage = "Cannot multi-edit root Canvas together with nested Canvas.";
            public static readonly GUIContent eventCamera = EditorGUIUtility.TrTextContent("Event Camera", "The Camera which the events are triggered through. This is used to determine clicking and hover positions if the Canvas is in World Space render mode.");
            public static readonly GUIContent renderCamera = EditorGUIUtility.TrTextContent("Render Camera", "The Camera which will render the canvas. This is also the camera used to send events.");
            public static readonly GUIContent sortingOrder = EditorGUIUtility.TrTextContent("Sort Order", "The order in which Screen Space - Overlay canvas will render");
            public static readonly GUIContent m_SortingLayerStyle = EditorGUIUtility.TrTextContent("Sorting Layer", "Name of the Renderer's sorting layer");
            public static readonly GUIContent targetDisplay = EditorGUIUtility.TrTextContent("Target Display", "Display on which to render the canvas when in overlay mode");
            public static readonly GUIContent m_SortingOrderStyle = EditorGUIUtility.TrTextContent("Order in Layer", "Renderer's order within a sorting layer");
            public static readonly GUIContent m_ShaderChannel = EditorGUIUtility.TrTextContent("Additional Shader Channels");
        }

        private bool m_AllNested = false;
        private bool m_AllRoot = false;

        private bool m_AllOverlay = false;
        private bool m_NoneOverlay = false;

        private string[] shaderChannelOptions = { "TexCoord1", "TexCoord2", "TexCoord3", "Normal", "Tangent" };


        enum PixelPerfect
        {
            Inherit,
            On,
            Off
        }

        private PixelPerfect pixelPerfect = PixelPerfect.Inherit;

        void OnEnable()
        {
            sortingLayerField = typeof(EditorGUILayout).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, CallingConventions.Standard, new System.Type[] { typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle) }, null);
            System.Type canvasEditorType = typeof(TransformUtils).Assembly.GetType("UnityEditor.DisplayUtility");
            if (canvasEditorType != null)
            {
                getDisplayNames = canvasEditorType.GetMethod("GetDisplayNames", BindingFlags.Static | BindingFlags.Public);
                getDisplayIndices = canvasEditorType.GetMethod("GetDisplayIndices", BindingFlags.Static | BindingFlags.Public);
            }

            m_RenderMode = serializedObject.FindProperty("m_RenderMode");
            m_Camera = serializedObject.FindProperty("m_Camera");
            m_PixelPerfect = serializedObject.FindProperty("m_PixelPerfect");
            m_PlaneDistance = serializedObject.FindProperty("m_PlaneDistance");

            m_SortingLayerID = serializedObject.FindProperty("m_SortingLayerID");
            m_SortingOrder = serializedObject.FindProperty("m_SortingOrder");
            m_TargetDisplay = serializedObject.FindProperty("m_TargetDisplay");
            m_OverrideSorting = serializedObject.FindProperty("m_OverrideSorting");
            m_PixelPerfectOverride = serializedObject.FindProperty("m_OverridePixelPerfect");
            m_ShaderChannels = serializedObject.FindProperty("m_AdditionalShaderChannelsFlag");

            m_OverlayMode = new AnimBool(m_RenderMode.intValue == 0);
            m_OverlayMode.valueChanged.AddListener(Repaint);

            m_CameraMode = new AnimBool(m_RenderMode.intValue == 1);
            m_CameraMode.valueChanged.AddListener(Repaint);

            m_WorldMode = new AnimBool(m_RenderMode.intValue == 2);
            m_WorldMode.valueChanged.AddListener(Repaint);

            m_SortingOverride = new AnimBool(m_OverrideSorting.boolValue);
            m_SortingOverride.valueChanged.AddListener(Repaint);

            if (m_PixelPerfectOverride.boolValue)
                pixelPerfect = m_PixelPerfect.boolValue ? PixelPerfect.On : PixelPerfect.Off;
            else
                pixelPerfect = PixelPerfect.Inherit;

            m_AllNested = true;
            m_AllRoot = true;
            m_AllOverlay = true;
            m_NoneOverlay = true;

            for (int i = 0; i < targets.Length; i++)
            {
                Canvas canvas = targets[i] as Canvas;

                if (canvas.transform.parent == null || canvas.transform.parent.GetComponentInParent<Canvas>() == null)
                    m_AllNested = false;
                else
                    m_AllRoot = false;

                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    m_NoneOverlay = false;
                else
                    m_AllOverlay = false;
            }
        }

        void OnDisable()
        {
            m_OverlayMode.valueChanged.RemoveListener(Repaint);
            m_CameraMode.valueChanged.RemoveListener(Repaint);
            m_WorldMode.valueChanged.RemoveListener(Repaint);
            m_SortingOverride.valueChanged.RemoveListener(Repaint);
        }

        private void AllRootCanvases()
        {
            bool isMrtkCanvas = ShowMRTKButton();

            var graphics = GetGraphicsWhichRequireScaleMeshEffect(targets);

            if (graphics.Count() != 0)
            {
                EditorGUILayout.HelpBox($"Canvas contains {graphics.Count()} {typeof(Graphic).Name}(s) which require a {typeof(ScaleMeshEffect).Name} to work with the {StandardShaderUtility.MrtkStandardShaderName} shader.", UnityEditor.MessageType.Warning);
                if (GUILayout.Button($"Add {typeof(ScaleMeshEffect).Name}(s)"))
                {
                    foreach (var graphic in graphics)
                    {
                        Undo.AddComponent<ScaleMeshEffect>(graphic.gameObject);
                    }
                }
            }

            EditorGUILayout.Space();
            if (XRSettingsUtilities.LegacyXREnabled &&
                (m_RenderMode.enumValueIndex == (int)RenderMode.ScreenSpaceOverlay))
            {
                EditorGUILayout.HelpBox("Using a render mode of ScreenSpaceOverlay while VR is enabled will cause the Canvas to continue to incur a rendering cost, even though the Canvas will not be visible in VR.", MessageType.Warning);
            }

            if (!isMrtkCanvas)
            {
                EditorGUILayout.PropertyField(m_RenderMode);
            }

            m_OverlayMode.target = m_RenderMode.intValue == 0;
            m_CameraMode.target = m_RenderMode.intValue == 1;
            m_WorldMode.target = m_RenderMode.intValue == 2;

            EditorGUI.indentLevel++;
            if (EditorGUILayout.BeginFadeGroup(m_OverlayMode.faded))
            {
                EditorGUILayout.PropertyField(m_PixelPerfect);
                EditorGUILayout.PropertyField(m_SortingOrder, Styles.sortingOrder);
                GUIContent[] displayNames = (GUIContent[]) getDisplayNames.Invoke(null, System.Array.Empty<object>());
                EditorGUILayout.IntPopup(m_TargetDisplay, displayNames, (int[])getDisplayIndices.Invoke(null, new object[] { }), Styles.targetDisplay);
            }
            EditorGUILayout.EndFadeGroup();

            if (EditorGUILayout.BeginFadeGroup(m_CameraMode.faded))
            {
                EditorGUILayout.PropertyField(m_PixelPerfect);
                EditorGUILayout.PropertyField(m_Camera, Styles.renderCamera);

                if (m_Camera.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("A Screen Space Canvas with no specified camera acts like an Overlay Canvas.",
                        MessageType.Warning);

                if (m_Camera.objectReferenceValue != null)
                    EditorGUILayout.PropertyField(m_PlaneDistance);

                EditorGUILayout.Space();

                if (m_Camera.objectReferenceValue != null)
                    sortingLayerField.Invoke(null, new object[] { Styles.m_SortingLayerStyle, m_SortingLayerID, EditorStyles.popup, EditorStyles.label });
                EditorGUILayout.PropertyField(m_SortingOrder, Styles.m_SortingOrderStyle);
            }
            EditorGUILayout.EndFadeGroup();

            if (EditorGUILayout.BeginFadeGroup(m_WorldMode.faded))
            {
                if (!isMrtkCanvas)
                {
                    EditorGUILayout.PropertyField(m_Camera, Styles.eventCamera);

                    if (m_Camera.objectReferenceValue == null)
                        EditorGUILayout.HelpBox("A World Space Canvas with no specified Event Camera may not register UI events correctly.",
                            MessageType.Warning);

                    EditorGUILayout.Space();
                }
                sortingLayerField.Invoke(null, new object[] { Styles.m_SortingLayerStyle, m_SortingLayerID, EditorStyles.popup });
                EditorGUILayout.PropertyField(m_SortingOrder, Styles.m_SortingOrderStyle);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel--;
        }

        private void AllNestedCanvases()
        {
            EditorGUI.BeginChangeCheck();
            pixelPerfect = (PixelPerfect)EditorGUILayout.EnumPopup("Pixel Perfect", pixelPerfect);

            if (EditorGUI.EndChangeCheck())
            {
                if (pixelPerfect == PixelPerfect.Inherit)
                {
                    m_PixelPerfectOverride.boolValue = false;
                }
                else if (pixelPerfect == PixelPerfect.Off)
                {
                    m_PixelPerfectOverride.boolValue = true;
                    m_PixelPerfect.boolValue = false;
                }
                else
                {
                    m_PixelPerfectOverride.boolValue = true;
                    m_PixelPerfect.boolValue = true;
                }
            }

            EditorGUILayout.PropertyField(m_OverrideSorting);
            m_SortingOverride.target = m_OverrideSorting.boolValue;

            if (EditorGUILayout.BeginFadeGroup(m_SortingOverride.faded))
            {
                GUIContent sortingOrderStyle = null;
                if (m_AllOverlay)
                {
                    sortingOrderStyle = Styles.sortingOrder;
                }
                else if (m_NoneOverlay)
                {
                    sortingOrderStyle = Styles.m_SortingOrderStyle;
                    sortingLayerField.Invoke(null, new object[] { Styles.m_SortingLayerStyle, m_SortingLayerID, EditorStyles.popup });
                }
                if (sortingOrderStyle != null)
                {
                    EditorGUILayout.PropertyField(m_SortingOrder, sortingOrderStyle);
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        private bool ShowMRTKButton()
        {
            Canvas canvas = (Canvas)target;

            if (!canvas.rootCanvas)
            {
                return false;
            }

            bool isMRTKCanvas = canvas.GetComponent<CanvasUtility>() != null;

            if (isMRTKCanvas)
            {
                if (GUILayout.Button(removeMRTKCanvas))
                {
                    EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(canvas.GetComponent<NearInteractionTouchableUnityUI>());
                        DestroyImmediate(canvas.GetComponent<CanvasUtility>());
                    };

                    isMRTKCanvas = false;
                }

                if (canvas.GetComponentInChildren<NearInteractionTouchableUnityUI>() == null)
                {
                    EditorGUILayout.HelpBox($"Canvas does not contain any {typeof(NearInteractionTouchableUnityUI).Name} components for supporting near interaction.", MessageType.Warning);
                    if (GUILayout.Button($"Add {typeof(NearInteractionTouchableUnityUI).Name}"))
                    {
                        Undo.AddComponent<NearInteractionTouchableUnityUI>(canvas.gameObject);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(makeMRTKCanvas))
                {
                    if (canvas.GetComponent<GraphicRaycaster>() == null)
                        Undo.AddComponent<GraphicRaycaster>(canvas.gameObject);
                    if (canvas.GetComponent<CanvasUtility>() == null)
                        Undo.AddComponent<CanvasUtility>(canvas.gameObject);
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.worldCamera = null;
                    isMRTKCanvas = true;
                }
            }

            return isMRTKCanvas;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (m_AllRoot || m_AllNested)
            {
                if (m_AllRoot)
                {
                    AllRootCanvases();
                }
                else if (m_AllNested)
                {
                    AllNestedCanvases();
                }

                int newShaderChannelValue = 0;
                EditorGUI.BeginChangeCheck();
                newShaderChannelValue = EditorGUILayout.MaskField(Styles.m_ShaderChannel, m_ShaderChannels.intValue, shaderChannelOptions);


                if (EditorGUI.EndChangeCheck())
                    m_ShaderChannels.intValue = newShaderChannelValue;

                if (m_RenderMode.intValue == 0) // Overlay canvas
                {
                    if (((newShaderChannelValue & (int)AdditionalCanvasShaderChannels.Normal) | (newShaderChannelValue & (int)AdditionalCanvasShaderChannels.Tangent)) != 0)
                        EditorGUILayout.HelpBox("Shader channels Normal and Tangent are most often used with lighting, which an Overlay canvas does not support. Its likely these channels are not needed.", MessageType.Warning);
                }
            }
            else
            {
                GUILayout.Label(Styles.s_RootAndNestedMessage, EditorStyles.helpBox);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static IEnumerable<Graphic> GetGraphicsWhichRequireScaleMeshEffect(Object[] targets)
        {
            var output = new List<Graphic>();

            foreach (var target in targets)
            {
                Graphic[] graphics = (target as Canvas).GetComponentsInChildren<Graphic>();

                foreach (var graphic in graphics)
                {
                    if (StandardShaderUtility.IsUsingMrtkStandardShader(graphic.material) &&
                        graphic.GetComponent<ScaleMeshEffect>() == null)
                    {
                        output.Add(graphic);
                    }
                }
            }

            return output;
        }
    }
}