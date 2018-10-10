using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl
{
    public class DevicePool : MixedRealityManager, IMixedRealityManager, IDevicePool
    {
        public string Name { get { return "DevicePool"; } }

        public uint Priority { get { return 0; } }

        public AppRoleEnum AppRole { get; set; }

        public IEnumerable<IUserDevice> Devices
        {
            get
            {
                foreach (IUserDevice device in devices.Values)
                {
                    if (device.DeviceID < 0)
                        continue;

                    yield return device;
                }
                yield break;
            }
        }

        private List<IUserDevice> uninitializedDevices = new List<IUserDevice>();
        private Dictionary<string, IUserDevice> devices = new Dictionary<string, IUserDevice>();
        private Dictionary<string, sbyte> assignments = new Dictionary<string, sbyte>();
        private IUserSlots userSlots;

        public void AddDevice(IUserDevice device)
        {
            uninitializedDevices.Add(device);
        }

        public bool GetAssignedUserSlot(IUserDevice device, out sbyte slotNum)
        {
            if (device == null)
                throw new System.NullReferenceException("Device null when trying to get assigned device slot.");            

            return assignments.TryGetValue(device.DeviceName, out slotNum);
        }

        public void RevokeAssignment(IUserDevice device)
        {
            if (device == null)
                throw new System.NullReferenceException("Device null when trying to revoke assigned device slot.");

            sbyte slotNum = -1;
            if (assignments.TryGetValue(device.DeviceName, out slotNum))
            {
                userSlots.RevokeAssignment(device, slotNum);
                assignments.Remove(device.DeviceName);
            }
        }

        public bool TryAssignDevice(IUserDevice device, sbyte slotNum)
        {
            if (userSlots.TryAssignDeviceToSlot(device, slotNum))
            {
                assignments.Add(device.DeviceName, slotNum);
                return true;
            }

            Debug.LogWarning("Couldn't reserver player slot " + slotNum + " for device " + device.DeviceName);
            return false;
        }

        public void DropDevice(string deviceName)
        {
            throw new NotImplementedException();
        }

        public void OnStateInitialized() { }

        public void OnSharingStop() { }

        public void Update()
        {
            // Initialize our devices
            for (int i = uninitializedDevices.Count - 1; i >= 0; i--)
            {
                if (uninitializedDevices[i] == null)
                {
                    uninitializedDevices.RemoveAt(i);
                }
                else if (uninitializedDevices[i].Initialized)
                {
                    IUserDevice device = uninitializedDevices[i];
                    uninitializedDevices.RemoveAt(i);
                    devices.Add(device.DeviceName, device);
                }
            }
            // Remove uninitialized devices as they're destroyed
            List<string> deadDevices = new List<string>();
            foreach (KeyValuePair<string, IUserDevice> device in devices)
            {
                if (device.Value == null || device.Value.IsDestroyed)
                {
                    deadDevices.Add(device.Key);
                }
                else if (!device.Value.Initialized)
                {
                    deadDevices.Add(device.Key);
                    uninitializedDevices.Add(device.Value);
                }
            }

            foreach (string deadDevice in deadDevices)
            {
                devices.Remove(deadDevice);
            }
        }

        public void OnSharingStart()
        {
            SceneScraper.FindInScenes<IUserSlots>(out userSlots);
        }

        public void Initialize() { }

        public void Reset() { }

        public void Enable() { }

        public void Disable() { }

        public void Destroy() { }
    }
}