// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A reticle that is capable of displaying interaction progress.
    /// </summary>
    /// <remarks>
    /// This may be used to show selection progress and touch proximity.
    /// </remarks>
    /// <seealso cref="VariableProgressReticleUpdateArgs"/>
    public interface IVariableProgressReticle
    {
        /// <summary>
        /// Update the progress of the visual.
        /// </summary>
        public void UpdateProgress(VariableProgressReticleUpdateArgs args);
    }

    /// <summary>
    /// A struct to store the arguments passed to <see cref="IVariableProgressReticle.UpdateProgress"/>.
    /// </summary>
    public struct VariableProgressReticleUpdateArgs
    {
        /// <summary>
        /// A value from 0 to 1 indicating interaction progress of an.
        /// </summary>
        /// <remarks>
        /// This may be used to show selection progress and touch proximity. 
        /// </remarks>
        public float Progress;

        /// <summary>
        /// Initializes a <see cref="VariableReticleUpdateArgs"/> struct.
        /// </summary>
        public VariableProgressReticleUpdateArgs(float progress)
        {
            Progress = progress;
        }
    }
}
