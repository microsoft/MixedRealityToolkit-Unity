using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pixie.Core;
using Pixie.Initialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Pixie.StateControl.Logging
{
    public class AppStatePlayback : MonoBehaviour, IAppStatePlayback
    {
        public PlaybackStateEnum State { get { return state; } }
        public float TotalDuration { get { return totalDuration; } }
        public float CurrentTime { get { return currentTime; } }

        [SerializeField]
        private PlaybackStateEnum state = PlaybackStateEnum.Stopped;

        private IStatePipe statePipe;
        private IAppStateData appStateData;
        private List<SyncListSnapshot> snapshots = new List<SyncListSnapshot>();
        private JsonSerializerSettings settings;
        private float currentTime;
        private float previousTime;
        private float totalDuration;
        private Dictionary<Type, List<SyncListSnapshot>> snapshotLookup = new Dictionary<Type, List<SyncListSnapshot>>();
        private Dictionary<Type, int> lastSnapshotIndex = new Dictionary<Type, int>();

        public void StartPlayback(string logFilePath)
        {
            if (string.IsNullOrEmpty(logFilePath))
                throw new Exception("Log file path can't be empty.");

            switch (state)
            {
                case PlaybackStateEnum.Stopped:
                    break;

                default:
                    throw new Exception("Can't start playback in state " + state);
            }

            if (appStateData == null)
                ComponentFinder.FindInScenes<IAppStateData>(out appStateData, ComponentFinder.FailModeEnum.Exception);

            if (statePipe == null)
                ComponentFinder.FindInScenes<IStatePipe>(out statePipe, ComponentFinder.FailModeEnum.Exception);

            // Clear any existing snapshot data / time settings
            snapshotLookup.Clear();
            snapshots.Clear();
            lastSnapshotIndex.Clear();
            currentTime = 0;
            previousTime = 0;

            // Load the playback file
            LoadLogInternal(logFilePath);

            // Set the target state arrays to read-only
            foreach (Type type in snapshotLookup.Keys)
            {
                IStateArrayBase stateArray;
                if (!appStateData.TryGetData(type, out stateArray))
                    throw new Exception("Error when stopping playback, type not found in app state: " + type);

                stateArray.WriteMode = StateArrayWriteModeEnum.Playback;
            }

            state = PlaybackStateEnum.Playing;
        }

        public void SetTime(float currentTime)
        {
            switch (state)
            {
                case PlaybackStateEnum.Playing:
                    break;

                default:
                    throw new Exception("Can't set time when state is " + state);
            }

            if (currentTime < 0)
                throw new Exception("Time " + currentTime + " is invalid.");

            if (currentTime > TotalDuration)
                currentTime = TotalDuration;

            previousTime = this.currentTime;
            this.currentTime = currentTime;

            // If we haven't budged, don't do anything
            if (previousTime == currentTime)
                return;

            // Starting at our last known index, move through our snapshots until we hit the target time
            foreach (KeyValuePair<Type, List<SyncListSnapshot>> typeLookup in snapshotLookup)
            {
                Type type = typeLookup.Key;
                List<SyncListSnapshot> snapshotsByType = typeLookup.Value;
                // This was our last known frame for this type
                int lastKnownIndex = lastSnapshotIndex[type];
                // Check whether we're moving forward or backward
                bool forward = (this.currentTime > previousTime);
                // Get the next index we want to check
                int nextIndex = forward ? lastKnownIndex + 1 : lastKnownIndex - 1;
                // If the snapshot is out of range, do nothing
                if (nextIndex < 0 || nextIndex >= snapshotsByType.Count)
                    continue;

                // Get the snapshot
                SyncListSnapshot nextSnapshot = snapshotsByType[nextIndex];
                // If the time of this snapshot hasn't elapsed yet, do nothing
                if (currentTime < nextSnapshot.NetworkTime)
                    continue;

                // Store this as the last known index
                lastSnapshotIndex[type] = nextIndex;
                // Send this snapshot to the state pipe
                statePipe.SendFlushedStates(type, nextSnapshot.ChangedStates);
            }
        }

        public void StopPlayback()
        {
            switch (state)
            {
                case PlaybackStateEnum.Playing:
                    break;

                default:
                    throw new Exception("Can't start playback in state " + state);
            }

            // Set all state arrays back to read-write
            foreach (Type type in snapshotLookup.Keys)
            {
                IStateArrayBase stateArray;
                if (!appStateData.TryGetData(type, out stateArray))
                    throw new Exception("Error when stopping playback, type not found in app state: " + type);

                stateArray.WriteMode = StateArrayWriteModeEnum.Write;
            }

            state = PlaybackStateEnum.Stopped;
        }

        private void LoadLogInternal(string logFilePath)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.None;
                settings.TypeNameHandling = TypeNameHandling.All;
            }

            totalDuration = 0;

            using (StreamReader streamReader = new StreamReader(logFilePath, System.Text.Encoding.UTF8))
            {
                while (!streamReader.EndOfStream)
                {
                    string snapshotJson = streamReader.ReadLine();
                    SyncListSnapshot snapshot = JsonConvert.DeserializeObject<SyncListSnapshot>(snapshotJson);
                    totalDuration = Mathf.Max(totalDuration, snapshot.NetworkTime);
                    snapshots.Add(snapshot);

                    // Get the type for this snapshot
                    Type type;
                    if (!TryGetType(snapshot.StateType, out type))
                        throw new Exception("Couldn't get type from snapshot state type name " + snapshot.StateType);

                    // Convert the snapshot objects from JObject into their actual type
                    for (int i = 0; i < snapshot.ChangedStates.Count; i++)
                    {
                        JObject changedStateJObject = (JObject)snapshot.ChangedStates[i];
                        object changedState = changedStateJObject.ToObject(type);
                        snapshot.ChangedStates[i] = changedState;
                    }

                    // Make sure this type exists in the app state data
                    IStateArrayBase stateArray;
                    if (!appStateData.TryGetData(type, out stateArray))
                        throw new Exception("Playback contains type not contained in app state: " + type);

                    // Sort the snapshots by type so we can access them quickly
                    List<SyncListSnapshot> typeSnapshots = null;
                    if (!snapshotLookup.TryGetValue(type, out typeSnapshots))
                    {
                        typeSnapshots = new List<SyncListSnapshot>();
                        snapshotLookup.Add(type, typeSnapshots);
                        lastSnapshotIndex.Add(type, 0);
                    }
                    typeSnapshots.Add(snapshot);
                }
                streamReader.Close();
            }
        }

        private bool TryGetType(string typeName, out Type type)
        {
            type = null;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return true;
            }

            return false;
        }
    }
}