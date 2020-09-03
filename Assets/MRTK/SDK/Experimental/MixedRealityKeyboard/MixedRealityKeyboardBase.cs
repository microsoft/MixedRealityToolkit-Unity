// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;
#if WINDOWS_UWP
using Windows.UI.ViewManagement;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
#endif

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Base class for objects that wish to launch and hide a system keyboard specifically for Windows Mixed Reality
    /// devices (HoloLens 2, Windows Mixed Reality).
    /// 
    /// Implements a workaround for UWP TouchScreenKeyboard bug which prevents
    /// UWP keyboard from showing up again after it is closed.
    /// Unity bug tracking the issue https://fogbugz.unity3d.com/default.asp?1137074_rttdnt8t1lccmtd3
    /// </summary>
    public abstract class MixedRealityKeyboardBase : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// Returns true if the keyboard is currently open.
        /// </summary>
        public bool Visible => state == KeyboardState.Showing;

        /// <summary>
        /// Returns the index of the caret within the text.
        /// </summary>
        public int CaretIndex
        {
            get;
            private set;
        } = 0;

        [Experimental, SerializeField, Tooltip("Event which triggers when the keyboard is shown.")]
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

#if WINDOWS_UWP
        private InputPane inputPane = null;

        private TouchScreenKeyboard keyboard = null;

        private Coroutine stateUpdate;
#endif

        private bool multiLine = false;

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
                inputPane.Showing += OnInputPaneShowing;
            }, false);
        }

        private void OnInputPaneHiding(InputPane inputPane, InputPaneVisibilityEventArgs args)
        {
            OnKeyboardHiding();
        }

        private void OnInputPaneShowing(InputPane inputPane, InputPaneVisibilityEventArgs args)
        {
            OnKeyboardShowing();
        }

        void OnDestroy()
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                inputPane = InputPane.GetForCurrentView();
                inputPane.Hiding -= OnInputPaneHiding;
                inputPane.Showing -= OnInputPaneShowing;
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

                    case KeyboardState.Hiding:
                        {
                            onHideKeyboard?.Invoke();
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

        public abstract string Text { get; protected set; }

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

            // 2019/08/14: We show the keyboard even when the keyboard is already visible because on HoloLens 1
            // and WMR the events OnKeyboardShowing and OnKeyboardHiding do not fire
            // if (state == KeyboardState.Showing)
            // {
            //     Debug.Log($"MixedRealityKeyboard.ShowKeyboard called but keyboard already visible.");
            //     return;
            // }

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
                // Handle character deletion.
                if (UnityEngine.Input.GetKeyDown(KeyCode.Delete) || 
                    UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (CaretIndex > 0)
                    {
                        Text = Text.Remove(CaretIndex - 1, 1);
                        keyboard.text = Text;
                        --CaretIndex;
                    }
                }

                // Add the new characters.
                var characterDelta = keyboard.text.Length - Text.Length;
                var caretWasAtEnd = IsPreviewCaretAtEnd();

                if (characterDelta > 0)
                {
                    var newCharacters = keyboard.text.Substring(Text.Length, characterDelta);
                    Text = Text.Insert(CaretIndex, newCharacters);
                    keyboard.text = Text;

                    if (caretWasAtEnd)
                    {
                        MovePreviewCaretToEnd();
                    }
                    else
                    {
                        CaretIndex += newCharacters.Length;
                    }
                }
                else if (characterDelta < 0)
                {
                    // Take what is currently in the keyboard and move the caret to the end.
                    Text = keyboard.text;
                    MovePreviewCaretToEnd();
                }

                // Handle the arrow keys.
                if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || 
                    UnityEngine.Input.GetKey(KeyCode.LeftArrow))
                {
                    CaretIndex = Mathf.Clamp(CaretIndex - 1, 0, Text.Length);
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || 
                    UnityEngine.Input.GetKey(KeyCode.RightArrow))
                {
                    CaretIndex = Mathf.Clamp(CaretIndex + 1, 0, Text.Length);
                }

                // Handle commit via the return key.
                if (!multiLine)
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
                    {
                        onCommitText?.Invoke();

                        HideKeyboard();
                    }
                }
            }
        }

        private bool IsPreviewCaretAtEnd() => CaretIndex == Text.Length;

        private void MovePreviewCaretToEnd() => CaretIndex = Text.Length;

        private void OnKeyboardHiding()
        {
            if (state != KeyboardState.Hidden)
            {
                state = KeyboardState.Hiding;
            }
        }

        private void OnKeyboardShowing() { }
#endif
    }
}
