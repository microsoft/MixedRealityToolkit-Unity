// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("Scripts/MRTK/Examples/DebugTextOutput")]
    public class DebugTextOutput : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshPro textMesh = null;

        public void SetTextWithTimestamp(string text)
        {
            // Do something on specified distance for fire event
            if (textMesh != null)
            {
                textMesh.text = $"{text} ({Time.unscaledTime.ToString()})";
            }
        }
    }
}