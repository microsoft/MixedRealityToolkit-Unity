using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl
{
    public class DeviceAssigner : MonoBehaviourSharingApp, IDeviceAssigner
    {
        private IDeviceSource deviceSource;
        private ISystemPrefabPool prefabs;
        private IUserView userView;
        private IAppStateReadWrite appState;

        private Dictionary<short, GameObject> deviceObjects = new Dictionary<short, GameObject>();
        
        private void OnDeviceConnected(short deviceID, string deviceName, bool isLocalDevice, Dictionary<string, string> properties)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    return;

                default:
                    break;
            }

            Debug.Log("OnDeviceConnected on server");

            DeviceTypeEnum deviceType = GetDeviceType(properties);

            UserDeviceState deviceState = new UserDeviceState(deviceID);
            // See if this device already exists in the app state
            if (appState.StateExists<UserDeviceState>(deviceID))
            {
                // If it does, change the connection state
                deviceState = appState.GetState<UserDeviceState>(deviceID);
                deviceState.DeviceType = GetDeviceType(properties);
                deviceState.DeviceName = deviceName;
                deviceState.ConnectionState = DeviceConnectionStateEnum.Connected;
                appState.SetState<UserDeviceState>(deviceState);
            }
            else
            {
                // If it doesn't exist, add a new state
                deviceState.DeviceType = GetDeviceType(properties);
                deviceState.DeviceName = deviceName;
                deviceState.ConnectionState = DeviceConnectionStateEnum.Connected;
                appState.AddState<UserDeviceState>(deviceState);
            }

            // Flush the app state immediately - too important to wait
            appState.Flush();

            if (!deviceObjects.ContainsKey(deviceID))
            {
                // Create a device object for this new device
                GameObject deviceObjectGo = prefabs.InstantiateDevice();
                foreach (IDeviceObject deviceObject in deviceObjectGo.GetComponentsInChildren(typeof(IDeviceObject)))
                {
                    deviceObject.AssignDeviceID(deviceID);
                }
                deviceObjectGo.name = "Connected Device " + deviceID;
                deviceObjects.Add(deviceID, deviceObjectGo);
            }
        }

        private void OnDeviceDisconnected(short deviceID, string deviceName, bool isLocalDevice, Dictionary<string, string> properties)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    return;

                default:
                    break;
            }

            Debug.Log("OnDeviceDisconnected on server");

            UserDeviceState deviceState = appState.GetState<UserDeviceState>(deviceID);
            deviceState.ConnectionState = DeviceConnectionStateEnum.NotConnected;
            appState.SetState<UserDeviceState>(deviceState);
            // Flush the app state immediately - too important to wait
            appState.Flush<UserDeviceState>();
        }

        public void RevokeAssignment(short deviceID)
        {
            Debug.Log("Revoking assignment for device ID " + deviceID);

            UserDeviceState deviceState = appState.GetState<UserDeviceState>(deviceID);

            // No need to revoke anything
            if (deviceState.UserID < 0)
                return;

            UserSlot userSlot = appState.GetState<UserSlot>(deviceState.UserID);
            userSlot.FillState = UserSlot.FillStateEnum.Empty;
            appState.SetState<UserSlot>(userSlot);

            deviceState.UserID = -1;
            appState.SetState<UserDeviceState>(deviceState);

            // Flush the app state immediately - too important to wait
            appState.Flush();
        }
        
        public bool TryAssignDevice(short deviceID, short userID, DeviceRoleEnum deviceRole)
        {
            Debug.Log("Assigning device ID " + deviceID + " to user " + userID);

            int deviceRoleIndex = -1;
            UserSlot userSlot = appState.GetState<UserSlot>(userID);
            for (int i = 0; i < userSlot.DeviceRoles.Length; i++)
            {
                if (userSlot.DeviceRoles[i] == deviceRole)
                {
                    deviceRoleIndex = i;
                    break;
                }
            }

            if (deviceRoleIndex < 0)
                throw new Exception("Couldn't find index for device role " + deviceRole);

            // Make sure no other device is assigned to this role
            foreach (UserDeviceState state in appState.GetStates<UserDeviceState>())
            {
                if (state.DeviceID == deviceID)
                    continue;

                if (state.UserID == userID && state.DeviceRoleIndex == deviceRoleIndex)
                {
                    Debug.Log("Can't assign device " + deviceID + " to user " + userID + " - " + state.DeviceID + " is already assigned.");
                    return false;
                }
            }

            UserDeviceState deviceState = appState.GetState<UserDeviceState>(deviceID);
            deviceState.UserID = userID;
            deviceState.DeviceRoleIndex = (byte)deviceRoleIndex;
            appState.SetState<UserDeviceState>(deviceState);

            // Set the slot state to filled by default
            userSlot.FillState = UserSlot.FillStateEnum.Filled;
            appState.SetState<UserSlot>(userSlot);

            // Flush the app state immediately - too important to wait
            appState.Flush();
            return true;
        }

        public bool GetAssignedUser(short userID, DeviceRoleEnum deviceRole, out short deviceID)
        {
            deviceID = -1;

            UserSlot userSlot = appState.GetState<UserSlot>(userID);

            int deviceRoleIndex = -1;
            for (int i = 0; i < userSlot.DeviceRoles.Length; i++)
            {
                if (userSlot.DeviceRoles[i] == deviceRole)
                {
                    deviceRoleIndex = i;
                    break;
                }
            }

            if (deviceRoleIndex < 0)
                throw new Exception("No device role index found for device role " + deviceRole);

            foreach (UserDeviceState state in appState.GetStates<UserDeviceState>())
            {
                if (!state.IsAssigned)
                    continue;

                if (state.UserID == userID && state.DeviceRoleIndex == deviceRoleIndex)
                {
                    deviceID = state.DeviceID;
                    break;
                }
            }

            return deviceID >= 0;
        }

        private DeviceTypeEnum GetDeviceType(Dictionary<string, string> properties)
        {
            if (properties == null)
                return DeviceTypeEnum.None;

            string deviceTypeString;
            if (properties.TryGetValue("DeviceType", out deviceTypeString))
            {
                DeviceTypeEnum deviceType = DeviceTypeEnum.None;
                if (Enum.TryParse<DeviceTypeEnum>(properties["DeviceType"], out deviceType))
                {
                    return deviceType;
                }
            }

            return DeviceTypeEnum.None;
        }
        
        public override void OnAppInitialize()
        {
            ComponentFinder.FindInScenes<IDeviceSource>(out deviceSource);
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
            ComponentFinder.FindInScenes<ISystemPrefabPool>(out prefabs);

            deviceSource.OnDeviceConnected += OnDeviceConnected;
            deviceSource.OnDeviceDisconnected += OnDeviceDisconnected;
        }
    }
}