// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
#if WINDOWS_UWP
using Windows.Perception;
using Windows.UI.Input.Spatial;
#elif DOTNETWINRT_PRESENT
using Microsoft.Windows.Perception;
using Microsoft.Windows.UI.Input.Spatial;
#endif
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Provides useful extensions for Windows-defined types.
    /// </summary>
    public static class WindowsExtensions
    {
#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        /// <summary>
        /// Converts a platform <see cref="SpatialInteractionSourceHandedness"/> into
        /// the equivalent value in MRTK's defined <see cref="Handedness"/>.
        /// </summary>
        /// <param name="handedness">The handedness value to convert.</param>
        /// <returns>The converted value in the new type.</returns>
        public static Handedness ToMRTKHandedness(this SpatialInteractionSourceHandedness handedness)
        {
            switch (handedness)
            {
                case SpatialInteractionSourceHandedness.Left:
                    return Handedness.Left;
                case SpatialInteractionSourceHandedness.Right:
                    return Handedness.Right;
                case SpatialInteractionSourceHandedness.Unspecified:
                default:
                    return Handedness.None;
            }
        }

        /// <summary>
        /// Tries to get an active SpatialInteractionSource with the corresponding handedness and input source type.
        /// </summary>
        /// <param name="handedness">The handedness of the source to get.</param>
        /// <param name="inputSourceType">The input source type of the source to get.</param>
        /// <returns>The input source or null if none could be found.</returns>
        public static SpatialInteractionSource GetSpatialInteractionSource(Handedness handedness, InputSourceType inputSourceType)
        {
            SpatialInteractionSourceHandedness sourceHandedness;
            switch (handedness)
            {
                default:
                    sourceHandedness = SpatialInteractionSourceHandedness.Unspecified;
                    break;
                case Handedness.Left:
                    sourceHandedness = SpatialInteractionSourceHandedness.Left;
                    break;
                case Handedness.Right:
                    sourceHandedness = SpatialInteractionSourceHandedness.Right;
                    break;
            }

            SpatialInteractionSourceKind sourceKind;
            switch (inputSourceType)
            {
                default:
                    sourceKind = SpatialInteractionSourceKind.Other;
                    break;
                case InputSourceType.Controller:
                    sourceKind = SpatialInteractionSourceKind.Controller;
                    break;
                case InputSourceType.Hand:
                    sourceKind = SpatialInteractionSourceKind.Hand;
                    break;
            }

            System.Collections.Generic.IReadOnlyList<SpatialInteractionSourceState> sourceStates =
                WindowsMixedRealityUtilities.SpatialInteractionManager?.GetDetectedSourcesAtTimestamp(PerceptionTimestampHelper.FromHistoricalTargetTime(System.DateTimeOffset.UtcNow));

            if (sourceStates == null)
            {
                return null;
            }

            foreach (SpatialInteractionSourceState sourceState in sourceStates)
            {
                if (sourceState.Source.Handedness == sourceHandedness && sourceState.Source.Kind == sourceKind)
                {
                    return sourceState.Source;
                }
            }

            return null;
        }
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
    }
}
