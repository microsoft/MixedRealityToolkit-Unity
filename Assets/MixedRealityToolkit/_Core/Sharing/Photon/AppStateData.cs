using Photon.Pun;
using MRTK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
#if BINARY_SERIALIZATION
using System.Runtime.Serialization.Formatters.Binary;
#else
using System.Text;
#endif
using UnityEngine;
using Photon.Realtime;
using MRTK.Networking;
using Microsoft.MixedReality.Toolkit;

namespace MRTK.StateControl.Photon
{
    [RequireComponent(typeof(AppStateDataSubscriptions))]
    [ExecuteAlways]
    public class AppStateData : MonoBehaviour, IAppStateData, IPunObservable, IStatePipe
    {
        public AppRoleEnum AppRole { get; private set; }
        public Action<Type, List<object>> OnReceiveChangedStates { get; set; }
        public bool Synchronized { get { return synchronized; } }
        public bool ReadyToSynchronize { get { return true; } }

        // Subscription source
        private AppStateDataSubscriptions subscriptions;
        // List of all state arrays being used
        private List<IStateArrayBase> stateList = new List<IStateArrayBase>();
        // Lookup of state array by type
        private Dictionary<Type, IStateArrayBase> stateLookupByType = new Dictionary<Type, IStateArrayBase>();
        private Dictionary<string, IStateArrayBase> stateLookupByName = new Dictionary<string, IStateArrayBase>();
        private HashSet<Player> targetsRequestingSync = new HashSet<Player>();
        private bool synchronized;
        // Profile sent by service
        private AppStateProfile profile;
        // Photon-specific components
        private PhotonView photonView;
        private PhotonServiceConnector connector;
        private object[] rpcMethodArgs = new object[2];

        private void OnEnable()
        {
            SetUpPhotonComponents();
        }

        public bool ContainsStateType(Type stateType)
        {
            return stateLookupByType.ContainsKey(stateType);
        }

