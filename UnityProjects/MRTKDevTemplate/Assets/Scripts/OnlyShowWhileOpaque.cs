// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This script disables the GameObject if the first detected XRDisplaySubsystem is
    /// not opaque. (If no XRDisplaySubsystem is found, the script assumes the platform's
    /// display is opaque.) Useful for only showing environment models on opaque/VR devices.
    /// </summary>
    internal class OnlyShowWhileOpaque : MonoBehaviour
    {
        private XRDisplaySubsystem displaySubsystem = null;
        
        private bool isVisible = false;

        private void Awake()
        {
            isVisible = gameObject.activeSelf;
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
            if (displaySubsystem != null && isVisible != displaySubsystem.displayOpaque)
            {
                isVisible = displaySubsystem.displayOpaque;
                gameObject.SetActive(isVisible);
            }
        }
    }
}
