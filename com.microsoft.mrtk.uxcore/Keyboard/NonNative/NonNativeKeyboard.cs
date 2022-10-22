// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Microsoft.MixedReality.Toolkit.UX.NonNativeFunctionKey;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple general use keyboard that is ideal for AR/VR applications that do not provide a native keyboard.
    /// </summary>
    /// 
    /// NOTE: This keyboard will not automatically appear when you select an InputField in your
    ///       Canvas. In order for the keyboard to appear you must call Keyboard.Instance.PresentKeyboard(string).
    ///       To retrieve the input from the Keyboard, subscribe to the textEntered event. Note that
    ///       tapping 'Close' on the Keyboard will not fire the textEntered event. You must tap 'Enter' to
    ///       get the textEntered event.
    public class NonNativeKeyboard : MonoBehaviour
    {
        public static NonNativeKeyboard Instance { get; private set; }

        /// <summary>
        /// Layout type enum for the type of keyboard layout to use.  
        /// This is used when spawning to enable the correct keys based on layout type.
        /// </summary>
        public enum LayoutType
        {
            Alpha,
            Symbol,
            URL,
            Email,
        }

        #region Callbacks

        /// <summary>
        /// Sent when the 'Enter' button is pressed.
        /// </summary>
        public UnityEvent<string> OnTextSubmit = null;

        /// <summary>
        /// Fired every time the text in the InputField changes.
        /// </summary>
        public UnityEvent<string> OnTextUpdate = null;

        /// <summary>
        /// Fired every time the close button is pressed.
        /// (Cleared when keyboard is closed.)
        /// </summary>
        public UnityEvent OnClose = null;

        /// <summary>
        /// Sent when the keyboard is placed.  This allows listener to know when someone else is co-opting the keyboard.
        /// </summary>
        public UnityEvent OnShow = null;

        #endregion Callbacks

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnTextUpdate?.Invoke(text);
            }
        }

        private string text;

        /// <summary>
        /// Bool to flag submitting on enter
        /// </summary>
        [field: SerializeField, Tooltip("The type of this button.")]
        public bool SubmitOnEnter { get; set; }

        /// <summary>
        /// The panel that contains the alpha keys.
        /// </summary>
        [SerializeField]
        private GameObject alphaKeysSection = null;

        /// <summary>
        /// The panel that contains the number and symbol keys.
        /// </summary>
        [SerializeField]
        private GameObject symbolKeysSection = null;

        /// <summary>
        /// References abc bottom panel.
        /// </summary>
        [SerializeField]
        private GameObject defaultBottomKeysSection = null;

        /// <summary>
        /// References .com bottom panel.
        /// </summary>
        [SerializeField]
        private GameObject urlBottomKeysSection = null;

        /// <summary>
        /// References @ bottom panel.
        /// </summary>
        [SerializeField]
        private GameObject emailBottomKeysSection = null;

        private LayoutType lastKeyboardLayout = LayoutType.Alpha;

        /// <summary>
        /// Make the keyboard disappear automatically after a timeout
        /// </summary>
        [field: SerializeField, Tooltip("The type of this button.")]
        public bool CloseOnInactivity { get; set; }

        /// <summary>
        /// Inactivity time that makes the keyboard disappear automatically.
        /// </summary>
        [field: SerializeField, Tooltip("The type of this button.")]
        public float CloseOnInactivityTime { get; set; } = 15;

        /// <summary>
        /// Time on which the keyboard should close on inactivity
        /// </summary>
        private float timeToClose;

        /// <summary>
        /// Event fired when shift key on keyboard is pressed.
        /// </summary>
        public UnityEvent<bool> OnKeyboardShifted = null;

        /// <summary>
        /// Event fired when char key on keyboard is pressed.
        /// </summary>
        public UnityEvent<NonNativeKey> OnKeyPressed = null;

        /// <summary>
        /// Accessor reporting shift state of keyboard.
        /// </summary>
        public bool IsShifted { get; private set; }

        /// <summary>
        /// Accessor reporting caps lock state of keyboard.
        /// </summary>
        public bool IsCapsLocked { get; private set; }

        /// <summary>
        /// User can add an audio source to the keyboard to have a click be heard on tapping a key 
        /// </summary>
        private AudioSource AudioSource
        {
            get
            {
                if (audioSource != null)
                {
                    return audioSource;
                }
                return audioSource = GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// User can add an audio source to the keyboard to have a click be heard on tapping a key 
        /// </summary>
        [SerializeField]
        private AudioSource audioSource;

        /// <summary>
        /// Deactivate on Awake.
        /// </summary>
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("There should only be one NonNativeKeyboard in a scene. Destroying a duplicate instance.");
                Destroy(gameObject);
            }
            Instance = this;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Makes sure the input field is always selected while the keyboard is up.
        /// </summary>
        private void LateUpdate()
        {
            CheckForCloseOnInactivityTimeExpired();
        }

        /// <summary>
        /// Called whenever the keyboard is disabled or deactivated.
        /// </summary>
        protected void OnDisable()
        {
            lastKeyboardLayout = LayoutType.Alpha;
            Clear();
        }

        /// <summary>
        /// Called whenever the keyboard is disabled or deactivated.
        /// </summary>
        protected void OnEnable()
        {
            PresentKeyboard();
        }

        /// <summary>
        /// Destroy unmanaged memory links.
        /// </summary>
        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #region Present Functions

        /// <summary>
        /// Present the default keyboard to the camera.
        /// </summary>
        public void PresentKeyboard()
        {
            ResetClosingTime();
            gameObject.SetActive(true);
            ActivateSpecificKeyboard(LayoutType.Alpha);

            OnShow.Invoke();
        }


        /// <summary>
        /// Presents the default keyboard to the camera, with start text.
        /// </summary>
        /// <param name="startText">The initial text to show in the keyboard's input field.</param>
        public void PresentKeyboard(string startText)
        {
            PresentKeyboard();
            Clear();
            Text = startText;
        }

        /// <summary>
        /// Presents a specific keyboard to the camera.
        /// </summary>
        /// <param name="keyboardType">Specify the keyboard type.</param>
        public void PresentKeyboard(LayoutType keyboardType)
        {
            PresentKeyboard();
            ActivateSpecificKeyboard(keyboardType);
        }

        /// <summary>
        /// Presents a specific keyboard to the camera, with start text.
        /// </summary>
        /// <param name="startText">The initial text to show in the keyboard's input field.</param>
        /// <param name="keyboardType">Specify the keyboard type.</param>
        public void PresentKeyboard(string startText, LayoutType keyboardType)
        {
            PresentKeyboard(startText);
            ActivateSpecificKeyboard(keyboardType);
        }

        #endregion Present Functions

        /// <summary>
        /// Activates a specific keyboard layout, and any sub keys.
        /// </summary>
        /// <param name="keyboardType">The keyboard layout type that should be activated</param>
        private void ActivateSpecificKeyboard(LayoutType keyboardType)
        {
            DisableAllKeyboards();
            ResetKeyboardState();
            lastKeyboardLayout = keyboardType;

            switch (keyboardType)
            {
                case LayoutType.URL:
                {
                    ShowAlphaKeyboard();
                    ShowAlphaKeyboardURLBottomKeysSection();
                    break;
                }

                case LayoutType.Email:
                {
                    ShowAlphaKeyboard();
                    ShowAlphaKeyboardEmailBottomKeysSection();
                    break;
                }

                case LayoutType.Symbol:
                {
                    ShowSymbolKeyboard();
                    break;
                }

                case LayoutType.Alpha:
                default:
                {
                    ShowAlphaKeyboard();
                    ShowAlphaKeyboardDefaultBottomKeysSection();
                    break;
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        #region Keyboard Functions

        /// <summary>
        /// Primary method for typing individual characters to a text field.
        /// </summary>
        public void ProcessValueKeyPress(NonNativeValueKey valueKey)
        {
            IndicateActivity();
            OnKeyPressed.Invoke(valueKey);

            if (!IsCapsLocked)
            {
                Shift(false);
            }

            Text += valueKey.CurrentValue;
            OnTextUpdate.Invoke(Text);
        }

        /// <summary>
        /// Trigger specific keyboard functionality.
        /// </summary>
        public void ProcessFunctionKeyPress(NonNativeFunctionKey functionKey)
        {
            IndicateActivity();
            OnKeyPressed.Invoke(functionKey);
            switch (functionKey.ButtonFunction)
            {
                case Function.Enter:
                {
                    Enter();
                    break;
                }

                case Function.Tab:
                {
                    Tab();
                    break;
                }

                case Function.ABC:
                {
                    ActivateSpecificKeyboard(lastKeyboardLayout);
                    break;
                }

                case Function.Symbol:
                {
                    ActivateSpecificKeyboard(LayoutType.Symbol);
                    break;
                }

                case Function.Close:
                {
                    Close();
                    break;
                }

                case Function.Shift:
                {
                    Shift(!IsShifted);
                    break;
                }

                case Function.CapsLock:
                {
                    CapsLock(!IsCapsLocked);
                    break;
                }

                case Function.Space:
                {
                    Space();
                    break;
                }

                case Function.Backspace:
                {
                    Backspace();
                    break;
                }

                case Function.UNDEFINED:
                {
                    Debug.LogErrorFormat("The {0} key on this keyboard hasn't been assigned a function.", functionKey.name);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Delete the character before the caret.
        /// </summary>
        public void Backspace()
        {
            if (Text.Length > 0)
            {
                Text = Text.Substring(0, Text.Length - 1);
            }
        }

        /// <summary>
        /// Fire the text entered event for objects listening to keyboard.
        /// Immediately closes keyboard.
        /// </summary>
        public void Enter()
        {
            if (SubmitOnEnter)
            {
                // Send text entered event and close the keyboard
                OnTextSubmit.Invoke(Text);

                Close();
            }
            else
            {
                Text += "\n";
            }

        }

        /// <summary>
        /// Set the keyboard to a single action shift state.
        /// </summary>
        /// <param name="newShiftState">value the shift key should have after calling the method</param>
        public void Shift(bool newShiftState)
        {
            IsShifted = newShiftState;
            OnKeyboardShifted.Invoke(IsShifted);

            if (IsCapsLocked && !newShiftState)
            {
                IsCapsLocked = false;
            }
        }

        /// <summary>
        /// Set the keyboard to a permanent shift state.
        /// </summary>
        /// <param name="newCapsLockState">Caps lock state the method is switching to</param>
        public void CapsLock(bool newCapsLockState)
        {
            IsCapsLocked = newCapsLockState;
            Shift(newCapsLockState);
        }

        /// <summary>
        /// Insert a space character.
        /// </summary>
        public void Space()
        {
            Text += " ";
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void Tab()
        {
            Text += "\t";
        }

        /// <summary>
        /// Close the keyboard.
        /// (Clears all event subscriptions.)
        /// </summary>
        public void Close()
        {
            OnClose.Invoke();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Clear the text input field.
        /// </summary>
        public void Clear()
        {
            ResetKeyboardState();
            Text = "";
        }

        #endregion

        #region Keyboard Layout Modes

        /// <summary>
        /// Show the alpha keyboard.
        /// </summary>
        public void ShowAlphaKeyboard()
        {
            alphaKeysSection.SetActive(true);
        }

        /// <summary>
        /// Show the default subkeys on the Alphanumeric keyboard.
        /// </summary>
        private void ShowAlphaKeyboardDefaultBottomKeysSection()
        {
            if (!defaultBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                defaultBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            defaultBottomKeysSection.SetActive(true);
        }

        /// <summary>
        /// Show the email subkeys on the Alphanumeric keyboard.
        /// </summary>
        private void ShowAlphaKeyboardEmailBottomKeysSection()
        {
            if (!emailBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                emailBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            emailBottomKeysSection.SetActive(true);
        }

        /// <summary>
        /// Show the URL subkeys on the Alphanumeric keyboard.
        /// </summary>
        private void ShowAlphaKeyboardURLBottomKeysSection()
        {
            if (!urlBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                urlBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            urlBottomKeysSection.SetActive(true);
        }

        /// <summary>
        /// Show the symbol keyboard.
        /// </summary>
        public void ShowSymbolKeyboard()
        {
            symbolKeysSection.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disable GameObjects for all keyboard elements.
        /// </summary>
        private void DisableAllKeyboards()
        {
            alphaKeysSection.SetActive(false);
            symbolKeysSection.gameObject.SetActive(false);

            urlBottomKeysSection.gameObject.SetActive(false);
            emailBottomKeysSection.gameObject.SetActive(false);
            defaultBottomKeysSection.gameObject.SetActive(false);
        }

        /// <summary>
        /// Reset temporary states of keyboard.
        /// </summary>
        private void ResetKeyboardState()
        {
            CapsLock(false);
        }

        #endregion Keyboard Layout Modes

        /// <summary>
        /// Respond to keyboard activity: reset timeout timer, play sound
        /// </summary>
        private void IndicateActivity()
        {
            ResetClosingTime();
            if (AudioSource != null)
            {
                AudioSource.Play();
            }
        }

        /// <summary>
        /// Reset inactivity closing timer
        /// </summary>
        private void ResetClosingTime()
        {
            if (CloseOnInactivity)
            {
                timeToClose = Time.time + CloseOnInactivityTime;
            }
        }

        /// <summary>
        /// Check if the keyboard has been left alone for too long and close
        /// </summary>
        private void CheckForCloseOnInactivityTimeExpired()
        {
            if (Time.time > timeToClose && CloseOnInactivity)
            {
                Close();
            }
        }
    }
}
