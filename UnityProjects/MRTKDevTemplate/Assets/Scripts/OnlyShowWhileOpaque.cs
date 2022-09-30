// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This script disables the target gameObject if the first detected XRDisplaySubsystem is
    /// not opaque. Useful for only showing environment models on opaque/VR devices.
    /// </summary>
    internal class OnlyShowWhileOpaque : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("GameObject to enable/disable based on XR display opacity. If null, defaults to this GameObject.")]
        private GameObject targetObject;

        private XRDisplaySubsystem displaySubsystem = null;

        private void Awake()
        {
            if (targetObject == null)
            {
                targetObject = gameObject;
            }
        }

        private void Update()
        {
            // TODO: possibly remove timeout once XRSubsystemHelpers are more efficient (#10852)
            if (displaySubsystem == null && Time.time < 5.0f)
            {
                displaySubsystem = XRSubsystemHelpers.DisplaySubsystem;
            }

            // If we've successfully found a displaySubsystem, but the current visibility
            // doesn't match the opacity of the XR display...
            if (displaySubsystem != null && targetObject.activeSelf != displaySubsystem.displayOpaque)
            {
                targetObject.SetActive(displaySubsystem.displayOpaque);
            }
        }
    }
}
