using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
	/// <summary>
	/// Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Project Settings.
	/// </summary>
	public class ProjectSettingsWindow : AutoConfigureWindow
	{
		// Nested Types
		private enum ProjectSetting
		{
			ActiveBuildToWsa,
			WsaSdkToUwp,
			WsaUwpBuildToD3D,
			WsaFastestQuality,
			WsaEnableVR
		}

		// Member Variables
		private Dictionary<ProjectSetting, bool> settings = new Dictionary<ProjectSetting, bool>();
		private Dictionary<ProjectSetting, string> names = new Dictionary<ProjectSetting, string>();
		private Dictionary<ProjectSetting, string> descriptions = new Dictionary<ProjectSetting, string>();

		// Private Methods
		private void ApplySettings()
		{
			// See the blow notes for why text asset serialization is required
			if (EditorSettings.serializationMode != SerializationMode.ForceText)
			{
				// NOTE: PlayerSettings.virtualRealitySupported would be ideal, except that it only reports/affects whatever platform tab
				// is currently selected in the Player settings window. As we don't have code control over what view is selected there
				// this property is fairly useless from script.

				// NOTE: There is no current way to change the default quality setting from script

				string title = "Updates require text serialization of assets";
				string message = "Unity doesn't provide apis for updating the default quality or enabling VR support.\n\n" +
					"Is it ok if we force text serialization of assets so that we can modify the properties directly?";

				bool forceText = EditorUtility.DisplayDialog(title, message, "Yes", "No");
				if (!forceText)
					return;

				EditorSettings.serializationMode = SerializationMode.ForceText;
			}

			// Apply individual settings
			if (settings[ProjectSetting.ActiveBuildToWsa])
			{
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WSAPlayer);
			}
			if (settings[ProjectSetting.WsaSdkToUwp])
			{
				EditorUserBuildSettings.wsaSDK = WSASDK.UWP;
			}
			if (settings[ProjectSetting.WsaUwpBuildToD3D])
			{
				EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
			}
			if (settings[ProjectSetting.WsaFastestQuality])
			{
				SetFastestDefaultQuality();
			}
			if (settings[ProjectSetting.WsaEnableVR])
			{
				EnableVirtualReality();
			}

			// Since we went behind Unity's back to tweak some settings we 
			// need to reload the project to have them take effect
			bool canReload = EditorUtility.DisplayDialog(
				"Project reload required!",
				"Some changes require a project reload to take effect.\n\nReload now?",
				"Yes", "No");

			if (canReload)
			{
				string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
				EditorApplication.OpenProject(projectPath);
			}
		}

		/// <summary>
		/// Enables virtual reality for WSA and ensures HoloLens is in the supported SDKs.
		/// </summary>
		private void EnableVirtualReality()
		{
			try
			{
				// Grab the text from the project settings asset file
				string settingsPath = "ProjectSettings/ProjectSettings.asset";
				string settings = File.ReadAllText(settingsPath);

				// We're looking for the list of VR devices for the current build target, then
				// ensuring that the HoloLens is in that list
				bool foundBuildTargetVRSettings = false;
				bool foundBuildTargetMetro = false;
				bool foundBuildTargetEnabled = false;
				bool foundDevices = false;
				bool foundHoloLens = false;

				var builder = new StringBuilder(); // Used to build the final output
				string[] lines = settings.Split(new char[] { '\n' });
				for (int i = 0; i < lines.Length; ++i)
				{
					string line = lines[i];

					// Look for the build target VR settings
					if (!foundBuildTargetVRSettings)
					{
						if (line.Contains("m_BuildTargetVRSettings:"))
						{
							// If no targets are enabled at all, just create the known entries and skip the rest of the tests
							if (line.Contains("[]"))
							{
								// Remove the empty array symbols
								line = line.Replace(" []", "\n");

								// Generate the new lines
								line += "  - m_BuildTarget: Metro\n";
								line += "    m_Enabled: 1\n";
								line += "    m_Devices:\n";
								line += "    - HoloLens";

								// Mark all fields as found so we don't search anymore
								foundBuildTargetVRSettings = true;
								foundBuildTargetMetro = true;
								foundBuildTargetEnabled = true;
								foundDevices = true;
								foundHoloLens = true;
							}
							else
							{
								// The target VR settngs were found but the others
								// still need to be searched for.
								foundBuildTargetVRSettings = true;
							}
						}

					}

					// Look for the build target for Metro
					else if (!foundBuildTargetMetro)
					{
						if (line.Contains("m_BuildTarget: Metro"))
						{
							foundBuildTargetMetro = true;
						}

					}

					else if (!foundBuildTargetEnabled)
					{
						if (line.Contains("m_Enabled"))
						{
							line = "    m_Enabled: 1";
							foundBuildTargetEnabled = true;
						}
					}

					// Look for the enabled Devices list
					else if (!foundDevices)
					{
						if (line.Contains("m_Devices:"))
						{
							// Clear the empty array symbols if any
							line = line.Replace(" []", "");
							foundDevices = true;
						}

					}

					// Once we've found the list look for HoloLens or the next non element
					else if (!foundHoloLens)
					{
						// If this isn't an element in the device list
						if (!line.Contains("-"))
						{
							// add the hololens element, and mark it found
							builder.Append("    - HoloLens\n");
							foundHoloLens = true;
						}

						// Otherwise test if this is the hololens device
						else if (line.Contains("HoloLens"))
						{
							foundHoloLens = true;
						}
					}

					builder.Append(line);

					// Write out a \n for all but the last line
					// NOTE: Specifically preserving unix line endings by avoiding StringBuilder.AppendLine
					if (i != lines.Length - 1)
						builder.Append('\n');
				}

				// Capture the final string
				settings = builder.ToString();

				File.WriteAllText(settingsPath, settings);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}


		private void LoadDefaults()
		{
			for (int i=(int)ProjectSetting.ActiveBuildToWsa; i <= (int)ProjectSetting.WsaEnableVR; i++)
			{
				settings[(ProjectSetting)i] = true;
			}
		}

		private void LoadStrings()
		{
			names[ProjectSetting.ActiveBuildToWsa] = "Target Windows Store";
			descriptions[ProjectSetting.ActiveBuildToWsa] = "Required\n\nSwitches the currently active target to produce a Windows Store app.\n\nSince HoloLens only supports Windows Store apps, this option should remain checked unless you plan to manually switch the target later before you build.";

			names[ProjectSetting.WsaSdkToUwp] = "Target UWP";
			descriptions[ProjectSetting.WsaSdkToUwp] = "Required\n\nSpecifies that the Windows Store app will target the Universal Windows Platform.\n\nSince HoloLens only supports UWP, this option should remain checked unless you plan to manually switch the target later before you build.";

			names[ProjectSetting.WsaUwpBuildToD3D] = "Build for Direct3D";
			descriptions[ProjectSetting.WsaUwpBuildToD3D] = "Recommended\n\nProduces an app that targets Direct3D instead of Xaml.\n\nPure Direct3D apps run faster than applications that include Xaml. This option should remain checked unless you plan to overlay Unity content with Xaml content or you plan to switch between Unity views and Xaml views at runtime.";

			names[ProjectSetting.WsaFastestQuality] = "Set Quality to Fastest";
			descriptions[ProjectSetting.WsaFastestQuality] = "Recommended\n\nChanges the quality settings for Windows Store apps to the 'Fastest' setting.\n\n'Fastest' is the recommended quality setting for HoloLens apps, but this option can be unchecked if you have already optimized your project for the HoloLens.";

			names[ProjectSetting.WsaEnableVR] = "Enable VR";
			descriptions[ProjectSetting.WsaEnableVR] = "Required\n\nEnables VR for Windows Store apps and adds the HoloLens as a target VR device.\n\nThe application will not compile for HoloLens and tools like Holographic Remoting will not function without this enabled. Therefore this option should remain checked unless you plan to manually perform these steps later.";
		}

		/// <summary>
		/// Modifies the WSA default quality setting to the fastest
		/// </summary>
		private void SetFastestDefaultQuality()
		{
			try
			{
				// Find the WSA element under the platform quality list and replace it's value with 0
				string settingsPath = "ProjectSettings/QualitySettings.asset";
				string matchPattern = @"(m_PerPlatformDefaultQuality.*Windows Store Apps:) (\d+)";
				string replacePattern = @"$1 0";

				string settings = File.ReadAllText(settingsPath);
				settings = Regex.Replace(settings, matchPattern, replacePattern, RegexOptions.Singleline);

				File.WriteAllText(settingsPath, settings);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private void SettingToggle(ProjectSetting setting)
		{
			// Draw and update setting flag
			settings[setting] = GUILayout.Toggle(settings[setting], new GUIContent(names[setting]));

			// If this control is the one under the mouse, update the status message
			if ((Event.current.type == EventType.Repaint) && (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)))
			{
				StatusMessage = descriptions[setting];
				Repaint();
			}
		}

		// Overrides
		protected override void OnApply()
		{
			// Apply custom first
			ApplySettings();

			// Pass to base
			base.OnApply();

			// Close
			Close();
		}

		protected override void OnEnable()
		{
			// Pass to base first
			base.OnEnable();

			// Set size
			this.minSize = new Vector2(350, 260);
			this.maxSize = this.minSize;

			// Load defaults
			LoadDefaults();
			LoadStrings();
		}

		protected override void OnGUI()
		{
			// Start Settings Section
			GUILayout.BeginVertical(EditorStyles.helpBox);

			// Individual Settings
			SettingToggle(ProjectSetting.ActiveBuildToWsa);
			SettingToggle(ProjectSetting.WsaSdkToUwp);
			SettingToggle(ProjectSetting.WsaUwpBuildToD3D);
			SettingToggle(ProjectSetting.WsaFastestQuality);
			SettingToggle(ProjectSetting.WsaEnableVR);

			// End Settings Section
			GUILayout.EndVertical();

			// Pass to base to render base controls
			base.OnGUI();
		}
	}
}