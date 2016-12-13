using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
	/// <summary>
	/// Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Capability Settings.
	/// </summary>
	public class CapabilitySettingsWindow : AutoConfigureWindow
	{
		// Member Variables
		private Dictionary<PlayerSettings.WSACapability, bool> currentCaps = new Dictionary<PlayerSettings.WSACapability, bool>();

		// Private Methods
		private void CapabilityToggle(PlayerSettings.WSACapability mCap, string status)
		{
			// Draw and update cached capability flag
			currentCaps[mCap] = GUILayout.Toggle(currentCaps[mCap], new GUIContent(" " + mCap.ToString()));

			// If this control is the one under the mouse, update the status message
			if ((Event.current.type == EventType.Repaint) && (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)))
			{
				StatusMessage = status;
				Repaint();
			}
		}

		private void LoadSetting(PlayerSettings.WSACapability cap)
		{
			currentCaps[cap] = PlayerSettings.WSA.GetCapability(cap);
		}

		private void LoadSettings()
		{
			LoadSetting(PlayerSettings.WSACapability.Microphone);
			LoadSetting(PlayerSettings.WSACapability.SpatialPerception);
			LoadSetting(PlayerSettings.WSACapability.WebCam);
			LoadSetting(PlayerSettings.WSACapability.InternetClient);
		}

		private void SaveSetting(PlayerSettings.WSACapability cap)
		{
			PlayerSettings.WSA.SetCapability(cap, currentCaps[cap]);
		}

		private void SaveSettings()
		{
			SaveSetting(PlayerSettings.WSACapability.Microphone);
			SaveSetting(PlayerSettings.WSACapability.SpatialPerception);
			SaveSetting(PlayerSettings.WSACapability.WebCam);
			SaveSetting(PlayerSettings.WSACapability.InternetClient);
		}

		// Overrides
		protected override void OnApply()
		{
			// Apply custom first
			SaveSettings();

			// Pass to base
			base.OnApply();

			// Notify
			// EditorUtility.DisplayDialog("Capabilities", "Capabilities applied.", "OK");

			// Close
			Close();
		}

		protected override void OnEnable()
		{
			// Pass to base first
			base.OnEnable();

			// Set size
			this.minSize = new Vector2(350, 310);
			this.maxSize = this.minSize;

			// Load current values
			LoadSettings();
		}

		protected override void OnGUI()
		{
			// Start Capabilities
			GUILayout.BeginVertical(EditorStyles.helpBox);

			CapabilityToggle(PlayerSettings.WSACapability.Microphone, @"Microphone

Required for access to the HoloLens microphone.
This includes behaviors like DictationRecognizer,
GrammarRecognizer, and KeywordRecognizer.
This capability is NOT required for the 'Select' keyword.

Recommendation: Only enable if your application 
needs access to the microphone beyond the
'Select' keyword. The microphone is considered a 
privacy sensitive resource.");

			CapabilityToggle(PlayerSettings.WSACapability.SpatialPerception, @"SpatialPerception

Required for access to the HoloLens world mapping
capabilities. These include behaviors like
SurfaceObserver, SpatialMappingManager and 
SpatialAnchor. 

Recommendation: Enabled, unless your application
doesn't use spatial mapping or spatial collisions
in any way. ");

			CapabilityToggle(PlayerSettings.WSACapability.WebCam, @"WebCam

Required for access to the HoloLens RGB camera 
(also known as the locatable camera). This 
includes APIs like PhotoCapture and VideoCapture.
This capability is NOT required for mixed reality 
streaming or for capturing photos or videos using
the start menu. 

Recommendation: Only enable if your application 
needs to programmatically capture photos or 
videos from the RGB camera. The RGB camera is
considered a privacy sensitive resource.");

			CapabilityToggle(PlayerSettings.WSACapability.InternetClient, @"Internet Client

Required if your application needs to access 
the Internet. 

Recommendation: Leave unchecked unless your 
application uses online services.");

			// End Capabilities
			GUILayout.EndVertical();

			// Pass to base to render base controls
			base.OnGUI();
		}
	}
}