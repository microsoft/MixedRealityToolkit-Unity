// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class DebugTextOutput : MonoBehaviour
    {
        [SerializeField]
        protected TextMesh textMesh = null;

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