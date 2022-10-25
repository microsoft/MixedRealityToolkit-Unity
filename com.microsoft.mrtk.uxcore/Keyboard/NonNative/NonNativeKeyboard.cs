// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Microsoft.MixedReality.Toolkit.UX.NonNativeFunctionKey;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple general use keyboard that provides an alternative to the native keyboard offered by each device.
    /// </summary>
    ///
    ///  <remarks>
    /// NOTE: This keyboard will not automatically appear when you select an InputField in your
    ///       Canvas. In order for the keyboard to appear you must call Keyboard.Instance.PresentKeyboard(string).
    ///       To retrieve the input from the Keyboard, subscribe to the textEntered event. Note that
    ///       tapping 'Close' on the Keyboard will not fire the textEntered event. You must tap 'Enter' to
    ///       get the textEntered event.
    /// </remarks>
    public class NonNativeKeyboard : MonoBehaviour
    {
        /// <summary>
        /// The instance of NonNativeKeyboard in the scene.
        /// </summary>
        /// <remarks>
        /// There can only be one instance of NonNativeKeyboard in a given scene.
        /// </remarks>
        public static NonNativeKeyboard Instance { get; private set; }

        /// <summary>
        /// Layout type enum for the type of keyboard layout to use.  
        /// Used during keyboard spawning to enable the correct keys based on layout type.
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
        /// Fired when the user submits the text (i.e. when 'Enter' button is pressed and SubmitOnEnter is true).
        /// </summary>
        [field: SerializeField, Tooltip("Fired when the user submits the text (i.e. when 'Enter' button is pressed and SubmitOnEnter is true).")]
        public NonNativeKeyboardTextEvent OnTextSubmit { get; private set; }

        /// <summary>
        /// Fired every time the text changes.
        /// </summary>
        [field: SerializeField, Tooltip("Fired every time the text changes.")]
        public NonNativeKeyboardTextEvent OnTextUpdate { get; private set; }

        /// <summary>
        /// Fired every time the close button is pressed.
        /// </summary>
        [field: SerializeField, Tooltip("Fired every time the close button is pressed.")]
        public NonNativeKeyboardTextEvent OnClose { get; private set; }

        /// <summary>
        /// Fired when the keyboard is shown.
        /// </summary>
        [field: SerializeField, Tooltip("Fired when the keyboard is shown.")]
        public UnityEvent OnShow { get; private set; }

        /// <summary>
        /// Fired when the shift status is changed.
        /// </summary>
        [field: SerializeField, Tooltip("Fired when the shift status is changed.")]
        public NonNativeKeyboardShiftEvent OnKeyboardShifted { get; private set; }

        /// <summary>
        /// Fired when any key on the keyboard is pressed.
        /// </summary>
        [field: SerializeField, Tooltip("Fired when any key on the keyboard is pressed.")]
        public NonNativeKeyboardPressEvent OnKeyPressed { get; private set; }

        #endregion Callbacks

        #region Properties

        /// <summary>
        /// Current text of the keyboard.
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnTextUpdate?.Invoke(text);
                }
            }
        }

        private string text;

        /// <summary>
        /// Whether the keyboard is currently active
        /// </summary>
        public bool Active => gameObject.activeInHierarchy;

        /// <summary>
        /// Whether submit on enter.
        /// </summary>
        [field: SerializeField, Tooltip("Whether submit on enter.")]
        public bool SubmitOnEnter { get; set; }

        /// <summary>
        /// Whether make the keyboard disappear automatically after a timeout.
        /// </summary>
        [field: SerializeField, Tooltip("Whether make the keyboard disappear automatically after a timeout.")]
        public bool CloseOnInactivity { get; set; }

        /// <summary>
        /// Inactivity time to wait until making the keyboard disappear in seconds.
        /// </summary>
        [field: SerializeField, Tooltip("Inactivity time to wait until making the keyboard disappear in seconds.")]
        public float CloseOnInactivityTime { get; set; } = 15;

        /// <summary>
        /// Accessor reporting shift state of keyboard.
        /// </summary>
        public bool IsShifted { get; private set; }

        /// <summary>
        /// Accessor reporting caps lock state of keyboard.
        /// </summary>
        public bool IsCapsLocked { get; private set; }

        #endregion Properties

        #region Keyboard component references

        /// <summary>
        /// The panel that contains the alpha keys.
        /// </summary>
        [SerializeField, Tooltip("The panel that contains the alpha keys.")]
        private GameObject alphaKeysSection = null;

        /// <summary>
        /// The panel that contains the number and symbol keys.
        /// </summary>
        [SerializeField, Tooltip("The panel that contains the number and symbol keys.")]
        private GameObject symbolKeysSection = null;

        /// <summary>
        /// References the default bottom panel.
        /// </summary>
        [SerializeField, Tooltip("References the default bottom panel.")]
        private GameObject defaultBottomKeysSection = null;

        /// <summary>
        /// References the .com bottom panel.
        /// </summary>
        [SerializeField, Tooltip("References the .com bottom panel.")]
        private GameObject urlBottomKeysSection = null;

        /// <summary>
        /// References the @ bottom panel.
        /// </summary>
        [SerializeField, Tooltip("References the @ bottom panel.")]
        private GameObject emailBottomKeysSection = null;

        #endregion Keyboard component references

        #region Private fields

        private LayoutType lastKeyboardLayout = LayoutType.Alpha;

        /// <summary>
        /// Time on which the keyboard should close on inactivity
        /// </summary>
        private float timeToClose;

        #endregion Private fields

        #region MonoBehaviours

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("There should only be one NonNativeKeyboard in a scene. Destroying a duplicate instance.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // Hide the keyboard on Awake
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            CheckForCloseOnInactivityTimeExpired();
        }

        private void OnDisable()
        {
            // Reset the keyboard layout for next use
            lastKeyboardLayout = LayoutType.Alpha;
            Clear();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #endregion MonoBehaviours

        #region Open Functions

        /// <summary>
        /// Opens the default keyboard
        /// </summary>
        public void Open()
        {
            Open(LayoutType.Alpha);
        }


        /// <summary>
        /// Opens the default keyboard with start text.
        /// </summary>
        /// <param name="startText">The initial text to put into <see cref="Text"/>.</param>
        public void Open(string startText)
        {
            Clear();
            Text = startText;
            Open();
        }

        /// <summary>
        /// Opens a specific keyboard.
        /// </summary>
        /// <param name="keyboardType">Specify the keyboard type.</param>
        public void Open(LayoutType keyboardType)
        {
            ResetClosingTime();
            gameObject.SetActive(true);
            ActivateSpecificKeyboard(keyboardType);
            OnShow.Invoke();
        }

        /// <summary>
        /// Opens a specific keyboard, with start text.
        /// </summary>
        /// <param name="startText">The initial text to put into <see cref="Text"/>.</param>
        /// <param name="keyboardType">Specify the keyboard type.</param>
        public void Open(string startText, LayoutType keyboardType)
        {
            Clear();
            Text = startText;
            Open(keyboardType);
        }

        #endregion Open Functions

        #region Keyboard Functions

        /// <summary>
        /// Process key presses from <see cref="NonNativeValueKey"/>.
        /// </summary>
        public void ProcessValueKeyPress(NonNativeValueKey valueKey)
        {
            ResetClosingTime();
            OnKeyPressed.Invoke(valueKey);

            Text += valueKey.CurrentValue;
            OnTextUpdate.Invoke(Text);

            if (!IsCapsLocked)
            {
                Shift(false);
            }
        }

        /// <summary>
        /// Process key presses from <see cref="NonNativeFunctionKey"/>.
        /// </summary>
        public void ProcessFunctionKeyPress(NonNativeFunctionKey functionKey)
        {
            ResetClosingTime();
            OnKeyPressed.Invoke(functionKey);
            switch (functionKey.KeyFunction)
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
            }
        }

        /// <summary>
        /// Delete the last character.
        /// </summary>
        public void Backspace()
        {
            if (Text.Length > 0)
            {
                Text = Text.Substring(0, Text.Length - 1);
            }
        }

        /// <summary>
        /// Fire <see cref="OnTextSubmit"/> and close the keyboard if <see cref="SubmitOnEnter"/> is set to true.
        /// Otherwise append a new line character.
        /// </summary>
        public void Enter()
        {
            if (SubmitOnEnter)
            {
                OnTextSubmit.Invoke(Text);
                Close();
            }
            else
            {
                Text += "\n";
            }
        }

        /// <summary>
        /// Set the shift state of the keyboard.
        /// </summary>
        /// <param name="newShiftState">value the shift key should have after calling the method</param>
        public void Shift(bool newShiftState)
        {
            if (newShiftState != IsShifted)
            {
                IsShifted = newShiftState;
                OnKeyboardShifted.Invoke(IsShifted);
            }

            if (IsCapsLocked && !newShiftState)
            {
                IsCapsLocked = false;
            }
        }

        /// <summary>
        /// Set the caps lock state of the keyboard.
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
        /// </summary>
        public void Close()
        {
            OnClose.Invoke(Text);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Clear the text field and reset keyboard state (e.g. Shift and CapsLock).
        /// </summary>
        public void Clear()
        {
            ResetKeyboardState();
            Text = "";
        }

        #endregion Keyboard Functions

        #region Keyboard Layout Modes

        private void ShowAlphaKeyboardUpperSection()
        {
            alphaKeysSection.SetActive(true);
        }

        private void ShowAlphaKeyboardDefaultBottomKeysSection()
        {
            if (!defaultBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                defaultBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            defaultBottomKeysSection.SetActive(true);
        }

        private void ShowAlphaKeyboardEmailBottomKeysSection()
        {
            if (!emailBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                emailBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            emailBottomKeysSection.SetActive(true);
        }

        private void ShowAlphaKeyboardURLBottomKeysSection()
        {
            if (!urlBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                urlBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            urlBottomKeysSection.SetActive(true);
        }

        private void ShowSymbolKeyboard()
        {
            symbolKeysSection.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disable GameObjects for all keyboard elements.
        /// </summary>
        private void DisableAllKeyboards()
        {
            alphaKeysSection.SetActive(false);
            defaultBottomKeysSection.gameObject.SetActive(false);
            urlBottomKeysSection.gameObject.SetActive(false);
            emailBottomKeysSection.gameObject.SetActive(false);
            symbolKeysSection.gameObject.SetActive(false);
        }

        #endregion Keyboard Layout Modes

        #region Private Functions

        /// <summary>
        /// Activates a specific keyboard layout, and any sub keys.
        /// </summary>
        /// <param name="keyboardType">The keyboard layout type that should be activated</param>
        private void ActivateSpecificKeyboard(LayoutType keyboardType)
        {
            DisableAllKeyboards();
            ResetKeyboardState();

            switch (keyboardType)
            {
                case LayoutType.URL:
                {
                    lastKeyboardLayout = keyboardType;
                    ShowAlphaKeyboardUpperSection();
                    ShowAlphaKeyboardURLBottomKeysSection();
                    break;
                }

                case LayoutType.Email:
                {
                    lastKeyboardLayout = keyboardType;
                    ShowAlphaKeyboardUpperSection();
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
                    lastKeyboardLayout = keyboardType;
                    ShowAlphaKeyboardUpperSection();
                    ShowAlphaKeyboardDefaultBottomKeysSection();
                    break;
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        /// <summary>
        /// Reset temporary states of keyboard (Shift and Caps Lock).
        /// </summary>
        private void ResetKeyboardState()
        {
            Shift(false);
            CapsLock(false);
        }

        /// <summary>
        /// Reset inactivity closing timer
        /// </summary>
        private void ResetClosingTime()
        {
            timeToClose = Time.time + CloseOnInactivityTime;
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
        #endregion Private Functions
    }
}
