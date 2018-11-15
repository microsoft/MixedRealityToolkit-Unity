using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// The manager that handles the playback of AudioEvents
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the audio manager
        /// </summary>
        private static AudioManager Instance;
        /// <summary>
        /// The currently-playing events at runtime
        /// </summary>
        private static List<ActiveEvent> ActiveEvents = new List<ActiveEvent>();
        /// <summary>
        /// The AudioSource components that have been added by a previously-played event
        /// </summary>
        private static List<AudioSource> AvailableSources = new List<AudioSource>();
        /// <summary>
        /// The language that all voice events should play in
        /// </summary>
        public static AudioLanguage CurrentLanguage = AudioLanguage.English;

        #region Interface

        /// <summary>
        /// Start playing an AudioEvent
        /// </summary>
        /// <param name="eventToPlay">The AudioEvent to play</param>
        /// <param name="emitterObject">The GameObject to play the event on</param>
        /// <returns>The reference for the runtime event that can be modified or stopped explicitly</returns>
        public static ActiveEvent PlayEvent(AudioEvent eventToPlay, GameObject emitterObject)
        {
            ValidateManager();

            if (eventToPlay.InstanceLimit > 0 && CountActiveInstances(eventToPlay) >= eventToPlay.InstanceLimit)
            {
                Debug.LogFormat("AudioManager: Instance limit reached for {0}.", eventToPlay.name);
                return null;
            }
            if (eventToPlay.Group != 0)
            {
                StopGroupInstances(eventToPlay.Group);
            }

            AudioSource tempSource = GetUnusedSource(emitterObject);
            if (tempSource == null)
            {
                tempSource = emitterObject.AddComponent<AudioSource>();
                tempSource.playOnAwake = false;
            }
            else
            {
                AvailableSources.Remove(tempSource);
            }

            ActiveEvent tempEvent = new ActiveEvent(eventToPlay, tempSource);
            tempEvent.Play();
            if (AvailableSources.Contains(tempSource))
            {
                AvailableSources.Remove(tempSource);
            }

            ActiveEvents.Add(tempEvent);

            return tempEvent;
        }

        /// <summary>
        /// Start playing an AudioEvent
        /// </summary>
        /// <param name="eventToPlay">The AudioEvent to play</param>
        /// <param name="emitter">The AudioSource component to play the event on</param>
        /// <returns>The reference for the runtime event that can be modified or stopped explicitly</returns>
        public static ActiveEvent PlayEvent(AudioEvent eventToPlay, AudioSource emitter)
        {
            ValidateManager();

            if (eventToPlay.InstanceLimit > 0 && CountActiveInstances(eventToPlay) >= eventToPlay.InstanceLimit)
            {
                Debug.LogFormat("AudioManager: Instance limit reached for {0}.", eventToPlay.name);
                return null;
            }
            if (eventToPlay.Group != 0)
            {
                StopGroupInstances(eventToPlay.Group);
            }

            ActiveEvent tempEvent = new ActiveEvent(eventToPlay, emitter);
            eventToPlay.SetActiveEventProperties(tempEvent);

            ActiveEvents.Add(tempEvent);
            if (AvailableSources.Contains(emitter))
            {
                AvailableSources.Remove(emitter);
            }

            return tempEvent;
        }

        /// <summary>
        /// Accessor for the list of ActiveEvents
        /// </summary>
        /// <returns>The list of ActiveEvents</returns>
        public static List<ActiveEvent> GetActiveEvents()
        {
            return ActiveEvents;
        }

        /// <summary>
        /// Clear an ActiveEvent from the list of ActiveEvents
        /// </summary>
        /// <param name="stoppedEvent">The event that is no longer playing to remove from the ActiveEvent list</param>
        public static void RemoveActiveEvent(ActiveEvent stoppedEvent)
        {
            AvailableSources.Add(stoppedEvent.source);
            ActiveEvents.Remove(stoppedEvent);
        }

        #endregion

        #region Private Functions

        private void Update()
        {
            for (int i = 0; i < ActiveEvents.Count; i++)
            {
                ActiveEvent tempEvent = ActiveEvents[i];
                if (tempEvent != null && tempEvent.source != null)
                {
                    tempEvent.Update();
                }
            }
        }

        /// <summary>
        /// Instantiate a new GameObject and add the AudioManager component
        /// </summary>
        private static void CreateInstance()
        {
            if (Instance != null)
            {
                return;
            }

            GameObject instanceObject = new GameObject("AudioManager");
            Instance = instanceObject.AddComponent<AudioManager>();
            DontDestroyOnLoad(instanceObject);
        }

        /// <summary>
        /// Get the number of active instances of an AudioEvent
        /// </summary>
        /// <param name="audioEvent">The event to query the number of active instances of</param>
        /// <returns>The number of active instances of the specified AudioEvent</returns>
        private static int CountActiveInstances(AudioEvent audioEvent)
        {
            int tempCount = 0;

            for (int i = 0; i < ActiveEvents.Count; i++)
            {
                if (ActiveEvents[i].rootEvent == audioEvent)
                {
                    tempCount++;
                }
            }

            return tempCount;
        }

        /// <summary>
        /// Call an immediate stop on all active audio events of a particular group
        /// </summary>
        /// <param name="groupNum">The group number to stop</param>
        private static void StopGroupInstances(int groupNum)
        {
            for (int i = 0; i < ActiveEvents.Count; i++)
            {
                ActiveEvent tempEvent = ActiveEvents[i];
                if (tempEvent.rootEvent.Group == groupNum)
                {
                    Debug.LogFormat("Stopping: {0}", tempEvent.rootEvent.name);
                    tempEvent.StopImmediate();
                }
            }
        }

        /// <summary>
        /// Look for an existing AudioSource component that is not currently playing
        /// </summary>
        /// <param name="emitterObject">The GameObject the AudioSource needs to be attached to</param>
        /// <returns>An AudioSource reference if one exists, otherwise null</returns>
        private static AudioSource GetUnusedSource(GameObject emitterObject)
        {
            ClearNullAudioSources();

            for (int i = 0; i < AvailableSources.Count; i++)
            {
                AudioSource tempSource = AvailableSources[i];
                if (tempSource.gameObject == emitterObject)
                {
                    return tempSource;
                }
            }

            return null;
        }

        /// <summary>
        /// Remove any references to AudioSource components that no longer exist
        /// </summary>
        private static void ClearNullAudioSources()
        {
            for (int i = AvailableSources.Count - 1; i >= 0; i--)
            {
                AudioSource tempSource = AvailableSources[i];
                if (tempSource == null)
                {
                    AvailableSources.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Make sure that the AudioManager has all of the required components
        /// </summary>
        /// <returns>Whether there is a valid AudioManager instance</returns>
        private static bool ValidateManager()
        {
            if (Instance == null)
            {
                CreateInstance();
                if (Instance == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        #endregion

        #region Editor

        /// <summary>
        /// Mute all ActiveEvents that are not soloed
        /// </summary>
        public static void ApplyActiveSolos()
        {
            ValidateManager();

            bool soloActive = false;
            for (int i = 0; i < ActiveEvents.Count; i++)
            {
                if (ActiveEvents[i].Soloed)
                {
                    soloActive = true;
                }
            }

            if (soloActive)
            {
                for (int i = 0; i < ActiveEvents.Count; i++)
                {
                    ActiveEvents[i].ApplySolo();
                }
            }
            else
            {
                ClearActiveSolos();
            }
        }

        /// <summary>
        /// Unmute all events
        /// </summary>
        public static void ClearActiveSolos()
        {
            ValidateManager();

            for (int i = 0; i < ActiveEvents.Count; i++)
            {
                ActiveEvents[i].ClearSolo();
            }
        }

        #endregion
    }

    public enum AudioLanguage
    {
        English,
        French,
        Italian,
        German,
        Spanish
    }
}