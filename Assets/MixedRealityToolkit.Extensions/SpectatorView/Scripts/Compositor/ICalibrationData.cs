using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    public interface ICalibrationData
    {
        void SetUnityCameraExtrinstics(Transform cameraGO);
        void SetUnityCameraIntrinsics(Camera camera);
    }
}