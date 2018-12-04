using UnityEngine;

namespace Pixie.DeviceControl
{
    public class DevicePing : MonoBehaviour, IDevicePing
    {
        public void Ping()
        {
            source.PlayOneShot(clip);
        }

        [SerializeField]
        private AudioClip clip;
        [SerializeField]
        private AudioSource source;
    }
}