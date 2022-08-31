// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [InitializeOnLoad]
    [System.Serializable]
    /// <summary>
    /// Root settings class that holds a mapping of build target groups to profiles.
    /// </summary>
    public class MRTKSettings : ScriptableObject
    {
        internal const string MRTKGeneratedFolder = "Assets/MRTK.Generated";
        internal const string MRTKSettingsPath = MRTKGeneratedFolder + "/MRTKSettings.asset";

        [SerializeField]
        private SerializableDictionary<BuildTargetGroup, MRTKProfile> settings = new SerializableDictionary<BuildTargetGroup, MRTKProfile>();

        [SerializeField]
        private MRTKBuildPreferences buildPreferences = default;

        internal MRTKBuildPreferences BuildPreferences => buildPreferences;

        private void OnEnable()
        {
            MRTKProfile.Instance = GetProfileForBuildTarget(BuildTargetGroup.Standalone);
        }

        /// <summary>
        /// Associates a profile with the specified build target group.
        /// </summary>
        /// <param name="targetGroup">An enum specifying which platform group this build is for.</param>
        /// <param name="profile">An instance of <see cref="MRTKProfile"/> to assign for the given key.</param>
        public void SetProfileForBuildTarget(BuildTargetGroup targetGroup, MRTKProfile profile)
        {
            // Ensures the editor's "runtime instance" is the most current for standalone settings
            if (targetGroup == BuildTargetGroup.Standalone)
            {
                MRTKProfile.Instance = profile;
            }
            settings[targetGroup] = profile;
        }

        /// <summary>
        /// Instance method to retrieve a profile for the specified build target group.
        /// </summary>
        /// <param name="targetGroup">An enum specifying which platform group this build is for.</param>
        /// <returns>The instance of <see cref="MRTKProfile"/> assigned to the key, or null if not.</returns>
        public MRTKProfile GetProfileForBuildTarget(BuildTargetGroup targetGroup)
        {
            settings.TryGetValue(targetGroup, out MRTKProfile ret);
            return ret;
        }

        /// <summary>
        /// Static method to obtain the profile for a given build target group,
        /// retrieved from <see cref="EditorBuildSettings">.
        /// </summary>
        /// <param name="targetGroup">An enum specifying which platform group this build is for.</param>
        /// <returns>The instance of <see cref="MRTKProfile"/> assigned to the key, or null if not.</returns>
        public static MRTKProfile ProfileForBuildTarget(BuildTargetGroup targetGroup)
        {
            MRTKSettings buildTargetSettings = GetOrCreateSettings();
            if (buildTargetSettings == null)
            {
                return null;
            }

            return buildTargetSettings.GetProfileForBuildTarget(targetGroup);
        }

        internal static MRTKSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MRTKSettings>(MRTKSettingsPath);
            if (settings == null)
            {
                if (!Directory.Exists(MRTKGeneratedFolder))
                {
                    Directory.CreateDirectory(MRTKGeneratedFolder);
                }

                settings = CreateInstance<MRTKSettings>();
                AssetDatabase.CreateAsset(settings, MRTKSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        static MRTKSettings()
        {
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    // Poke the MRTKSettings instance to ensure OnEnable() is called.
                    // This ensures MRTKProfile.Instance is set, if one exists
                    // for Standalone/Editor, while in the editor.
                    _ = GetOrCreateSettings();
                }
            };
        }
    }
}
