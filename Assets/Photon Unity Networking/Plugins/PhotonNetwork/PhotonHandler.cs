// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonHandler.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3) || UNITY_6
#define UNITY_MIN_5_4
#endif

using System;
using System.Collections;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using SupportClassPun = ExitGames.Client.Photon.SupportClass;

#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

/// <summary>
/// Internal Monobehaviour that allows Photon to run an Update loop.
/// </summary>
internal class PhotonHandler : Photon.MonoBehaviour
{
    public static PhotonHandler SP;

    public int updateInterval;  // time [ms] between consecutive SendOutgoingCommands calls

    public int updateIntervalOnSerialize;  // time [ms] between consecutive RunViewUpdate calls (sending syncs, etc)

    private int nextSendTickCount = 0;

    private int nextSendTickCountOnSerialize = 0;

    private static bool sendThreadShouldRun;

    private static Stopwatch timerToStopConnectionInBackground;

    protected internal static bool AppQuits;

    protected internal static Type PingImplementation = null;

    protected void Awake()
    {
        if (SP != null && SP != this && SP.gameObject != null)
        {
            GameObject.DestroyImmediate(SP.gameObject);
        }

        SP = this;
        DontDestroyOnLoad(this.gameObject);

        this.updateInterval = 1000 / PhotonNetwork.sendRate;
        this.updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;

        PhotonHandler.StartFallbackSendAckThread();
    }


    #if UNITY_MIN_5_4

