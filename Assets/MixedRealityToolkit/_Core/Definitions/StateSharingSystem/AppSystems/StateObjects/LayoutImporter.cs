using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    public class LayoutImporter : MonoBehaviour, ILayoutImporter
    {
        public AppRoleEnum AppRole { get; set; }

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
                stateView.AddStateObject(firstStateObject.ItemNum, stateObjectPair.Key, firstStateObject.StateType);
                yield return null;
            }

            Importing = false;
        }

        private IEnumerator PushToAppStateOverTime(IAppStateReadWrite appState)
        {
            foreach (SessionState sessionState in appState.GetStates<SessionState>())
            {
                foreach (KeyValuePair<GameObject,List<IStateObjectBase>> stateObjectPair in assets)
                {
                    IStateObjectBase firstStateObject = stateObjectPair.Value[0];
                    sbyte itemNum = appState.AddStateOfType(firstStateObject.StateType, sessionState.ItemNum, firstStateObject.ItemNum);
                    yield return null;
                }

                appState.Flush();
            }

            Importing = false;
        }

        public void OnSharingStart() { }
        public void OnStateInitialized() { }
        public void OnSharingStop() { }
    }
}