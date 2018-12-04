using Pixie.StateControl;
using System.Collections;
using UnityEngine;
using Pixie.AppSystems.TimeSync;

namespace Pixie.AppSystems.Sessions
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

        public StageProgressionTypeEnum ProgressionType { get { return type; } }
        public StageStateEnum State { get { return state; } }

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
        private IAppStateReadWrite appState;

        private bool forceMoveNext = false;

        public void ForceComplete()
        {
            forceMoveNext = true;
        }

        public abstract void OnStageEnter(ITimeSource timeSource, IAppStateReadWrite appState);

        public abstract void OnStageExit(ITimeSource timeSource, IAppStateReadWrite appState);

        public IEnumerator Run(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            this.timeSource = timeSource;
            this.appState = appState;
            // Set our global time source so wait for seconds works as expected
            WaitForSeconds.TimeSource = timeSource;

            timeStarted = timeSource.Time;
            timeElapsed = 0;
            forceMoveNext = false;

            OnStageEnter(timeSource, appState);

            state = StageStateEnum.TransitionIn;
            yield return null;

            IEnumerator enteringTask = TransitionIn(timeSource, appState);
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

            IEnumerator runningTask = RunStage(timeSource, appState);
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
                        if (TimeElapsed > maxTime)
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

            IEnumerator exitingTask = TransitionOut(timeSource, appState);
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
            OnStageExit(timeSource, appState);

            // Store the final time elapsed
            timeElapsed = timeSource.Time - timeStarted;

            yield break;
        }

        public abstract IEnumerator TransitionIn(ITimeSource timeSource, IAppStateReadWrite appState);

        public abstract IEnumerator RunStage(ITimeSource timeSource, IAppStateReadWrite appState);

        public abstract IEnumerator TransitionOut(ITimeSource timeSource, IAppStateReadWrite appState);

        /// <summary>
        /// Internal implementation of WaitForSeconds that uses a static TimeSource
        /// Allows for using 'WaitForSeconds' within SessionStage coroutines
        /// </summary>
        protected struct WaitForSeconds : IEnumerator
        {
            public static ITimeSource TimeSource;

            private float seconds;
            private float timeStarted;

            public WaitForSeconds(float seconds)
            {
                this.seconds = seconds;
                this.timeStarted = TimeSource.Time;
            }

            public object Current { get { return null; } }

            public bool MoveNext()
            {
                return (TimeSource.Time < timeStarted + seconds);
            }

            public void Reset() { }
        }
    }
}