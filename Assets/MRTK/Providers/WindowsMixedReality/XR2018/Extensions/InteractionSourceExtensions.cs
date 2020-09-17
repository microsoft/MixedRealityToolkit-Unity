// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;
using Windows.Foundation;
using Windows.Perception;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
    /// <summary>
    /// Extensions for the InteractionSource class to expose the renderable model.
    /// </summary>
    public static class InteractionSourceExtensions
    {
#if WINDOWS_UWP
        /// <summary>
        /// Attempts to call the Windows API for loading the controller renderable model at runtime.
        /// </summary>
        /// <param name="interactionSource">The source to try loading the model for.</param>
        /// <returns>A stream of the glTF model for loading.</returns>
        /// <remarks>Doesn't work in-editor.</remarks>
        public static IAsyncOperation<IRandomAccessStreamWithContentType> TryGetRenderableModelAsync(this InteractionSource interactionSource)
        {
            IAsyncOperation<IRandomAccessStreamWithContentType> returnValue = null;

            // GetForCurrentView and GetDetectedSourcesAtTimestamp were both introduced in the same Windows version.
            // We need only check for one of them.
            if (WindowsApiChecker.IsMethodAvailable(
                "Windows.UI.Input.Spatial",
                "SpatialInteractionManager",
                "GetForCurrentView"))
            {
                returnValue = interactionSource.GetSpatialInteractionSource()?.Controller.TryGetRenderableModelAsync();
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionSource"></param>
        /// <returns></returns>
        public static SpatialInteractionSourceState GetSpatialInteractionSourceState(this InteractionSource interactionSource)
        {
            IReadOnlyList<SpatialInteractionSourceState> sources = WindowsMixedRealityUtilities.SpatialInteractionManager?.GetDetectedSourcesAtTimestamp(PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.UtcNow));

            for (var i = 0; i < sources?.Count; i++)
            {
                if (sources[i].Source.Id.Equals(interactionSource.id))
                {
                    return sources[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionSource"></param>
        /// <returns></returns>
        public static SpatialInteractionSource GetSpatialInteractionSource(this InteractionSource interactionSource) => interactionSource.GetSpatialInteractionSourceState()?.Source;
#endif // WINDOWS_UWP
    }
}
