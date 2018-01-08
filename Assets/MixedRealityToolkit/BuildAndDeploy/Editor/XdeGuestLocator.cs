// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Emulator Utility Class
    /// </summary>
    public static class XdeGuestLocator
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct XdePeerHostIdentifier
        {
            public Guid GuestDiscoveryGUID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] GuestMACAddress;
            public int PeerDiscoveryPort;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct XdePeerGuestIdentifier
        {
            public Guid GuestDiscoveryGUID;
            public int GuestTcpPort;
            public int GuestSvcVersion;
        }

        public static bool IsSearching { get; private set; }
        public static bool HasData { get; private set; }
        public static IPAddress GuestIpAddress { get; private set; }

        static XdeGuestLocator()
        {
            HasData = false;
            IsSearching = false;
        }

        public static void FindGuestAddressAsync()
        {
            if (IsSearching)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(FindGuestAddress);
        }

        private static void FindGuestAddress(object state)
        {
            IsSearching = true;
            HasData = false;
            GuestIpAddress = IPAddress.None;

            UnicastIPAddressInformation internalSwitchAddressInfo = null;
            try
            {
                internalSwitchAddressInfo = GetInternalSwitchAddressInfo();
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError("Failed to locate internal switch adapter");
            }

            if (internalSwitchAddressInfo != null)
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    try
                    {
                        // Bind to next available UDP port for a listen operation
                        socket.Blocking = true;
                        socket.ReceiveTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;

                        socket.Bind(new IPEndPoint(internalSwitchAddressInfo.Address, 0));
                        var localPort = (socket.LocalEndPoint as IPEndPoint).Port;

                        // Send out a probe to 'devices' connected to the internal switch
                        // listening on port 3553 (Microsoft Device Emulator specific)
                        var broadcastAddress = GetBroadcastAddressForAddress(internalSwitchAddressInfo.Address, internalSwitchAddressInfo.IPv4Mask);
                        var broadcastTarget = new IPEndPoint(broadcastAddress, 3553);

                        //
                        // WORKAROUND: We don't have easy access to WMI to go querying
                        // for virtual machine information so we just cover finding
                        // the first 255 potential candidates xx 00 - xx FF.
                        //
                        // It sounds like a lot but we're talking super tiny
                        // payloads on an internal interface. It's very fast.
                        //
                        for (int i = 0; i <= 0xFF; i++)
                        {
                            var probe = GenerateProbe(localPort, i);
                            socket.SendTo(probe, broadcastTarget);
                        }

                        // Return the endpoint information for the first 'device' that replies
                        // (we don't necessarily care about the returned identifier info)
                        var responseBytes = new byte[Marshal.SizeOf(typeof(XdePeerGuestIdentifier))];

                        EndPoint guestEndpoint = new IPEndPoint(broadcastAddress, 0);

                        socket.ReceiveFrom(responseBytes, ref guestEndpoint);
                        GuestIpAddress = (guestEndpoint as IPEndPoint).Address;
                        HasData = true;
                    }
                    catch (SocketException)
                    {
                        // Do nothing, our probe went unanswered or failed
                    }
                }
            }

            IsSearching = false;
        }

        private static UnicastIPAddressInformation GetInternalSwitchAddressInfo()
        {
            var internalSwitch = GetInternalNetworkSwitchInterface();
            return internalSwitch.GetIPProperties().UnicastAddresses.Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
        }

        private static NetworkInterface GetInternalNetworkSwitchInterface()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Where(i => i.Name.Contains("Windows Phone Emulator")).FirstOrDefault();
        }

        private static IPAddress GetBroadcastAddressForAddress(IPAddress address, IPAddress mask)
        {
            var addressInt = BitConverter.ToInt32(address.GetAddressBytes(), 0);
            var maskInt = BitConverter.ToInt32(mask.GetAddressBytes(), 0);
            return new IPAddress(BitConverter.GetBytes((addressInt | ~maskInt)));
        }

        private static byte[] GenerateProbe(int port, int machineIndex)
        {
            var identifier = new XdePeerHostIdentifier();
            identifier.PeerDiscoveryPort = port;
            identifier.GuestDiscoveryGUID = new Guid("{963ef858-2efe-4eb4-8d2d-fed5408e6441}");
            identifier.GuestMACAddress = new byte[] { 0x02, 0xDE, 0xDE, 0xDE, 0xDE, (byte)machineIndex };

            return GetStructureBytes(identifier);
        }

        private static byte[] GetStructureBytes(object obj)
        {
            var bytes = new byte[Marshal.SizeOf(obj)];

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
            handle.Free();

            return bytes;
        }
    }
}
