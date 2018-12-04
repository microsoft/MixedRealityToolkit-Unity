using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.Initialization
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        public Scene LastLoadedScene { get { return lastLoadedScene; } }
        public Scene ActiveScene { get { return activeScene; } }
        private Scene activeScene;
        private Scene lastLoadedScene;
        private string activeSceneName;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            lastLoadedScene = scene;

            // Immediately switch the scene back to the active scene so UNet sync still works
            if (activeSceneName == scene.name)
            {
                activeScene = scene;
                SceneManager.SetActiveScene(activeScene);
            }
        }

        public virtual ISceneLoadOp LoadLocalScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool activateAutomatically = true)
        {
            if (string.IsNullOrEmpty(sceneName))
                throw new NullReferenceException("Scene name cannot be null.");

            Scene targetScene = default(Scene);
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                {
                    targetScene = scene;
                    break;
                }
            }

            if (targetScene.isLoaded)
                throw new Exception("Can't load scene " + sceneName + " - already loaded!");

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOp.allowSceneActivation = activateAutomatically;
            SceneLoadOp sceneLoadOp = new SceneLoadOp(asyncOp, sceneName, activateAutomatically);

            return new SceneLoadOp(asyncOp, sceneName, activateAutomatically);
        }

        // In this implementation we're not doing anything fancy
        // Other scene loaders with networking may want to handle this differently
        public virtual ISceneLoadOp LoadSharedScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool activateAutomatically = true)
        {
            return LoadLocalScene(sceneName, mode, activateAutomatically);
        }

        public virtual ISceneUnloadOp UnloadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                throw new NullReferenceException("Scene name cannot be null.");

            Scene targetScene = default(Scene);
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene.name == sceneName)
                {
                    targetScene = scene;
                    break;
                }
            }

            if (targetScene.IsValid() && !targetScene.isLoaded)
                throw new Exception("Can't load scene " + sceneName + " - not loaded!");
            
            AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(sceneName);
            return new SceneUnloadOp(asyncOp, targetScene);
        }

        public void SetActiveScene(string sceneName)
        {
            this.activeSceneName = sceneName;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                {
                    activeScene = scene;
                    break;
                }
            }

            if (activeScene.IsValid() && activeScene.isLoaded)
                SceneManager.SetActiveScene(activeScene);
        }
    }
}