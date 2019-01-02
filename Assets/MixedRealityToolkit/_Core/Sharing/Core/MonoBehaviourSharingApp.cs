using UnityEngine;

namespace Pixie.Core
{
    public class MonoBehaviourSharingApp : MonoBehaviour, ISharingAppObject
    {
        public AppRoleEnum AppRole { get; set; }

        public DeviceTypeEnum DeviceType { get; set; }

        public virtual void OnAppConnect() { }

        public virtual void OnAppInitialize() { }

        public virtual void OnAppSynchronize() { }

        public virtual void OnAppShutDown() { }
    }
}