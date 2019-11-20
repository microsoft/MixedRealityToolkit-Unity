// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Utilities.Facades
{
    /// <summary>
    /// Links service facade objects to active services.
    /// </summary>
    [InitializeOnLoad]
    public static class MixedRealityToolkitFacadeHandler
    {
        private static List<Transform> childrenToDelete = new List<Transform>();
        private static MixedRealityToolkit previousActiveInstance;
        private static long previousFrameCount;
        private static short editorUpdateTicks;
        private const short EditorUpdateTickInterval = 15;

        // While a scene save is occuring, facade creation is disabled
        // and currently present facades get deleted.
        private static bool sceneSaving = false;

        static MixedRealityToolkitFacadeHandler()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update += OnUpdate;
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        #region callbacks

        private static void OnSceneGUI(SceneView sceneView)
        {
            UpdateServiceFacades();
        }

        private static void OnUpdate()
        {
            editorUpdateTicks++;
            if (editorUpdateTicks > EditorUpdateTickInterval)
            {
                editorUpdateTicks = 0;
                UpdateServiceFacades();
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // If scripts were reloaded, nuke everything and start over
            CleanupCurrentFacades();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            CleanupCurrentFacades();
        }

        private static void OnSceneSaving(Scene scene, string path)
        {
            sceneSaving = true;
            CleanupCurrentFacades();
        }

        private static void OnSceneSaved(Scene scene)
        {
            sceneSaving = false;
        }

        #endregion

        private static void CleanupCurrentFacades()
        {
            foreach (MixedRealityToolkit toolkitInstance in GameObject.FindObjectsOfType<MixedRealityToolkit>())
            {
                DestroyAllChildren(toolkitInstance);
            }
            previousActiveInstance = null;
        }

        private static HashSet<IMixedRealityService> GetAllServices()
        {
            HashSet<IMixedRealityService> serviceList = new HashSet<IMixedRealityService>(MixedRealityServiceRegistry.GetAllServices());

            // These are core systems that are likely out-of-box services and known to have register DataProviders
            // Search for any dataproviders that service facades can be created for
            var dataProviderManagers = new IMixedRealityService[]{CoreServices.InputSystem, CoreServices.SpatialAwarenessSystem};
            foreach (var system in dataProviderManagers)
            {
                var dataProviderAccess = system as IMixedRealityDataProviderAccess;
                if (dataProviderAccess != null)
                {
                    foreach (var dataProvider in dataProviderAccess.GetDataProviders())
                    {
                        serviceList.Add(dataProvider);
                    }
                }
            }

            return serviceList;
        }

        private static void UpdateServiceFacades()
        {
            // If compiling or saving, don't modify service facades
            if (sceneSaving || EditorApplication.isCompiling)
            {
                return;
            }

            // If MRTK has no active instance
            // or there is no active profile for the active instance
            // or we are instructed to not use service inspectors
            // Return early and clean up any facade instances
            if (!MixedRealityToolkit.IsInitialized ||
                !MixedRealityToolkit.Instance.HasActiveProfile ||
                !MixedRealityToolkit.Instance.ActiveProfile.UseServiceInspectors)
            {
                DestroyFacades();
                return;
            }

            var mrtkTransform = MixedRealityToolkit.Instance.transform;
            bool newMRTKActiveInstance = previousActiveInstance != null && MixedRealityToolkit.Instance != previousActiveInstance;

            var serviceSet = GetAllServices();

            // Update existing service facade GameObjects
            for (int i = ServiceFacade.ActiveFacadeObjects.Count - 1; i >= 0; i--)
            {
                var facade = ServiceFacade.ActiveFacadeObjects[i];

                // if this facade is no longer valid, remove item
                if (facade == null)
                {
                    ServiceFacade.ActiveFacadeObjects.Remove(facade);
                }
                // If service facade is not part of the current service list,
                // Remove from the list so that the facade is not-duply-created in the following serviceSet enumeration loop
                else if (!serviceSet.Contains(facade.Service))
                {
                    ServiceFacade.ActiveFacadeObjects.Remove(facade);
                    GameObjectExtensions.DestroyGameObject(facade.gameObject);
                }
                else
                {
                    // Else item is valid and exists in our list. Remove from list
                    serviceSet.Remove(facade.Service);

                    //Ensure valid facades are parented under the current MRTK active instance
                    if (facade.transform.parent != mrtkTransform)
                    {
                        facade.transform.parent = mrtkTransform;
                    }
                }
            }

            // Remaining services need to be created and added into scene
            foreach (var service in serviceSet)
            {
                // Find where we need to place service based on name ordering
                int idx = 0;
                for (; idx < mrtkTransform.childCount; idx++)
                {
                    if (mrtkTransform.GetChild(idx).name.CompareTo(service.GetType().Name) >= 0)
                    {
                        break;
                    }
                }

                CreateFacade(mrtkTransform, service, idx);
            }

            previousActiveInstance = MixedRealityToolkit.Instance;
        }

        private static void DestroyFacades()
        {
            foreach (var facade in ServiceFacade.ActiveFacadeObjects)
            {
                if (facade != null)
                {
                    GameObjectExtensions.DestroyGameObject(facade.gameObject);
                }
            }

            ServiceFacade.ActiveFacadeObjects.Clear();
        }

        private static void CreateFacade(Transform parent, IMixedRealityService service, int facadeIndex)
        {
            GameObject facadeObject = new GameObject();
            facadeObject.transform.parent = parent;
            facadeObject.transform.SetSiblingIndex(facadeIndex);

            ServiceFacade facade = facadeObject.AddComponent<ServiceFacade>();
            facade.SetService(service);
        }

        private static void DestroyAllChildren(MixedRealityToolkit instance)
        {
            Transform instanceTransform = instance.transform;

            childrenToDelete.Clear();
            foreach (Transform child in instanceTransform.transform)
            {
                childrenToDelete.Add(child);
            }

            foreach (ServiceFacade facade in ServiceFacade.ActiveFacadeObjects)
            {
                if (!childrenToDelete.Contains(facade.transform))
                {
                    childrenToDelete.Add(facade.transform);
                }
            }

            foreach (Transform child in childrenToDelete)
            {
                GameObjectExtensions.DestroyGameObject(child.gameObject);
            }

            childrenToDelete.Clear();
        }
    }
}
