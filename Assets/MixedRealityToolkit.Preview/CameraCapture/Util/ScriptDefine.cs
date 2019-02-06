#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.CameraCapture
{
	class ScriptDefines
	{
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			CheckAndAdd("UnityEngine.XR.ARFoundation.ARSubsystemManager", "USE_ARFOUNDATION");
		}

		/// <summary>Checks if a class is present in any loaded assemblies, and adds a script define to Unity's list if it's not already there.</summary>
		/// <param name="type">The Type name to check for including namespace, but not the fully qualified assembly name.</param>
		/// <param name="define">The scripting define to add.</param>
		private static void CheckAndAdd(string type, string define)
		{
			if (HasType(type) && EnsureDefine(define))
			{
				Debug.LogWarningFormat("Added #define '{0}' to your project! Please remember to add it to your version control. You will need to remove the #define manually if you remove the library this code depends on (Project Settings->Player->Scripting Define Symbols).", define);
			}
		}

		/// <summary> Checks if any of the loaded assemblies have the indicated Type. </summary>
		/// <param name="typeName">The Type name to check for including namespace, but not the fully qualified assembly name.</param>
		/// <returns>Is it present?</returns>
		private static bool HasType(string typeName)
		{
			// Look through all loaded assemblies to find this type!
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				if (Type.GetType(typeName+", "+assemblies[i].FullName, false) != null)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary> Checks Unity's scripting defines, and ensures ours is present in it. This will trigger a recompile if it's added. </summary>
		/// <returns>Was it added?</returns>
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