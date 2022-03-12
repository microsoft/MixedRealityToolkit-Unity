// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityControllerVisualizationProfile))]
    public class MixedRealityControllerVisualizationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Definition");

        private static readonly GUIContent[] HandednessSelections =
        {
            new GUIContent("Left Hand"),
            new GUIContent("Right Hand"),
            new GUIContent("Both"),
        };

        private SerializedProperty renderMotionControllers;
        private SerializedProperty defaultControllerVisualizationType;

        private SerializedProperty usePlatformControllerModels;
        private SerializedProperty platformControllerModelMaterial;
        private SerializedProperty globalLeftHandedControllerModel;
        private SerializedProperty globalRightHandedControllerModel;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;

        private static bool showControllerDefinitions = true;
        private SerializedProperty controllerVisualizationSettings;

        private readonly XRPipelineUtility xrPipelineUtility = new XRPipelineUtility();

        private MixedRealityControllerVisualizationProfile thisProfile;

        private float defaultLabelWidth;

        private const string ProfileTitle = "Controller Visualization Settings";
        private const string ProfileDescription = "Define all the custom controller visualizations you'd like to use for each controller type when they're rendered in the scene.\n\n" +
                                    "Global settings are the default fallback, and any specific controller definitions take precedence.";

        protected override void OnEnable()
        {
            base.OnEnable();

            defaultLabelWidth = EditorGUIUtility.labelWidth;

#if UNITY_2019
            xrPipelineUtility.Enable();
#endif // UNITY_2019

            thisProfile = target as MixedRealityControllerVisualizationProfile;

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            defaultControllerVisualizationType = serializedObject.FindProperty("defaultControllerVisualizationType");
            usePlatformControllerModels = serializedObject.FindProperty("usePlatformModels");
            platformControllerModelMaterial = serializedObject.FindProperty("platformModelMaterial");
            globalLeftHandedControllerModel = serializedObject.FindProperty("globalLeftControllerModel");
            globalRightHandedControllerModel = serializedObject.FindProperty("globalRightControllerModel");
            globalLeftHandModel = serializedObject.FindProperty("globalLeftHandVisualizer");
            globalRightHandModel = serializedObject.FindProperty("globalRightHandVisualizer");
            controllerVisualizationSettings = serializedObject.FindProperty("controllerVisualizationSettings");
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

                EditorGUILayout.LabelField("Visualization Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(renderMotionControllers);

                    EditorGUILayout.PropertyField(defaultControllerVisualizationType);

                    if (thisProfile.DefaultControllerVisualizationType == null ||
                        thisProfile.DefaultControllerVisualizationType.Type == null)
                    {
                        EditorGUILayout.HelpBox("A default controller visualization type must be defined!", MessageType.Error);
                    }
                }

                var leftHandModelPrefab = globalLeftHandedControllerModel.objectReferenceValue as GameObject;
                var rightHandModelPrefab = globalRightHandedControllerModel.objectReferenceValue as GameObject;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Global Controller Model Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(usePlatformControllerModels);
                    if (usePlatformControllerModels.boolValue)
                    {
                        EditorGUILayout.PropertyField(platformControllerModelMaterial);
                    }

                    if (usePlatformControllerModels.boolValue && (leftHandModelPrefab != null || rightHandModelPrefab != null))
                    {
                        EditorGUILayout.HelpBox("When platform models are used, an attempt is made to obtain controller models from the platform SDK. The global left and right models are only shown if no model can be obtained.", MessageType.Warning);
                    }

                    EditorGUI.BeginChangeCheck();
                    leftHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalLeftHandedControllerModel.displayName, "Note: If the default model is not found, the fallback is the global left hand model."), leftHandModelPrefab, typeof(GameObject), false) as GameObject;

                    if (EditorGUI.EndChangeCheck() && CheckVisualizer(leftHandModelPrefab))
                    {
                        globalLeftHandedControllerModel.objectReferenceValue = leftHandModelPrefab;
                    }

                    EditorGUI.BeginChangeCheck();
                    rightHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalRightHandedControllerModel.displayName, "Note: If the default model is not found, the fallback is the global right hand model."), rightHandModelPrefab, typeof(GameObject), false) as GameObject;

                    if (EditorGUI.EndChangeCheck() && CheckVisualizer(rightHandModelPrefab))
                    {
                        globalRightHandedControllerModel.objectReferenceValue = rightHandModelPrefab;
                    }

                    EditorGUILayout.PropertyField(globalLeftHandModel);
                    EditorGUILayout.PropertyField(globalRightHandModel);
                }

                EditorGUIUtility.labelWidth = defaultLabelWidth;

                EditorGUILayout.Space();
                showControllerDefinitions = EditorGUILayout.Foldout(showControllerDefinitions, "Controller Definitions", true);
                if (showControllerDefinitions)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        RenderControllerList(controllerVisualizationSettings);
                    }
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
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile;
        }

        private void RenderControllerList(SerializedProperty controllerList)
        {
            if (thisProfile.ControllerVisualizationSettings.Length != controllerList.arraySize) { return; }

            EditorGUILayout.Space();

            if (InspectorUIUtility.RenderIndentedButton(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                controllerList.InsertArrayElementAtIndex(controllerList.arraySize);
                var index = controllerList.arraySize - 1;
                var controllerSetting = controllerList.GetArrayElementAtIndex(index);

                var mixedRealityControllerMappingDescription = controllerSetting.FindPropertyRelative("description");
                mixedRealityControllerMappingDescription.stringValue = typeof(GenericJoystickController).Name;

                var mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");
                mixedRealityControllerHandedness.intValue = 1;

                serializedObject.ApplyModifiedProperties();

                thisProfile.ControllerVisualizationSettings[index].ControllerType.Type = typeof(GenericJoystickController);
                return;
            }

#if UNITY_2019
            xrPipelineUtility.RenderXRPipelineTabs();
#endif // UNITY_2019

            for (int i = 0; i < controllerList.arraySize; i++)
            {
                var controllerSetting = controllerList.GetArrayElementAtIndex(i);
                var mixedRealityControllerMappingDescription = controllerSetting.FindPropertyRelative("description");
                SystemType controllerType = thisProfile.ControllerVisualizationSettings[i].ControllerType;
                bool hasValidType = controllerType != null &&
                                    controllerType.Type != null;

                if (hasValidType)
                {
                    MixedRealityControllerAttribute controllerAttribute = MixedRealityControllerAttribute.Find(controllerType.Type);
                    if (controllerAttribute != null && !controllerAttribute.SupportedUnityXRPipelines.IsMaskSet(xrPipelineUtility.SelectedPipeline))
                    {
                        continue;
                    }
                }
                else if (!MixedRealityProjectPreferences.ShowNullDataProviders)
                {
                    continue;
                }

                EditorGUILayout.Space();

                mixedRealityControllerMappingDescription.stringValue = hasValidType
                    ? controllerType.Type.Name.ToProperCase()
                    : "Undefined Controller";

                serializedObject.ApplyModifiedProperties();
                SerializedProperty mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"{mixedRealityControllerMappingDescription.stringValue} {((Handedness)mixedRealityControllerHandedness.intValue).ToString().ToProperCase()} Hand{(mixedRealityControllerHandedness.intValue == (int)(Handedness.Both) ? "s" : "")}", EditorStyles.boldLabel);

                    if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                    {
                        controllerList.DeleteArrayElementAtIndex(i);
                        return;
                    }
                }

                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("controllerType"));
                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("controllerVisualizationType"));

                if (!hasValidType)
                {
                    EditorGUILayout.HelpBox("A controller type must be defined!", MessageType.Error);
                }

                var handednessValue = mixedRealityControllerHandedness.intValue - 1;

                // Reset in case it was set to something other than left, right, or both.
                if (handednessValue < 0 || handednessValue > 2) { handednessValue = 0; }

                EditorGUI.BeginChangeCheck();
                handednessValue = EditorGUILayout.IntPopup(new GUIContent(mixedRealityControllerHandedness.displayName, mixedRealityControllerHandedness.tooltip), handednessValue, HandednessSelections, null);
                if (EditorGUI.EndChangeCheck())
                {
                    mixedRealityControllerHandedness.intValue = handednessValue + 1;
                }

                var overrideModel = controllerSetting.FindPropertyRelative("overrideModel");
                var overrideModelPrefab = overrideModel.objectReferenceValue as GameObject;

                var controllerUsePlatformModelOverride = controllerSetting.FindPropertyRelative("usePlatformModels");
                EditorGUILayout.PropertyField(controllerUsePlatformModelOverride);
                if (controllerUsePlatformModelOverride.boolValue)
                {
                    var platformModelMaterial = controllerSetting.FindPropertyRelative("platformModelMaterial");
                    EditorGUILayout.PropertyField(platformModelMaterial);
                }

                if (controllerUsePlatformModelOverride.boolValue && overrideModelPrefab != null)
                {
                    EditorGUILayout.HelpBox("When platform model is used, the override model will only be used if the default model cannot be loaded from the driver.", MessageType.Warning);
                }

                EditorGUI.BeginChangeCheck();
                overrideModelPrefab = EditorGUILayout.ObjectField(new GUIContent(overrideModel.displayName, "If no override model is set, the global model is used."), overrideModelPrefab, typeof(GameObject), false) as GameObject;
                if (overrideModelPrefab == null && !controllerUsePlatformModelOverride.boolValue)
                {
                    EditorGUILayout.HelpBox("No override model was assigned and this controller will not attempt to use the platform's model, the global model will be used instead", MessageType.Warning);
                }

                if (EditorGUI.EndChangeCheck() && CheckVisualizer(overrideModelPrefab))
                {
                    overrideModel.objectReferenceValue = overrideModelPrefab;
                }
            }
        }

        private bool CheckVisualizer(GameObject modelPrefab)
        {
            if (modelPrefab == null) { return true; }

            if (PrefabUtility.GetPrefabAssetType(modelPrefab) == PrefabAssetType.NotAPrefab)
            {
                Debug.LogWarning("Assigned GameObject must be a prefab.");
                return false;
            }

            var componentList = modelPrefab.GetComponentsInChildren<IMixedRealityControllerVisualizer>();

            if (componentList == null || componentList.Length == 0)
            {
                if (thisProfile.DefaultControllerVisualizationType != null &&
                    thisProfile.DefaultControllerVisualizationType.Type != null)
                {
                    modelPrefab.AddComponent(thisProfile.DefaultControllerVisualizationType.Type);
                    return true;
                }

                Debug.LogError("No controller visualization type specified!");
            }
            else if (componentList.Length == 1)
            {
                return true;
            }
            else if (componentList.Length > 1)
            {
                Debug.LogWarning("Found too many IMixedRealityControllerVisualizer components on your prefab. There can only be one.");
            }

            return false;
        }
    }
}