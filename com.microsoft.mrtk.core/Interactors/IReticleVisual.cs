// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A customizable visual component of a reticle.
    /// </summary>
    /// <remarks>
    /// Implementations of <see cref="IReticleVisual"/> can receive updates to the base reticle's
    /// position and normal every frame, if the base reticle is shown. For more information on how
    /// set a custom reticle, see <see cref="XRBaseInteractable.AttachCustomReticle(IXRInteractor)"/>.
    /// </remarks>c
    public interface IReticleVisual
    {
        /// <summary>
        /// Updates the visual parts of the reticle.
        /// </summary>
        public void UpdateVisual(ReticleVisualUpdateArgs args);
    }

    /// <summary>
    /// A struct to store the arguments passed to <see cref="IReticleVisual.UpdateVisual"/>
    /// including the interactor associated with the reticle, and reticle position and normal.
    /// </summary>
    public struct ReticleVisualUpdateArgs
    {
        /// <summary>
        /// XRRayInteractor that the reticle serves as a visual for.
        /// </summary>
        public IXRInteractor Interactor;

        /// <summary>
        /// The desired reticle position from the raycast hit.
        /// </summary>
        public Vector3 ReticlePosition;

        /// <summary>
        /// The desired reticle normal from the raycast hit.
        /// </summary>
        public Vector3 ReticleNormal;

        /// <summary>
        /// Initializes a <see cref="ReticleVisualUpdateArgs"/> struct.
        /// </summary>
        public ReticleVisualUpdateArgs(IXRInteractor interactor, Vector3 reticlePosition, Vector3 reticleNormal)
        {
            Interactor = interactor;
            ReticlePosition = reticlePosition;
            ReticleNormal = reticleNormal;
        }
    }
}
