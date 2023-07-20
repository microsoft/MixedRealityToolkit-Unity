// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A helper class designed to force enable <see cref="GUI.enabled"/> over some lifetime. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This should be utilized within a <see langword="using"/> code block.
    /// </para>
    /// <para>
    /// This class is similar to the scope classes in Unity, like <see cref="UnityEditor.EditorGUILayout.VerticalScope"/>.
    /// </para>
    /// </remarks> 
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

        /// <summary>
        /// Reset the previous state of <see cref="GUI.enabled"/>, before this class was initialized.
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reset the previous state of <see cref="GUI.enabled"/>, before this class was initialized.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            GUI.enabled = wasGUIEnabled;
        }
    }
}