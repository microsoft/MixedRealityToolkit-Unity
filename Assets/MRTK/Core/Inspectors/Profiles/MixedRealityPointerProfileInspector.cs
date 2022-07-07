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
        private static readonly GUIContent GazeCursorPrefabContent = new GUIContent("Gaze Cursor Prefab");
        private static readonly GUIContent UseEyeTrackingDataContent = new GUIContent("Use Eye Tracking Data");
        private static readonly GUIContent RaycastLayerMaskContent = new GUIContent("Default Raycast LayerMasks");
        private static readonly GUIContent PointerRaycastLayerMaskContent = new GUIContent("Pointer Raycast LayerMasks");

#if UNITY_2019_3_OR_NEWER
        private const string EnableGazeCapabilityContent = "To use eye tracking with UWP, the GazeInput capability needs to be set in the manifest." +
            "\nPlease click the button below to set it in the Unity UWP Player Settings and check the Visual Studio appxmanifest capabilities to ensure it's enabled.";
#endif // UNITY_2019_3_OR_NEWER

        private const string ProfileTitle = "Pointer Settings";
        private const string ProfileDescription = "Pointers attach themselves onto controllers as they are initialized.";

        private SerializedProperty pointingExtent;
        private SerializedProperty defaultRaycastLayerMasks;
        private static bool showPointerOptionProperties = true;
        private SerializedProperty pointerOptions;

        private SerializedProperty debugDrawPointingRays;
        private SerializedProperty debugDrawPointingRayColors;
        private SerializedProperty gazeCursorPrefab;
        private SerializedProperty gazeProviderType;
        private SerializedProperty useHeadGazeOverride;
        private SerializedProperty useEyeTrackingDataWhenAvailable;

        private static bool showGazeProviderProperties = true;
        private UnityEditor.Editor gazeProviderEditor;

        private SerializedProperty pointerMediator;
        private SerializedProperty primaryPointerSelector;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointingExtent = serializedObject.FindProperty("pointingExtent");
            defaultRaycastLayerMasks = serializedObject.FindProperty("pointingRaycastLayerMasks");
            pointerOptions = serializedObject.FindProperty("pointerOptions");
            debugDrawPointingRays = serializedObject.FindProperty("debugDrawPointingRays");
            debugDrawPointingRayColors = serializedObject.FindProperty("debugDrawPointingRayColors");
            gazeCursorPrefab = serializedObject.FindProperty("gazeCursorPrefab");
            gazeProviderType = serializedObject.FindProperty("gazeProviderType");
            useHeadGazeOverride = serializedObject.FindProperty("useHeadGazeOverride");
            useEyeTrackingDataWhenAvailable = serializedObject.FindProperty("isEyeTrackingEnabled");
            pointerMediator = serializedObject.FindProperty("pointerMediator");
            primaryPointerSelector = serializedObject.FindProperty("primaryPointerSelector");
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
                EditorGUILayout.PropertyField(pointingExtent);
                EditorGUILayout.PropertyField(defaultRaycastLayerMasks, RaycastLayerMaskContent, true);
                EditorGUILayout.PropertyField(pointerMediator);
                EditorGUILayout.PropertyField(primaryPointerSelector);

                GUIStyle boldFoldout = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Gaze Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(gazeCursorPrefab, GazeCursorPrefabContent);
                    EditorGUILayout.PropertyField(gazeProviderType);
                    EditorGUILayout.PropertyField(useHeadGazeOverride);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(useEyeTrackingDataWhenAvailable, UseEyeTrackingDataContent);
                    // Render a help link for getting started with eyetracking documentation
                    string helpURL = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input/eye-tracking/eye-tracking-basic-setup";
                    InspectorUIUtility.RenderDocumentationButton(helpURL);
                    EditorGUILayout.EndHorizontal();

#if UNITY_2019_3_OR_NEWER
                    if (useEyeTrackingDataWhenAvailable.boolValue && MixedRealityOptimizeUtils.IsBuildTargetUWP() && !PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.GazeInput))
                    {
                        EditorGUILayout.HelpBox(EnableGazeCapabilityContent, MessageType.Warning);
                        if (InspectorUIUtility.RenderIndentedButton("Set GazeInput capability"))
                        {
                            PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.GazeInput, true);
                        }
                    }
