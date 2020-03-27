// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Adds ability to override head gaze on a gaze provider.
    /// </summary>
    public interface IMixedRealityGazeProviderWithOverride
    {
        /// <summary>
        /// Allows head gaze to be overridden, typically by platform-specific values.
        /// </summary>
        void OverrideHeadGaze(Vector3 position, Vector3 forward);
    }
}