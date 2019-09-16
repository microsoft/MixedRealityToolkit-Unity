// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using TMPro;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Simple click handler to display button clicks from a <see cref="Microsoft.MixedReality.Toolkit.Experimental.Utilities.ScrollingObjectCollection"/>
    /// </summary>
    public class ScrollButtonClick : MonoBehaviour
    {
        public TextMeshPro clickTextbox;
        public void DisplayClick(GameObject sender)
        {
            clickTextbox.text = "Click happened with" + sender;
        }


    }
}
