using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Currently active AudioEvents along with their AudioSource components for instance limiting events
    /// </summary>
    public class ActiveEvent
    {
        private AudioSource primarySource = null;
        public AudioSource PrimarySource
        {
            get
            {
                return this.primarySource;
            }
            set
            {
                this.primarySource = value;
                if (this.primarySource != null)
                {
                    this.primarySource.enabled = true;
                }
            }
        }
        private AudioSource secondarySource = null;
        public AudioSource SecondarySource
        {
            get
            {
                return this.secondarySource;
            }
            set
            {
                this.secondarySource = value;
                if (this.secondarySource != null)
                {
                    this.secondarySource.enabled = true;
                }
            }
        }
        public AudioEvent audioEvent = null;
        public bool isStoppable = true;
        public float volDest = 1;
        public float altVolDest = 1;
        public float currentFade = 0;
        public bool playingAlt = false;
        public float activeTime = 0;
        public bool stopLoop = false;
    }
}