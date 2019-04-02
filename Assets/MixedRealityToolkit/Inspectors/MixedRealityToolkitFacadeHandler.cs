// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Facades
{
    [InitializeOnLoad]
    public static class MixedRealityToolkitFacadeHandler
    {
        private static List<Transform> childrenToDelete = new List<Transform>();
        private static List<ServiceFacade> childrenToSort = new List<ServiceFacade>();

        static MixedRealityToolkitFacadeHandler()
        {
            SceneView.onSceneGUIDelegate += UpdateServiceFacades;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (MixedRealityToolkit.Instance == null)
                return;

            // When the play state changes just nuke everything and start over
            DestroyAllChildren();
        }

        private static void UpdateServiceFacades(SceneView sceneView)
        {
            if (MixedRealityToolkit.Instance == null)
                return;

            // If we're not using inspectors, destroy them all now
            if (MixedRealityToolkit.Instance.HasActiveProfile && !MixedRealityToolkit.Instance.ActiveProfile.UseServiceInspectors)
            {
                DestroyAllChildren();
                return;
            }

            childrenToSort.Clear();
            
            int facadeIndex = 0;
            ServiceFacade facade = null;
            foreach (IMixedRealityService service in MixedRealityToolkit.Instance.ActiveSystems.Values)
            {
                facade = CreateFacade(MixedRealityToolkit.Instance.transform, service, ref facadeIndex);
            }

            foreach (Tuple<Type,IMixedRealityService> registeredService in MixedRealityToolkit.Instance.RegisteredMixedRealityServices)
            {
                facade = CreateFacade(MixedRealityToolkit.Instance.transform, registeredService.Item2, ref facadeIndex);
            }

            /* Disabling this until we fix the child order bug.
             * childrenToSort.Sort(
                delegate (ServiceFacade s1, ServiceFacade s2) 
                {
                    if (s1.Service.Priority == s2.Service.Priority)
                    {
                        return s1.ServiceType.Name.CompareTo(s2.ServiceType.Name);
                    }

                    return s1.Service.Priority.CompareTo(s2.Service.Priority); }
                );*/

            // Set sibling index first
            for (int i = 0; i < childrenToSort.Count; i++)
                childrenToSort[i].transform.SetSiblingIndex(i);
                        
            // Delete any stragglers
            childrenToDelete.Clear();
            for (int i = facadeIndex; i < MixedRealityToolkit.Instance.transform.childCount; i++)
                childrenToDelete.Add(MixedRealityToolkit.Instance.transform.GetChild(i));

            foreach (Transform childToDelete in childrenToDelete)
                GameObject.DestroyImmediate(childToDelete.gameObject);
        }

        private static ServiceFacade CreateFacade(Transform parent, IMixedRealityService service, ref int facadeIndex)
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

            facade.SetService(service);

            childrenToSort.Add(facade);

            facadeIndex++;

            return facade;
        }

        private static void DestroyAllChildren()
        {
            Transform instanceTransform = MixedRealityToolkit.Instance.transform;

            childrenToDelete.Clear();
            foreach (Transform child in instanceTransform.transform)
                childrenToDelete.Add(child);

            foreach (Transform child in childrenToDelete)
                GameObject.DestroyImmediate(child.gameObject);

            childrenToDelete.Clear();
            childrenToSort.Clear();

        }
    }
}
