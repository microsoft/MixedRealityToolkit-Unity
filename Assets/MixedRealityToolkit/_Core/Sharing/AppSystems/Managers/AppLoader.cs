using Pixie.Core;
using Pixie.DeviceControl;
using Pixie.DeviceControl.Users;
using Pixie.Initialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.AppSystems.Managers
{
    public class AppLoader : MonoBehaviour, IUserProfiles
    {
        public IEnumerable<IUserProfile> Profiles { get { return profiles; } }
        
        public IUserProfile GetBestMatch(UserRoleEnum userRole, UserTeamEnum userTeam, DeviceTypeEnum userDevice)
        {
            // TEMP
            foreach (UserProfile profile in profiles)
            {
                if (profile.UserRole == userRole)
                    return profile;
            }
            return profiles[0];
        }

        [SerializeField]
        private UserProfile[] profiles;
        [SerializeField]
        private bool autoStart = true;
        [SerializeField]
        private UserProfile profile;

        private IAppManager appManager;
        private ISceneLoader sceneLoader;
        
        private void Start()
        {
            if (autoStart)
                StartCoroutine(LoadApp());
        }

        public void SetProfile(string userProfileName)
        {
            foreach (UserProfile profile in profiles)
            {
                if (profile.name == userProfileName)
                {
                    this.profile = profile;
                    break;
                }
            }
                    
            StartCoroutine(LoadApp());
        }

        private IEnumerator LoadApp()
        {
            ComponentFinder.FindInScenes<ISceneLoader>(out sceneLoader);

            switch (profile.DeviceType)
            {
                case DeviceTypeEnum.IOT:
                    sceneLoader.SceneOpMode = SceneOpTypeEnum.Immediate;
                    break;

                default:
                    sceneLoader.SceneOpMode = SceneOpTypeEnum.Async;
                    break;
            }

            sceneLoader.SetActiveScene(profile.ActiveSceneName);

            // Load all of our launch scenes
            foreach (string launchSceneName in profile.LaunchSceneNames)
            {
                // TEMP until we get this coroutine problem sorted on UWP
                SceneManager.LoadScene(launchSceneName, LoadSceneMode.Additive);
                //yield return StartCoroutine(sceneLoader.LoadLocalScene(launchSceneName));
                yield return null;
            }

            Debug.LogError("Finished loading scenes");

            // Find our app manager from the scene and kick it off
            ComponentFinder.FindInScenes<IAppManager>(out appManager);

            // Hand off control to the app manager!
            appManager.StartApp(profile);
            yield break;
        }
    }
}