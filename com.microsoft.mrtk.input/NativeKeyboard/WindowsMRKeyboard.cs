// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;
#if WINDOWS_UWP
using UnityEngine.InputSystem;
using Windows.UI.ViewManagement;
using System.Collections;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Class allowing for launching and hiding a system keyboard specifically for Windows Mixed Reality
    /// devices (HoloLens 2, Windows Mixed Reality).
    /// </summary>
    [AddComponentMenu("MRTK/Input/Windows MR Keyboard")]
    public class WindowsMRKeyboard : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// Returns true if the keyboard is currently open.
        /// </summary>
        public bool Visible => state == KeyboardState.Showing;

        /// <summary>
        /// Returns the index of the caret within the text.
        /// </summary>
        public int CaretIndex { get; private set; } = 0;

        [SerializeField, Tooltip("Event which triggers when the keyboard is shown.")]
        private UnityEvent onShowKeyboard = new UnityEvent();

        /// <summary>
        /// Event which triggers when the keyboard is shown.
        /// </summary>
        public UnityEvent OnShowKeyboard
        {
            get => onShowKeyboard;
            set => onShowKeyboard = value;
        }

        [SerializeField, Tooltip("Event which triggers when commit action is invoked on the keyboard. (Usually the return key.)")]
        private UnityEvent onCommitText = new UnityEvent();

        /// <summary>
        /// Event which triggers when commit action is invoked on the keyboard. (Usually the return key.)
        /// </summary>
        public UnityEvent OnCommitText
        {
            get => onCommitText;
            set => onCommitText = value;
        }

        [SerializeField, Tooltip("Event which triggers when the keyboard is hidden.")]
        private UnityEvent onHideKeyboard = new UnityEvent();

        /// <summary>
        /// Event which triggers when the keyboard is hidden.
        /// </summary>
        public UnityEvent OnHideKeyboard
        {
            get => onHideKeyboard;
            set => onHideKeyboard = value;
        }

        #endregion properties

        #region Private enums

        private enum KeyboardState
        {
            Hiding,
            Hidden,
            Showing,
        }

        #endregion Private enums

        #region Private fields

        private KeyboardState state = KeyboardState.Hidden;

        private bool multiLine = false;

#if WINDOWS_UWP
        private InputPane inputPane = null;
        private TouchScreenKeyboard keyboard = null;

        private Coroutine stateUpdate;

        private string keyboardLanguage = string.Empty;
#endif

        #endregion Private fields

        #region MonoBehaviour Implementation

#if WINDOWS_UWP

        /// <summary>
        /// Initializes the UWP input pane.
        /// </summary>
        protected virtual void Start()
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                inputPane = InputPane.GetForCurrentView();
                inputPane.Hiding += OnInputPaneHiding;
            }, false);
        }

        private void OnInputPaneHiding(InputPane inputPane, InputPaneVisibilityEventArgs args)
        {
            OnKeyboardHiding();
        }

        void OnDestroy()
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                inputPane = InputPane.GetForCurrentView();
                inputPane.Hiding -= OnInputPaneHiding;
            }, false);
        }

        private IEnumerator UpdateState()
        {
            while (true)
            {
                switch (state)
                {
                    case KeyboardState.Showing:
                        {
                            UpdateText();
                        }
                        break;
                }

                yield return null;
            }
        }
#endif // WINDOWS_UWP

        private void OnDisable()
        {
            HideKeyboard();
        }

        #endregion MonoBehaviour Implementation

        public string Text { get; protected set; } = string.Empty;

        /// <summary>
        /// Closes the keyboard for user interaction.
        /// </summary>
        public void HideKeyboard()
        {
            if (state != KeyboardState.Hidden)
            {
                state = KeyboardState.Hidden;
            }

#if WINDOWS_UWP
            UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryHide(), false);

            if (stateUpdate != null)
            {
                StopCoroutine(stateUpdate);
                stateUpdate = null;
            }
#endif
        }

        /// <summary>
        /// Opens the keyboard for user interaction.
        /// </summary>
        /// <param name="text">Initial text to populate the keyboard with.</param>
        /// <param name="multiLine">True, if the return key should signal a newline rather than a commit.</param>
        public virtual void ShowKeyboard(string text = "", bool multiLine = false)
        {
            Text = text;
            this.multiLine = multiLine;

            state = KeyboardState.Showing;

#if WINDOWS_UWP
            if (keyboard != null)
            {
                keyboard.text = Text;
                UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryShow(), false);
            }
            else
            {
                keyboard = TouchScreenKeyboard.Open(Text, TouchScreenKeyboardType.Default, false, this.multiLine, false, false);
            }

            onShowKeyboard?.Invoke();
            keyboard.selection = new RangeInt(Text.Length, 0);
            CaretIndex = Text.Length;
            if (stateUpdate == null)
            {
                stateUpdate = StartCoroutine(UpdateState());
            }
#endif
        }

        /// <summary>
        /// Removes the current text from the keyboard.
        /// </summary>
        public virtual void ClearKeyboardText()
        {
            Text = string.Empty;
            CaretIndex = 0;
#if WINDOWS_UWP
            if (keyboard != null)
            {
                keyboard.text = string.Empty;
            }
#endif
        }

#if WINDOWS_UWP
        private void UpdateText()
        {
            if (keyboard != null)
            {
                Text = keyboard.text;
                CaretIndex = keyboard.selection.end;
                // Handle commit via the return key.
                if (!multiLine)
                {
                    if (Keyboard.current[Key.Enter].wasPressedThisFrame)
                    {
                        onCommitText?.Invoke();

                        HideKeyboard();
                    }
                }
            }
        }

        private void OnKeyboardHiding()
        {
            UnityEngine.WSA.Application.InvokeOnAppThread(() => onHideKeyboard?.Invoke(), false);
            state = KeyboardState.Hidden;
        }
#endif

    }
}
