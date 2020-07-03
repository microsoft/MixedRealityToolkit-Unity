// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// When the button is selected, it triggers starting the specified scene.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/OnLoadStartScene")]
    public class OnLoadStartScene : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Name of the scene to be loaded when the button is selected.")]
        private string SceneToBeLoaded = "";

        public void Start()
        {
            LoadNewScene();
        }

        private void LoadNewScene()
        {
            if (SceneToBeLoaded != "")
            {
                SceneManager.LoadSceneAsync(SceneToBeLoaded, LoadSceneMode.Additive);
            }
        }
    }
}
