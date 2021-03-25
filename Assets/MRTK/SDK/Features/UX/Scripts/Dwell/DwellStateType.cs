// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// The states of the dwell handler
    /// </summary>
    public enum DwellStateType
    {
        /// <summary>
        /// Default state
        /// </summary>
        None = 0,
        /// <summary>
        /// State reached when Focus enters target
        /// </summary>
        FocusGained,
        /// <summary>
        /// State reached when Focus stays on target for dwellIntentDelay seconds. Signifies user's intent to interact with the target.
        /// </summary>
        DwellIntended,
        /// <summary>
        /// State reached when Focus stays on target for dwellIntentDelay + dwellStartDelay seconds. Typically tied to starting to show feedback for dwell.
        /// </summary>
        DwellStarted,
        /// <summary>
        /// State reached when Focus stays on target for dwellIntentDelay + dwellStartDelay + timeToCompleteDwell seconds. Typically invokes the button clicked event.
        /// </summary>
        DwellCompleted,
        /// <summary>
        /// State reached when DwellStarted state is reached but focus exits the target before timeToCompleteDwell or cancel dwell is requested via code.
        /// </summary>
        DwellCanceled
    }
}

