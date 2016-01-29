namespace HoloToolkit.Unity
{
    /// <summary>
    /// The sound container for an event, with rules about how to play back audio clips
    /// </summary>
    [System.Serializable]
    public class AudioContainer
    {
        public AudioContainerType containerType = AudioContainerType.Random;
        public bool looping = false;
        public float loopTime = 0;
        public AudioClip[] sounds = null;
        public float crossfadeTime = 0f;
        public int currentClip = 0;
    }
}