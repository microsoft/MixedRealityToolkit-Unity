using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.Initialization
{
    public class SceneLoadOp : ISceneLoadOp
    {
        public SceneLoadOp(AsyncOperation asyncOp, string sceneName, bool activateAutomatically)
        {
            this.asyncOp = asyncOp;
            this.sceneName = sceneName;
            this.activateAutomatically = activateAutomatically;

            asyncOp.allowSceneActivation = activateAutomatically;
        }
        public bool ReadyToActivate
        {
            get { return asyncOp.progress >= 0.9f; }
        }

        public bool Finished
        {
            get
            {
                if (!asyncOp.isDone)
                    return false;

                Scene targetScene = SceneManager.GetSceneByName(sceneName);
                return targetScene.IsValid()
                    && targetScene.isLoaded;
            }
        }

        private AsyncOperation asyncOp;
        private string sceneName;
        private bool activateAutomatically;

        public void Activate()
        {
            asyncOp.allowSceneActivation = true;
        }

        public void Dispose()
        {
            asyncOp = null;
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            return !Finished;
        }

        public void Reset() { }
    }
}