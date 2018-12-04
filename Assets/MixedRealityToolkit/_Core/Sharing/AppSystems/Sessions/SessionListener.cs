using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Pixie.AppSystems.Sessions
{
    /// <summary>
    /// Listens for changes in SessionState and calls session object update methods on ISessionObjects
    /// Use on clients when you need to update client-side ISessionObject objects.
    /// </summary>
    public class SessionListener : MonoBehaviourSharingApp
    {
        private List<ISessionObject> sessionObjects = new List<ISessionObject>();
        private SessionState oldState;
        private IAppStateReadOnly appState;
        private bool initialized;
        private HashSet<short> sessionStageNums = new HashSet<short>();

        public override void OnAppInitialize()
        {
            // Get our app state
            ComponentFinder.FindInScenes<IAppStateReadOnly>(out appState, ComponentFinder.FailModeEnum.Exception);
            initialized = true;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            // If we're not initialized yet, just add the objects to our list
            if (!initialized)
            {
                ComponentFinder.FindAllNewInScene<ISessionObject>(scene, sessionObjects, ComponentFinder.SearchTypeEnum.Recursive);
                return;
            }

            // Search for new session objects
            foreach (ISessionObject sessionObject in ComponentFinder.FindAllNewInScene<ISessionObject>(scene, sessionObjects, ComponentFinder.SearchTypeEnum.Recursive))
            {
                // If the session has already begun, call session start as well
                switch (oldState.State)
                {
                    case SessionStateEnum.InProgress:
                        sessionObject.OnSessionStart();
                        sessionObject.OnSessionStageBegin();
                        break;

                    default:
                        break;
                }
            }
        }

        public override void OnAppShutDown()
        {

        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void Update()
        {
            if (!initialized)
                return;

            if (!appState.StateExists<SessionState>(0))
                return;

            SessionState currentState = appState.GetState<SessionState>(0);

            // Have we started / completed a session?
            if (currentState.State != oldState.State)
            {
                switch (currentState.State)
                {
                    case SessionStateEnum.InProgress:
                        // Clear our started stage nums
                        sessionStageNums.Clear();
                        // If the new state is InProgress, session has started
                        foreach (ISessionObject sessionObject in sessionObjects)
                            sessionObject.OnSessionStart();
                        break;

                    case SessionStateEnum.Completed:
                        // If the new state is Completed, session has ended
                        foreach (ISessionObject sessionObject in sessionObjects)
                            sessionObject.OnSessionEnd();
                        break;

                    case SessionStateEnum.LoadingLayoutScene:
                        break;
                }
            }

            // Have we started a new stage?
            if (currentState.CurrentStageNum != oldState.CurrentStageNum)
            {
                if (sessionStageNums.Contains(oldState.CurrentStageNum))
                {
                    // If we have started the previous stage, call on stage end
                    foreach (ISessionObject sessionObject in sessionObjects)
                        sessionObject.OnSessionEnd();
                }

                if (!sessionStageNums.Add(currentState.CurrentStageNum))
                {
                    // If we haven't started this session stage yet, call on stage begin
                    foreach (ISessionObject sessionObject in sessionObjects)
                        sessionObject.OnSessionStageBegin();
                }
            }

            // Do we need to call session update?
            switch (currentState.State)
            {
                case SessionStateEnum.InProgress:
                    foreach (ISessionObject sessionObject in sessionObjects)
                        sessionObject.OnSessionUpdate(currentState);
                    break;

                default:
                    break;
            }

            oldState = currentState;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}