    protected void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            PhotonNetwork.networkingPeer.NewSceneLoaded();
            PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
        };
    }
    
    #else
    
    /// <summary>Called by Unity after a new level was loaded.</summary>
    protected void OnLevelWasLoaded(int level)
    {
        PhotonNetwork.networkingPeer.NewSceneLoaded();
        PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
    }

    #endif


    /// <summary>Called by Unity when the application is closed. Disconnects.</summary>
    protected void OnApplicationQuit()
    {
        PhotonHandler.AppQuits = true;
        PhotonHandler.StopFallbackSendAckThread();
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// Called by Unity when the application gets paused (e.g. on Android when in background).
    /// </summary>
    /// <remarks>
    /// Some versions of Unity will give false values for pause on Android (and possibly on other platforms).
    /// Sets a disconnect timer when PhotonNetwork.BackgroundTimeout > 0.001f.
    /// </remarks>
    /// <param name="pause"></param>
    protected void OnApplicationPause(bool pause)
    {
        if (PhotonNetwork.BackgroundTimeout > 0.001f)
        {
            if (timerToStopConnectionInBackground == null)
            {
                timerToStopConnectionInBackground = new Stopwatch();
            }
            timerToStopConnectionInBackground.Reset();

            if (pause)
            {
                timerToStopConnectionInBackground.Start();
            }
            else
            {
                timerToStopConnectionInBackground.Stop();
            }
        }
    }

    /// <summary>Called by Unity when the play mode ends. Used to cleanup.</summary>
    protected void OnDestroy()
    {
        //Debug.Log("OnDestroy on PhotonHandler.");
        PhotonHandler.StopFallbackSendAckThread();
        //PhotonNetwork.Disconnect();
    }

    protected void Update()
    {
        if (PhotonNetwork.networkingPeer == null)
        {
            Debug.LogError("NetworkPeer broke!");
            return;
        }

        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated || PhotonNetwork.connectionStateDetailed == ClientState.Disconnected || PhotonNetwork.offlineMode)
        {
            return;
        }

        // the messageQueue might be paused. in that case a thread will send acknowledgements only. nothing else to do here.
        if (!PhotonNetwork.isMessageQueueRunning)
        {
            return;
        }

        bool doDispatch = true;
        while (PhotonNetwork.isMessageQueueRunning && doDispatch)
        {
            // DispatchIncomingCommands() returns true of it found any command to dispatch (event, result or state change)
            Profiler.BeginSample("DispatchIncomingCommands");
            doDispatch = PhotonNetwork.networkingPeer.DispatchIncomingCommands();
            Profiler.EndSample();
        }

        int currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000);  // avoiding Environment.TickCount, which could be negative on long-running platforms
        if (PhotonNetwork.isMessageQueueRunning && currentMsSinceStart > this.nextSendTickCountOnSerialize)
        {
            PhotonNetwork.networkingPeer.RunViewUpdate();
            this.nextSendTickCountOnSerialize = currentMsSinceStart + this.updateIntervalOnSerialize;
            this.nextSendTickCount = 0;     // immediately send when synchronization code was running
        }

        currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000);
        if (currentMsSinceStart > this.nextSendTickCount)
        {
            bool doSend = true;
            while (PhotonNetwork.isMessageQueueRunning && doSend)
            {
                // Send all outgoing commands
                Profiler.BeginSample("SendOutgoingCommands");
                doSend = PhotonNetwork.networkingPeer.SendOutgoingCommands();
                Profiler.EndSample();
            }

            this.nextSendTickCount = currentMsSinceStart + this.updateInterval;
        }
    }

    protected void OnJoinedRoom()
    {
        PhotonNetwork.networkingPeer.LoadLevelIfSynced();
    }

    protected void OnCreatedRoom()
    {
        PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
    }

    public static void StartFallbackSendAckThread()
    {
#if !UNITY_WEBGL
        if (sendThreadShouldRun)
        {
            return;
        }

        sendThreadShouldRun = true;
        SupportClassPun.CallInBackground(FallbackSendAckThread);   // thread will call this every 100ms until method returns false
#endif
    }

    public static void StopFallbackSendAckThread()
    {
#if !UNITY_WEBGL
        sendThreadShouldRun = false;
#endif
    }

    public static bool FallbackSendAckThread()
    {
        if (sendThreadShouldRun && PhotonNetwork.networkingPeer != null)
        {
            // check if the client should disconnect after some seconds in background
            if (timerToStopConnectionInBackground != null && PhotonNetwork.BackgroundTimeout > 0.001f)
            {
                if (timerToStopConnectionInBackground.ElapsedMilliseconds > PhotonNetwork.BackgroundTimeout * 1000)
                {
                    return sendThreadShouldRun;
                }
            }

            PhotonNetwork.networkingPeer.SendAcksOnly();
        }

        return sendThreadShouldRun;
    }


    #region Photon Cloud Ping Evaluation


    private const string PlayerPrefsKey = "PUNCloudBestRegion";

    internal static CloudRegionCode BestRegionCodeCurrently = CloudRegionCode.none; // default to none
    internal static CloudRegionCode BestRegionCodeInPreferences
    {
        get
        {
            string prefsRegionCode = PlayerPrefs.GetString(PlayerPrefsKey, "");
            if (!string.IsNullOrEmpty(prefsRegionCode))
            {
                CloudRegionCode loadedRegion = Region.Parse(prefsRegionCode);
                return loadedRegion;
            }

            return CloudRegionCode.none;
        }
        set
        {
            if (value == CloudRegionCode.none)
            {
                PlayerPrefs.DeleteKey(PlayerPrefsKey);
            }
            else
            {
                PlayerPrefs.SetString(PlayerPrefsKey, value.ToString());
            }
        }
    }


    internal protected static void PingAvailableRegionsAndConnectToBest()
    {
        SP.StartCoroutine(SP.PingAvailableRegionsCoroutine(true));
    }


    internal IEnumerator PingAvailableRegionsCoroutine(bool connectToBest)
    {
        BestRegionCodeCurrently = CloudRegionCode.none;
        while (PhotonNetwork.networkingPeer.AvailableRegions == null)
        {
            if (PhotonNetwork.connectionStateDetailed != ClientState.ConnectingToNameServer && PhotonNetwork.connectionStateDetailed != ClientState.ConnectedToNameServer)
            {
                Debug.LogError("Call ConnectToNameServer to ping available regions.");
                yield break; // break if we don't connect to the nameserver at all
            }

            Debug.Log("Waiting for AvailableRegions. State: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.Server + " PhotonNetwork.networkingPeer.AvailableRegions " + (PhotonNetwork.networkingPeer.AvailableRegions != null));
            yield return new WaitForSeconds(0.25f); // wait until pinging finished (offline mode won't ping)
        }

        if (PhotonNetwork.networkingPeer.AvailableRegions == null || PhotonNetwork.networkingPeer.AvailableRegions.Count == 0)
        {
            Debug.LogError("No regions available. Are you sure your appid is valid and setup?");
            yield break; // break if we don't get regions at all
        }

        PhotonPingManager pingManager = new PhotonPingManager();
        foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
        {
            SP.StartCoroutine(pingManager.PingSocket(region));
        }

        while (!pingManager.Done)
        {
            yield return new WaitForSeconds(0.1f); // wait until pinging finished (offline mode won't ping)
        }


        Region best = pingManager.BestRegion;
        PhotonHandler.BestRegionCodeCurrently = best.Code;
        PhotonHandler.BestRegionCodeInPreferences = best.Code;

        Debug.Log("Found best region: " + best.Code + " ping: " + best.Ping + ". Calling ConnectToRegionMaster() is: " + connectToBest);


        if (connectToBest)
        {
            PhotonNetwork.networkingPeer.ConnectToRegionMaster(best.Code);
        }
    }



    #endregion

}
