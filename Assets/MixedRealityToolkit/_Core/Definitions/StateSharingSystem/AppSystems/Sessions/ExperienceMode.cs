using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    [CreateAssetMenu(fileName = "ExperienceMode", menuName = "Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem/ExperienceMode", order = 1)]
    public class ExperienceMode : ScriptableObject, IExperienceMode
    {
        public string Name { get { return name; } }
        public short ID { get { return id; } }
        public string LayoutSceneName { get { return layoutScene.name; } }
        public IEnumerable<GameObject> SessionStagePrefabs { get { return sessionStagePrefabs; } }
        public IEnumerable<StandInSetting> StandInSettings { get { return standInSettings; } }
        public IEnumerable<string> StageNames
        {
            get
            {
                foreach (GameObject sessionStagePrefab in sessionStagePrefabs)
                {
                    yield return sessionStagePrefab.name;
                }
            }
        }

        [SerializeField]
        private short id;
        [SerializeField]
        private GameObject[] sessionStagePrefabs;
        [SerializeField]
        private StandInSetting[] standInSettings;
        [SerializeField]
        private UnityEngine.Object layoutScene;
    }
}