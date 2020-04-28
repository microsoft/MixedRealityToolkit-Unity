// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// MRTK project preferences access and inspector rendering logic
    /// </summary>
    public static class MixedRealityProjectPreferences
    {
        #region Lock Profile Preferences

        private static readonly GUIContent LockContent = new GUIContent("Lock SDK profiles", "Locks the SDK profiles from being edited.");
        private const string LOCK_KEY = "_MixedRealityToolkit_Editor_LockProfiles";
        private static bool lockPrefLoaded;
        private static bool lockProfiles;

        /// <summary>
        /// Should the default profile inspectors be disabled to prevent editing?
        /// </summary>
        public static bool LockProfiles
        {
            get
            {
                if (!lockPrefLoaded)
                {
                    lockProfiles = ProjectPreferences.Get(LOCK_KEY, true);
                    lockPrefLoaded = true;
                }

                return lockProfiles;
            }
            set => ProjectPreferences.Set(LOCK_KEY, lockProfiles = value);
        }

        #endregion Lock Profile Preferences

        #region Ignore startup settings prompt

        private static readonly GUIContent IgnoreContent = new GUIContent("Ignore MRTK project configurator", "Prevents settings dialog popup from showing.");
        private const string IGNORE_KEY = "_MixedRealityToolkit_Editor_IgnoreSettingsPrompts";
        private static bool ignorePrefLoaded;
        private static bool ignoreSettingsPrompt;

        /// <summary>
        /// Should the project configurator show when the project isn't configured according to MRTK's recommendations?
        /// </summary>
        public static bool IgnoreSettingsPrompt
        {
            get
            {
                if (!ignorePrefLoaded)
                {
                    ignoreSettingsPrompt = ProjectPreferences.Get(IGNORE_KEY, false);
                    ignorePrefLoaded = true;
                }

                return ignoreSettingsPrompt;
            }
            set => ProjectPreferences.Set(IGNORE_KEY, ignoreSettingsPrompt = value);
        }

        #endregion Ignore startup settings prompt

        #region Auto-Enable UWP Capabilities

        private static readonly GUIContent AutoEnableCapabilitiesContent = new GUIContent("Auto-enable UWP capabilities", "When this setting is enabled, MRTK services requiring particular UWP capabilities will be auto-enabled in Publishing Settings.\n\nOnly valid for UWP Build Target projects.\n\nUWP Capabilities can be viewed under Player Settings > Publishing Settings.");
        private const string AUTO_ENABLE_CAPABILITIES_KEY = "_MixedRealityToolkit_Editor_AutoEnableUWPCapabilities";
        private static bool autoEnabledCapabilitiesPrefLoaded;
        private static bool autoEnabledCapabilitiesSettingsPrompt;

        /// <summary>
        /// Should the UWP capabilities required by MRTK services be auto-enabled in Publishing Settings?
        /// </summary>
        /// <remarks>Only valid for UWP Build Target projects. UWP Capabilities can be viewed under Player Settings > Publishing Settings.</remarks>
        public static bool AutoEnableUWPCapabilities
        {
            get
            {
                if (!autoEnabledCapabilitiesPrefLoaded)
                {
                    autoEnabledCapabilitiesSettingsPrompt = ProjectPreferences.Get(AUTO_ENABLE_CAPABILITIES_KEY, true);
                    autoEnabledCapabilitiesPrefLoaded = true;
                }

                return autoEnabledCapabilitiesSettingsPrompt;
            }
            set => ProjectPreferences.Set(AUTO_ENABLE_CAPABILITIES_KEY, autoEnabledCapabilitiesSettingsPrompt = value);
        }

        #endregion Auto-Enable UWP Capabilities

        #region Run optimal configuration analysis on Play

        private static readonly GUIContent RunOptimalConfigContent = new GUIContent("Run optimal configuration analysis", "Run optimal configuration analysis for current project and log warnings on entering play mode or building.");
        private const string RUN_OPTIMAL_CONFIG_KEY = "MixedRealityToolkit_Editor_RunOptimalConfig";
        private static bool runOptimalConfigPrefLoaded;
        private static bool runOptimalConfig;

        /// <summary>
        /// Should configuration analysis be run and warnings logged when settings don't match MRTK's recommendations?
        /// </summary>
        public static bool RunOptimalConfiguration
        {
            get
            {
                if (!runOptimalConfigPrefLoaded)
                {
                    runOptimalConfig = ProjectPreferences.Get(RUN_OPTIMAL_CONFIG_KEY, true);
                    runOptimalConfigPrefLoaded = true;
                }

                return runOptimalConfig;
            }
            set => ProjectPreferences.Set(RUN_OPTIMAL_CONFIG_KEY, runOptimalConfig = value);
        }

        #endregion Run optimal configuration analysis on Play

        #region Project configuration cache

        // This section contains data that gets cached for future reference to help detect configuration
        // changes that may result in a need to alert the application developer (ex: count of installed
        // plugins of a specific type). There is no UI in the project settings dialog for these properties.

        private const string AUDIO_SPATIALIZER_COUNT_KEY = "MixedRealityToolkit_Editor_AudioSpatializerCount";
        private static bool audioSpatializerCountLoaded;
        private static int audioSpatializerCount;

        /// <summary>
        /// The cached number of audio spatializers that were most recently detected.
        /// </summary>
        /// <remarks>Used to track when the number of installed spatializers changes.</remarks>
        public static int AudioSpatializerCount
        {
            get
            {
                if (!audioSpatializerCountLoaded)
                {
                    audioSpatializerCount = ProjectPreferences.Get(AUDIO_SPATIALIZER_COUNT_KEY, 0);
                    audioSpatializerCountLoaded = true;
                }

                return audioSpatializerCount;
            }
            set
            {
                audioSpatializerCount = value;
                ProjectPreferences.Set(AUDIO_SPATIALIZER_COUNT_KEY, audioSpatializerCount);
            }
        }

        #endregion Project configuration cache

        [SettingsProvider]
        private static SettingsProvider Preferences()
        {
            var provider = new SettingsProvider("Project/Mixed Reality Toolkit", SettingsScope.Project)
            {
                guiHandler = GUIHandler,

                keywords = new HashSet<string>(new[] { "Mixed", "Reality", "Toolkit" })
            };

            void GUIHandler(string searchContext)
            {
                EditorGUILayout.HelpBox("These settings are serialized into ProjectPreferences.asset in the MixedRealityToolkit-Generated folder.\nThis file can be checked into source control to maintain consistent settings across collaborators.", MessageType.Info);

                var prevLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 250f;

                bool lockProfilesResult = EditorGUILayout.Toggle(LockContent, LockProfiles);
                if (lockProfilesResult != LockProfiles)
                {
                    LockProfiles = lockProfilesResult;
                }

                if (!LockProfiles)
                {
                    EditorGUILayout.HelpBox("This is only to be used to update the default SDK profiles. If any edits are made, and not checked into the Mixed Reality Toolkit - Unity repository, the changes may be lost next time you update your local copy.", MessageType.Warning);
                }

                bool ignoreResult = EditorGUILayout.Toggle(IgnoreContent, IgnoreSettingsPrompt);
                if (IgnoreSettingsPrompt != ignoreResult)
                {
                    IgnoreSettingsPrompt = ignoreResult;
                }

                bool autoEnableResult = EditorGUILayout.Toggle(AutoEnableCapabilitiesContent, AutoEnableUWPCapabilities);
                if (AutoEnableUWPCapabilities != autoEnableResult)
                {
                    AutoEnableUWPCapabilities = autoEnableResult;
                }

                var scriptLock = EditorGUILayout.Toggle("Is script reloading locked?", EditorAssemblyReloadManager.LockReloadAssemblies);
                if (EditorAssemblyReloadManager.LockReloadAssemblies != scriptLock)
                {
                    EditorAssemblyReloadManager.LockReloadAssemblies = scriptLock;
                }

                EditorGUI.BeginChangeCheck();
                runOptimalConfig = EditorGUILayout.Toggle(RunOptimalConfigContent, RunOptimalConfiguration);

                // Save the preference
                if (EditorGUI.EndChangeCheck())
                {
                    RunOptimalConfiguration = runOptimalConfig;
                }

                EditorGUIUtility.labelWidth = prevLabelWidth;
            }

            return provider;
        }
    }

}
