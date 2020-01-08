// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script continuously updates the orientation of the associated game object to keep facing the camera/user.
    /// </summary>
    [System.Obsolete("This component is no longer supported", true)]
    [AddComponentMenu("Scripts/MRTK/Obsolete/KeepFacingCamera")]
    public class KeepFacingCamera : MonoBehaviour
    {
        private Vector3 origForwardVector;

        private void Awake()
        {
            Debug.LogError(this.GetType().Name + " is deprecated");
        }

        private void Start()
        {
            // Let's figure out the original orientation of the target to keep it in the same orientation with respect to the camera when moving
            origForwardVector = Quaternion.FromToRotation(Vector3.forward, transform.rotation.eulerAngles).eulerAngles.normalized;
        }

        private void Update()
        {
            Vector3 target2CamDir = transform.position - CameraCache.Main.transform.position;
            transform.rotation = Quaternion.FromToRotation(origForwardVector, target2CamDir);
        }
    }
}
