// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

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
        /// <summary>
        /// TODO
        /// </summary>
        public bool Visible { get { return state == KeyboardState.Showing; } }

        /// <summary>
        /// TODO
        /// </summary>
        public string PreviewText
        {
            get;
            private set;
        } = string.Empty;

        /// <summary>
        /// TODO
        /// </summary>
        public int PreviewCaretIndex
        {
            get;
            private set;
        } = 0;

        /// <summary>
        /// TODO
        /// </summary>
        public string Text
        {
            get;
            private set;
        } = string.Empty;

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
                    {
                        UpdateText();
                    }
                    break;

                case KeyboardState.Hiding:
                    {
                        ClearText();
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
        /// TODO
        /// </summary>
        /// <param name="multiLine"></param>
        public void ShowKeyboard(bool multiLine = false)
        {
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
                keyboard.text = string.Empty;
#if !UNITY_EDITOR && UNITY_WSA
                UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryShow(), false);
#endif
            }
            else
            {
                keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, this.multiLine, false, false);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void HideKeyboard()
        {
            ClearText();
            State = KeyboardState.Hidden;

#if !UNITY_EDITOR && UNITY_WSA
            UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryHide(), false);
#endif //!UNITY_EDITOR && UNITY_WSA
        }

        private void UpdateText()
        {
            if (keyboard != null)
            {
                // Handle character deletion.
                if (UnityEngine.Input.GetKeyDown(KeyCode.Delete) || 
                    UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (PreviewCaretIndex > 0)
                    {
                        PreviewText = PreviewText.Remove(PreviewCaretIndex - 1, 1);
                        keyboard.text = PreviewText;
                        --PreviewCaretIndex;
                    }
                }

                // Add the new characters.
                var characterDelta = keyboard.text.Length - PreviewText.Length;
                var caretWasAtEnd = IsPreviewCaretAtEnd();

                if (characterDelta > 0)
                {
                    var newCharacters = keyboard.text.Substring(PreviewText.Length, characterDelta);
                    PreviewText = PreviewText.Insert(PreviewCaretIndex, newCharacters);
                    keyboard.text = PreviewText;

                    if (caretWasAtEnd)
                    {
                        MovePreviewCaretToEnd();
                    }
                    else
                    {
                        PreviewCaretIndex += newCharacters.Length;
                    }
                }
                else if (characterDelta < 0)
                {
                    Debug.LogWarning("MixedRealityKeyboard expected a longer or equal string but received a shorter one.");

                    // Take what is currently in the keyboard and move the caret to the end.
                    PreviewText = keyboard.text;
                    MovePreviewCaretToEnd();
                }

                // Handle the arrow keys.
                if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PreviewCaretIndex = Mathf.Clamp(PreviewCaretIndex - 1, 0, PreviewText.Length);
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
                {
                    PreviewCaretIndex = Mathf.Clamp(PreviewCaretIndex + 1, 0, PreviewText.Length);
                }

                // Handle commit via the return key.
                if (!multiLine)
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
                    {
                        CommitText();
                        HideKeyboard();
                    }
                }
            }
        }

        private void CommitText()
        {
            Text = PreviewText;
        }

        private void ClearText()
        {
            PreviewText = string.Empty;
            PreviewCaretIndex = 0;

            if (keyboard != null)
            {
                keyboard.text = PreviewText;
            }
        }

        private bool IsPreviewCaretAtEnd()
        {
            return PreviewCaretIndex == PreviewText.Length;
        }

        private void MovePreviewCaretToEnd()
        {
            PreviewCaretIndex = PreviewText.Length;
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
