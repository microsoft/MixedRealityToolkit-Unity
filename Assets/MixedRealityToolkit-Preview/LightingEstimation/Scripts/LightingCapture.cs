// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Rendering;

using Microsoft.MixedReality.Toolkit.CameraCapture;
using Microsoft.MixedReality.Toolkit.LightEstimation;

public class LightingCapture : MonoBehaviour
{
	#region Fields
	[Header("Settings")]
	[Tooltip("Resolution (pixels) per-face of the generated lighting Cubemap.")]
	[SerializeField] int mapResolution = 128;
	
	[Header("Stamp Optimizations")]
	[Tooltip("Should the component only do the initial wraparound stamp? If true, only one picture will be taken, at the very beginning.")]
	[SerializeField]              bool  singleStampOnly     = false;
	[Tooltip("When stamping a camera picture onto the Cubemap, scale it up by this so it covers a little more space. This can mean fewer total stamps needed to complete the Cubemap, at the expense of a less perfect reflection.")]
	[SerializeField, Range(1, 4)] float stampFovMultiplier  = 2f;
	[Tooltip("This is the distance (meters) the camera must travel for a stamp to expire. When a stamp expires, the Camera will take another picture in that direction when given the opportunity. Zero means no expiration.")]
	[SerializeField]              float stampExpireDistance = 0;

	[Header("Directional Lighting")]
	[Tooltip("Should the system calculate information for a directional light? This will scrape the lower mips of the Cubemap to find the direction and color of the brightest values, and apply it to the scene's light.")]
	[SerializeField]             bool  useDirectionalLight       = true;
	[Tooltip("When finding the primary light color, it will average the brightest 20% of the pixels, and use that color for the light. This sets the cap for the saturation of that color.")]
	[SerializeField, Range(0,1)] float maxLightColorSaturation   = 0.3f;
	[Tooltip("The light eases into its new location when the information is updated. This is the speed at which it eases to its new destination, measured in degrees per second.")]
	[SerializeField]             float lightAngleAdjustPerSecond = 45f;

	[Header("Optional Overrides")]
	[Tooltip("Defaults to Camera.main. Which object should we be looking at for our orientation and position?")]
	[SerializeField] Transform       cameraOrientation;
	[Tooltip("Default will pick from the scene, or create one automatically. If you have settings you'd like to configure on your probe, hook it in here.")]
	[SerializeField] ReflectionProbe probe;
	[Tooltip("Default will pick the first directional light in the scene. If no directional light is found, one will be created!")]
	[SerializeField] Light           directionalLight;
	
	ICameraCapture captureCamera;
	CubeMapper     map = null;
	Histogram      histogram = new Histogram();
	Material       startSky;
	int            stampCount;
	Texture2D      tex;

	// For easing the light directions
	Quaternion lightTargetDir;
	Quaternion lightStartDir;
	float      lightStartTime = -1;
	float      lightTargetDuration;

	public CubeMapper CubeMapper { get { return map; } }
	#endregion

	#region Unity Events
	private void Awake ()
	{
		// Pick camera based on platform, PhotoCaptureCamera has camera controls,
		// but doesn't use front camera on PC like WebcamCamera does.
		
		#if WINDOWS_UWP && !UNITY_EDITOR
		captureCamera = new CameraCaptureUWP();
		#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		_camera = new CameraCaptureARFoundation();
		#else
		captureCamera = new CameraCaptureWebcam(Camera.main.transform, Camera.main.fieldOfView);
		#endif

		// Make sure we have access to a probe in the scene
		if (probe == null) 
			probe = FindObjectOfType<ReflectionProbe>();
		if (probe == null)
		{
			GameObject probeObj = new GameObject("_LightCaptureProbe", typeof(ReflectionProbe));
			probeObj.transform.SetParent(transform, false);
			probe = probeObj.GetComponent<ReflectionProbe>();
			
			probe.size          = Vector3.one * 10000;
			probe.boxProjection = false;
		}

		// Same with a camera object
		if (cameraOrientation == null)
			cameraOrientation = Camera.main.transform;

		// And check for a directional light in the scene
		if (useDirectionalLight && directionalLight == null)
		{
			Light[] lights = FindObjectsOfType<Light>();
			for (int i = 0; i < lights.Length; i++)
			{
				if (lights[i].type == LightType.Directional)
				{
					directionalLight = lights[i];
					break;
				}
			}
			if (directionalLight == null)
			{
				GameObject lightObj = new GameObject("_DirectionalLight", typeof(Light));
				lightObj.transform.SetParent(transform, false);
				directionalLight = lightObj.GetComponent<Light>();
				directionalLight.type = LightType.Directional;
			}
		}

		// Save initial settings in case we wish to restore them
		startSky = RenderSettings.skybox;
	}
	private void OnDisable()
	{
		// Restore and render a default probe
		if (probe != null && probe.isActiveAndEnabled)
		{
			probe.mode        = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
			probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
			probe.RenderProbe();
		}

		RenderSettings.skybox = startSky;

		captureCamera.Shutdown();
	}
	private void OnEnable()
	{
		probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
		
		CameraResolution resolution = new CameraResolution();
		resolution.nativeResolution = NativeResolutionMode.Smallest;
		resolution.resize           = ResizeWhen.Never;

		captureCamera.Initialize(true, resolution, ()=>
		{
			if (map == null)
			{
				map = new CubeMapper();
				map.Create(captureCamera.FieldOfView * stampFovMultiplier, mapResolution);
				map.StampExpireDistance = stampExpireDistance;
			}
			probe.customBakedTexture = map.Map;
			RenderSettings.skybox = map.SkyMaterial;
			RenderSettings.ambientMode = AmbientMode.Skybox;
		});
	}
	private void OnValidate()
	{
		if (map != null)
			map.StampExpireDistance = stampExpireDistance;
	}

