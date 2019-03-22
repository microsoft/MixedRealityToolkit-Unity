// ----------------------------------------------------------------------------
// <copyright file="PhotonEditorUtils.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Unity Editor Utils
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using System.IO;


namespace Photon.Pun
{
    [InitializeOnLoad]
    public class PhotonEditorUtils
    {
        /// <summary>True if the ChatClient of the Photon Chat API is available. If so, the editor may (e.g.) show additional options in settings.</summary>
        public static bool HasChat;

        /// <summary>True if the VoiceClient of the Photon Voice API is available. If so, the editor may (e.g.) show additional options in settings.</summary>
        public static bool HasVoice;

        public static bool HasPun;

        /// <summary>True if the PhotonEditorUtils checked the available products / APIs. If so, the editor may (e.g.) show additional options in settings.</summary>
        public static bool HasCheckedProducts;

        static PhotonEditorUtils()
        {
            HasVoice = Type.GetType("Photon.Voice.VoiceClient, Assembly-CSharp") != null || Type.GetType("Photon.Voice.VoiceClient, Assembly-CSharp-firstpass") != null || Type.GetType("Photon.Voice.VoiceClient, PhotonVoice.API") != null;
            HasChat = Type.GetType("Photon.Chat.ChatClient, Assembly-CSharp") != null || Type.GetType("Photon.Chat.ChatClient, Assembly-CSharp-firstpass") != null || Type.GetType("Photon.Chat.ChatClient, PhotonChat") != null;
            HasPun = Type.GetType("Photon.Pun.PhotonNetwork, Assembly-CSharp") != null || Type.GetType("Photon.Pun.PhotonNetwork, Assembly-CSharp-firstpass") != null || Type.GetType("Photon.Pun.PhotonNetwork, PhotonUnityNetworking") != null;
            PhotonEditorUtils.HasCheckedProducts = true;

            if (HasPun)
            {
                // MOUNTING SYMBOLS
                #if !PHOTON_UNITY_NETWORKING
                AddScriptingDefineSymbolToAllBuildTargetGroups("PHOTON_UNITY_NETWORKING");
                #endif

                #if !PUN_2_0_OR_NEWER
                AddScriptingDefineSymbolToAllBuildTargetGroups("PUN_2_0_OR_NEWER");
                #endif

                #if !PUN_2_OR_NEWER
                AddScriptingDefineSymbolToAllBuildTargetGroups("PUN_2_OR_NEWER");
                #endif
            }
        }

        /// <summary>
        /// Adds a given scripting define symbol to all build target groups
        /// You can see all scripting define symbols ( not the internal ones, only the one for this project), in the PlayerSettings inspector
        /// </summary>
        /// <param name="defineSymbol">Define symbol.</param>
        public static void AddScriptingDefineSymbolToAllBuildTargetGroups(string defineSymbol)
        {
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToList();

                if (!defineSymbols.Contains(defineSymbol))
                {
                    defineSymbols.Add(defineSymbol);

                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Could not set Photon " + defineSymbol + " defines for build target: " + target + " group: " + group + " " + e);
                    }
                }
            }
        }


        /// <summary>
        /// Removes PUN2's Script Define Symbols from project
        /// </summary>
        public static void CleanUpPunDefineSymbols()
        {
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                    .Split(';')
                    .Select(d => d.Trim())
                    .ToList();

                List<string> newDefineSymbols = new List<string>();
                foreach (var symbol in defineSymbols)
                {
                    if ("PHOTON_UNITY_NETWORKING".Equals(symbol) || symbol.StartsWith("PUN_2_"))
                    {
                        continue;
                    }

                    newDefineSymbols.Add(symbol);
                }

                try
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", newDefineSymbols.ToArray()));
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Could not set clean up PUN2's define symbols for build target: {0} group: {1}, {2}", target, group, e);
                }
            }
        }


        /// <summary>
        /// Gets the parent directory of a path. Recursive Function, will return null if parentName not found
        /// </summary>
        /// <returns>The parent directory</returns>
        /// <param name="path">Path.</param>
        /// <param name="parentName">Parent name.</param>
        public static string GetParent(string path, string parentName)
        {
            var dir = new DirectoryInfo(path);

            if (dir.Parent == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(parentName))
            {
                return dir.Parent.FullName;
            }

            if (dir.Parent.Name == parentName)
            {
                return dir.Parent.FullName;
            }

            return GetParent(dir.Parent.FullName, parentName);
        }

		/// <summary>
		/// Check if a GameObject is a prefab asset or part of a prefab asset, as opposed to an instance in the scene hierarchy
		/// </summary>
		/// <returns><c>true</c>, if a prefab asset or part of it, <c>false</c> otherwise.</returns>
		/// <param name="go">The GameObject to check</param>
		public static bool IsPrefab(GameObject go)
		{
			#if UNITY_2018_3_OR_NEWER
				return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null;
			#else
				return EditorUtility.IsPersistent(go);
			#endif
		}
    }


    public class CleanUpDefinesOnPunDelete : UnityEditor.AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions rao)
        {
            if ("Assets/Photon/PhotonUnityNetworking".Equals(assetPath))
            {
                PhotonEditorUtils.CleanUpPunDefineSymbols();
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
