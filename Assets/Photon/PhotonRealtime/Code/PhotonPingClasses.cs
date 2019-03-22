// ----------------------------------------------------------------------------
// <copyright file="PhotonPingClasses.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Provides implementations of the PhotonPing for various platforms and
//   use cases.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif

#if SUPPORTED_UNITY
namespace Photon.Realtime
{
    using System;
    using System.Net.Sockets;
    using ExitGames.Client.Photon;

    #if UNITY_WEBGL
    // import WWW class
    using UnityEngine;
    #endif

    #if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS && !UNITY_PS3 && !UNITY_WINRT)
    /// <summary>Uses C# Socket class from System.Net.Sockets (as Unity usually does).</summary>
    /// <remarks>Incompatible with Windows 8 Store/Phone API.</remarks>
    public class PingMonoEditor : PhotonPing
    {
        private Socket sock;

        /// <summary>
        /// Sends a "Photon Ping" to a server.
        /// </summary>
        /// <param name="ip">Address in IPv4 or IPv6 format. An address containing a '.' will be interpretet as IPv4.</param>
        /// <returns>True if the Photon Ping could be sent.</returns>
        public override bool StartPing(string ip)
        {
            base.Init();

            try
            {
                if (ip.Contains("."))
                {
                    this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                else
                {
                    this.sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                }

                this.sock.ReceiveTimeout = 5000;
                this.sock.Connect(ip, 5055);

                this.PingBytes[this.PingBytes.Length - 1] = this.PingId;
                this.sock.Send(this.PingBytes);
                this.PingBytes[this.PingBytes.Length - 1] = (byte)(this.PingId+1);  // invalidate the result, as we re-use the buffer
            }
            catch (Exception e)
            {
                this.sock = null;
                Console.WriteLine(e);
            }

            return false;
        }

        public override bool Done()
        {
            if (this.GotResult || this.sock == null)
            {
                return true;
            }

            if (this.sock.Available <= 0)
            {
                return false;
            }

            int read = this.sock.Receive(this.PingBytes, SocketFlags.None);
            bool replyMatch = this.PingBytes[this.PingBytes.Length - 1] == this.PingId && read == this.PingLength;

            this.Successful = replyMatch;
            this.GotResult = true;
            return true;
        }

        public override void Dispose()
        {
            try
            {
                this.sock.Close();
            }
            catch
            {
            }
            this.sock = null;
        }

    }
    #endif


    #if UNITY_WEBGL
    public class PingHttp : PhotonPing
    {
        private WWW webRequest;

        public override bool StartPing(string address)
        {
            base.Init();

            address = "https://" + address + "/photon/m/?ping&r=" + UnityEngine.Random.Range(0, 10000);
            this.webRequest = new WWW(address);
            return true;
        }

        public override bool Done()
        {
            if (this.webRequest.isDone)
            {
                Successful = true;
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
            this.webRequest.Dispose();
        }
    }
    #endif
}
#endif