using Pixie.AppSystems.TimeSync;
using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.AppSystems.Sessions
{
    public class SessionManager : MonoBehaviourSharingApp, ISessionManager, ITimeSource
    {
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
        private sbyte sessionID = 0;

        [SerializeField]
        [Header("Stage prefabs")]
        private ExperienceMode[] experienceModes;
        private ISessionStage[] sessionStages;
        private ISessionStage currentStage;
        private byte stageNum;
        private SessionStateEnum sessionState = SessionStateEnum.Uninitialized;

        private IAppStateReadWrite appState;
        private ExperienceMode experienceMode;
        private IEnumerator runSessionTask;
        private bool paused = false;
        private float sessionTime;
        private float sessionDeltaTime;
        private List<ISessionObject> sessionObjects = new List<ISessionObject>();
        
        public void SetExperienceMode(short experienceModeID)
        {
            if (sessionState != SessionStateEnum.ChoosingExperience)
                throw new InvalidOperationException("Can't select experience mode unless state is " + SessionStateEnum.ChoosingExperience);

            foreach (ExperienceMode availableMode in experienceModes)
            {
                if (availableMode.ID == experienceModeID)
                {
                    experienceMode = availableMode;
                    // If a layout scene is defined, set state to loading layout scene
                    // Otherwise, skip ahead to ready
                    sessionState = string.IsNullOrEmpty(experienceMode.LayoutSceneName) ? SessionStateEnum.ReadyToStart : SessionStateEnum.LoadingLayoutScene;
                    UpdateSessionState();
                    return;
                }
            }

            throw new Exception("Couldn't find game mode ID " + experienceModeID);
        }

        public void SetLayoutSceneLoaded()
        {
            if (sessionState != SessionStateEnum.LoadingLayoutScene)
                throw new InvalidOperationException("Can't set " + SessionStateEnum.ReadyToStart + " unless state is " + SessionStateEnum.LoadingLayoutScene);

            sessionState = SessionStateEnum.ReadyToStart;
            UpdateSessionState();
        }

        public void StartSession()
        {
            if (sessionState != SessionStateEnum.ReadyToStart)
                throw new InvalidOperationException("Can't begin session before session is completed!");

            if (appState == null)
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
                    throw new NullReferenceException("Experience stage prefab had no IExperienceStage script attached.");

                sessionStagesList.Add(experienceStage);
            }

            sessionStages = sessionStagesList.ToArray();
            currentStage = sessionStages[0];
            runSessionTask = RunSessionInternal(appState);

            sessionState = SessionStateEnum.InProgress;
            UpdateSessionState();
        }

        public bool TryUpdateSession()
        {
            if (sessionState != SessionStateEnum.InProgress)
                throw new InvalidOperationException("Can't update session unless state is " + SessionStateEnum.InProgress);

            if (runSessionTask == null)
                throw new Exception("Can't try update session - run task is null!");

            if (paused)
                return false;

            sessionDeltaTime = NetworkTime.DeltaTime;
            sessionTime += sessionDeltaTime;

            SessionState currentState = UpdateSessionState();

            // Let our session objects know we've updated the session
            foreach (ISessionObject sessionObject in sessionObjects)
                sessionObject.OnSessionUpdate(currentState);

            if (!runSessionTask.MoveNext())
            {
                // The session task has ended
                runSessionTask = null;
            }
            return true;
        }

        public void SetPaused(bool paused)
        {
            if (sessionState != SessionStateEnum.InProgress)
                throw new InvalidOperationException("Can't set paused unless state is " + SessionStateEnum.InProgress);

            this.paused = paused;
        }

        public void ForceCompleteStage()
        {
            if (currentStage == null)
                throw new InvalidOperationException("Can't force complete stage - session hasn't started.");

            Debug.Log("Force-completing stage " + currentStage.name);
            currentStage.ForceComplete();
        }

        public void ResetSession()
        {
            if (sessionState != SessionStateEnum.Completed)
                throw new InvalidOperationException("Can't update session unless state is " + SessionStateEnum.Completed);

            sessionState = SessionStateEnum.WaitingForApp;
        }
              
        private IEnumerator RunSessionInternal(IAppStateReadWrite appState)
        {
            // Let our session objects know we're starting a session
            foreach (ISessionObject sessionObject in sessionObjects)
                sessionObject.OnSessionStart();

            stageNum = 0;
            while (stageNum < sessionStages.Length)
            {
                Debug.Log("Running stage...");
                currentStage = sessionStages[stageNum];

                // Let our session objects know we're starting a stage
                foreach (ISessionObject sessionObject in sessionObjects)
                    sessionObject.OnSessionStageBegin();

                IEnumerator runStageTask = currentStage.Run(this, appState);
                while (runStageTask.MoveNext())
                {
                    // Update our session state
                    yield return runStageTask.Current;
                }
                stageNum++;
                yield return null;

                // Let our session objects know we're ending a stage
                foreach (ISessionObject sessionObject in sessionObjects)
                    sessionObject.OnSessionStageBegin();
            }

            sessionState = SessionStateEnum.Completed;
            UpdateSessionState();

            // Let our session objects know we're ending the session
            foreach (ISessionObject sessionObject in sessionObjects)
                sessionObject.OnSessionEnd();

            yield break;
        }

        private SessionState UpdateSessionState()
        {
            SessionState state = appState.GetState<SessionState>(sessionID);
            state.State = sessionState;
            if (currentStage != null)
            {
                state.CurrentStageNum = stageNum;
                state.CurrentStageState = CurrentStage.State;
                state.CurrentStageType = CurrentStage.ProgressionType;
                state.CurrentStageStartTime = CurrentStage.TimeStarted;
            }
            if (experienceMode != null)
            {
                state.LayoutSceneName = experienceMode.LayoutSceneName;
            }

            appState.SetState<SessionState>(state);

            return state;
        }
        
        #region ISharingAppObject implementation

        public override void OnAppInitialize()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                case AppRoleEnum.Host:
                    break;

                default:
                    Debug.LogWarning("This tool should only be run on a server. Disabling.");
                    gameObject.SetActive(false);
                    break;
            }

            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);

            experienceMode = null;
            sessionState = SessionStateEnum.ChoosingExperience;

            // Gather up all session objects in the scene
            // Subscribe to scene load events so we can gather up further objects
            ComponentFinder.FindAllInScenes<ISessionObject>(sessionObjects, ComponentFinder.SearchTypeEnum.Recursive);

            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;

            sessionState = SessionStateEnum.ChoosingExperience;
        }

        private void SceneUnloaded(Scene scene)
        {
            // Remove any dead objects from our session objects
            for (int i = sessionObjects.Count - 1; i >= 0; i--)
            {
                if (sessionObjects[i] == null)
                    sessionObjects.RemoveAt(i);
            }
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (ISessionObject newObject in ComponentFinder.FindAllNewInScene<ISessionObject>(scene, sessionObjects, ComponentFinder.SearchTypeEnum.Recursive))
            {
                switch (sessionState)
                {
                    case SessionStateEnum.InProgress:
                        // If we've already started, tell the new objects we find
                        newObject.OnSessionStart();
                        newObject.OnSessionStageBegin();
                        break;

                    default:
                        // Otherwise, they'll be udpated normally
                        break;
                }
            }
        }
        
        public override void OnAppShutDown()
        {
            SceneManager.sceneUnloaded -= SceneUnloaded;
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        #endregion
    }
}