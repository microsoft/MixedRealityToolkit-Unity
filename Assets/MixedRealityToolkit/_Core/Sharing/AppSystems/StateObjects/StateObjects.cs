using System;
using System.Collections.Generic;
using System.Reflection;
using Pixie.Core;
using Pixie.DeviceControl.Users;
using Pixie.Initialization;
using Pixie.StateControl;
using UnityEngine;

namespace Pixie.AppSystems.StateObjects
{
    public class StateObjects : MonoBehaviourSharingApp, IStateView
    {
        [SerializeField]
        protected GameObject[] stateViewPrefabs;

        private Dictionary<Type, GameObject> stateViewPrefabLookup = new Dictionary<Type, GameObject>();
        //private Dictionary<Type, MethodInfo> checkObjectMethodLookup = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, Dictionary<short, IStateObjectBase>> stateViewLookups = new Dictionary<Type, Dictionary<short, IStateObjectBase>>();
        private Dictionary<Type, Dictionary<short, GameObject>> stateViewGameObjects = new Dictionary<Type, Dictionary<short, GameObject>>();
        private object[] methodInvokeArgs = new object[1];
        private IAppStateReadWrite appState;
        private IUserManager users;
        // A copy of state view lookup values that we can safely iterate through while still modifying stateViewLookups
        private List<KeyValuePair<Type, Dictionary<short, IStateObjectBase>>> stateViewLookupsReadOnly = new List<KeyValuePair<Type, Dictionary<short, IStateObjectBase>>>();

