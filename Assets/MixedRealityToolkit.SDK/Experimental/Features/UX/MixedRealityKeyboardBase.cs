// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;
#if WINDOWS_UWP
using Windows.UI.ViewManagement;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
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
    public abstract class MixedRealityKeyboardBase<T> : MonoBehaviour where T : Selectable
    {
#if WINDOWS_UWP
        protected T inputField;

        #region Properties

        public bool Visible => State == KeyboardState.Showing;

        #endregion properties

        #region Private fields

        private InputPane inputPane = null;
        private TouchScreenKeyboard keyboard = null;

        private KeyboardState State = KeyboardState.Hidden;

        private Coroutine stateUpdate;

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

        private void Awake()
        {
            if ((inputField = GetComponent<T>()) == null)
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                inputPane = InputPane.GetForCurrentView();
                inputPane.Hiding += (inputPane, args) => OnKeyboardHiding();
                inputPane.Showing += (inputPane, args) => OnKeyboardShowing();
            }, false);
        }

        private IEnumerator UpdateState()
        {
            switch (State)
            {
                case KeyboardState.Showing:
                    UpdateText(keyboard?.text);
                    break;

                case KeyboardState.Hiding:
                    ClearText();
                    State = KeyboardState.Hidden;
                    break;
            }

            yield return null;
        }

        private void OnDisable()
        {
            HideKeyboard();
        }

        #endregion unity functions

        protected void HideKeyboard()
        {
            ClearText();
            State = KeyboardState.Hidden;
            UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryHide(), false);

            if (stateUpdate != null)
            {
                StopCoroutine(stateUpdate);
                stateUpdate = null;
            }
        }

        protected void ShowKeyboard()
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
                UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryShow(), false);
            }
            else
            {
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            }

            if (stateUpdate == null)
            {
                stateUpdate = StartCoroutine(UpdateState());
            }
        }

        #region Input pane event handlers

        private void OnKeyboardHiding()
        {
            if (State != KeyboardState.Hidden)
            {
                State = KeyboardState.Hiding;
            }
        }

        private void OnKeyboardShowing() { }

        #endregion Input pane event handlers

        protected abstract void UpdateText(string text);

        protected abstract void ClearText();

#endif
    }
}