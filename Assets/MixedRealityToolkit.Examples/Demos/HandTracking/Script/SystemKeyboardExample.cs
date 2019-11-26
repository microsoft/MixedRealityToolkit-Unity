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
    [AddComponentMenu("Scripts/MRTK/Examples/SystemKeyboardExample")]
    public class SystemKeyboardExample : MonoBehaviour
    {
#if WINDOWS_UWP
        private MixedRealityKeyboard wmrKeyboard;
#elif UNITY_IOS || UNITY_ANDROID
        private TouchScreenKeyboard touchscreenKeyboard;
#endif

        [SerializeField]
        private TextMeshPro debugMessage = null;

#pragma warning disable 0414
        [SerializeField]
        private MixedRealityKeyboardPreview mixedRealityKeyboardPreview = null;
#pragma warning restore 0414

        /// <summary>
        /// Opens a platform specific keyboard.
        /// </summary>
        public void OpenSystemKeyboard()
        {
#if WINDOWS_UWP
            wmrKeyboard.ShowKeyboard(wmrKeyboard.Text, false);
#elif UNITY_IOS || UNITY_ANDROID
            touchscreenKeyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false);
#endif
        }

        #region MonoBehaviour Implementation

        private void Start()
        {
            // Initially hide the preview.
            if (mixedRealityKeyboardPreview != null)
            {
                mixedRealityKeyboardPreview.gameObject.SetActive(false);
            }

#if WINDOWS_UWP
            // Windows mixed reality keyboard initialization goes here
            wmrKeyboard = gameObject.AddComponent<MixedRealityKeyboard>();

            if (wmrKeyboard.OnShowKeyboard != null)
            {
                wmrKeyboard.OnShowKeyboard.AddListener(() =>
                {
                    if (mixedRealityKeyboardPreview != null)
                    {
                        mixedRealityKeyboardPreview.gameObject.SetActive(true);
                    }
                });
            }

            if (wmrKeyboard.OnHideKeyboard != null)
            {
                wmrKeyboard.OnHideKeyboard.AddListener(() =>
                {
                    if (mixedRealityKeyboardPreview != null)
                    {
                        mixedRealityKeyboardPreview.gameObject.SetActive(false);
                    }
                });
            }
#elif UNITY_IOS || UNITY_ANDROID
            // non-Windows mixed reality keyboard initialization goes here
#else
            debugMessage.text = "Keyboard not supported on this platform.";
#endif
        }

        private void Update()
        {
#if WINDOWS_UWP
            // Windows mixed reality keyboard update goes here
            if (wmrKeyboard.Visible)
            {
                if (debugMessage != null)
                {
                    debugMessage.text = "Typing: " + wmrKeyboard.Text;
                }

                if (mixedRealityKeyboardPreview != null)
                {
                    mixedRealityKeyboardPreview.Text = wmrKeyboard.Text;
                    mixedRealityKeyboardPreview.CaretIndex = wmrKeyboard.CaretIndex;
                }
            }
            else
            {
                var keyboardText = wmrKeyboard.Text;

                if (string.IsNullOrEmpty(keyboardText))
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "Open keyboard to type text.";
                    }
                }
                else
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "Typed: " + keyboardText;
                    }
                }

                if (mixedRealityKeyboardPreview != null)
                {
                    mixedRealityKeyboardPreview.Text = string.Empty;
                    mixedRealityKeyboardPreview.CaretIndex = 0;
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
                string KeyboardText = touchscreenKeyboard.text;
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

        #endregion MonoBehaviour Implementation
    }
}