// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARCA
{
	public class MarkerDetectionHololens : MonoBehaviour 
	{
		// Delegate fires when marker with id MarkerId is found 
		// markerId: the marker id
		// pos : the marker position in World-Space
		// rot : the marker rotation
		public delegate void OnMarkerDetectedDelegate(int markerId, Vector3 pos, Quaternion rot);
		public OnMarkerDetectedDelegate OnMarkerDetected;
		[Tooltip("A component for capturing photos from the HoloLens webcam")]
		public CameraCaptureHololens HoloLensCapture;
		[Tooltip("The physical size of the marker to search for in metres")]
		public float MarkerSize = 0.05f;
		[Tooltip("Sound played when the hololens enters capturing mode")]
		public AudioSource SuccessSound;
		[Tooltip("Time the user has to hold an airtap before entering capturing mode")]
		public float AirtapTimeToCapture;
        [Tooltip("Time the camera will be capturing")]
	    public float CaptureTimeout = 10f;
	    
		private float currentCaptureTimeout;
        private MarkerDetector detector;
		private bool capturing = false;
		private Quaternion startRotation;

		void Start () 
		{
#if NETFX_CORE
			detector = new MarkerDetector();
			detector.Initialize();

			HoloLensCapture.OnFrameCapture += ProcessImage;
#endif
		}

	    void Update()
	    {
	        if (!capturing || currentCaptureTimeout <= 0)
			{
				return;
			} 
				
	        currentCaptureTimeout -= Time.deltaTime;

	        if (currentCaptureTimeout <= 0 && capturing)
			{
				Debug.Log("Capture timed out");
                StopCapture();
			}
	    }

		public void StartCapture()
		{
		    currentCaptureTimeout = CaptureTimeout;
#if NETFX_CORE
            if(!capturing)
			{
            	HoloLensCapture.StartCapture();
            }
#else
            Debug.LogWarning("Capturing only supported on HoloLens platform");
#endif
		    capturing = true;
        }

        /// <summary>
        /// 
        /// </summary>
	    public void KeepAliveCapture()
	    {
	        currentCaptureTimeout = CaptureTimeout;
	    }

		public void StopCapture()
		{
			capturing = false;
		    currentCaptureTimeout = 0;

#if NETFX_CORE
			HoloLensCapture.StopCapture();
#else
            Debug.LogWarning("Capturing only supported on HoloLens platform");
#endif
		}

		void ProcessImage(List<byte> imageData, int imageWidth, int imageHeight)
		{
#if NETFX_CORE
			detector.Detect(imageData, imageWidth, imageHeight, MarkerSize);
			Vector3 pos; 
			Quaternion rot; 
			int[] detectedMarkerIds;
			detector.GetMarkerIds(out detectedMarkerIds);

			for(int i=0; i<detectedMarkerIds.Length; i++)
			{
				if(!detector.GetMarkerPose(detectedMarkerIds[i], out pos, out rot))
				{
					Debug.Log("Can't resolve marker position for marker id: " + detectedMarkerIds[i]);
					continue;
				}
				else
				{
					if(OnMarkerDetected != null)
					{
						OnMarkerDetected(detectedMarkerIds[i], pos, rot);
					}
				}
			}
#endif
		}

		void OnDestroy()
		{
#if NETFX_CORE
			detector.Terminate();
			HoloLensCapture.OnFrameCapture -= ProcessImage;
#endif
		}
	}
}