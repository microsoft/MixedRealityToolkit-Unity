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
        public void UpdateVisuals(VariableReticleArgs args);
    }

    public struct VariableReticleArgs
    {
        /// <summary>
        /// XRRayInteractor that the reticle serves as a visual for.
        /// </summary>
        public XRRayInteractor RayInteractor;

        /// <summary>
        /// The reticle game object. 
        /// </summary>
        public GameObject Reticle;

        /// <summary>
        /// The desired reticle position from the raycast hit.
        /// </summary>
        public Vector3 ReticlePosition;

        /// <summary>
        /// The desired reticle normal from the raycast hit.
        /// </summary>
        public Vector3 ReticleNormal;

        public VariableReticleArgs(XRRayInteractor rayInteractor, GameObject customReticle, Vector3 reticlePosition, Vector3 reticleNormal)
        {
            RayInteractor = rayInteractor;
            Reticle = customReticle;
            ReticlePosition = reticlePosition;
            ReticleNormal = reticleNormal;
        }
    }
}
