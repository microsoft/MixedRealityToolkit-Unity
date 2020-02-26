// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A collection of shared functionality for MRTK shader GUIs.
    /// </summary>
    public static class MixedRealityToolkitShaderGUIUtilities
    {
        /// <summary>
        /// GUI content styles which are common among shader GUIs.
        /// </summary>
        public static class Styles
        {
            public static readonly GUIContent DepthWriteWarning = new GUIContent("<color=yellow>Warning:</color> Depth buffer sharing is enabled for this project, but this material does not write depth. Enabling depth will improve reprojection, but may cause rendering artifacts in translucent materials.");
            public static readonly GUIContent DepthWriteFixNowButton = new GUIContent("Fix Now", "Enables Depth Write For This Material");
        }

        /// <summary>
        /// Displays a depth write warning and fix button if depth buffer sharing is enabled.
        /// </summary>
        /// <param name="materialEditor">The material editor to display the warning in.</param>
        /// <param name="dialogTitle">The title of the dialog window to display when the user selects the fix button.</param>
        /// <param name="dialogMessage">The message in the dialog window when the user selects the fix button.</param>
        /// <returns>True if the user opted to fix the warning, false otherwise.</returns>
        public static bool DisplayDepthWriteWarning(MaterialEditor materialEditor, string dialogTitle = "Depth Write", string dialogMessage = "Change this material to write to the depth buffer?")
        {
            bool dialogConfirmed = false;

            if (MixedRealityOptimizeUtils.IsDepthBufferSharingEnabled())
            {
                var defaultValue = EditorStyles.helpBox.richText;
                EditorStyles.helpBox.richText = true;

                if (materialEditor.HelpBoxWithButton(Styles.DepthWriteWarning, Styles.DepthWriteFixNowButton))
                {
                    if (EditorUtility.DisplayDialog(dialogTitle, dialogMessage, "Yes", "No"))
                    {
                        dialogConfirmed = true;
                    }
                }

                EditorStyles.helpBox.richText = defaultValue;
            }

            return dialogConfirmed;
        }
    }
}
