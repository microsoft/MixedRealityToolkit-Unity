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
#if UNITY_WSA
        [DllImport("SpectatorViewPlugin", EntryPoint = "InitializeCalibration")]
#else
        [DllImport("SpectatorViewPlugin.Editor", EntryPoint = "InitializeCalibration")]
#endif
        internal static extern bool InitializeCalibrationNative();

#if UNITY_WSA
        [DllImport("SpectatorViewPlugin", EntryPoint = "ProcessChessboardImage")]
#else
        [DllImport("SpectatorViewPlugin.Editor", EntryPoint = "ProcessChessboardImage")]
#endif
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

#if UNITY_WSA
        [DllImport("SpectatorViewPlugin", EntryPoint = "ProcessChessboardIntrinsics")]
#else
        [DllImport("SpectatorViewPlugin.Editor", EntryPoint = "ProcessChessboardIntrinsics")]
#endif
        internal static extern bool ProcessChessboardIntrinsicsNative(
            float squareSize,
            float[] intrinsics,
            int numIntrinsics);

#if UNITY_WSA
        [DllImport("SpectatorViewPlugin", EntryPoint = "ProcessArUcoData")]
#else
        [DllImport("SpectatorViewPlugin.Editor", EntryPoint = "ProcessArUcoData")]
#endif
        internal static extern bool ProcessArUcoImageNative(
            byte[] image,
            int imageWidth,
            int imageHeight,
            int[] markerIds,
            int numMarkers,
            float[] markerCornersInWorld,
            float[] markerCornersRelativeToCamera,
            int numMarkerCornerValues);

#if UNITY_WSA
        [DllImport("SpectatorViewPlugin", EntryPoint = "ProcessIndividualArUcoExtrinsics")]
#else
        [DllImport("SpectatorViewPlugin.Editor", EntryPoint = "ProcessIndividualArUcoExtrinsics")]
#endif
        internal static extern bool ProcessIndividualArUcoExtrinsicsNative(
            float[] intrinsics,
            float[] extrinsics,
            int numExtrinsics);

#if UNITY_WSA
        [DllImport("SpectatorViewPlugin", EntryPoint = "ProcessGlobalArUcoExtrinsics")]
#else
        [DllImport("SpectatorViewPlugin.Editor", EntryPoint = "ProcessGlobalArUcoExtrinsics")]
#endif
        internal static extern bool ProcessGlobalArUcoExtrinsicsNative(
            float[] intrinsics,
            float[] extrinsics,
            int numExtrinsics);

#if UNITY_WSA
        [DllImport("SpectatorViewPlugin", EntryPoint = "GetLastErrorMessage")]
#else
        [DllImport("SpectatorViewPlugin.Editor", EntryPoint = "GetLastErrorMessage")]
#endif
        internal static extern bool GetLastErrorMessageNative(
            char[] buff,
            int size);

        private bool initialized = false;

        /// <summary>
        /// Creates an API instance and initializes the plugin.
        /// Note: Creating multiple CalibrationAPI instances at the same time is not supported.
        /// </summary>
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

        /// <summary>
        /// Processes an image containing ArUco markers cooresponding with world space coordinates provided in the headset calibraiton data.
        /// </summary>
        /// <param name="data">Headset calibration data</param>
        /// <param name="dslrImage">RGB24 dslr image data</param>
        /// <param name="imageWidth">Image width in pixels</param>
        /// <param name="imageHeight">Image height in pixels</param>
        /// <returns>Returns true if ArUco markers were found in the image that had valid world space coordinates in the provided headset calibration data.</returns>
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

                if (!ProcessArUcoImageNative(
                    dslrImage,
                    imageWidth,
                    imageHeight,
                    markerIds.ToArray(),
                    markerIds.Count,
                    markerCorners.ToArray(),
                    markerCornersRelativeToCamera.ToArray(),
                    markerCorners.Count))
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

        /// <summary>
        /// Creates a list of marker pairs that are corrected for the unity camera location specified in the headset calibration data.
        /// </summary>
        /// <param name="data">Headset calibration data</param>
        /// <returns>Marker corners transformed to correct for the unity camera orientation</returns>
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

        /// <summary>
        /// Calculates camera extrinsics for each provided aruco data set
        /// </summary>
        /// <param name="intrinsics">Camera intrinsics for the dslr camera</param>
        /// <param name="numExtrinsics">The number of extrinsics that should be calculated. This should be equal to the number of successful calls made to <see cref="ProcessArUcoData(HeadsetCalibrationData, byte[], int, int)"/></param>
        /// <returns>A list of camera extrinsic values</returns>
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

        /// <summary>
        /// Calculates one camera extrinsic value based on all provided ArUco data sets
        /// </summary>
        /// <param name="intrinsics">Camera intrinsics</param>
        /// <returns>Camera extrinsics</returns>
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

        /// <summary>
        /// Prints the last error available for the SpectatorViewPlugin/SpectatorViewPlugin.Editor dll
        /// </summary>
        public void PrintLastError()
        {
            char[] message = new char[256];
            if (GetLastErrorMessageNative(message, message.Length))
            {
                var output = new string(message);
                Debug.LogError(output);
            }
        }

        /// <summary>
        /// Process a chessboard image for calculating camera intrinsics
        /// </summary>
        /// <param name="image">RGB24 image data in bytes</param>
        /// <param name="imageWidth">image width in pixels</param>
        /// <param name="imageHeight">image height in pixels</param>
        /// <param name="boardWidth">chessboard width in squares</param>
        /// <param name="boardHeight">chessboard height in squares</param>
        /// <param name="cornersImage">RGB24 helper image byte data for displaying detected corners</param>
        /// <param name="heatmapImage">RGB24 helper image byte data for displaying detected corner heatmaps</param>
        /// <param name="cornerImageRadias">radias for drawing detected corners in the cornersImage</param>
        /// <param name="heatmapWidth">the number of horizontal bins used when generating the heatmapImage</param>
        /// <returns>Returns true if the specified chessboard was detected in the provided image, otherwise false</returns>
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

        /// <summary>
        /// Calculates camera intrinsics using all provided chessboard images
        /// </summary>
        /// <param name="chessSquareSize">Size of a chessboard square in meters</param>
        /// <returns>Returns the calculated camera intriniscs, null if the camera intrinsics could not be calculated</returns>
        public CalculatedCameraIntrinsics CalculateChessboardIntrinsics(float chessSquareSize)
        {
            int intrinsicsSize = 12;
            float[] output = new float[intrinsicsSize];
            if (ProcessChessboardIntrinsicsNative(chessSquareSize, output, 1))
            {
                var intrinsics = new CalculatedCameraIntrinsics(
                    output[11],
                    new Vector2(output[0], output[1]), // Focal length
                    (uint)output[9], // Image width
                    (uint)output[10], // Image height
                    new Vector2(output[2], output[3]), // Principal point
                    new Vector3(output[4], output[5], output[6]), // Radial distortion
                    new Vector2(output[7], output[8]), // Tangential distortion
                    Matrix4x4.identity); // Undistorted projection matrix, Note: not currently calculated

                return intrinsics;
            }

            return null;
        }
    }
}
