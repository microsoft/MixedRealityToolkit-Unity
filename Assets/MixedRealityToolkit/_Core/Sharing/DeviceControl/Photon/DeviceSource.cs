using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Pixie.Core;
using UnityEngine;

namespace Pixie.DeviceControl.Photon
{
    public class DeviceSource : MonoBehaviourPunCallbacks, IDeviceSource, IPunObservable
    {
        public AppRoleEnum AppRole { get; set; }
        public DeviceTypeEnum DeviceType { get; set; }

        public short LocalDeviceID
        {
            get
            {
                short deviceID = -1;
                if (!deviceIDLookup.TryGetValue(PhotonNetwork.LocalPlayer.UserId, out deviceID))
                    return -1;

                return deviceID;
            }
        }
        
        public bool LocalDeviceConnected { get { return LocalDeviceID >= 0; } }

        public IEnumerable<short> ConnectedDevices
        {
            get
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    short deviceID = -1;
                    if (deviceIDLookup.TryGetValue(player.UserId, out deviceID))
                        yield return deviceID;
                }
            }
        }

        public event DeviceEventDelegate OnDeviceConnected;
        public event DeviceEventDelegate OnDeviceDisconnected;

        private Dictionary<string, short> deviceIDLookup = new Dictionary<string, short>();
        private short nextDeviceID = 1;
        private object[] rpcArgs = new object[4];

        public void PingDevice(short deviceID)
        {
            // Find the user id associated with this device ID
            string userID = string.Empty;
            foreach (KeyValuePair<string, short> device in deviceIDLookup)
            {
                if (device.Value == deviceID)
                {
                    userID = device.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(userID))
                Debug.LogError("Couldn't ping deviceID " + deviceID + " - no accompanying userID found.");

            Player targetPlayer = null;
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                if (player.UserId == userID)
                {
                    targetPlayer = player;
                    break;
                }
            }

            if (targetPlayer == null)
                Debug.LogError("Couldn't ping deviceID " + deviceID + " - no accompanying player found.");

            photonView.RPC("PingTargetDevice", targetPlayer);
        }

        public void DisconnectDevice(short deviceID)
        {
            foreach (KeyValuePair<string, short> device in deviceIDLookup)
            {
                if (device.Value == deviceID)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    // Let the server handle this
                    return;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(newPlayer.UserId))
                throw new Exception("User id is empty - set RoomOptions.PublishUserID to true");

            short deviceID = -1;
            // It's the same player joining again - no need to generate a new ID
            if (!deviceIDLookup.TryGetValue(newPlayer.UserId, out deviceID))
            {
                // Otherwise, generate a new ID and add it to the lookup
                deviceID = nextDeviceID;
                nextDeviceID++;
            }

            Dictionary<string, string> deviceProperties = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in newPlayer.CustomProperties)
                deviceProperties.Add(entry.Key.ToString(), entry.Value.ToString());

            Debug.Log("Device " + deviceID + " has connected.");

            rpcArgs[0] = newPlayer.UserId;
            rpcArgs[1] = string.IsNullOrEmpty(newPlayer.NickName) ? "Device " + deviceID : newPlayer.NickName;
            rpcArgs[2] = deviceID;
            rpcArgs[3] = deviceProperties;

            // Let everyone else know that a device has been added
            // This will invoke OnDeviceConnected for everyone including server
            photonView.RPC("AddDevice", RpcTarget.AllBufferedViaServer, rpcArgs);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    // Let the server handle this
                    return;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(otherPlayer.UserId))
                throw new Exception("User id is empty - set RoomOptions.PublishUserID to true");

            short deviceID = -1;
            if (!deviceIDLookup.TryGetValue(otherPlayer.UserId, out deviceID))
                throw new Exception("User id has no associated device ID - this should never happen!");

            Dictionary<string, string> deviceProperties = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in otherPlayer.CustomProperties)
                deviceProperties.Add(entry.Key.ToString(), entry.Value.ToString());

            Debug.Log("Device " + deviceID + " has disconnected.");

            rpcArgs[0] = otherPlayer.UserId;
            rpcArgs[1] = string.IsNullOrEmpty(otherPlayer.NickName) ? "Device " + deviceID : otherPlayer.NickName;
            rpcArgs[2] = deviceID;
            rpcArgs[3] = deviceProperties;

            // Let everyone else know that a device has been added
            // This will invoke OnDeviceDisconnected for everyone including server
            photonView.RPC("RemoveDevice", RpcTarget.AllBufferedViaServer, rpcArgs);
        }

        [PunRPC]
        private void AddDevice(string userID, string deviceName, short deviceID, Dictionary<string, string> deviceProperties)
        {
            Debug.Log("Added device: " + userID + ", " + deviceID);

            deviceIDLookup.Add(userID, deviceID);

            bool isLocalDevice = (PhotonNetwork.LocalPlayer.UserId == userID);

            OnDeviceConnected(deviceID, deviceName, isLocalDevice, deviceProperties);
        }

        [PunRPC]
        private void RemoveDevice(string userID, string deviceName, short deviceID, Dictionary<string, string> deviceProperties)
        {
            Debug.Log("removed device: " + userID + ", " + deviceID);

            deviceIDLookup.Remove(userID);

            bool isLocalDevice = (PhotonNetwork.LocalPlayer.UserId == userID);

            OnDeviceConnected(deviceID, deviceName, isLocalDevice, deviceProperties);
        }

        [PunRPC]
        private void PingTargetDevice()
        {
            IDevicePing ping = (IDevicePing)gameObject.GetComponent(typeof(IDevicePing));

            if (ping == null)
            {
                // If we don't have a ping component, just debug a ping
                Debug.Log("PING");
                return;
            }

            ping.Ping();
        }
        
        public void OnAppInitialize() { }

        public void OnAppConnect() { }

        public void OnAppShutDown() { }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(DeviceSource))]
        public class DeviceSourceEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                DeviceSource ds = (DeviceSource)target;

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField("Devices:");

                foreach (KeyValuePair<string, short> deviceID in ds.deviceIDLookup)
                {
                    GUI.color = Color.white;

                    if (ds.LocalDeviceID == deviceID.Value)
                        GUI.color = Color.green;

                    GUILayout.Button("Device " + deviceID.Value + " (" + deviceID.Key + ")");
                }

                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}