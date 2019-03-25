using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl
{
    [MixedRealityExtensionService(SupportedPlatforms.WindowsUniversal | SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone)]
    public class AppState : BaseExtensionService, IAppState, IAppStateReadOnly, IAppStateReadWrite
    {
        public AppState(IMixedRealityServiceRegistrar registrar, string name, uint priority, BaseMixedRealityProfile profile) : base(registrar, name, priority, profile)
        {
            appStateProfile = profile as AppStateProfile;
            this.Name = name;
        }

        public override void Initialize()
        {
            dataSource = (IAppStateData)GameObject.FindObjectOfType(appStateProfile.DataSourceType);
            if (dataSource == null)
            {
                Debug.Log("Couldn't find component of type " + appStateProfile.DataSourceType.Name + " in scene - creating object now.");
                GameObject dataSourceObject = new GameObject(Name + " DataSource");
                try
                {
                    dataSource = (IAppStateData)dataSourceObject.AddComponent(appStateProfile.DataSourceType);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception when adding component of type " + appStateProfile.DataSourceType.Name + " to dataSourceObject");
                    Debug.LogException(e);
                }
            }

            if (dataSource == null)
            {
                throw new Exception("Can't proceed without a data source of type " + typeof(IAppStateData).Name + " - defined type " + appStateProfile.DataSourceType + " yielded no results.");
            }

            // Tell our data source to fire itself up
            dataSource.OnServiceInitialized(appStateProfile, new List<ISharingAppObject>() { this });

            // Create our state arrays
            foreach (Type itemStateType in appStateProfile.SynchronizedTypes)
                dataSource.CreateStateArray(itemStateType);

            Initialized = true;
        }

        public AppRoleEnum AppRole { get { return appStateProfile.AppRole; } }

        public bool Initialized { get; protected set; }

        public bool Synchronized
        {
            get
            {
                // We haven't been initialized yet
                if (dataSource == null)
                    return false;

                return dataSource.Synchronized;
            }
        }

        public bool ReadyToSynchronize
        {
            get { return Initialized && readyToSynchronize; }
            set { readyToSynchronize = value; }
        }

        public IEnumerable<Type> ItemStateTypes
        {
            get
            {
                if (!Initialized)
                    yield break;

                foreach (Type stateType in appStateProfile.SynchronizedTypes)
                    yield return stateType;
            }
        }

        // Source of our sync lists (implementation will be network back end specific)
        IAppStateData dataSource;
        // Component used for setting subscription preferences
        IAppStateDataSubscriptions dataSubscriptions;
        // Used for invoking generic set state methods
        private object[] methodInvokeArgs = new object[1];
        // Methods used to call set state with generic state objects
        private Dictionary<Type, MethodInfo> setStateMethodLookup = new Dictionary<Type, MethodInfo>();
        // Copy of app state profile
        private AppStateProfile appStateProfile;

        private bool readyToSynchronize = false;

        public void Flush()
        {
            foreach (IStateArrayBase stateArray in dataSource)
                stateArray.Flush();
        }

        public void Flush<T>(IEnumerable<short> keys) where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArrayBase stateArray = GetStateArray(typeof(T));
            stateArray.Flush(keys);
        }

        public void Flush<T>(short key) where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArrayBase stateArray = GetStateArray(typeof(T));
            stateArray.Flush(key);
        }

        public void Flush<T>() where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArrayBase stateArray = GetStateArray(typeof(T));
            stateArray.Flush();
        }

        public void SetState<T>(T state) where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArrayBase stateArray = GetStateArray(typeof(T));
            (stateArray as IStateArray<T>)[state.Key] = state;
        }

        public T GetState<T>(short key) where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArrayBase stateArray = GetStateArray(typeof(T));
            return (stateArray as IStateArray<T>)[key];
        }

        public bool IsEmpty<T>() where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArrayBase stateArray;
            if (!dataSource.TryGetData(typeof(T), out stateArray))
                return true;

            return stateArray.IsEmpty;
        }

        public bool StateExists<T>(short key)
        {
            IStateArrayBase stateArray;
            if (!dataSource.TryGetData(typeof(T), out stateArray))
                return false;

            return stateArray.KeyExists(key);
        }

        public IEnumerable<T> GetStates<T>() where T : struct, IItemState, IItemStateComparer<T>
        {
            return GetStateArray(typeof(T)) as IStateArray<T>;
        }

        public IEnumerable<object> GetStates(Type type)
        {
            return GetStateArray(type).GetStates();
        }

        public int GetNumStates(Type type)
        {
            return GetStateArray(type).Count;
        }

        public int GetNumStates<T>() where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArrayBase stateArray = GetStateArray(typeof(T));
            return stateArray.Count;
        }

        public void AddState<T>(T state) where T : struct, IItemState, IItemStateComparer<T>
        {
            IStateArray<T> stateArray = GetStateArray(typeof(T)) as IStateArray<T>;
            stateArray.AddState(state);
        }

        public short AddStateOfType(Type type, short key = -1)
        {
            IStateArrayBase stateArray = GetStateArray(type);

            if (key < 0)
                key = stateArray.GetNextAvailableKey();

            if (key < 0)
                throw new Exception("Cant' get next available key from state array!");

            IItemState newState = (IItemState)Activator.CreateInstance(type, new object[] { key });

            stateArray.AddState(newState);

            return key;
        }

        public void OnAppConnect()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                    // Flush our generated required states
                    Flush();
                    break;

                default:
                    break;
            }
        }

        public void OnAppSynchronize() { }

        private IStateArrayBase GetStateArray(Type type)
        {
            IStateArrayBase stateArray;
            if (!dataSource.TryGetData(type, out stateArray))
                throw new Exception("No state array of type " + type.Name + " found!");

            return stateArray;
        }
    }
}