// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// A convenient way to play sounds in response to button actions / states
    /// </summary>
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonSounds : ProfileButtonBase<ButtonSoundProfile>
    {
        const float MinTimeBetweenSameClip = 0.1f;
        
        [SerializeField]
        private AudioSource audioSource;
        private static string lastClipName; 
        private static float lastClipTime;
        private ButtonStateEnum lastState = ButtonStateEnum.Disabled;

        private void Start ()
        {
            Button button = GetComponent<Button>();
            button.OnButtonCanceled += OnButtonCanceled;
            button.OnButtonHeld += OnButtonHeld;
            button.OnButtonPressed += OnButtonPressed;
            button.OnButtonReleased += OnButtonReleased;
            button.StateChange += StateChange;

            audioSource = GetComponent<AudioSource>();
        }

        private void StateChange(ButtonStateEnum newState)
        {
            // Don't play the same state multiple times
            if (lastState == newState)
                return;

            lastState = newState;

            // Don't play sounds for inactive buttons
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
                return;

            if (Profile == null)
            {
                Debug.LogError("Sound profile was null in button " + name);
                return;
            }

            switch (newState)
            {
                case ButtonStateEnum.Observation:
                    PlayClip(Profile.ButtonObservation, Profile.ButtonObservationVolume);
                    break;

                case ButtonStateEnum.ObservationTargeted:
                    PlayClip(Profile.ButtonObservationTargeted, Profile.ButtonObservationTargetedVolume);
                    break;

                case ButtonStateEnum.Targeted:
                    PlayClip(Profile.ButtonTargeted, Profile.ButtonTargetedVolume);
                    break;

                default:
                    break;
            }
        }

        private void OnButtonCanceled(GameObject go)
        {
            PlayClip(Profile.ButtonCanceled, Profile.ButtonCanceledVolume);
        }

        private void OnButtonHeld(GameObject go)
        {
            PlayClip(Profile.ButtonHeld, Profile.ButtonHeldVolume);
        }

        private void OnButtonPressed(GameObject go)
        {
            PlayClip(Profile.ButtonPressed, Profile.ButtonPressedVolume);
        }

        private void OnButtonReleased (GameObject go)
        {
            PlayClip(Profile.ButtonReleased, Profile.ButtonReleasedVolume);
        }

        private void PlayClip (AudioClip clip, float volume)
        {
            if (clip != null)
            {
                // Don't play the clip if we're spamming it
                if (clip.name == lastClipName && (Time.realtimeSinceStartup - lastClipTime) < MinTimeBetweenSameClip)
                {
                    return;
                }

                lastClipName = clip.name;
                lastClipTime = Time.realtimeSinceStartup;
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(clip, volume);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(clip, transform.position, volume);
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(CompoundButtonSounds))]
        public class CustomEditor : MRTKEditor { }
#endif
    }
}