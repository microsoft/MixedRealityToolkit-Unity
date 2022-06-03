// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A wrapper around the MRTKSettings asset file to display it in the Unity Project Settings window.
    /// </summary>
    internal static class MRTKSettingsProvider
    {
        private static UnityEditor.Editor cachedEditor = null;

        [SettingsProvider]
        public static SettingsProvider CreateMRTKSettingsProvider()
        {
            const string SettingsWindowPath = "Project/MRTK3";

            SettingsProvider provider = new SettingsProvider(SettingsWindowPath, SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    UnityEditor.Editor.CreateCachedEditor(MRTKSettings.GetOrCreateSettings(), null, ref cachedEditor);

                    if (cachedEditor != null)
                    {
                        cachedEditor.OnInspectorGUI();
                    }
                },

                keywords = new HashSet<string>(new[] { "Mixed", "Reality", "Toolkit", "MRTK", "MRTK3" })
            };

            return provider;
        }
    }
}
