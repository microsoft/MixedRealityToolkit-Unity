using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.Initialization
{
    public partial class SceneUnloadOp : ISceneUnloadOp
    {
        public SceneUnloadOp(AsyncOperation asyncOp, Scene targetScene)
        {
            this.asyncOp = asyncOp;
            this.targetScene = targetScene;
        }

        private AsyncOperation asyncOp;
        private Scene targetScene;

        public bool Finished
        {
            get { return asyncOp.isDone && !targetScene.isLoaded; }
        }

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return !Finished;
        }

        public void Reset() { }
    }
}