// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A reticle that implements some visual effect controllable by a single float value.
    /// </summary>
    public interface IVariableReticle
    {
        /// <summary>
        /// Updates visuals as needed for the variable reticle.
        /// </summary>
        public void UpdateVisuals(VariableReticleUpdateArgs args);
    }

    /// <summary>
    /// A struct to store the arguments passed to UpdateVisuals
    /// including the interactor associated with the reticle, and reticle position and normal.
    /// </summary>
    public struct VariableReticleUpdateArgs
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
        /// Constructor for <see cref="VariableReticleUpdateArgs"/>.
        /// </summary>
        public VariableReticleUpdateArgs(IXRInteractor interactor, Vector3 reticlePosition, Vector3 reticleNormal)
        {
            Interactor = interactor;
            ReticlePosition = reticlePosition;
            ReticleNormal = reticleNormal;
        }
    }
}
