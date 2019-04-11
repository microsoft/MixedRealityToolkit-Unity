using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
    [MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone|SupportedPlatforms.MacStandalone|SupportedPlatforms.LinuxStandalone|SupportedPlatforms.WindowsUniversal)]
    public class SceneTransitionService : BaseExtensionService, ISceneTransitionService, IMixedRealityExtensionService
    {
        private SceneTransitionServiceProfile sceneTransitionServiceProfile;

        private GameObject progressIndicatorObject;
        private IProgressIndicator progressIndicator;

        public SceneTransitionService(IMixedRealityServiceRegistrar registrar,  string name,  uint priority,  BaseMixedRealityProfile profile) : base(registrar, name, priority, profile) 
		{
            sceneTransitionServiceProfile = (SceneTransitionServiceProfile)profile;
		}

        public override void Initialize()
        {
         
        }

        public override void Enable()
        {
            CreateProgressIndicator();
        }

        public override void Update()
        {

        }

        public override void Disable()
        {
            CleanUpProgressIndicator();
        }

        public override void Destroy()
        {
            CleanUpProgressIndicator();
        }

        private void CreateProgressIndicator()
        {
            // Do service initialization here.
            if (sceneTransitionServiceProfile.ProgressIndicatorPrefab == null)
            {
                Debug.LogWarning("No progress indicator prefab found in profile.");
                return;
            }

            progressIndicatorObject = GameObject.Instantiate(sceneTransitionServiceProfile.ProgressIndicatorPrefab);
            progressIndicator = (IProgressIndicator)progressIndicatorObject.GetComponent(typeof(IProgressIndicator));

            if (progressIndicator == null)
            {
                Debug.LogError("Progress indicator prefab doesn't have a script implementing IProgressIndicator.");
                return;
            }

            // Ensure progress indicator doesn't get destroyed
            progressIndicatorObject.transform.DontDestroyOnLoad();

            progressIndicator.Disable();
        }

        private void CleanUpProgressIndicator()
        {
            if (progressIndicatorObject != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(progressIndicatorObject);
                }
                else
                {
                    GameObject.DestroyImmediate(progressIndicatorObject);
                }
            }
        }
    }
}