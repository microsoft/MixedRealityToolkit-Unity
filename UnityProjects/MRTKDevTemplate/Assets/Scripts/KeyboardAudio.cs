using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Adds touch events to the NonNativeKeyboard buttons (and a tap sound)
    /// </summary>
    public class KeyboardAudio : MonoBehaviour
    {
        [SerializeField]
        private AudioClip clickSound = null;

        private AudioSource clickSoundPlayer;

        private void Start()
        {
            EnableTouch();
        }

        private void EnableTouch()
        {
            clickSoundPlayer = gameObject.AddComponent<AudioSource>();
            clickSoundPlayer.playOnAwake = false;
            clickSoundPlayer.clip = clickSound;
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.onClick.AddListener(PlayClick);
            }
        }

        private void PlayClick()
        {
            if (clickSound != null)
            {
                clickSoundPlayer.Play();
            }
        }
    }
}
