using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.Initialization
{
    public enum SceneOpTypeEnum
    {
        Immediate,
        Async,
    }

    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        public class EmptyOp : ISceneLoadOp, ISceneUnloadOp
        {
            public bool ReadyToActivate { get { return true; } }
            public bool Finished { get { return true; } }
            public object Current { get { return null; } }

            public void Activate() { }
            public bool MoveNext() { return false; }
            public void Reset() { }
        }

        public SceneOpTypeEnum SceneOpMode { get { return sceneOpMode; } set { sceneOpMode = value; } }
        public Scene LastLoadedScene { get { return lastLoadedScene; } }
        public Scene ActiveScene { get { return activeScene; } }
        private Scene activeScene;
        private Scene lastLoadedScene;
        private string activeSceneName;
        private EmptyOp emptyOp = new EmptyOp();

        [SerializeField]
        private SceneOpTypeEnum sceneOpMode;

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

            if (activeSceneName == scene.name)
            {
                activeScene = scene;
                SceneManager.SetActiveScene(activeScene);
            }
        }

        public virtual ISceneLoadOp LoadLocalScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool activateAutomatically = true)
        {
            Debug.LogError("Loading local scene " + sceneName);

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

            switch (sceneOpMode)
            {
                case SceneOpTypeEnum.Async:
                    AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    asyncOp.allowSceneActivation = activateAutomatically;
                    SceneLoadOp sceneLoadOp = new SceneLoadOp(asyncOp, sceneName, activateAutomatically);

                    Debug.LogError("Returning scene load op");

                    return new SceneLoadOp(asyncOp, sceneName, activateAutomatically);

                default:
                case SceneOpTypeEnum.Immediate:
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                    return emptyOp;
            }
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

            switch (sceneOpMode)
            {
                case SceneOpTypeEnum.Async:
                    AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(sceneName);
                    return new SceneUnloadOp(asyncOp, targetScene);

                case SceneOpTypeEnum.Immediate:
                default:
                    SceneManager.UnloadScene(sceneName);
                    return emptyOp;
            }
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