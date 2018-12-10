using UnityEngine.SceneManagement;

namespace Pixie.Initialization
{
    public interface ISceneLoader
    {
        ISceneLoadOp LoadLocalScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool activateAutomatically = true);
        ISceneLoadOp LoadSharedScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool activateAutomatically = true);
        ISceneUnloadOp UnloadScene(string sceneName);

        SceneOpTypeEnum SceneOpMode { get; set; }
        Scene LastLoadedScene { get; }
        Scene ActiveScene { get; }

        void SetActiveScene(string sceneName);
    }
}