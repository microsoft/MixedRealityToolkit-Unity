// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.UI.ViewManagement;
#endif 

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Class that can launch and hide a system keyboard specifically for HoloLens 2.
    /// 
    /// Implements a workaround for UWP TouchScreenKeyboard bug which prevents
    /// UWP keyboard from showing up again after it is closed.
    /// Unity bug tracking the issue https://fogbugz.unity3d.com/default.asp?1137074_rttdnt8t1lccmtd3
    /// </summary>
    public class MixedRealityKeyboard : MonoBehaviour
    {
        [Experimental]
        #region Properties
        private string text;
        public string Text
        {
            get
            {
                return text;
            }

            private set
            {
                if (text != value)
                {
                    var oldValue = text;
                    text = value;
                }
            }
        }

        public bool Visible { get { return state == KeyboardState.Showing; } }


        private KeyboardState State
        {
            get
            {
                return state;
            }

            set
            {
                if (state != value)
                {
                    state = value;
                }
            }
        }
        #endregion properties

        #region Private fields
        private TouchScreenKeyboard keyboard;

#if !UNITY_EDITOR && UNITY_WSA
        private InputPane inputPane = null;
#endif //!UNITY_EDITOR && UNITY_WSA

        private KeyboardState state = KeyboardState.Hidden;
        #endregion private fields

        #region Private enums
        private enum KeyboardState
        {
            Hiding,
            Hidden,
            Showing,
        }
        #endregion Private enums

        #region Unity functions
        private void Start()
        {
#if !UNITY_EDITOR && UNITY_WSA
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                inputPane = InputPane.GetForCurrentView();
                inputPane.Hiding += (inputPane, args) => OnKeyboardHiding();
                inputPane.Showing += (inputPane, args) => OnKeyboardShowing();
            }, false);
#endif //!UNITY_EDITOR && UNITY_WSA
        }

        private void Update()
        {
            switch (State)
            {
                case KeyboardState.Showing:
                    CommitText();
                    break;

                case KeyboardState.Hiding:
                    ClearText();
                    State = KeyboardState.Hidden;
                    break;

                case KeyboardState.Hidden:
                default:
                    break;
            }
        }
        #endregion unity functions

        public void HideKeyboard()
        {
            ClearText();
            State = KeyboardState.Hidden;

#if !UNITY_EDITOR && UNITY_WSA
            UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryHide(), false);
#endif //!UNITY_EDITOR && UNITY_WSA
        }

        public void ShowKeyboard()
        {
            // 2019/08/14: We show the keyboard even when the keyboard is already visible because on HoloLens 1
            // and WMR the events OnKeyboardShowing and OnKeyboardHiding do not fire
            //if (state == KeyboardState.Showing)
            //{
            //    Debug.Log($"MixedRealityKeyboard.ShowKeyboard called but keyboard already visible.");
            //    return;
            //}

            State = KeyboardState.Showing;

            if (keyboard != null)
            {
                keyboard.text = string.Empty;
#if !UNITY_EDITOR && UNITY_WSA
                UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryShow(), false);
#endif
            }
            else
            {
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            }
        }

        #region private functions
        private void CommitText()
        {
            if (keyboard != null)
            {
                Text = keyboard.text;
            }
        }

        private void ClearText()
        {
            if (keyboard != null)
            {
                keyboard.text = string.Empty;
            }
        }
        #endregion private functions

        #region Input pane event handlers 
        private void OnKeyboardHiding()
        {
            if (State != KeyboardState.Hidden)
            {
                State = KeyboardState.Hiding;
            }
        }

        private void OnKeyboardShowing()
        {
        }
        #endregion Input pane event handlers
    }
}
