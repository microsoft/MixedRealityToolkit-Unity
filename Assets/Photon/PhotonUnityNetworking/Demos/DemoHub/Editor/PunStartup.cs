using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class PunStartup : MonoBehaviour
{

    static PunStartup()
    {
        bool doneBefore = EditorPrefs.GetBool("PunDemosOpenedBefore");
        if (!doneBefore)
        {
            EditorApplication.update += OnUpdate;
        }
    }

    static void OnUpdate()
    {
		if (EditorApplication.isUpdating || Application.isPlaying)
        {
            return;
        }

        bool doneBefore = EditorPrefs.GetBool("PunDemosOpenedBefore");
        EditorPrefs.SetBool("PunDemosOpenedBefore", true);
        EditorApplication.update -= OnUpdate;

        if (doneBefore)
        {
            return;
        }

        if (string.IsNullOrEmpty(SceneManagerHelper.EditorActiveSceneName) && EditorBuildSettings.scenes.Length == 0)
        {
            LoadPunDemoHub();
            SetPunDemoBuildSettings();
            Debug.Log("No scene was open. Loaded PUN Demo Hub Scene and added demos to build settings. Ready to go! This auto-setup is now disabled in this Editor.");
        }
    }

    [MenuItem("Window/Photon Unity Networking/Configure Demos (build setup)", false, 5)]
    public static void SetupDemo()
    {
        SetPunDemoBuildSettings();
    }

    //[MenuItem("Window/Photon Unity Networking/PUN Demo Loader Reset")]
    //protected static void ResetDemoLoader()
    //{
    //    EditorPrefs.DeleteKey("PunDemosOpenedBefore");
    //}

    public static void LoadPunDemoHub()
    {
		string scenePath = FindAssetPath("DemoHub-Scene t:scene");
        if (!string.IsNullOrEmpty(scenePath))
        {
				EditorSceneManager.OpenScene (scenePath);
				Selection.activeObject = AssetDatabase.LoadMainAssetAtPath (scenePath);
        }
    }

    /// <summary>Finds the asset path base on its name or search query: https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html </summary>
    /// <returns>The asset path. String.Empty, if not found.</returns>
    /// <param name="asset">Asset filter for AssetDatabase.FindAssets.</param>
    public static string FindAssetPath(string asset)
	{
		string[] guids = AssetDatabase.FindAssets(asset, null);
		if (guids.Length < 1)
		{
		    Debug.LogError("We have a problem finding the asset: " + asset);
			return string.Empty;
		} else
		{
			return AssetDatabase.GUIDToAssetPath(guids[0]);
		}
	}

    /// <summary>
    /// Finds scenes in "Assets/Photon Unity Networking/Demos/", excludes those in folder "PUNGuide_M2H" and applies remaining scenes to build settings. The one with "Hub" in it first.
    /// </summary>
    public static void SetPunDemoBuildSettings()
    {
		string _PunPath = string.Empty;

		string _thisPath = PhotonNetwork.FindAssetPath ("PunStartUp");

		_thisPath = Application.dataPath + _thisPath.Substring (6); // remove "Assets/"

		_PunPath = PhotonEditorUtils.GetParent(_thisPath,"Photon");

		if (_PunPath == null)
		{
			_PunPath = Application.dataPath+"Photon";
		}

		// find path of pun guide

		string[] tempPaths = Directory.GetDirectories(_PunPath, "Demos*", SearchOption.AllDirectories);
		if (tempPaths == null)
		{
			return;
		}

		List<EditorBuildSettingsScene> sceneAr = new List<EditorBuildSettingsScene> ();

        // find scenes of guide
		foreach (string guidePath in tempPaths)
		{
			tempPaths = Directory.GetFiles (guidePath, "*.unity", SearchOption.AllDirectories);

			if (tempPaths == null || tempPaths.Length == 0)
			{
				return;
			}

			// add found guide scenes to build settings
			for (int i = 0; i < tempPaths.Length; i++)
			{
				//Debug.Log(tempPaths[i]);
				string path = tempPaths [i].Substring (Application.dataPath.Length - "Assets".Length);
				path = path.Replace ('\\', '/');
				//Debug.Log(path);

				if (path.Contains ("PUNGuide_M2H"))
				{
					continue;
				}

				// edited to avoid old scene to be included.
				if (path.Contains ("DemoHub-Scene"))
				{
					sceneAr.Insert (0, new EditorBuildSettingsScene (path, true));
					continue;
				}

				sceneAr.Add (new EditorBuildSettingsScene (path, true));
			}
		}

        EditorBuildSettings.scenes = sceneAr.ToArray();
        EditorSceneManager.OpenScene(sceneAr[0].path);
    }
}