#pragma warning disable 1587
/// \file
/// <summary>Part of the [Optional GUI](@ref optionalGui).</summary>
#pragma warning restore 1587


using ExitGames.Client.Photon;
using UnityEngine;


/// <summary>
/// Basic GUI to show traffic and health statistics of the connection to Photon, 
/// toggled by shift+tab.
/// </summary>
/// <remarks>
/// The shown health values can help identify problems with connection losses or performance.
/// Example: 
/// If the time delta between two consecutive SendOutgoingCommands calls is a second or more,
/// chances rise for a disconnect being caused by this (because acknowledgements to the server
/// need to be sent in due time).
/// </remarks>
/// \ingroup optionalGui
public class PhotonStatsGui : MonoBehaviour
{
    /// <summary>Shows or hides GUI (does not affect if stats are collected).</summary>
    public bool statsWindowOn = true;

    /// <summary>Option to turn collecting stats on or off (used in Update()).</summary>
    public bool statsOn = true;

    /// <summary>Shows additional "health" values of connection.</summary>
    public bool healthStatsVisible;

    /// <summary>Shows additional "lower level" traffic stats.</summary>
    public bool trafficStatsOn;

    /// <summary>Show buttons to control stats and reset them.</summary>
    public bool buttonsOn;

    /// <summary>Positioning rect for window.</summary>
    public Rect statsRect = new Rect(0, 100, 200, 50);

    /// <summary>Unity GUI Window ID (must be unique or will cause issues).</summary>
    public int WindowId = 100;


    public void Start()
    {
        if (this.statsRect.x <= 0)
        {
            this.statsRect.x = Screen.width - this.statsRect.width;
        }
    }

    /// <summary>Checks for shift+tab input combination (to toggle statsOn).</summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            this.statsWindowOn = !this.statsWindowOn;
            this.statsOn = true;    // enable stats when showing the window
        }
    }

    public void OnGUI()
    {
        if (PhotonNetwork.networkingPeer.TrafficStatsEnabled != statsOn)
        {
            PhotonNetwork.networkingPeer.TrafficStatsEnabled = this.statsOn;
        }

        if (!this.statsWindowOn)
        {
            return;
        }

        this.statsRect = GUILayout.Window(this.WindowId, this.statsRect, this.TrafficStatsWindow, "Messages (shift+tab)");
    }

    public void TrafficStatsWindow(int windowID)
    {
        bool statsToLog = false;
        TrafficStatsGameLevel gls = PhotonNetwork.networkingPeer.TrafficStatsGameLevel;
        long elapsedMs = PhotonNetwork.networkingPeer.TrafficStatsElapsedMs / 1000;
        if (elapsedMs == 0)
        {
            elapsedMs = 1;
        }

        GUILayout.BeginHorizontal();
        this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons");
        this.healthStatsVisible = GUILayout.Toggle(this.healthStatsVisible, "health");
        this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic");
        GUILayout.EndHorizontal();
        
        string total = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", gls.TotalOutgoingMessageCount, gls.TotalIncomingMessageCount, gls.TotalMessageCount);
        string elapsedTime = string.Format("{0}sec average:", elapsedMs);
        string average = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", gls.TotalOutgoingMessageCount / elapsedMs, gls.TotalIncomingMessageCount / elapsedMs, gls.TotalMessageCount / elapsedMs);
        GUILayout.Label(total);
        GUILayout.Label(elapsedTime);
        GUILayout.Label(average);

        if (this.buttonsOn)
        {
            GUILayout.BeginHorizontal();
            this.statsOn = GUILayout.Toggle(this.statsOn, "stats on");
            if (GUILayout.Button("Reset"))
            {
                PhotonNetwork.networkingPeer.TrafficStatsReset();
                PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
            }
            statsToLog = GUILayout.Button("To Log");
            GUILayout.EndHorizontal();
        }

        string trafficStatsIn = string.Empty;
        string trafficStatsOut = string.Empty;
        if (this.trafficStatsOn)
        {
            trafficStatsIn = "Incoming: " + PhotonNetwork.networkingPeer.TrafficStatsIncoming.ToString();
            trafficStatsOut = "Outgoing: " + PhotonNetwork.networkingPeer.TrafficStatsOutgoing.ToString();
            GUILayout.Label(trafficStatsIn);
            GUILayout.Label(trafficStatsOut);
        }

        string healthStats = string.Empty;
        if (this.healthStatsVisible)
        {
            healthStats = string.Format(
                "ping: {6}[+/-{7}]ms resent:{8}\nmax ms between\nsend: {0,4} dispatch: {1,4}\nlongest dispatch for:\nev({3}):{2,3}ms op({5}):{4,3}ms",
                gls.LongestDeltaBetweenSending,
                gls.LongestDeltaBetweenDispatching,
                gls.LongestEventCallback,
                gls.LongestEventCallbackCode,
                gls.LongestOpResponseCallback,
                gls.LongestOpResponseCallbackOpCode,
                PhotonNetwork.networkingPeer.RoundTripTime,
                PhotonNetwork.networkingPeer.RoundTripTimeVariance,
                PhotonNetwork.networkingPeer.ResentReliableCommands);
            GUILayout.Label(healthStats);
        }

        if (statsToLog)
        {
            string complete = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", total, elapsedTime, average, trafficStatsIn, trafficStatsOut, healthStats);
            Debug.Log(complete);
        }

        // if anything was clicked, the height of this window is likely changed. reduce it to be layouted again next frame
        if (GUI.changed)
        {
            this.statsRect.height = 100;
        }

        GUI.DragWindow();
    }
}
