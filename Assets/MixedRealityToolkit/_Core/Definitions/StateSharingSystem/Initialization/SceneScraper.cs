using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization
{
    /// <summary>
    /// This class wraps some of Unity's strange / arbitrary scene loading behavior in a container to make it less confusing.
    /// Also ensures that the active scene remains the scene where this script was initially loaded.
    /// This is to ensure that UNet synchronization continues to work while loading scenes additively.
    /// </summary>
    public class SceneScraper : MonoBehaviour
    {
        public static Scene LastLoadedScene { get { return lastLoadedScene; } }
        private static Scene startup;
        private static Scene lastLoadedScene;

        private static List<GameObject> rootGameObjects = new List<GameObject>();
        private static Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

        private static SceneScraper instance;

        public static ISceneLoadOp LoadUnactivatedScene(string sceneName)
        {
            return new SceneLoadOp(sceneName);
        }

        public static IEnumerator LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new NullReferenceException("Scene name cannot be null.");
            }
            
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOp.allowSceneActivation = true;
            yield return asyncOp;
            // Wait for the scene to fully load before exiting
            while (!(lastLoadedScene.IsValid() && lastLoadedScene.isLoaded))
            {
                yield return null;
            }
            yield break;
        }

        public static IEnumerator UnloadScene(string sceneName)
        {
            loadedScenes.Remove(sceneName);
            AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = true;

            yield return asyncOp;
            Debug.Log(asyncOp.progress);
        }

        public static bool FindInScenes<T> (out T component, bool throwExceptionIfNotFound = true) where T : class
        {
            component = null;

            foreach (Scene scene in loadedScenes.Values)
            {
                if (FindInScene<T> (scene, out component, false))
                {
                    return true;
                }
            }

            if (throwExceptionIfNotFound)
            {
                throw new System.NullReferenceException("Component type " + typeof(T).ToString() + " not found in loaded scenes");
            }

            return false;
        }

        private static bool FindInScene<T>(Scene scene, out T component, bool throwExceptionIfNotFound = true) where T : class
        {
            component = null;
            if (!scene.IsValid())
            {
                return false;
            }

            if (!scene.isLoaded)
            {
                return false;
            }

            rootGameObjects.Clear();
            scene.GetRootGameObjects(rootGameObjects);
            foreach (GameObject rootGameObject in rootGameObjects)
            {
                component = rootGameObject.GetComponent(typeof(T)) as T;
                if (component != null)
                {
                    break;
                }
            }

            return component != null;
        }

        /// <summary>
        /// Finds all objects of type T attached to root gameobjects and adds them to list components.
        /// Returns all objects NOT found in components list.
        /// </summary>
        public static IEnumerable<T> FindAllInScenes<T>(List<T> components) where T : class
        {
            List<T> newComponents = new List<T>();

            foreach (Scene scene in loadedScenes.Values)
            {
                FindAllInScene<T>(scene, newComponents, components);
            }

            components.AddRange(newComponents);

            return newComponents;
        }

        private static void FindAllInScene<T>(Scene scene, List<T> newComponents, List<T> existingComponents) where T : class
        {
            rootGameObjects.Clear();
            scene.GetRootGameObjects(rootGameObjects);
            foreach (GameObject rootGameObject in rootGameObjects)
            {
                foreach (Component c in rootGameObject.GetComponents(typeof(T)))
                {
                    T component = c as T;
                    if (component != null && !existingComponents.Contains(component))
                    {
                        newComponents.Add(component);
                    }
                }
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
            loadedScenes.Clear();

            startup = SceneManager.GetActiveScene();
            loadedScenes.Add(startup.name, startup);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnloaded;
            loadedScenes.Clear();
        }

        private void SceneUnloaded(Scene scene)
        {
            if (loadedScenes.ContainsKey(scene.name))
            {
                loadedScenes.Remove(scene.name);
            }
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            lastLoadedScene = scene;

            // Store for later retrieval
            if (!loadedScenes.ContainsKey(scene.name))
            {
                loadedScenes.Add(scene.name, scene);
            }

            // Immediately switch the scene back to the active scene so UNet sync still works
            SceneManager.SetActiveScene(startup);
        }

        private class SceneLoadOp : ISceneLoadOp
        {
            const float UnityActivationLoadCutoff = 0.9f;

            public bool ReadyToActivate
            {
                get
                {
                    return asyncOp.progress >= UnityActivationLoadCutoff;
                }
            }

            public bool Activated
            {
                get
                {
                    return loadedScenes.TryGetValue(sceneName, out loadedScene) && loadedScene.IsValid() && loadedScene.isLoaded;
                }
            }

            private AsyncOperation asyncOp;
            private string sceneName;
            private Scene loadedScene;

            public SceneLoadOp(string sceneName)
            {
                this.sceneName = sceneName;
                this.asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                asyncOp.allowSceneActivation = false;
            }

            public void Activate()
            {
                if (!ReadyToActivate)
                    return;

                asyncOp.allowSceneActivation = true;
            }

            public void Dispose()
            {
                asyncOp = null;
                sceneName = null;
                loadedScene = default(Scene);
            }
        }

        public static void UnloadSceneAutomatically(string sceneName)
        {
            instance.StartCoroutine(UnloadScene(sceneName));
        }
    }
}