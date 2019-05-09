// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    public class CalculatedCameraIntrinsics : CameraIntrinsics
    {
        /// <summary>
        /// Reprojection error for the calculated intrinsics
        /// </summary>
        public float ReprojectionError;

        /// <inheritdoc />
        public CalculatedCameraIntrinsics(
            float reprojectionError,
            Vector2 focalLength,
            uint imageWidth,
            uint imageHeight,
            Vector2 principalPoint,
            Vector3 radialDistortion,
            Vector2 tangentialDistortion,
            Matrix4x4 undistortedProjectionTransform) :
            base(
                focalLength,
                imageWidth,
                imageHeight,
                principalPoint,
                radialDistortion,
                tangentialDistortion,
                undistortedProjectionTransform)
        {
            ReprojectionError = reprojectionError;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"reprojection error: {ReprojectionError.ToString("G4")} {base.ToString()}";
        }

        public byte[] Serialize()
        {
            var str = JsonUtility.ToJson(this);
            var payload = Encoding.UTF8.GetBytes(str);
            return payload;
        }

        public static bool TryDeserialize(byte[] payload, out CalculatedCameraIntrinsics intrinsics)
        {
            intrinsics = null;

            try
            {
                var str = Encoding.UTF8.GetString(payload);
                intrinsics = JsonUtility.FromJson<CalculatedCameraIntrinsics>(str);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception thrown deserializing camera intrinsics: {e.ToString()}");
                return false;
            }
        }
    }
}
