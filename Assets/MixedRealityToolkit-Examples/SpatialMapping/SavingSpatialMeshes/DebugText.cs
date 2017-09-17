// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Examples
{
    [RequireComponent(typeof(TextMesh))]
    public class DebugText : MonoBehaviour
    {
        private TextMesh mText;

        private void Awake()
        {
            mText = GetComponent<TextMesh>();
        }

        public void SetText(string text)
        {
            mText.text = text;
        }
    }
}
