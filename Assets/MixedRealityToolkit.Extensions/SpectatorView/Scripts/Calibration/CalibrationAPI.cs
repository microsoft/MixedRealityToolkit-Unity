// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class CalibrationAPI
    {
        [DllImport("CalibrationPlugin", EntryPoint = "InitializeCalibration")]
        internal static extern bool InitializeCalibrationNative();

        [DllImport("CalibrationPlugin", EntryPoint = "ProcessChessboardImage")]
        internal static extern bool ProcessChessboardImageNative(
            byte[] image,
            int imageWidth,
            int imageHeight,
            int boardWidth,
            int boardHeight,
            byte[] cornersImage,
            byte[] heatmapImage,
            int cornerImageRadias,
            int heatmapWidth);

        [DllImport("CalibrationPlugin", EntryPoint = "ProcessChessboardIntrinsics")]
        internal static extern bool ProcessChessboardIntrinsicsNative(
            float squareSize,
            float[] intrinsics,
            int numIntrinsics);

        [DllImport("CalibrationPlugin", EntryPoint = "ProcessArUcoData")]
        internal static extern bool ProcessArUcoImageNative(
            byte[] image,
            int imageWidth,
            int imageHeight,
            int[] markerIds,
            int numMarkers,
            float[] markerCornersInWorld,
            float[] markerCornersRelativeToCamera,
            float[] planarCorners,
            int numMarkerCornerValues,
            float[] orientation);

        [DllImport("CalibrationPlugin", EntryPoint = "ProcessArUcoIntrinsics")]
        internal static extern bool ProcessArUcoIntrinsicsNative(
            float[] intrinsics,
            int numIntrinsics);

        [DllImport("CalibrationPlugin", EntryPoint = "ProcessIndividualArUcoExtrinsics")]
        internal static extern bool ProcessIndividualArUcoExtrinsicsNative(
            float[] intrinsics,
            float[] extrinsics,
            int numExtrinsics);

        [DllImport("CalibrationPlugin", EntryPoint = "ProcessGlobalArUcoExtrinsics")]
        internal static extern bool ProcessGlobalArUcoExtrinsicsNative(
            float[] intrinsics,
            float[] extrinsics,
            int numExtrinsics);

        [DllImport("CalibrationPlugin", EntryPoint = "GetLastErrorMessage")]
        internal static extern bool GetLastErrorMessageNative(
            char[] buff,
            int size);

        private bool initialized = false;

        public CalibrationAPI()
        {
            try
            {
                if (InitializeCalibrationNative())
                {
                    initialized = true;
                    return;
                }
            }
            catch { }

            initialized = false;
            Debug.LogError("Failed to initialize CalibrationPlugin dll for calibration");
        }

        public bool ProcessArUcoData(HeadsetCalibrationData data, byte[] dslrImage, int imageWidth, int imageHeight)
        {
            if (!initialized)
            {
                Debug.LogWarning("Calibration data wasn't processed. CalibrationPlugin dll failed to initialize for calibration");
                return false;
            }

            try
            {
                float[] orientation = new float[7];
                orientation[0] = data.headsetData.position.x;
                orientation[1] = data.headsetData.position.y;
                orientation[2] = -1.0f * data.headsetData.position.z;
                orientation[3] = data.headsetData.rotation.y;
                orientation[4] = -1.0f * data.headsetData.rotation.z;
                orientation[5] = -1.0f * data.headsetData.rotation.x;
                orientation[6] = data.headsetData.rotation.w;

                List<int> markerIds = new List<int>();
                List<float> markerCorners = new List<float>();
                foreach (var markerPair in data.markers)
                {
                    markerIds.Add(markerPair.id);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.topLeft.x);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.topLeft.y);
                    markerCorners.Add(-1.0f * markerPair.qrCodeMarkerCorners.topLeft.z);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.topRight.x);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.topRight.y);
                    markerCorners.Add(-1.0f * markerPair.qrCodeMarkerCorners.topRight.z);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.bottomRight.x);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.bottomRight.y);
                    markerCorners.Add(-1.0f * markerPair.qrCodeMarkerCorners.bottomRight.z);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.bottomLeft.x);
                    markerCorners.Add(markerPair.qrCodeMarkerCorners.bottomLeft.y);
                    markerCorners.Add(-1.0f * markerPair.qrCodeMarkerCorners.bottomLeft.z);
                }

                List<MarkerCorners> markersRelativeToCamera = CalcMarkerCornersRelativeToCamera(data);
                List<float> markerCornersRelativeToCamera = new List<float>();
                foreach (var corners in markersRelativeToCamera)
                {
                    markerCornersRelativeToCamera.Add(corners.topLeft.x);
                    markerCornersRelativeToCamera.Add(corners.topLeft.y);
                    markerCornersRelativeToCamera.Add(-1.0f * corners.topLeft.z);
                    markerCornersRelativeToCamera.Add(corners.topRight.x);
                    markerCornersRelativeToCamera.Add(corners.topRight.y);
                    markerCornersRelativeToCamera.Add(-1.0f * corners.topRight.z);
                    markerCornersRelativeToCamera.Add(corners.bottomRight.x);
                    markerCornersRelativeToCamera.Add(corners.bottomRight.y);
                    markerCornersRelativeToCamera.Add(-1.0f * corners.bottomRight.z);
                    markerCornersRelativeToCamera.Add(corners.bottomLeft.x);
                    markerCornersRelativeToCamera.Add(corners.bottomLeft.y);
                    markerCornersRelativeToCamera.Add(-1.0f * corners.bottomLeft.z);
                }

                List<MarkerCorners> planarMarkers = new List<MarkerCorners>();
                var originMarkerTransform = Matrix4x4.TRS(
                    data.markers[0].arucoMarkerCorners.topLeft,
                    data.markers[0].arucoMarkerCorners.orientation,
                    Vector3.one);
                var markerAsOriginTransform = originMarkerTransform.inverse;
                foreach (var markerPair in data.markers)
                {
                    var arUcoCorners = markerPair.arucoMarkerCorners;

                    // Note: a better flattening algorithm could likely be used
                    var planarCorners = new MarkerCorners();
                    planarCorners.topLeft = markerAsOriginTransform.MultiplyPoint(arUcoCorners.topLeft);
                    planarCorners.topRight = markerAsOriginTransform.MultiplyPoint(arUcoCorners.topRight);
                    planarCorners.bottomRight = markerAsOriginTransform.MultiplyPoint(arUcoCorners.bottomRight);
                    planarCorners.bottomLeft = markerAsOriginTransform.MultiplyPoint(arUcoCorners.bottomLeft);

                    planarCorners.topLeft.z = 0;
                    planarCorners.topRight.z = 0;
                    planarCorners.bottomRight.z = 0;
                    planarCorners.bottomLeft.z = 0;

                    planarMarkers.Add(planarCorners);
                }
                List<float> planarMarkerCorners = new List<float>();
                foreach (var corners in planarMarkers)
                {
                    planarMarkerCorners.Add(corners.topLeft.x);
                    planarMarkerCorners.Add(corners.topLeft.y);
                    planarMarkerCorners.Add(-1.0f * corners.topLeft.z);
                    planarMarkerCorners.Add(corners.topRight.x);
                    planarMarkerCorners.Add(corners.topRight.y);
                    planarMarkerCorners.Add(-1.0f * corners.topRight.z);
                    planarMarkerCorners.Add(corners.bottomRight.x);
                    planarMarkerCorners.Add(corners.bottomRight.y);
                    planarMarkerCorners.Add(-1.0f * corners.bottomRight.z);
                    planarMarkerCorners.Add(corners.bottomLeft.x);
                    planarMarkerCorners.Add(corners.bottomLeft.y);
                    planarMarkerCorners.Add(-1.0f * corners.bottomLeft.z);
                }

                if (!ProcessArUcoImageNative(
                    dslrImage,
                    imageWidth,
                    imageHeight,
                    markerIds.ToArray(),
                    markerIds.Count,
                    markerCorners.ToArray(),
                    markerCornersRelativeToCamera.ToArray(),
                    planarMarkerCorners.ToArray(),
                    markerCorners.Count,
                    orientation))
                {
                    PrintLastError();
                    return false;
                }

                return true;
            }
            catch
            {
                PrintLastError();
            }

            return false;
        }

        public static List<MarkerCorners> CalcMarkerCornersRelativeToCamera(HeadsetCalibrationData data)
        {
            List<MarkerCorners> markersRelativeToCamera = new List<MarkerCorners>();
            var cameraTransform = Matrix4x4.TRS(data.headsetData.position, data.headsetData.rotation, Vector3.one);
            var inverseCameraTransform = cameraTransform.inverse;
            foreach (var markerPair in data.markers)
            {
                var arUcoCorners = markerPair.arucoMarkerCorners;

                var cornersRelativeToCamera = new MarkerCorners();
                cornersRelativeToCamera.topLeft = inverseCameraTransform.MultiplyPoint(arUcoCorners.topLeft);
                cornersRelativeToCamera.topRight = inverseCameraTransform.MultiplyPoint(arUcoCorners.topRight);
                cornersRelativeToCamera.bottomRight = inverseCameraTransform.MultiplyPoint(arUcoCorners.bottomRight);
                cornersRelativeToCamera.bottomLeft = inverseCameraTransform.MultiplyPoint(arUcoCorners.bottomLeft);
                var orientationTransform = inverseCameraTransform * Matrix4x4.TRS(Vector3.zero, arUcoCorners.orientation, Vector3.one);
                cornersRelativeToCamera.orientation = Quaternion.LookRotation(orientationTransform.GetColumn(2), orientationTransform.GetColumn(1));
                markersRelativeToCamera.Add(cornersRelativeToCamera);
            }
            return markersRelativeToCamera;
        }

        public List<CalculatedCameraIntrinsics> CalculateArUcoIntrinsics()
        {
            if (!initialized)
            {
                Debug.LogWarning("Unable to calculate intrinsics. CalibrationPlugin dll failed to initialize for calibration");
                return null;
            }

            int numIntrinsics = 5;
            int intrinsicsSize = 12;
            float[] output = new float[numIntrinsics * intrinsicsSize];
            try
            {
                if (ProcessArUcoIntrinsicsNative(output, numIntrinsics))
                {
                    List<CalculatedCameraIntrinsics> intrinsics = new List<CalculatedCameraIntrinsics>();

                    // Note: The undistorted projection matrix is not calculated
                    for (int i = 0; i < numIntrinsics; i++)
                    {
                        int offset = intrinsicsSize * i;
                        var temp = new CalculatedCameraIntrinsics(
                            output[offset + 11],
                            new Vector2(output[offset + 0], output[offset + 1]), // Focal length
                            (uint)output[offset + 9], // Image width
                            (uint)output[offset + 10], // Image height
                            new Vector2(output[offset + 2], output[offset + 3]), // Principal point
                            new Vector3(output[offset + 4], output[offset + 5], output[offset + 6]), // Radial distortion
                            new Vector2(output[offset + 7], output[offset + 8]), // Tangential distortion
                            Matrix4x4.identity); // Undistorted projection matrix

                        intrinsics.Add(temp);
                    }

                    return intrinsics;
                }
            }
            catch
            {
                PrintLastError();
            }

            Debug.LogError("Failed to calculate intrinsics");
            return null;
        }

        public List<CalculatedCameraExtrinsics> CalculateIndividualArUcoExtrinsics(CameraIntrinsics intrinsics, int numExtrinsics)
        {
            int intrinsicsSize = 12;
            float[] inputIntrinsics = new float[intrinsicsSize];
            inputIntrinsics[0] = intrinsics.FocalLength.x;
            inputIntrinsics[1] = intrinsics.FocalLength.y;
            inputIntrinsics[2] = intrinsics.PrincipalPoint.x;
            inputIntrinsics[3] = intrinsics.PrincipalPoint.y;
            inputIntrinsics[4] = intrinsics.RadialDistortion.x;
            inputIntrinsics[5] = intrinsics.RadialDistortion.y;
            inputIntrinsics[6] = intrinsics.RadialDistortion.z;
            inputIntrinsics[7] = intrinsics.TangentialDistortion.x;
            inputIntrinsics[8] = intrinsics.TangentialDistortion.y;
            inputIntrinsics[9] = intrinsics.ImageWidth;
            inputIntrinsics[10] = intrinsics.ImageHeight;
            inputIntrinsics[11] = 0.0f; // reprojection error, unused

            int sizeExtrinsics = 7;
            float[] extrinsics = new float[numExtrinsics * sizeExtrinsics];
            if (!ProcessIndividualArUcoExtrinsicsNative(inputIntrinsics, extrinsics, numExtrinsics))
            {
                PrintLastError();
                return null;
            }
            else
            {
                List<CalculatedCameraExtrinsics> output = new List<CalculatedCameraExtrinsics>();
                for (int i = 0; i < numExtrinsics; i++)
                {
                    int offset = i * sizeExtrinsics;
                    bool succeeded = (extrinsics[offset] > 0.01f);

                    Vector3 rightHandedPosition = new Vector3(extrinsics[offset + 1], extrinsics[offset + 2], extrinsics[offset + 3]);

                    Vector3 rodriguesVector = new Vector3(extrinsics[offset + 4], extrinsics[offset + 5], extrinsics[offset + 6]);
                    var angle = Mathf.Rad2Deg * rodriguesVector.magnitude;
                    var axis = rodriguesVector.normalized;
                    Quaternion rightHandedRotation = Quaternion.AngleAxis(angle, axis);

                    var rightHandedMatrix = Matrix4x4.TRS(rightHandedPosition, rightHandedRotation, Vector3.one);
                    var leftHandedMatrix = rightHandedMatrix;
                    leftHandedMatrix.m02 *= -1.0f;
                    leftHandedMatrix.m12 *= -1.0f;
                    leftHandedMatrix.m22 *= -1.0f;
                    leftHandedMatrix.m32 *= -1.0f;

                    var inverse = leftHandedMatrix.inverse;
                    var inversePosition = inverse.GetColumn(3);
                    var inverseRotation = Quaternion.LookRotation(inverse.GetColumn(2), inverse.GetColumn(1)) * Quaternion.Euler(0, 0, 180);

                    var calcExtrinsics = new CalculatedCameraExtrinsics();
                    calcExtrinsics.ViewFromWorld = Matrix4x4.TRS(inversePosition, inverseRotation, Vector3.one);
                    calcExtrinsics.Succeeded = succeeded;
                    output.Add(calcExtrinsics);
                }

                return output;
            }
        }

        public CalculatedCameraExtrinsics CalculateGlobalArUcoExtrinsics(CameraIntrinsics intrinsics)
        {
            int intrinsicsSize = 12;
            float[] inputIntrinsics = new float[intrinsicsSize];
            inputIntrinsics[0] = intrinsics.FocalLength.x;
            inputIntrinsics[1] = intrinsics.FocalLength.y;
            inputIntrinsics[2] = intrinsics.PrincipalPoint.x;
            inputIntrinsics[3] = intrinsics.PrincipalPoint.y;
            inputIntrinsics[4] = intrinsics.RadialDistortion.x;
            inputIntrinsics[5] = intrinsics.RadialDistortion.y;
            inputIntrinsics[6] = intrinsics.RadialDistortion.z;
            inputIntrinsics[7] = intrinsics.TangentialDistortion.x;
            inputIntrinsics[8] = intrinsics.TangentialDistortion.y;
            inputIntrinsics[9] = intrinsics.ImageWidth;
            inputIntrinsics[10] = intrinsics.ImageHeight;
            inputIntrinsics[11] = 0.0f; // reprojection error, unused

            int sizeExtrinsics = 7;
            int numExtrinsics = 1;
            float[] extrinsics = new float[sizeExtrinsics];
            if (!ProcessGlobalArUcoExtrinsicsNative(inputIntrinsics, extrinsics, numExtrinsics))
            {
                PrintLastError();
                return null;
            }

            bool succeeded = (extrinsics[0] > 0.01f);

            Vector3 rightHandedPosition = new Vector3(extrinsics[1], extrinsics[2], extrinsics[3]);

            Vector3 rodriguesVector = new Vector3(extrinsics[4], extrinsics[5], extrinsics[6]);
            var angle = Mathf.Rad2Deg * rodriguesVector.magnitude;
            var axis = rodriguesVector.normalized;
            Quaternion rightHandedRotation = Quaternion.AngleAxis(angle, axis);

            var rightHandedMatrix = Matrix4x4.TRS(rightHandedPosition, rightHandedRotation, Vector3.one);
            var leftHandedMatrix = rightHandedMatrix;
            leftHandedMatrix.m02 *= -1.0f;
            leftHandedMatrix.m12 *= -1.0f;
            leftHandedMatrix.m22 *= -1.0f;
            leftHandedMatrix.m32 *= -1.0f;

            var inverse = leftHandedMatrix.inverse;
            var inversePosition = inverse.GetColumn(3);
            var inverseRotation = Quaternion.LookRotation(inverse.GetColumn(2), inverse.GetColumn(1)) * Quaternion.Euler(0, 0, 180);

            var calcExtrinsics = new CalculatedCameraExtrinsics();
            calcExtrinsics.ViewFromWorld = Matrix4x4.TRS(inversePosition, inverseRotation, Vector3.one);
            calcExtrinsics.Succeeded = succeeded;
            return calcExtrinsics;
        }

        public void PrintLastError()
        {
            char[] message = new char[256];
            if (GetLastErrorMessageNative(message, message.Length))
            {
                var output = new string(message);
                Debug.LogError(output);
            }
        }

        public bool ProcessChessboardImage(
            byte[] image,
            int imageWidth,
            int imageHeight,
            int boardWidth,
            int boardHeight,
            byte[] cornersImage = null,
            byte[] heatmapImage = null,
            int cornerImageRadias = 3,
            int heatmapWidth = 12)
        {
            cornersImage = cornersImage == null ? new byte[image.Length] : cornersImage;
            heatmapImage = heatmapImage == null ? new byte[image.Length] : heatmapImage;

            return ProcessChessboardImageNative(
                image,
                imageWidth,
                imageHeight,
                boardWidth,
                boardHeight,
                cornersImage,
                heatmapImage,
                cornerImageRadias,
                heatmapWidth);
        }

        public List<CalculatedCameraIntrinsics> CalculateChessboardIntrinsics(float chessSquareSize)
        {
            int numIntrinsics = 1;
            int intrinsicsSize = 12;
            float[] output = new float[numIntrinsics * intrinsicsSize];
            if (ProcessChessboardIntrinsicsNative(chessSquareSize, output, numIntrinsics))
            {
                List<CalculatedCameraIntrinsics> intrinsics = new List<CalculatedCameraIntrinsics>();

                // Note: The undistorted projection matrix is not calculated
                for (int i = 0; i < numIntrinsics; i++)
                {
                    int offset = intrinsicsSize * i;
                    var temp = new CalculatedCameraIntrinsics(
                        output[offset + 11],
                        new Vector2(output[offset + 0], output[offset + 1]), // Focal length
                        (uint)output[offset + 9], // Image width
                        (uint)output[offset + 10], // Image height
                        new Vector2(output[offset + 2], output[offset + 3]), // Principal point
                        new Vector3(output[offset + 4], output[offset + 5], output[offset + 6]), // Radial distortion
                        new Vector2(output[offset + 7], output[offset + 8]), // Tangential distortion
                        Matrix4x4.identity); // Undistorted projection matrix

                    intrinsics.Add(temp);
                }

                return intrinsics;
            }

            return null;
        }
    }
}
