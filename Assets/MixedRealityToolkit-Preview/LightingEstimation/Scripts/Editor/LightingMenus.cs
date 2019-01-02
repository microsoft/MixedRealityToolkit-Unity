using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightingMenus {
	[MenuItem("Mixed Reality Toolkit/Lighting Estimation/Create Estimation Object", priority = 1)]
	static void CreateLEObject() {
		GameObject go = new GameObject("LightEstimation", typeof(LightingCapture));
		EditorGUIUtility.PingObject(go);
	}
}
