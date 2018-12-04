using System;
using System.Collections;
using System.Collections.Generic;
using Pixie.Core;
using Pixie.StateControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.AppSystems.StateObjects
{
    public class LayoutImporter : MonoBehaviourSharingApp, ILayoutImporter
    {
        public bool Importing { get; private set; }

        private Dictionary<GameObject, List<IStateObjectBase>> assets = new Dictionary<GameObject, List<IStateObjectBase>>();
        
        public void GatherStateObjects(string layoutSceneName)
        {
            if (Importing)
                throw new Exception("Already importing in LayoutImporter!");

            assets.Clear();

            Importing = true;

            Scene layoutScene = SceneManager.GetSceneByName(layoutSceneName);

            if (!layoutScene.IsValid())
                throw new Exception("Layout scene " + layoutSceneName + " is invalid!");

            if (!layoutScene.isLoaded)
                throw new Exception("Layout scene " + layoutSceneName + " is not yet loaded!");
            
            foreach (GameObject rootGameObject in layoutScene.GetRootGameObjects())
            {
                FindLayoutAssets(rootGameObject.transform);
            }

            Importing = false;
        }

        private void FindLayoutAssets(Transform parent)
        {
            IStateObjectBase stateObject = (IStateObjectBase)parent.gameObject.GetComponent(typeof(IStateObjectBase));
            if (stateObject != null)
            {
                GameObject stateObjectGo = stateObject.gameObject;
                List<IStateObjectBase> stateObjects;
                if (!assets.TryGetValue(stateObjectGo, out stateObjects))
                {
                    stateObjects = new List<IStateObjectBase>();
                    assets.Add(stateObjectGo, stateObjects);
                }
                stateObjects.Add(stateObject);
            }

            foreach (Transform child in parent)
            {
                FindLayoutAssets(child);
            }
        }

        public void PushStateObjectsToStateView(IStateView stateView)
        {
            if (Importing)
                throw new Exception("Can't push state objects while already importing!");

            Importing = true;
            StartCoroutine(PushToStateViewOverTime(stateView));
        }

        public void PushStateObjectsToAppState(IAppStateReadWrite appState)
        {
            Debug.Log("PUSHING TO APP STATE");

            if (Importing)
                throw new Exception("Can't push state objects while already importing!");

            Importing = true;
            StartCoroutine(PushToAppStateOverTime(appState));
        }

        private IEnumerator PushToStateViewOverTime(IStateView stateView)
        {
            foreach (KeyValuePair<GameObject, List<IStateObjectBase>> stateObjectPair in assets)
            {
                IStateObjectBase firstStateObject = stateObjectPair.Value[0];
                stateView.AddStateObject(firstStateObject.ItemID, stateObjectPair.Key, firstStateObject.StateType);
                yield return null;
            }

            Importing = false;
        }

        private IEnumerator PushToAppStateOverTime(IAppStateReadWrite appState)
        {
            foreach (SessionState sessionState in appState.GetStates<SessionState>())
            {
                Debug.Log("Pushing to app state for session state " + sessionState.ItemID);

                foreach (KeyValuePair<GameObject,List<IStateObjectBase>> stateObjectPair in assets)
                {
                    Debug.Log("Adding " + stateObjectPair.Value.Count + "state objects for GO " + stateObjectPair.Key.name);

                    IStateObjectBase firstStateObject = stateObjectPair.Value[0];
                    short itemNum = appState.AddStateOfType(firstStateObject.StateType, firstStateObject.ItemID);
                    yield return null;
                }

                appState.Flush();
            }

            Importing = false;
        }
    }
}