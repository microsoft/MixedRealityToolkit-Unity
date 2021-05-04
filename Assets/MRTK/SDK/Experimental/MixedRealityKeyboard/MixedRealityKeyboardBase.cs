// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;
#if WINDOWS_UWP
using Windows.Globalization;
using Windows.UI.ViewManagement;
using Microsoft.MixedReality.Toolkit.Utilities;
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
    /// <remarks>
    /// <para>If using Unity 2019 or 2020, make sure the version >= 2019.4.25 or 2020.3.2 to ensure the latest fixes for Unity keyboard bugs are present.</para>
    /// </remarks>
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

        [Experimental, SerializeField, Tooltip("Whether disable user's interaction with other UI elements while typing. Use this option to decrease the chance of keyboard getting accidentally closed.")]
        private bool disableUIInteractionWhenTyping = false;

        /// <summary>
        /// Whether disable user's interaction with other UI elements while typing.
        /// Use this option to decrease the chance of keyboard getting accidentally closed.
        /// </summary>
        public bool DisableUIInteractionWhenTyping
        {
            get => disableUIInteractionWhenTyping;
            set
            {
                if (value != disableUIInteractionWhenTyping && value == false && inputModule != null && inputModule.ProcessPaused)
                {
                    inputModule.ProcessPaused = false;
                }
                disableUIInteractionWhenTyping = value;
            }
        }

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

        private MixedRealityInputModule inputModule = null;

#if WINDOWS_UWP
        private InputPane inputPane = null;
        private TouchScreenKeyboard keyboard = null;

        private Coroutine stateUpdate;

        private string keyboardLanguage = string.Empty;
#endif

        #endregion Private fields

        #region MonoBehaviour Implementation

#if WINDOWS_UWP
        protected virtual void Awake()
        {
            inputModule = CameraCache.Main.GetComponent<MixedRealityInputModule>();
        }

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
            if (DisableUIInteractionWhenTyping && inputModule != null)
            {
                inputModule.ProcessPaused = false;
            }
        }

        private void OnInputPaneShowing(InputPane inputPane, InputPaneVisibilityEventArgs args)
        {
            OnKeyboardShowing();
            if (DisableUIInteractionWhenTyping && inputModule != null)
            {
                inputModule.ProcessPaused = true;
            }
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
#if UNITY_2019_3_OR_NEWER
            keyboard.selection = new RangeInt(Text.Length, 0);
#endif
            MovePreviewCaretToEnd();
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
#if UNITY_2019_3_OR_NEWER
                Text = keyboard.text;
                CaretIndex = keyboard.selection.end;
#else
                // Check the current language of the keyboard
                string newKeyboardLanguage = Language.CurrentInputMethodLanguageTag;
                if (newKeyboardLanguage != keyboardLanguage)
                {
                    keyboard.text = Text;
                    // For the languages requiring IME (Chinese, Japanese and Korean) move the caret to the end
                    // As we do not support editing in the middle of a string
                    if (IsIMERequired(newKeyboardLanguage))
                    {
                        MovePreviewCaretToEnd();
                    }
                }
                keyboardLanguage = newKeyboardLanguage;

                var characterDelta = keyboard.text.Length - Text.Length;
                // Handle character deletion.
                if (UnityEngine.Input.GetKey(KeyCode.Backspace) ||
                    UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
                {
                    // Handle languages requiring IME
                    if (Text.Length > keyboard.text.Length && IsIMERequired(keyboardLanguage))
                    {
                        Text = keyboard.text;
                        CaretIndex = Mathf.Clamp(CaretIndex + characterDelta, 0, Text.Length);
                    }
                    else if (CaretIndex > 0)
                    {
                        Text = Text.Remove(CaretIndex - 1, 1);
                        keyboard.text = Text;
                        --CaretIndex;
                    }
                }
                // Handle other character changes for languages requiring IME
                else if (IsIMERequired(keyboardLanguage))
                {
                    Text = keyboard.text;
                    MovePreviewCaretToEnd();
                }
                else
                {
                    // Add the new characters.

                    var caretWasAtEnd = IsPreviewCaretAtEnd();

                    if (characterDelta > 0)
                    {
                        var newCharacters = keyboard.text.Substring(Text.Length, characterDelta);
                        Text = Text.Insert(CaretIndex, newCharacters);
                        if (keyboard.text != Text)
                        {
                            keyboard.text = Text;
                        }

                        if (caretWasAtEnd)
                        {
                            MovePreviewCaretToEnd();
                        }
                        else
                        {
                            CaretIndex += newCharacters.Length;
                        }
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
                }

#endif
                // Handle commit via the return key.
                if (!multiLine)
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
                    {
                        onCommitText?.Invoke();

                        HideKeyboard();
                    }
                }

                SyncCaret();
            }
        }

        private bool IsPreviewCaretAtEnd() => CaretIndex == Text.Length;

        private void MovePreviewCaretToEnd() => CaretIndex = Text.Length;

        private void OnKeyboardHiding()
        {
            UnityEngine.WSA.Application.InvokeOnAppThread(() => onHideKeyboard?.Invoke(), false);
            state = KeyboardState.Hidden;
        }

        private void OnKeyboardShowing() { }

        private bool IsIMERequired(string language)
        {
            return language.StartsWith("zh") || language.StartsWith("ja") || language.StartsWith("ko");
        }
#endif
        protected virtual void SyncCaret() { }

    }
}
