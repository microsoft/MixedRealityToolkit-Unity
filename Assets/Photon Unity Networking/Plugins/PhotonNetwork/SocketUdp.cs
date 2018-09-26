// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpSocket.cs" company="Exit Games GmbH">
//   Protocol & Photon Client Lib - Copyright (C) 2013 Exit Games GmbH
// </copyright>
// <summary>
//   Uses the UDP socket for a peer to send and receive enet/Photon messages.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IPHONE && !UNITY_PS3 && !UNITY_WINRT && !UNITY_WP8)

namespace ExitGames.Client.Photon
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Security;
    using System.Threading;

    /// <summary> Internal class to encapsulate the network i/o functionality for the realtime libary.</summary>
    internal class SocketUdp : IPhotonSocket, IDisposable
    {
        private Socket sock;

        private readonly object syncer = new object();

        public SocketUdp(PeerBase npeer) : base(npeer)
        {
            if (this.ReportDebugOfLevel(DebugLevel.ALL))
            {
                this.Listener.DebugReturn(DebugLevel.ALL, "CSharpSocket: UDP, Unity3d.");
            }

            this.Protocol = ConnectionProtocol.Udp;
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

        public override bool Connect()
        {
            lock (this.syncer)
            {
                bool baseOk = base.Connect();
                if (!baseOk)
                {
                    return false;
                }

                this.State = PhotonSocketState.Connecting;

                Thread dns = new Thread(this.DnsAndConnect);
                dns.Name = "photon dns thread";
                dns.IsBackground = true;
                dns.Start();

                return true;
            }
        }

        public override bool Disconnect()
        {
            if (this.ReportDebugOfLevel(DebugLevel.INFO))
            {
                this.EnqueueDebugReturn(DebugLevel.INFO, "CSharpSocket.Disconnect()");
            }

            this.State = PhotonSocketState.Disconnecting;

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
                        this.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
                    }

                    this.sock = null;
                }
            }

            this.State = PhotonSocketState.Disconnected;
            return true;
        }

        /// <summary>used by PhotonPeer*</summary>
        public override PhotonSocketError Send(byte[] data, int length)
        {
            lock (this.syncer)
            {
                if (this.sock == null || !this.sock.Connected)
                {
                    return PhotonSocketError.Skipped;
                }

                try
                {
                    sock.Send(data, 0, length, SocketFlags.None);
                }
                catch (Exception e)
                {
                    if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                    {
                        this.EnqueueDebugReturn(DebugLevel.ERROR, "Cannot send to: " + this.ServerAddress + ". " + e.Message);
                    }
                    return PhotonSocketError.Exception;
                }
            }

            return PhotonSocketError.Success;
        }

        public override PhotonSocketError Receive(out byte[] data)
        {
            data = null;
            return PhotonSocketError.NoData;
        }

        internal void DnsAndConnect()
        {
            IPAddress ipAddress = null;
            try
            {
                lock (this.syncer)
                {
                    ipAddress = IPhotonSocket.GetIpAddress(this.ServerAddress);
                    if (ipAddress == null)
                    {
                        throw new ArgumentException("Invalid IPAddress. Address: " + this.ServerAddress);
                    }
                    if (ipAddress.AddressFamily != AddressFamily.InterNetwork && ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        throw new ArgumentException("AddressFamily '" + ipAddress.AddressFamily + "' not supported. Address: " + this.ServerAddress);
                    }

                    this.sock = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    this.sock.Connect(ipAddress, this.ServerPort);

                    this.AddressResolvedAsIpv6 = (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
                    this.State = PhotonSocketState.Connected;
                    
                    this.peerBase.OnConnect();
                }
            }
            catch (SecurityException se)
            {
                if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    this.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + this.ServerAddress + "' (" + ((ipAddress == null ) ? "": ipAddress.AddressFamily.ToString()) + ") failed: " + se.ToString());
                }

                this.HandleException(StatusCode.SecurityExceptionOnConnect);
                return;
            }
            catch (Exception se)
            {
                if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    this.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + this.ServerAddress + "' (" + ((ipAddress == null) ? "" : ipAddress.AddressFamily.ToString()) + ") failed: " + se.ToString());
                }

                this.HandleException(StatusCode.ExceptionOnConnect);
                return;
            }

            Thread run = new Thread(new ThreadStart(ReceiveLoop));
            run.Name = "photon receive thread";
            run.IsBackground = true;
            run.Start();
        }

        /// <summary>Endless loop, run in Receive Thread.</summary>
        public void ReceiveLoop()
        {
            byte[] inBuffer = new byte[this.MTU];
            while (this.State == PhotonSocketState.Connected)
            {
                try
                {
                    int read = this.sock.Receive(inBuffer);
                    this.HandleReceivedDatagram(inBuffer, read, true);
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
            } //while Connected receive

            // on exit of the receive-loop: disconnect socket
            this.Disconnect();
        }
    } //class

}
#endif
