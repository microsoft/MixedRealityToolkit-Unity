// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
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
        /// <param name="showCloneButton">If true, draw the clone button, if false, don't</param>
        /// <param name="renderProfileInBox">if true, render box around profile content, if false, don't</param>
        /// <param name="serviceType">Optional service type to limit available profile types.</param>
        /// <param name="profileRequiredOverride">Optional parameter to used to specify that a profile must be selected</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(SerializedProperty property, Type profileType, bool showCloneButton = true, bool renderProfileInBox = false, Type serviceType = null, bool profileRequiredOverride = false)
        {
            return RenderProfileInternal(property, profileType, showCloneButton, renderProfileInBox, serviceType, profileRequiredOverride);
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> property.</param>
        /// <param name="showCloneButton">If true, draw the clone button, if false, don't</param>
        /// <param name="renderProfileInBox">if true, render box around profile content, if false, don't</param>
        /// <param name="serviceType">Optional service type to limit available profile types.</param>
        /// <param name="profileRequiredOverride">Optional parameter to used to specify that a profile must be selected</param>
        /// <returns>True, if the profile changed.</returns>
        private static bool RenderProfileInternal(SerializedProperty property, Type profileType,
            bool showCloneButton, bool renderProfileInBox, Type serviceType = null, bool profileRequiredOverride = false)
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

            Type[] profileTypes = new Type[] { };

            bool requiresProfile = IsProfileRequired(serviceType) || profileRequiredOverride;
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
                    profileTypes = MixedRealityProfileUtility.GetProfileTypesForService(serviceType).ToArray();
                }
            }
            else
            {
                profileTypes = new Type[] { profileType };
            }

            // Draw the profile dropdown if a valid profileType exists
            if (profileTypes.Length != 0)
            {
                changed |= MixedRealityInspectorUtility.DrawProfileDropDownList(property, profile, oldObject, profileTypes, requiresProfile, showCloneButton);
            }
            else if (requiresProfile)
            {
                EditorGUILayout.HelpBox("No ProfileType exists which is suitable for " + serviceType.Name + ". This service requires a profile to function properly!", MessageType.Error);
            }

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

        /// <summary>
        /// Inspect the attributes of the provided system type to determine if a configuration profile is required.
        /// </summary>
        /// <param name="serviceType">The system type representing the service.</param>
        /// <returns>
        /// True if the service is decorated with an attribute indicating a profile is required, false otherwise.
        /// </returns>
        protected static bool IsProfileRequired(SystemType serviceType)
        {
            return IsProfileRequired(serviceType?.Type);
        }

        /// <summary>
        /// Inspect the attributes of the provided type to determine if a configuration profile is required.
        /// </summary>
        /// <param name="type">The type representing the service.</param>
        /// <returns>
        /// True if the type is decorated with an attribute indicating a profile is required, false otherwise.
        /// </returns>
        protected static bool IsProfileRequired(Type type)
        {
            // Services marked with the MixedRealityExtensionServiceAttribute (or a derivative)
            // support specifying whether or not a profile is required.
            MixedRealityExtensionServiceAttribute attribute = (type != null) ? MixedRealityExtensionServiceAttribute.Find(type) : null;
            return attribute != null && attribute.RequiresProfile;
        }
    }
}
