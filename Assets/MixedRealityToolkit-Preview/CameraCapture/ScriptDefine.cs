#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraCapture
{
	class ScriptDefines
	{
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			CheckAndAdd("UnityEngine.XR.ARFoundation.ARSubsystemManager", "USE_ARFOUNDATION");
		}

		private static void CheckAndAdd(string type, string define)
		{
			if (HasType(type) && EnsureDefine(define))
			{
				Debug.LogWarningFormat("Added #define '{0}' to your project! Please remember to add it to your version control. You will need to remove the #define manually if you remove the library this code depends on (Project Settings->Player->Scripting Define Symbols).", define);
			}
		}

		private static bool HasType(string typeName)
		{
			// Look through all loaded assemblies to find this type!
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				if (Type.GetType(typeName+", "+assemblies[i].FullName, false) != null)
					return true;
			}
			return false;
		}

		private static bool EnsureDefine(string defineName)
		{
			// Get all the scripting defines as a nice friendly list
			string   defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			string[] words   = string.IsNullOrEmpty(defines) ? new string[]{ } : defines.Split(';');

			// Find where our define might be
			int index = Array.IndexOf(words, defineName);

			// If it's not present, append it to the list, and add it in!
			if (index == -1)
			{
				Array.Resize(ref words, words.Length+1);
				words[words.Length-1] = defineName;

				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", words));

				return true;
			}
			return false;
		}
	}
}
#endif