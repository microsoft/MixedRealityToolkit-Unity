// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Extensions.EditorClassExtensions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    /// <summary>
    /// Base class for all <see cref="Microsoft.MixedReality.Toolkit.Core.Definitions.BaseMixedRealityProfile"/> Inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityProfileInspector : Editor
    {
        private const string IsCustomProfileProperty = "isCustomProfile";
        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");

        private static BaseMixedRealityProfile profile;
        private static SerializedObject targetProfile;
        private static BaseMixedRealityProfile profileToCopy;
        private static StringBuilder dropdownKeyBuilder = new StringBuilder();

        protected virtual void OnEnable()
        {
            targetProfile = serializedObject;
            profile = target as BaseMixedRealityProfile;
        }

        [Obsolete("Use MixedRealityInspectorUtility.CheckMixedRealityConfigured instead")]
        public static bool CheckMixedRealityConfigured(bool flag = true)
        {
            return false;
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.Core.Definitions.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.Core.Definitions.BaseMixedRealityProfile"/> property.</param>
        /// <param name="guiContent">The GUIContent for the field.</param>
        /// <param name="showAddButton">Optional flag to hide the create button.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(SerializedProperty property, GUIContent guiContent, bool showAddButton = true)
        {
            return RenderProfileInternal(property, guiContent, showAddButton);
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.Core.Definitions.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.Core.Definitions.BaseMixedRealityProfile"/> property.</param>
        /// <param name="showAddButton">Optional flag to hide the create button.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(SerializedProperty property, bool showAddButton = true)
        {
            return RenderProfileInternal(property, null, showAddButton);
        }

        private static bool RenderProfileInternal(SerializedProperty property, GUIContent guiContent, bool showAddButton)
        {
            bool changed = false;
            EditorGUILayout.BeginHorizontal();

            var oldObject = property.objectReferenceValue;

            if (guiContent == null)
            {
                EditorGUILayout.PropertyField(property);
            }
            else
            {
                EditorGUILayout.PropertyField(property, guiContent);
            }

            if (property.objectReferenceValue == null)
            {
                if (showAddButton)
                {
                    if (GUILayout.Button(NewProfileContent, EditorStyles.miniButton, GUILayout.Width(20f)))
                    {
                        var profileTypeName = property.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                        Debug.Assert(profileTypeName != null, "No Type Found");

                        ScriptableObject instance = CreateInstance(profileTypeName);
                        var newProfile = instance.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject)) as BaseMixedRealityProfile;
                        property.objectReferenceValue = newProfile;
                        property.serializedObject.ApplyModifiedProperties();
                        changed = true;
                    }
                }
            }
            else
            {
                var renderedProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                Debug.Assert(renderedProfile != null);
                Debug.Assert(profile != null, "No profile was set in OnEnable. Did you forget to call base.OnEnable in a derived profile class?");

                if (!renderedProfile.IsCustomProfile && profile.IsCustomProfile)
                {
                    if (GUILayout.Button(new GUIContent("</>", "Replace with a copy of the default profile."), EditorStyles.miniButton, GUILayout.Width(32f)))
                    {
                        profileToCopy = renderedProfile;
                        var profileTypeName = property.objectReferenceValue.GetType().Name;
                        Debug.Assert(profileTypeName != null, "No Type Found");

                        ScriptableObject instance = CreateInstance(profileTypeName);
                        var newProfile = instance.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject)) as BaseMixedRealityProfile;
                        property.objectReferenceValue = newProfile;
                        property.serializedObject.ApplyModifiedProperties();
                        PasteProfileValuesDelay(newProfile);
                        changed = true;
                    }
                }
            }

            if (oldObject != property.objectReferenceValue)
            {
                changed = true;
            }

            EditorGUILayout.EndHorizontal();

            // Check fields within profile for other nested profiles
            // Draw them when found
            if (property.objectReferenceValue != null)
            {
                Type profileType = property.objectReferenceValue.GetType();
                if (typeof(BaseMixedRealityProfile).IsAssignableFrom(profileType))
                {
                    string showFoldoutKey = GetSubProfileDropdownKey(property);
                    bool showFoldout = SessionState.GetBool(showFoldoutKey, false);
                    showFoldout = EditorGUILayout.Foldout(showFoldout, showFoldout ? "Hide " + property.displayName + " contents" : "Show " + property.displayName + " contents");

                    if (showFoldout)
                    {
                        Editor subProfileEditor = Editor.CreateEditor(property.objectReferenceValue);

                        // If this is a default MRTK configuration profile, ask it to render as a sub-profile
                        if (typeof(BaseMixedRealityToolkitConfigurationProfileInspector).IsAssignableFrom(subProfileEditor.GetType()))
                        {
                            BaseMixedRealityToolkitConfigurationProfileInspector configProfile = (BaseMixedRealityToolkitConfigurationProfileInspector)subProfileEditor;
                            configProfile.RenderAsSubProfile = true;
                        }

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUI.indentLevel++;
                        subProfileEditor.OnInspectorGUI();
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }

                    SessionState.SetBool(showFoldoutKey, showFoldout);
                }
            }

            return changed;
        }

        private static string GetSubProfileDropdownKey(SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
                throw new Exception("Can't get sub profile dropdown key for a property that is null.");

            dropdownKeyBuilder.Clear();
            dropdownKeyBuilder.Append("MRTK_SubProfile_ShowDropdown_");
            dropdownKeyBuilder.Append(property.name);
            dropdownKeyBuilder.Append("_");
            dropdownKeyBuilder.Append(property.objectReferenceValue.GetType().Name);
            return dropdownKeyBuilder.ToString();
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Create Copy from Profile Values", false, 0)]
        protected static async void CreateCopyProfileValues()
        {
            profileToCopy = profile;
            ScriptableObject newProfile = CreateInstance(profile.GetType().ToString());
            profile = newProfile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles") as BaseMixedRealityProfile;
            Debug.Assert(profile != null);

            await new WaitUntil(() => profileToCopy != profile);

            Selection.activeObject = null;
            PasteProfileValues();
            Selection.activeObject = profile;

            if (!profileToCopy.IsCustomProfile)
            {
                // For now we only replace it if it's the master configuration profile.
                // Sub-profiles are easy to update in the master configuration inspector.
                if (MixedRealityToolkit.Instance.ActiveProfile.GetType() == profile.GetType())
                {
                    MixedRealityToolkit.Instance.ActiveProfile = profile as MixedRealityToolkitConfigurationProfile;
                }
            }
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Copy Profile Values", false, 1)]
        private static void CopyProfileValues()
        {
            profileToCopy = profile;
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", true)]
        private static bool PasteProfileValuesValidation()
        {
            return profile != null &&
                   targetProfile != null &&
                   profileToCopy != null &&
                   targetProfile.FindProperty(IsCustomProfileProperty).boolValue &&
                   profile.GetType() == profileToCopy.GetType();
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", false, 2)]
        private static void PasteProfileValues()
        {
            Undo.RecordObject(profile, "Paste Profile Values");
            bool targetIsCustom = targetProfile.FindProperty(IsCustomProfileProperty).boolValue;
            string originalName = targetProfile.targetObject.name;
            EditorUtility.CopySerialized(profileToCopy, targetProfile.targetObject);
            targetProfile.Update();
            targetProfile.FindProperty(IsCustomProfileProperty).boolValue = targetIsCustom;
            targetProfile.ApplyModifiedProperties();
            targetProfile.targetObject.name = originalName;
            Debug.Assert(targetProfile.FindProperty(IsCustomProfileProperty).boolValue == targetIsCustom);
            AssetDatabase.SaveAssets();
        }

        private static async void PasteProfileValuesDelay(BaseMixedRealityProfile newProfile)
        {
            await new WaitUntil(() => profile == newProfile);
            Selection.activeObject = null;
            PasteProfileValues();
            Selection.activeObject = newProfile;
        }
    }
}