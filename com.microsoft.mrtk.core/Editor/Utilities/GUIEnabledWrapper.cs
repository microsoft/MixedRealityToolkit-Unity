﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Similar to the scope classes in Unity (i.e VerticalScope),
    /// This class is a helper class designed to force enable GUI.enabled over some lifetime.
    /// Should be utilized with using{} code block.
    /// </summary>
    public class GUIEnabledWrapper : IDisposable
    {
        private readonly bool wasGUIEnabled;

        /// <summary>
        /// Captures whether the Unity editor GUI state was enabled or not. Then forces enable to true
        /// </summary>
        public GUIEnabledWrapper()
        {
            wasGUIEnabled = GUI.enabled;
            GUI.enabled = true;
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