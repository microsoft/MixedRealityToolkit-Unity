using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    public abstract class SessionStageBase : MonoBehaviour, ISessionStage
    {
        public float MaxTime { get { return maxTime; } }

        public float TimeStarted { get { return timeStarted; } }

        public float TimeElapsed
        {
            get
            {
                switch (state)
                {
                    case StageStateEnum.NotStarted:
                        return 0f;

                    case StageStateEnum.Completed:
                        return timeElapsed;

                    default:
                       return timeSource.Time - timeStarted;
                }
            }
        }

        public abstract AudioClip MusicClip { get; }
        public abstract AudioClip OneShotClip { get; }
        public abstract AudioClip AmbientClip { get; }
        public float MusicVolume { get { return musicVolume; } }
        public float OneShotVolume { get { return oneShotVolume; } }
        public float AmbientVolume { get { return ambientVolume; } }
        public StageProgressionTypeEnum ProgressionType { get { return type; } }
        public StageStateEnum State { get { return state; } }

        [SerializeField]
        protected byte stageNum;
        [SerializeField]
        [Range(0.1f, 1f)]
        private float musicVolume = 1f;
        [SerializeField]
        [Range(0.1f, 1f)]
        private float oneShotVolume = 0.5f;
        [SerializeField]
        [Range(0.1f, 1f)]
        private float ambientVolume = 0.5f;

        [Header("General Settings")]
        [SerializeField]
        private StageProgressionTypeEnum type = StageProgressionTypeEnum.Timed;
        [SerializeField]
        protected StageStateEnum state = StageStateEnum.NotStarted;
        [SerializeField]
        protected float maxTime = 600f;

        private float timeStarted;
        private float timeElapsed;

        private ITimeSource timeSource;

        private bool forceMoveNext = false;

        public void ForceComplete()
        {
            forceMoveNext = true;
        }

        public abstract void OnStageEnter(
            ITimeSource timeSource,
            IAppStateReadWrite appState, 
            IUserView users,
            IStateView stateView);

        public abstract void OnStageExit(
            ITimeSource timeSource,
            IAppStateReadWrite appState,
            IUserView users,
            IStateView stateView);

        public IEnumerator Run(
            ITimeSource timeSource, 
            IAppStateReadWrite appState,
            IUserView users,
            IStateView stateView)
        {
            this.timeSource = timeSource;
            timeStarted = timeSource.Time;
            timeElapsed = 0;
            forceMoveNext = false;

            OnStageEnter(timeSource, appState, users, stateView);

            state = StageStateEnum.TransitionIn;
            yield return null;

            IEnumerator enteringTask = TransitionIn(timeSource, appState, users, stateView);
            while (enteringTask.MoveNext())
            {
                yield return enteringTask.Current;
                if (forceMoveNext)
                {
                    Debug.Log("Breaking from transition in.");
                    forceMoveNext = false;
                    break;
                }
            }

            state = StageStateEnum.Running;
            yield return null;

            IEnumerator runningTask = RunStage(timeSource, appState, users, stateView);
            while (runningTask.MoveNext())
            {
                yield return runningTask.Current;

                // Check our elapsed time
                switch (type)
                {
                    case StageProgressionTypeEnum.Timed:
                        break;

                    default:
                        // This is a timed experience
                        // If time elapsed exceeds max time, move on
                        if (timeElapsed > maxTime)
                        {
                            forceMoveNext = true;
                        }
                        break;
                }

                if (forceMoveNext)
                {
                    Debug.Log("Breaking from running.");
                    forceMoveNext = false;
                    break;
                }
            };
            
            state = StageStateEnum.TransitionOut;
            yield return null;

            IEnumerator exitingTask = TransitionOut(timeSource, appState, users, stateView);
            while (exitingTask.MoveNext())
            {
                yield return exitingTask.Current;
                if (forceMoveNext)
                {
                    Debug.Log("Breaking from transition out.");
                    forceMoveNext = false;
                    break;
                }
            }

            state = StageStateEnum.Completed;

            Debug.Log("On Stage exit in session stage " + name);
            OnStageExit(timeSource, appState, users, stateView);

            // Store the final time elapsed
            timeElapsed = timeSource.Time - timeStarted;

            yield break;
        }

        public abstract IEnumerator TransitionIn(
            ITimeSource timeSource,
            IAppStateReadWrite appState, 
            IUserView users, 
            IStateView stateView);

        public abstract IEnumerator RunStage(
            ITimeSource timeSource,
            IAppStateReadWrite appState,
            IUserView users,
            IStateView stateView);

        public abstract IEnumerator TransitionOut(
            ITimeSource timeSource,
            IAppStateReadWrite appState,
            IUserView users,
            IStateView stateView);
    }
}