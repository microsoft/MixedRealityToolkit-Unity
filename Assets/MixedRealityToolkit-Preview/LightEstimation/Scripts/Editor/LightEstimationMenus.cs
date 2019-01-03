// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LightEstimation {
	public class LightEstimationMenus
	{
		[MenuItem("Mixed Reality Toolkit/Light Estimation/Create Estimation Object", priority = 1)]
		static void CreateLEObject()
		{
			GameObject go = new GameObject("LightEstimation", typeof(LightCapture));
			EditorGUIUtility.PingObject(go);
		}
	}
}