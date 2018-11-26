// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    [ExecuteInEditMode]
    public class ButtonLocalizedText : MonoBehaviour
    {
        public TextMesh TextMesh;

        public string Text
        {
            get
            {
                return TextMesh.text;
            }
            set
            {
                TextMesh.text = value;
            }
        }
    }
}