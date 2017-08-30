// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using Windows.Devices.Haptics;
using Windows.Perception;
using Windows.UI.Input.Spatial;
#endif
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Extensions for the InteractionSource class to add haptics.
    /// </summary>
    public static class HapticsExtensions
    {
#if UNITY_WSA
        public static void StartHaptics(this InteractionSource interactionSource, float intensity)
        {
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
                            if (hapticsFeedback.Waveform.Equals(0x1004))
                            {
                                simpleHapticsController.SendHapticFeedback(hapticsFeedback, intensity);
                                return;
                            }
                        }
                    }
                }
            }, true);
#endif
        }

        public static void StartHaptics(this InteractionSource interactionSource, float intensity, float durationInSeconds)
        {
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
                            if (hapticsFeedback.Waveform.Equals(0x1004))
                            {
                                simpleHapticsController.SendHapticFeedbackForDuration(hapticsFeedback, intensity, new TimeSpan(Convert.ToInt64(durationInSeconds) * TimeSpan.TicksPerSecond));
                                return;
                            }
                        }
                    }
                }
            }, true);
#endif
        }

        public static void StopHaptics(this InteractionSource interactionSource)
        {
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
#endif
        }
#endif
    }
}