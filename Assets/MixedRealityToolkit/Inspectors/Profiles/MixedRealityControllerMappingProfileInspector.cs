// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Editor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfile))]
    public class MixedRealityControllerMappingProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private struct ControllerRenderProfile
        {
            public SupportedControllerType SupportedControllerType;
            public Handedness Handedness;
            public MixedRealityInteractionMapping[] Interactions;

            public ControllerRenderProfile(SupportedControllerType supportedControllerType, Handedness handedness, MixedRealityInteractionMapping[] interactions)
            {
                SupportedControllerType = supportedControllerType;
                Handedness = handedness;
                Interactions = interactions;
            }
        }

        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Definition");
        private static readonly GUIContent GenericTypeContent = new GUIContent("Generic Type");
        private static readonly GUIContent HandednessTypeContent = new GUIContent("Handedness");

        private static MixedRealityControllerMappingProfile thisProfile;

        private SerializedProperty mixedRealityControllerMappings;

        private static bool showControllerDefinitions = false;

        private const string ProfileTitle = "Controller Input Mapping Settings";
        private const string ProfileDescription = "Use this profile to define all the controllers and their inputs your users will be able to use in your application.\n\n" +
                                    "You'll want to define all your Input Actions first. They can then be wired up to hardware sensors, controllers, gestures, and other input devices.";

        private readonly List<ControllerRenderProfile> controllerRenderList = new List<ControllerRenderProfile>();

        protected override void OnEnable()
        {
            base.OnEnable();

            mixedRealityControllerMappings = serializedObject.FindProperty("mixedRealityControllerMappings");
            thisProfile = target as MixedRealityControllerMappingProfile;
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target), false))
            {
                serializedObject.Update();

                RenderControllerList(mixedRealityControllerMappings);

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfile;
        }

        private void RenderControllerList(SerializedProperty controllerList)
        {
            if (thisProfile.MixedRealityControllerMappings.Length != controllerList.arraySize) { return; }

            if (InspectorUIUtility.RenderIndentedButton(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                AddController(controllerList, typeof(GenericJoystickController));
                return;
            }

            controllerRenderList.Clear();

            showControllerDefinitions = EditorGUILayout.Foldout(showControllerDefinitions, "Controller Definitions");
            if (showControllerDefinitions)
            {
                using (var outerVerticalScope = new GUILayout.VerticalScope())
                {
                    GUILayout.HorizontalScope horizontalScope = null;

                    for (int i = 0; i < thisProfile.MixedRealityControllerMappings.Length; i++)
                    {
                        MixedRealityControllerMapping controllerMapping = thisProfile.MixedRealityControllerMappings[i];
                        Type controllerType = controllerMapping.ControllerType;
                        if (controllerType == null) { continue; }

                        Handedness handedness = controllerMapping.Handedness;
                        bool useCustomInteractionMappings = controllerMapping.HasCustomInteractionMappings;
                        SupportedControllerType supportedControllerType = controllerMapping.SupportedControllerType;

                        var controllerMappingProperty = controllerList.GetArrayElementAtIndex(i);
                        var handednessProperty = controllerMappingProperty.FindPropertyRelative("handedness");

                        #region Profile Migration

                        // Between MRTK v2 RC2 and GA, the HoloLens clicker and HoloLens voice select input were migrated from
                        // SupportedControllerType.WindowsMixedReality && Handedness.None to SupportedControllerType.GGVHand && Handedness.None
                        if (supportedControllerType == SupportedControllerType.WindowsMixedReality && handedness == Handedness.None)
                        {
                            for (int j = 0; j < thisProfile.MixedRealityControllerMappings.Length; j++)
                            {
                                if (thisProfile.MixedRealityControllerMappings[j].SupportedControllerType == SupportedControllerType.GGVHand &&
                                    thisProfile.MixedRealityControllerMappings[j].Handedness == Handedness.None)
                                {
                                    if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }

                                    serializedObject.ApplyModifiedProperties();

                                    for (int k = 0; k < controllerMapping.Interactions.Length; k++)
                                    {
                                        MixedRealityInteractionMapping currentMapping = controllerMapping.Interactions[k];

                                        if (currentMapping.InputType == DeviceInputType.Select)
                                        {
                                            thisProfile.MixedRealityControllerMappings[j].Interactions[0].MixedRealityInputAction = currentMapping.MixedRealityInputAction;
                                        }
                                        else if (currentMapping.InputType == DeviceInputType.SpatialGrip)
                                        {
                                            thisProfile.MixedRealityControllerMappings[j].Interactions[1].MixedRealityInputAction = currentMapping.MixedRealityInputAction;
                                        }
                                    }

                                    serializedObject.Update();
                                    controllerList.DeleteArrayElementAtIndex(i);
                                    EditorUtility.DisplayDialog("Mappings updated", "The \"HoloLens Voice and Clicker\" mappings have been migrated to a new serialization. Please save this asset.", "Okay, thanks!");
                                    return;
                                }
                            }
                        }

                        #endregion Profile Migration

                        if (!useCustomInteractionMappings)
                        {
                            bool skip = false;

                            // Merge controllers with the same supported controller type.
                            for (int j = 0; j < controllerRenderList.Count; j++)
                            {
                                if (controllerRenderList[j].SupportedControllerType == supportedControllerType &&
                                    controllerRenderList[j].Handedness == handedness)
                                {
                                    try
                                    {
                                        thisProfile.MixedRealityControllerMappings[i].SynchronizeInputActions(controllerRenderList[j].Interactions);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        Debug.LogError($"Controller mappings between {thisProfile.MixedRealityControllerMappings[i].Description} and {controllerMapping.Description} do not match. Error message: {e.Message}");
                                    }
                                    serializedObject.ApplyModifiedProperties();
                                    skip = true;
                                }
                            }

                            if (skip) { continue; }
                        }

                        controllerRenderList.Add(new ControllerRenderProfile(supportedControllerType, handedness, thisProfile.MixedRealityControllerMappings[i].Interactions));

                        string controllerTitle = thisProfile.MixedRealityControllerMappings[i].Description;
                        var interactionsProperty = controllerMappingProperty.FindPropertyRelative("interactions");

                        if (useCustomInteractionMappings)
                        {
                            if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }

                            GUILayout.Space(24f);

                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                using (horizontalScope = new GUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.LabelField(controllerTitle, EditorStyles.boldLabel);

                                    if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                                    {
                                        controllerList.DeleteArrayElementAtIndex(i);
                                        return;
                                    }
                                }

                                EditorGUI.BeginChangeCheck();

                                // Generic Type dropdown
                                Type[] genericTypes = MixedRealityControllerMappingProfile.CustomControllerMappingTypes;
                                var genericTypeListContent = new GUIContent[genericTypes.Length];
                                var genericTypeListIds = new int[genericTypes.Length];
                                int currentGenericType = -1;
                                for (int genericTypeIdx = 0; genericTypeIdx < genericTypes.Length; genericTypeIdx++)
                                {
                                    var attribute = MixedRealityControllerAttribute.Find(genericTypes[genericTypeIdx]);
                                    if (attribute != null)
                                    {
                                        genericTypeListContent[genericTypeIdx] = new GUIContent(attribute.SupportedControllerType.ToString().Replace("Generic", "").ToProperCase() + " Controller");
                                    }
                                    else
                                    {
                                        genericTypeListContent[genericTypeIdx] = new GUIContent("Unknown Controller");
                                    }

                                    genericTypeListIds[genericTypeIdx] = genericTypeIdx;

                                    if (controllerType == genericTypes[genericTypeIdx])
                                    {
                                        currentGenericType = genericTypeIdx;
                                    }
                                }
                                Debug.Assert(currentGenericType != -1);

                                currentGenericType = EditorGUILayout.IntPopup(GenericTypeContent, currentGenericType, genericTypeListContent, genericTypeListIds);
                                controllerType = genericTypes[currentGenericType];

                                {
                                    // Handedness dropdown
                                    var attribute = MixedRealityControllerAttribute.Find(controllerType);
                                    if (attribute != null && attribute.SupportedHandedness.Length >= 1)
                                    {
                                        // Make sure handedness is valid for the selected controller type.
                                        if (Array.IndexOf(attribute.SupportedHandedness, (Handedness)handednessProperty.intValue) < 0)
                                        {
                                            handednessProperty.intValue = (int)attribute.SupportedHandedness[0];
                                        }

                                        if (attribute.SupportedHandedness.Length >= 2)
                                        {
                                            var handednessListContent = new GUIContent[attribute.SupportedHandedness.Length];
                                            var handednessListIds = new int[attribute.SupportedHandedness.Length];
                                            for (int handednessIdx = 0; handednessIdx < attribute.SupportedHandedness.Length; handednessIdx++)
                                            {
                                                handednessListContent[handednessIdx] = new GUIContent(attribute.SupportedHandedness[handednessIdx].ToString());
                                                handednessListIds[handednessIdx] = (int)attribute.SupportedHandedness[handednessIdx];
                                            }

                                            handednessProperty.intValue = EditorGUILayout.IntPopup(HandednessTypeContent, handednessProperty.intValue, handednessListContent, handednessListIds);
                                        }
                                    }
                                    else
                                    {
                                        handednessProperty.intValue = (int)Handedness.None;
                                    }
                                }

                                if (EditorGUI.EndChangeCheck())
                                {
                                    interactionsProperty.ClearArray();
                                    serializedObject.ApplyModifiedProperties();
                                    thisProfile.MixedRealityControllerMappings[i].ControllerType.Type = genericTypes[currentGenericType];
                                    thisProfile.MixedRealityControllerMappings[i].SetDefaultInteractionMapping(true);
                                    serializedObject.ApplyModifiedProperties();
                                    return;
                                }

                                if (InspectorUIUtility.RenderIndentedButton("Edit Input Action Map"))
                                {
                                    ControllerPopupWindow.Show(controllerMapping, interactionsProperty, handedness);
                                }

                                if (InspectorUIUtility.RenderIndentedButton("Reset Input Actions"))
                                {
                                    interactionsProperty.ClearArray();
                                    serializedObject.ApplyModifiedProperties();
                                    thisProfile.MixedRealityControllerMappings[i].SetDefaultInteractionMapping(true);
                                    serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }
                        else
                        {
                            if (supportedControllerType == SupportedControllerType.GGVHand &&
                                handedness == Handedness.None)
                            {
                                controllerTitle = "HoloLens Voice and Clicker";
                            }

                            if (handedness != Handedness.Right)
                            {
                                if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }
                                horizontalScope = new GUILayout.HorizontalScope();
                            }

                            var buttonContent = new GUIContent(controllerTitle, ControllerMappingLibrary.GetControllerTextureScaled(controllerType, handedness));

                            if (GUILayout.Button(buttonContent, MixedRealityStylesUtility.ControllerButtonStyle, GUILayout.Height(128f), GUILayout.MinWidth(32f), GUILayout.ExpandWidth(true)))
                            {
                                ControllerPopupWindow.Show(controllerMapping, interactionsProperty, handedness);
                            }
                        }
                    }

                    if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }
                }
            }
        }

        private void AddController(SerializedProperty controllerList, Type controllerType)
        {
            controllerList.InsertArrayElementAtIndex(controllerList.arraySize);
            var index = controllerList.arraySize - 1;
            var mixedRealityControllerMapping = controllerList.GetArrayElementAtIndex(index);
            var handednessProperty = mixedRealityControllerMapping.FindPropertyRelative("handedness");
            handednessProperty.intValue = (int)Handedness.None;
            var interactionsProperty = mixedRealityControllerMapping.FindPropertyRelative("interactions");
            interactionsProperty.ClearArray();
            serializedObject.ApplyModifiedProperties();
            thisProfile.MixedRealityControllerMappings[index].ControllerType.Type = controllerType;
            thisProfile.MixedRealityControllerMappings[index].SetDefaultInteractionMapping(true);
        }
    }
}