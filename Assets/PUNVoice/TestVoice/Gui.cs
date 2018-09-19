using UnityEngine;
using System.Collections;
using System;

public class Gui : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
		PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsEnabled = true;
        //PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut = ExitGames.Client.Photon.DebugLevel.ALL;
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

	float dataRateNextTime = 0;
    int prevInBytes;
    int prevOutBytes;
    int dataRateIn;
    int dataRateOut;
	// Update is called once per frame
	void OnGUI()
    {
        // TODO: better way to ref recorder
        PhotonVoiceRecorder rec = null;
        foreach (var r in FindObjectsOfType<PhotonVoiceRecorder>())
        {
            if (r.photonView.isMine)
            {
                rec = r;
                break;
            }
        }
        
        var lStyle = new GUIStyle("label");
        lStyle.fontSize = 24 * Screen.height / 600;
        lStyle.wordWrap = false;
        var lStyleSmall = new GUIStyle("label");
        lStyleSmall.fontSize = 16 * Screen.height / 600;
        lStyleSmall.wordWrap = false;
        var bStyle = new GUIStyle("button");
        bStyle.fontSize = 28 * Screen.height / 600;
        var bStyleSmall = new GUIStyle("button");
        bStyleSmall.fontSize = 16 * Screen.height / 600;

        var roomName = "";
        if (PhotonNetwork.inRoom)
        {
            roomName = PhotonNetwork.room.name;
        }
        string rttString = String.Format(
            "RTT/Var/Que: {0}/{1}/{2}",
            PhotonNetwork.networkingPeer.RoundTripTime.ToString(),
            PhotonNetwork.networkingPeer.RoundTripTimeVariance,
            PhotonNetwork.networkingPeer.QueuedIncomingCommands);
        GUILayout.Label("PUN: " + PhotonNetwork.connectionStateDetailed.ToString() + " " + roomName + " " + rttString, lStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect", bStyle))
        {
#if UNITY_5_3
                PhotonNetwork.ConnectUsingSettings(string.Format("1.{0}", UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex));
#else
                PhotonNetwork.ConnectUsingSettings(string.Format("1.{0}", Application.loadedLevel));
#endif        
        }
        if (GUILayout.Button("Disconnect", bStyle))
        {
            PhotonNetwork.Disconnect();
        }
        GUILayout.EndHorizontal();
        roomName = "";
        if (PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined) 
        {
            roomName = PhotonVoiceNetwork.CurrentRoomName;
        }

        if (dataRateNextTime < Time.time)
        {
            dataRateNextTime = Time.time + 1;
            dataRateIn = (PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsIncoming.TotalPacketBytes - prevInBytes)/1;
            dataRateOut = (PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsOutgoing.TotalPacketBytes - prevOutBytes)/1;
            prevInBytes = PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsIncoming.TotalPacketBytes;
            prevOutBytes = PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsOutgoing.TotalPacketBytes;
        }

        rttString = String.Format(
            "RTT/Var/Que: {0}/{1}/{2}",
            PhotonVoiceNetwork.Client.loadBalancingPeer.RoundTripTime.ToString(),
            PhotonVoiceNetwork.Client.loadBalancingPeer.RoundTripTimeVariance,
            PhotonVoiceNetwork.Client.loadBalancingPeer.QueuedIncomingCommands);
        GUILayout.Label("PhotonVoice: " + PhotonVoiceNetwork.ClientState.ToString() + " " + roomName + " " + rttString, lStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Data rate in/out bytes/sec: " + dataRateIn + "/" + dataRateOut, lStyleSmall);
        if (PhotonVoiceNetwork.Client.loadBalancingPeer != null)
        {
            GUILayout.Label("Traffic bytes: " + PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsIncoming.TotalPacketBytes + "/" + PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsOutgoing.TotalPacketBytes, lStyleSmall);
        }
        GUILayout.Label("Frames Sent/Rcvd/Lost: " + PhotonVoiceNetwork.Client.FramesSent + "/" + PhotonVoiceNetwork.Client.FramesReceived + "/" + PhotonVoiceNetwork.Client.FramesLost, lStyleSmall);
        GUILayout.Label("Voice RTT/Var: " + PhotonVoiceNetwork.Client.RoundTripTime + "/" + PhotonVoiceNetwork.Client.RoundTripTimeVariance, lStyleSmall);
        
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        GUILayout.Label("Speakers:", lStyleSmall);
        foreach (var s in FindObjectsOfType<PhotonVoiceSpeaker>())
        {
            if (s.IsVoiceLinked)
            {
                GUILayout.Label("lag=" + s.CurrentBufferLag, lStyleSmall);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect", bStyle))
        {
            PhotonVoiceNetwork.Connect();
        }
        if (GUILayout.Button("Disconnect", bStyle))
        {
            PhotonVoiceNetwork.Disconnect();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button((PhotonVoiceNetwork.Client.DebugEchoMode ? "[X] " : "[ ] ")  + "Debug Echo", bStyle))
        {
            PhotonVoiceNetwork.Client.DebugEchoMode = !PhotonVoiceNetwork.Client.DebugEchoMode;
        }

        if (rec != null && rec.photonView.isMine)
        {
            
            if (GUILayout.Button((rec.Transmit ? "[X] ": "[ ] ") + "Transmit", bStyle))
            {
                rec.Transmit = !rec.Transmit;
            }
            if (GUILayout.Button((rec.VoiceDetector.On ? "[X] " : "[ ] ") + "Detect", bStyle))
            {
                rec.Detect = !rec.Detect;
            }            
            if (GUILayout.Button((rec.VoiceDetectorCalibrating ? "[X] " : "[ ] ") + "Calibrate Detector", bStyle))
            {
                rec.VoiceDetectorCalibrate(2000);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Transmitting: " + rec.IsTransmitting, lStyleSmall);
            GUILayout.Label("Avg Amp: " + (rec.LevelMeter == null ? "" : rec.LevelMeter.CurrentAvgAmp.ToString("0.000000") + "/" + rec.LevelMeter.AccumAvgPeakAmp.ToString("0.000000")), lStyleSmall);
            GUILayout.Label("Peak Amp: " + (rec.LevelMeter == null ? "" : rec.LevelMeter.CurrentPeakAmp.ToString("0.000000")), lStyleSmall);
            GUILayout.Label("Detector Threshold: " + (rec.VoiceDetector == null ? "" : rec.VoiceDetector.Threshold.ToString("0.000000")), lStyleSmall);
            GUILayout.Label("Audio group (rec): " + rec.AudioGroup.ToString(), lStyleSmall);
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set Group (offs Debug Echo): ", lStyleSmall);
        for (byte i = 0; i < 5; i++)
        {
            if (GUILayout.Button((PhotonVoiceNetwork.Client.GlobalAudioGroup == i ? "[X] " : "[ ] ")  + (i == 0 ? "No" : i.ToString()), bStyleSmall))
            {
                PhotonVoiceNetwork.Client.GlobalAudioGroup = i;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
		GUILayout.Label("Mic: ", lStyleSmall);
        foreach (var x in Microphone.devices)
        {            
            if (GUILayout.Button((PhotonVoiceNetwork.MicrophoneDevice == x ? "[X] " : "[ ] ") + x, bStyleSmall))
            {
                PhotonVoiceNetwork.MicrophoneDevice = x;
            }
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;        
	}    
}
