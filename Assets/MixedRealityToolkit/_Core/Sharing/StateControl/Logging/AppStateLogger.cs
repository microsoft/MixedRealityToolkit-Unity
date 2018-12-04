using Pixie.AppSystems.TimeSync;
using Pixie.Core;
using Pixie.Initialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Pixie.StateControl.Logging
{
    public class AppStateLogger : MonoBehaviourSharingApp, IAppStateLogger, ISharingAppObject
    {
        public int NumLoggedStates { get { return numLoggedStates; } }
        public int NumQueuedSnapshots { get { return snapshots.Count; } }
        public LogStateEnum State { get { return state; } }

        [SerializeField]
        private LogStateEnum state = LogStateEnum.Stopped;

        private IAppStateData appStateData;
        private List<SyncListSnapshot> snapshots = new List<SyncListSnapshot>();
        private JsonSerializerSettings settings;
        private int numLoggedStates;
        private float timeLoggingStart;
        private string logFilePath;

        public void StartLogging(string logFilePath)
        {
            if (string.IsNullOrEmpty(logFilePath))
                throw new Exception("Path cannot be empty.");

            switch (state)
            {
                case LogStateEnum.Stopped:
                    break;

                default:
                    throw new Exception("Can't start log in state " + state);
            }

            if (appStateData == null)
                ComponentFinder.FindInScenes<IAppStateData>(out appStateData, ComponentFinder.FailModeEnum.Exception);

            this.logFilePath = logFilePath;
            timeLoggingStart = NetworkTime.Time;

            // Get an initial snapshot for all state arrays
            // (If we paused logging, things may have changed in the meantime - we have to start with a reliable keyframe)
            foreach (IStateArrayBase stateArray in appStateData)
            {
                SyncListSnapshot snapshot = new SyncListSnapshot();
                snapshot.NetworkTime = 0;
                snapshot.StateType = stateArray.StateType.FullName;
                snapshot.ChangedStates = new List<object>(stateArray.GetStates());

                numLoggedStates += snapshot.ChangedStates.Count;

                snapshots.Add(snapshot);
            }

            // Subscribe to changes
            appStateData.OnReceiveChangedStates += OnReceiveChangedStates;

            state = LogStateEnum.Logging;
        }

        public void StopLogging()
        {
            switch (state)
            {
                case LogStateEnum.Logging:
                    break;

                default:
                    throw new Exception("Can't stop log in state " + state);
            }

            if (appStateData != null)
                appStateData.OnReceiveChangedStates -= OnReceiveChangedStates;

            // Write log to disk
            FlushLog();
        }

        private void FlushLog()
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.None;
                settings.TypeNameHandling = TypeNameHandling.All;
            }

            using (StreamWriter streamWriter = new StreamWriter(logFilePath, false, System.Text.Encoding.UTF8))
            {
                foreach (SyncListSnapshot snapshot in snapshots)
                {
                    string snapshotJson = JsonConvert.SerializeObject(snapshot, settings);
                    streamWriter.WriteLine(snapshotJson);
                }

                streamWriter.Close();
            }

            snapshots.Clear();
            state = LogStateEnum.Stopped;
        }

        private void OnReceiveChangedStates(Type type, List<object> changedStates)
        {
            SyncListSnapshot snapshot = new SyncListSnapshot();
            snapshot.NetworkTime = NetworkTime.Time - timeLoggingStart;
            snapshot.StateType = type.FullName;
            snapshot.ChangedStates = changedStates;

            numLoggedStates += snapshot.ChangedStates.Count;

            snapshots.Add(snapshot);
        }

        public override void OnAppInitialize()
        {
            state = LogStateEnum.Stopped;
        }
    }
}