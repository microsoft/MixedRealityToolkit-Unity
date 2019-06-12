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
    /// Class that can launch and hide a system keyboard specifically for Windows Mixed Reality
    /// devices (HoloLens 2, Windows Mixed Reality).
    /// 
    /// Implements a workaround for UWP TouchScreenKeyboard bug which prevents
    /// UWP keyboard from showing up again after it is closed.
    /// Unity bug tracking the issue https://fogbugz.unity3d.com/default.asp?1137074_rttdnt8t1lccmtd3
    /// </summary>
    public class MixedRealityKeyboard : MonoBehaviour
    {
        [Experimental]
        #region Serialized fields
        [SerializeField]
        [Tooltip("Event raised when the text has changed")]
        public KeyboardTextEvent TextChanged = new KeyboardTextEvent();
        [Tooltip("Event raised when the keyboard is shown")]
        public UnityEvent KeyboardShown = new UnityEvent();
        [Tooltip("Event raised when the keyboard is hidden")]
        public UnityEvent KeyboardHidden = new UnityEvent();
        #endregion

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
                    TextChanged?.Invoke(new KeyboardTextData(oldValue, value, this));
                }
            }
        }
        
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
                    if (state == KeyboardState.showing)
                    {
                        KeyboardShown.Invoke();
                    }
                    else if (state == KeyboardState.hidden)
                    {
                        KeyboardHidden.Invoke();
                    }
                }
            }
        }
        #endregion properties

        #region Private fields
        private TouchScreenKeyboard keyboard;

#if !UNITY_EDITOR && UNITY_WSA
    private InputPane inputPane = null;
#endif //!UNITY_EDITOR && UNITY_WSA

        private KeyboardState state = KeyboardState.hidden;
        #endregion private fields

        #region Private enums
        private enum KeyboardState
        {
            hidden,
            showing,
            hiding
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
                case KeyboardState.showing:
                    CommitText();
                    break;

                case KeyboardState.hiding:
                    ClearText();
                    State = KeyboardState.hidden;
                    break;

                case KeyboardState.hidden:
                default:
                    break;
            }
        }
        #endregion unity functions

        public void HideKeyboard()
        {
            Debug.Log($"TryHide keyboard");

            ClearText();
            State = KeyboardState.hidden;

#if !UNITY_EDITOR && UNITY_WSA
        UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryHide(), false);
#endif //!UNITY_EDITOR && UNITY_WSA
        }

        public void ShowKeyboard()
        {
            if (state == KeyboardState.showing)
            {
                Debug.Log($"Already shown");
                return;
            }

            Debug.Log($"TryShow keyboard");
            State = KeyboardState.showing;

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
            Debug.Log($"Closing keyboard");
            if (State != KeyboardState.hidden)
            {
                State = KeyboardState.hiding;
            }
        }

        private void OnKeyboardShowing()
        {
            Debug.Log($"Showing keyboard");
        }
        #endregion Input pane event handlers
    } 
}