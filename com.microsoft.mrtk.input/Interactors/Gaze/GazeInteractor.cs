// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An XRRayInteractor that enables eye gaze for focus and interaction.
    /// Functionally empty while we learn more about the tooling necessary for successful gaze interaction development
    /// </summary>
    [AddComponentMenu("MRTK/Input/Gaze Interactor")]
    public class GazeInteractor : XRRayInteractor, IGazeInteractor
    {
    }
}
