// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An XRRayInteractor that enables eye gaze for focus and interaction.
    /// </summary>
    public class GazeInteractor : XRRayInteractor, IMRTKInteractorVisuals, IGazeInteractor
    {

        // No gaze visuals/reticles/rays/etc yet.
        /// <inheritdoc />
        public virtual void SetVisuals(bool isVisible) { }

        // No gaze visuals/reticles/rays/etc yet.
        /// <inheritdoc />
        public virtual void UpdateVisuals(XRBaseInteractable interactable) { }
    }
}
