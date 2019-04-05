// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_WSA
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;

#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using Windows.Devices.Haptics;
using Windows.Foundation;
using Windows.Perception;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#elif UNITY_EDITOR_WIN
using System.Runtime.InteropServices;
#endif

#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
    /// <summary>
    /// Extensions for the InteractionSource class to add haptics and expose the renderable model.
    /// </summary>
    public static class InteractionSourceExtensions
    {
#if UNITY_EDITOR_WIN && UNITY_WSA
        [DllImport("EditorMotionController")]
        private static extern bool StartHaptics([In] uint controllerId, [In] float intensity, [In] float durationInSeconds);

        [DllImport("EditorMotionController")]
        private static extern bool StopHaptics([In] uint controllerId);
#endif // UNITY_EDITOR_WIN && UNITY_WSA

        // This value is standardized according to www.usb.org/developers/hidpage/HUTRR63b_-_Haptics_Page_Redline.pdf
        private const ushort ContinuousBuzzWaveform = 0x1004;

#if UNITY_WSA
        public static void StartHaptics(this InteractionSource interactionSource, float intensity)
        {
            interactionSource.StartHaptics(intensity, float.MaxValue);
        }

        public static void StartHaptics(this InteractionSource interactionSource, float intensity, float durationInSeconds)
        {
            if (!WindowsApiChecker.UniversalApiContractV4_IsAvailable && !Application.isEditor)
            {
                return;
            }

#if !UNITY_EDITOR
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                IReadOnlyList<SpatialInteractionSourceState> sources = SpatialInteractionManager.GetForCurrentView().GetDetectedSourcesAtTimestamp(PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));

                foreach (SpatialInteractionSourceState sourceState in sources)
                {
                    if (sourceState.Source.Id.Equals(interactionSource.id))
                    {
                        SimpleHapticsController simpleHapticsController = sourceState.Source.Controller.SimpleHapticsController;
                        foreach (SimpleHapticsControllerFeedback hapticsFeedback in simpleHapticsController.SupportedFeedback)
                        {
                            if (hapticsFeedback.Waveform.Equals(ContinuousBuzzWaveform))
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
                    }
                }
            }, true);
#elif UNITY_EDITOR_WIN
            StartHaptics(interactionSource.id, intensity, durationInSeconds);
#endif // !UNITY_EDITOR
        }

        public static void StopHaptics(this InteractionSource interactionSource)
        {
            if (!WindowsApiChecker.UniversalApiContractV4_IsAvailable && !Application.isEditor)
            {
                return;
            }

#if !UNITY_EDITOR
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                IReadOnlyList<SpatialInteractionSourceState> sources = SpatialInteractionManager.GetForCurrentView().GetDetectedSourcesAtTimestamp(PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));

                foreach (SpatialInteractionSourceState sourceState in sources)
                {
                    if (sourceState.Source.Id.Equals(interactionSource.id))
                    {
                        sourceState.Source.Controller.SimpleHapticsController.StopFeedback();
                    }
                }
            }, true);
#elif UNITY_EDITOR_WIN
            StopHaptics(interactionSource.id);
#endif // !UNITY_EDITOR
        }

#if !UNITY_EDITOR
        public static IAsyncOperation<IRandomAccessStreamWithContentType> TryGetRenderableModelAsync(this InteractionSource interactionSource)
        {
            IAsyncOperation<IRandomAccessStreamWithContentType> returnValue = null;

            if (WindowsApiChecker.UniversalApiContractV5_IsAvailable)
            {
                UnityEngine.WSA.Application.InvokeOnUIThread(() =>
                {
                    IReadOnlyList<SpatialInteractionSourceState> sources = SpatialInteractionManager.GetForCurrentView().GetDetectedSourcesAtTimestamp(PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));

                    for (var i = 0; i < sources.Count; i++)
                    {
                        if (sources[i].Source.Id.Equals(interactionSource.id))
                        {
                            returnValue = sources[i].Source.Controller.TryGetRenderableModelAsync();
                        }
                    }
                }, true);
            }

            return returnValue;
        }
#endif // !UNITY_EDITOR
#endif // UNITY_WSA
    }
}
