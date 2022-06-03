// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Useful extensions for parsing <see cref="InputTrackingState"/> flags.
    /// </summary>
    public static class InputTrackingStateExtensions
    {
        /// <summary>
        /// Returns true iff the state is at least both positionally and rotationally tracked.
        /// </summary>
        public static bool HasPositionAndRotation(this InputTrackingState state)
        {

            return (state & (InputTrackingState.Position | InputTrackingState.Rotation)) ==
                            (InputTrackingState.Position | InputTrackingState.Rotation);
        }
    }
}
