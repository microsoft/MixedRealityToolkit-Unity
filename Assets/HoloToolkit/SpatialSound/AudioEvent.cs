namespace HoloToolkit.Unity
{
    /// <summary>
    /// The different rules for how audio should be played back
    /// </summary>
    public enum AudioContainerType
    {
        Random,
        Sequence,
        Simultaneous,
        ContinuousSequence,
        ContinuousRandom
    }

    /// <summary>
    /// The different types of spatial positioning. HRTF/Spatial Sound options not currently available.
    /// </summary>
    public enum PositioningType
    {
        TwoD,
        ThreeD,
        SpatialSound
    }

    /// <summary>
    /// The main object of UAudioManager: contains settings and a container for playing audio clips
    /// </summary>
    [System.Serializable]
    public class AudioEvent
    {
        public string name = "_NewEvent";
        public PositioningType spatialization = PositioningType.TwoD;
        public UnityEngine.Audio.AudioMixerGroup bus = null;
        [Range(-3, 3)]
        public float pitchCenter = 1;
        public float pitchRandomization = 0;
        [Range(0, 1)]
        public float volumeCenter = 1;
        public float volumeRandomization = 0;
        public float fadeInTime = 0;
        public int instanceLimit = 0;
        public float instanceBuffer = 0;
        public AudioContainer container = new AudioContainer();
        public static bool IsContinuous(AudioEvent audioEvent)
        {
            return audioEvent.container.containerType == AudioContainerType.ContinuousRandom ||
                   audioEvent.container.containerType == AudioContainerType.ContinuousSequence;
        }
    }
}