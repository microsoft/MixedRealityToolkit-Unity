// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfile))]
    public class MixedRealityControllerMappingProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private readonly struct ControllerMappingSignature
        {
            public SupportedControllerType SupportedControllerType { get; }
            public Handedness Handedness { get; }

            public ControllerMappingSignature(SupportedControllerType supportedControllerType, Handedness handedness)
            {
                SupportedControllerType = supportedControllerType;
                Handedness = handedness;
            }
        }

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
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input))
            {
                return;
            }

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
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

            // Generating the set of controllers that belong to each Controller Mapping Signature
            Dictionary<ControllerMappingSignature, List<string>> controllersAffectedByMappingSignatures = new Dictionary<ControllerMappingSignature, List<string>>();
            for (int i = 0; i < thisProfile.MixedRealityControllerMappings.Length; i++)
            {
                MixedRealityControllerMapping controllerMapping = thisProfile.MixedRealityControllerMappings[i];
                Type controllerType = controllerMapping.ControllerType;
                if (controllerType == null) { continue; }

                Handedness handedness = controllerMapping.Handedness;
                SupportedControllerType supportedControllerType = controllerMapping.SupportedControllerType;

                ControllerMappingSignature currentSignature = new ControllerMappingSignature(supportedControllerType, handedness);
                if (!controllersAffectedByMappingSignatures.ContainsKey(currentSignature))
                {
                    controllersAffectedByMappingSignatures.Add(currentSignature, new List<string>());
                }
                controllersAffectedByMappingSignatures[currentSignature].Add(controllerType.ToString());
            }

            showControllerDefinitions = EditorGUILayout.Foldout(showControllerDefinitions, "Controller Definitions", true);
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
                                    ResetInputActions(ref thisProfile.MixedRealityControllerMappings[i]);
                                }
                            }
                        }
                        else
                        {
                            if (handedness != Handedness.Right)
                            {
                                if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }
                                horizontalScope = new GUILayout.HorizontalScope();
                            }


                            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            var buttonContent = new GUIContent(controllerTitle, ControllerMappingLibrary.GetControllerTextureScaled(controllerType, handedness));
                            if (GUILayout.Button(buttonContent, MixedRealityStylesUtility.ControllerButtonStyle, GUILayout.Height(128f), GUILayout.MinWidth(32), GUILayout.ExpandWidth(true)))
                            {
                                ControllerMappingSignature buttonSignature = new ControllerMappingSignature(supportedControllerType, handedness);
                                ControllerPopupWindow.Show(controllerMapping, interactionsProperty, handedness, controllersAffectedByMappingSignatures[buttonSignature]);
                            }
                            if (GUILayout.Button(EditorGUIUtility.IconContent("_Menu"), new GUIStyle("iconButton")))
                            {
                                // create the menu and add items to it
                                GenericMenu menu = new GenericMenu();

                                // Caching the index of this controller mapping for the anonymous function
                                int index = i;
                                menu.AddItem(new GUIContent("Reset to default input actions"), false, () => ResetInputActions(ref thisProfile.MixedRealityControllerMappings[index]));
                                menu.ShowAsContext();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }
                }
            }
        }

        /// <summary>
        /// Resets the input actions of the controller mapping according to the mapping's GetDefaultInteractionMappings() function
        /// </summary>
        /// <param name="controllerMapping">A reference to the controller mapping struct getting reset</param>
        private void ResetInputActions(ref MixedRealityControllerMapping controllerMapping)
        {
            controllerMapping.SetDefaultInteractionMapping(true);
            serializedObject.ApplyModifiedProperties();
            ControllerPopupWindow.RepaintWindow();
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
