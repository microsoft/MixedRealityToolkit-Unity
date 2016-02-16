using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// HandsDetected determines if the hand is currently detected or not.
    /// </summary>
    public class HandsManager : Singleton<HandsManager>
    {
        /// <summary>
        /// HandDetected tracks the hand detected state.
        /// Returns true if the list of tracked hands is not empty.
        /// </summary>
        public bool HandDetected
        {
            get { return trackedHands.Count > 0; }
        }

        private List<uint> trackedHands = new List<uint>();

        void Awake()
        {
            SourceManager.SourceDetected += SourceManager_SourceDetected;
            SourceManager.SourceLost += SourceManager_SourceLost;
        }

        private void SourceManager_SourceDetected(SourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != SourceKind.Hand)
            {
                return;
            }

            trackedHands.Add(state.source.id);
        }

        private void SourceManager_SourceLost(SourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != SourceKind.Hand)
            {
                return;
            }

            if (trackedHands.Contains(state.source.id))
            {
                trackedHands.Remove(state.source.id);
            }
        }

        void OnDestroy()
        {
            // Unregister the SourceManager events.
            SourceManager.SourceDetected -= SourceManager_SourceDetected;
            SourceManager.SourceLost -= SourceManager_SourceLost;
        }
    }
}
