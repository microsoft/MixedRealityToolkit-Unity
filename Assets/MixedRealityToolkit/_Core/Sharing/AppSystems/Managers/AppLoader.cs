using Pixie.Core;
using Pixie.DeviceControl;
using Pixie.DeviceControl.Users;
using Pixie.Initialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private bool autoSelectProfile = true;
        [SerializeField]
        private UserProfile profile;

        private IAppManager appManager;
        private ISceneLoader sceneLoader;
        
        private void Start()
        {
            if (autoSelectProfile)
                SetProfile(profile.name);
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

            sceneLoader.SetActiveScene(profile.ActiveSceneName);

            // Load all of our launch scenes
            foreach (string launchSceneName in profile.LaunchSceneNames)
                yield return sceneLoader.LoadLocalScene(launchSceneName);

            // Find our app manager from the scene and kick it off
            ComponentFinder.FindInScenes<IAppManager>(out appManager);

            // Hand off control to the app manager!
            appManager.StartApp(profile);
            yield break;
        }
    }
}