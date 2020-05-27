// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UEditor = UnityEditor.Editor;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Helper class to get CanvasUtility onto Canvas objects.
    /// </summary>

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Canvas))]
    public class MixedRealityCanvasInspector : UEditor
    {
        private static readonly GUIContent MakeMRTKCanvas = new GUIContent("Convert to MRTK Canvas", "Configures the GameObject for MRKT use:\n1. Switches Canvas to world space\n2. Removes world space Camera\n3. Ensures GraphicRaycaster component\n4. Ensures CanvasUtility component");
        private static readonly GUIContent RemoveMRTKCanvas = new GUIContent("Convert to Unity Canvas", "Configures the GameObject for regular use:\n1. Removes CanvasUtility component\n2. Removes NearInteractionTouchableUnityUI component");

        private readonly List<Graphic> graphicsWhichRequireScaleMeshEffect = new List<Graphic>();
        private Type canvasEditorType = null;
        private UEditor internalEditor = null;
        private Canvas canvas = null;
        private bool isRootCanvas = false;

        private void OnEnable()
        {
            canvasEditorType = Type.GetType("UnityEditor.CanvasEditor, UnityEditor");
            if (canvasEditorType != null)
            {
                internalEditor = CreateEditor(targets, canvasEditorType);
                canvas = target as Canvas;
                isRootCanvas = canvas.transform.parent == null || canvas.transform.parent.GetComponentInParent<Canvas>() == null;
            }
        }

        private void OnDisable()
        {
            if (canvasEditorType != null)
            {
                MethodInfo onDisable = canvasEditorType.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
                if (onDisable != null)
                {
                    onDisable.Invoke(internalEditor, null);
                }
                DestroyImmediate(internalEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            if (isRootCanvas && canvas != null)
            {
                ShowMRTKButton();

                List<Graphic> graphics = GetGraphicsWhichRequireScaleMeshEffect(targets);

                if (graphics.Count != 0)
                {
                    EditorGUILayout.HelpBox($"Canvas contains {graphics.Count} {typeof(Graphic).Name}(s) which require a {typeof(ScaleMeshEffect).Name} to work with the {StandardShaderUtility.MrtkStandardShaderName} shader.", MessageType.Warning);
                    if (GUILayout.Button($"Add {typeof(ScaleMeshEffect).Name}(s)"))
                    {
                        foreach (var graphic in graphics)
                        {
                            Undo.AddComponent<ScaleMeshEffect>(graphic.gameObject);
                        }
                    }
                }

                EditorGUILayout.Space();
            }

            if (internalEditor != null)
            {
                internalEditor.OnInspectorGUI();
            }
        }

        private bool ShowMRTKButton()
        {
            if (!canvas.rootCanvas)
            {
                return false;
            }

            bool isMRTKCanvas = canvas.GetComponent<CanvasUtility>() != null;

            if (isMRTKCanvas)
            {
                if (GUILayout.Button(RemoveMRTKCanvas))
                {
                    EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(canvas.GetComponent<NearInteractionTouchableUnityUI>());
                        DestroyImmediate(canvas.GetComponent<CanvasUtility>());
                    };

                    isMRTKCanvas = false;
                }

                if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera != null && !Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("World Space Canvas should not have a camera set to work properly with MRTK. At runtime, it'll get its camera set automatically.", MessageType.Error);
                    if (GUILayout.Button("Clear World Camera"))
                    {
                        Undo.RecordObject(canvas, "Clear World Camera");
                        canvas.worldCamera = null;
                    }
                }

                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    EditorGUILayout.HelpBox($"Canvas must be set to World Space to work properly with MRTK.", MessageType.Warning);
                    if (GUILayout.Button("Update Render Mode to World Space"))
                    {
                        Undo.RecordObject(target, "Change Render Mode");
                        canvas.renderMode = RenderMode.WorldSpace;
                    }
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
                if (GUILayout.Button(MakeMRTKCanvas))
                {
                    if (canvas.GetComponent<GraphicRaycaster>() == null)
                    {
                        Undo.AddComponent<GraphicRaycaster>(canvas.gameObject);
                    }

                    if (canvas.GetComponent<CanvasUtility>() == null)
                    {
                        Undo.AddComponent<CanvasUtility>(canvas.gameObject);
                    }

                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.worldCamera = null;
                    isMRTKCanvas = true;
                }
            }

            return isMRTKCanvas;
        }

        private List<Graphic> GetGraphicsWhichRequireScaleMeshEffect(UnityEngine.Object[] targets)
        {
            graphicsWhichRequireScaleMeshEffect.Clear();

            foreach (UnityEngine.Object target in targets)
            {
                Graphic[] graphics = (target as Canvas).GetComponentsInChildren<Graphic>();

                foreach (Graphic graphic in graphics)
                {
                    if (StandardShaderUtility.IsUsingMrtkStandardShader(graphic.material) &&
                        graphic.GetComponent<ScaleMeshEffect>() == null)
                    {
                        graphicsWhichRequireScaleMeshEffect.Add(graphic);
                    }
                }
            }

            return graphicsWhichRequireScaleMeshEffect;
        }
    }
}