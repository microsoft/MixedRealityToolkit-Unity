//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    public class ButtonSoundProfile : ButtonProfile
    {
        // Direct interaction clips
        public AudioClip ButtonCancelled;
        public AudioClip ButtonHeld;
        public AudioClip ButtonPressed;
        public AudioClip ButtonReleased;

        // State change clips
        public AudioClip ButtonObservation;
        public AudioClip ButtonObservationTargeted;
        public AudioClip ButtonTargeted;

        // Volumes
        public float ButtonCancelledVolume = 1f;
        public float ButtonHeldVolume = 1f;
        public float ButtonPressedVolume = 1f;
        public float ButtonReleasedVolume = 1f;
        public float ButtonObservationVolume = 1f;
        public float ButtonObservationTargetedVolume = 1f;
        public float ButtonTargetedVolume = 1f;
    }
}