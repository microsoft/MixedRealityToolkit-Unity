using System.Text;
using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;

public class SupportLogger : MonoBehaviour
{
    public bool LogTrafficStats = true;

    public void Start()
    {
        GameObject go = GameObject.Find("PunSupportLogger");
        if (go == null)
        {
            go = new GameObject("PunSupportLogger");
            DontDestroyOnLoad(go);
            SupportLogging sl = go.AddComponent<SupportLogging>();
            sl.LogTrafficStats = this.LogTrafficStats;
        }
    }
}

public class SupportLogging : MonoBehaviour
{
    public bool LogTrafficStats;

    public void Start()
    {
        if (LogTrafficStats)
        {
            this.InvokeRepeating("LogStats", 10, 10);
        }
    }


    protected void OnApplicationPause(bool pause)
    {
        Debug.Log("SupportLogger OnApplicationPause: " + pause + " connected: " + PhotonNetwork.connected);
    }

    public void OnApplicationQuit()
    {
        this.CancelInvoke();
    }

    public void LogStats()
    {
        if (this.LogTrafficStats)
        {
            Debug.Log("SupportLogger " + PhotonNetwork.NetworkStatisticsToString());
        }
    }

    private void LogBasics()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("SupportLogger Info: PUN {0}: ", PhotonNetwork.versionPUN);

        sb.AppendFormat("AppID: {0}*** GameVersion: {1} PeerId: {2} ", PhotonNetwork.networkingPeer.AppId.Substring(0, 8), PhotonNetwork.networkingPeer.AppVersion, PhotonNetwork.networkingPeer.PeerID);
        sb.AppendFormat("Server: {0}. Region: {1} ", PhotonNetwork.ServerAddress, PhotonNetwork.networkingPeer.CloudRegion);
        sb.AppendFormat("HostType: {0} ", PhotonNetwork.PhotonServerSettings.HostType);


        Debug.Log(sb.ToString());
    }


    public void OnConnectedToPhoton()
    {
        Debug.Log("SupportLogger OnConnectedToPhoton().");
        this.LogBasics();

        if (LogTrafficStats)
        {
            PhotonNetwork.NetworkStatisticsEnabled = true;
        }
    }

    public void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.Log("SupportLogger OnFailedToConnectToPhoton("+cause+").");
        this.LogBasics();
    }

    public void OnJoinedLobby()
    {
        Debug.Log("SupportLogger OnJoinedLobby(" + PhotonNetwork.lobby + ").");
    }

    public void OnJoinedRoom()
    {
        Debug.Log("SupportLogger OnJoinedRoom(" + PhotonNetwork.room + "). " + PhotonNetwork.lobby + " GameServer:" + PhotonNetwork.ServerAddress);
    }

    public void OnCreatedRoom()
    {
        Debug.Log("SupportLogger OnCreatedRoom(" + PhotonNetwork.room + "). " + PhotonNetwork.lobby + " GameServer:" + PhotonNetwork.ServerAddress);
    }

    public void OnLeftRoom()
    {
        Debug.Log("SupportLogger OnLeftRoom().");
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("SupportLogger OnDisconnectedFromPhoton().");
    }
}
