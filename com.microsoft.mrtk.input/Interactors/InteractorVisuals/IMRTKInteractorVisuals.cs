// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This interface encourages the separation of an interactor's visuals from the
    /// the interaction logic; SetVisuals is used to encapsulate the overall visibility
    /// of the interactor's visuals, and UpdateVisuals is a per-frame update function
    /// that can be used to update the visuals according to the relevant interactable.
    /// </summary>
    public interface IMRTKInteractorVisuals
    {
        /// <summary>
        /// Set whether the visuals on this interactor
        /// should be active or not.
        /// </summary>
        void SetVisuals(bool visible);

        /// <summary>
        /// Used to update the visuals based on the interactable it is engaging with
        /// </summary>
        void UpdateVisuals(XRBaseInteractable interactable);
    }
}
