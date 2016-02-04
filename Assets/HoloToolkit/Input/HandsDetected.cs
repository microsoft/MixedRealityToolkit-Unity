using UnityEngine;
using UnityEngine.VR.WSA.Input;


namespace HoloToolkit.Unity
{
    /// <summary>
    /// HandsDetected determines if the hand is currently detected or not.
    /// </summary>
    public class HandsDetected : Singleton<HandsDetected>
    {
        /// <summary>
        /// HandDetected tracks the hand detected state.
        /// </summary>
        public bool HandDetected
        {
            get;
            private set;
        }

        void Awake()
        {
            SourceManager.SourceDetected += SourceManager_SourceDetected;
            SourceManager.SourceUpdated += SourceManager_SourceUpdated;
            SourceManager.SourceLost += SourceManager_SourceLost;
        }

        private void SourceManager_SourceDetected(SourceState state)
        {
            HandDetected = true;
        }

        private void SourceManager_SourceUpdated(SourceState state)
        {
            // SourceUpdated sets HandDetected to true so that in the case of
            // using two hands, if one hand is lost, the HandDetected state reflects
            // that there is still one hand detected.
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
            SourceManager.SourceUpdated -= SourceManager_SourceUpdated;
            SourceManager.SourceLost -= SourceManager_SourceLost;
        }
    }
}

