// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Utilities;
#if WINDOWS_UWP
using Windows.UI.Input.Spatial;
#elif DOTNETWINRT_PRESENT
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
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
    }
}
