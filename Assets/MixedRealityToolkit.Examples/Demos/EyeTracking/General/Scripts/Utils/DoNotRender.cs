// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Simple class that automatically hides a target on startup. This is, for example, useful for trigger zones and visual guides that are useful 
    /// to show in the Editor, but not in the final application.
    /// </summary>
    public class DoNotRender : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
