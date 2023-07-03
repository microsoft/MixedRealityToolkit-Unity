// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Adds the chosen tap sound to the NonNativeKeyboard buttons
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class KeyboardAudio : MonoBehaviour
    {
        private AudioSource clickSoundPlayer;

        private void Start()
        {
            EnableTouch();
        }

        private void EnableTouch()
        {
            clickSoundPlayer = gameObject.GetComponent<AudioSource>();
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.onClick.AddListener(PlayClick);
            }
        }

        private void PlayClick()
        {
            if (clickSoundPlayer.enabled && clickSoundPlayer.gameObject.activeInHierarchy)
            {
                clickSoundPlayer.Play();
            }
        }
    }
}
