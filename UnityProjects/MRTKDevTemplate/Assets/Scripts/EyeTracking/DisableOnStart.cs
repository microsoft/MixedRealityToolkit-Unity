// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Simple class that automatically hides a target on startup. This is, for example, useful for nested canvas object.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/DisableOnStart")]
    public class DisableOnStart : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
