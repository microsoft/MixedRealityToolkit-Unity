using CameraCapture;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CubeCreator : EditorWindow {

	#region Fields
	[SerializeField] int     _resolution = 128;
	[SerializeField] bool    _overrideGyro;
	[SerializeField] Vector3 _overrideDir = new Vector3(0,0,1);
	[SerializeField] float   _fovModifier = 1;
	[SerializeField] Object  _previewAsset;

	ICameraCapture _cam;
	CubeMapper     _map;
	Editor         _previewEditor;
	GUIStyle       _previewStyle;
	#endregion

	#region Menu Items
	[MenuItem("Mixed Reality Toolkit/Lighting Estimation/Camera Cubemap Creator")]
	static void Init() {
		CubeCreator window = GetWindow<CubeCreator>();
        window.Show();
	}

	[UnityEditor.MenuItem("Mixed Reality Toolkit/Lighting Estimation/Save Cubemap From Probe")]
	static void SaveFromProbe() {
		// Ensure we have a probe available
		GameObject probeObject = Selection.activeGameObject;
		if (probeObject.GetComponent<ReflectionProbe>() == null) {
			ReflectionProbe[] probes = FindObjectsOfType<ReflectionProbe>();
			if (probes.Length > 1) {
				Debug.LogWarning("More than one Reflection Probe found, defaulting to the first one! Please select the probe you want in the Heirarchy window.");
			} else if (probes.Length == 0) {
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
	private void OnEnable() {
		Texture2D tex = new Texture2D(1,1);
		tex.hideFlags = HideFlags.HideAndDontSave;
		tex.SetPixels(new Color[]{ new Color(1,1,1,0)});
		tex.Apply();

		_previewStyle = new GUIStyle();
		_previewStyle.normal.background = tex;

		_cam = new CameraCaptureWebcam(null, Camera.main.fieldOfView);

		CameraResolution resolution = new CameraResolution();
		resolution.nativeResolution = NativeResolutionMode.Largest;
		resolution.resize           = ResizeWhen.Never;

		_cam.Initialize(true, resolution, ()=>{
			
		});
	}
	private void OnDisable() {
		_cam.Shutdown();
		if (_map != null)
			_map.Destroy();
	}
	private void OnGUI() {
		// Display create new options
		GUILayout.Label("Cubemap Creator", EditorStyles.boldLabel);
		_resolution = Mathf.NextPowerOfTwo(EditorGUILayout.DelayedIntField("Cubemap Resolution", _resolution));
		if (GUILayout.Button("Create New")) {
			if (_map != null) {
				_map.Destroy();
			}
			_map = new CubeMapper();
			_map.Create(_cam.FieldOfView, _resolution);

			Stamp();
		}

		// Stamping options
		GUILayout.Label("Options", EditorStyles.boldLabel);
		_fovModifier = EditorGUILayout.Slider("Stamp FOV modifier", _fovModifier, 1, 4);
		_overrideGyro = EditorGUILayout.Toggle("Override gyro direction", _overrideGyro);
		EditorGUI.BeginDisabledGroup(!_overrideGyro);
		_overrideDir = EditorGUILayout.Vector3Field("Override direction", _overrideDir);
		EditorGUI.EndDisabledGroup();
		
		// Display options for the current cubemap
		GUILayout.Label("Current cubemap", EditorStyles.boldLabel);
		if (_map != null) {
			if (GUILayout.Button("Stamp from camera")) {
				Stamp();
			}
		}

		// Show an interactive preview if we have one
		Rect r = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
		if (_previewAsset != null) {
			if (_previewEditor == null)
				_previewEditor = Editor.CreateEditor(_previewAsset);
			
			_previewEditor.OnInteractivePreviewGUI(r, _previewStyle);
		} else {
			EditorGUI.DrawRect(r, new Color(0,0,0,0.1f));
		}
	}
	#endregion

	#region Helper Methods
	private void Stamp() {
		if (_map == null)
			return;

		_cam.RequestImage((tex,mat) => {
			Quaternion rot;
			if (_overrideGyro) {
				rot = Quaternion.LookRotation(_overrideDir, Vector3.up);
			} else {
				rot = EditorGyro.GetRotation();
			}
			_map.Stamp(tex, Vector3.zero, rot);
				
			string path = "Assets/CamCubemap.png";
			SaveCubemap(_map.Map, path);
			_previewAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
		});
	}
	#endregion

	#region Static Helper Methods
	static void SaveCubemap(Cubemap aMap, string aPath) {
		// Save the cubemap to file
		byte[] pngData = CubeMapper.CreateCubemapTex(aMap).EncodeToPNG();
		File.WriteAllBytes(aPath, pngData);
		AssetDatabase.Refresh();

		// Make sure it's marked as a cubemap
		Texture         file     = AssetDatabase.LoadAssetAtPath<Texture>(aPath);
		TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(aPath);
		if (file.dimension != UnityEngine.Rendering.TextureDimension.Cube) {
			importer.textureShape = TextureImporterShape.TextureCube;
			importer.SaveAndReimport();
		}
	}
	#endregion
}
