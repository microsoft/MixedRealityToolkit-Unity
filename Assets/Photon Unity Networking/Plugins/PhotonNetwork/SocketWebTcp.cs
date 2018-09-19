#if UNITY_WEBGL || UNITY_XBOXONE
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketTcp.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Internal class to encapsulate the network i/o functionality for the realtime libary.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using UnityEngine;
using SupportClassPun = ExitGames.Client.Photon.SupportClass;


namespace ExitGames.Client.Photon
{
    /// <summary>
    /// Internal class to encapsulate the network i/o functionality for the realtime libary.
    /// </summary>
    public class SocketWebTcp : IPhotonSocket, IDisposable
    {
        private WebSocket sock;

        private readonly object syncer = new object();

        public SocketWebTcp(PeerBase npeer) : base(npeer)
        {
            ServerAddress = npeer.ServerAddress;
            if (this.ReportDebugOfLevel(DebugLevel.INFO))
            {
                Listener.DebugReturn(DebugLevel.INFO, "new SocketWebTcp() for Unity. Server: " + ServerAddress);
            }

            this.Protocol = ConnectionProtocol.WebSocket;
            this.PollReceive = false;
        }

        public void Dispose()
        {
            this.State = PhotonSocketState.Disconnecting;

            if (this.sock != null)
            {
                try
                {
                    if (this.sock.Connected) this.sock.Close();
                }
                catch (Exception ex)
                {
                    this.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + ex);
                }
            }

            this.sock = null;
            this.State = PhotonSocketState.Disconnected;
        }

        GameObject websocketConnectionObject;
        public override bool Connect()
        {
            //bool baseOk = base.Connect();
            //if (!baseOk)
            //{
            //    return false;
            //}


            State = PhotonSocketState.Connecting;

            if (this.websocketConnectionObject != null)
            {
                UnityEngine.Object.Destroy(this.websocketConnectionObject);
            }

            this.websocketConnectionObject = new GameObject("websocketConnectionObject");
            MonoBehaviour mb = this.websocketConnectionObject.AddComponent<MonoBehaviourExt>();
            this.websocketConnectionObject.hideFlags = HideFlags.HideInHierarchy;
            UnityEngine.Object.DontDestroyOnLoad(this.websocketConnectionObject);

            this.sock = new WebSocket(new Uri(ServerAddress));
            this.sock.Connect();

            mb.StartCoroutine(this.ReceiveLoop());
            return true;
        }


        public override bool Disconnect()
        {
            if (ReportDebugOfLevel(DebugLevel.INFO))
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "SocketWebTcp.Disconnect()");
            }

            State = PhotonSocketState.Disconnecting;

            lock (this.syncer)
            {
                if (this.sock != null)
                {
                    try
                    {
                        this.sock.Close();
                    }
                    catch (Exception ex)
                    {
                        this.Listener.DebugReturn(DebugLevel.ERROR, "Exception in Disconnect(): " + ex);
                    }
                    this.sock = null;
                }
            }

            if (this.websocketConnectionObject != null)
            {
                UnityEngine.Object.Destroy(this.websocketConnectionObject);
            }

            State = PhotonSocketState.Disconnected;
            return true;
        }

        /// <summary>
        /// used by TPeer*
        /// </summary>
        public override PhotonSocketError Send(byte[] data, int length)
        {
            if (this.State != PhotonSocketState.Connected)
            {
                return PhotonSocketError.Skipped;
            }

            try
            {
                if (this.ReportDebugOfLevel(DebugLevel.ALL))
                {
                    this.Listener.DebugReturn(DebugLevel.ALL, "Sending: " + SupportClassPun.ByteArrayToString(data));
                }
                this.sock.Send(data);
            }
            catch (Exception e)
            {
                this.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send to: " + this.ServerAddress + ". " + e.Message);

                HandleException(StatusCode.Exception);
                return PhotonSocketError.Exception;
            }

            return PhotonSocketError.Success;
        }

        public override PhotonSocketError Receive(out byte[] data)
        {
            data = null;
            return PhotonSocketError.NoData;
        }


        internal const int ALL_HEADER_BYTES = 9;
        internal const int TCP_HEADER_BYTES = 7;
        internal const int MSG_HEADER_BYTES = 2;

        public IEnumerator ReceiveLoop()
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "ReceiveLoop()");
            while (!this.sock.Connected && this.sock.Error == null)
            {
                yield return new WaitForSeconds(0.1f); // while connecting
            }

            if (this.sock.Error != null)
            {
                this.Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread. Server: " + this.ServerAddress + ":" + this.ServerPort + " Error: " + this.sock.Error);
				this.HandleException(StatusCode.ExceptionOnConnect);
            }
            else
            {
                // connected
                if (this.ReportDebugOfLevel(DebugLevel.ALL))
                {
                    this.Listener.DebugReturn(DebugLevel.ALL, "Receiving by websocket. this.State: " + State);
                }
                State = PhotonSocketState.Connected;
				while (State == PhotonSocketState.Connected)
				{
					if (this.sock.Error != null)
					{
						this.Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread (inside loop). Server: "+this.ServerAddress+":"+this.ServerPort+" Error: " + this.sock.Error);
						this.HandleException(StatusCode.ExceptionOnReceive);
						break;
					}
					else
					{
						byte[] inBuff = this.sock.Recv();
						if (inBuff == null || inBuff.Length == 0)
						{
							yield return new WaitForSeconds(0.02f); // nothing received. wait a bit, try again
							continue;
						}

						if (this.ReportDebugOfLevel(DebugLevel.ALL))
						{
							this.Listener.DebugReturn(DebugLevel.ALL, "TCP << " + inBuff.Length + " = " + SupportClassPun.ByteArrayToString(inBuff));
						}

						if (inBuff.Length > 0)
						{
							try
							{
								HandleReceivedDatagram(inBuff, inBuff.Length, false);
							}
							catch (Exception e)
							{
                                if (this.State != PhotonSocketState.Disconnecting && this.State != PhotonSocketState.Disconnected)
                                {
                                    if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                                    {
                                        this.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + this.State + ". Server: '" + this.ServerAddress + "' Exception: " + e);
                                    }

                                    this.HandleException(StatusCode.ExceptionOnReceive);
                                }
							}
						}
					}
				}
            }

            this.Disconnect();
        }
    }

	internal class MonoBehaviourExt : MonoBehaviour {}
}

#endif
