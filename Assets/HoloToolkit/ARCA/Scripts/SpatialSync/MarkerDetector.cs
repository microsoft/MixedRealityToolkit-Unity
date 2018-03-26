// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ARCA
{
	public class MarkerDetector 
	{
		[DllImport("OpenCVWrapper", EntryPoint="MarkerDetector_Initialize")]
		internal static extern void InitalizeMarkerDetector();
		[DllImport("OpenCVWrapper", EntryPoint="MarkerDetector_Terminate")]
		internal static extern void TerminateMarkerDetector();
		[DllImport("OpenCVWrapper", EntryPoint="MarkerDetector_DetectMarkers")]
		internal static extern bool DetectMarkers(int _imageWidth, int _imageHeight, IntPtr _imageDate, float _markerSize);
		[DllImport("OpenCVWrapper", EntryPoint="MarkerDetector_GetNumMarkersDetected")]
		internal static extern bool GetNumMarkersDetected(out int _numMarkersDetected);
		[DllImport("OpenCVWrapper", EntryPoint="MarkerDetector_GetDetectedMarkerIds")]
		internal static extern bool GetDetectedMarkerIds(IntPtr _detectedMarkers);
		[DllImport("OpenCVWrapper", EntryPoint="MarkerDetector_GetDetectedMarkerPose")]	
		internal static extern bool GetDetectedMarkerPose(int _markerId, out float _xPos, out float _yPos, out float _zPos, out float _xRot, out float _yRot, out float _zRot);

		// Initalize the detection code
		public void Initialize() 
		{
			InitalizeMarkerDetector();
		}

		// Terminate the detection code
		public void Terminate()
		{
			TerminateMarkerDetector();
		}

		public bool Detect(List<byte> _imageData, int _width, int _height, float _markerSize)
		{
			unsafe
			{
				fixed (byte* fbyteArray = _imageData.ToArray())
				{
					return DetectMarkers(_width, _height, new IntPtr(fbyteArray), _markerSize);
				}
			}
		}

		public int GetNumMarkersDetected()
		{
			int numMarkersDetected;
			GetNumMarkersDetected(out numMarkersDetected);
			return numMarkersDetected; 
		}

		public bool GetMarkerIds(out int[] _markerIds)
		{
			int numMarkersDetected;
			GetNumMarkersDetected(out numMarkersDetected);
				
			int[] markerIds = new int[numMarkersDetected];
			unsafe
			{
				fixed(int* fmarkerIds = markerIds)
				{
					bool success = GetDetectedMarkerIds(new IntPtr(fmarkerIds));
					_markerIds = new int[numMarkersDetected];
					for(int i=0; i<numMarkersDetected; i++)
					{
						_markerIds[i] = fmarkerIds[i];
					}

					return success;
				}
			}
		}

		public bool GetMarkerPose(int _markerId, out Vector3 _markerPosition, out Quaternion _markerRotation)
		{
			_markerPosition = Vector3.zero;
			_markerRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
			
			int numMarkersDetected;
				
			GetNumMarkersDetected(out numMarkersDetected);
			
			if(numMarkersDetected <= 0)
			{
				return false;
			}

			float xPos, yPos, zPos, xRot, yRot, zRot;
			bool success = GetDetectedMarkerPose(_markerId, out xPos, out yPos, out zPos, out xRot, out yRot, out zRot);
			if(success)
			{
				Debug.Log("Found marker with id: " + _markerId);

				// Account for the offset of the hololens camera from the transform pos
				Vector3 position = new Vector3(xPos, yPos, zPos);
				Vector3 offset = new Vector3(0.00f, 0.0f, 0.06f);
				position += offset;
				_markerPosition = Camera.main.cameraToWorldMatrix.MultiplyPoint(new Vector3(position.x, -position.y, -position.z));

				Vector3 rotation = new Vector3(xRot, yRot, zRot);
				float theta = rotation.magnitude;
				rotation.Normalize();
				_markerRotation = Camera.main.transform.rotation * Quaternion.AngleAxis(theta * Mathf.Rad2Deg, rotation);
				_markerRotation = Quaternion.Euler(_markerRotation.eulerAngles.x,_markerRotation.eulerAngles.y, -_markerRotation.eulerAngles.z);

				return true;
			}
			else
			{
				Debug.Log("Could not find marker with id: " + _markerId);
			}

			return false;
		}
	}
}