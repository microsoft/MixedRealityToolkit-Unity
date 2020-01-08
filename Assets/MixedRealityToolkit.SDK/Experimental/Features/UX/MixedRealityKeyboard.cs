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
    [AddComponentMenu("Scripts/MRTK/SDK/MixedRealityKeyboard")]
    public class MixedRealityKeyboard : MonoBehaviour
    {
        /// <summary>
        /// Returns true if the keyboard is currently open.
        /// </summary>
        public bool Visible { get { return state == KeyboardState.Showing; } }

        /// <summary>
        /// Returns the committed text.
        /// </summary>
        public string Text
        {
            get;
            private set;
        } = string.Empty;

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
            get { return onShowKeyboard; }
            set { onShowKeyboard = value; }
        }

        [SerializeField, Tooltip("Event which triggers when commit action is invoked on the keyboard. (Usually the return key.)")]
        private UnityEvent onCommitText = new UnityEvent();

        /// <summary>
        /// Event which triggers when commit action is invoked on the keyboard. (Usually the return key.)
        /// </summary>
        public UnityEvent OnCommitText
        {
            get { return onCommitText; }
            set { onCommitText = value; }
        }

        [SerializeField, Tooltip("Event which triggers when the keyboard is hidden.")]
        private UnityEvent onHideKeyboard = new UnityEvent();

        /// <summary>
        /// Event which triggers when the keyboard is hidden.
        /// </summary>
        public UnityEvent OnHideKeyboard
        {
            get { return onHideKeyboard; }
            set { onHideKeyboard = value; }
        }

        private enum KeyboardState
        {
            Hiding,
            Hidden,
            Showing,
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
                }
            }
        }

        private KeyboardState state = KeyboardState.Hidden;
        private TouchScreenKeyboard keyboard;
#if !UNITY_EDITOR && UNITY_WSA
        private InputPane inputPane = null;
#endif //!UNITY_EDITOR && UNITY_WSA
        private bool multiLine = false;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Initializes the UWP input pane.
        /// </summary>
        protected virtual void Start()
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

        /// <summary>
        /// Updates the keyboard based on current keyboard state.
        /// </summary>
        protected virtual void Update()
        {
            switch (State)
            {
                case KeyboardState.Showing:
                    {
                        UpdateText();
                    }
                    break;

                case KeyboardState.Hiding:
                    {
                        if (onHideKeyboard != null)
                        {
                            onHideKeyboard.Invoke();
                        }

                        State = KeyboardState.Hidden;
                    }
                    break;

                case KeyboardState.Hidden:
                default:
                    break;
            }
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Opens the keyboard for user interaction.
        /// </summary>
        /// <param name="text">Initial text to populate the keyboard with.</param>
        /// <param name="multiLine">True, if the return key should signal a newline rather than a commit.</param>
        public virtual void ShowKeyboard(string text = "", bool multiLine = false)
        {
            this.Text = text;
            this.multiLine = multiLine;

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
                keyboard.text = Text;
#if !UNITY_EDITOR && UNITY_WSA
                UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryShow(), false);
#endif
            }
            else
            {
                keyboard = TouchScreenKeyboard.Open(Text, TouchScreenKeyboardType.Default, false, this.multiLine, false, false);
            }

            if (onShowKeyboard != null)
            {
                onShowKeyboard.Invoke();
            }
        }

        /// <summary>
        /// Closes the keyboard for user interaction.
        /// </summary>
        public virtual void HideKeyboard()
        {
            if (State != KeyboardState.Hidden)
            {
                State = KeyboardState.Hiding;
            }

#if !UNITY_EDITOR && UNITY_WSA
            UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryHide(), false);
#endif //!UNITY_EDITOR && UNITY_WSA
        }

        /// <summary>
        /// Removes the current text from the keyboard.
        /// </summary>
        public virtual void ClearKeyboardText()
        {
            Text = string.Empty;
            CaretIndex = 0;

            if (keyboard != null)
            {
                keyboard.text = Text;
            }
        }

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
                        if (onCommitText != null)
                        {
                            onCommitText.Invoke();
                        }

                        HideKeyboard();
                    }
                }
            }
        }

        private bool IsPreviewCaretAtEnd()
        {
            return CaretIndex == Text.Length;
        }

        private void MovePreviewCaretToEnd()
        {
            CaretIndex = Text.Length;
        }

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
    }
}
