// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_WSA
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using UnityEngine.XR.WSA.Input;
#endif // UNITY_WSA

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using System;
using System.Collections.Generic;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if WINDOWS_UWP
using Windows.Devices.Haptics;
using Windows.Foundation;
using Windows.Perception;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#elif (UNITY_WSA && DOTNETWINRT_PRESENT)
using Microsoft.Windows.Perception;
using Microsoft.Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
    /// <summary>
    /// Extensions for the InteractionSource class to add haptics and expose the renderable model.
    /// </summary>
    public static class InteractionSourceExtensions
    {
#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        /// <summary>
        /// Gets the current native SpatialInteractionSourceState for this InteractionSource.
        /// </summary>
        /// <param name="interactionSource">This InteractionSource to search for via the native Windows APIs.</param>
        /// <returns>The current native SpatialInteractionSourceState.</returns>
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
        /// Gets the current native SpatialInteractionSource for this InteractionSource.
        /// </summary>
        /// <param name="interactionSource">This InteractionSource to search for via the native Windows APIs.</param>
        /// <returns>The current native SpatialInteractionSource.</returns>
        public static SpatialInteractionSource GetSpatialInteractionSource(this InteractionSource interactionSource) => interactionSource.GetSpatialInteractionSourceState()?.Source;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if UNITY_WSA
        private const string HapticsNamespace = "Windows.Devices.Haptics";
        private const string SimpleHapticsController = "SimpleHapticsController";
        private const string SendHapticFeedback = "SendHapticFeedback";

        private static readonly bool IsHapticsAvailable = WindowsApiChecker.IsMethodAvailable(HapticsNamespace, SimpleHapticsController, SendHapticFeedback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionSource"></param>
        /// <param name="intensity"></param>
        /// <remarks>Doesn't work in-editor.</remarks>
        public static void StartHaptics(this InteractionSource interactionSource, float intensity) => interactionSource.StartHaptics(intensity, float.MaxValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionSource"></param>
        /// <param name="intensity"></param>
        /// <param name="durationInSeconds"></param>
        /// <remarks>Doesn't work in-editor.</remarks>
        public static void StartHaptics(this InteractionSource interactionSource, float intensity, float durationInSeconds)
        {
            if (!IsHapticsAvailable)
            {
                return;
            }

#if WINDOWS_UWP
            SimpleHapticsController simpleHapticsController = interactionSource.GetSpatialInteractionSource()?.Controller.SimpleHapticsController;
            foreach (SimpleHapticsControllerFeedback hapticsFeedback in simpleHapticsController?.SupportedFeedback)
            {
                if (hapticsFeedback.Waveform.Equals(KnownSimpleHapticsControllerWaveforms.BuzzContinuous))
                {
                    if (durationInSeconds.Equals(float.MaxValue))
                    {
                        simpleHapticsController.SendHapticFeedback(hapticsFeedback, intensity);
                    }
                    else
                    {
                        simpleHapticsController.SendHapticFeedbackForDuration(hapticsFeedback, intensity, TimeSpan.FromSeconds(durationInSeconds));
                    }
                    return;
                }
            }
#endif // WINDOWS_UWP
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionSource"></param>
        /// <remarks>Doesn't work in-editor.</remarks>
        public static void StopHaptics(this InteractionSource interactionSource)
        {
            if (!IsHapticsAvailable)
            {
                return;
            }

#if WINDOWS_UWP
            interactionSource.GetSpatialInteractionSource()?.Controller.SimpleHapticsController.StopFeedback();
#endif // WINDOWS_UWP
        }
#endif // UNITY_WSA

#if WINDOWS_UWP
        private const string SpatialNamespace = "Windows.UI.Input.Spatial";
        private const string SpatialInteractionController = "SpatialInteractionController";
        private const string TryGetRenderableModelAsyncName = "TryGetRenderableModelAsync";

        private static readonly bool IsTryGetRenderableModelAvailable = WindowsApiChecker.IsMethodAvailable(SpatialNamespace, SpatialInteractionController, TryGetRenderableModelAsyncName);

        /// <summary>
        /// Attempts to call the Windows API for loading the controller renderable model at runtime.
        /// </summary>
        /// <param name="interactionSource">The source to try loading the model for.</param>
        /// <returns>A stream of the glTF model for loading.</returns>
        /// <remarks>Doesn't work in-editor.</remarks>
        public static IAsyncOperation<IRandomAccessStreamWithContentType> TryGetRenderableModelAsync(this InteractionSource interactionSource)
        {
            if (IsTryGetRenderableModelAvailable)
            {
                return interactionSource.GetSpatialInteractionSource()?.Controller.TryGetRenderableModelAsync();
            }

            return null;
        }
#endif // WINDOWS_UWP
    }
}
