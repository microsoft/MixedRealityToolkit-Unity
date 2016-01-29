namespace HoloToolkit.Unity
{
    /// <summary>
    /// A single audio clip with playback settings
    /// </summary>
    [System.Serializable]
    public class AudioClip
    {
        public UnityEngine.AudioClip sound = null;
        public bool looping = false;
        public float delayCenter = 0;
        public float delayRandomization = 0;
    }
}