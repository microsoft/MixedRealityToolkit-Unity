using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    public class StateObjects : MixedRealityManager, IMixedRealityManager, IStateView
    {
        public AppRoleEnum AppRole { get; set; }

        public string Name { get { return "StateObjects"; } }

        public uint Priority { get { return 0; } }

        [SerializeField]
        protected GameObject[] stateViewPrefabs;

        private Dictionary<Type, GameObject> stateViewPrefabLookup = new Dictionary<Type, GameObject>();
        private Dictionary<Type, MethodInfo> checkObjectMethodLookup = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, Dictionary<sbyte, IStateObjectBase>> stateViewLookups = new Dictionary<Type, Dictionary<sbyte, IStateObjectBase>>();
        private Dictionary<Type, Dictionary<sbyte, GameObject>> stateViewGameObjects = new Dictionary<Type, Dictionary<sbyte, GameObject>>();
        private object[] methodInvokeArgs = new object[1];
        private IAppStateReadWrite appState;
        private IUserManager users;
        // A copy of state view lookup values that we can safely iterate through while still modifying stateViewLookups
        private List<KeyValuePair<Type, Dictionary<sbyte, IStateObjectBase>>> stateViewLookupsReadOnly = new List<KeyValuePair<Type, Dictionary<sbyte, IStateObjectBase>>>();
        
        /// <summary>
        /// Makes sure an object has been instantiated for all item states in appState
        /// Initialized / updates all existing IStateObjectBase objects
        /// </summary>
        public void OnSessionUpdate(SessionState sessionState)
        {
            foreach (Type itemStateType in appState.ItemStateTypes)
            {
                // If we have no prefab then this is not a type we need to check
                GameObject prefab;
                if (!stateViewPrefabLookup.TryGetValue(itemStateType, out prefab))
                    continue;

                MethodInfo method;
                if (!checkObjectMethodLookup.TryGetValue(itemStateType, out method))
                {
                    // Create a new generic method using this type and store it for later
                    method = GetType().GetMethod("CheckForObjects").MakeGenericMethod(new Type[] { itemStateType });
                    checkObjectMethodLookup.Add(itemStateType, method);
                }

                // Load the args into our array and invoke the generic method
                methodInvokeArgs[0] = prefab;
                
                method.Invoke(this, methodInvokeArgs);
            }

            stateViewLookupsReadOnly.Clear();
            stateViewLookupsReadOnly.AddRange(stateViewLookups);
            // Update all the objects we've already created
            foreach (KeyValuePair<Type, Dictionary<sbyte,IStateObjectBase>> stateViewLookup in stateViewLookupsReadOnly)
            {
                foreach (KeyValuePair<sbyte,IStateObjectBase> stateObjectPair in stateViewLookup.Value)
                {
                    IStateObjectBase stateObject = stateObjectPair.Value;

                    if (!stateObject.IsInitialized)
                    {
                        try
                        {
                            // If it's not initialized yet, do so now
                            stateObject.Initialize(stateObjectPair.Key, appState, users, this);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Exception while attempting to initialize state object " + stateObject.name);
                            Debug.LogException(e);
                        }
                    }
                    else
                    {
                        try
                        {
                            stateObject.CheckAppState();

                            switch (AppRole)
                            {
                                case AppRoleEnum.Client:
                                    stateObject.UpdateClientListeners();
                                    break;

                                case AppRoleEnum.Server:
                                    stateObject.UpdateServerListeners();
                                    break;

                                case AppRoleEnum.Host:
                                    stateObject.UpdateClientListeners();
                                    stateObject.UpdateServerListeners();
                                    break;
                            }                           
                        } 
                        catch (Exception e)
                        {
                            Debug.LogError("Exception while attempting to update state object " + stateObject.name);
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Adds a state object that was found in the scene, as opposed to one that was generated automatically
        /// </summary>
        public void AddStateObject(sbyte itemKey, GameObject stateObjectGo, Type stateType)
        {
            if (stateObjectGo == null)
                throw new ArgumentNullException("StateObject cannot be null.");

            if (itemKey < 0)
                throw new IndexOutOfRangeException("Item num must be initialized.");
            
            // Add the gameobject to lookup by ID
            MethodInfo methodInfo = GetType().GetMethod("FindOrCreateGameObjectLookup").MakeGenericMethod(stateType);
            methodInfo.Invoke(this, new object[] {stateObjectGo, itemKey} );

            try
            {
                // NOW get ALL the state object base components
                // A single game object may have MORE THAN ONE state object script
                Component[] stateObjectComponents = stateObjectGo.GetComponentsInChildren(typeof(IStateObjectBase));
                foreach (IStateObjectBase stateObjectComponent in stateObjectComponents)
                {
                    // Add this to the correct lookup
                    Dictionary<sbyte, IStateObjectBase> stateObjectLookup = FindOrCreateStateObjectLookup(stateObjectComponent.GetType());
                    stateObjectLookup.Add(itemKey, stateObjectComponent);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Exception while trying to add existing state object " + stateType + " item num " + itemKey + ":");
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Returns state object script of type T associated with item state S
        /// </summary>
        public bool GetStateObject<S,T>(sbyte itemKey, out T stateObject, bool throwExceptionIfTypeNotFound = true) where S : IItemState<S> where T : Component, IStateObject<S>
        {
            if (itemKey < 0)
            {
                throw new IndexOutOfRangeException("Item num must be initialized.");
            }

            stateObject = null;
            
            Dictionary<sbyte, IStateObjectBase> stateView = FindOrCreateStateObjectLookup(typeof(T));
            IStateObjectBase stateObjectBase = null;
            if(stateView.TryGetValue(itemKey, out stateObjectBase))
            {
                stateObject = stateObjectBase as T;
            }

            return stateObject != null;
        }

        public IEnumerable<T> GetStateObjects<S,T>() where S : IItemState<S> where T : Component, IStateObject<S>
        {
            Type stateObjectType = typeof(T);                        
            Dictionary<sbyte, IStateObjectBase> stateView = FindOrCreateStateObjectLookup(stateObjectType);
            foreach (KeyValuePair<sbyte,IStateObjectBase> stateObjectPair in stateView)
            {
                yield return stateObjectPair.Value as T;
            }
            yield break;
        }

        /// <summary>
        /// Checks whether single GameObject has been instantiated for item state type S
        /// </summary>
        public void CheckForObjects<S>(GameObject prefab) where S : struct, IItemState<S>
        {
            // Get our lookup
            Dictionary<sbyte, GameObject> lookup = FindOrCreateGameObjectLookup<S>();

            // Go through each state in the list and see if we have an object for it
            GameObject stateObjectGo = null;

            try
            {
                foreach (S objectState in appState.GetStates<S>())
                {
                    if (!lookup.TryGetValue(objectState.Key, out stateObjectGo))
                    {
                        // If it doesn't exist, create it and initialize it
                        stateObjectGo = GameObject.Instantiate(prefab, transform) as GameObject;
                        // Add this game object to our game object lookup
                        lookup.Add(objectState.Key, stateObjectGo);

                        // NOW get ALL the state object base components
                        // A single game object may have MORE THAN ONE state object script
                        Component[] stateObjectComponents = stateObjectGo.GetComponentsInChildren(typeof(IStateObjectBase));
                        foreach (Component stateObjectComponent in stateObjectComponents)
                        {
                            IStateObjectBase stateObjectBase = stateObjectComponent as IStateObjectBase;
                            // Add this to the correct lookup
                            Dictionary<sbyte, IStateObjectBase> stateObjectLookup = FindOrCreateStateObjectLookup(stateObjectBase.GetType());
                            stateObjectLookup.Add(objectState.Key, stateObjectBase);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception while attempting to create object state for " + typeof(S).ToString());
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Returns a dictionary storing the single object associated with all state object scripts targeting item state type S
        /// </summary>
        public Dictionary<sbyte, GameObject> FindOrCreateGameObjectLookup<S> (GameObject itemToAdd = null, sbyte itemKey = -1) where S : IItemState<S>
        {
            Type key = typeof(S);
            Dictionary<sbyte, GameObject> lookup = null;
            if (!stateViewGameObjects.TryGetValue(key, out lookup))
            {
                lookup = new Dictionary<sbyte, GameObject>();
                stateViewGameObjects.Add(key, lookup);
            }

            if (itemToAdd != null)
                lookup.Add(itemKey, itemToAdd);

            return lookup;
        }

        protected Dictionary<sbyte, IStateObjectBase> FindOrCreateStateObjectLookup(Type stateObjectType)
        {
            Dictionary<sbyte, IStateObjectBase> lookup = null;
            if (!stateViewLookups.TryGetValue(stateObjectType, out lookup))
            {
                lookup = new Dictionary<sbyte, IStateObjectBase>();
                stateViewLookups.Add(stateObjectType, lookup);
            }
            return lookup;
        }

        public IEnumerable<GameObject> GetActiveObjects()
        {
            foreach (Dictionary<sbyte, GameObject> lookup in stateViewGameObjects.Values)
            {
                foreach (GameObject stateViewGameObject in lookup.Values)
                {
                    if (!stateViewGameObject.activeSelf)
                        continue;

                    yield return stateViewGameObject;
                }
            }
        }

        public IEnumerable<IStateObjectBase> GetActiveStateObjects()
        {
            foreach (Dictionary<sbyte, IStateObjectBase> lookup in stateViewLookups.Values)
            {
                foreach (IStateObjectBase stateViewObject in lookup.Values)
                {
                    if (!stateViewObject.gameObject.activeSelf)
                        continue;

                    yield return stateViewObject;
                }
            }
        }

        #region ISharingAppObject implementation

        public void OnSharingStart()
        {
            SceneScraper.FindInScenes<IAppStateReadWrite>(out appState);
            SceneScraper.FindInScenes<IUserManager>(out users);

            foreach (GameObject stateViewPrefab in stateViewPrefabs)
            {
                // Get all of the state object base components on the prefab's root (child components will not be used)
                Component[] stateObjectBaseComponents = stateViewPrefab.GetComponents(typeof(IStateObjectBase));
                if (stateObjectBaseComponents.Length == 0)
                    throw new Exception("State view prefab " + stateViewPrefab.name + " has no " + typeof(IStateObjectBase).Name + " on root GameObject.");

                // If there are multiple components, make sure they're all using the same type
                Type lookupType = null;
                foreach (IStateObjectBase stateObjectBaseComponent in stateObjectBaseComponents)
                {
                    if (lookupType == null)
                    {
                        lookupType = stateObjectBaseComponent.StateType;
                    }
                    else if (lookupType != stateObjectBaseComponent.StateType)
                    {
                        throw new Exception("State view prefab " + stateViewPrefab.name + " must not have " + typeof(IStateObjectBase).Name + " for more than one type.");
                    }
                }

                // Add to the lookup
                stateViewPrefabLookup.Add(lookupType, stateViewPrefab);
            }
        }

        public void OnStateInitialized() { }

        public void OnSharingStop() { }

        public void OnSessionStart() { }

        public void OnSessionStageBegin() { }

        public void OnSessionStageEnd() { }

        public void OnSessionEnd()
        {
            foreach (KeyValuePair<Type, Dictionary<sbyte, GameObject>> stateViewGameObject in stateViewGameObjects)
            {
                foreach (GameObject go in stateViewGameObject.Value.Values)
                {
                    GameObject.Destroy(go);
                }
            }

            stateViewGameObjects.Clear();
            stateViewLookups.Clear();
        }

        #endregion

        public void Initialize() { }

        public void Reset() { }

        public void Enable() { }

        public void Update() { }

        public void Disable() { }

        public void Destroy() { }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(StateObjects))]
        public class StateObjectsEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                StateObjects so = (StateObjects)target;

                DrawInspector(so);
            }

            public void DrawInspector(StateObjects so)
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField("State view Lookups");
                foreach (KeyValuePair<Type, Dictionary<sbyte, IStateObjectBase>> lookup in so.stateViewLookups)
                {
                    DrawStateViewLookup(lookup.Key, lookup.Value);
                }
                UnityEditor.EditorGUILayout.EndVertical();

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField("GameObject Lookups");
                foreach (KeyValuePair<Type,Dictionary<sbyte,GameObject>> lookup in so.stateViewGameObjects)
                {
                    DrawGamObjectLookup(lookup.Key, lookup.Value);
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }

            private void DrawStateViewLookup(Type key, Dictionary<sbyte, IStateObjectBase> lookup)
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField(key.Name);
                foreach (KeyValuePair<sbyte, IStateObjectBase> stateObject in lookup)
                {
                    UnityEditor.EditorGUILayout.LabelField(stateObject.Key + ":" + stateObject.Value.name);
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }

            private void DrawGamObjectLookup(Type key, Dictionary<sbyte, GameObject> lookup)
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField(key.Name);
                foreach (KeyValuePair<sbyte, GameObject> stateObject in lookup)
                {
                    UnityEditor.EditorGUILayout.LabelField(stateObject.Key + ":" + stateObject.Value.name);
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}