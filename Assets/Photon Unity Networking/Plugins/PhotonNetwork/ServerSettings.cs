#pragma warning disable 1587
/// \file
/// <summary>ScriptableObject defining a server setup. An instance is created as <b>PhotonServerSettings</b>.</summary>
#pragma warning restore 1587

#define PHOTON_VOICE

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;


public class Region
{
    public CloudRegionCode Code;
    public string HostAndPort;
    public int Ping;

    public static CloudRegionCode Parse(string codeAsString)
    {
        codeAsString = codeAsString.ToLower();

        CloudRegionCode code = CloudRegionCode.none;
        if (Enum.IsDefined(typeof(CloudRegionCode), codeAsString))
        {
            code = (CloudRegionCode)Enum.Parse(typeof(CloudRegionCode), codeAsString);
        }

        return code;
    }

    internal static CloudRegionFlag ParseFlag(string codeAsString)
    {
        codeAsString = codeAsString.ToLower();

        CloudRegionFlag code = 0;
        if (Enum.IsDefined(typeof(CloudRegionFlag), codeAsString))
        {
            code = (CloudRegionFlag)Enum.Parse(typeof(CloudRegionFlag), codeAsString);
        }

        return code;
    }

    public override string ToString()
    {
        return string.Format("'{0}' \t{1}ms \t{2}", this.Code, this.Ping, this.HostAndPort);
    }
}


/// <summary>
/// Collection of connection-relevant settings, used internally by PhotonNetwork.ConnectUsingSettings.
/// </summary>
[Serializable]
public class ServerSettings : ScriptableObject
{
    public enum HostingOption { NotSet = 0, PhotonCloud = 1, SelfHosted = 2, OfflineMode = 3, BestRegion = 4 }
    public HostingOption HostType = HostingOption.NotSet;
    public ConnectionProtocol Protocol = ConnectionProtocol.Udp;

    // custom server values (not used for PhotonCloud)
    public string ServerAddress = "";     // the address to be used (including region-suffix)
    public int ServerPort = 5055;
#if PHOTON_VOICE    
    public int VoiceServerPort = 5055;
#endif
    public string AppID = "";
    public string VoiceAppID = "";

    public CloudRegionCode PreferredRegion;
    public CloudRegionFlag EnabledRegions = (CloudRegionFlag)(-1);

    public bool JoinLobby;
    public bool EnableLobbyStatistics;

    public List<string> RpcList = new List<string>();   // set by scripts and or via Inspector

    [HideInInspector]
    public bool DisableAutoOpenWizard;


    public void UseCloudBestRegion(string cloudAppid)
    {
        this.HostType = HostingOption.BestRegion;
        this.AppID = cloudAppid;
    }

    public void UseCloud(string cloudAppid)
    {
        this.HostType = HostingOption.PhotonCloud;
        this.AppID = cloudAppid;
    }

    public void UseCloud(string cloudAppid, CloudRegionCode code)
    {
        this.HostType = HostingOption.PhotonCloud;
        this.AppID = cloudAppid;
        this.PreferredRegion = code;
    }

    public void UseMyServer(string serverAddress, int serverPort, string application)
    {
        this.HostType = HostingOption.SelfHosted;
        this.AppID = (application != null) ? application : "master";

        this.ServerAddress = serverAddress;
        this.ServerPort = serverPort;
    }

    public override string ToString()
    {
        return "ServerSettings: " + HostType + " " + ServerAddress;
    }
}
