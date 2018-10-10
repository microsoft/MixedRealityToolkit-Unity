using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    /// <summary>
    /// Pipe used to send / receive data from clients
    /// This should be use SPARINGLY
    /// It's much more efficient to change a state by means of an ACTION sent via command
    /// </summary>
    public class AppStatePipe : NetworkBehaviour, IStatePipeInput, IStatePipeOutput
    {
        public bool Sending
        {
            get { return sending; }
        }

        public Queue<object> StatesReceived
        {
            get { return statesReceived; }
        }

        [SerializeField]
        private bool sending = true;
        private Queue<object> statesReceived = new Queue<object>();
        private Queue<object> statesToSend = new Queue<object>();
                
        [Client]
        public void SendStates(IEnumerable<object> objects)
        {
            foreach (object obj in objects)
            {
                Debug.Log("Sending " + obj.ToString());
                statesToSend.Enqueue(obj);
            }
        }

        [TargetRpc]
        [Command(channel = Globals.UNet.ChannelAllCosts)]
        protected void Target_StatesReceived(NetworkConnection conn)
        {
            Debug.Log("Target_StatesReceived in " + name);

            sending = false;
        }

        /// <summary>
        /// Unity won't call the correct TargetRpc when multiple scripts with
        /// the same function name exist on one object.
        /// </summary>
        [Server]
        protected void OnServerReceiveStates()
        {
            Debug.Log("OnServerReceiveStates in " + name);

            Target_StatesReceived(connectionToClient);
        }

        [Command(channel = Globals.UNet.ChannelReliableFragmented)]
        protected void Cmd_ReceiveStates(byte[] stateQueueBytes)
        {
            try
            {
                if (statesReceived.Count > 0)
                {
                    foreach (object state in DeserializeStateQueue(stateQueueBytes))
                    {
                        statesReceived.Enqueue(state);
                    }
                }
                else
                {
                    statesReceived = DeserializeStateQueue(stateQueueBytes);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error when attempting to deserialize state queue bytes!");
                Debug.LogException(e);
            }

            OnServerReceiveStates();
        }

        private void LateUpdate()
        {
            if (isServer)
                return;

            if (sending)
                return;

            if (statesToSend.Count > 0)
            {
                Debug.Log("Found states to send, sending now");
                sending = true;
                byte[] stateQueueBytes = SerializeStateQueue(statesToSend);
                Cmd_ReceiveStates(stateQueueBytes);
                statesToSend.Clear();
            }
        }

        private static byte[] SerializeStateQueue(Queue<object> states)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, states);
                return stream.GetBuffer();
            }
        }

        private static Queue<object> DeserializeStateQueue(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as Queue<object>;
            }
        }
    }
}