        public void CreateStateArray(Type type)
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
                object newStateArray = Activator.CreateInstance(stateArrayType, new object[] { this });

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
                // Send for all players that are subscribed to this type
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (subscriptions.IsUserSubscribedToType(player.UserId, stateArrayType))
                        photonView.RPC("ReceiveFlushedStates", player, rpcMethodArgs);
                }
            }
            else
            {
                Debug.LogWarning("Not connected - simulating flush call locally.");
                // If we're not connected, then flush the states locally
                ReceiveFlushedStates(stateArrayType.FullName, flushedStateBytes);
            }
        }

        [PunRPC]
        public void RequestSynchronizedStates(PhotonMessageInfo messageInfo)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                    break;

                default:
                    throw new Exception("This call should only be received by server.");
            }

            if (!targetsRequestingSync.Add(messageInfo.Sender))
                throw new Exception("Target has already requested synchronization.");

            StartCoroutine(SynchronizeWithTargetOverTime(messageInfo.Sender));
        }

        [PunRPC]
        public void SetSynchronized(bool synchronized)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    break;

                default:
                    throw new Exception("This call should only be received by client.");
            }

            this.synchronized = synchronized;
        }

        [PunRPC]
        public void ReceiveSynchronizedStates(string stateArrayTypeName, byte[] synchronizedStates)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    break;

                default:
                    throw new Exception("This call should only be received by client.");
            }

            IStateArrayBase stateArray;
            if (!stateLookupByName.TryGetValue(stateArrayTypeName, out stateArray))
                Debug.LogError("Received flushed states for state array that doesn't exist: " + stateArrayTypeName);

            List<object> states = DeserializeStateList(synchronizedStates, stateArray.StateType);

            Debug.Log("Received synchronized states for type " + stateArrayTypeName);

            stateArray.ReceiveSynchronizedStates(states);
        }

        [PunRPC]
        public void ReceiveFlushedStates(string stateArrayTypeName, byte[] flushedStatesBytes)
        {
            IStateArrayBase stateArray;
            if (!stateLookupByName.TryGetValue(stateArrayTypeName, out stateArray))
                Debug.LogError("Received flushed states for state array that doesn't exist: " + stateArrayTypeName);

            List<object> states = DeserializeStateList(flushedStatesBytes, stateArray.StateType);

            stateArray.ReceiveFlushedStates(states);

            if (OnReceiveChangedStates != null)
                OnReceiveChangedStates(stateArray.StateType, states);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        public void OnServiceInitialized(AppStateProfile profile, List<ISharingAppObject> sharingAppObjects)
        {
            SetUpPhotonComponents();

            // Make sure we're in the list of sharing app objects
            if (!sharingAppObjects.Contains(this))
                sharingAppObjects.Add(this);

            this.profile = profile;
            AppRole = profile.AppRole;

            switch (AppRole)
            {
                case AppRoleEnum.Server:
                    subscriptions.SetLocalSubscriptionMode(SubscriptionModeEnum.All);
                    break;

                case AppRoleEnum.Client:
                    subscriptions.SetLocalSubscriptionMode(profile.SubscriptionMode, profile.SubscribedTypes);
                    break;
            }

            if (!Application.isPlaying)
                return;

            connector.ConnectToService(profile.AppRole, profile.ExperienceName, sharingAppObjects);
        }

        public void OnAppConnect() { }

        public void OnAppSynchronize()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Server:
                    // Server is synchronized by default
                    synchronized = true;
                    return;

                default:
                    break;
            }

            Debug.Log("Requesting synchronized states from server...");
            // Ask the server to send us synchronized states
            photonView.RPC("RequestSynchronizedStates", RpcTarget.MasterClient);
        }

        private void SetUpPhotonComponents()
        {
            subscriptions = gameObject.EnsureComponent<AppStateDataSubscriptions>();
            photonView = gameObject.EnsureComponent<PhotonView>();
            connector = gameObject.EnsureComponent<PhotonServiceConnector>();

            photonView.ObservedComponents = new List<Component>();
            photonView.ObservedComponents.Add(this);
            photonView.ObservedComponents.Add(subscriptions);
            photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
        }

        private IEnumerator SynchronizeWithTargetOverTime(Player target)
        {
            List<object> states = new List<object>();
            foreach (KeyValuePair<string,IStateArrayBase> stateArray in stateLookupByName)
            {
                // If this target isn't subscribed to this type, skip it
                if (!subscriptions.IsUserSubscribedToType(target.UserId, stateArray.Value.StateType))
                    continue;

                //Debug.Log("Sending synced states for state array " + stateArray.Key + " to target " + target.ActorNumber);

                states.Clear();
                foreach (object state in stateArray.Value.GetStates())
                    states.Add(state);

                byte[] synchronizedStateBytes = SerializeStateList(states);

                photonView.RPC("ReceiveSynchronizedStates", target, new object[] { stateArray.Key, synchronizedStateBytes });

                // TODO get rid of magic number
                yield return new WaitForSeconds(0.15f);
            }

            //Debug.Log("Target " + target.ActorNumber + " is synchronized");

            // Tell the recipient they are synced up
            photonView.RPC("SetSynchronized", target, new object[] { true });
            // Remove from our list of targets
            targetsRequestingSync.Remove(target);
            yield break;            
        }

        private static byte[] SerializeStateList(List<object> states)
        {
#if BINARY_SERIALIZATION
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, states);
                return stream.GetBuffer();
            }
#else
            JsonData jsonData = new JsonData();
            for (int i = 0; i < states.Count; i++)
            {
                jsonData.Data.Add(JsonUtility.ToJson(states[i]));
            }
            string statesAsString = JsonUtility.ToJson(jsonData);
            return Encoding.ASCII.GetBytes(statesAsString);
#endif
        }

        private static List<object> DeserializeStateList(byte[] bytes, Type stateType)
        {
#if BINARY_SERIALIZATION
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as List<object>;
            }
#else
            string statesAsString = Encoding.ASCII.GetString(bytes);
            // This will convert the string to a list of JSON strings
            JsonData jsonData = JsonUtility.FromJson<JsonData>(statesAsString);
            List<object> stateObjects = new List<object>();
            // Before returning them, convert them to system object
            for (int i = 0; i < jsonData.Data.Count; i++)
            {
                stateObjects.Add(JsonUtility.FromJson(jsonData.Data[i], stateType));
            }
            return stateObjects;
#endif
        }
    }

#if !BINARY_SERIALIZATION
    /// <summary>
    /// Helper class for Unity's JsonUtility.
    /// Utility can't serialize a list of strings unless nested in another class.
    /// </summary>
    [Serializable]
    public class JsonData
    {
        public List<string> Data = new List<string>();
    }
#endif
}