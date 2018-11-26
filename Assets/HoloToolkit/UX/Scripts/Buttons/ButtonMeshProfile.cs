// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    public class ButtonMeshProfile : ButtonProfile
    {
        [Tooltip("Name of the shader color property that will be modified (default '_Color')")]
        public string ColorPropertyName = "_Color";
       
        [Tooltip("Name of the shader float property that will be modified.")]
        public string ValuePropertyName = string.Empty;

        [Tooltip("If true, button properties are lerped instead of instantaneous.")]
        public bool SmoothStateChanges = false;

        [Tooltip("Whether to hold pressed events for a short period.")]
        public bool StickyPressedEvents = false;
        
        [Tooltip("How long to hold sticky pressed events.")]
        public float StickyPressedTime = 0.15f;

        [Range(0.01f, 1f)]
        [Tooltip("How quickly to animate scale, offset, color and value properties")]
        public float AnimationSpeed = 1f;

        [HideInMRTKInspector]
        public CompoundButtonMesh.MeshButtonDatum[] ButtonStates = new CompoundButtonMesh.MeshButtonDatum[]{ new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)0), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)1),
            new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)2), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)3),
            new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)4), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)5) };

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ButtonMeshProfile))]
        public class CustomEditor : ProfileInspector
        {
            /// <summary>
            /// Draws a custom editor for mesh button datum so they're human-editable
            /// </summary>
            protected override void DrawCustomFooter() {

                ButtonMeshProfile meshProfile = (ButtonMeshProfile)target;
                CompoundButtonMesh meshButton = null;
                if (targetComponent is CompoundButtonMesh)
                    meshButton = targetComponent as CompoundButtonMesh;

                // Validate our button states - ensure there's one for each button state enum value
                ButtonStateEnum[] buttonStates = (ButtonStateEnum[])System.Enum.GetValues(typeof(ButtonStateEnum));
                List<CompoundButtonMesh.MeshButtonDatum> missingStates = new List<CompoundButtonMesh.MeshButtonDatum>();
                foreach (ButtonStateEnum buttonState in buttonStates) {
                    bool foundState = false;
                    foreach (CompoundButtonMesh.MeshButtonDatum datum in meshProfile.ButtonStates) {
                        if (datum.ActiveState == buttonState) {
                            foundState = true;
                            break;
                        }
                    }

                    if (!foundState) {
                        CompoundButtonMesh.MeshButtonDatum missingState = new CompoundButtonMesh.MeshButtonDatum(buttonState);
                        missingState.Name = buttonState.ToString();
                        missingStates.Add(missingState);
                    }
                }

                // If any were missing, add them to our button states
                // They may be out of order but we don't care
                if (missingStates.Count > 0) {
                    missingStates.AddRange(meshProfile.ButtonStates);
                    meshProfile.ButtonStates = missingStates.ToArray();
                }

                foreach (CompoundButtonMesh.MeshButtonDatum datum in meshProfile.ButtonStates) {
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.LabelField(datum.ActiveState.ToString(), UnityEditor.EditorStyles.boldLabel);
                    UnityEditor.EditorGUI.indentLevel++;
                    if (meshButton != null && meshButton.TargetTransform == null) {
                        UnityEditor.EditorGUILayout.LabelField("(No target transform specified for scale / offset)", UnityEditor.EditorStyles.miniLabel);
                    } else {
                        datum.Offset = UnityEditor.EditorGUILayout.Vector3Field("Offset", datum.Offset);
                        datum.Scale = UnityEditor.EditorGUILayout.Vector3Field("Scale", datum.Scale);

                        if (datum.Scale == Vector3.zero) {
                            GUI.color = warningColor;
                            if (GUILayout.Button("Warning: Button state scale is zero. Click here to fix.", UnityEditor.EditorStyles.miniButton)) {
                                datum.Scale = Vector3.one;
                            }
                        }
                    }

                    GUI.color = defaultColor;
                    if (meshButton != null && meshButton.Renderer == null) {
                        UnityEditor.EditorGUILayout.LabelField("(No target renderer specified for color / value material properties)", UnityEditor.EditorStyles.miniLabel);
                    } else {
                        if (!string.IsNullOrEmpty(meshProfile.ColorPropertyName)) {
                            datum.StateColor = UnityEditor.EditorGUILayout.ColorField(meshProfile.ColorPropertyName + " value", datum.StateColor);
                        }
                        if (!string.IsNullOrEmpty(meshProfile.ValuePropertyName)) {
                            datum.StateValue = UnityEditor.EditorGUILayout.FloatField(meshProfile.ValuePropertyName + " value", datum.StateValue);
                        }

                    }
                    UnityEditor.EditorGUI.indentLevel--;
                }
            }
        }
#endif
    }
}