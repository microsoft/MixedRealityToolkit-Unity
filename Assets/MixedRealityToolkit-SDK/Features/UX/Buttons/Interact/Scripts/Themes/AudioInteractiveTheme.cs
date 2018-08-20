// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Themes
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioInteractiveTheme : InteractiveTheme<AudioClip>
    {

        public AudioClip Tap;

        public bool PositionalAudio = true;

        private AudioSource mAudioSource;

        private void OnEnable()
        {
            InteractiveThemeManager.AddAudioTheme(this);
            mAudioSource = GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            InteractiveThemeManager.RemoveAudioTheme(this.Tag);
        }

        public void PlayStateAudio(AudioClip clip, GameObject gameObject)
        {
            if (PositionalAudio)
            {
                // can move to game object location, there are no visual elements on this game object
                transform.position = gameObject.transform.position;
            }

            mAudioSource.clip = clip;
            mAudioSource.Play();
        }

        public void PlayTap(GameObject gameObject)
        {
            if (PositionalAudio)
            {
                transform.position = gameObject.transform.position;
            }

            if (Tap != null)
            {
                mAudioSource.clip = Tap;
                mAudioSource.Play();
            }
        }
    }

}
