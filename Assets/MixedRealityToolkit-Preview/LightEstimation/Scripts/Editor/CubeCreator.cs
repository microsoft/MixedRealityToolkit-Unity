// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Preview.CameraCapture;

namespace Microsoft.MixedReality.Toolkit.Preview.LightEstimation
{
	public class CubeCreator : EditorWindow
	{
		#region Fields
		[SerializeField] private int     resolution = 128;
		[SerializeField] private bool    overrideGyro;
		[SerializeField] private Vector3 overrideDir = new Vector3(0,0,1);
		[SerializeField] private float   fovModifier = 1;
		[SerializeField] private Object  previewAsset;

		private ICameraCapture cam;
		private CubeMapper     map;
		private Editor         previewEditor;
		private GUIStyle       previewStyle;
		#endregion

		#region Menu Items
		[MenuItem("Mixed Reality Toolkit/Light Estimation/Camera Cubemap Creator")]
		private static void Init()
		{
			CubeCreator window = GetWindow<CubeCreator>();
			window.Show();
		}

		[UnityEditor.MenuItem("Mixed Reality Toolkit/Light Estimation/Save Cubemap From Probe")]
		private static void SaveFromProbe()
		{
			// Ensure we have a probe available
			GameObject probeObject = Selection.activeGameObject;
			if (probeObject.GetComponent<ReflectionProbe>() == null) {
				ReflectionProbe[] probes = FindObjectsOfType<ReflectionProbe>();
				if (probes.Length > 1)
				{
					Debug.LogWarning("More than one Reflection Probe found, defaulting to the first one! Please select the probe you want in the Heirarchy window.");
				}
				else if (probes.Length == 0)
				{
					Debug.LogError("No Reflection Probes are in the scene to save!");
					return;
				}
				probeObject = probes[0].gameObject;
			}

			// Now save it
			ReflectionProbe probe = probeObject.GetComponent<ReflectionProbe>();
			SaveCubemap( (Cubemap)probe.customBakedTexture, "Assets/CamCubemap.png" );
		}
		#endregion

		#region Unity Events
		private void OnEnable()
		{
			Texture2D tex = new Texture2D(1,1);
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.SetPixels(new Color[]{ new Color(1,1,1,0)});
			tex.Apply();

			previewStyle = new GUIStyle();
			previewStyle.normal.background = tex;

			cam = new CameraCaptureWebcam(null, Camera.main.fieldOfView);

			CameraResolution resolution = new CameraResolution();
			resolution.nativeResolution = NativeResolutionMode.Largest;
			resolution.resize           = ResizeWhen.Never;

			cam.Initialize(true, resolution, ()=>{ });
		}
		private void OnDisable()
		{
			cam.Shutdown();
			if (map != null)
				map.Destroy();
		}
		private void OnGUI()
		{
			// Display create new options
			GUILayout.Label("Cubemap Creator", EditorStyles.boldLabel);
			resolution = Mathf.NextPowerOfTwo(EditorGUILayout.DelayedIntField("Cubemap Resolution", resolution));
			if (GUILayout.Button("Create New"))
			{
				if (map != null)
				{
					map.Destroy();
				}
				map = new CubeMapper();
				map.Create(cam.FieldOfView, resolution);

				Stamp();
			}

			// Stamping options
			GUILayout.Label("Options", EditorStyles.boldLabel);
			fovModifier = EditorGUILayout.Slider("Stamp FOV modifier", fovModifier, 1, 4);
			overrideGyro = EditorGUILayout.Toggle("Override gyro direction", overrideGyro);
			EditorGUI.BeginDisabledGroup(!overrideGyro);
			overrideDir = EditorGUILayout.Vector3Field("Override direction", overrideDir);
			EditorGUI.EndDisabledGroup();
		
			// Display options for the current cubemap
			GUILayout.Label("Current cubemap", EditorStyles.boldLabel);
			if (map != null)
			{
				if (GUILayout.Button("Stamp from camera"))
				{
					Stamp();
				}
			}

			// Show an interactive preview if we have one
			Rect r = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
			if (previewAsset != null)
			{
				if (previewEditor == null)
					previewEditor = Editor.CreateEditor(previewAsset);
			
				previewEditor.OnInteractivePreviewGUI(r, previewStyle);
			}
			else
			{
				EditorGUI.DrawRect(r, new Color(0,0,0,0.1f));
			}
		}
		#endregion

		#region Helper Methods
		private void Stamp()
		{
			if (map == null)
				return;

			cam.RequestImage((tex,mat) =>
			{
				Quaternion rot;
				if (overrideGyro)
				{
					rot = Quaternion.LookRotation(overrideDir, Vector3.up);
				}
				else
				{
					rot = EditorGyro.GetRotation();
				}
				map.Stamp(tex, Vector3.zero, rot);
				
				// Generate a unique filename
				string path = "Assets/CamCubemap.png";
				int    curr = 1;
				while (File.Exists(path))
				{
					curr += 1;
					path = string.Format("Assets/CamCubemap{0}.png", curr);
				}

				SaveCubemap(map.Map, path);
				previewAsset = AssetDatabase.LoadAssetAtPath<Object>(path);

				// Ping it, so the user knows we made it
				Selection.activeObject = previewAsset;
				EditorGUIUtility.PingObject(previewAsset);
			});
		}
		#endregion

		#region Static Helper Methods
		private static void SaveCubemap(Cubemap aMap, string aPath)
		{
			// Save the cubemap to file
			byte[] pngData = CubeMapper.CreateCubemapTex(aMap).EncodeToPNG();
			File.WriteAllBytes(aPath, pngData);
			AssetDatabase.Refresh();

			// Make sure it's marked as a cubemap
			Texture         file     = AssetDatabase.LoadAssetAtPath<Texture>(aPath);
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(aPath);
			if (file.dimension != UnityEngine.Rendering.TextureDimension.Cube)
			{
				importer.textureShape = TextureImporterShape.TextureCube;
				importer.SaveAndReimport();
			}
		}
		#endregion
	}
}