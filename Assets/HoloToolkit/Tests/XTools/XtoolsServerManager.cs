using UnityEngine;
using System;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.XTools;

public class XtoolsServerManager : Singleton<XtoolsServerManager>
{

    /// <summary>
	/// Normally you'd define a list of all the message types used in your app at the global scope,
	/// but since this is just a test class we define it here.  Note that the first message type has to start
	/// with UserMessageIDStart so as not to conflict with XTools internal messages.  
	/// </summary>
	public enum TestMessageID : byte
    {
        HeadTransform = MessageID.UserMessageIDStart,
        RequestAnchorName,
        RequestAnchorData,
        PostAnchorName,
        PostAnchorData,        
        Max
    }

    /// <summary>
    /// Using the UserMessageChannelStart for sending user messages.
    /// </summary>
    public enum UserMessageChannels
    {
        Anchors = MessageChannel.UserMessageChannelStart,
    }
    
    /// <summary>
    /// Cache the local user's ID to use when sending messages.
    /// </summary>
    public long localUserID
    {
        get; set;
    }

    public delegate void MessageCallback(NetworkInMessage msg);
    private Dictionary<TestMessageID, MessageCallback> _MessageHandlers = new Dictionary<TestMessageID, MessageCallback>();

    public Dictionary<TestMessageID, MessageCallback> MessageHandlers
    {
        get
        {
            return _MessageHandlers;
        }
    }

    /// <summary>
	/// Helper object that we use to route incoming message callbacks to the member
	/// functions of this class
	/// </summary>
	NetworkConnectionAdapter connectionAdapter;

    /// <summary>
    /// Cache the connection object for the session server
    /// </summary>
    NetworkConnection serverConnection;

    void Start()
    {
        InitializeXTools();
    }

    void InitializeXTools()
    {
        XToolsStage xtoolsStage = XToolsStage.Instance;
        if (xtoolsStage != null)
        {           
            this.serverConnection = xtoolsStage.Manager.GetServerConnection();
           
            this.connectionAdapter = new NetworkConnectionAdapter();
            this.connectionAdapter.MessageReceivedCallback += OnMessageReceived;
            
            // Cache the local user ID
            this.localUserID = xtoolsStage.Manager.GetLocalUser().GetID();
           
            for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)TestMessageID.Max; index++)
            {
                Debug.Log(index);
                if (MessageHandlers.ContainsKey((TestMessageID)index) == false)
                {
                    MessageHandlers.Add((TestMessageID)index, null);
                }

                this.serverConnection.AddListener(index, this.connectionAdapter);
            }
        }
    }

    private NetworkOutMessage CreateMessage(byte MessageType)
    {
        NetworkOutMessage msg = serverConnection.CreateMessage(MessageType);
        msg.Write(MessageType);
        // Add the local userID so that the remote clients know whose message they are receiving
        msg.Write(localUserID);
        return msg;
    }
    
    public void SendHeadTransform(Vector3 position, Quaternion rotation)
    {
        // If we are connected to a session, broadcast our head info
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)TestMessageID.HeadTransform);

            AppendTransform(msg, position, rotation);

            msg.Write(DateTime.Now.ToBinary());

            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Immediate,                   // Send immediately, do not buffer
                MessageReliability.UnreliableSequenced,      // Do not retransmit, but don't deliver out of order
                MessageChannel.Avatar);                      // Only order with respect to other messages in the Avatar channel
        }
    }

    public bool SendRequestAnchorName()
    {
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)TestMessageID.RequestAnchorName);

            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Medium,                         // Send immediately, do not buffer
                MessageReliability.ReliableSequenced,           // Do not retransmit, but don't deliver out of order
                (MessageChannel)UserMessageChannels.Anchors);   // Only order with respect to other messages in the Avatar channel

            return true;
        }

        return false;
    }

    public bool SendRequestAnchorData()
    {
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)TestMessageID.RequestAnchorData);

            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Medium,                         // Send immediately, do not buffer
                MessageReliability.ReliableSequenced,           // Do not retransmit, but don't deliver out of order
                (MessageChannel)UserMessageChannels.Anchors);   // Only order with respect to other messages in the Avatar channel

            return true;
        }

        return false;
    }

    public void SendPostAnchorName(string Name)
    {
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)TestMessageID.PostAnchorName);
            msg.Write(new XString(Name));

            Debug.Log("Sending anchor name: " + Name);
            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Medium,                         // Send immediately, do not buffer
                MessageReliability.ReliableSequenced,           // Do not retransmit, but don't deliver out of order
                (MessageChannel)UserMessageChannels.Anchors);   // Only order with respect to other messages in the Avatar channel
        }
    }

    public void SendPostAnchorData(byte[] anchorData)
    {
        if (this.serverConnection != null && this.serverConnection.IsConnected())
        {
            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage msg = CreateMessage((byte)TestMessageID.PostAnchorData);

            if (anchorData != null)
            {
                msg.Write(anchorData.Length);
                msg.WriteArray(anchorData, (uint)anchorData.Length);
            }
            else
            {
                msg.Write(1);
                msg.WriteArray(new byte[1], 1);
            }

            Debug.Log("Sending anchor.");
            // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.  
            this.serverConnection.Broadcast(
                msg,
                MessagePriority.Medium,                         // Send immediately, do not buffer
                MessageReliability.ReliableSequenced,           // Do not retransmit, but don't deliver out of order
                (MessageChannel)UserMessageChannels.Anchors);   // Only order with respect to other messages in the Avatar channel
        }
    }

    void OnDestroy()
    {
        if (this.serverConnection != null)
        {
            for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)TestMessageID.Max; index++)
            {
                this.serverConnection.RemoveListener(index, this.connectionAdapter);
            }
            this.connectionAdapter.MessageReceivedCallback -= OnMessageReceived;
        }
    }

    void OnMessageReceived(NetworkConnection connection, NetworkInMessage msg)
    {
        byte messageType = msg.ReadByte();
        MessageCallback messageHandler = MessageHandlers[(TestMessageID)messageType];
        if (messageHandler != null)
        {
            messageHandler(msg);
        }
    }

    #region HelperFunctionsForWriting
    void AppendTransform(NetworkOutMessage msg, Vector3 position, Quaternion rotation)
    {
        // Pack the head position and rotation into the packet
        AppendVector3(msg, position);
        AppendQuaternion(msg, rotation);
    }

    void AppendVector3(NetworkOutMessage msg, Vector3 vector)
    {
        msg.Write(vector.x);
        msg.Write(vector.y);
        msg.Write(vector.z);
    }

    void AppendQuaternion(NetworkOutMessage msg, Quaternion rotation)
    {
        msg.Write(rotation.x);
        msg.Write(rotation.y);
        msg.Write(rotation.z);
        msg.Write(rotation.w);
    }
    #endregion

    #region HelperFunctionsForReading 
    public Vector3 ReadVector3(NetworkInMessage msg)
    {
        return new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
    }

    public Quaternion ReadQuaternion(NetworkInMessage msg)
    {
        return new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
    }
    #endregion
}