        public void UpdateStateObjects()
        {
            foreach (Type itemStateType in appState.ItemStateTypes)
            {
                // If we have no prefab then this is not a type we need to check
                GameObject prefab;
                if (!stateViewPrefabLookup.TryGetValue(itemStateType, out prefab))
                    continue;

                CheckForObjects(itemStateType, prefab);
            }

            stateViewLookupsReadOnly.Clear();
            stateViewLookupsReadOnly.AddRange(stateViewLookups);
            // Update all the objects we've already created
            foreach (KeyValuePair<Type, Dictionary<short, IStateObjectBase>> stateViewLookup in stateViewLookupsReadOnly)
            {
                foreach (KeyValuePair<short, IStateObjectBase> stateObjectPair in stateViewLookup.Value)
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

        public void OnSessionUpdate(SessionState sessionState)
        {
            UpdateStateObjects();
        }
        
        /// <summary>
        /// Adds a state object that was found in the scene, as opposed to one that was generated automatically
        /// </summary>
        public void AddStateObject(short itemKey, GameObject stateObjectGo, Type stateType)
        {
            if (stateObjectGo == null)
                throw new ArgumentNullException("StateObject cannot be null.");

            if (itemKey < 0)
                throw new IndexOutOfRangeException("Item num must be initialized.");

            try
            {
                // Find / create the lookup for the game object and add it
                Type genericStateObjectType = typeof(IStateObject<>);
                Type stateObjectType = genericStateObjectType.MakeGenericType(new Type[] { stateType });
                FindOrCreateGameObjectLookup(stateObjectType, stateObjectGo, itemKey);

                // NOW get ALL the state object base components
                // A single game object may have MORE THAN ONE state object script
                Component[] stateObjectComponents = stateObjectGo.GetComponentsInChildren(typeof(IStateObjectBase));
                foreach (IStateObjectBase stateObjectComponent in stateObjectComponents)
                {
                    // Add this to the correct lookup
                    Dictionary<short, IStateObjectBase> stateObjectLookup = FindOrCreateStateObjectLookup(stateObjectComponent.GetType());
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
        public bool GetStateObject<S,T>(short itemKey, out T stateObject, bool throwExceptionIfTypeNotFound = true) where S : IItemState, IItemStateComparer<S> where T : Component, IStateObject<S>
        {
            if (itemKey < 0)
            {
                throw new IndexOutOfRangeException("Item num must be initialized.");
            }

            stateObject = null;
            
            Dictionary<short, IStateObjectBase> stateView = FindOrCreateStateObjectLookup(typeof(T));
            IStateObjectBase stateObjectBase = null;
            if(stateView.TryGetValue(itemKey, out stateObjectBase))
            {
                stateObject = stateObjectBase as T;
            }

            return stateObject != null;
        }

        public IEnumerable<T> GetStateObjects<S,T>() where S : IItemState, IItemStateComparer<S> where T : Component, IStateObject<S>
        {
            Type stateObjectType = typeof(T);                        
            Dictionary<short, IStateObjectBase> stateView = FindOrCreateStateObjectLookup(stateObjectType);
            foreach (KeyValuePair<short,IStateObjectBase> stateObjectPair in stateView)
            {
                yield return stateObjectPair.Value as T;
            }
            yield break;
        }

        /// <summary>
        /// Checks whether single GameObject has been instantiated for item state type S
        /// </summary>
        public void CheckForObjects(Type stateType, GameObject prefab)
        {
            // Get our lookup
            Dictionary<short, GameObject> lookup = FindOrCreateGameObjectLookup(stateType);

            // Go through each state in the list and see if we have an object for it
            GameObject stateObjectGo = null;

            try
            {
                foreach (IItemState objectState in appState.GetStates(stateType))
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
                            Dictionary<short, IStateObjectBase> stateObjectLookup = FindOrCreateStateObjectLookup(stateObjectBase.GetType());
                            stateObjectLookup.Add(objectState.Key, stateObjectBase);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception while attempting to create object state for " + stateType.ToString());
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Returns a dictionary storing the single object associated with all state object scripts targeting item state type S
        /// </summary>
        public Dictionary<short, GameObject> FindOrCreateGameObjectLookup (Type stateObjectType, GameObject itemToAdd = null, short itemKey = -1)
        {
            Dictionary<short, GameObject> lookup = null;
            if (!stateViewGameObjects.TryGetValue(stateObjectType, out lookup))
            {
                lookup = new Dictionary<short, GameObject>();
                stateViewGameObjects.Add(stateObjectType, lookup);
            }

            if (itemToAdd != null)
                lookup.Add(itemKey, itemToAdd);

            return lookup;
        }

        protected Dictionary<short, IStateObjectBase> FindOrCreateStateObjectLookup(Type stateObjectType)
        {
            Dictionary<short, IStateObjectBase> lookup = null;
            if (!stateViewLookups.TryGetValue(stateObjectType, out lookup))
            {
                lookup = new Dictionary<short, IStateObjectBase>();
                stateViewLookups.Add(stateObjectType, lookup);
            }
            return lookup;
        }

        public IEnumerable<GameObject> GetActiveObjects()
        {
            foreach (Dictionary<short, GameObject> lookup in stateViewGameObjects.Values)
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
            foreach (Dictionary<short, IStateObjectBase> lookup in stateViewLookups.Values)
            {
                foreach (IStateObjectBase stateViewObject in lookup.Values)
                {
                    if (!stateViewObject.gameObject.activeSelf)
                        continue;

                    yield return stateViewObject;
                }
            }
        }

        public override void OnAppInitialize()
        {
            Debug.Log("OnAppInitialize in state objects");

            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
            ComponentFinder.FindInScenes<IUserManager>(out users);

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

        public void OnSessionStart() { }

        public void OnSessionStageBegin() { }

        public void OnSessionStageEnd() { }

        public void OnSessionEnd()
        {
            foreach (KeyValuePair<Type, Dictionary<short, GameObject>> stateViewGameObject in stateViewGameObjects)
            {
                foreach (GameObject go in stateViewGameObject.Value.Values)
                {
                    GameObject.Destroy(go);
                }
            }

            stateViewGameObjects.Clear();
            stateViewLookups.Clear();
        }

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
                UnityEditor.EditorGUILayout.LabelField("Available prefabs (" + so.stateViewGameObjects.Count + ")");
                foreach (KeyValuePair<Type,GameObject> availablePrefab in so.stateViewPrefabLookup)
                {
                    DrawAvailablePrefab(availablePrefab.Key, availablePrefab.Value);
                }
                UnityEditor.EditorGUILayout.EndVertical();

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField("State view Lookups");
                foreach (KeyValuePair<Type, Dictionary<short, IStateObjectBase>> lookup in so.stateViewLookups)
                {
                    DrawStateViewLookup(lookup.Key, lookup.Value);
                }
                UnityEditor.EditorGUILayout.EndVertical();

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField("GameObject Lookups");
                foreach (KeyValuePair<Type,Dictionary<short,GameObject>> lookup in so.stateViewGameObjects)
                {
                    DrawGamObjectLookup(lookup.Key, lookup.Value);
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }

            private void DrawAvailablePrefab(Type type, GameObject gameObject)
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField(type.Name);
                if (GUILayout.Button(gameObject.name))
                    UnityEditor.Selection.activeGameObject = gameObject;
                UnityEditor.EditorGUILayout.EndVertical();
            }

            private void DrawStateViewLookup(Type key, Dictionary<short, IStateObjectBase> lookup)
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField(key.Name);
                foreach (KeyValuePair<short, IStateObjectBase> stateObject in lookup)
                {
                    UnityEditor.EditorGUILayout.LabelField(stateObject.Key + ":" + stateObject.Value.name);
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }

            private void DrawGamObjectLookup(Type key, Dictionary<short, GameObject> lookup)
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField(key.Name);
                foreach (KeyValuePair<short, GameObject> stateObject in lookup)
                {
                    UnityEditor.EditorGUILayout.LabelField(stateObject.Key + ":" + stateObject.Value.name);
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}