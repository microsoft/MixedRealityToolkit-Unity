// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    /// <summary>
    /// Interface for a lost tracking visual. Used by the ILostTrackingService extension.
    /// </summary>
    public interface ILostTrackingVisual
    {
        /// <summary>
        /// Completely enables or disables the visual. Should probably be linked to the root game object's active value.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Sets all visual components to the layer provided.
        /// </summary>
        /// <param name="layer">The layer to set for the visual components.</param>
        void SetLayer(int layer);

        /// <summary>
        /// Resets the visual state to default.
        /// </summary>
        void ResetVisual();
    }
}