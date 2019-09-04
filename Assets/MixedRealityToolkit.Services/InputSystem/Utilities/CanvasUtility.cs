// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// Helper class for setting up canvases for use in the MRTK.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class CanvasUtility : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(CanvasUtility))]
        public class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                Canvas canvas = ((CanvasUtility)target).GetComponent<Canvas>();

                if (canvas == null)
                {
                    Debug.LogError("Requires Canvas");
                    base.OnInspectorGUI();
                    return;
                }

                if (IsPartOfScene(canvas.gameObject) && CanSupportMrtkInput(canvas) && (canvas.worldCamera != null) && !Application.isPlaying)
                {
                    UnityEditor.EditorGUILayout.HelpBox("World Space Canvas should have no camera set to work properly with Mixed Reality Toolkit. At runtime, they'll get their camera set automatically.", UnityEditor.MessageType.Error);
                    if (GUILayout.Button("Clear World Camera"))
                    {
                        UnityEditor.Undo.RecordObject(canvas, "Clear World Camera");
                        canvas.worldCamera = null;
                    }
                }

                if (CanSupportMrtkInput(canvas) && (canvas.GetComponentInChildren<NearInteractionTouchableUnityUI>() == null))
                {
                    UnityEditor.EditorGUILayout.HelpBox($"Canvas does not contain any {typeof(NearInteractionTouchableUnityUI).Name} components for supporting near interaction.", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button("Add NearInteractionTouchable"))
                    {
                        UnityEditor.Undo.AddComponent<NearInteractionTouchableUnityUI>(canvas.gameObject);
                    }
                }

                var graphics = GetGraphicsWhichRequireScaleMeshEffect(canvas.gameObject);

                if (graphics.Count != 0)
                {
                    UnityEditor.EditorGUILayout.HelpBox($"Canvas contains {graphics.Count} {typeof(Graphic).Name}(s) which require a {typeof(ScaleMeshEffect).Name} to work with the {StandardShaderUtility.MrtkStandardShaderName} shader.", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button($"Add {typeof(ScaleMeshEffect).Name}(s)"))
                    {
                        foreach (var graphic in graphics)
                        {
                            UnityEditor.Undo.AddComponent<ScaleMeshEffect>(graphic.gameObject);
                        }
                    }
                }

                base.OnInspectorGUI();
            }
        }
#endif

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        private void Start()
        {
            Canvas canvas = GetComponent<Canvas>();
            Debug.Assert(canvas != null);

            if (CanSupportMrtkInput(canvas))
            {
                if (canvas.worldCamera == null)
                {
                    Debug.Assert(InputSystem?.FocusProvider?.UIRaycastCamera != null, this);
                    canvas.worldCamera = InputSystem?.FocusProvider?.UIRaycastCamera;

                    if (EventSystem.current == null)
                    {
                        Debug.LogError("No EventSystem detected. UI events will not be propagated to Unity UI.");
                    }
                }
                else
                {
                    Debug.LogError("World Space Canvas should have no camera set to work properly with Mixed Reality Toolkit. At runtime, they'll get their camera set automatically.");
                }
            }
        }

        private static bool IsPartOfScene(GameObject gameObject)
        {
            return (gameObject.scene.name != null);
        }

        private static bool CanSupportMrtkInput(Canvas canvas)
        {
            return (canvas.isRootCanvas && (canvas.renderMode == RenderMode.WorldSpace));
        }

        private static List<Graphic> GetGraphicsWhichRequireScaleMeshEffect(GameObject gameObject)
        {
            var output = new List<Graphic>();
            Graphic[] graphics = gameObject.GetComponentsInChildren<Graphic>();

            foreach (var graphic in graphics)
            {
                if (StandardShaderUtility.IsUsingMrtkStandardShader(graphic.material) && 
                    graphic.GetComponent<ScaleMeshEffect>() == null)
                {
                    output.Add(graphic);
                }
            }

            return output;
        }
    }
}
