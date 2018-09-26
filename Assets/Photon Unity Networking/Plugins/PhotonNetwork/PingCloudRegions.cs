using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ExitGames.Client.Photon;
using UnityEngine;
using Debug = UnityEngine.Debug;
using SupportClassPun = ExitGames.Client.Photon.SupportClass;


#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IPHONE && !UNITY_PS3 && !UNITY_WINRT)

using System.Net.Sockets;

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
        //Debug.Log("Got: " + SupportClassPun.ByteArrayToString(PingBytes));
        bool replyMatch = PingBytes[PingBytes.Length - 1] == PingId && read == PingLength;
        if (!replyMatch) Debug.Log("ReplyMatch is false! ");


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



public class PhotonPingManager
{
    public bool UseNative;
    public static int Attempts = 5;
    public static bool IgnoreInitialAttempt = true;
    public static int MaxMilliseconsPerPing = 800;  // enter a value you're sure some server can beat (have a lower rtt)


    public Region BestRegion
    {
        get
        {
            Region result = null;
            int bestRtt = Int32.MaxValue;
            foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
            {
                Debug.Log("BestRegion checks region: " + region);
                if (region.Ping != 0 && region.Ping < bestRtt)
                {
                    bestRtt = region.Ping;
                    result = region;
                }
            }

            return (Region)result;
        }
    }

    public bool Done { get { return this.PingsRunning == 0; } }
    private int PingsRunning;


    /// <remarks>
    /// Affected by frame-rate of app, as this Coroutine checks the socket for a result once per frame.
    /// </remarks>
    public IEnumerator PingSocket(Region region)
    {
        region.Ping = Attempts*MaxMilliseconsPerPing;

        this.PingsRunning++;        // TODO: Add try-catch to make sure the PingsRunning are reduced at the end and that the lib does not crash the app
        PhotonPing ping;
        //Debug.Log("PhotonHandler.PingImplementation " + PhotonHandler.PingImplementation);
        if (PhotonHandler.PingImplementation == typeof(PingNativeDynamic))
        {
            Debug.Log("Using constructor for new PingNativeDynamic()"); // it seems on android, the Activator can't find the default Constructor
            ping = new PingNativeDynamic();
        }
        else if (PhotonHandler.PingImplementation == typeof(PingMono))
        {
            ping = new PingMono();  // using this type explicitly saves it from IL2CPP bytecode stripping
        }
        else
        {
            ping = (PhotonPing) Activator.CreateInstance(PhotonHandler.PingImplementation);
        }

        //Debug.Log("Ping is: " + ping + " type " + ping.GetType());

        float rttSum = 0.0f;
        int replyCount = 0;


        // PhotonPing.StartPing() requires a plain IP address without port (on all but Windows 8 platforms).
        // So: remove port and do the DNS-resolving if needed
        string cleanIpOfRegion = region.HostAndPort;
        int indexOfColon = cleanIpOfRegion.LastIndexOf(':');
        if (indexOfColon > 1)
        {
            cleanIpOfRegion = cleanIpOfRegion.Substring(0, indexOfColon);
        }
        cleanIpOfRegion = ResolveHost(cleanIpOfRegion);
        //Debug.Log("Resolved and port-less IP is: " + cleanIpOfRegion);


        for (int i = 0; i < Attempts; i++)
        {
            bool overtime = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                ping.StartPing(cleanIpOfRegion);
            }
            catch (Exception e)
            {
                Debug.Log("catched: " + e);
                this.PingsRunning--;
                break;
            }


            while (!ping.Done())
            {
                if (sw.ElapsedMilliseconds >= MaxMilliseconsPerPing)
                {
                    overtime = true;
                    break;
                }
                yield return 0; // keep this loop tight, to avoid adding local lag to rtt.
            }
            int rtt = (int)sw.ElapsedMilliseconds;


            if (IgnoreInitialAttempt && i == 0)
            {
                // do nothing.
            }
            else if (ping.Successful && !overtime)
            {
                rttSum += rtt;
                replyCount++;
                region.Ping = (int)((rttSum) / replyCount);
                //Debug.Log("region " + region.Code + " RTT " + region.Ping + " success: " + ping.Successful + " over: " + overtime);
            }

            yield return new WaitForSeconds(0.1f);
        }

        this.PingsRunning--;

        //Debug.Log("this.PingsRunning: " + this.PingsRunning + " this debug: " + ping.DebugString);
        yield return null;
    }

#if UNITY_WINRT && !UNITY_EDITOR

    public static string ResolveHost(string hostName)
    {
        return hostName;
    }

#else

    /// <summary>
    /// Attempts to resolve a hostname into an IP string or returns empty string if that fails.
    /// </summary>
    /// <remarks>
    /// To be compatible with most platforms, the address family is checked like this:</br>
    /// if (ipAddress.AddressFamily.ToString().Contains("6")) // ipv6...
    /// </reamrks>
    /// <param name="hostName">Hostname to resolve.</param>
    /// <returns>IP string or empty string if resolution fails</returns>
    public static string ResolveHost(string hostName)
    {
        string ipv4Address = string.Empty;

        try
        {
            IPAddress[] address = Dns.GetHostAddresses(hostName);
            //foreach (IPAddress adr in address)
            //{
            //    Debug.Log(hostName + " -> Adress: " + adr + " family: " + adr.AddressFamily.ToString());
            //}

            if (address.Length == 1)
            {
                return address[0].ToString();
            }

            // if we got more addresses, try to pick a IPv4 one
            for (int index = 0; index < address.Length; index++)
            {
                IPAddress ipAddress = address[index];
                if (ipAddress != null)
                {
                    // checking ipAddress.ToString() means we don't have to import System.Net.Sockets, which is not available on some platforms (Metro)
                    if (ipAddress.ToString().Contains(":"))
                    {
                        return ipAddress.ToString();
                    }
                    if (string.IsNullOrEmpty(ipv4Address))
                    {
                        ipv4Address = address.ToString();
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception caught! " + e.Source + " Message: " + e.Message);
        }

        return ipv4Address;
    }
#endif
}
