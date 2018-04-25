using UnityEngine;
using UnityEditor;

namespace HoloToolkit.ARCapture
{
	[CustomEditor(typeof(MarkerGeneration3D), true)]
	public class MarkerGeneration3DEditor : Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if(GUILayout.Button("Generate"))
			{
				MarkerGeneration3D cubeToSphere = (MarkerGeneration3D)target;
				cubeToSphere.Generate();
			}
		}
	}
}
