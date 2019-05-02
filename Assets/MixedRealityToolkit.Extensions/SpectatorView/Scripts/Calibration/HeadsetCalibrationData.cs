// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class HeadsetCalibrationData
    {
        public float timestamp;
        public HeadsetData headsetData;
        public List<MarkerPair> markers;
        public PVImageData imageData;

        public byte[] Serialize()
        {
            var str = JsonUtility.ToJson(this);
            var payload = Encoding.ASCII.GetBytes(str);
            return payload;
        }

        public static bool TryDeserialize(byte[] payload, out HeadsetCalibrationData headsetCalibrationData)
        {
            headsetCalibrationData = null;

            try
            {
                var str = Encoding.ASCII.GetString(payload);
                headsetCalibrationData = JsonUtility.FromJson<HeadsetCalibrationData>(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    [Serializable]
    internal struct MarkerPair
    {
        public int id;
        public MarkerCorners qrCodeMarkerCorners;
        public MarkerCorners arucoMarkerCorners;
    }

    [Serializable]
    internal struct MarkerCorners
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
        public Quaternion orientation;
    }

    [Serializable]
    internal class PVImageData
    {
        public PixelFormat pixelFormat;
        public CameraResolution resolution;
        public CameraIntrinsics intrinsics;
        public CameraExtrinsics extrinsics;
        public byte[] pngData;

        // Non-serialized items
        public byte[] pixelData { get; set; }
        public CameraFrame frame { get; set; }
    }

    [Serializable]
    internal struct HeadsetData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [Serializable]
    internal class HeadsetCalibrationDataRequest
    {
        public float timestamp;

        public byte[] Serialize()
        {
            var str = JsonUtility.ToJson(this);
            var payload = Encoding.ASCII.GetBytes(str);
            return payload;
        }

        public static bool TryDeserialize(byte[] payload, out HeadsetCalibrationDataRequest request)
        {
            request = null;

            try
            {
                var str = Encoding.ASCII.GetString(payload);
                request = JsonUtility.FromJson<HeadsetCalibrationDataRequest>(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
