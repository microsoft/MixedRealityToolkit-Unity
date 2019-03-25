using UnityEngine;
using Photon.Pun;
using MRTK.Core;
using Photon.Realtime;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace MRTK.Networking
{
    public class PhotonServiceConnector : MonoBehaviourPunCallbacks
    {
        public bool Connected { get { return connected; } }
        public bool JoinedExperience { get { return joinedRoom; } }

        private AppRoleEnum appRole = AppRoleEnum.Server;
        private bool connecting = false;
        private bool connected = false;
        private bool connectedToMaster = false;
        private bool joinedRoom = false;

        public void ConnectToService(AppRoleEnum appRole, string experienceName, IEnumerable<ISharingAppObject> sharingAppObjects)
        {
            if (connecting || connected)
                throw new Exception("Already connected or trying to connect.");

            this.appRole = appRole;
            connecting = true;
            StartCoroutine(ConnectToService(experienceName, sharingAppObjects));
        }

        public override void OnConnected()
        {
            connected = true;
        }

        public override void OnConnectedToMaster()
        {
            connectedToMaster = true;
        }

        public override void OnJoinedRoom()
        {
            joinedRoom = true;
        }

        private IEnumerator ConnectToService(string experienceName, IEnumerable<ISharingAppObject> sharingAppObjects)
        {
            if (!PhotonNetwork.ConnectUsingSettings())
                Debug.LogError("Couldn't connect using photon settings.");

            switch (appRole)
            {
                case AppRoleEnum.Client:
                    while (!connectedToMaster)
                        yield return new WaitForSeconds(0.5f);

                    if (!PhotonNetwork.JoinRoom(experienceName))
                        Debug.LogError("Couldn't connect to room.");
                    break;

                case AppRoleEnum.Server:
                    while (!connected)
                        yield return new WaitForSeconds(0.5f);

                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.PublishUserId = true;
                    TypedLobby typedLobby = new TypedLobby(SceneManager.GetActiveScene().name, LobbyType.Default);
                    if (!PhotonNetwork.CreateRoom(experienceName, roomOptions, typedLobby))
                        Debug.LogError("Couldn't connect to room.");
                    break;
            }

            while (!joinedRoom)
                yield return null;

            foreach (ISharingAppObject sharingAppObject in sharingAppObjects)
                sharingAppObject.OnAppConnect();

            bool readyToSynchronize = false;
            while (!readyToSynchronize)
            {
                readyToSynchronize = true;
                foreach (ISharingAppObject sharingAppObject in sharingAppObjects)
                    readyToSynchronize &= sharingAppObject.ReadyToSynchronize;
                yield return null;
            }

            foreach (ISharingAppObject sharingAppObject in sharingAppObjects)
                sharingAppObject.OnAppSynchronize();

            connecting = false;
        }
    }
}