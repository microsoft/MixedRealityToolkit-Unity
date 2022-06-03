// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all interactors which offer
    /// variable selection must implement.
    /// </summary>
    public interface IVariableSelectInteractor : IXRSelectInteractor, IXRHoverInteractor
    {
        /// <summary>
        /// Returns a value [0,1] representing the variable
        /// amount of "selection" that this interactor is performing.
        /// </summary>
        /// <remarks>
        /// For gaze-pinch interactors, this is the pinch progress;
        /// for motion controllers, this is the analog trigger press amount.
        /// </remarks>
        float SelectProgress { get; }
    }
}