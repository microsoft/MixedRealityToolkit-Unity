// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

#if NETFX_CORE
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    /// <summary>
    /// Simple socket client which uses a generic length-type-data protocol.
    /// Chunk format(everything in little-endian order) :
    /// uint32_t data_length;
    /// uint8_t data[data_length];
    /// NOTE! The code assumes you are running on a little-endian system.
    /// </summary>
    public class SocketerClient
    {
        /// <summary>
        /// Underlying protocol to use.  Each has advantages and disadvantages.
        /// </summary>
        public enum Protocol
        {
            /// <summary>
            /// Fast, simple, connectionless, unidirectional, no guaranteed ordering, no error checking, no way to know if your packets are arriving.
            /// </summary>
            UDP,
            /// <summary>
            /// Still pretty fast, connections (so you know if you're talking), bidirectional, guaranteed order of arrival and error checking.
            /// </summary>
            TCP,
        }

        public enum ProtocolDirection
        {
            /// <summary>
            /// Waits for the other Socketer to connect to it.  TCP Socketers can still SendNetworkMessage after a connection.  UDP Listeners are not able to SendNetworkMessage.
            /// Does not need to have a Host specified.  Listeners may listen to multiple Senders.  TCP Listeners, when calling SendNetworkMessage, will send to every
            /// connected Sender.
            /// </summary>
            Listener,
            /// <summary>
            /// Actively tries to connect/send to the other Socketer.  TCP Socketers can still receive messages.  UDP Senders cannot.  Must have a Host specified.
            /// May only send to one listening Socketer.
            /// </summary>
            Sender,
        }

        /// <summary>
        /// Called when a message is received.  Will typically NOT be on the same thread as your UI/Update thread.  This is by design.  The Unity MonoBehaviour
        /// does call this on the Update thread; see that class for a suggested implementation of handling threads.  If a Dispatcher is available, simply call
        /// that in your handler for this event.
        /// </summary>
        public event Action<SocketerClient, MessageEvent> Message;
        /// <summary>
        /// Called when a TCP connection is made.  Not called for UDP Socketers.  Will typically NOT be on the same thread as your UI/Update thread.
        /// </summary>
        public event Action<SocketerClient, int, string> Connected;
        /// <summary>
        /// Called when a TCP connection is lost.  Not called for UDP Socketers.  Will typically NOT be on the same thread as your UI/Update thread.
        /// </summary>
        public event Action<SocketerClient, int, string> Disconnected;

        /// <summary>
        /// The protocol to use:  TCP or UDP.
        /// </summary>
        public Protocol SocketProtocol { get; private set; }
        /// <summary>
        /// Whether this object should listen for incoming connections/packets, or send them.
        /// TCP Socketers are able to both send and recieve once a connection is intitiated.
        /// </summary>
        public ProtocolDirection SocketDirection { get; private set; }

        // implementation note:  this class simply wraps several other libraries, and gives them consistent behavior.
        private SocketClient tcpClient;
        private SocketServer tcpServer;
        private CrossPlatformUDPClient udpSender;
        private CrossPlatformUDPClient udpListener;

        private bool sentConnect;
        private bool sentDisconnect;

        /// <summary>
        /// The IP address to connect or send packets to.  Only populated for senders.
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// The port to use.  Suggested values are between 10000 and 40000, and must agree.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// This is a special case property that you probably don't have to touch.  It removes socketer's own headers
        /// that are layered on top of UDP (it does not affect TCP).  The effect is that the type parameter on
        /// SendNetworkMessage is ignored, which can be helpful if you wish to implement other protocols (like Open Sound Control)
        /// on top of Socketer.
        /// </summary>
        public bool SuppressUDPHeaders { get; set; }
        public static int OutputQueueLength => SocketServer.OutputQueueLength;

        /// <summary>
        /// Gets the IP of this machine.  Suitable for use as a Host on another Socketer.
        /// </summary>
        /// <returns>The host of the local Socketer.</returns>
        public static string GetLocalIPAddress()
        {
            return CrossPlatformUDPClient.GetLocalIPAddress();
        }

        /// <summary>
        /// Creates a Socketer that sends packets or initiates connections.
        /// Actively tries to connect/send to the other Socketer.  TCP Socketers can still receive messages.  UDP Senders cannot.  Must have a Host specified.
        /// May only send to one listening Socketer.
        /// </summary>
        /// <param name="protocol">Underlying protocol to use.  Each has advantages and disadvantages.</param>
        /// <param name="host">The IP address to connect or send packets to.</param>
        /// <param name="port">The port to use.  Suggested values are between 10000 and 40000, and must agree.
        /// with the Socketer on the other end.</param>
        /// <returns></returns>
        public static SocketerClient CreateSender(Protocol protocol, string host, int port)
        {
            return new SocketerClient(protocol, host, port);
        }

        /// <summary>
        /// Creates a Socker that waits for packets or connections.
        /// Waits for the other Socketer to connect to it.  TCP Socketers can still SendNetworkMessage after a connection.  UDP Listeners are not able to SendNetworkMessage.
        /// Does not need to have a Host specified.  Listeners may listen to multiple Senders.  TCP Listeners, when calling SendNetworkMessage, will send to every
        /// connected Sender.
        /// </summary>
        /// <param name="protocol">Underlying protocol to use.  Each has advantages and disadvantages.</param>
        /// <param name="port">The port to use.  Suggested values are between 10000 and 40000, and must agree
        /// with the Socketer on the other end.</param>
        /// <returns></returns>
        public static SocketerClient CreateListener(Protocol protocol, int port)
        {
            return new SocketerClient(protocol, port);
        }

        /// <summary>
        /// Creates a Socketer that sends packets or initiates connections.
        /// Actively tries to connect/send to the other Socketer.  TCP Socketers can still receive messages.  UDP Senders cannot.  Must have a Host specified.
        /// May only send to one listening Socketer.  You must call Start to begin.
        /// </summary>
        /// <param name="protocol">Underlying protocol to use.  Each has advantages and disadvantages.</param>
        /// <param name="host">The IP address to connect or send packets to.</param>
        /// <param name="port">The port to use.  Suggested values are between 10000 and 40000, and must agree
        /// with the Socketer on the other end.</param>
        public SocketerClient(Protocol protocol, string host, int port)
        {
            this.Host = host;
            this.Port = port;
            this.SocketProtocol = protocol;
            this.SocketDirection = ProtocolDirection.Sender;
            if (protocol == Protocol.TCP)
            {
                tcpClient = new SocketClient(host, port);
                tcpClient.Message += TcpClient_Message;
                tcpClient.Connected += TcpClient_Connected;
                tcpClient.Disconnected += TcpClient_Disconnected;
            }
            else
            {
                udpSender = new CrossPlatformUDPClient();
            }
        }

        /// <summary>
        /// Creates a Socker that waits for packets or connections.
        /// Waits for the other Socketer to connect to it.  TCP Socketers can still SendNetworkMessage after a connection.  UDP Listeners are not able to SendNetworkMessage.
        /// Does not need to have a Host specified.  Listeners may listen to multiple Senders.  TCP Listeners, when calling SendNetworkMessage, will send to every
        /// connected Sender.  You must call Start to begin.
        /// </summary>
        /// <param name="protocol">Underlying protocol to use.  Each has advantages and disadvantages.</param>
        /// <param name="port">The port to use.  Suggested values are between 10000 and 40000, and must agree
        /// with the Socketer on the other end.</param>
        public SocketerClient(Protocol protocol, int port)
        {
            this.Port = port;
            this.SocketProtocol = protocol;
            this.SocketDirection = ProtocolDirection.Listener;
            if (protocol == Protocol.TCP)
            {
                tcpServer = new SocketServer(port);
                tcpServer.Message += TcpServer_Message;
                tcpServer.Connect += TcpServer_Connect;
                tcpServer.Disconnect += TcpServer_Disconnect;
            }
            else
            {
                udpListener = new CrossPlatformUDPClient(port);
                udpListener.Message += UdpListener_Message;
            }
        }

        /// <summary>
        /// Begins either listening or sending.
        /// </summary>
        public void Start()
        {
            if (tcpClient != null)
            {
                tcpClient.Start();
            }

            if (tcpServer != null)
            {
                tcpServer.Start();
            }
        }

        /// <summary>
        /// Stops listening or sending.  This should be called when you are done.
        /// </summary>
        public void Stop()
        {
            if (tcpClient != null)
            {
                tcpClient.Stop();
            }

            if (tcpServer != null)
            {
                tcpServer.Stop();
            }

            if (udpListener != null)
            {
                udpListener.Close();
            }

            if (udpSender != null)
            {
                udpSender.Close();
            }
        }

        /// <summary>
        /// Disconnects the client
        /// </summary>
        /// <param name="sourceId"></param>
        public void Disconnect(int sourceId)
        {
            if (tcpClient != null)
            {
                tcpClient.Disconnect();
            }

            if (tcpServer != null)
            {
                tcpServer.StopClient(sourceId);
            }
        }

        /// <summary>
        /// Sends a string to the other side.  Works for all Socketers except UDP Listeners.
        /// If the other Socketer is not listening, the messages are lost.
        /// </summary>
        /// <param name="message">Message contents.</param>
        public void SendNetworkMessage(string message, int sourceId = 0)
        {
            SendNetworkMessage(System.Text.Encoding.UTF8.GetBytes(message), sourceId);
        }

        /// <summary>
        /// Sends a byte array to the other side.  Works for all Socketers except UDP Listeners.
        /// If the other Socketer is not listening, the messages are lost.
        /// </summary>
        /// <param name="message">Message contents.</param>
        public void SendNetworkMessage(byte[] message, int sourceId = 0)
        {
            if (tcpClient != null && tcpClient.IsConnected)
            {
                tcpClient.SendBuffer(message);
            }

            if (tcpServer != null)
            {
                Socket socket = tcpServer.FindClient(sourceId);
                if (socket != null)
                {
                    tcpServer.SendTo(message, socket);
                }
                else
                {
                    tcpServer.Broadcast(message);
                }
            }

            if (udpSender != null)
            {
                if (SuppressUDPHeaders)
                {
                    udpSender.Send(message, message.Length, Host, Port);
                }
                else
                {
                    // the tcp code already implements the length protocol, the udp code does not.  So implement it here.
                    byte[] wrappedMessage = new byte[message.Length + 4];
                    using (MemoryStream m = new MemoryStream(wrappedMessage))
                    {
                        using (BinaryWriter w = new BinaryWriter(m))
                        {
                            w.Write((UInt32)message.Length);
                            w.Write(message);
                            udpSender.Send(wrappedMessage, wrappedMessage.Length, Host, Port);
                        }
                    }
                }
            }
        }

        private void TcpClient_Message(object sender, SocketClient.MessageEventArgs e)
        {
            Message?.Invoke(this, new MessageEvent() { SourceHost = Host, Message = e.Data });
        }

        private void TcpServer_Message(object sender, SocketServer.MessageEventArgs e)
        {
            Message?.Invoke(this, new MessageEvent() { SourceHost = e.RemoteEndpoint.Address.ToString(), Message = e.Data, SourceId = e.SourceId });
        }

        private void UdpListener_Message(byte[] data, string sourceHost)
        {
            if (SuppressUDPHeaders)
            {
                Message?.Invoke(this, new MessageEvent() { SourceHost = sourceHost, Message = data });
            }
            else
            {
                // the tcp code already implements the length protocol, the udp code does not.  So implement it here.
                UInt32 length = BitConverter.ToUInt32(data, 0);
                byte[] message = new byte[length];
                Array.Copy(data, 4, message, 0, message.Length);

                Message?.Invoke(this, new MessageEvent() { SourceHost = sourceHost, Message = message });
            }
        }

        private void TcpClient_Disconnected(object sender, SocketClient.ConnectionEventArgs e)
        {
            if (sentConnect && !sentDisconnect)
            {
                sentDisconnect = true;
                Disconnected?.Invoke(this, 0, Host);
            }
        }

        private void TcpClient_Connected(object sender, SocketClient.ConnectionEventArgs e)
        {
            sentConnect = true;
            sentDisconnect = false;
            Connected?.Invoke(this, 0, Host);
        }

        private void TcpServer_Disconnect(object sender, SocketServer.ConnectionEventArgs e)
        {
            Disconnected?.Invoke(this, e.SourceId, e.RemoteAddress.Address.ToString());
        }

        private void TcpServer_Connect(object sender, SocketServer.ConnectionEventArgs e)
        {
            Connected?.Invoke(this, e.SourceId, e.RemoteAddress.Address.ToString());
        }

#region Private UDP Implementation
        private class CrossPlatformUDPClient
        {
            public static string GetLocalIPAddress()
            {
#if !NETFX_CORE
#if NETCOREAPP1_1
                var t = Dns.GetHostEntryAsync(Dns.GetHostName());
                t.Wait();
                var host = t.Result;
#else
                var host = Dns.GetHostEntry(Dns.GetHostName());
#endif
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
#else
                var icp = NetworkInformation.GetInternetConnectionProfile();
                HostNameType hostNameType = HostNameType.Ipv4;
                if (icp?.NetworkAdapter == null) return null;
                var hostname =
                    NetworkInformation.GetHostNames()
                        .FirstOrDefault(
                            hn =>
                                hn.Type == hostNameType &&
                                hn.IPInformation?.NetworkAdapter != null && 
                                hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);

                if (hostname != null)
                {
                    // the ip address
                    return hostname.CanonicalName;
                }
#endif
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }

#if !NETFX_CORE
            UdpClient client;
            bool threadShouldRun = true;
#else
            private Windows.Networking.Sockets.DatagramSocket client;
#endif

            public event Action<byte[], string> Message;

            public CrossPlatformUDPClient(int port)
            {
#if !NETFX_CORE
                client = new UdpClient(port);
                (new Thread(ClientThreadFunc)).Start();
#else
                client = new Windows.Networking.Sockets.DatagramSocket();
                client.BindEndpointAsync(null, port.ToString()).GetResults();
                client.MessageReceived += OnMessageRecieved;
#endif
            }

            public CrossPlatformUDPClient()
            {
#if !NETFX_CORE
                client = new UdpClient();
#else
                client = new Windows.Networking.Sockets.DatagramSocket();
                client.Control.DontFragment = true;
                client.Control.QualityOfService = Windows.Networking.Sockets.SocketQualityOfService.LowLatency;
                client.Control.OutboundUnicastHopLimit = 255;
#endif
            }

            public void JoinMulticastGroup(IPAddress address)
            {
#if !NETFX_CORE
                client.JoinMulticastGroup(address);
#else
                client.JoinMulticastGroup(new HostName(address.ToString()));
#endif
            }

            public void Send(byte[] data, int length, string host, int port)
            {
#if !NETFX_CORE
#if NETCOREAPP1_1
                client.SendAsync(data, length, host, port).Wait();
#else
                client.Send(data, length, host, port);
#endif
#else

                var hostname = new HostName(host);

                var outputStreamAyncOperation = client.GetOutputStreamAsync(hostname, port.ToString());
                outputStreamAyncOperation.Completed += new AsyncOperationCompletedHandler<IOutputStream>((asyncInfo, asyncStatus) =>
                { 
                    var outputStream = asyncInfo.GetResults();
                    var writeOperation = outputStream.WriteAsync(data.AsBuffer());
                    writeOperation.Completed += new AsyncOperationWithProgressCompletedHandler<uint,uint>((info, status) =>
                       {
                           outputStream.FlushAsync().Completed += new AsyncOperationCompletedHandler<bool>((info2, status2) =>
                           {
                               outputStream.Dispose();
                           });
                       });
                });
#endif
            }

#if !NETFX_CORE
            private void ClientThreadFunc()
            {
                while (threadShouldRun)
                {
                    IPEndPoint endpoint = null;
#if NETCOREAPP1_1
                    var t = client.ReceiveAsync();
                    t.Wait();
                    endpoint = t.Result.RemoteEndPoint;
                    byte[] data = t.Result.Buffer;
#else
                    byte[] data = client.Receive(ref endpoint);
#endif
                    string host = endpoint.Address.ToString();
                    if (Message != null)
                    {
                        Message.Invoke(data, host);
                    }
                }
            }
#else
            public void OnMessageRecieved(object sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
            {
                DataReader reader = args.GetDataReader();
                uint bytesToRead = reader.UnconsumedBufferLength;
                byte[] buffer = new byte[bytesToRead];
                reader.ReadBytes(buffer);
        
                string host = args.RemoteAddress.ToString();

                if (Message != null)
                {
                    Message.Invoke(buffer, host);
                }
            }
#endif

            public void Close()
            {
#if !NETFX_CORE
                threadShouldRun = false;
#endif

#if NETCOREAPP1_1 || NETFX_CORE
                client.Dispose();
#else
                client.Close();
#endif
            }
        }
#endregion

#region Private TCP Implementation
#if !NETFX_CORE
        private class SocketClient
        {
            private string host;
            private Int32 port;
            private TcpClient tcpClient;
            private BinaryWriter writer;
            private BinaryReader reader;
            private bool threadShouldRun;
            // If a connection fails, wait this many ms
            private const int timeoutMS = 500;

            internal class ConnectionEventArgs : EventArgs
            {
            }

            internal class MessageEventArgs : EventArgs
            {
                public byte[] Data;
            }

            public event EventHandler<ConnectionEventArgs> Connected;
            public event EventHandler<ConnectionEventArgs> Disconnected;
            public event EventHandler<MessageEventArgs> Message;

            private void OnConnect()
            {
                if (Connected != null)
                {
                    Connected(this, new ConnectionEventArgs { });
                }
            }

            private void OnDisconnected()
            {
                if (Disconnected != null)
                {
                    Disconnected(this, new ConnectionEventArgs { });
                }
            }

            private void OnMessage(byte[] data)
            {
                if (Message != null)
                {
                    Message(this, new MessageEventArgs { Data = data });
                }
            }

            class CommandHeader
            {
                public UInt32 size;
                public CommandHeader(int size)
                {
                    this.size = (UInt32)size;
                }
            };

            class CommandData
            {
                public byte[] data;
            }

            public SocketClient(string host, Int32 port)
            {
                this.host = host;
                this.port = port;
            }

            public bool IsConnected
            {
                get
                {
                    return tcpClient != null && tcpClient.Connected;
                }
            }

            /// <summary>
            /// Starts the client thread. The client will then
            /// try connecting on the port continually, sending data, and 
            /// parsing any messages it receives and raising events
            /// </summary>
            public void Start()
            {
                threadShouldRun = true;
                (new Thread(ClientThreadFunc)).Start();
            }

            public void Stop()
            {
                threadShouldRun = false;
                Disconnect();
            }

            public bool SendBuffer(byte[] data)
            {
                CommandHeader header = new CommandHeader(data.Length);
                try
                {
                    writer.Write(header.size);
                }
                catch (IOException e)
                {
                    Debug.LogError("send header failed. " + e.Message);
                    Disconnect();
                    return false;
                }

                if (data.Length > 0)
                {
                    try
                    {
                        writer.Write(data);
                    }
                    catch (IOException e)
                    {
                        Debug.LogError("send data failed. " + e.Message);
                        Disconnect();
                        return false;
                    }
                }
                writer.Flush();
                return true;
            }

            public void Disconnect()
            {
                bool disconnected = false;
                if (tcpClient != null)
                {
#if NETCOREAPP1_1
                    tcpClient.Dispose();
#else
                    tcpClient.Close();
#endif
                    tcpClient = null;
                    disconnected = true;
                }
                if (writer != null)
                {
#if NETCOREAPP1_1
                    writer.Dispose();
#else
                    writer.Close();
#endif
                }
                if (disconnected)
                {
                    OnDisconnected();
                }
            }


            private bool Connect()
            {
                try
                {
#if NETCOREAPP1_1
                    tcpClient = new TcpClient();
                    tcpClient.ConnectAsync(host, port).Wait();
#else
                    tcpClient = new TcpClient(host, port);
#endif
                    tcpClient.NoDelay = true;
                    writer = new BinaryWriter(tcpClient.GetStream());
                    reader = new BinaryReader(tcpClient.GetStream());
                    if (ReadHandshake())
                    {
                        WriteHandshake();
                        OnConnect();
                    }
                    else
                    {
                        return false;
                    }
                }
#if NETCOREAPP1_1
                catch (AggregateException e)
                {
                    if (e.InnerException is SocketException)
                    {
                        return false;
                    }
                    throw e;
                }
#endif
                catch (SocketException e)
                {
                    Debug.LogWarning($"An error occurred while accessing the socket: {e}");
                    return false;
                }
                return true;
            }

            private bool ReadHandshake()
            {
                try
                {
                    UInt32 handshake = reader.ReadUInt32();
                    return handshake == Socketer.HandshakeCode;
                }
                catch (IOException)
                {
                    Disconnect();
                    return false;
                }
            }

            private void WriteHandshake()
            {
                byte[] handshakeBytes = BitConverter.GetBytes(Socketer.HandshakeCode);
                try
                {
                    writer.Write(handshakeBytes);
                    writer.Flush();

                }
                catch (IOException)
                {
                    Disconnect();
                }
            }

            private CommandData ReceiveBuffer()
            {
                try
                {
                    UInt32 dataLength = reader.ReadUInt32();
                    byte[] data = new byte[dataLength];
                    int bytesRead = 0;
                    while (bytesRead < dataLength)
                    {
                        bytesRead += reader.Read(data, bytesRead, (int)dataLength - bytesRead);
                    }
                    return new CommandData { data = data };
                }
                catch (IOException e)
                {
                    Debug.LogWarning($"Receive buffer failed: {e}");
                    Disconnect();
                    return null;
                }
            }

            private void ClientThreadFunc()
            {
                while (threadShouldRun)
                {
                    if (!IsConnected)
                    {
                        Debug.Log("Trying to connect to " + host + ":" + port);
                        if (!Connect())
                        {
                            Thread.Sleep(timeoutMS);
                            continue;
                        }
                    }

                    CommandData data = ReceiveBuffer();
                    if (data != null)
                    {
                        OnMessage(data.data);
                    }
                }
            }
        }
#else
        private class SocketClient
        {
            private Windows.Networking.Sockets.StreamSocket client;
            private DataWriter writer;
            private string host;
            private int port;
            private bool isConnected = false;
            private bool shouldConnect = false;

            internal class ConnectionEventArgs : EventArgs
            {
            }

            internal class MessageEventArgs : EventArgs
            {
                public byte[] Data;
            }

            public event EventHandler<ConnectionEventArgs> Connected;
            public event EventHandler<ConnectionEventArgs> Disconnected;
            public event EventHandler<MessageEventArgs> Message;

            private void OnConnect()
            {
                if (Connected != null)
                {
                    Connected(this, new ConnectionEventArgs { });
                }
            }

            private void OnDisconnected()
            {
                if (Disconnected != null)
                {
                    Disconnected(this, new ConnectionEventArgs { });
                }
            }

            private void OnMessage(byte[] data)
            {
                if (Message != null)
                {
                    Message(this, new MessageEventArgs { Data = data });
                }
            }

            public SocketClient(string host, Int32 port)
            {
                this.host = host;
                this.port = port;
            }

            public bool IsConnected
            {
                get { return isConnected; }
            }

            public async void Start()
            {
                HostName hostName = new HostName(host);

                shouldConnect = true;

                while (shouldConnect)
                {
                    await Connect(hostName);

                    if (!shouldConnect) return;

                    await ProcessConnection();
                }
            }

            private async System.Threading.Tasks.Task Connect(HostName hostName)
            {
                while (!isConnected && shouldConnect)
                {
                    try
                    {
                        if (client == null)
                        {
                            client = new Windows.Networking.Sockets.StreamSocket();
                        }
                        await client.ConnectAsync(hostName, port.ToString());
                        writer = new DataWriter(client.OutputStream);
                        writer.ByteOrder = ByteOrder.LittleEndian;
                        isConnected = true;
                    }
                    catch
                    {
                    }
                }
            }

            private async System.Threading.Tasks.Task ProcessConnection()
            {
                if (client == null)
                {
                    Disconnect();
                    return;
                }
                DataReader reader = new DataReader(client.InputStream);
                reader.ByteOrder = ByteOrder.LittleEndian;

                try
                {
                    await ReadHandshake(reader);
                    WriteHandshake();

                    OnConnect();

                    while (isConnected)
                    {

                        uint header = await reader.LoadAsync(4);
                        if (header != 4)
                        {
                            Disconnect();
                        }
                        else
                        {
                            uint length = reader.ReadUInt32();

                            uint bufferRead = await reader.LoadAsync(length);
                            if (bufferRead != length)
                            {
                                Disconnect();
                            }
                            else
                            {
                                byte[] data = new byte[length];
                                reader.ReadBytes(data);

                                OnMessage(data);
                            }
                        }
                    }
                }
                catch
                {
                    Disconnect();
                }
            }

            private async Task ReadHandshake(DataReader reader)
            {
                uint bytesRead = await reader.LoadAsync(sizeof(UInt32));
                if (bytesRead == sizeof(UInt32))
                {
                    UInt32 handshake = reader.ReadUInt32();
                    if (handshake == Socketer.HandshakeCode)
                    {
                        return;
                    }
                }
                throw new Exception("Invalid handshake message");
            }

            private async void WriteHandshake()
            {
                writer.WriteUInt32(Socketer.HandshakeCode);
                await writer.StoreAsync();
                await writer.FlushAsync();
            }

            public void Stop()
            {
                shouldConnect = false;
                Disconnect();
            }

            public void Disconnect()
            {
                if (writer != null)
                {
                    try
                    {
                        writer.DetachStream();
                    }
                    catch
                    {
                    }
                    
                    writer.Dispose();
                    writer = null;
                }

                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }

                isConnected = false;

                OnDisconnected();
            }

            public async void SendBuffer(byte[] data)
            {
                try
                {
                    writer.WriteUInt32((uint)data.Length);
                    writer.WriteBytes(data);

                    await writer.StoreAsync();
                    await writer.FlushAsync();
                }
                catch
                {
                    Disconnect();
                }
            }
        }
#endif

        private class SocketServer
        {
            public Socket serverSocket;
            private Dictionary<Socket, EndPoint> clients = new Dictionary<Socket, EndPoint>();

            private static int outputQueueLength;
            public static int OutputQueueLength => outputQueueLength;

            internal class ConnectionEventArgs : EventArgs
            {
                public int SourceId;
                public IPEndPoint RemoteAddress;
            }

            internal class MessageEventArgs : EventArgs
            {
                public IPEndPoint RemoteEndpoint;
                public byte[] Data;
                public int SourceId;
            }

            public event EventHandler<MessageEventArgs> Message;
            public event EventHandler<ConnectionEventArgs> Connect;
            public event EventHandler<ConnectionEventArgs> Disconnect;

            public void StopClient(int sourceId)
            {
                Socket clientSocket = FindClient(sourceId);
                if (clientSocket != null)
                {
                    clientSocket.Dispose();
                    RemoveClient(clientSocket);
                }
            }

            private void OnConnect(Socket socket)
            {
                if (Connect != null)
                {
                    Connect(this, new ConnectionEventArgs { SourceId = socket.GetHashCode(), RemoteAddress = (IPEndPoint)socket.RemoteEndPoint });
                }
            }

            private void OnDisconnect(Socket socket, EndPoint endpoint)
            {
                if (Disconnect != null)
                {
                    Disconnect(this, new ConnectionEventArgs { SourceId = socket.GetHashCode(), RemoteAddress = (IPEndPoint)endpoint });
                }
            }

            private void OnMessage(EndPoint remoteEndpoint, byte[] data, int sourceId)
            {
                if (Message != null)
                {
                    Message(this, new MessageEventArgs { RemoteEndpoint = (IPEndPoint)remoteEndpoint, Data = data, SourceId = sourceId });
                }
            }

            public SocketServer(int port)
            {
                outputQueueLength = 0;
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#if !NETFX_CORE
                serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#endif
                serverSocket.Bind(new IPEndPoint(0, port));
            }

            private void AddClient(Socket socket)
            {
                lock (clients)
                {
                    clients[socket] = socket.RemoteEndPoint;
                }
            }

            private void RemoveClient(Socket socket)
            {
                lock (clients)
                {
                    if (!clients.ContainsKey(socket))
                        return;

                    EndPoint ep = clients[socket];
                    clients.Remove(socket);
                    OnDisconnect(socket, ep);
                }
            }

#if NETFX_CORE
            private delegate void AcceptCallback(SocketError result, Socket socket);
            private delegate void ReceiveAllCallback(SocketError result);
            private delegate void SendAllCallback(SocketError result);

            private static void AcceptAsync(Socket socket, AcceptCallback callback)
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.Completed += (sender, e2) => { callback(e2.SocketError, e2.AcceptSocket); e2.Dispose(); };
                if (!socket.AcceptAsync(e))
                {
                    callback(e.SocketError, e.AcceptSocket);
                    e.Dispose();
                }
            }

            private static void ReceiveAllAsync_Helper(object sender, SocketAsyncEventArgs e)
            {
                if (e.SocketError != SocketError.Success || e.BytesTransferred == e.Count)
                {
                    ((ReceiveAllCallback)e.UserToken)(e.SocketError);
                    e.Dispose();
                    return;
                }
                e.SetBuffer(e.Buffer, e.Offset + e.BytesTransferred, e.Count - e.BytesTransferred);
                if (!((Socket)sender).ReceiveAsync(e))
                {
                    ReceiveAllAsync_Helper(sender, e);
                }
            }

            private static void ReceiveAllAsync(Socket socket, byte[] buffer, int offset, int count, ReceiveAllCallback callback)
            {
                try
                {
                    SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                    e.SetBuffer(buffer, offset, count);
                    e.Completed += ReceiveAllAsync_Helper;
                    e.UserToken = callback;
                    if (!socket.ReceiveAsync(e))
                    {
                        ReceiveAllAsync_Helper(socket, e);
                    }
                }
                catch
                {
                }
            }

            private static void ReceiveAllAsync(Socket socket, byte[] buffer, ReceiveAllCallback callback)
            {
                ReceiveAllAsync(socket, buffer, 0, buffer.Length, callback);
            }

            private static void SendAllAsync_Helper(object sender, SocketAsyncEventArgs e)
            {
                Interlocked.Add(ref outputQueueLength, -e.BytesTransferred);
                if (e.SocketError != SocketError.Success || e.BytesTransferred == e.Count)
                {
                    if (e.Count - e.BytesTransferred > 0)
                    {
                        Interlocked.Add(ref outputQueueLength, -(e.Count - e.BytesTransferred));
                    }
                    ((SendAllCallback)e.UserToken)(e.SocketError);
                    e.Dispose();
                    return;
                }
                e.SetBuffer(e.Buffer, e.Offset + e.BytesTransferred, e.Count - e.BytesTransferred);
                if (!((Socket)sender).SendAsync(e))
                {
                    SendAllAsync_Helper(sender, e);
                }
            }

            private static void SendAllAsync(Socket socket, byte[] buffer, int offset, int count, SendAllCallback callback)
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.SetBuffer(buffer, offset, count);
                e.Completed += SendAllAsync_Helper;
                e.UserToken = callback;
                if (!socket.SendAsync(e))
                {
                    SendAllAsync_Helper(socket, e);
                }
            }
            private static void SendAllAsync(Socket socket, byte[] buffer, SendAllCallback callback)
            {
                SendAllAsync(socket, buffer, 0, buffer.Length, callback);
            }

            private void OnAccept(SocketError result, Socket socket)
            {
                AcceptAsync(serverSocket, OnAccept);
                if (result == SocketError.Success)
                {
                    HandleClient(socket);
                }
                else
                {
                    Debug.LogError("Socket operation failed: " + result);
                }
            }

            private void DoStart()
            {
                AcceptAsync(serverSocket, OnAccept);
            }

            private void HandleClient(Socket socket)
            {
                AddClient(socket);
                WriteHandshake(socket);
                ReadHandshake(socket);
            }

            private void WriteHandshake(Socket socket)
            {
                byte[] handshakeBytes = BitConverter.GetBytes(Socketer.HandshakeCode);
                DoSendTo(handshakeBytes, socket);
            }

            private void ReadHandshake(Socket socket)
            {
                byte[] handshakeBytes = new byte[4];
                ReceiveAllAsync(socket, handshakeBytes, status =>
                {
                    try
                    {
                        if (status != SocketError.Success)
                        {
                            RemoveClient(socket);
                            return;
                        }
                        UInt32 handshake = BitConverter.ToUInt32(handshakeBytes, 0);
                        if (handshake != Socketer.HandshakeCode)
                        {
                            RemoveClient(socket);
                            return;
                        }
                        OnConnect(socket);
                        ReadFromClient(socket);
                    }
                    catch { }
                });
            }

            private void ReadFromClient(Socket socket)
            {
                byte[] header = new byte[4];
                ReceiveAllAsync(socket, header, status =>
                {
                    try
                    {
                        if (status != SocketError.Success)
                        {
                            Debug.LogError("Socket recv failed: " + status);
                            RemoveClient(socket);
                            return;
                        }
                        UInt32 size = BitConverter.ToUInt32(header, 0);
                        byte[] buf = new byte[size];
                        ReceiveAllAsync(socket, buf, status2 =>
                        {
                            try
                            {
                                if (status2 != SocketError.Success)
                                {
                                    Debug.LogError("Socket recv failed: " + status2);
                                    RemoveClient(socket);
                                    return;
                                }

                                OnMessage(socket.RemoteEndPoint, buf, socket.GetHashCode());
                                ReadFromClient(socket);
                            }
                            catch { }
                        });
                    }
                    catch { }
                });
            }

            private void DoSendTo(byte[] data, Socket client)
            {
                Interlocked.Add(ref outputQueueLength, data.Length);
                SendAllAsync(client, data, status =>
                {
                    if (status != SocketError.Success)
                    {
                        Debug.LogError("Socket send failed: " + status);
                        // Infer that the client is probably dead and remove it
                        // To avoid concurrent modification, we iterate over a clone of the client list
                        RemoveClient(client);
                    }
                });
            }

            private void DoBroadcast(byte[] data)
            {
                // Create a copy of the clients list so we can modify/remove clients if needed
                List<Socket> tmpClients = new List<Socket>(clients.Keys);
                foreach (Socket client in tmpClients)
                {
                    DoSendTo(data, client);
                }
            }

            private static void DoCloseSocket(Socket socket)
            {
                /* WinRT has no Close method, so dispose the socket instead. */
                socket.Dispose();
            }
