// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Similar to the scope classes in Unity (i.e VerticalScope), 
    /// this class is a helper class designed to manage GUI.enabled over some lifetime
    /// Should be utilized with using{} code block
    /// </summary>
    public class GUIEnabledWrapper : IDisposable
    {
        private bool wasGUIEnabled;

        /// <summary>
        /// If overwrite is true, then whatever enable value is provided will be set for lifetime of exec action
        /// If overwrite is false, then will only enable GUI if already was enabled
        /// </summary>
        /// <param name="enable">desired GUI.enabled value</param>
        /// <param name="overwrite">control to disregard whether GUI.enabled was already set</param>
        public GUIEnabledWrapper(bool enable, bool overwrite = false)
        {
            this.wasGUIEnabled = GUI.enabled;
            if (overwrite)
            {
                GUI.enabled = enable;
            }
            else
            {
                GUI.enabled = enable && wasGUIEnabled;
            }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            GUI.enabled = wasGUIEnabled;
        }
    }
}