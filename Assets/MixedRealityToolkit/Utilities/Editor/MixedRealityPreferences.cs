// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public static class MixedRealityPreferences
    {
        #region Lock Profile Preferences

        private static readonly GUIContent LockContent = new GUIContent("Lock SDK profiles", "Locks the SDK profiles from being edited.\n\nThis setting only applies to the currently running project.");
        private const string LOCK_KEY = "LockProfiles";
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
                    lockProfiles = EditorPreferences.Get(LOCK_KEY, true);
                    lockPrefLoaded = true;
                }

                return lockProfiles;
            }
            set => EditorPreferences.Set(LOCK_KEY, lockProfiles = value);
        }

        #endregion Lock Profile Preferences

        #region Ignore startup settings prompt

        private static readonly GUIContent IgnoreContent = new GUIContent("Ignore settings prompt on startup", "Prevents settings dialog popup from showing on startup.\n\nThis setting applies to all projects using the Mixed Reality Toolkit.");
        private const string IGNORE_KEY = "_MixedRealityToolkit_Editor_IgnoreSettingsPrompts";
        private static bool ignorePrefLoaded;
        private static bool ignoreSettingsPrompt;

        /// <summary>
        /// Should the settings prompt show on startup?
        /// </summary>
        public static bool IgnoreSettingsPrompt
        {
            get
            {
                if (!ignorePrefLoaded)
                {
                    ignoreSettingsPrompt = EditorPrefs.GetBool(IGNORE_KEY, false);
                    ignorePrefLoaded = true;
                }

                return ignoreSettingsPrompt;
            }
            set => EditorPrefs.SetBool(IGNORE_KEY, ignoreSettingsPrompt = value);
        }

        #endregion Ignore startup settings prompt

        #region Auto-Enable UWP Capabilities

        private static readonly GUIContent AutoEnableCapabilitiesContent = new GUIContent("Auto-Enable UWP Capabilities", "When this setting is enabled, MRTK services requiring particular UWP capabilities will be auto-enabled in Publishing Settings.\n\nOnly valid for UWP Build Target projects.\n\nUWP Capabilities can be viewed under Player Settings > Publishing Settings.");
        private const string AUTO_ENABLE_CAPABILITIES_KEY = "_MixedRealityToolkit_Editor_AutoEnableUWPCapabilities";
        private static bool autoEnabledCapabilitiesPrefLoaded;
        private static bool autoEnabledCapabiltiiesSettingsPrompt;

        /// <summary>
        /// Should the settings prompt show on startup?
        /// </summary>
        public static bool AutoEnableUWPCapabilities
        {
            get
            {
                if (!autoEnabledCapabilitiesPrefLoaded)
                {
                    autoEnabledCapabiltiiesSettingsPrompt = EditorPrefs.GetBool(IGNORE_KEY, true);
                    autoEnabledCapabilitiesPrefLoaded = true;
                }

                return autoEnabledCapabiltiiesSettingsPrompt;
            }
            set => EditorPrefs.SetBool(IGNORE_KEY, autoEnabledCapabiltiiesSettingsPrompt = value);
        }

        #endregion Auto-Enable UWP Capabilities

        #region Run optimal configuration analysis on Play

        private static readonly GUIContent RunOptimalConfigContent = new GUIContent("Run optimal configuration analysis on play", "Run optimal configuration analysis for current project and log warnings on entering play mode.\n\nThis setting applies to all projects using the Mixed Reality Toolkit.");
        private const string RUN_OPTIMAL_CONFIG_KEY = "MixedRealityToolkit_Editor_RunOptimalConfig";
        private static bool runOptimalConfigPrefLoaded;
        private static bool runOptimalConfig;

        /// <summary>
        /// Should the settings prompt show on startup?
        /// </summary>
        public static bool RunOptimalConfiguration
        {
            get
            {
                if (!runOptimalConfigPrefLoaded)
                {
                    runOptimalConfig = EditorPrefs.GetBool(RUN_OPTIMAL_CONFIG_KEY, true);
                    runOptimalConfigPrefLoaded = true;
                }

                return runOptimalConfig;
            }
            set => EditorPrefs.SetBool(RUN_OPTIMAL_CONFIG_KEY, runOptimalConfig = value);
        }

        #endregion Run optimal configuration analysis on Play

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
