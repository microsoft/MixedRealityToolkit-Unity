// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif

using Microsoft.MixedReality.Toolkit.Extensions.MarkerDetection;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.MarkerDetection
{
    public class UnityArUcoMarkerDetectorPluginAPI
    {
        [DllImport("UnityArUcoMarkerDetectorPlugin", EntryPoint = "CheckPlugin")]
        internal static extern bool CheckPluginNative();

        [DllImport("UnityArUcoMarkerDetectorPlugin", EntryPoint = "Initialize")]
        internal static extern void InitializeNative(IntPtr iSpatialCoordinateSystem, int arucoDictionaryId, float markerSize);

        [DllImport("UnityArUcoMarkerDetectorPlugin", EntryPoint = "Dispose")]
        internal static extern void DisposeNative();

        [DllImport("UnityArUcoMarkerDetectorPlugin", EntryPoint = "DetectMarkers")]
        internal static extern bool DetectMarkersNative();

        [DllImport("UnityArUcoMarkerDetectorPlugin", EntryPoint = "GetDetectedMarkersCount")]
        internal static extern int GetDetectedMarkersCountNative();

        [DllImport("UnityArUcoMarkerDetectorPlugin", EntryPoint = "GetDetectedMarkerIds")]
        internal static extern bool GetDetectedMarkerIdsNative(int[] detectedIds, int size);

        [DllImport("UnityArUcoMarkerDetectorPlugin", EntryPoint = "GetDetectedMarkerPose")]
        internal static extern bool GetDetectedMarkerPoseNative(int detectedId, float[] position, float[] quaternion, float[] cameraToWorldUnity);

        [SerializeField] float _markerSize = 0.03f; // meters
        [SerializeField] int _requiredObservations = 15;
        private const int _arucoDictionaryId = 10; // equivalent to cv::aruco::DICT_6X6_250
        private bool _initialized = false;
        private Dictionary<int, List<Marker>> _markerObservations = new Dictionary<int, List<Marker>>();
        private Dictionary<int, Marker> _verifiedMarkers = new Dictionary<int, Marker>();

        public bool Initialize(float markerSize)
        {
            _markerSize = markerSize;
            bool dllExists = false;
            try
            {
                dllExists = CheckPluginNative();
            }
            catch { }

            if (!dllExists)
            {
                Debug.LogError("DLL does not exist or is configured incorrectly");
                return false;
            }

            try
            {
                IntPtr coordinateSystem = IntPtr.Zero;
#if UNITY_WSA
                coordinateSystem = WorldManager.GetNativeISpatialCoordinateSystemPtr();
#endif
                InitializeNative(coordinateSystem, _arucoDictionaryId, _markerSize);
                _initialized = true;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public void Destroy()
        {
            try
            {
                DisposeNative();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            _initialized = false;
            _markerObservations.Clear();
            _verifiedMarkers.Clear();
        }

        public bool DetectMarkers()
        {
            if (_initialized)
            {
                try
                {
                    if (DetectMarkersNative())
                    {
                        var count = GetDetectedMarkersCountNative();
                        if (count > 0)
                        {
                            int[] ids = new int[count];
                            if (GetDetectedMarkerIdsNative(ids, ids.Length))
                            {
                                foreach (var id in ids)
                                {
                                    var position = new float[3];
                                    var rodriguesRotation = new float[3];
                                    var cameraToWorldUnity = new float[16];
                                    if (GetDetectedMarkerPoseNative(id, position, rodriguesRotation, cameraToWorldUnity))
                                    {
                                        var cameraToWorld = new Matrix4x4();
                                        cameraToWorld.m00 = cameraToWorldUnity[0];
                                        cameraToWorld.m01 = cameraToWorldUnity[1];
                                        cameraToWorld.m02 = cameraToWorldUnity[2];
                                        cameraToWorld.m03 = cameraToWorldUnity[3];
                                        cameraToWorld.m10 = cameraToWorldUnity[4];
                                        cameraToWorld.m11 = cameraToWorldUnity[5];
                                        cameraToWorld.m12 = cameraToWorldUnity[6];
                                        cameraToWorld.m13 = cameraToWorldUnity[7];
                                        cameraToWorld.m20 = cameraToWorldUnity[8];
                                        cameraToWorld.m21 = cameraToWorldUnity[9];
                                        cameraToWorld.m22 = cameraToWorldUnity[10];
                                        cameraToWorld.m23 = cameraToWorldUnity[11];
                                        cameraToWorld.m30 = cameraToWorldUnity[12];
                                        cameraToWorld.m31 = cameraToWorldUnity[13];
                                        cameraToWorld.m32 = cameraToWorldUnity[14];
                                        cameraToWorld.m33 = cameraToWorldUnity[15];
                                        var matrix = "Unity CameraToWorld from Plugin ";
                                        matrix += cameraToWorld.m00 + ", ";
                                        matrix += cameraToWorld.m01 + ", ";
                                        matrix += cameraToWorld.m02 + ", ";
                                        matrix += cameraToWorld.m03 + ", ";
                                        matrix += cameraToWorld.m10 + ", ";
                                        matrix += cameraToWorld.m11 + ", ";
                                        matrix += cameraToWorld.m12 + ", ";
                                        matrix += cameraToWorld.m13 + ", ";
                                        matrix += cameraToWorld.m20 + ", ";
                                        matrix += cameraToWorld.m21 + ", ";
                                        matrix += cameraToWorld.m22 + ", ";
                                        matrix += cameraToWorld.m23 + ", ";
                                        matrix += cameraToWorld.m30 + ", ";
                                        matrix += cameraToWorld.m31 + ", ";
                                        matrix += cameraToWorld.m32 + ", ";
                                        matrix += cameraToWorld.m33;
                                        Debug.Log(matrix);

                                        Vector3 positionInOpenCVCameraSpace = new Vector3(position[0], position[1], position[2]);

                                        // The below logic ensures the following marker orientation:
                                        // Positive x axis is in the left direction of the observed marker
                                        // Positive y axis is in the upward direction of the observed marker
                                        // Positive z axis is facing outward from the observed marker
                                        Vector3 rodriguesVector = new Vector3(rodriguesRotation[0], rodriguesRotation[1], rodriguesRotation[2]);
                                        var angle = Mathf.Rad2Deg * rodriguesVector.magnitude;
                                        var axis = rodriguesVector.normalized;
                                        Quaternion rotationInOpenCVCameraSpace = Quaternion.AngleAxis(angle, axis);
                                        rotationInOpenCVCameraSpace = Quaternion.Euler(
                                            -1.0f * rotationInOpenCVCameraSpace.eulerAngles.x,
                                            rotationInOpenCVCameraSpace.eulerAngles.y,
                                            -1.0f * rotationInOpenCVCameraSpace.eulerAngles.z) * Quaternion.Euler(0, 0, 180);

                                        // CameraToWorld matrices assume a camera's front direction is in the negative z-direction
                                        // However, the values obtained from OpenCV assumes the camera's front direction is in the postiive z direction
                                        // Therefore, we negate the z components of our opencv camera transform
                                        var transformInOpenCVCameraSpace = Matrix4x4.TRS(positionInOpenCVCameraSpace, rotationInOpenCVCameraSpace, Vector3.one);
                                        var transformInUnityCameraSpace = transformInOpenCVCameraSpace;
                                        transformInUnityCameraSpace.m20 *= -1.0f;
                                        transformInUnityCameraSpace.m21 *= -1.0f;
                                        transformInUnityCameraSpace.m22 *= -1.0f;
                                        transformInUnityCameraSpace.m23 *= -1.0f;

                                        var transformInUnityWorld = cameraToWorld * transformInUnityCameraSpace;

                                        var positionInUnityWorld = transformInUnityWorld.GetColumn(3);
                                        var rotationInUnityWorld = Quaternion.LookRotation(transformInUnityWorld.GetColumn(2), transformInUnityWorld.GetColumn(1));

                                        var marker = new Marker(id, positionInUnityWorld, rotationInUnityWorld);
                                        Debug.Log("Marker detected: " + marker.ToString());

                                        if (!_markerObservations.ContainsKey(marker.Id))
                                        {
                                            _markerObservations.Add(id, new List<Marker>());
                                        }

                                        _markerObservations[marker.Id].Add(marker);
                                    }
                                }
                            }
                        }

                        foreach (var markerListPair in _markerObservations)
                        {
                            if (markerListPair.Value.Count > _requiredObservations)
                            {
                                var marker = CalcAverageMarker(markerListPair.Value);
                                _verifiedMarkers[marker.Id] = marker;
                            }
                        }

                        return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                Debug.Log("Marker detection api call failed");
                return false;
            }
            else
            {
                Debug.Log("Marker detector wasn't yet initialized");
                return false;
            }
        }

        public Dictionary<int, Marker> GetKnownMarkers()
        {
            return _verifiedMarkers;
        }

        private Marker CalcAverageMarker(List<Marker> markers)
        {
            var count = (float) markers.Count;
            var averagePos = Vector3.zero;
            int id = -1;
            List<Quaternion> rotations = new List<Quaternion>();
            foreach (var marker in markers)
            {
                averagePos += marker.Position / count;
                rotations.Add(marker.Rotation);
                id = marker.Id;
            }

            var averageRot = CalcAverageQuaternion(rotations.ToArray());
            return new Marker(id, averagePos, averageRot);
        }

        private Quaternion CalcAverageQuaternion(Quaternion[] quaternions)
        {
            Quaternion mean = quaternions[0];
            var text = "Quaternions: ";
            for (int i = 1; i < quaternions.Length; i++)
            {
                text += "{" + quaternions[i].x + ", " + quaternions[i].y + ", " + quaternions[i].z + ", " + quaternions[i].w + "}";
                float weight = 1.0f / (i + 1);
                mean = Quaternion.Slerp(mean, quaternions[i], weight);
            }
            Debug.Log(text);
            Debug.Log("Mean Quaternion: " + mean.x + ", " + mean.y + ", " + mean.z + ", " + mean.w);
            return mean;
        }
    }
}
