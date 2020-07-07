// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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