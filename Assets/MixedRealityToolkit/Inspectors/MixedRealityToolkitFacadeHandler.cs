// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Facades
{
    /// <summary>
    /// Links service facade objects to active services.
    /// </summary>
    [InitializeOnLoad]
    public static class MixedRealityToolkitFacadeHandler
    {
        private static List<Transform> childrenToDelete = new List<Transform>();
        private static List<IMixedRealityService> servicesToSort = new List<IMixedRealityService>();
        private static MixedRealityToolkit previousActiveInstance;

        static MixedRealityToolkitFacadeHandler()
        {
            SceneView.onSceneGUIDelegate += UpdateServiceFacades;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // If scripts were reloaded, nuke everything and start over
            foreach (MixedRealityToolkit toolkitInstance in GameObject.FindObjectsOfType<MixedRealityToolkit>())
            {
                DestroyAllChildren(toolkitInstance);
            }
            previousActiveInstance = null;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            foreach (MixedRealityToolkit toolkitInstance in GameObject.FindObjectsOfType<MixedRealityToolkit>())
            {
                DestroyAllChildren(toolkitInstance);
            }
            previousActiveInstance = null;
        }

        private static void UpdateServiceFacades(SceneView sceneView)
        {
            if (!MixedRealityToolkit.IsInitialized)
            {   // Nothing to do here.
                return;
            }

            if (EditorApplication.isCompiling)
            {   // Wait for compilation to complete before creating or destroying facades
                return;
            }

            if (previousActiveInstance != null && MixedRealityToolkit.Instance != previousActiveInstance)
            {   // We've changed active instances. Destroy all children in the previous instance.
                DestroyAllChildren(previousActiveInstance);
            }

            if (MixedRealityToolkit.Instance.HasActiveProfile && !MixedRealityToolkit.Instance.ActiveProfile.UseServiceInspectors)
            {   // If we're not using inspectors, destroy them all now
                DestroyAllChildren(MixedRealityToolkit.Instance);
                return;
            }

            servicesToSort.Clear();

            foreach (IMixedRealityService service in MixedRealityToolkit.Instance.ActiveSystems.Values)
            {
                servicesToSort.Add(service);
            }

            foreach (Tuple<Type, IMixedRealityService> registeredService in MixedRealityToolkit.Instance.RegisteredMixedRealityServices)
            {
                servicesToSort.Add(registeredService.Item2);
            }

            servicesToSort.Sort(
                delegate (IMixedRealityService s1, IMixedRealityService s2)
                {
                    string s1Name = s1.GetType().Name;
                    string s2Name = s2.GetType().Name;

                    if (s1Name == s2Name)
                    {
                        return s1.Priority.CompareTo(s2.Priority);
                    }

                    return s1Name.CompareTo(s2Name);
                });

            for (int i = 0; i < servicesToSort.Count; i++)
            {
                CreateFacade(MixedRealityToolkit.Instance.transform, servicesToSort[i], i);
            }

            // Delete any stragglers
            childrenToDelete.Clear();
            for (int i = servicesToSort.Count; i < MixedRealityToolkit.Instance.transform.childCount; i++)
            {
                childrenToDelete.Add(MixedRealityToolkit.Instance.transform.GetChild(i));
            }

            foreach (Transform childToDelete in childrenToDelete)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(childToDelete.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(childToDelete.gameObject);
                }
            }

            try
            {
                // Update all self-registered facades
                foreach (ServiceFacade facade in ServiceFacade.ActiveFacadeObjects)
                {
                    if (facade == null)
                    {
                        continue;
                    }

                    facade.CheckIfStillValid();
                }
            }
            catch(Exception)
            {
                Debug.LogWarning("Service Facades should remain parented under the MixedRealityToolkit instance.");
            }

            previousActiveInstance = MixedRealityToolkit.Instance;
        }

        private static void CreateFacade(Transform parent, IMixedRealityService service, int facadeIndex)
        {
            ServiceFacade facade = null;
            if (facadeIndex > parent.transform.childCount - 1)
            {
                GameObject facadeObject = new GameObject();
                facadeObject.transform.parent = parent;
                facade = facadeObject.AddComponent<ServiceFacade>();
            }
            else
            {
                Transform child = parent.GetChild(facadeIndex);
                facade = child.GetComponent<ServiceFacade>();
                if (facade == null)
                {
                    facade = child.gameObject.AddComponent<ServiceFacade>();
                }
            }

            if (facade.transform.hasChanged)
            {
                facade.transform.localPosition = Vector3.zero;
                facade.transform.localRotation = Quaternion.identity;
                facade.transform.localScale = Vector3.one;
                facade.transform.hasChanged = false;
            }

            facade.SetService(service, parent);
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
                    childrenToDelete.Add(facade.transform);
            }

            foreach (Transform child in childrenToDelete)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }

            childrenToDelete.Clear();
            servicesToSort.Clear();
        }
    }
}
