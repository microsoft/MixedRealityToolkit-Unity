// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class OpenKeyboard : MonoBehaviour
    {
        private MixedRealityKeyboard keyboard;
        public static string keyboardText = "";
        public TextMesh debugMessage;

        public void Start()
        {
            keyboard = gameObject.AddComponent<MixedRealityKeyboard>();
            keyboard.TextChanged.AddListener((eventData) => debugMessage.text = "typing... " + keyboard.Text);
            keyboard.KeyboardHidden.AddListener(() => debugMessage.text = "typed " + keyboard.Text);
        }

        public void Open()
        {
            keyboard.ShowKeyboard();
        }

        public void Close()
        {
            keyboard.HideKeyboard();
        }
    }
}