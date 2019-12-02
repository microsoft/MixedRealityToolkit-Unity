// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor.Search;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Base class for all Mixed Reality Toolkit specific <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityProfileInspector
    {
        public bool RenderAsSubProfile { get; set; }

        private static GUIContent WarningIconContent = null;

        /// <summary>
        /// Helper function to determine if the current profile is assigned to the active instance of MRTK.
        /// In some cases profile data refers to other profile data in the MRTK config profile.
        /// In these cases, we don't want to render when the active instance isn't using this profile,
        /// because it may produce an inaccurate combination of settings.
        /// </summary>
        protected abstract bool IsProfileInActiveInstance();

        /// <summary>
        /// Internal enum used for back navigation along profile hierarchy. 
        /// Indicates what type of parent profile the current profile will return to for going back
        /// </summary>
        protected enum BackProfileType
        {
            Configuration,
            Input,
            SpatialAwareness,
            RegisteredServices
        };

        // NOTE: Must match number of elements in BackProfileType
        protected readonly string[] BackProfileDescriptions = {
            "Back to Configuration Profile",
            "Back to Input Profile",
            "Back to Spatial Awareness Profile",
            "Back to Registered Service Providers Profile"
        };

        protected virtual void Awake()
        {
            if (WarningIconContent == null)
            {
                WarningIconContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon").image,
                    "This profile is part of the default set from the Mixed Reality Toolkit SDK. You can make a copy of this profile, and customize it if needed.");
            }
        }

        /// <summary>
        /// Render the Mixed Reality Toolkit Logo and search field.
        /// </summary>
        /// <returns>True if the rest of the inspector should be drawn.</returns>
        protected bool RenderMRTKLogoAndSearch()
        {
            // If we're being rendered as a sub profile, don't show the logo
            if (RenderAsSubProfile)
            {
                return true;
            }

            if (MixedRealitySearchInspectorUtility.DrawSearchInterface(target))
            {
                return false;
            }

            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            return true;
        }

        /// <summary>
        /// Draws a documentation link for the service.
        /// </summary>
        protected void RenderDocumentation(Object profileObject)
        {
            if (profileObject == null)
            {   // Can't proceed if profile is null.
                return;
            }

            HelpURLAttribute helpURL = profileObject.GetType().GetCustomAttribute<HelpURLAttribute>();
            if (helpURL != null)
            {
                InspectorUIUtility.RenderDocumentationButton(helpURL.URL);
            }
        }

        protected bool DrawBacktrackProfileButton(BackProfileType returnProfileTarget = BackProfileType.Configuration)
        {
            // We cannot select the correct profile if there is no instance
            if (!MixedRealityToolkit.IsInitialized)
            {
                return false;
            }

            string backText = BackProfileDescriptions[(int)returnProfileTarget];
            BaseMixedRealityProfile backProfile = null;
            switch (returnProfileTarget)
            {
                case BackProfileType.Configuration:
                    backProfile = MixedRealityToolkit.Instance.ActiveProfile;
                    break;
                case BackProfileType.Input:
                    backProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
                    break;
                case BackProfileType.SpatialAwareness:
                    backProfile = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile;
                    break;
                case BackProfileType.RegisteredServices:
                    backProfile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
                    break;
            }

            return DrawBacktrackProfileButton(backText, backProfile);
        }

        /// <summary>
        /// Renders a button that will take user back to a specified profile object
        /// </summary>
        /// <returns>True if button was clicked</returns>
        protected bool DrawBacktrackProfileButton(string message, UnityEngine.Object activeObject)
        {
            // If we're being rendered as a sub profile, don't show the button
            if (RenderAsSubProfile)
            {
                return false;
            }

            if (GUILayout.Button(message))
            {
                Selection.activeObject = activeObject;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper function to render header correctly for all profiles
        /// </summary>
        /// <param name="title">Title of profile</param>
        /// <param name="description">profile tooltip describing purpose</param>
        /// <param name="selectionObject">The profile object. Used to re-select the object after MRTK instance is created.</param>
        /// <param name="isProfileInitialized">profile properties are full initialized for rendering</param>
        /// <param name="backText">Text for back button if not rendering as sub-profile</param>
        /// <param name="backProfile">Target profile to return to if not rendering as sub-profile</param>
        /// <returns>True if the rest of the profile should be rendered.</returns>
        protected bool RenderProfileHeader(string title, string description, Object selectionObject, bool isProfileInitialized = true, BackProfileType returnProfileTarget = BackProfileType.Configuration)
        {
            if (!RenderMRTKLogoAndSearch())
            {
                CheckEditorPlayMode();
                return false;
            }

            var profile = target as BaseMixedRealityProfile;
            if (!RenderAsSubProfile)
            {
                CheckEditorPlayMode();

                if (!profile.IsCustomProfile)
                {
                    EditorGUILayout.HelpBox("Default MRTK profiles cannot be edited. Create a clone of this profile to modify settings.", MessageType.Warning);
                    if (GUILayout.Button(new GUIContent("Clone")))
                    {
                        MixedRealityProfileCloneWindow.OpenWindow(null, (BaseMixedRealityProfile)target, null);
                    }
                }

                if (IsProfileInActiveInstance())
                {
                    DrawBacktrackProfileButton(returnProfileTarget);
                }

                if (!isProfileInitialized)
                {
                    if (!MixedRealityToolkit.IsInitialized)
                    {
                        EditorGUILayout.HelpBox("There is not a MRTK instance in your scene. Some properties may not be editable", MessageType.Error);
                        if (InspectorUIUtility.RenderIndentedButton(new GUIContent("Add Mixed Reality Toolkit instance to scene"), EditorStyles.miniButton))
                        {
                            MixedRealityInspectorUtility.AddMixedRealityToolkitToScene(MixedRealityInspectorUtility.GetDefaultConfigProfile());
                            // After the toolkit has been created, set the selection back to this item so the user doesn't get lost
                            Selection.activeObject = selectionObject;
                        }
                    }
                    else if(!MixedRealityToolkit.Instance.HasActiveProfile)
                    {
                        EditorGUILayout.HelpBox("There is no active profile assigned in the current MRTK instance. Some properties may not be editable.", MessageType.Error);
                    }
                }
            }
            else
            {
                if (!isProfileInitialized && profile.IsCustomProfile)
                {
                    EditorGUILayout.HelpBox("Some properties may not be editable in this profile. Please refer to the error messages below to resolve editing.", MessageType.Warning);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent(title, description), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                RenderDocumentation(selectionObject);
            }

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

            return true;
        }

        /// <summary>
        /// If application is playing, then show warning to the user and disable inspector GUI
        /// </summary>
        /// <returns>true if application is playing, false otherwise</returns>
        protected bool CheckEditorPlayMode()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Mixed Reality Toolkit settings cannot be edited while in play mode.", MessageType.Warning);
                GUI.enabled = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if various input settings are set correctly to read the input actions for the active MRTK instance. If any failures, show appropriate error message
        /// </summary>
        protected void CheckMixedRealityInputActions()
        {
            if (MixedRealityToolkit.IsInitialized && MixedRealityToolkit.Instance.HasActiveProfile)
            {
                if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
                {
                    EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Warning);
                }

                if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile == null)
                {
                    EditorGUILayout.HelpBox("No input system profile found, please specify an input system profile in the main configuration.", MessageType.Error);
                }
                else if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
                {
                    EditorGUILayout.HelpBox("No input actions profile found, please specify an input action profile in the main configuration.", MessageType.Error);
                }
                else if (!IsProfileInActiveInstance())
                {
                    EditorGUILayout.HelpBox("This profile is not assigned to the active MRTK instance in your scene. Some properties may not be editable", MessageType.Error);
                }
            }
        }
    }
}
