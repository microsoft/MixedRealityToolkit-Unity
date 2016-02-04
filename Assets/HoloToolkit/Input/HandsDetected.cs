using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;


namespace HoloToolkit.Unity
{
    /// <summary>
    /// HandsDetected determins if the hand is detected or lost.
    /// </summary>
    public class HandsDetected : Singleton<HandsDetected>
    {
        /// <summary>
        /// HandDetected is a bool that tracks the hand detected state.
        /// </summary>
        public bool HandDetected
        {
            get;
            private set;
        }

        void Awake()
        {
            SourceManager.SourceDetected += SourceManager_SourceDetected;
            SourceManager.SourceLost += SourceManager_SourceLost;
        }

        private void SourceManager_SourceDetected(SourceState state)
        {
            HandDetected = true;
        }

        private void SourceManager_SourceLost(SourceState state)
        {
            HandDetected = false;
        }

        void OnDestroy()
        {
            // Unregister the SourceManager events.
            SourceManager.SourceDetected -= SourceManager_SourceDetected;
            SourceManager.SourceLost -= SourceManager_SourceLost;
        }
    }
}

