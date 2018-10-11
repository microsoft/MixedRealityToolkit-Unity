using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    public class AppState : MixedRealityManager, IMixedRealityManager, IAppStateReadOnly, IAppStateReadWrite, IAppStateClient, IAppStateServer, ISharingAppObject
    {
        public string Name { get { return "AppState"; } }

        public uint Priority { get { return 0; } }

        public AppRoleEnum AppRole { get; set; }

        public bool Synchronized
        {
            get
            {
                foreach (IAppStateSource source in stateSources)
                {
                    if (!source.Synchronized)
                        return false;
                }
                return true;
            }
            set
            {
                foreach (IAppStateSource source in stateSources)
                {
                    source.Synchronized = value;
                }
            }
        }

        public IEnumerable<Type> ItemStateTypes { get { return stateLookup.Keys; } }

        public virtual IEnumerable<Type> RequiredStateTypes { get { yield break; } }

        // List of all state arrays being used
        private List<IStateArrayBase> stateList = new List<IStateArrayBase>();
        // Lookup of state array by type
        private Dictionary<Type, IStateArrayBase> stateLookup = new Dictionary<Type, IStateArrayBase>();
        // State sources contributing to our state arrray list
        private List<IAppStateSource> stateSources = new List<IAppStateSource>();
        // Lookup of generic methods we've used to create our state arrays from IAppStateSources
        private Dictionary<Type, MethodInfo> createStateArrayMethodLookup = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> checkTypeMethodLookup = new Dictionary<Type, MethodInfo>();
        // Client-side list of changed states
        private Dictionary<Type, Dictionary<sbyte, object>> unflushedStateLookup = new Dictionary<Type, Dictionary<sbyte, object>>();

        // Pipe used by client to send states to server
        IStatePipeInput statePipeInput;
        // Pipes receiving states from clients
        IStatePipeOutputSource statePipeOutputSource;
        // Used for invoking generic set state methods
        private object[] methodInvokeArgs = new object[1];
        // Methods used to call set state with generic state objects
        private Dictionary<Type, MethodInfo> setStateMethodLookup = new Dictionary<Type, MethodInfo>();

        public void AssignStatePipeInput(IStatePipeInput statePipeInput)
        {
            this.statePipeInput = statePipeInput;
        }

        public void AssignStateOutputSource(IStatePipeOutputSource statePipeOutputSource)
        {
            this.statePipeOutputSource = statePipeOutputSource;
        }

        public void Flush()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                    FlushOutputPipes();
                    // Servers just flush the state arrays, which pushes data to client
                    foreach (IStateArrayBase stateArray in stateList)
                    {
                        stateArray.Flush();
                    }
                    break;

                case AppRoleEnum.Client:
                    FlushInputPipe();
                    break;

                case AppRoleEnum.Host:
                    FlushOutputPipes();
                    // Servers just flush the state arrays, which pushes data to client
                    foreach (IStateArrayBase stateArray in stateList)
                    {
                        stateArray.Flush();
                    }
                    FlushInputPipe();
                    break;
            }
        }

        public void Flush<T>() where T : struct, IItemState<T>
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                case AppRoleEnum.Host:
                    (stateLookup[typeof(T)] as StateArrayServer<T>).Flush();
                    break;

                case AppRoleEnum.Client:
                    FlushInputPipe<T>();
                    break;
            }
        }

        public void SetState<T>(T state) where T : struct, IItemState<T>
        {
            Type type = typeof(T);
            IStateArrayBase stateArray;
            if (!stateLookup.TryGetValue(type, out stateArray))
                throw new Exception("No state array found for state object type " + type.Name);

            (stateArray as IStateArray<T>)[state.Key] = state;
        }

        public T GetState<T>(sbyte stateKey) where T : struct, IItemState<T>
        {
            Type type = typeof(T);
            IStateArrayBase stateArray;
            if (!stateLookup.TryGetValue(type, out stateArray))
                throw new Exception("No state array found for state object type " + type.Name);

            return (stateArray as IStateArray<T>)[stateKey];
        }

        public bool IsEmpty<T>() where T : struct, IItemState<T>
        {
            IStateArrayBase stateArray = stateLookup[typeof(T)] as IStateArrayBase;

            if (stateArray == null)
                throw new Exception("No state array found for state object type " + typeof(T).Name);

            return stateArray.IsEmpty;
        }

        public bool StateExists<T>(sbyte stateKey)
        {
            Type type = typeof(T);
            IStateArrayBase stateArray;
            if (!stateLookup.TryGetValue(type, out stateArray))
                return false;

            return stateArray.KeyExists(stateKey);
        }

        public IEnumerable<T> GetStates<T>() where T : struct, IItemState<T>
        {
            Type stateType = typeof(T);
            if (!stateLookup.ContainsKey(stateType))
                throw new Exception("No state array found for state object type " + stateType.Name);

            return stateLookup[typeof(T)] as IStateArray<T>;
        }

        public int GetNumStates<T>() where T : struct, IItemState<T>
        {
            Type stateType = typeof(T);
            if (!stateLookup.ContainsKey(stateType))
                throw new Exception("No state array found for state object type " + stateType.Name);

            return stateLookup[stateType].Count;
        }

        public void AddState<T>(T state) where T : struct, IItemState<T>
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This method should only be called on server.");

                default:
                    break;
            }

            StateArrayServer<T> stateArray = stateLookup[typeof(T)] as StateArrayServer<T>;
            stateArray.AddState(state);
        }

        public sbyte AddStateOfType(Type type, sbyte sessionNum, sbyte stateKey = -1)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This method should only be called on server.");

                default:
                    break;
            }

            IStateArrayBase stateArray;
            if (!stateLookup.TryGetValue(type, out stateArray))
                throw new Exception("Can't add state of type " + type.Name + " - no state array exists!");

            if (stateKey < 0)
                stateKey = stateArray.GetNextAvailableKey();

            if (stateKey < 0)
                throw new Exception("Cant' get next available key from state array!");

            object newState = Activator.CreateInstance(type, new object[] { sessionNum, stateKey });

            //Debug.Log("Created new state: " + newState.ToString());

            MethodInfo methodInfo = GetType().GetMethod("AddState").MakeGenericMethod(type);
            methodInfo.Invoke(this, new object[] { newState });

            return stateKey;
        }

        public void CreateStateArray<T>(SyncListStruct<T> syncList) where T : struct, IItemState<T>
        {
            IStateArray<T> stateArray = null;
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                case AppRoleEnum.Host:
                    stateArray = new StateArrayServer<T>();
                    break;

                case AppRoleEnum.Client:
                    stateArray = new StateArrayClient<T>();
                    // On the client we want to listen for external changes made
                    stateArray.OnStateChangedExternal += OnStateChangedExternal;
                    break;
            }
            stateArray.Create(syncList);

            stateList.Add(stateArray);
            stateLookup.Add(typeof(T), stateArray);

            // Create a list of unflushed states for the client
            unflushedStateLookup.Add(typeof(T), new Dictionary<sbyte, object>());
        }

        public bool IsFieldSyncListType<T>(Type fieldType) where T : struct, IItemState<T>
        {
            return fieldType.IsSubclassOf(typeof(SyncListStruct<T>));
        }

        private void OnStateChangedExternal(Type stateType, sbyte stateKey)
        {
            // Get the newly changed state from the state array
            IStateArrayBase stateArray = stateLookup[stateType];
            Dictionary<sbyte, object> unflushedStates = unflushedStateLookup[stateType];
            object newState = stateArray.GetState(stateKey);
            // If this value already exists in our unflushed states, overwrite the previous state - only the latest matters
            if (!unflushedStates.ContainsKey(stateKey))
            {
                unflushedStates.Add(stateKey, newState);
            }
            else
            {
                unflushedStates[stateKey] = newState;
            }
        }

        private void FlushInputPipe()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                    throw new Exception("This method should only be called on client or host.");

                default:
                    break;
            }

            if (statePipeInput == null)
                throw new Exception("Can't flush app state on client unless " + typeof(IStatePipeInput).Name + " is assigned!");

            // If clients have a state pipe, they push recently changed states to the server
            foreach (KeyValuePair<Type, Dictionary<sbyte, object>> unflushedStates in unflushedStateLookup)
            {
                if (unflushedStates.Value.Count > 0)
                {
                    statePipeInput.SendStates(unflushedStates.Value.Values);
                    unflushedStates.Value.Clear();
                }
            }
        }

        private void FlushInputPipe<T>() where T : struct, IItemState<T>
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                    throw new Exception("This method should only be called on a client or host.");

                default:
                    break;
            }

            if (statePipeInput == null)
                throw new Exception("Can't flush app state on client unless " + typeof(IStatePipeInput).Name + " is assigned!");

            Dictionary<sbyte, object> unflushedStates = unflushedStateLookup[typeof(T)];
            if (unflushedStates.Count > 0)
            {
                statePipeInput.SendStates(unflushedStates.Values);
                unflushedStates.Clear();
            }
        }

        private void FlushOutputPipes()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This method should only be called on server.");

                default:
                    break;
            }

            if (statePipeOutputSource == null)
                throw new Exception("Can't flush output pipes unless " + typeof(IStatePipeOutputSource).Name + " is assigned!");

            foreach (IStatePipeOutput output in statePipeOutputSource.StatePipeOutputs)
            {
                while (output.StatesReceived.Count > 0)
                {
                    object state = output.StatesReceived.Dequeue();
                    Type stateType = state.GetType();
                    MethodInfo method;
                    if (!setStateMethodLookup.TryGetValue(stateType, out method))
                    {
                        method = GetType().GetMethod("SetState").MakeGenericMethod(new Type[] { stateType });
                        setStateMethodLookup.Add(stateType, method);
                    }
                    methodInvokeArgs[0] = state;
                    method.Invoke(this, methodInvokeArgs);
                }
            }
        }

        public void Initialize()
        {
            stateSources.Clear();

            HashSet<Type> requiredStateTypes = new HashSet<Type>(RequiredStateTypes);

            // Gather all state sources contributing to this app state
            Component[] components = gameObject.GetComponentsInChildren(typeof(IAppStateSource), true);

            // Create the rest of our state arrays with reflection
            foreach (IAppStateSource stateSource in components)
            {
                stateSources.Add(stateSource);

                Type stateSourceType = stateSource.GetType();

                HashSet<Type> stateTypes = new HashSet<Type>(stateSource.StateTypes);
                // Go through each public field in state source
                foreach (FieldInfo field in stateSourceType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    Type stateType = null;

                    foreach (Type availableStateType in stateTypes)
                    {
                        MethodInfo checkTypeMethod;
                        if (!checkTypeMethodLookup.TryGetValue(availableStateType, out checkTypeMethod))
                        {
                            // Create a new generic method using this type and store it for later
                            checkTypeMethod = GetType().GetMethod("IsFieldSyncListType");
                            checkTypeMethod = checkTypeMethod.MakeGenericMethod(availableStateType);
                            checkTypeMethodLookup.Add(availableStateType, checkTypeMethod);
                        }
                        bool isStateArray = (bool)checkTypeMethod.Invoke(this, new object[] { field.FieldType });

                        if (isStateArray)
                        {
                            // Store this so it gets removed from the hash set
                            stateType = availableStateType;

                            // Get the sync list and create a state array with it
                            object syncList = field.GetValue(stateSource);
                            MethodInfo createStateArrayMethod;
                            if (!createStateArrayMethodLookup.TryGetValue(availableStateType, out createStateArrayMethod))
                            {
                                // Create a new generic method using this type and store it for later
                                createStateArrayMethod = GetType().GetMethod("CreateStateArray");
                                createStateArrayMethod = createStateArrayMethod.MakeGenericMethod(availableStateType);
                                createStateArrayMethodLookup.Add(availableStateType, createStateArrayMethod);
                            }
                            createStateArrayMethod.Invoke(this, new object[] { syncList });
                            break;
                        }
                    }

                    stateTypes.Remove(stateType);
                    requiredStateTypes.Remove(stateType);
                }

                if (stateTypes.Count > 0)
                {
                    string types = string.Empty;
                    foreach (Type type in stateTypes)
                    {
                        types += type.Name + " ";
                    }
                    throw new Exception("Types were included in state source ItemStateTypes which were not found in a public field: " + types);
                }
            }

            if (requiredStateTypes.Count > 0)
            {
                string types = string.Empty;
                foreach (Type type in requiredStateTypes)
                {
                    types += type.Name + " ";
                }
                throw new Exception("Couldn't find the following required state types: " + types);
            }

            switch (AppRole)
            {
                case AppRoleEnum.Host:
                case AppRoleEnum.Server:
                    // Now that we've gathered all our states,
                    // ask each state source to generate any states it needs to work correctly
                    foreach (IAppStateSource source in stateSources)
                    {
                        source.GenerateRequiredStates(this);
                    }
                    break;

                default:
                    // Don't generate states on the client - server will propigate them
                    break;
            }
        }

        public void Reset() { }

        public void Enable() { }

        public void Update() { }

        public void Disable() { }

        public void Destroy() { }

        // TODO determine if these are still alongside MRTK
        public void OnSharingStart() { }

        public void OnStateInitialized() { }

        public void OnSharingStop() { }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(AppState))]
        public class AppStateEditor : UnityEditor.Editor
        {
            public static HashSet<string> visibleTypes = new HashSet<string>();

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                AppState appState = (AppState)target;

                GUI.color = Color.gray;
                UnityEditor.EditorGUILayout.BeginVertical();
                UnityEditor.EditorGUILayout.LabelField("State Sources: " + appState.stateSources.Count);
                foreach (IAppStateSource stateSource in appState.stateSources)
                {
                    GUI.color = Color.white;
                    UnityEditor.EditorGUILayout.LabelField(stateSource.GetType().Name, UnityEditor.EditorStyles.boldLabel);
                    foreach (Type type in stateSource.StateTypes)
                    {
                        UnityEditor.EditorGUI.indentLevel++;
                        bool showType = UnityEditor.EditorGUILayout.Foldout(visibleTypes.Contains(type.Name), type.Name);
                        if (showType)
                        {
                            visibleTypes.Add(type.Name);
                            UnityEditor.EditorGUILayout.BeginVertical();
                            IStateArrayBase stateArray = appState.stateLookup[type];
                            foreach (object state in stateArray.GetStates())
                            {
                                UnityEditor.EditorGUILayout.TextArea(state.ToString());
                            }
                            UnityEditor.EditorGUILayout.Space();
                            UnityEditor.EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            visibleTypes.Remove(type.Name);
                        }
                        UnityEditor.EditorGUI.indentLevel--;
                    }
                }
                UnityEditor.EditorGUILayout.EndVertical();

                // Force this to update to reflect most recent states
                Repaint();
            }
        }
#endif
    }
}