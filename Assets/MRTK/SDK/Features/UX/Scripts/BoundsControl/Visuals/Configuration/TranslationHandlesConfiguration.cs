// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Configuration for <see cref="TranslationHandles"/> used in <see cref="BoundsControl"/>
    /// This class provides all data members needed to create translation handles for <see cref="BoundsControl"/>
    /// </summary>
    [CreateAssetMenu(fileName = "TranslationHandlesConfiguration", menuName = "Mixed Reality Toolkit/Bounds Control/Translation Handles Configuration")]
    public class TranslationHandlesConfiguration : PerAxisHandlesConfiguration
    {
        TranslationHandlesConfiguration()
        {
            // translation handles are turned off by default
            ShowHandleForX = false;
            ShowHandleForY = false;
            ShowHandleForZ = false;
        }
       
        /// <summary>
        /// Fabricates an instance of TranslationHandles, applying
        /// this config to it whilst creating it.
        /// </summary>
        /// <returns>New TranslationHandles</returns>
        internal virtual TranslationHandles ConstructInstance()
        {
            // Return a new TranslationHandles, using this config as the active config.
            return new TranslationHandles(this);
        }
    }
}
