// ----------------------------------------------------------------------------
// <copyright file="ServerSettingsInspector.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the ServerSettings scriptable object.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

using Photon.Pun;

using ExitGames.Client.Photon;

[CustomEditor(typeof (ServerSettings))]
public class ServerSettingsInspector : Editor
{
    private string versionPhoton;

    private bool showSettings;

    public void Awake()
    {
        this.versionPhoton = System.Reflection.Assembly.GetAssembly(typeof(PhotonPeer)).GetName().Version.ToString();
    }


    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Version");
        EditorGUILayout.LabelField("Pun: " + PhotonNetwork.PunVersion + " Photon lib: " + this.versionPhoton);
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        showSettings = EditorGUILayout.Foldout(showSettings, new GUIContent("Settings", "Core Photon Server/Cloud settings."));
        SerializedProperty settingsSp = serializedObject.FindProperty("AppSettings");
        if (showSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("AppIdRealtime"));
            if (PhotonEditorUtils.HasChat)
            {
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("AppIdChat"));
            }
            if (PhotonEditorUtils.HasVoice)
            {
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("AppIdVoice"));
            }
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("AppVersion"));
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("UseNameServer"));
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("FixedRegion"));
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("Server"));
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("Port"));
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("Protocol"));
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("EnableLobbyStatistics"));
            EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("NetworkLogging"));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("StartInOfflineMode"), new GUIContent("Start In Offline Mode", "Simulates an online connection.\nPUN can be used as usual."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PunLogging"), new GUIContent("PUN Logging", "Log output by PUN."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("EnableSupportLogger"), new GUIContent("Enable Support Logger", "Logs additional info for debugging."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RunInBackground"), new GUIContent("Run In Background", "Enables apps to keep the connection without focus. Android and iOS ignore this."));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Rpc Tools");
        if (GUILayout.Button("Refresh RPCs",EditorStyles.miniButton))
        {
            PhotonEditor.UpdateRpcList();
            Repaint();
        }
        if (GUILayout.Button("Clear RPCs",EditorStyles.miniButton))
        {
            PhotonEditor.ClearRpcList();
        }
        if (GUILayout.Button("Log HashCode",EditorStyles.miniButton))
        {
            Debug.Log("RPC-List HashCode: " + RpcListHashCode() + ". Make sure clients that send each other RPCs have the same RPC-List.");
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Best Region Preference");
        if (GUILayout.Button("Reset", EditorStyles.miniButton))
        {
            ServerSettings.ResetBestRegionCodeInPreferences();
        }
        EditorGUILayout.EndHorizontal();
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
}