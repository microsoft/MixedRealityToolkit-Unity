// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom build processor that determines the correct <cref see="MRTKProfile"> to bundle
    /// with the build assets.
    /// </summary>
    internal class MRTKBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        // Required for implementing IPreprocessBuildWithReport
        int IOrderedCallback.callbackOrder => 0;

        /// <summary>
        /// Clean old settings assets from the build.
        /// </summary>
        private void Clean()
        {
            // Acquire the array of preloaded assets.
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets == null) { return; }

            // Remove all instances of MRTKSettings from old preloaded assets,
            // and reset the preloaded assets to our filtered list.
            List<Object> assets = preloadedAssets.ToList();
            foreach (Object assetObject in preloadedAssets)
            {
                if (assetObject != null
                    && assetObject.GetType() == typeof(MRTKProfile))
                {
                    assets.Remove(assetObject);
                }
            }

            PlayerSettings.SetPreloadedAssets(assets.ToArray());
        }

        /// <summary>
        /// Clean old settings and bundle new settings.
        /// </summary>
        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            Clean();

            // Get MRTK Settings from current editor config API.
            MRTKSettings currentSettings = MRTKSettings.GetOrCreateSettings();
            if (currentSettings == null)
            {
                throw new BuildFailedException(@"Could not find MRTK Settings asset for build! 
                                                Check your Assets/XR/Settings folder, as well as 
                                                Project Settings/Mixed Reality Toolkit");
            }

            // Get the specific profile that is appropriate for our currently build target.
            MRTKProfile activeProfile = currentSettings.GetProfileForBuildTarget(report.summary.platformGroup);
            if (activeProfile == null)
            {
                throw new BuildFailedException(@"No valid MRTK Profile for build target platform. 
                                                Check Project Settings/Mixed Reality Toolkit 
                                                and apply a valid MRTKProfile to your target platform.");
            }

            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

            if (!preloadedAssets.Contains(activeProfile))
            {
                List<Object> assets = preloadedAssets.ToList();
                assets.Add(activeProfile);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
                Debug.Log($"Wrote MRTK profile '{activeProfile.name}' for {report.summary.platformGroup} to build assets.");
            }
        }

        /// <summary>
        /// Clean old settings post-build.
        /// </summary>
        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            Clean();
        }
    }
}