#else
            private static void ReceiveAll(Socket socket, byte[] buffer, int offset, int count)
            {
                int pos = 0;
                while (pos < count)
                {
                    int res = socket.Receive(buffer, offset + pos, count - pos, SocketFlags.None);
                    if (res <= 0)
                    {
                        throw new Exception("Socket receive failed: no more data");
                    }
                    pos += res;
                }
            }

            private static void ReceiveAll(Socket socket, byte[] buffer)
            {
                ReceiveAll(socket, buffer, 0, buffer.Length);
            }

            private static void SendAll(Socket socket, byte[] buffer, int offset, int count)
            {
                int pos = 0;
                while (pos < count)
                {
                    int res = socket.Send(buffer, offset + pos, count - pos, SocketFlags.None);
                    if (res <= 0)
                    {
                        throw new Exception("Socket send failed: shutdown");
                    }
                    pos += res;
                }
            }
            private static void SendAll(Socket socket, byte[] buffer)
            {
                SendAll(socket, buffer, 0, buffer.Length);
            }

            private void ReadFromClient(Socket socket)
            {
                byte[] header = new byte[4];
                ReceiveAll(socket, header);

                UInt32 size = BitConverter.ToUInt32(header, 0);
                byte[] buf = new byte[size];
                ReceiveAll(socket, buf);

                int sourceId = socket.GetHashCode();
                OnMessage(socket.RemoteEndPoint, buf, sourceId);
            }

            private void ClientThreadFunc(object socketObject)
            {
                Socket socket = (Socket)socketObject;

                try
                {
                    WriteHandshake(socket);
                    ReadHandshake(socket);
                    OnConnect(socket);
                    while (true)
                    {
                        ReadFromClient(socket);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    RemoveClient(socket);
                }
            }

            private void WriteHandshake(Socket socket)
            {
                byte[] handshakeBytes = BitConverter.GetBytes(Socketer.HandshakeCode);
                SendAll(socket, handshakeBytes);
            }

            private void ReadHandshake(Socket socket)
            {
                byte[] handshakeBytes = new byte[4];
                ReceiveAll(socket, handshakeBytes);
                UInt32 handshake = BitConverter.ToUInt32(handshakeBytes, 0);
                if (handshake != Socketer.HandshakeCode)
                {
                    throw new Exception("Invalid handshake message");
                }
            }

            private void ServerThreadFunc()
            {
                while (true)
                {
                    try
                    {
                        Socket socket = serverSocket.Accept();
                        AddClient(socket);
                        (new Thread(ClientThreadFunc)).Start(socket);
                    }
                    catch (Exception e)
                    {
                        if (serverSocket.Connected)
                        {
                            Debug.LogException(e);
                        }
                        break;
                    }
                }
            }

            private void DoStart()
            {
                (new Thread(ServerThreadFunc)).Start();
            }

            private void DoSendTo(byte[] data, Socket client)
            {
                try
                {
                    SendAll(client, data);
                }
                catch (Exception e)
                {
                    // Infer that the client is probably dead and remove it
                    // To avoid concurrent modification, we iterate over a clone of the client list
                    Debug.LogException(e);
                    RemoveClient(client);
                }
            }

            private void DoBroadcast(byte[] data)
            {
                // Create a copy of the clients list so we can modify/remove clients if needed
                List<Socket> tmpClients = null;
                lock (clients)
                {
                    tmpClients = new List<Socket>(clients.Keys);
                }
                foreach (Socket client in tmpClients)
                {
                    DoSendTo(data, client);
                }
            }

            private static void DoCloseSocket(Socket socket)
            {
#if NETCOREAPP1_1
                socket.Dispose();
#else
                socket.Close();
#endif
            }
#endif
            public void Start()
            {
                serverSocket.Listen(1);
                DoStart();
            }

            private byte[] PackMessageData(byte[] data)
            {
                byte[] newData = new byte[data.Length + 4];
                System.Buffer.BlockCopy(BitConverter.GetBytes((UInt32)data.Length), 0, newData, 0, 4);
                System.Buffer.BlockCopy(data, 0, newData, 4, data.Length);
                return newData;
            }
            public void Broadcast(byte[] data)
            {
                byte[] newData = PackMessageData(data);
                DoBroadcast(newData);
            }

            public void SendTo(byte[] data, Socket socket)
            {
                byte[] newData = PackMessageData(data);
                DoSendTo(newData, socket);
            }

            private static void KillSocket(Socket sock)
            {
                try
                {
                    DoCloseSocket(sock);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            public void Stop()
            {
                KillSocket(serverSocket);

                // Create a copy of the clients list so we can remove clients
                Dictionary<Socket, EndPoint> tmpClients = new Dictionary<Socket, EndPoint>(clients);
                foreach (var entry in tmpClients)
                {
                    KillSocket(entry.Key);
                    RemoveClient(entry.Key);
                    OnDisconnect(entry.Key, entry.Value);
                }
            }

            public Socket FindClient(int sourceId)
            {
                lock (clients)
                {
                    foreach (Socket socket in clients.Keys)
                    {
                        if (socket.GetHashCode() == sourceId)
                        {
                            return socket;
                        }
                    }
                }
                return null;
            }
        }
#endregion
    }
}