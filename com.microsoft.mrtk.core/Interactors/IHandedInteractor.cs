// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all interactors with the concept of handedness implement.
    /// </summary>
    public interface IHandedInteractor : IXRInteractor
    {
        /// <summary>
        /// Returns the Handedness of this interactor.
        /// </summary>
        public Handedness Handedness { get; }
    }
}
