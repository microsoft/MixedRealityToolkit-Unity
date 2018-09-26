// ----------------------------------------------------------------------------
// <copyright file="ServerSettingsInspector.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the ServerSettings scriptable object.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#define PHOTON_VOICE

using System;
using ExitGames.Client.Photon;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (ServerSettings))]
public class ServerSettingsInspector : Editor
{
    public enum ProtocolChoices
    {
        Udp = ConnectionProtocol.Udp,
        Tcp = ConnectionProtocol.Tcp
    } // has to be extended when rHTTP becomes available

    private bool showMustHaveRegion;
    private bool showAppIdHint;
#if PHOTON_VOICE
    private bool showVoiceAppIdHint;
#endif

    public override void OnInspectorGUI()
    {
        ServerSettings settings = (ServerSettings) target;
        Undo.RecordObject(settings, "Edit PhotonServerSettings");

        settings.HostType = (ServerSettings.HostingOption) EditorGUILayout.EnumPopup("Hosting", settings.HostType);
        EditorGUI.indentLevel = 1;

        switch (settings.HostType)
        {
            case ServerSettings.HostingOption.BestRegion:
            case ServerSettings.HostingOption.PhotonCloud:
                // region selection
                if (settings.HostType == ServerSettings.HostingOption.PhotonCloud)
                {
                    settings.PreferredRegion = (CloudRegionCode)EditorGUILayout.EnumPopup("Region", settings.PreferredRegion);
                }
                else
                {
                    CloudRegionFlag valRegions = (CloudRegionFlag)EditorGUILayout.EnumMaskField("Enabled Regions", settings.EnabledRegions);

                    if (valRegions != settings.EnabledRegions)
                    {
                        settings.EnabledRegions = valRegions;
                        this.showMustHaveRegion = valRegions == 0;
                    }
                    if (this.showMustHaveRegion)
                    {
                        EditorGUILayout.HelpBox("You should enable at least two regions for 'Best Region' hosting.", MessageType.Warning);
                    }
                }

                // appid
                string valAppId = EditorGUILayout.TextField("AppId", settings.AppID);
                if (valAppId != settings.AppID)
                {
                    settings.AppID = valAppId;
                    this.showAppIdHint = !IsAppId(settings.AppID);
                }
                if (this.showAppIdHint)
                {
                    EditorGUILayout.HelpBox("The Photon Cloud needs an AppId (GUID) set.\nYou can find it online in your Dashboard.", MessageType.Warning);
                }

                // protocol
                ProtocolChoices valProtocol = settings.Protocol == ConnectionProtocol.Tcp ? ProtocolChoices.Tcp : ProtocolChoices.Udp;
                valProtocol = (ProtocolChoices) EditorGUILayout.EnumPopup("Protocol", valProtocol);
                settings.Protocol = (ConnectionProtocol) valProtocol;
                #if UNITY_WEBGL
                EditorGUILayout.HelpBox("WebGL always use Secure WebSockets as protocol.\nThis setting gets ignored in current export.", MessageType.Warning);
                #endif
                break;

            case ServerSettings.HostingOption.SelfHosted:
                // address and port (depends on protocol below)
                bool hidePort = false;
                if (settings.Protocol == ConnectionProtocol.Udp && (settings.ServerPort == 4530 || settings.ServerPort == 0))
                {
                    settings.ServerPort = 5055;
                }
                else if (settings.Protocol == ConnectionProtocol.Tcp && (settings.ServerPort == 5055 || settings.ServerPort == 0))
                {
                    settings.ServerPort = 4530;
                }
                #if RHTTP
                if (settings.Protocol == ConnectionProtocol.RHttp)
                {
                    settings.ServerPort = 0;
                    hidePort = true;
                }
                #endif
                settings.ServerAddress = EditorGUILayout.TextField("Server Address", settings.ServerAddress);
                settings.ServerAddress = settings.ServerAddress.Trim();
                if (!hidePort)
                {
                    settings.ServerPort = EditorGUILayout.IntField("Server Port", settings.ServerPort);
                }
                // protocol
                valProtocol = settings.Protocol == ConnectionProtocol.Tcp ? ProtocolChoices.Tcp : ProtocolChoices.Udp;
                valProtocol = (ProtocolChoices) EditorGUILayout.EnumPopup("Protocol", valProtocol);
                settings.Protocol = (ConnectionProtocol) valProtocol;
                #if UNITY_WEBGL
                EditorGUILayout.HelpBox("WebGL always use Secure WebSockets as protocol.\nThis setting gets ignored in current export.", MessageType.Warning);
                #endif

                // appid
                settings.AppID = EditorGUILayout.TextField("AppId", settings.AppID);
                break;

            case ServerSettings.HostingOption.OfflineMode:
                EditorGUI.indentLevel = 0;
                EditorGUILayout.HelpBox("In 'Offline Mode', the client does not communicate with a server.\nAll settings are hidden currently.", MessageType.Info);
                break;

            case ServerSettings.HostingOption.NotSet:
                EditorGUI.indentLevel = 0;
                EditorGUILayout.HelpBox("Hosting is 'Not Set'.\nConnectUsingSettings() will not be able to connect.\nSelect another option or run the PUN Wizard.", MessageType.Info);
                break;

            default:
                DrawDefaultInspector();
                break;
        }

        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Client Settings");
        EditorGUI.indentLevel = 1;
        //EditorGUILayout.LabelField("game version");
        settings.JoinLobby = EditorGUILayout.Toggle("Auto-Join Lobby", settings.JoinLobby);
        settings.EnableLobbyStatistics = EditorGUILayout.Toggle("Enable Lobby Stats", settings.EnableLobbyStatistics);
        //EditorGUILayout.LabelField("automaticallySyncScene");
        //EditorGUILayout.LabelField("autoCleanUpPlayerObjects");
        //EditorGUILayout.LabelField("log level");
        //EditorGUILayout.LabelField("lobby stats");
        //EditorGUILayout.LabelField("sendrate / serialize rate");
        //EditorGUILayout.LabelField("quick resends");
        //EditorGUILayout.LabelField("max resends");
        //EditorGUILayout.LabelField("enable crc checking");


        if (PhotonEditor.CheckPunPlus())
        {
            settings.Protocol = ConnectionProtocol.Udp;
            EditorGUILayout.HelpBox("You seem to use PUN+.\nPUN+ only supports reliable UDP so the protocol is locked.", MessageType.Info);
        }

        settings.AppID = settings.AppID.Trim();


        // RPC-shortcut list
        EditorGUI.indentLevel = 0;
        SerializedObject sObj = new SerializedObject(target);
        SerializedProperty sRpcs = sObj.FindProperty("RpcList");
        EditorGUILayout.PropertyField(sRpcs, true);
        sObj.ApplyModifiedProperties();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Refresh RPCs"))
        {
            PhotonEditor.UpdateRpcList();
            Repaint();
        }
        if (GUILayout.Button("Clear RPCs"))
        {
            PhotonEditor.ClearRpcList();
        }
        if (GUILayout.Button("Log HashCode"))
        {
            Debug.Log("RPC-List HashCode: " + RpcListHashCode() + ". Make sure clients that send each other RPCs have the same RPC-List.");
        }
        GUILayout.Space(20);
        GUILayout.EndHorizontal();

#if PHOTON_VOICE
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Photon Voice Settings");
        switch (settings.HostType)
        {
            case ServerSettings.HostingOption.BestRegion:
            case ServerSettings.HostingOption.PhotonCloud:
                // voice appid
                string valVoiceAppId = EditorGUILayout.TextField("Voice AppId", settings.VoiceAppID);
                if (valVoiceAppId != settings.VoiceAppID)
                {
                    settings.VoiceAppID = valVoiceAppId;
                    this.showVoiceAppIdHint = !IsAppId(settings.VoiceAppID);
                }
                if (this.showVoiceAppIdHint)
                {
                    EditorGUILayout.HelpBox("The Photon Voice needs an AppId (GUID) set.\nYou can find it online in your Dashboard.", MessageType.Warning);
                }
                break;
            case ServerSettings.HostingOption.SelfHosted:
                if (settings.VoiceServerPort == 0)
                {
                    settings.VoiceServerPort = 5055;
                }
                settings.VoiceServerPort = EditorGUILayout.IntField("Voice Server Port", settings.VoiceServerPort);
                break;
            case ServerSettings.HostingOption.OfflineMode:
            case ServerSettings.HostingOption.NotSet:
                break;
        }
        
#endif

        //SerializedProperty sp = serializedObject.FindProperty("RpcList");
        //EditorGUILayout.PropertyField(sp, true);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);     // even in Unity 5.3+ it's OK to SetDirty() for non-scene objects.
        }
    }

    private int RpcListHashCode()
    {
        // this is a hashcode generated to (more) easily compare this Editor's RPC List with some other
        int hashCode = PhotonNetwork.PhotonServerSettings.RpcList.Count + 1;
        foreach (string s in PhotonNetwork.PhotonServerSettings.RpcList)
        {
            int h1 = s.GetHashCode();
            hashCode = ((h1 << 5) + h1) ^ hashCode;
        }

        return hashCode;
    }

    /// <summary>Checks if a string is a Guid by attempting to create one.</summary>
    /// <param name="val">The potential guid to check.</param>
    /// <returns>True if new Guid(val) did not fail.</returns>
    public static bool IsAppId(string val)
    {
        try
        {
            new Guid(val);
        }
        catch
        {
            return false;
        }
        return true;
    }
}