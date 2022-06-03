// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A reticle that implements some visual effect controllable by a single float value.
    /// </summary>
    public interface IVariableReticle
    {
        /// <summary>
        /// Sets the desired select progress for the variable reticle.
        /// </summary>
        public void UpdateVisuals(float value);
    }
}
