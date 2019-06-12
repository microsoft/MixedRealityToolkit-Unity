// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
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
        /// <returns></returns>
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
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        protected void RenderMRTKLogo()
        {
            // If we're being rendered as a sub profile, don't show the logo
            if (RenderAsSubProfile)
            {
                return;
            }

            MixedRealityEditorUtility.RenderMixedRealityToolkitLogo();
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
        /// <param name="message"></param>
        /// <param name="activeObject"></param>
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
        protected void RenderProfileHeader(string title, string description, Object selectionObject, bool isProfileInitialized = true, BackProfileType returnProfileTarget = BackProfileType.Configuration)
        {
            RenderMRTKLogo();

            var profile = target as BaseMixedRealityProfile;
            if (!RenderAsSubProfile)
            {
                if (!profile.IsCustomProfile)
                {
                    EditorGUILayout.HelpBox("Default MRTK profiles cannot be edited. Create a clone of this profile to modify settings.", MessageType.Warning);
                    if (MixedRealityEditorUtility.RenderIndentedButton(new GUIContent("Clone"), EditorStyles.miniButton))
                    {
                        MixedRealityProfileCloneWindow.OpenWindow(null, (BaseMixedRealityProfile)target, null);
                    }
                }

                if (!isProfileInitialized)
                {
                    EditorGUILayout.HelpBox("This profile is not assigned to an active MRTK instance in any of your scenes. Some properties may not be visible", MessageType.Error);

                    if (!MixedRealityToolkit.IsInitialized)
                    {
                        if (MixedRealityEditorUtility.RenderIndentedButton(new GUIContent("Add Mixed Reality Toolkit instance to scene"), EditorStyles.miniButton))
                        {
                            MixedRealityInspectorUtility.AddMixedRealityToolkitToScene(MixedRealityInspectorUtility.GetDefaultConfigProfile());
                            // After the toolkit has been created, set the selection back to this item so the user doesn't get lost
                            Selection.activeObject = selectionObject;
                        }
                    }
                }
            }
            else
            {
                if (!isProfileInitialized && profile.IsCustomProfile)
                {
                    EditorGUILayout.HelpBox("Some properties may not be editable in this profile. Please refer to the error messages below to resolve editing.", MessageType.Warning);
                }

                if (MixedRealityToolkit.IsInitialized)
                {
                    if (IsProfileInActiveInstance())
                    {
                        DrawBacktrackProfileButton(returnProfileTarget);
                    }
                    else if (!isProfileInitialized)
                    {
                        EditorGUILayout.HelpBox("This profile is not assigned to an active MRTK instance in any of your scenes. Some properties may not be editable", MessageType.Error);

                        if (!MixedRealityToolkit.IsInitialized)
                        {
                            if (MixedRealityEditorUtility.RenderIndentedButton(new GUIContent("Add Mixed Reality Toolkit instance to scene"), EditorStyles.miniButton))
                            {
                                MixedRealityInspectorUtility.AddMixedRealityToolkitToScene(MixedRealityInspectorUtility.GetDefaultConfigProfile());
                                // After the toolkit has been created, set the selection back to this item so the user doesn't get lost
                                Selection.activeObject = selectionObject;
                            }
                        }
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(title, description), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }

        /// <summary>
        /// If MRTK is in scene and input system is disabled, then show error message
        /// </summary>
        protected void RenderMixedRealityInputConfigured()
        {
            if (MixedRealityToolkit.IsInitialized && !MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);
            }
        }
    }
}
