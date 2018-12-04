using Pixie.Initialization;
using UnityEngine.SceneManagement;

namespace Pixie.Networking
{
    public class PhotonSceneLoader : SceneLoader
    {
        public override ISceneLoadOp LoadSharedScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool activateAutomatically = true)
        {
            return base.LoadSharedScene(sceneName, mode, activateAutomatically);
        }
    }
}