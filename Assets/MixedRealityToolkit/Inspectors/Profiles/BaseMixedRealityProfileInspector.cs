// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Base class for all <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> Inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityProfileInspector : UnityEditor.Editor
    {
        private const string IsCustomProfileProperty = "isCustomProfile";
        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");
        private static readonly String BaseMixedRealityProfileClassName = typeof(BaseMixedRealityProfile).Name;

        private static BaseMixedRealityProfile profile;
        private static SerializedObject targetProfile;
        private static BaseMixedRealityProfile profileToCopy;
        private static StringBuilder dropdownKeyBuilder = new StringBuilder();

        protected virtual void OnEnable()
        {
            if (target == null)
            {
                // Either when we are recompiling, or the inspector window is hidden behind another one, the target can get destroyed (null) and thereby will raise an ArgumentException when accessing serializedObject. For now, just return.
                return;
            }

            targetProfile = serializedObject;
            profile = target as BaseMixedRealityProfile;
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> property.</param>
        /// <param name="guiContent">The GUIContent for the field.</param>
        /// <param name="showAddButton">Optional flag to hide the create button.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(SerializedProperty property, GUIContent guiContent, bool showAddButton = true, Type serviceType = null)
        {
            return RenderProfileInternal(property, guiContent, showAddButton, serviceType);
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> property.</param>
        /// <param name="showAddButton">Optional flag to hide the create button.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(SerializedProperty property, bool showAddButton = true, Type serviceType = null)
        {
            return RenderProfileInternal(property, null, showAddButton, serviceType);
        }

        private static bool RenderProfileInternal(SerializedProperty property, GUIContent guiContent, bool showAddButton, Type serviceType = null)
        {
            profile = property.serializedObject.targetObject as BaseMixedRealityProfile;

            bool changed = false;

            var oldObject = property.objectReferenceValue;

            // If we're constraining this to a service type, check whether the profile is valid
            // If it isn't, issue a warning.
            if (serviceType != null && oldObject != null)
            {
                if (!IsProfileForService(oldObject.GetType(), serviceType))
                {
                    EditorGUILayout.HelpBox("This profile is not supported for " + serviceType.Name + ". Using an unsupported service may result in unexpected behavior.", MessageType.Warning);
                }
            }

            EditorGUILayout.BeginHorizontal();
            RenderProfileField(property, guiContent, GetProfileTypesForService(serviceType));
            if (property.objectReferenceValue == null)
            {
                var profileTypeName = property.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                if (showAddButton && IsConcreteProfileType(profileTypeName))
                {
                    if (GUILayout.Button(NewProfileContent, EditorStyles.miniButton, GUILayout.Width(20f)))
                    {
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

                if (GUILayout.Button(new GUIContent("Clone", "Replace with a copy of the default profile."), EditorStyles.miniButton, GUILayout.Width(42f)))
                {
                    MixedRealityProfileCloneWindow.OpenWindow(profile, renderedProfile, property);
                }
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
                    showFoldout = EditorGUILayout.Foldout(showFoldout, showFoldout ? "Hide " + property.displayName + " contents" : "Show " + property.displayName + " contents", true);

                    if (showFoldout)
                    {
                        UnityEditor.Editor subProfileEditor = UnityEditor.Editor.CreateEditor(property.objectReferenceValue);

                        // If this is a default MRTK configuration profile, ask it to render as a sub-profile
                        if (typeof(BaseMixedRealityToolkitConfigurationProfileInspector).IsAssignableFrom(subProfileEditor.GetType()))
                        {
                            BaseMixedRealityToolkitConfigurationProfileInspector configProfile = (BaseMixedRealityToolkitConfigurationProfileInspector)subProfileEditor;
                            configProfile.RenderAsSubProfile = true;
                        }

                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        subProfileEditor.OnInspectorGUI();

                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        EditorGUILayout.EndVertical();
                        EditorGUI.indentLevel--;
                    }

                    SessionState.SetBool(showFoldoutKey, showFoldout);
                }
            }

            return changed;
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

        /// <summary>
        /// Given a service type, finds all sub-classes of BaseMixedRealityProfile that are
        /// designed to configure that service.
        /// </summary>
        private static IReadOnlyCollection<Type> GetProfileTypesForService(Type serviceType)
        {
            if (serviceType == null)
            {
                return Array.Empty<Type>();
            }

            // This is a little inefficient in that it has to enumerate all of the mixed reality
            // profiles in order to make this enumeration. It would be possible to cache the results
            // of this, but then it would be necessary to listen to file/asset creation/destruction
            // events in order to refresh the cache. If this ends up being a perf bottleneck
            // in inspectors this would be one possible way to alleviate the issue.
            HashSet<Type> allTypes = new HashSet<Type>();
            BaseMixedRealityProfile[] allProfiles = ScriptableObjectExtensions.GetAllInstances<BaseMixedRealityProfile>();
            for (int i = 0; i < allProfiles.Length; i++)
            {
                BaseMixedRealityProfile profile = allProfiles[i];
                if (IsProfileForService(profile.GetType(), serviceType))
                {
                    allTypes.Add(profile.GetType());
                }
            }
            return allTypes.ToReadOnlyCollection();
        }

        /// <summary>
        /// Returns true if the given profile type is designed to configure the given service.
        /// </summary>
        private static bool IsProfileForService(Type profileType, Type serviceType)
        {
            foreach (MixedRealityServiceProfileAttribute serviceProfileAttribute in profileType.GetCustomAttributes(typeof(MixedRealityServiceProfileAttribute), true))
            {
                if (serviceProfileAttribute.ServiceType.IsAssignableFrom(serviceType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Renders the profile field, optionally restricting the set of selectable types based on the given
        /// profileTypes parameter.
        /// </summary>
        /// <remarks>
        /// If profileTypes has a count greater than one, then it won't be possible to use
        /// EditorGUILayout.ObjectField to restrict the set of profiles to a single type - in this
        /// case all profiles of BaseMixedRealityProfile will be visible in the picker.
        /// 
        /// However in the case where there is just a single profile type for the service, we can improve
        /// upon the user experience by limiting the set of things that show in the picker by restricting
        /// the set of profiles listed to only that type.
        /// </remarks>
        private static void RenderProfileField(SerializedProperty property, GUIContent guiContent, IReadOnlyCollection<Type> profileTypes)
        {
            if (profileTypes.Count == 1)
            {
                if (guiContent != null)
                {
                    EditorGUILayout.ObjectField(property, profileTypes.Single(), guiContent);
                }
                else
                {
                    EditorGUILayout.ObjectField(property, profileTypes.Single());
                }
            }
            else
            {
                if (guiContent != null)
                {
                    EditorGUILayout.ObjectField(property, guiContent);
                }
                else
                {
                    EditorGUILayout.ObjectField(property);
                }
            }
        }

        private static bool IsConcreteProfileType(String profileTypeName)
        {
            return profileTypeName != BaseMixedRealityProfileClassName;
        }
    }
}