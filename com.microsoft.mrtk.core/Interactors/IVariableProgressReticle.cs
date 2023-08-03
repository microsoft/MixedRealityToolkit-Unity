// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A reticle that is compable of displaying progress of an interaction.
    /// </summary>
    /// <remarks>
    /// This is used to show selection progress and touch proximity.
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
        /// A value from 0 to 1 indicating progress of am interaction.
        /// </summary>
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
