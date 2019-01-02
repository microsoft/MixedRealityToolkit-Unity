using CameraCapture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightingCapture : MonoBehaviour {
	#region Fields
	[Header("Settings")]
	[Tooltip("Resolution (pixels) per-face of the generated lighting Cubemap.")]
	[SerializeField] int _mapResolution = 128;
	
	[Header("Stamp Optimizations")]
	[Tooltip("Should the component only do the initial wraparound stamp? If true, only one picture will be taken, at the very beginning.")]
	[SerializeField]              bool  _singleStampOnly     = false;
	[Tooltip("When stamping a camera picture onto the Cubemap, scale it up by this so it covers a little more space. This can mean fewer total stamps needed to complete the Cubemap, at the expense of a less perfect reflection.")]
	[SerializeField, Range(1, 4)] float _stampFovMultiplier  = 2f;
	[Tooltip("This is the distance (meters) the camera must travel for a stamp to expire. When a stamp expires, the Camera will take another picture in that direction when given the opportunity. Zero means no expiration.")]
	[SerializeField]              float _stampExpireDistance = 0;

	[Header("Directional Lighting")]
	[Tooltip("Should the system calculate information for a directional light? This will scrape the lower mips of the Cubemap to find the direction and color of the brightest values, and apply it to the scene's light.")]
	[SerializeField]             bool  _useDirectionalLight       = true;
	[Tooltip("When finding the primary light color, it will average the brightest 20% of the pixels, and use that color for the light. This sets the cap for the saturation of that color.")]
	[SerializeField, Range(0,1)] float _maxLightColorSaturation   = 0.3f;
	[Tooltip("The light eases into its new location when the information is updated. This is the speed at which it eases to its new destination, measured in degrees per second.")]
	[SerializeField]             float _lightAngleAdjustPerSecond = 45f;

	[Header("Optional Overrides")]
	[Tooltip("Defaults to Camera.main. Which object should we be looking at for our orientation and position?")]
	[SerializeField] Transform       _cameraOrientation;
	[Tooltip("Default will pick from the scene, or create one automatically. If you have settings you'd like to configure on your probe, hook it in here.")]
	[SerializeField] ReflectionProbe _probe;
	[Tooltip("Default will pick the first directional light in the scene. If no directional light is found, one will be created!")]
	[SerializeField] Light           _directionalLight;
	
	ICameraCapture _camera;
	CubeMapper     _map = null;
	Histogram      _histogram = new Histogram();
	Material       _startSky;
	int            _stampCount;
	Texture2D      _tex;

	// For easing the light directions
	Quaternion _lightTargetDir;
	Quaternion _lightStartDir;
	float      _lightStartTime = -1;
	float      _lightTargetDuration;

	public CubeMapper CubeMapper { get { return _map; } }
	#endregion

	#region Unity Events
	private void Awake () {
		// Pick camera based on platform, PhotoCaptureCamera has camera controls,
		// but doesn't use front camera on PC like WebcamCamera does.
		
		#if WINDOWS_UWP && !UNITY_EDITOR
		_camera = new CameraCaptureUWP();
		#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		_camera = new CameraCaptureARFoundation();
		#else
		_camera = new CameraCaptureWebcam(Camera.main.transform, Camera.main.fieldOfView);
		#endif

		// Make sure we have access to a probe in the scene
		if (_probe == null) 
			_probe = FindObjectOfType<ReflectionProbe>();
		if (_probe == null) {
			GameObject probeObj = new GameObject("_LightCaptureProbe", typeof(ReflectionProbe));
			probeObj.transform.SetParent(transform, false);
			_probe = probeObj.GetComponent<ReflectionProbe>();
			
			_probe.size          = Vector3.one * 10000;
			_probe.boxProjection = false;
		}

		// Same with a camera object
		if (_cameraOrientation == null)
			_cameraOrientation = Camera.main.transform;

		// And check for a directional light in the scene
		if (_useDirectionalLight && _directionalLight == null) {
			Light[] lights = FindObjectsOfType<Light>();
			for (int i = 0; i < lights.Length; i++) {
				if (lights[i].type == LightType.Directional) {
					_directionalLight = lights[i];
					break;
				}
			}
			if (_directionalLight == null) {
				GameObject lightObj = new GameObject("_DirectionalLight", typeof(Light));
				lightObj.transform.SetParent(transform, false);
				_directionalLight = lightObj.GetComponent<Light>();
				_directionalLight.type = LightType.Directional;
			}
		}

		// Save initial settings in case we wish to restore them
		_startSky = RenderSettings.skybox;
	}
	private void OnDisable() {
		// Restore and render a default probe
		if (_probe != null && _probe.isActiveAndEnabled) {
			_probe.mode        = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
			_probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
			_probe.RenderProbe();
		}

		RenderSettings.skybox = _startSky;

		_camera.Shutdown();
	}
	private void OnEnable() {
		_probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
		
		CameraResolution resolution = new CameraResolution();
		resolution.nativeResolution = NativeResolutionMode.Smallest;
		resolution.resize           = ResizeWhen.Never;

		_camera.Initialize(true, resolution, ()=>{
			if (_map == null) {
				_map = new CubeMapper();
				_map.Create(_camera.FieldOfView * _stampFovMultiplier, _mapResolution);
				_map.StampExpireDistance = _stampExpireDistance;
			}
			_probe.customBakedTexture = _map.Map;
			RenderSettings.skybox = _map.SkyMaterial;
			RenderSettings.ambientMode = AmbientMode.Skybox;
		});
	}
	private void OnValidate() {
		if (_map != null) {
			_map.StampExpireDistance = _stampExpireDistance;
		}
	}

	private void Update () {
		// ditch out if we already have our first stamp
		if ((_stampCount > 0 && _singleStampOnly) )
			return;

		// check the cache to see if our current orientation would benefit from a new stamp
		if (_camera.IsReady && !_camera.IsRequestingImage) {
			if (!_map.IsCached(_cameraOrientation.position, _cameraOrientation.rotation)) {
				_camera.RequestImage(OnReceivedImage);
			}
		}

		// If we have a target light rotation to get to, start lerping! 
		if (_lightStartTime > 0) {
			float t =  Mathf.Clamp01( (Time.time - _lightStartTime) / _lightTargetDuration );
			if (t >= 1)
				_lightStartTime = 0;
			
			// This is a cheap cubic in/out easing function, so we aren't doing this linear (ew)
			t = t<.5f ? 4*t*t*t : (t-1)*(2*t-2)*(2*t-2)+1; 
			_directionalLight.transform.localRotation = Quaternion.Lerp(_lightStartDir, _lightTargetDir, t);
		}
	}
	#endregion

	private void OnGUI()
	{
		if (_camera != null)
			GUILayout.Label("FOV:" + _camera.FieldOfView);
	}
	#region Private Methods
	private void OnReceivedImage(Texture aTexture, Matrix4x4 aCamera) {
		Debug.Log("FOV: " + _camera.FieldOfView);
		_map.Stamp(aTexture, aCamera.GetColumn(3), aCamera.rotation);
		_stampCount += 1;

		DynamicGI.UpdateEnvironment();

		if (_useDirectionalLight) {
			UpdateDirectionalLight();
		}
	}
	private void UpdateDirectionalLight() {
		if (_directionalLight == null)
			return;

		// Calculate the light direction
		Vector3 dir = _map.GetWeightedDirection(ref _histogram);
		dir.y = Mathf.Abs(dir.y); // Don't allow upward facing lights! In many cases, 'light' from below is just a large surface that reflects from an overhead source

		if (_lightStartTime < 0 || _lightAngleAdjustPerSecond == 0) {
			_directionalLight.transform.forward = -dir;
			_lightStartTime = 0;
		} else {
			_lightTargetDir = Quaternion.LookRotation( -dir );
			_lightStartDir  = _directionalLight.transform.localRotation;
			_lightStartTime = Time.time;
			_lightTargetDuration = Quaternion.Angle(_lightTargetDir, _lightStartDir) / _lightAngleAdjustPerSecond;
			
			if (_lightTargetDuration <= 0) {
				_directionalLight.transform.forward = -dir;
				_lightStartTime = 0;
			}
		}

		// Calculate a color and intensity from the cubemap's histogram

		// grab the color for the brightest 20% of the image
		float bright = _histogram.FindPercentage(.8f);
		Color color  = _histogram.GetColor(bright, 1); 

		// For the final color, we use a 'value' of 1, since we use the intensity for brightness of the light source.
		// Also, light sources are rarely very saturated, so we cap that as well
		float hue, sat, val;
		Color.RGBToHSV(color, out hue, out sat, out val);
		_directionalLight.color = Color.HSVToRGB(hue, Mathf.Min(sat, _maxLightColorSaturation), 1);
		_directionalLight.intensity = bright;
	}
	#endregion

	#region Public Methods
	public void SetExposure(int exp)
	{
		CameraCaptureUWP cam = _camera as CameraCaptureUWP;
		if (cam != null)
			cam.Exposure = exp;
	}
	public void SetWhitebalance(int wb)
	{
		CameraCaptureUWP cam = _camera as CameraCaptureUWP;
		if (cam != null)
			cam.Whitebalance = wb;
	}

	public void Clear() {
		_map.Clear();
		_stampCount = 0;
	}
	#endregion
}
