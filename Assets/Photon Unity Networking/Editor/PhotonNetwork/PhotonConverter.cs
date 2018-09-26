// ----------------------------------------------------------------------------
// <copyright file="PhotonConverter.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Script to convert a Unity Networking project to PhotonNetwork.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
#define UNITY_MIN_5_3
#endif


using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class PhotonConverter : Photon.MonoBehaviour
{
    public static void RunConversion()
    {
        //Ask if user has made a backup.
        int option = EditorUtility.DisplayDialogComplex("Conversion", "Attempt automatic conversion from Unity Networking to Photon Unity Networking \"PUN\"?", "Yes", "No!", "Pick Script Folder");
        switch (option)
        {
            case 0:
                break;
            case 1:
                return;
            case 2:
                PickFolderAndConvertScripts();
                return;
            default:
                return;
        }

        //REAAAALY?
        bool result = EditorUtility.DisplayDialog("Conversion", "Disclaimer: The code conversion feature is quite crude, but should do it's job well (see the sourcecode). A backup is therefore strongly recommended!", "Yes, I've made a backup: GO", "Abort");
        if (!result)
        {
            return;
        }
        Output(EditorApplication.timeSinceStartup + " Started conversion of Unity networking -> Photon");

        //Ask to save current scene (optional)
        //EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorUtility.DisplayProgressBar("Converting..", "Starting.", 0);

        //Convert NetworkViews to PhotonViews in Project prefabs
        //Ask the user if we can move all prefabs to a resources folder
        bool movePrefabs = EditorUtility.DisplayDialog("Conversion", "Can all prefabs that use a PhotonView be moved to a Resources/ folder? You need this if you use Network.Instantiate.", "Yes", "No");


        string[] prefabs = Directory.GetFiles("Assets/", "*.prefab", SearchOption.AllDirectories);
        foreach (string prefab in prefabs)
        {
            EditorUtility.DisplayProgressBar("Converting..", "Object:" + prefab, 0.6f);

            Object[] objs = (Object[])AssetDatabase.LoadAllAssetsAtPath(prefab);
            int converted = 0;
            foreach (Object obj in objs)
            {
                if (obj != null && obj.GetType() == typeof(GameObject))
                    converted += ConvertNetworkView(((GameObject)obj).GetComponents<NetworkView>(), false);
            }
            if (movePrefabs && converted > 0)
            {
                //This prefab needs to be under the root of a Resources folder!
                string path = prefab.Replace("\\", "/");
                int lastSlash = path.LastIndexOf("/");
                int resourcesIndex = path.LastIndexOf("/Resources/");
                if (resourcesIndex != lastSlash - 10)
                {
                    if (path.Contains("/Resources/"))
                    {
                        Debug.LogWarning("Warning, prefab [" + prefab + "] was already in a resources folder. But has been placed in the root of another one!");
                    }
                    //This prefab NEEDS to be placed under a resources folder
                    string resourcesFolder = path.Substring(0, lastSlash) + "/Resources/";
                    EnsureFolder(resourcesFolder);
                    string newPath = resourcesFolder + path.Substring(lastSlash + 1);
                    string error = AssetDatabase.MoveAsset(prefab, newPath);
                    if (error != "")
                        Debug.LogError(error);
                    Output("Fixed prefab [" + prefab + "] by moving it into a resources folder.");
                }
            }
        }

        //Convert NetworkViews to PhotonViews in scenes
        string[] sceneFiles = Directory.GetFiles("Assets/", "*.unity", SearchOption.AllDirectories);
        foreach (string sceneName in sceneFiles)
        {
            //EditorSceneManager.OpenScene(sceneName);
            EditorUtility.DisplayProgressBar("Converting..", "Scene:" + sceneName, 0.2f);

            int converted2 = ConvertNetworkView((NetworkView[])GameObject.FindObjectsOfType(typeof(NetworkView)), true);
            if (converted2 > 0)
            {
                //This will correct all prefabs: The prefabs have gotten new components, but the correct ID's were lost in this case
                PhotonViewHandler.HierarchyChange();    //TODO: most likely this is triggered on change or on save

                Output("Replaced " + converted2 + " NetworkViews with PhotonViews in scene: " + sceneName);
                //EditorSceneManager.SaveOpenScenes();
            }
        }

        //Convert C#/JS scripts (API stuff)
        List<string> scripts = GetScriptsInFolder("Assets");

        EditorUtility.DisplayProgressBar("Converting..", "Scripts..", 0.9f);
        ConvertScripts(scripts);

        Output(EditorApplication.timeSinceStartup + " Completed conversion!");
        EditorUtility.ClearProgressBar();

        EditorUtility.DisplayDialog("Completed the conversion", "Don't forget to add \"PhotonNetwork.ConnectWithDefaultSettings();\" to connect to the Photon server before using any multiplayer functionality.", "OK");
    }

    public static void PickFolderAndConvertScripts()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Pick source folder to convert", Directory.GetCurrentDirectory(), "");
        if (string.IsNullOrEmpty(folderPath))
        {
            EditorUtility.DisplayDialog("Script Conversion", "No folder was selected. No files were changed. Please start over.", "Ok.");
            return;
        }

        bool result = EditorUtility.DisplayDialog("Script Conversion", "Scripts in this folder will be modified:\n\n" + folderPath + "\n\nMake sure you have backups of these scripts.\nConversion is not guaranteed to work!", "Backup done. Go!", "Abort");
        if (!result)
        {
            return;
        }

        List<string> scripts = GetScriptsInFolder(folderPath);
        ConvertScripts(scripts);

        EditorUtility.DisplayDialog("Script Conversion", "Scripts are now converted to PUN.\n\nYou will need to update\n- scenes\n- components\n- prefabs and\n- add \"PhotonNetwork.ConnectWithDefaultSettings();\"", "Ok");
    }


    public static List<string> GetScriptsInFolder(string folder)
    {
        List<string> scripts = new List<string>();

        try
        {
            scripts.AddRange(Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories));
            scripts.AddRange(Directory.GetFiles(folder, "*.js", SearchOption.AllDirectories));
            scripts.AddRange(Directory.GetFiles(folder, "*.boo", SearchOption.AllDirectories));
        }
        catch (System.Exception ex)
        {
            Debug.Log("Getting script list from folder " + folder + " failed. Exception:\n" + ex.ToString());
        }

        return scripts;
    }

    static void ConvertScripts(List<string> scriptPathList)
    {
        bool ignoreWarningIsLogged = false;

        foreach (string script in scriptPathList)
        {
            if (script.Contains("PhotonNetwork")) //Don't convert this file (and others)
            {
                if (!ignoreWarningIsLogged)
                {
                    ignoreWarningIsLogged = true;
                    Debug.LogWarning("Conversion to PUN ignores all files with \"PhotonNetwork\" in their file-path.\nCheck: " + script);
                }
                continue;
            }
            if (script.Contains("Image Effects"))
            {
                continue;
            }

            ConvertToPhotonAPI(script);
        }

        foreach (string script in scriptPathList)
        {
            AssetDatabase.ImportAsset(script, ImportAssetOptions.ForceUpdate);
        }
    }

    static void ConvertToPhotonAPI(string file)
    {
        string text = File.ReadAllText(file);

        bool isJS = file.Contains(".js");

        file = file.Replace("\\", "/"); // Get Class name for JS
        string className = file.Substring(file.LastIndexOf("/")+1);
        className = className.Substring(0, className.IndexOf("."));


        //REGEXP STUFF
        //Valid are: Space { } , /n /r
        //string NOT_VAR = @"([^A-Za-z0-9_\[\]\.]+)";
        string NOT_VAR_WITH_DOT = @"([^A-Za-z0-9_]+)";

        //string VAR_NONARRAY = @"[^A-Za-z0-9_]";


        //NetworkView
        {
            text = PregReplace(text, NOT_VAR_WITH_DOT + "NetworkView" + NOT_VAR_WITH_DOT, "$1PhotonView$2");
            text = PregReplace(text, NOT_VAR_WITH_DOT + "networkView" + NOT_VAR_WITH_DOT, "$1photonView$2");
            text = PregReplace(text, NOT_VAR_WITH_DOT + "stateSynchronization" + NOT_VAR_WITH_DOT, "$1synchronization$2");
            text = PregReplace(text, NOT_VAR_WITH_DOT + "NetworkStateSynchronization" + NOT_VAR_WITH_DOT, "$1ViewSynchronization$2");   // map Unity enum to ours
            //.RPC
            text = PregReplace(text, NOT_VAR_WITH_DOT + "RPCMode.Server" + NOT_VAR_WITH_DOT, "$1PhotonTargets.MasterClient$2");
            text = PregReplace(text, NOT_VAR_WITH_DOT + "RPCMode" + NOT_VAR_WITH_DOT, "$1PhotonTargets$2");
        }

        //NetworkMessageInfo: 100%
        {
            text = PregReplace(text, NOT_VAR_WITH_DOT + "NetworkMessageInfo" + NOT_VAR_WITH_DOT, "$1PhotonMessageInfo$2");
            text = PregReplace(text, NOT_VAR_WITH_DOT + "networkView" + NOT_VAR_WITH_DOT, "$1photonView$2");
        }

        //NetworkViewID:
        {
            text = PregReplace(text, NOT_VAR_WITH_DOT + "NetworkViewID" + NOT_VAR_WITH_DOT, "$1int$2"); //We simply use an int
        }

        //NetworkPlayer
        {
            text = PregReplace(text, NOT_VAR_WITH_DOT + "NetworkPlayer" + NOT_VAR_WITH_DOT, "$1PhotonPlayer$2");
        }

        //Network
        {
            //Monobehaviour callbacks
            {
                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnPlayerConnected" + NOT_VAR_WITH_DOT, "$1OnPhotonPlayerConnected$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnPlayerDisconnected" + NOT_VAR_WITH_DOT, "$1OnPhotonPlayerDisconnected$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnNetworkInstantiate" + NOT_VAR_WITH_DOT, "$1OnPhotonInstantiate$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnSerializeNetworkView" + NOT_VAR_WITH_DOT, "$1OnPhotonSerializeView$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "BitStream" + NOT_VAR_WITH_DOT, "$1PhotonStream$2");

                //Not completely the same meaning
                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnServerInitialized" + NOT_VAR_WITH_DOT, "$1OnCreatedRoom$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnConnectedToServer" + NOT_VAR_WITH_DOT, "$1OnJoinedRoom$2");

                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnFailedToConnectToMasterServer" + NOT_VAR_WITH_DOT, "$1OnFailedToConnectToPhoton$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "OnFailedToConnect" + NOT_VAR_WITH_DOT, "$1OnFailedToConnect_OBSELETE$2");
            }

            //Variables
            {

                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.connections" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.playerList$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.isServer" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.isMasterClient$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.isClient" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.isNonMasterClientInRoom$2");

                text = PregReplace(text, NOT_VAR_WITH_DOT + "NetworkPeerType" + NOT_VAR_WITH_DOT, "$1ConnectionState$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.peerType" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.connectionState$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "ConnectionState.Server" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.isMasterClient$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "ConnectionState.Client" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.isNonMasterClientInRoom$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "PhotonNetwork.playerList.Length" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.playerList.Count$2");

                /*DROPPED:
                    minimumAllocatableViewIDs
                    natFacilitatorIP is dropped
                    natFacilitatorPort is dropped
                    connectionTesterIP
                    connectionTesterPort
                    proxyIP
                    proxyPort
                    useProxy
                    proxyPassword
                 */
            }

            //Methods
            {
                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.InitializeServer" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.CreateRoom$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.Connect" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.JoinRoom$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.GetAveragePing" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.GetPing$2");
                text = PregReplace(text, NOT_VAR_WITH_DOT + "Network.GetLastPing" + NOT_VAR_WITH_DOT, "$1PhotonNetwork.GetPing$2");
                /*DROPPED:
                    TestConnection
                    TestConnectionNAT
                    HavePublicAddress
                */
            }

            //Overall
            text = PregReplace(text, NOT_VAR_WITH_DOT + "Network" + NOT_VAR_WITH_DOT, "$1PhotonNetwork$2");


        //Changed methods
             string ignoreMe = @"([A-Za-z0-9_\[\]\(\) ]+)";

         text = PregReplace(text, NOT_VAR_WITH_DOT + "PhotonNetwork.GetPing\\(" + ignoreMe+"\\);", "$1PhotonNetwork.GetPing();");
        text = PregReplace(text, NOT_VAR_WITH_DOT + "PhotonNetwork.CloseConnection\\(" + ignoreMe+","+ignoreMe+"\\);", "$1PhotonNetwork.CloseConnection($2);");

        }

        //General
        {
            if (text.Contains("Photon")) //Only use the PhotonMonoBehaviour if we use photonView and friends.
            {
                if (isJS)//JS
                {
                    if (text.Contains("extends MonoBehaviour"))
                        text = PregReplace(text, "extends MonoBehaviour", "extends Photon.MonoBehaviour");
                    else
                        text = "class " + className + " extends Photon.MonoBehaviour {\n" + text + "\n}";
                }
                else //C#
                    text = PregReplace(text, ": MonoBehaviour", ": Photon.MonoBehaviour");
            }
        }

        File.WriteAllText(file, text);
    }


    ///  default path: "Assets"
    public static void ConvertRpcAttribute(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }

        List<string> scripts = GetScriptsInFolder(path);
        foreach (string file in scripts)
        {
            string text = File.ReadAllText(file);
            string textCopy = text;
            if (file.EndsWith("PhotonConverter.cs"))
            {
                continue;
            }

            text = text.Replace("[RPC]", "[PunRPC]");
            text = text.Replace("@RPC", "@PunRPC");

            if (!text.Equals(textCopy))
            {
                File.WriteAllText(file, text);
                Debug.Log("Converted RPC to PunRPC in: " + file);
            }
        }
    }

    static string PregReplace(string input, string[] pattern, string[] replacements)
    {
        if (replacements.Length != pattern.Length)
            Debug.LogError("Replacement and Pattern Arrays must be balanced");

        for (var i = 0; i < pattern.Length; i++)
        {
            input = Regex.Replace(input, pattern[i], replacements[i]);
        }

        return input;
    }
    static string PregReplace(string input, string pattern, string replacement)
    {
        return Regex.Replace(input, pattern, replacement);

    }

    static void EnsureFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }
    }

    static int ConvertNetworkView(NetworkView[] netViews, bool isScene)
    {
        for (int i = netViews.Length - 1; i >= 0; i--)
        {
            NetworkView netView = netViews[i];
            PhotonView view = netView.gameObject.AddComponent<PhotonView>();
            Undo.RecordObject(view, null);

            if (isScene)
            {
                //Get scene ID
                string str = netView.viewID.ToString().Replace("SceneID: ", "");
                int firstSpace = str.IndexOf(" ");
                str = str.Substring(0, firstSpace);
                int oldViewID = int.Parse(str);

                view.viewID = oldViewID;
                
                #if !UNITY_MIN_5_3
                EditorUtility.SetDirty(view);
                EditorUtility.SetDirty(view.gameObject);
                #endif
            }
            view.observed = netView.observed;
            if (netView.stateSynchronization == NetworkStateSynchronization.Unreliable)
            {
                view.synchronization = ViewSynchronization.Unreliable;
            }
            else if (netView.stateSynchronization == NetworkStateSynchronization.ReliableDeltaCompressed)
            {
                view.synchronization = ViewSynchronization.ReliableDeltaCompressed;
            }
            else
            {
                view.synchronization = ViewSynchronization.Off;
            }
            DestroyImmediate(netView, true);
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        return netViews.Length;
    }

    static void Output(string str)
    {
        Debug.Log(((int)EditorApplication.timeSinceStartup) + " " + str);
    }

    static void ConversionError(string file, string str)
    {
        Debug.LogError("Scrip conversion[" + file + "]: " + str);
    }

}
