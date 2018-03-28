using System.Collections;
using System.Collections.Generic;
using ARCA;
using UnityEngine;
using UnityEditor;

namespace HoloToolkit.ARCapture
{
	[CustomEditor(typeof(MarkerGeneration3D), true)]
	public class MarkerGeneration3DEditor : Editor {

		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();

			if(GUILayout.Button("Generate"))
			{
				MarkerGeneration3D cubeToSphere = (MarkerGeneration3D)target;
				cubeToSphere.Generate();
			}
		}
	}
}
