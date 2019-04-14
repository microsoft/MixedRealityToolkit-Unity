// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class OpenKeyboard : MonoBehaviour
    {
        // For System Keyboard
        public TouchScreenKeyboard keyboard;
        public static string keyboardText = "";
        public TextMesh debugMessage;

#if UNITY_WSA
        private void Update()
        {
            if (keyboard != null)
            {
                keyboardText = keyboard.text;
                if (TouchScreenKeyboard.visible)
                {
                    debugMessage.text = "typing... " + keyboardText;
                }
                else
                {
                    debugMessage.text = "typed " + keyboardText;
                    keyboard = null;
                }
            }
        }
#endif

        public void OpenSystemKeyboard()
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        }
    }
}