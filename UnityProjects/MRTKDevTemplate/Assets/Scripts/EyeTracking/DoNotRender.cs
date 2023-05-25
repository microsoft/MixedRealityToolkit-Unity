// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Simple class that automatically hides a target on startup. This is, for example, useful for trigger zones and visual guides that are useful 
    /// to show in the Editor, but not in the final application.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/DoNotRender")]
    public class DoNotRender : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
