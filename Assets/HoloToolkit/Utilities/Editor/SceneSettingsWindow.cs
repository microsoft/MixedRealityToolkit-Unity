// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
	/// Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Scene Settings.
	/// </summary>
	public class SceneSettingsWindow : AutoConfigureWindow<SceneSettingsWindow.SceneSetting>
	{
		#region Nested Types
		public enum SceneSetting
		{
			CameraToOrigin,
			CameraClearBlack,
			NearClipPlane,
			FieldOfView,
		}
		#endregion // Nested Types

		#region Overrides / Event Handlers
		protected override void ApplySettings()
		{
			// Ensure we have a camera
			if (Camera.main == null)
			{
				Debug.LogWarning(@"Could not apply settings - no camera tagged with ""MainCamera""");
				return;
			}

			// Apply individual settings
			if (Values[SceneSetting.CameraToOrigin])
			{
				Camera.main.transform.position = Vector3.zero;
			}
			if (Values[SceneSetting.CameraClearBlack])
			{
				Camera.main.clearFlags = CameraClearFlags.SolidColor;
				Camera.main.backgroundColor = Color.clear;
			}
			if (Values[SceneSetting.NearClipPlane])
			{
				Camera.main.nearClipPlane = 0.85f;
			}
			if (Values[SceneSetting.FieldOfView])
			{
				Camera.main.fieldOfView = 16.0f;
			}
		}

		protected override void LoadSettings()
		{
			for (int i = (int)SceneSetting.CameraToOrigin; i <= (int)SceneSetting.FieldOfView; i++)
			{
				Values[(SceneSetting)i] = true;
			}
		}

		protected override void LoadStrings()
		{
			Names[SceneSetting.CameraToOrigin] = "Move Camera to Origin";
			Descriptions[SceneSetting.CameraToOrigin] = "Moves the main camera to the origin of the scene (0,0,0).\n\nWhen a HoloLens application starts, the users head is the center of the world. Not having the main camera at 0,0,0 will result in holograms not appearing where they are expeted. This option should remain checked unless you have code that explicitly deals with any offset.";

			Names[SceneSetting.CameraClearBlack] = "Camera Clears to Black";
			Descriptions[SceneSetting.CameraClearBlack] = "Causes the camera to render to a black background instead of the default skybox.\n\nIn HoloLens the color black is transparent. Rendering to a black background allows the user to see the real world wherever there are no holograms. This option should remain checked unless you are building a VR-like experience or are implementing advanced rendering techniques.";

			Names[SceneSetting.NearClipPlane] = "Update Near Clipping Plane";
			Descriptions[SceneSetting.NearClipPlane] = "Updates the near clipping plane of the main camera to the recommended setting.\n\nThe recommended near clipping plane is designed to reduce eye fatigue. This option should remain checked unless you have a specific need to allow closer inspection of holograms and understand the impact of closely focused objects. (e.g. vergence accommodation conflict)";

			Names[SceneSetting.FieldOfView] = "Update Field of View";
			Descriptions[SceneSetting.FieldOfView] = "Updates the main camera Field of View.\n\nAllows the Unity Editor to more closely reflect what will be seen on the device at runtime. This option should remain checked unless you design-time requirements for a specific FOV.";
		}

		protected override void OnEnable()
		{
			// Pass to base first
			base.OnEnable();

			// Set size
			this.minSize = new Vector2(350, 240);
			this.maxSize = this.minSize;
		}
		#endregion // Overrides / Event Handlers
	}
}