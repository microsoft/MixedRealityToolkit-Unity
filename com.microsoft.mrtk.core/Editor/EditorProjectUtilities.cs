// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [InitializeOnLoad]
    public static class EditorProjectUtilities
    {
        private const string SessionStateKey = "EditorProjectUtilitiesSessionStateKey";

        /// <summary>
        /// Static constructor that allows for executing code on project load.
        /// </summary>
        static EditorProjectUtilities()
        {
            // This InitializeOnLoad handler only runs once at editor launch in order to adjust for Unity version
            // differences. These don't need to (and should not be) run on an ongoing basis. This uses the
            // volatile SessionState which is clear when Unity launches to ensure that this only runs the
            // expensive work once.
            if (!SessionState.GetBool(SessionStateKey, false))
            {
                SessionState.SetBool(SessionStateKey, true);
                CheckMinimumEditorVersion();
            }
        }

        /// <summary>
        /// Checks that a supported version of Unity is being used with this project.
        /// </summary>
        /// <remarks>
        /// This method displays a message to the user allowing them to continue or to exit the editor.
        /// </remarks>
        public static void CheckMinimumEditorVersion()
        {
#if !UNITY_2021_3_OR_NEWER
            DisplayIncorrectEditorVersionDialog();
#endif
        }

        /// <summary>
        /// Displays a message indicating that a project was loaded in an unsupported version of Unity and allows the user
        /// to continue or exit.
        /// </summary>
        private static void DisplayIncorrectEditorVersionDialog()
        {
            if (!EditorUtility.DisplayDialog(
                "Mixed Reality Toolkit",
                "The Mixed Reality Toolkit requires Unity 2021.3 or newer.\n\nUsing an older version of Unity may result in compile-time errors or incorrect behavior.",
                "Continue", "Close Editor"))
            {
                EditorApplication.Exit(0);
            }
        }
    }
}
