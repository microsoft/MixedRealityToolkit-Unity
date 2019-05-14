using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    public class CalculatedCameraCalibration
    {
        public CalculatedCameraExtrinsics Extrinsics;
        public CalculatedCameraIntrinsics Intrinsics;

        public byte[] Serialize()
        {
            var str = JsonUtility.ToJson(this);
            var payload = Encoding.UTF8.GetBytes(str);
            return payload;
        }

        public static bool TryDeserialize(byte[] payload, out CalculatedCameraCalibration calibrationData)
        {
            calibrationData = null;

            try
            {
                var str = Encoding.UTF8.GetString(payload);
                calibrationData = JsonUtility.FromJson<CalculatedCameraCalibration>(str);
                return calibrationData.Extrinsics != null &&
                       calibrationData.Intrinsics != null &&
                       calibrationData.Intrinsics.ImageWidth > 0 &&
                       calibrationData.Intrinsics.ImageHeight > 0;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception thrown deserializing camera intrinsics: {e.ToString()}");
                return false;
            }
        }
    }
}