// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SceneSystem;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    /// <summary>
    /// Utility class to load scenes through MRTK Scene System using a scene transition.
    /// Otherwise, it uses Scene System's LoadContent()
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Extensions/LoadContentScene")]
    public class LoadContentScene : MonoBehaviour
    {
        [SerializeField]
        private LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        [SerializeField]
        private SceneInfo contentScene = SceneInfo.Empty;
        [SerializeField]
        private bool loadOnStartup = false;

        private void Start()
        {
            if (loadOnStartup)
            {
                LoadContent();
            }
        }

        private static readonly ProfilerMarker LoadContentPerfMarker = new ProfilerMarker("[MRTK] LoadContentScene.LoadContent");

        /// <summary>
        /// Load a scene with contentScene.Name
        /// </summary>
        public void LoadContent()
        {
            using (LoadContentPerfMarker.Auto())
            {
                ISceneTransitionService transitions = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();
                if (transitions.TransitionInProgress)
                {
                    return;
                }
                transitions.DoSceneTransition(() => CoreServices.SceneSystem.LoadContent(contentScene.Name, loadSceneMode));
            }
        }
    }
}