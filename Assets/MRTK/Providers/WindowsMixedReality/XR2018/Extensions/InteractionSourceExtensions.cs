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
                IReadOnlyList<SpatialInteractionSourceState> sources = null;

                UnityEngine.WSA.Application.InvokeOnUIThread(() =>
                {
                    sources = SpatialInteractionManager.GetForCurrentView()?.GetDetectedSourcesAtTimestamp(PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));
                }, true);

                for (var i = 0; i < sources?.Count; i++)
                {
                    if (sources[i].Source.Id.Equals(interactionSource.id))
                    {
                        returnValue = sources[i].Source.Controller.TryGetRenderableModelAsync();
                    }
                }
            }

            return returnValue;
        }
#endif // WINDOWS_UWP
    }
}
