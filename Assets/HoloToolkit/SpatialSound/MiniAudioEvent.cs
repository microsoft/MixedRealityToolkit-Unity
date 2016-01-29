using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The main object of UAudioMiniManager: contains the settings and a container for playing audio clips
    /// </summary>
    [System.Serializable]
    public class MiniAudioEvent : AudioEvent
    {
        /// <summary>
        /// Main AudioSource to be set in the inspector
        /// </summary>
        public AudioSource primarySource = null;
        /// <summary>
        /// Secondary AudioSource for continuous containers to be set in the inspector
        /// </summary>
        public AudioSource secondarySource = null;
    }
}