// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Profiles
{
    public class ButtonSoundProfile : ButtonProfile
    {
        // Direct interaction clips
        [HideInMRTKInspector]
        public AudioClip ButtonCanceled;
        [HideInMRTKInspector]
        public AudioClip ButtonHeld;
        [HideInMRTKInspector]
        public AudioClip ButtonPressed;
        [HideInMRTKInspector]
        public AudioClip ButtonReleased;

        // State change clips
        [HideInMRTKInspector]
        public AudioClip ButtonObservation;
        [HideInMRTKInspector]
        public AudioClip ButtonObservationTargeted;
        [HideInMRTKInspector]
        public AudioClip ButtonTargeted;

        // Volumes
        [HideInMRTKInspector]
        public float ButtonCanceledVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonHeldVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonPressedVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonReleasedVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonObservationVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonObservationTargetedVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonTargetedVolume = 1f;
    }
}