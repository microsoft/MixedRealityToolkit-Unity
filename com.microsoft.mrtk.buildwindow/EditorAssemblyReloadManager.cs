﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public static class EditorAssemblyReloadManager
    {
        private static bool locked = false;

        /// <summary>
        /// Locks the Editor's ability to reload assemblies.<para/>
        /// </summary>
        /// <remarks>
        /// <para>This is useful for ensuring async tasks complete in the editor without having to worry if any script
        /// changes that happen during the running task will cancel it when the editor re-compiles the assemblies.</para>
        /// </remarks>
        public static bool LockReloadAssemblies
        {
            set
            {
                locked = value;

                if (locked)
                {
                    EditorApplication.LockReloadAssemblies();

                    if ((EditorWindow.focusedWindow != null) &&
                        !Application.isBatchMode)
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Assembly reloading temporarily paused."));
                    }
                }
                else
                {
                    EditorApplication.UnlockReloadAssemblies();
                    EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                    if ((EditorWindow.focusedWindow != null) &&
                        !Application.isBatchMode)
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Assembly reloading resumed."));
                    }
                }
            }
            get => locked;
        }
    }
}
