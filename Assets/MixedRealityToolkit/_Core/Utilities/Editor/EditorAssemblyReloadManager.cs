// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Editor
{
    public static class EditorAssemblyReloadManager
    {
        private static bool locked = false;

        /// <summary>
        /// Locks the Editor's ability to reload assemblies.<para/>
        /// <remarks>
        /// This is useful for ensuring async tasks complete in the editor without having to worry if any script
        /// changes that happen during the running task will cancel it when the editor re-compiles the assemblies.
        /// </remarks>
        /// </summary>
        public static bool LockReloadAssemblies
        {
            set
            {
                locked = value;

                if (locked)
                {
                    EditorApplication.LockReloadAssemblies();
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Assembly reloading temporarily paused."));
                }
                else
                {
                    EditorApplication.UnlockReloadAssemblies();
                    EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Assembly reloading resumed."));
                }
            }
        }
    }
}
