// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    /// <remarks>
    /// This feature is is being deprecated in 2.5 and will be removed in a future release.
    /// The code that remains will actively seek to remove existing facades in scenes to ensure that
    /// developers that update to 2.5 will get their scenes cleaned up appropriately.
    /// </remarks>
    [InitializeOnLoad]
    public static class MixedRealityToolkitFacadeHandler
    {
        private static readonly List<Transform> childrenToDelete = new List<Transform>();

        // While a scene save is occurring, facade creation is disabled
        // and currently present facades get deleted.
        private static bool sceneSaving = false;

        static MixedRealityToolkitFacadeHandler()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        #region callbacks

        private static void OnSceneGUI(SceneView sceneView)
        {
            UpdateServiceFacades();
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
#pragma warning disable 0618
                !MixedRealityToolkit.Instance.ActiveProfile.UseServiceInspectors)
#pragma warning restore 0618
            {
                DestroyFacades();
                return;
            }
        }

        private static void DestroyFacades()
        {
            for (int i = ServiceFacade.ActiveFacadeObjects.Count - 1; i >= 0; i--)
            {
                var facade = ServiceFacade.ActiveFacadeObjects[i];
                if (facade != null)
                {
                    GameObjectExtensions.DestroyGameObject(facade.gameObject);
                }
            }

            ServiceFacade.ActiveFacadeObjects.Clear();
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
