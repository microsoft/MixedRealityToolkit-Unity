﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Pixie.Core;
using UnityEngine;

namespace Pixie.StateControl
{
    public class AppState : MonoBehaviourSharingApp, IAppStateReadOnly, IAppStateReadWrite
    {        
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
            set
            {
                if (dataSource == null)
                    throw new Exception("Trying to set synchronized before app state is initialized!");

                switch (AppRole)
                {
                    case AppRoleEnum.Client:
                        throw new Exception("Client should not modify this property.");
                    default:
                        break;
                }

                dataSource.Synchronized = value;
            }
        }

        public IEnumerable<Type> ItemStateTypes { get { return itemStateTypes; } }
        
        // State sources contributing to our state arrray list
        private List<IAppStateSource> stateSources = new List<IAppStateSource>();
        private HashSet<Type> itemStateTypes = new HashSet<Type>();
        // Source of our sync lists (implementation will be network back end specific)
        IAppStateData dataSource;
        // Used for invoking generic set state methods
        private object[] methodInvokeArgs = new object[1];
        // Methods used to call set state with generic state objects
        private Dictionary<Type, MethodInfo> setStateMethodLookup = new Dictionary<Type, MethodInfo>();

        public void Flush()
        {
            foreach (IStateArrayBase stateArray in dataSource)
                stateArray.Flush();
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

        public override void OnAppInitialize()
        {
            dataSource = (IAppStateData)gameObject.GetComponent(typeof(IAppStateData));
            if (dataSource == null)
                throw new Exception("Can't proceed without a data source of type " + typeof(IAppStateData).Name);

            stateSources.Clear();

            // Gather all state sources contributing to this app state
            Component[] components = gameObject.GetComponentsInChildren(typeof(IAppStateSource), true);

            // Create the rest of our state arrays with reflection
            foreach (IAppStateSource stateSource in components)
            {
                stateSources.Add(stateSource);

                foreach (Type stateType in stateSource.StateTypes)
                {
                    itemStateTypes.Add(stateType);
                    dataSource.CreateStateArray(stateType, AppRole);
                }
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

            Initialized = true;
        }

        public override void OnAppConnect()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Host:
                case AppRoleEnum.Server:
                    // Flush our generated required states
                    Flush();
                    break;

                default:
                    break;
            }
        }

        private IStateArrayBase GetStateArray(Type type)
        {
            IStateArrayBase stateArray;
            if (!dataSource.TryGetData(type, out stateArray))
                throw new Exception("No state array of type " + type.Name + " found!");

            return stateArray;
        }

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
                            IStateArrayBase stateArray;
                            if (appState.dataSource.TryGetData(type, out stateArray))
                            {
                                foreach (object state in stateArray.GetStates())
                                {
                                    UnityEditor.EditorGUILayout.TextArea(state.ToString());
                                }
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