using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    public abstract class StateObject<T> : MonoBehaviour, IStateObject<T>, IStateObjectBase where T : struct, IItemState<T>
    {
        public sbyte ItemNum { get { return itemNum; } }
        public Type StateType { get { return typeof(T); } }
        public UserState UserState
        {
            get
            {
                if (!IsUserType)
                    throw new Exception("Cannot retrieve user state from a non-user type state object.");

                return userState;
            }
        }
        public T CurrentState { get { return newState; } }
        public T PreviousState { get { return oldState; } }
        public IAppStateReadOnly AppState { get { return appState; } }
        public IUserView Users { get { return users; } }
        public IStateView StateView { get { return stateView; } }
        public abstract bool IsUserType { get; }
        public virtual bool IsInitialized { get { return isInitialized; } }
        public virtual bool AutoName { get { return true; } }

        public bool IsLocalUser
        {
            get
            {
                if (!IsUserType)
                    return false;

                if (!isInitialized)
                    return false;

                if (!users.LocalUserSpawned)
                    return false;

                return itemNum == users.LocalUserObject.UserNum;
            }
        }

        [SerializeField]
        private sbyte itemNum = -1;
        private T oldState;
        private T newState;
        private UserState userState;
        private SessionState oldSessionState;
        private SessionState newSessionState;
        private bool isInitialized = false;
        private IAppStateReadWrite appState;
        private IUserView users;
        private IStateView stateView;
        private List<IStateListener<T>> stateListeners = new List<IStateListener<T>>();
        private List<IAppListener<T>> appListeners = new List<IAppListener<T>>();

        private void Awake()
        {
            newState = default(T);
            oldState = newState;
        }
        
        public void Initialize(sbyte itemNum, IAppStateReadWrite appState, IUserView users, IStateView stateView)
        {
            this.itemNum = itemNum;

            this.appState = appState;
            this.users = users;
            this.stateView = stateView;

            newState = appState.GetState<T>(itemNum);
            oldState = newState;

            newSessionState = appState.GetState<SessionState>(newState.Filter);
            oldSessionState = newSessionState;

            if (AutoName)
            {
                gameObject.name = typeof(T).ToString() + " - " + itemNum;
            }

            // Gather up all the state object listeners on our game object
            // (This can include components that extend this class)
            foreach (IStateListener<T> listener in gameObject.GetComponents(typeof(IStateListener<T>)))
            {
                stateListeners.Add(listener);
                listener.OnStateInitialize(this, newState);
            }

            foreach (IAppListener<T> listener in gameObject.GetComponents(typeof(IAppListener<T>)))
            {
                appListeners.Add(listener);
            }

            isInitialized = true;

            OnInitialized();
        }

        public void CheckAppState()
        {
            if (!IsInitialized)
            {
                throw new Exception("Attempting to update a state object before it's been initialized.");
            }

            if (IsUserType)
            {
                userState = appState.GetState<UserState>(ItemNum);

                /*switch (users.GetFillState(itemNum))
                {
                    case UserSlot.FillStateEnum.Filled:
                        gameObject.SetActive(true);
                        break;

                    case UserSlot.FillStateEnum.Empty:
                    case UserSlot.FillStateEnum.Ignore:
                        gameObject.SetActive(false);
                        // EXIT - nothing to do here
                        return;
                }*/
            }

            oldState = newState;
            newState = appState.GetState<T>(itemNum);

            oldSessionState = newSessionState;
            newSessionState = appState.GetState<SessionState>(newState.Filter);

            if (oldState.IsDifferent(newState))
            {
                for (int i = 0; i < stateListeners.Count; i++)
                {
                    stateListeners[i].OnStateChange(this, oldState, newState);
                }
            }

            for (int i = 0; i < appListeners.Count; i++)
            {
                appListeners[i].OnUpdateApp(this);
            }
        }

        public void ChangeState(T changedState)
        {
            if (changedState.IsDifferent(newState))
            {
                newState = changedState;
                appState.SetState<T>(changedState);
            }
        }

        public bool IsSimulatedPlayer(IUserView users)
        {
            if (!IsUserType)
                return false;

            if (!IsInitialized)
                return false;

            IUserObject userObject = null;
            if (users.GetUserObject(itemNum, out userObject))
            {
                return userObject.Simulated;
            }

            return false;
        }

        public void UpdateClientListeners()
        {
            for (int i = 0; i < appListeners.Count; i++)
            {
                appListeners[i].OnUpdateAppClient(this);
            }
        }

        public void UpdateServerListeners()
        {
            for (int i = 0; i < appListeners.Count; i++)
            {
                appListeners[i].OnUpdateAppServer(this);
            }
        }

        protected virtual void OnInitialized() { }
    }
}