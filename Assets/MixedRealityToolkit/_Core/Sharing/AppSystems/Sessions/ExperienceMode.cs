using Pixie.DeviceControl.Users;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AppSystems.Sessions
{
    [CreateAssetMenu(fileName = "ExperienceMode", menuName = "Pixie/ExperienceMode", order = 1)]
    public class ExperienceMode : ScriptableObject, IExperienceMode
    {
        public string Name { get { return name; } }
        public short ID { get { return id; } }
        public string LayoutSceneName { get { return layoutSceneName; } }
        public IEnumerable<GameObject> SessionStagePrefabs { get { return sessionStagePrefabs; } }
        public IEnumerable<UserDefinition> UserDefinitions { get { return userDefinitions; } }
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
        private UserDefinition[] userDefinitions;
        [SerializeField]
        private string layoutSceneName;
    }
}