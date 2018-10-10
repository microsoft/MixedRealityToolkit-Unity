using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    public class SessionManager : MixedRealityManager, IMixedRealityManager, ISessionManager, ITimeSource
    {
        public string Name { get { return "SessionManager"; } }

        public uint Priority { get { return 0; } }

        public AppRoleEnum AppRole { get; set; }

        public SessionStateEnum State { get { return sessionState; } }

        public IEnumerable<IExperienceMode> AvailableModes { get { return experienceModes; } }

        public byte StageNum { get { return stageNum; } }

        public IExperienceMode ExperienceMode { get { return experienceMode; } }

        public ISessionStageBase CurrentStage
        {
            get
            {
                if (currentStage == null)
                {
                    throw new NullReferenceException("Session not initialized.");
                }

                return currentStage;
            }
        }

        public bool Paused { get { return paused; } }

        float ITimeSource.Time { get { return sessionTime; } }

        float ITimeSource.DeltaTime { get { return sessionDeltaTime; } }

        [SerializeField]
        [Header("Temporary field - session num will be determined dynamically")]
        private sbyte sessionNum = 0;

        [SerializeField]
        [Header("Stage prefabs")]
        private ExperienceMode[] experienceModes;
        private ISessionStage[] sessionStages;
        private ISessionStage currentStage;
        private byte stageNum;
        private SessionStateEnum sessionState = SessionStateEnum.WaitingForUsers;

        private IAppStateReadWrite appState;
        private IUserManager users;
        private IStateView stateView;
        private ExperienceMode experienceMode;
        private IEnumerator runSessionTask;
        private bool paused = false;
        private float sessionTime;
        private float sessionDeltaTime;

        #region stage progression calls

        public void SetExperienceMode(short experienceModeID)
        {
            if (State != SessionStateEnum.ChoosingExperience)
                throw new InvalidOperationException("Can't select experience mode unless state is " + SessionStateEnum.ChoosingExperience);

            foreach (ExperienceMode availableMode in experienceModes)
            {
                if (availableMode.ID == experienceModeID)
                {
                    experienceMode = availableMode;
                    sessionState = SessionStateEnum.LoadingLayoutScene;
                    UpdateSessionState();
                    return;
                }
            }

            throw new Exception("Couldn't find game mode ID " + experienceModeID);
        }

        public void SetLayoutSceneLoaded()
        {
            if (sessionState != SessionStateEnum.LoadingLayoutScene)
                throw new InvalidOperationException("Can't set " + SessionStateEnum.WaitingForUsers + " unless state is " + SessionStateEnum.LoadingLayoutScene);

            sessionState = SessionStateEnum.WaitingForUsers;
            UpdateSessionState();
        }

        public void SetUsersReadyToStart()
        {
            if (sessionState != SessionStateEnum.WaitingForUsers)
                throw new InvalidOperationException("Can't set " + SessionStateEnum.UsersReadyToStart + " unless state is " + SessionStateEnum.WaitingForUsers);
            
            sessionState = SessionStateEnum.UsersReadyToStart;
            UpdateSessionState();
        }

        #endregion

        public bool TryUpdateSession()
        {
            if (runSessionTask == null)
                throw new Exception("Can't try update session - run task is null!");

            if (paused)
                return false;

            sessionDeltaTime = NetworkTime.DeltaTime;
            sessionTime += sessionDeltaTime;

            if (!runSessionTask.MoveNext())
            {
                // The session task has ended
                runSessionTask = null;
            }
            return true;
        }

        public void ForceCompleteStage()
        {
            if (currentStage == null)
            {
                throw new InvalidOperationException("Can't force complete stage - session hasn't started.");
            }

            Debug.Log("Force-completing stage " + currentStage.name);
            currentStage.ForceComplete();
        }

        public void StartSession()
        {
            if (sessionState != SessionStateEnum.UsersReadyToStart)
                throw new InvalidOperationException("Can't begin session before session is completed!");

            if (appState == null || users == null || stateView == null)
                throw new NullReferenceException("Can't begin session until data sources are set!");

            if (experienceMode == null)
                throw new Exception("Can't begin session until game mode is set!");

            // If session stages exist, destroy them now
            if (sessionStages != null)
            {
                Debug.Log("Destroying existing session stages");
                foreach (ISessionStage stage in sessionStages)
                {
                    GameObject.Destroy(stage.gameObject);
                }
                sessionStages = null;
            }

            Debug.Log("Creating new session stages from game mode " + experienceMode.Name);
            // create objects for each of our experience stages
            List<ISessionStage> sessionStagesList = new List<ISessionStage>();
            foreach (GameObject sessionStagePrefab in experienceMode.SessionStagePrefabs)
            {
                GameObject sessionStageObject = GameObject.Instantiate(sessionStagePrefab, transform);
                sessionStageObject.name = sessionStagePrefab.name;
                ISessionStage experienceStage = sessionStageObject.GetComponent(typeof(ISessionStage)) as ISessionStage;

                if (experienceStage == null)
                {
                    throw new NullReferenceException("Experience stage prefab had no IExperienceStage script attached.");
                }

                sessionStagesList.Add(experienceStage);
            }

            sessionStages = sessionStagesList.ToArray();
            currentStage = sessionStages[0];
            runSessionTask = RunSessionInternal(appState, users, stateView);

            sessionState = SessionStateEnum.InProgress;
            UpdateSessionState();
        }

        public void ResetSession()
        {
            sessionState = SessionStateEnum.WaitingForUsers;
        }

        public void SetPaused(bool paused)
        {
            this.paused = paused;
        }
               
        private IEnumerator RunSessionInternal(IAppStateReadWrite appState, IUserManager users, IStateView stateView)
        {
            UpdateSessionState();

            int stageNum = 0;
            while (stageNum < sessionStages.Length)
            {
                Debug.Log("Running stage...");
                currentStage = sessionStages[stageNum];

                IEnumerator runStageTask = currentStage.Run(this, appState, users, stateView);
                while (runStageTask.MoveNext())
                {
                    // Update our session state
                    UpdateSessionState();
                    yield return runStageTask.Current;
                }
                stageNum++;
                yield return null;
            }

            sessionState = SessionStateEnum.Completed;
            UpdateSessionState();

            yield break;
        }

        private void UpdateSessionState()
        {
            SessionState state = appState.GetState<SessionState>(sessionNum);
            state.State = sessionState;
            if (currentStage != null)
            {
                state.CurrentStageNum = stageNum;
                state.CurrentStageType = CurrentStage.ProgressionType;
                state.CurrentStageStartTime = CurrentStage.TimeStarted;
            }
            if (experienceMode != null)
            {
                state.LayoutSceneName = experienceMode.LayoutSceneName;
            }

            appState.SetState<SessionState>(state);
        }
        
        #region ISharingAppObject implementation

        public void OnSharingStart()
        {
            SceneScraper.FindInScenes<IAppStateReadWrite>(out appState);
            SceneScraper.FindInScenes<IUserManager>(out users);

            experienceMode = null;
            sessionState = SessionStateEnum.ChoosingExperience;
        }

        public void OnStateInitialized()
        {
            SceneScraper.FindInScenes<IStateView>(out stateView);
        }

        public void OnSharingStop() { }

        #endregion

        public void Initialize() { }

        public void Reset() { }

        public void Enable() { }

        public void Update() { }

        public void Disable() { }

        public void Destroy() { }
    }
}