// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Source code for the EditorGyro DLL can be found here:
// https://github.com/maluoi/UnityEditorGyro

// DLLs are not essential, but are nice for testing in-editor for some computers, they are currently not included
// You can find them precompiled in this package here: https://github.com/maluoi/UnityEditorGyro/tree/master/Downloads
// Here are some quick steps to do this:
// 1. Extract the x86 and x64 DLLs from the .unitypackage, and put them into the corresponding folders 
// of LightEstimation-Examples/EditorGyro/Plugins.
// 2. Mark both DLLs for the Editor platform only using Unity's inspector.
// 3. Remove this #define.
#define NO_EDITORGYRO_DLL

using UnityEngine;
#if UNITY_EDITOR_WIN
using System.Runtime.InteropServices;
#endif

namespace Microsoft.MixedReality.Toolkit.Preview.LightEstimation
{
	public static class EditorGyro
	{
		#if UNITY_EDITOR_WIN && !NO_EDITORGYRO_DLL
		[DllImport("GyroDLL", EntryPoint = "GyroInitialize")]
		private static extern int Initialize();
		[DllImport("GyroDLL", EntryPoint = "GyroIsInitialized")]
		private static extern int IsInitialized();
		[DllImport("GyroDLL", EntryPoint = "GyroGetRotation")]
		private static extern Quaternion GetRawRotation();

		private static bool isInitialized = false;
		/// <summary>
		/// Gets the current orientation of the hardware from the Gyroscope! This is equivalent to
		/// Unity's Input.gyro.attitude, and will in fact call Input.gyro.attitude when built to
		/// your platform of preference.
		/// </summary>
		public static Quaternion GetRotation()
		{
			if (!isInitialized)
			{
				isInitialized = Initialize() >= 0;
			}
			if (!isInitialized)
			{
				return Quaternion.identity;
			}
			return Quaternion.AngleAxis(90, Vector3.right) * GetRawRotation();
		}
		#else
		/// <summary>
		/// Calls straight to Input.gyro.attitude on Mac, or on device.
		/// </summary>
		public static Quaternion GetRotation()
		{
			return Input.gyro.attitude;
		}
		#endif
	}
}