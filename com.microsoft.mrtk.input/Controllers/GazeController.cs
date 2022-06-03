// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An XRController for binding to gaze input.
    /// </summary>
    /// <remarks>
    /// All properties in this class are now deprecated. ArticulatedHandController provides
    /// equivalent functionality for each pinching hand. GazeController itself may be deprecated
    /// in a future version. This class remains for compatibility with other systems.
    /// </remarks>
    [AddComponentMenu("Scripts/Microsoft/MRTK/Input/Gaze Controller")]
    public class GazeController : ActionBasedController
    {
        // All properties in GazeController were deprecated and now have been removed.
        // GazeController itself will also be removed in the future.
    }
}
