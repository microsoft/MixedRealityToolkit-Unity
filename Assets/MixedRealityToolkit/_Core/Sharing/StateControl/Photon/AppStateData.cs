using Photon.Pun;
using Pixie.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Pixie.StateControl.Photon
{
    public class AppStateData : MonoBehaviour, IAppStateData, IPunObservable, IStatePipe
    {
        public Action<Type, List<object>> OnReceiveChangedStates { get; set; }

        public bool Synchronized
        {
            get { return synchronized; }
            set
            {
                photonView.RPC("SetSynchronized", RpcTarget.AllBufferedViaServer, new object[] { value });
            }
        }

        // List of all state arrays being used
        private List<IStateArrayBase> stateList = new List<IStateArrayBase>();
        // Lookup of state array by type
        private Dictionary<Type, IStateArrayBase> stateLookupByType = new Dictionary<Type, IStateArrayBase>();
        private Dictionary<string, IStateArrayBase> stateLookupByName = new Dictionary<string, IStateArrayBase>();
        private bool synchronized;
        // Photon-specific component
        private PhotonView photonView;
        private object[] rpcMethodArgs = new object[2];

        public bool ContainsStateType(Type stateType)
        {
            return stateLookupByType.ContainsKey(stateType);
        }

        public void CreateStateArray(Type type, AppRoleEnum appRole)
        {
            if (stateLookupByType.ContainsKey(type))
                throw new Exception("App state data already contains state array of type " + type.Name);

            photonView = GetComponent<PhotonView>();

            if (photonView == null)
                throw new Exception("No photon view component found on app state data!");

            try
            {
                // Create a state array type from the generic base type
                Type stateArrayGenericType = typeof(StateArray<>);
                Type[] typeArgs = new Type[] { type };
                Type stateArrayType = stateArrayGenericType.MakeGenericType(typeArgs);
                object newStateArray = Activator.CreateInstance(stateArrayType, new object[] { this, appRole });

                // Get the state array base interface and store it locally
                IStateArrayBase stateArrayBase = newStateArray as IStateArrayBase;
                stateList.Add(stateArrayBase);
                stateLookupByName.Add(type.FullName, stateArrayBase);
                stateLookupByType.Add(type, stateArrayBase);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception("Error when attempting to create state array of type " + type.Name);
            }
        }

        public bool TryGetData(Type type, out IStateArrayBase stateArray)
        {
            return stateLookupByType.TryGetValue(type, out stateArray);
        }

        public bool TryGetData<T>(out IStateArray<T> stateArray) where T : struct, IItemState, IItemStateComparer<T>
        {
            stateArray = null;

            IStateArrayBase stateArrayBase;
            if (stateLookupByType.TryGetValue(typeof(T), out stateArrayBase))
            {
                stateArray = stateArrayBase as IStateArray<T>;
            }

            return stateArray != null;
        }

        public IEnumerator<IStateArrayBase> GetEnumerator()
        {
            return stateList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return stateList.GetEnumerator();
        }

        public void SendFlushedStates(Type stateArrayType, List<object> flushedStates)
        {
            //Debug.Log("Sending " + flushedStates.Count + " states of type " + stateArrayType.FullName);

            byte[] flushedStateBytes = SerializeStateList(flushedStates);

            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                // Call the RPC - this will force the list to update
                rpcMethodArgs[0] = stateArrayType.FullName;
                rpcMethodArgs[1] = flushedStateBytes;
                photonView.RPC("ReceiveFlushedStates", RpcTarget.AllBuffered, rpcMethodArgs);
            }
            else
            {
                Debug.LogWarning("Not connected - simulating flush call locally.");
                // If we're not connected, then flush the states locally
                ReceiveFlushedStates(stateArrayType.FullName, flushedStateBytes);
            }
        }

        [PunRPC]
        public void SetSynchronized(bool synchronized)
        {
            this.synchronized = synchronized;
        }

        [PunRPC]
        public void ReceiveFlushedStates(string stateArrayTypeName, byte[] flushedStatesBytes)
        {
            List<object> flushedStates = DeserializeStateList(flushedStatesBytes);

            IStateArrayBase stateArray;
            if (!stateLookupByName.TryGetValue(stateArrayTypeName, out stateArray))
                Debug.LogError("Received flushed states for state array that doesn't exist: " + stateArrayTypeName);

            stateArray.ReceiveFlushedStates(flushedStates);
            
            if (OnReceiveChangedStates != null)
                OnReceiveChangedStates(stateArray.StateType, flushedStates);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        private static byte[] SerializeStateList(List<object> states)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, states);
                return stream.GetBuffer();
            }
        }

        private static List<object> DeserializeStateList(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as List<object>;
            }
        }
    }
}