#endif // UNITY_2019_3_OR_NEWER

                    EditorGUILayout.Space();

                    showGazeProviderProperties = EditorGUILayout.Foldout(showGazeProviderProperties, "Gaze Provider Settings", true, boldFoldout);
                    if (showGazeProviderProperties && CameraCache.Main != null)
                    {
                        var gazeProvider = CameraCache.Main.GetComponent<IMixedRealityGazeProvider>();
                        CreateCachedEditor((Object)gazeProvider, null, ref gazeProviderEditor);

                        // Provide a convenient way to toggle the gaze provider as enabled/disabled via editor
                        gazeProvider.Enabled = EditorGUILayout.Toggle("Enable Gaze Provider", gazeProvider.Enabled);

                        if (gazeProviderEditor != null)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                // Draw out the rest of the Gaze Provider's settings
                                gazeProviderEditor.OnInspectorGUI();
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                showPointerOptionProperties = EditorGUILayout.Foldout(showPointerOptionProperties, "Pointer Options", true, boldFoldout);

                if (showPointerOptionProperties)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        RenderPointerList(pointerOptions);
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
                var raycastLayerMask = newPointerOption.FindPropertyRelative("prioritizedLayerMasks");

                // Reset new entry
                controllerType.intValue = 0;
                handedness.intValue = 0;
                prefab.objectReferenceValue = null;
                raycastLayerMask.arraySize = 0;
            }

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a new Pointer Option entry.", MessageType.Warning);
                return;
            }

            bool anyPrefabChanged = false;

            for (int i = 0; i < list.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    Color prevColor = GUI.color;

                    var pointerOption = list.GetArrayElementAtIndex(i);
                    var controllerType = pointerOption.FindPropertyRelative("controllerType");
                    var handedness = pointerOption.FindPropertyRelative("handedness");
                    var prefab = pointerOption.FindPropertyRelative("pointerPrefab");
                    var prioritizedLayerMasks = pointerOption.FindPropertyRelative("prioritizedLayerMasks");

                    GameObject pointerPrefab = prefab.objectReferenceValue as GameObject;
                    IMixedRealityPointer pointer = pointerPrefab != null ? pointerPrefab.GetComponent<IMixedRealityPointer>() : null;

                    // Display an error if the prefab doesn't have a IMixedRealityPointer Component
                    if (pointer.IsNull())
                    {
                        InspectorUIUtility.DrawError($"The prefab associated with this pointer option needs an {typeof(IMixedRealityPointer).Name} component");
                        GUI.color = MixedRealityInspectorUtility.ErrorColor;
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

                    // Ultimately sync the pointer prefab's value with the pointer option's
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(prioritizedLayerMasks, PointerRaycastLayerMaskContent, true);
                    if (EditorGUI.EndChangeCheck() && pointer.IsNotNull())
                    {
                        Undo.RecordObject(pointerPrefab, "Sync Pointer Prefab");

                        int prioritizedLayerMasksCount = prioritizedLayerMasks.arraySize;
                        if (pointer.PrioritizedLayerMasksOverride?.Length != prioritizedLayerMasksCount)
                        {
                            pointer.PrioritizedLayerMasksOverride = new LayerMask[prioritizedLayerMasksCount];
                        }

                        for (int j = 0; j < prioritizedLayerMasksCount; j++)
                        {
                            pointer.PrioritizedLayerMasksOverride[j] = prioritizedLayerMasks.GetArrayElementAtIndex(j).intValue;
                        }

                        PrefabUtility.RecordPrefabInstancePropertyModifications(pointerPrefab);
                        EditorUtility.SetDirty(pointerPrefab);
                        anyPrefabChanged = true;
                    }

                    GUI.color = prevColor;
                }
                EditorGUILayout.Space();
            }

            if (anyPrefabChanged)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
