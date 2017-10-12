// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// A theme for handling the label string on the toggle button
    /// </summary>
    public class LabelTheme : MonoBehaviour
    {
        [Tooltip("Default label string")]
        public string Default;
        [Tooltip("selected label string")]
        public string Selected;
    }
}
