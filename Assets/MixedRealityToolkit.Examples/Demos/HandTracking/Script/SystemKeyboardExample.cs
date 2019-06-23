// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class SystemKeyboardExample : MonoBehaviour
    {
        private MixedRealityKeyboard wmrKeyboard;
        private TouchScreenKeyboard touchscreenKeyboard;
        public static string keyboardText = "";
        public TextMesh debugMessage;

        private void Start()
        {
#if !UNITY_EDITOR && UNITY_WSA
            // Windows mixed reality keyboard initialization goes here
            wmrKeyboard = gameObject.AddComponent<MixedRealityKeyboard>();
#else
            // non-Windows mixed reality keyboard initialization goes here
#endif
        }

        public void OpenSystemKeyboard()
        {
#if !UNITY_EDITOR && UNITY_WSA
            wmrKeyboard.ShowKeyboard();
#else
            touchscreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
#endif
        }

        private void Update()
        {
#if !UNITY_EDITOR && UNITY_WSA
            // Windows mixed reality keyboard update goes here
            keyboardText = wmrKeyboard.Text;
            if (wmrKeyboard.Visible)
            {
                debugMessage.text = "typing... " + keyboardText;
            }
            else
            {
                if (keyboardText == null || keyboardText.Length == 0)
                {
                    debugMessage.text = "open keyboard to type text";
                }
                else
                {
                    debugMessage.text = "typed " + keyboardText;
                }
            }
#else
            // non-Windows mixed reality keyboard initialization goes here
            // for non-Windows mixed reality keyboards just use Unity's default
            // touchscreenkeyboard. 
            // We will use touchscreenkeyboard once Unity bug is fixed
            // Unity bug tracking the issue https://fogbugz.unity3d.com/default.asp?1137074_rttdnt8t1lccmtd3
            if (touchscreenKeyboard != null)
            {
                keyboardText = touchscreenKeyboard.text;
                if (TouchScreenKeyboard.visible)
                {
                    debugMessage.text = "typing... " + keyboardText;
                }
                else
                {
                    debugMessage.text = "typed " + keyboardText;
                    touchscreenKeyboard = null;
                }
            }
#endif
        }
    }
}