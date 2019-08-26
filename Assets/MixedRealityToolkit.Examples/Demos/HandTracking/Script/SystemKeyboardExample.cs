// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// An example script that delegates keyboard API access either to the WMR workaround
    /// (MixedRealityKeyboard) or Unity's TouchScreenKeyboard API depending on the platform.
    /// </summary>
    /// <remarks>
    /// Note that like Unity's TouchScreenKeyboard API, this script only supports WSA, iOS,
    /// and Android.
    /// </remarks>
    public class SystemKeyboardExample : MonoBehaviour
    {
#if WINDOWS_UWP
        private MixedRealityKeyboard wmrKeyboard;
#elif UNITY_IOS || UNITY_ANDROID
        private TouchScreenKeyboard touchscreenKeyboard;
#endif

        public static string KeyboardText = "";

        [SerializeField]
        private TextMeshPro debugMessage = null;

        private void Start()
        {
#if WINDOWS_UWP
            // Windows mixed reality keyboard initialization goes here
            wmrKeyboard = gameObject.AddComponent<MixedRealityKeyboard>();
#elif UNITY_IOS || UNITY_ANDROID
            // non-Windows mixed reality keyboard initialization goes here
#else
            debugMessage.text = "Keyboard not supported on this platform.";
#endif
        }

        public void OpenSystemKeyboard()
        {
#if WINDOWS_UWP
            wmrKeyboard.ShowKeyboard();
#elif UNITY_IOS || UNITY_ANDROID
            touchscreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
#endif
        }

        private void Update()
        {
#if WINDOWS_UWP
            // Windows mixed reality keyboard update goes here
            KeyboardText = wmrKeyboard.Text;
            if (wmrKeyboard.Visible)
            {
                if (debugMessage != null)
                {
                    debugMessage.text = "typing... " + KeyboardText;
                }
            }
            else
            {
                if (KeyboardText == null || KeyboardText.Length == 0)
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "open keyboard to type text";
                    }
                }
                else
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "typed " + KeyboardText;
                    }
                }
            }
#elif UNITY_IOS || UNITY_ANDROID
            // non-Windows mixed reality keyboard initialization goes here
            // for non-Windows mixed reality keyboards just use Unity's default
            // touchscreenkeyboard. 
            // We will use touchscreenkeyboard once Unity bug is fixed
            // Unity bug tracking the issue https://fogbugz.unity3d.com/default.asp?1137074_rttdnt8t1lccmtd3
            if (touchscreenKeyboard != null)
            {
                KeyboardText = touchscreenKeyboard.text;
                if (TouchScreenKeyboard.visible)
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "typing... " + KeyboardText;
                    }
                }
                else
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "typed " + KeyboardText;
                    }

                    touchscreenKeyboard = null;
                }
            }
#endif
        }
    }
}