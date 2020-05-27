// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Base class for all <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> Inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityProfileInspector : UnityEditor.Editor
    {
        private static readonly StringBuilder dropdownKeyBuilder = new StringBuilder();

        protected virtual void OnEnable()
        {
            if (target == null)
            {
                // Either when we are recompiling, or the inspector window is hidden behind another one, the target can get destroyed (null) and thereby will raise an ArgumentException when accessing serializedObject. For now, just return.
                return;
            }
        }

        /// <summary>
        /// Renders a non-editable object field and an editable dropdown of a profile.
        /// </summary>
        public static void RenderReadOnlyProfile(SerializedProperty property)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(property.objectReferenceValue != null ? "" : property.displayName, property.objectReferenceValue, typeof(BaseMixedRealityProfile), false, GUILayout.ExpandWidth(true));
                EditorGUI.EndDisabledGroup();
            }

            if (property.objectReferenceValue != null)
            {
                bool showReadOnlyProfile = SessionState.GetBool(property.name + ".ReadOnlyProfile", false);

                using (new EditorGUI.IndentLevelScope())
                {
                    RenderFoldout(ref showReadOnlyProfile, property.displayName, () =>
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            UnityEditor.Editor subProfileEditor = CreateEditor(property.objectReferenceValue);
                            // If this is a default MRTK configuration profile, ask it to render as a sub-profile
                            if (typeof(BaseMixedRealityToolkitConfigurationProfileInspector).IsAssignableFrom(subProfileEditor.GetType()))
                            {
                                BaseMixedRealityToolkitConfigurationProfileInspector configProfile = (BaseMixedRealityToolkitConfigurationProfileInspector)subProfileEditor;
                                configProfile.RenderAsSubProfile = true;
                            }
                            subProfileEditor.OnInspectorGUI();
                        }
                    });
                }

                SessionState.SetBool(property.name + ".ReadOnlyProfile", showReadOnlyProfile);
            }
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> property.</param>
        /// <param name="profileType">Profile type to filter available values to set on the provided property. If null, defaults to type <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/></param>
        /// <param name="showAddButton">If true, draw the clone button, if false, don't</param>
        /// <param name="renderProfileInBox">if true, render box around profile content, if false, don't</param>
        /// <param name="serviceType">Optional service type to limit available profile types.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(SerializedProperty property, Type profileType, bool showAddButton = true, bool renderProfileInBox = false, Type serviceType = null)
        {
            return RenderProfileInternal(property, profileType, showAddButton, renderProfileInBox, serviceType);
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> property.</param>
        /// <param name="showAddButton">If true, draw the clone button, if false, don't</param>
        /// <param name="renderProfileInBox">if true, render box around profile content, if false, don't</param>
        /// <param name="serviceType">Optional service type to limit available profile types.</param>
        /// <returns>True, if the profile changed.</returns>
        private static bool RenderProfileInternal(SerializedProperty property, Type profileType,
            bool showAddButton, bool renderProfileInBox, Type serviceType = null)
        {
            var profile = property.serializedObject.targetObject as BaseMixedRealityProfile;
            bool changed = false;
            var oldObject = property.objectReferenceValue;

            if (profileType != null && !profileType.IsSubclassOf(typeof(BaseMixedRealityProfile)) && profileType != typeof(BaseMixedRealityProfile))
            {
                // If they've drag-and-dropped a non-profile scriptable object, set it to null.
                profileType = null;
            }

            // If we're constraining this to a service type, check whether the profile is valid
            // If it isn't, issue a warning.
            if (serviceType != null && oldObject != null)
            {
                if (!MixedRealityProfileUtility.IsProfileForService(oldObject.GetType(), serviceType))
                {
                    EditorGUILayout.HelpBox("This profile is not supported for " + serviceType.Name + ". Using an unsupported service may result in unexpected behavior.", MessageType.Warning);
                }
            }

            if (profileType == null)
            {
                // Find the profile type so we can limit the available object field options
                if (serviceType != null)
                {
                    // If GetProfileTypesForService has a count greater than one, then it won't be possible to use
                    // EditorGUILayout.ObjectField to restrict the set of profiles to a single type - in this
                    // case all profiles of BaseMixedRealityProfile will be visible in the picker.
                    //
                    // However in the case where there is just a single profile type for the service, we can improve
                    // upon the user experience by limiting the set of things that show in the picker by restricting
                    // the set of profiles listed to only that type.
                    var availableTypes = MixedRealityProfileUtility.GetProfileTypesForService(serviceType);
                    if (availableTypes.Count == 1)
                    {
                        profileType = availableTypes.First();
                    }
                }

                // If the profile type is still null, just set it to base profile type
                if (profileType == null)
                {
                    profileType = typeof(BaseMixedRealityProfile);
                }
            }

            // Draw the profile dropdown
            changed |= MixedRealityInspectorUtility.DrawProfileDropDownList(property, profile, oldObject, profileType, showAddButton);

            Debug.Assert(profile != null, "No profile was set in OnEnable. Did you forget to call base.OnEnable in a derived profile class?");

            // Draw the sub-profile editor
            MixedRealityInspectorUtility.DrawSubProfileEditor(property.objectReferenceValue, renderProfileInBox);

            return changed;
        }

        /// <summary>
        /// Render Bold/HelpBox style Foldout
        /// </summary>
        /// <param name="currentState">reference bool for current visibility state of foldout</param>
        /// <param name="title">Title in foldout</param>
        /// <param name="renderContent">code to execute to render inside of foldout</param>
        /// <param name="preferenceKey">optional argument, current show/hide state will be tracked associated with provided preference key</param>
        protected static void RenderFoldout(ref bool currentState, string title, Action renderContent, string preferenceKey = null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            bool isValidPreferenceKey = !string.IsNullOrEmpty(preferenceKey);
            bool state = currentState;
            if (isValidPreferenceKey)
            {
                state = SessionState.GetBool(preferenceKey, currentState);
            }

            currentState = EditorGUILayout.Foldout(state, title, true, MixedRealityStylesUtility.BoldFoldoutStyle);

            if (isValidPreferenceKey && currentState != state)
            {
                SessionState.SetBool(preferenceKey, currentState);
            }

            if (currentState)
            {
                renderContent();
            }

            EditorGUILayout.EndVertical();
        }

        private static string GetSubProfileDropdownKey(SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
            {
                throw new Exception("Can't get sub profile dropdown key for a property that is null.");
            }

            dropdownKeyBuilder.Clear();
            dropdownKeyBuilder.Append("MRTK_SubProfile_ShowDropdown_");
            dropdownKeyBuilder.Append(property.name);
            dropdownKeyBuilder.Append("_");
            dropdownKeyBuilder.Append(property.objectReferenceValue.GetType().Name);
            return dropdownKeyBuilder.ToString();
        }

        /// <summary>
        /// Checks if the profile is locked
        /// </summary>
        protected static bool IsProfileLock(BaseMixedRealityProfile profile)
        {
            return MixedRealityProjectPreferences.LockProfiles && !profile.IsCustomProfile;
        }
    }
}
