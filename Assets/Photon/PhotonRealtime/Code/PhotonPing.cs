using System;
using System.Collections;
using System.Threading;

#if NETFX_CORE
using System.Diagnostics;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

#if !NO_SOCKET && !NETFX_CORE
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
#endif


namespace Photon.Realtime
{

    public abstract class PhotonPing : IDisposable
    {
        public string DebugString = "";
        public bool Successful;

        protected internal bool GotResult;

        protected internal int PingLength = 13;

        protected internal byte[] PingBytes = new byte[] { 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x00 };

        protected internal byte PingId;

        public virtual bool StartPing(string ip)
        {
            throw new NotImplementedException();
        }

        public virtual bool Done()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        protected internal void Init()
        {
            this.GotResult = false;
            this.Successful = false;
            PingId = (byte) (Environment.TickCount%255);
        }
    }


    #if !NETFX_CORE && !NO_SOCKET
    /// <summary>Uses C# Socket class from System.Net.Sockets (as Unity usually does).</summary>
    /// <remarks>Incompatible with Windows 8 Store/Phone API.</remarks>
    public class PingMono : PhotonPing
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

                sock.ReceiveTimeout = 5000;
                sock.Connect(ip, 5055);

                PingBytes[PingBytes.Length - 1] = PingId;
                sock.Send(PingBytes);
                PingBytes[PingBytes.Length - 1] = (byte)(PingId - 1);
            }
            catch (Exception e)
            {
                sock = null;
                Console.WriteLine(e);
            }

            return false;
        }

        public override bool Done()
        {
            if (this.GotResult || sock == null)
            {
                return true;
            }

            if (sock.Available <= 0)
            {
                return false;
            }

            int read = sock.Receive(PingBytes, SocketFlags.None);

            bool replyMatch = PingBytes[PingBytes.Length - 1] == PingId && read == PingLength;
            if (!replyMatch) this.DebugString += " ReplyMatch is false! ";


            this.Successful = read == PingBytes.Length && PingBytes[PingBytes.Length - 1] == PingId;
            this.GotResult = true;
            return true;
        }

        public override void Dispose()
        {
            try
            {
                sock.Close();
            }
            catch
            {
            }
            sock = null;
        }

    }
    #endif


    #if NETFX_CORE
    /// <summary>Windows store API implementation of PhotonPing</summary>
    public class PingWindowsStore : PhotonPing
    {
        private DatagramSocket sock;
        private readonly object syncer = new object();

        public override bool StartPing(string host)
        {
            base.Init();

            EndpointPair endPoint = new EndpointPair(null, string.Empty, new HostName(host), "5055");
            this.sock = new DatagramSocket();
            this.sock.MessageReceived += OnMessageReceived;

            var result = this.sock.ConnectAsync(endPoint);
            result.Completed = this.OnConnected;
            this.DebugString += " End StartPing";
            return true;
        }

        public override bool Done()
        {
            return this.GotResult;
        }

        public override void Dispose()
        {
            this.sock = null;
        }

        private void OnConnected(IAsyncAction asyncinfo, AsyncStatus asyncstatus)
        {
            if (asyncinfo.AsTask().IsCompleted)
            {
                PingBytes[PingBytes.Length - 1] = PingId;

                DataWriter writer;
                writer = new DataWriter(sock.OutputStream);
                writer.WriteBytes(PingBytes);
                var res = writer.StoreAsync();
                res.AsTask().Wait(100);

                writer.DetachStream();
                writer.Dispose();

                PingBytes[PingBytes.Length - 1] = (byte)(PingId - 1);
            }
            else
            {
                // TODO: handle error
            }
        }

        private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            lock (syncer)
            {
                DataReader reader = null;
                try
                {
                    reader = args.GetDataReader();
                    uint receivedByteCount = reader.UnconsumedBufferLength;
                    if (receivedByteCount > 0)
                    {
                        var resultBytes = new byte[receivedByteCount];
                        reader.ReadBytes(resultBytes);

                        //TODO: check result bytes!


                        this.Successful = receivedByteCount == PingLength && resultBytes[resultBytes.Length - 1] == PingId;
                        this.GotResult = true;

                    }
                }
                catch
                {
                    // TODO: handle error
                }
            }
        }
    }
    #endif


    #if NATIVE_SOCKETS
	/// <summary>Abstract base class to provide proper resource management for the below native ping implementations</summary>
	public abstract class PingNative : PhotonPing
	{
		// Native socket states - according to EnetConnect.h state definitions
		protected enum NativeSocketState : byte
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 2,
			ConnectionError = 3,
			SendError = 4,
			ReceiveError = 5,
			Disconnecting = 6
		}

		protected IntPtr pConnectionHandler = IntPtr.Zero;

		~PingNative()
		{
			Dispose();
		}
	}

    /// <summary>Uses dynamic linked native Photon socket library via DllImport("PhotonSocketPlugin") attribute (as done by Unity Android and Unity PS3).</summary>
    public class PingNativeDynamic : PingNative
    {
        public override bool StartPing(string ip)
        {
            lock (SocketUdpNativeDynamic.syncer)
            {
                base.Init();

				if(pConnectionHandler == IntPtr.Zero)
				{
					pConnectionHandler = SocketUdpNativeDynamic.egconnect(ip);
					SocketUdpNativeDynamic.egservice(pConnectionHandler);
					byte state = SocketUdpNativeDynamic.eggetState(pConnectionHandler);
					while (state == (byte) NativeSocketState.Connecting)
					{
						SocketUdpNativeDynamic.egservice(pConnectionHandler);
						state = SocketUdpNativeDynamic.eggetState(pConnectionHandler);
					}
				}

                PingBytes[PingBytes.Length - 1] = PingId;
                SocketUdpNativeDynamic.egsend(pConnectionHandler, PingBytes, PingBytes.Length);
                SocketUdpNativeDynamic.egservice(pConnectionHandler);

                PingBytes[PingBytes.Length - 1] = (byte) (PingId - 1);
                return true;
            }
        }

        public override bool Done()
        {
            lock (SocketUdpNativeDynamic.syncer)
            {
                if (this.GotResult || pConnectionHandler == IntPtr.Zero)
                {
                    return true;
                }

                int available = SocketUdpNativeDynamic.egservice(pConnectionHandler);
                if (available < PingLength)
                {
                    return false;
                }

                int pingBytesLength = PingBytes.Length;
                int bytesInRemainginDatagrams = SocketUdpNativeDynamic.egread(pConnectionHandler, PingBytes, ref pingBytesLength);
                this.Successful = (PingBytes != null && PingBytes[PingBytes.Length - 1] == PingId);
                //Debug.Log("Successful: " + this.Successful + " bytesInRemainginDatagrams: " + bytesInRemainginDatagrams + " PingId: " + PingId);

                this.GotResult = true;
                return true;
            }
        }

        public override void Dispose()
        {
            lock (SocketUdpNativeDynamic.syncer)
            {
                if (this.pConnectionHandler != IntPtr.Zero)
                    SocketUdpNativeDynamic.egdisconnect(this.pConnectionHandler);
                this.pConnectionHandler = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    #if NATIVE_SOCKETS && NATIVE_SOCKETS_STATIC
    /// <summary>Uses static linked native Photon socket library via DllImport("__Internal") attribute (as done by Unity iOS and Unity Switch).</summary>
    public class PingNativeStatic : PingNative
    {
		public override bool StartPing(string ip)
        {
            base.Init();

            lock (SocketUdpNativeStatic.syncer)
			{
				if(pConnectionHandler == IntPtr.Zero)
				{
					pConnectionHandler = SocketUdpNativeStatic.egconnect(ip);
					SocketUdpNativeStatic.egservice(pConnectionHandler);
					byte state = SocketUdpNativeStatic.eggetState(pConnectionHandler);
					while (state == (byte) NativeSocketState.Connecting)
					{
						SocketUdpNativeStatic.egservice(pConnectionHandler);
						state = SocketUdpNativeStatic.eggetState(pConnectionHandler);
						Thread.Sleep(0); // suspending execution for a moment is critical on Switch for the OS to update the socket
					}
				}

                PingBytes[PingBytes.Length - 1] = PingId;
                SocketUdpNativeStatic.egsend(pConnectionHandler, PingBytes, PingBytes.Length);
                SocketUdpNativeStatic.egservice(pConnectionHandler);

                PingBytes[PingBytes.Length - 1] = (byte) (PingId - 1);
                return true;
            }
        }

        public override bool Done()
        {
            lock (SocketUdpNativeStatic.syncer)
            {
                if (this.GotResult || pConnectionHandler == IntPtr.Zero)
                {
                    return true;
                }

                int available = SocketUdpNativeStatic.egservice(pConnectionHandler);
                if (available < PingLength)
                {
                    return false;
                }

                int pingBytesLength = PingBytes.Length;
                int bytesInRemainginDatagrams = SocketUdpNativeStatic.egread(pConnectionHandler, PingBytes, ref pingBytesLength);
                this.Successful = (PingBytes != null && PingBytes[PingBytes.Length - 1] == PingId);
                //Debug.Log("Successful: " + this.Successful + " bytesInRemainginDatagrams: " + bytesInRemainginDatagrams + " PingId: " + PingId);

                this.GotResult = true;
                return true;
            }
        }

        public override void Dispose()
        {
            lock (SocketUdpNativeStatic.syncer)
            {
                if (pConnectionHandler != IntPtr.Zero)
                    SocketUdpNativeStatic.egdisconnect(pConnectionHandler);
                pConnectionHandler = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }
    #endif
    #endif
}