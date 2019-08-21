// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

                if (canvas.GetComponent<GraphicRaycaster>() == null)
                {
                    UnityEditor.EditorGUILayout.HelpBox($"For Mixed Reality Toolkit input support add a {typeof(GraphicRaycaster).Name} component", UnityEditor.MessageType.Info);
                    if (GUILayout.Button($"Add {typeof(GraphicRaycaster).Name}"))
                    {
                        GraphicRaycaster graphicRaycaster = UnityEditor.Undo.AddComponent<GraphicRaycaster>(canvas.gameObject);

                        Component[] components = canvas.GetComponents<Component>();
                        int canvasUtilIndex = 0;
                        int raycasterIndex = 0;

                        for (int i = 0; i < components.Length; i++)
                        {
                            if (components[i] is CanvasUtility)
                                canvasUtilIndex = i;

                            if (components[i] == graphicRaycaster)
                                raycasterIndex = i;
                        }

                        if (raycasterIndex > canvasUtilIndex)
                        {
                            for (int i = 0; i < raycasterIndex - canvasUtilIndex; i++)
                            {
                                UnityEditorInternal.ComponentUtility.MoveComponentUp(graphicRaycaster);
                            }
                        }
                    }
                }
                else if (CanSupportMrtkInput(canvas) && (canvas.GetComponentInChildren<NearInteractionTouchableUnityUI>() == null))
                {
                    UnityEditor.EditorGUILayout.HelpBox($"Canvas does not contain any {typeof(NearInteractionTouchableUnityUI).Name} components for supporting near interaction.", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button($"Add {typeof(NearInteractionTouchableUnityUI).Name}"))
                    {
                        UnityEditor.Undo.AddComponent<NearInteractionTouchableUnityUI>(canvas.gameObject);
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

        public static bool CanSupportMrtkInput(Canvas canvas)
        {
            return (canvas.isRootCanvas && (canvas.renderMode == RenderMode.WorldSpace));
        }
    }
}