	private void Update ()
	{
		// ditch out if we already have our first stamp
		if ((stampCount > 0 && singleStampOnly) )
			return;

		// check the cache to see if our current orientation would benefit from a new stamp
		if (captureCamera.IsReady && !captureCamera.IsRequestingImage)
		{
			if (!map.IsCached(cameraOrientation.position, cameraOrientation.rotation))
			{
				captureCamera.RequestImage(OnReceivedImage);
			}
		}

		// If we have a target light rotation to get to, start lerping! 
		if (lightStartTime > 0)
		{
			float t =  Mathf.Clamp01( (Time.time - lightStartTime) / lightTargetDuration );
			if (t >= 1)
				lightStartTime = 0;
			
			// This is a cheap cubic in/out easing function, so we aren't doing this linear (ew)
			t = t<.5f ? 4*t*t*t : (t-1)*(2*t-2)*(2*t-2)+1; 
			directionalLight.transform.localRotation = Quaternion.Lerp(lightStartDir, lightTargetDir, t);
		}
	}
	#endregion
	
	#region Private Methods
	private void OnReceivedImage(Texture texture, Matrix4x4 camera)
	{
		map.Stamp(texture, camera.GetColumn(3), camera.rotation);
		stampCount += 1;

		DynamicGI.UpdateEnvironment();

		if (useDirectionalLight)
			UpdateDirectionalLight();
	}
	private void UpdateDirectionalLight()
	{
		if (directionalLight == null)
			return;

		// Calculate the light direction
		Vector3 dir = map.GetWeightedDirection(ref histogram);
		dir.y = Mathf.Abs(dir.y); // Don't allow upward facing lights! In many cases, 'light' from below is just a large surface that reflects from an overhead source

		if (lightStartTime < 0 || lightAngleAdjustPerSecond == 0)
		{
			directionalLight.transform.forward = -dir;
			lightStartTime = 0;
		}
		else 
		{
			lightTargetDir = Quaternion.LookRotation( -dir );
			lightStartDir  = directionalLight.transform.localRotation;
			lightStartTime = Time.time;
			lightTargetDuration = Quaternion.Angle(lightTargetDir, lightStartDir) / lightAngleAdjustPerSecond;
			
			if (lightTargetDuration <= 0)
			{
				directionalLight.transform.forward = -dir;
				lightStartTime = 0;
			}
		}

		// Calculate a color and intensity from the cubemap's histogram

		// grab the color for the brightest 20% of the image
		float bright = histogram.FindPercentage(.8f);
		Color color  = histogram.GetColor(bright, 1); 

		// For the final color, we use a 'value' of 1, since we use the intensity for brightness of the light source.
		// Also, light sources are rarely very saturated, so we cap that as well
		float hue, sat, val;
		Color.RGBToHSV(color, out hue, out sat, out val);
		directionalLight.color = Color.HSVToRGB(hue, Mathf.Min(sat, maxLightColorSaturation), 1);
		directionalLight.intensity = bright;
	}
	#endregion

	#region Public Methods
	public void SetExposure(int exp)
	{
		CameraCaptureUWP cam = captureCamera as CameraCaptureUWP;
		if (cam != null)
			cam.Exposure = exp;
	}
	public void SetWhitebalance(int wb)
	{
		CameraCaptureUWP cam = captureCamera as CameraCaptureUWP;
		if (cam != null)
			cam.Whitebalance = wb;
	}

	public void Clear()
	{
		map.Clear();
		stampCount = 0;
	}
	#endregion
}
