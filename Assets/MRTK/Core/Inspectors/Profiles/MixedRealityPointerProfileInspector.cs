// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityPointerProfile))]
    public class MixedRealityPointerProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerTypeContent = new GUIContent("Controller Type", "The type of Controller this pointer will attach itself to at runtime.");
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove Pointer Option");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New Pointer Option", "Add Pointer Option");

        private const string ProfileTitle = "Pointer Settings";
        private const string ProfileDescription = "Pointers attach themselves onto controllers as they are initialized.";

        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private static bool showPointerOptionProperties = true;
        private SerializedProperty pointerOptions;
        private SerializedProperty debugDrawPointingRays;
        private SerializedProperty debugDrawPointingRayColors;
        private SerializedProperty gazeCursorPrefab;
        private SerializedProperty gazeProviderType;
        private SerializedProperty useHeadGazeOverride;
        private SerializedProperty isEyeTrackingEnabled;
        private SerializedProperty showCursorWithEyeGaze;
        private SerializedProperty pointerMediator;
        private SerializedProperty primaryPointerSelector;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointingExtent = serializedObject.FindProperty("pointingExtent");
            pointingRaycastLayerMasks = serializedObject.FindProperty("pointingRaycastLayerMasks");
            pointerOptions = serializedObject.FindProperty("pointerOptions");
            debugDrawPointingRays = serializedObject.FindProperty("debugDrawPointingRays");
            debugDrawPointingRayColors = serializedObject.FindProperty("debugDrawPointingRayColors");
            gazeCursorPrefab = serializedObject.FindProperty("gazeCursorPrefab");
            gazeProviderType = serializedObject.FindProperty("gazeProviderType");
            useHeadGazeOverride = serializedObject.FindProperty("useHeadGazeOverride");
            isEyeTrackingEnabled = serializedObject.FindProperty("isEyeTrackingEnabled");
            showCursorWithEyeGaze = serializedObject.FindProperty("showCursorWithEyeGaze");
            pointerMediator = serializedObject.FindProperty("pointerMediator");
            //primaryPointerSelector = serializyedObject.FindProperty("primaryPointerSelector");
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input))
            {
                return;
            }

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Gaze Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(gazeCursorPrefab, new GUIContent("Gaze Cursor Prefab"));
                    EditorGUILayout.PropertyField(gazeProviderType);
                    EditorGUILayout.PropertyField(useHeadGazeOverride);
                    EditorGUILayout.PropertyField(isEyeTrackingEnabled);
                    EditorGUILayout.Space();

                    if (InspectorUIUtility.RenderIndentedButton("Customize Gaze Provider Settings"))
                    {
                        Selection.activeObject = CameraCache.Main.gameObject;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Pointer Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(pointingExtent);
                    EditorGUILayout.PropertyField(pointingRaycastLayerMasks, new GUIContent("Default Raycast LayerMasks"), true);
                    EditorGUILayout.PropertyField(pointerMediator);
                    //EditorGUILayout.PropertyField(primaryPointerSelector);

                    EditorGUILayout.Space();
                    showPointerOptionProperties = EditorGUILayout.Foldout(showPointerOptionProperties, "Pointer Options", true);
                    if (showPointerOptionProperties)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            RenderPointerList(pointerOptions);
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debug Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(debugDrawPointingRays);
                    EditorGUILayout.PropertyField(debugDrawPointingRayColors, true);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile;
        }

        private void RenderPointerList(SerializedProperty list)
        {
            if (InspectorUIUtility.RenderIndentedButton(AddButtonContent, EditorStyles.miniButton))
            {
                pointerOptions.arraySize += 1;

                var newPointerOption = list.GetArrayElementAtIndex(list.arraySize - 1);
                var controllerType = newPointerOption.FindPropertyRelative("controllerType");
                var handedness = newPointerOption.FindPropertyRelative("handedness");
                var prefab = newPointerOption.FindPropertyRelative("pointerPrefab");
                var raycastLayerMask = newPointerOption.FindPropertyRelative("pointingRaycastLayerMasks");

                // Reset new entry
                controllerType.intValue = 0;
                handedness.intValue = 0;
                prefab.objectReferenceValue = null;
                //raycastLayerMask.arra = ((MixedRealityPointerProfile)target).PointingRaycastLayerMasks;
            }

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a new Pointer Option entry.", MessageType.Warning);
                return;
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    Color prevColor = GUI.color;

                    var pointerOption = list.GetArrayElementAtIndex(i);
                    var controllerType = pointerOption.FindPropertyRelative("controllerType");
                    var handedness = pointerOption.FindPropertyRelative("handedness");
                    var prefab = pointerOption.FindPropertyRelative("pointerPrefab");

                    // Display an error if the prefab doesn't have a IMixedRealityPointer Component
                    if (prefab.objectReferenceValue != null && ((GameObject)prefab.objectReferenceValue).GetComponent<IMixedRealityPointer>() == null)
                    {
                        InspectorUIUtility.DrawError($"The prefab associated with this pointer option needs an {typeof(IMixedRealityPointer).Name} component");

                        GUI.color = MixedRealityInspectorUtility.ErrorColor;
                    }
                    // if the prefab does have the component, provide a field to display and edit it's PrioritzedLayerMaskOverrides if it specifies a way to get it
                    else
                    {
                        var pointer = ((GameObject)prefab.objectReferenceValue).GetComponent<IMixedRealityPointer>();
                        if (pointer.PrioritizedLayerMasksOverride != null)
                        {
                            //TODO
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(prefab);
                        if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                        {
                            list.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }

                    EditorGUILayout.PropertyField(controllerType, ControllerTypeContent);
                    EditorGUILayout.PropertyField(handedness);


                    GUI.color = prevColor;
                }
                EditorGUILayout.Space();
            }
        }
    }
}