using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl
{
    public class UserDevice : NetworkBehaviour, IUserDevice
    {
        private static short currentDeviceID = 100;

        public short DeviceID { get { return deviceID; } }

        public bool Initialized { get { return !string.IsNullOrEmpty(deviceName); } }

        public bool IsDestroyed { get { return isDestroyed; } }

        public RuntimePlatform DevicePlatform { get { return devicePlatform; } }
        public UserDeviceEnum DeviceType { get { return deviceType; } }

        public string DeviceName { get { return deviceName; } }

        [SyncVar]
        [SerializeField]
        private RuntimePlatform devicePlatform = RuntimePlatform.WindowsEditor;

        [SyncVar]
        [SerializeField]
        private UserDeviceEnum deviceType = UserDeviceEnum.Unknown;

        [SyncVar]
        [SerializeField]
        private string deviceName = string.Empty;

        [SyncVar]
        [SerializeField]
        private short deviceID = -1;

        private bool isDestroyed;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            devicePlatform = Application.platform;
            deviceName = SystemInfo.deviceName;
            deviceType = UserDeviceEnum.Unknown;
            deviceID = GetNextDeviceID();

            Cmd_UpdateDeviceInfo(devicePlatform, deviceType, deviceName, deviceID);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            IDevicePool devicePool;
            SceneScraper.FindInScenes<IDevicePool>(out devicePool);
            devicePool.AddDevice(this);
        }

        private void OnDestroy()
        {
            isDestroyed = true;
        }

        [Command]
        private void Cmd_UpdateDeviceInfo (
            RuntimePlatform devicePlatform,
            UserDeviceEnum deviceType,
            string deviceName, 
            short deviceID)
        {
            Debug.Log("Cmd_UpdateDeviceInfo - device type: " + deviceType + ", device name: " + deviceName);

            this.devicePlatform = devicePlatform;
            this.deviceType = deviceType;
            this.deviceName = deviceName;
            this.deviceID = deviceID;
        }

        private static short GetNextDeviceID()
        {
            currentDeviceID++;
            return currentDeviceID;
        }
    }
}