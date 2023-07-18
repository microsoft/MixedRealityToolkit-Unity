// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Flags used to represent a combination of different interaction methodologies.
    /// </summary>
    [System.Flags]
    public enum InteractionFlags
    {
        /// <summary>
        /// No active interaction method.
        /// </summary>
        None = 0,

        /// <summary>
        /// Near interaction, typically from a <c>Microsoft.MixedReality.Toolkit.Input.GrabInteractor</c> object.
        /// </summary>
        Near = 1 << 0,

        /// <summary>
        /// Far-ray interaction, typically from a <c>Microsoft.MixedReality.Toolkit.Input.MRTKRayInteractor</c> object.
        /// </summary>
        Ray = 1 << 1,

        /// <summary>
        /// Gaze-pinch interaction, typically from a <c>Microsoft.MixedReality.Toolkit.Input.GazePinchInteractor</c> object.
        /// </summary>
        Gaze = 1 << 2,

        /// <summary>
        /// A generic or an unknown interaction, typically from an interactor that does not
        /// implement or inherit from any MRTK-standard interfaces or base implementations.
        /// </summary>
        Generic = 1 << 31
    }

    /// <summary>
    /// Extension methods specific to the <see cref="InteractionFlags"/> enum.
    /// </summary>
    public static class InteractionFlagsExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="InteractionFlags"/> value.</param>
        /// <param name="b"><see cref="InteractionFlags"/> mask.</param>
        /// <returns><see langword="true"/> if all of the bits in the specified mask are set in the
        /// current value.</returns>
        public static bool IsMaskSet(this InteractionFlags a, InteractionFlags b)
        {
            return ((a & b) == b);
        }
    }
}