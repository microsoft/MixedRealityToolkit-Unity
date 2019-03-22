// ----------------------------------------------------------------------------
// <copyright file="RegionHandler.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   The RegionHandler class provides methods to ping a list of regions,
//   to find the one with best ping.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System;
    using System.Net;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY
    using UnityEngine;
    using Debug = UnityEngine.Debug;
    #endif
    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif

    /// <summary>
    /// Provides methods to work with Photon's regions (Photon Cloud) and can be use to find the one with best ping.
    /// </summary>
    /// <remarks>
    /// When a client uses a Name Server to fetch the list of available regions, the LoadBalancingClient will create a RegionHandler
    /// and provide it via the OnRegionListReceived callback.
    ///
    /// Your logic can decide to either connect to one of those regional servers, or it may use PingMinimumOfRegions to test
    /// which region provides the best ping.
    ///
    /// It makes sense to make clients "sticky" to a region when one gets selected.
    /// This can be achieved by storing the SummaryToCache value, once pinging was done.
    /// When the client connects again, the previous SummaryToCache helps limiting the number of regions to ping.
    /// In best case, only the previously selected region gets re-pinged and if the current ping is not much worse, this one region is used again.
    /// </remarks>
    public class RegionHandler
    {
        /// <summary>A list of region names for the Photon Cloud. Set by the result of OpGetRegions().</summary>
        /// <remarks>
        /// Implement ILoadBalancingCallbacks and register for the callbacks to get OnRegionListReceived(RegionHandler regionHandler).
        /// You can also put a "case OperationCode.GetRegions:" into your OnOperationResponse method to notice when the result is available.
        /// </remarks>
        public List<Region> EnabledRegions { get; protected internal set; }

        private string availableRegionCodes;

        private Region bestRegionCache;

        /// <summary>
        /// When PingMinimumOfRegions was called and completed, the BestRegion is identified by best ping.
        /// </summary>
        public Region BestRegion
        {
            get
            {
                if (this.EnabledRegions == null)
                {
                    return null;
                }
                if (this.bestRegionCache != null)
                {
                    return this.bestRegionCache;
                }

                Region result = null;
                int bestRtt = Int32.MaxValue;
                foreach (Region region in this.EnabledRegions)
                {
                    if (region.Ping != 0 && region.Ping < bestRtt)
                    {
                        bestRtt = region.Ping;
                        result = region;
                    }
                }

                this.bestRegionCache = result;
                return result;
            }
        }

        /// <summary>
        /// This value summarizes the results of pinging the currently available EnabledRegions (after PingMinimumOfRegions finished).
        /// </summary>
        public string SummaryToCache
        {
            get
            {
				if (this.BestRegion != null) {
					return this.BestRegion.Code + ";" + this.BestRegion.Ping + ";" + this.availableRegionCodes;
				}

				return this.availableRegionCodes;
            }
        }


        public void SetRegions(OperationResponse opGetRegions)
        {
            if (opGetRegions.OperationCode != OperationCode.GetRegions)
            {
                return;
            }
            if (opGetRegions.ReturnCode != ErrorCode.Ok)
            {
                return;
            }

            string[] regions = opGetRegions[ParameterCode.Region] as string[];
            string[] servers = opGetRegions[ParameterCode.Address] as string[];
            if (regions == null || servers == null || regions.Length != servers.Length)
            {
                //TODO: log error
                //Debug.LogError("The region arrays from Name Server are not ok. Must be non-null and same length. " + (regions == null) + " " + (servers == null) + "\n" + opGetRegions.ToStringFull());
                return;
            }

            this.bestRegionCache = null;
            this.EnabledRegions = new List<Region>(regions.Length);

            for (int i = 0; i < regions.Length; i++)
            {
                Region tmp = new Region(regions[i], servers[i]);
                if (string.IsNullOrEmpty(tmp.Code))
                {
                    continue;
                }

                this.EnabledRegions.Add(tmp);
            }

            Array.Sort(regions);
            this.availableRegionCodes = string.Join(",", regions);
        }

        private List<RegionPinger> pingerList;
        private Action<RegionHandler> onCompleteCall;
        private int previousPing;
        public bool IsPinging { get; private set; }


        public bool PingMinimumOfRegions(Action<RegionHandler> onCompleteCallback, string previousSummary)
        {
            if (this.EnabledRegions == null || this.EnabledRegions.Count == 0)
            {
                //TODO: log error
                //Debug.LogError("No regions available. Maybe all got filtered out or the AppId is not correctly configured.");
                return false;
            }

            if (this.IsPinging)
            {
                //TODO: log warning
                //Debug.LogWarning("PingMinimumOfRegions() skipped, because this RegionHander is already pinging some regions.");
                return false;
            }

            this.IsPinging = true;
            this.onCompleteCall = onCompleteCallback;


            if (string.IsNullOrEmpty(previousSummary))
            {
                return this.PingEnabledRegions();
            }

            string[] values = previousSummary.Split(';');
            if (values.Length < 3)
            {
                return this.PingEnabledRegions();
            }

            int prevBestRegionPing;
            bool secondValueIsInt = Int32.TryParse(values[1], out prevBestRegionPing);
            if (!secondValueIsInt)
            {
                return this.PingEnabledRegions();
            }

            string prevBestRegionCode = values[0];
            string prevAvailableRegionCodes = values[2];


            if (string.IsNullOrEmpty(prevBestRegionCode))
            {
                return this.PingEnabledRegions();
            }
            if (string.IsNullOrEmpty(prevAvailableRegionCodes))
            {
                return this.PingEnabledRegions();
            }
            if (!this.availableRegionCodes.Equals(prevAvailableRegionCodes) || !this.availableRegionCodes.Contains(prevBestRegionCode))
            {
                return this.PingEnabledRegions();
            }
            if (prevBestRegionPing >= RegionPinger.PingWhenFailed)
            {
                return this.PingEnabledRegions();
            }

            // let's check only the preferred region to detect if it's still "good enough"
            this.previousPing = prevBestRegionPing;

            Region preferred = this.EnabledRegions.Find(r => r.Code.Equals(prevBestRegionCode));
            RegionPinger singlePinger = new RegionPinger(preferred, this.OnPreferredRegionPinged);
            singlePinger.Start();

            return true;
        }

        private void OnPreferredRegionPinged(Region preferredRegion)
        {
            if (preferredRegion.Ping > this.previousPing * 1.50f)
            {
                this.PingEnabledRegions();
            }
            else
            {
                this.IsPinging = false;
                this.onCompleteCall(this);
            }
        }


        private bool PingEnabledRegions()
        {
            if (this.EnabledRegions == null || this.EnabledRegions.Count == 0)
            {
                //TODO: log
                //Debug.LogError("No regions available. Maybe all got filtered out or the AppId is not correctly configured.");
                return false;
            }

            this.pingerList = new List<RegionPinger>();
            foreach (Region region in this.EnabledRegions)
            {
                RegionPinger rp = new RegionPinger(region, this.OnRegionDone);
                this.pingerList.Add(rp);
                rp.Start(); // TODO: check return value
            }

            return true;
        }

        private void OnRegionDone(Region region)
        {
            foreach (RegionPinger pinger in this.pingerList)
            {
                if (!pinger.Done)
                {
                    return;
                }
            }

            this.IsPinging = false;
            this.onCompleteCall(this);
        }
    }

    public class RegionPinger
    {
        public static int Attempts = 5;
        public static bool IgnoreInitialAttempt = true;
        public static int MaxMilliseconsPerPing = 800; // enter a value you're sure some server can beat (have a lower rtt)
        public static int PingWhenFailed = Attempts * MaxMilliseconsPerPing;

        private Region region;
        private string regionAddress;
        public int CurrentAttempt = 0;

        public bool Done { get; private set; }
        private Action<Region> onDoneCall;

        private PhotonPing ping;

        #if UNITY_WEBGL
        // for WebGL exports, a coroutine is used to run pings. this is done on a temporary game object/monobehaviour
        private MonoBehaviour coroutineMonoBehaviour;
        #endif


        public RegionPinger(Region region, Action<Region> onDoneCallback)
        {
            this.region = region;
            this.region.Ping = PingWhenFailed;
            this.Done = false;
            this.onDoneCall = onDoneCallback;
        }

        private PhotonPing GetPingImplementation()
        {
            PhotonPing ping = null;

            #if !NETFX_CORE
            if (LoadBalancingPeer.PingImplementation == typeof(PingMono))
            {
                ping = new PingMono(); // using this type explicitly saves it from IL2CPP bytecode stripping
            }
            #endif
            #if NATIVE_SOCKETS
            if (LoadBalancingPeer.PingImplementation == typeof(PingNativeDynamic))
            {
                ping = new PingNativeDynamic();
            }
            #endif
            #if UNITY_WEBGL
            if (LoadBalancingPeer.PingImplementation == typeof(PingHttp))
            {
                ping = new PingHttp();
            }
            #endif

            if (ping == null)
            {
                ping = (PhotonPing)Activator.CreateInstance(LoadBalancingPeer.PingImplementation);
            }

            return ping;
        }


        public bool Start()
        {
            // all addresses for Photon region servers will contain a :port ending. this needs to be removed first.
            // PhotonPing.StartPing() requires a plain (IP) address without port or protocol-prefix (on all but Windows 8.1 and WebGL platforms).
            string address = this.region.HostAndPort;
            int indexOfColon = address.LastIndexOf(':');
            if (indexOfColon > 1)
            {
                address = address.Substring(0, indexOfColon);
            }
            this.regionAddress = ResolveHost(address);


            this.ping = this.GetPingImplementation();


            this.Done = false;
            this.CurrentAttempt = 0;

            #if UNITY_WEBGL
            GameObject go = new GameObject();
            go.name = "RegionPing_" + this.region.Code + "_" + this.region.Cluster;
            this.coroutineMonoBehaviour = go.AddComponent<MonoBehaviourEmpty>();        // is defined below, as special case for Unity WegGL
            this.coroutineMonoBehaviour.StartCoroutine(this.RegionPingCoroutine());
            #else
            SupportClass.StartBackgroundCalls(this.RegionPingThreaded, 0, "RegionPing_" + this.region.Code+"_"+this.region.Cluster);
            #endif

            return true;
        }

        protected internal bool RegionPingThreaded()
        {
            this.region.Ping = PingWhenFailed;

            float rttSum = 0.0f;
            int replyCount = 0;


            Stopwatch sw = new Stopwatch();
            for (this.CurrentAttempt = 0; this.CurrentAttempt < Attempts; this.CurrentAttempt++)
            {
                bool overtime = false;
                sw.Reset();
                sw.Start();

                try
                {
                    this.ping.StartPing(this.regionAddress);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("RegionPinger.RegionPingThreaded() catched an exception for ping.StartPing(). Exception: " + e + " Source: " + e.Source + " Message: " + e.Message);
                    break;
                }


                while (!this.ping.Done())
                {
                    if (sw.ElapsedMilliseconds >= MaxMilliseconsPerPing)
                    {
                        overtime = true;
                        break;
                    }
                    #if !NETFX_CORE
                    System.Threading.Thread.Sleep(0);
                    #endif
                }


                sw.Stop();
                int rtt = (int)sw.ElapsedMilliseconds;


                if (IgnoreInitialAttempt && this.CurrentAttempt == 0)
                {
                    // do nothing.
                }
                else if (this.ping.Successful && !overtime)
                {
                    rttSum += rtt;
                    replyCount++;
                    this.region.Ping = (int)((rttSum) / replyCount);
                }

                #if !NETFX_CORE
                System.Threading.Thread.Sleep(10);
                #endif
            }

            this.Done = true;
            this.onDoneCall(this.region);

            return false;
        }


        #if SUPPORTED_UNITY
        /// <remarks>
        /// Affected by frame-rate of app, as this Coroutine checks the socket for a result once per frame.
        /// </remarks>
        protected internal IEnumerator RegionPingCoroutine()
        {
            this.region.Ping = PingWhenFailed;

            float rttSum = 0.0f;
            int replyCount = 0;


            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < Attempts; i++)
            {
                bool overtime = false;
                sw.Reset();
                sw.Start();

                try
                {
                    this.ping.StartPing(this.regionAddress);
                }
                catch (Exception e)
                {
                    Debug.Log("catched: " + e);
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


                sw.Stop();
                int rtt = (int) sw.ElapsedMilliseconds;


                if (IgnoreInitialAttempt && i == 0)
                {
                    // do nothing.
                }
                else if (this.ping.Successful && !overtime)
                {
                    rttSum += rtt;
                    replyCount++;
                    this.region.Ping = (int) ((rttSum) / replyCount);
                }

                yield return new WaitForSeconds(0.1f);
            }


            #if UNITY_WEBGL
            GameObject.Destroy(this.coroutineMonoBehaviour.gameObject);   // this method runs as coroutine on a temp object, which gets destroyed now.
            #endif

            this.Done = true;
            this.onDoneCall(this.region);
            yield return null;
        }
        #endif

        /// <summary>
        /// Attempts to resolve a hostname into an IP string or returns empty string if that fails.
        /// </summary>
        /// <remarks>
        /// To be compatible with most platforms, the address family is checked like this:<br/>
        /// if (ipAddress.AddressFamily.ToString().Contains("6")) // ipv6...
        /// </remarks>
        /// <param name="hostName">Hostname to resolve.</param>
        /// <returns>IP string or empty string if resolution fails</returns>
        public static string ResolveHost(string hostName)
        {

			if (hostName.StartsWith("wss://"))
			{
				hostName = hostName.Substring(6);
			}
			if (hostName.StartsWith("ws://"))
			{
				hostName = hostName.Substring(5);
			}

            string ipv4Address = string.Empty;

            try
            {
                #if UNITY_WSA || NETFX_CORE || UNITY_WEBGL
                return hostName;
                #else
                IPAddress[] address = Dns.GetHostAddresses(hostName);
                if (address.Length == 1)
                {
                    return address[0].ToString();
                }

                // if we got more addresses, try to pick a IPv6 one
                // checking ipAddress.ToString() means we don't have to import System.Net.Sockets, which is not available on some platforms (Metro)
                for (int index = 0; index < address.Length; index++)
                {
                    IPAddress ipAddress = address[index];
                    if (ipAddress != null)
                    {
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
                #endif
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("RegionPinger.ResolveHost() catched an exception for Dns.GetHostAddresses(). Exception: " + e + " Source: " + e.Source + " Message: " + e.Message);
            }

            return ipv4Address;
        }
    }

    #if UNITY_WEBGL
    internal class MonoBehaviourEmpty : MonoBehaviour { }
    #endif
}