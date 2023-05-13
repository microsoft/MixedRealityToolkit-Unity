// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Microsoft.MixedReality.Toolkit.UX.NonNativeFunctionKey;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple general use keyboard that provides an alternative to the native keyboard offered by each device.
    /// </summary>
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
            /// <summary>
            /// Enables the alpha keys section and the alpha space section.
            /// </summary>
            Alpha,
            /// <summary>
            /// Enables the symbol keys section.
            /// </summary>
            Symbol,
            /// <summary>
            /// Enables the alpha keys section and the url space section.
            /// </summary>
            URL,
            /// <summary>
            /// Enables the alpha keys section and the email space section.
            /// </summary>
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
        /// The InputField that the keyboard uses to show the currently edited text.
        /// If you are using the Keyboard prefab you can ignore this field as it will
        /// be already assigned.
        /// </summary>
        [field: SerializeField, Tooltip("The input field used to view the typed text.")]
        public TMP_InputField InputField = null;


        /// <summary>
        /// A wrapper for InputField.text. 
        /// </summary>
        [SerializeField, Tooltip("The input field used to view the typed text.")]
        public string Text
        {
            get => InputField == null ? null : InputField.text;
            set
            {
                if (InputField != null) InputField.text = value;
            }
        }

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

        /// <summary>
        /// The position of the caret in the text field.
        /// </summary>
        private int m_CaretPosition = 0;

        /// <summary>
        /// The panel that contains the alpha keys.
        /// </summary>
        [SerializeField, Tooltip("The panel that contains the alpha keys.")]
        public GameObject alphaKeysSection = null;

        /// <summary>
        /// The panel that contains the number and symbol keys.
        /// </summary>
        [SerializeField, Tooltip("The panel that contains the number and symbol keys.")]
        public GameObject symbolKeysSection = null;

        /// <summary>
        /// References the default bottom panel.
        /// </summary>
        [SerializeField, Tooltip("References the default bottom panel.")]
        public GameObject defaultBottomKeysSection = null;

        /// <summary>
        /// References the .com bottom panel.
        /// </summary>
        [SerializeField, Tooltip("References the .com bottom panel.")]
        public GameObject urlBottomKeysSection = null;

        /// <summary>
        /// References the @ bottom panel.
        /// </summary>
        [SerializeField, Tooltip("References the @ bottom panel.")]
        public GameObject emailBottomKeysSection = null;

        #endregion Properties

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

             
            if (InputField != null)
            {
                // Setting the keyboardType to an undefined TouchScreenKeyboardType,
                // which prevents the MRTK keyboard from triggering the system keyboard itself.
                InputField.keyboardType = (TouchScreenKeyboardType)(-1);
            }
            else
            {
                Debug.LogError("You must set an input field for the NonNativeKeyboard.");
            }

            if (OnKeyPressed == null)
            {
                OnKeyPressed = new NonNativeKeyboardPressEvent();
            }
            if (OnTextSubmit == null)
            {
                OnTextSubmit = new NonNativeKeyboardTextEvent();
            }
            if (OnTextUpdate == null)
            {
                OnTextUpdate = new NonNativeKeyboardTextEvent();
            }
            if (OnClose == null)
            {
                OnTextUpdate = new NonNativeKeyboardTextEvent();
            }
            if (OnShow == null)
            {
                OnShow = new UnityEvent();
            }
            if (OnKeyboardShifted == null)
            {
                OnKeyboardShifted = new NonNativeKeyboardShiftEvent();
            }

            // Hide the keyboard on Awake
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Set up Dictation, CanvasEX, and automatically select the TextInput object.
        /// </summary>
        protected void Start()
        {
            // Delegate Subscription
            if (InputField != null)
            {
                InputField.onValueChanged.AddListener(DoTextUpdated);
            }
        }

        /// <summary>
        /// Intermediary function for text update events.
        /// Workaround for strange leftover reference when unsubscribing.
        /// </summary>
        /// <param name="value">String value.</param>
        private void DoTextUpdated(string value) => OnTextUpdate?.Invoke(value);

        private void LateUpdate()
        {
            CheckForCloseOnInactivityTimeExpired();
        }

        private void UpdateCaretPosition(int newPos) => InputField.caretPosition = newPos;

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
        /// <param name="startText">The initial text to put into <see cref="InputField"/>.</param>
        public void Open(string startText)
        {
            Clear();
            if (InputField != null)
            {
                Text = startText;
                UpdateCaretPosition(Text.Length);
            }
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
            OnShow?.Invoke();

            if (InputField != null)
            {
                InputField.ActivateInputField();
                UpdateCaretPosition(Text.Length);
            }
        }

        /// <summary>
        /// Opens a specific keyboard, with start text.
        /// </summary>
        /// <param name="startText">The initial text to put into <see cref="Text"/>.</param>
        /// <param name="keyboardType">Specify the keyboard type.</param>
        public void Open(string startText, LayoutType keyboardType)
        {
            Clear();
            if (InputField != null)
            {
                InputField.text = startText;
                UpdateCaretPosition(Text.Length);
            }
            Open(keyboardType);
        }

        #endregion Open Functions

        #region Keyboard Functions

        /// <summary>
        /// Process key presses from <see cref="NonNativeValueKey"/>.
        /// </summary>
        public void ProcessValueKeyPress(NonNativeValueKey valueKey)
        {
            if (InputField != null)
            {
                ResetClosingTime();
                OnKeyPressed?.Invoke(valueKey);

                if (InputField != null)
                {
                    m_CaretPosition = InputField.caretPosition;
                    Text = Text.Insert(m_CaretPosition, valueKey.CurrentValue);
                    m_CaretPosition += valueKey.CurrentValue.Length;

                    UpdateCaretPosition(m_CaretPosition);
                }

                if (!IsCapsLocked)
                {
                    Shift(false);
                }
            }
            else
            {
                Debug.LogError("You must set an input field for the NonNativeKeyboard.");
            }
        }

        /// <summary>
        /// Process key presses from <see cref="NonNativeFunctionKey"/>.
        /// </summary>
        public void ProcessFunctionKeyPress(NonNativeFunctionKey functionKey)
        {
            if (InputField != null)
            {
                ResetClosingTime();
                OnKeyPressed?.Invoke(functionKey);
                switch (functionKey.KeyFunction)
                {
                    case Function.Enter:
                        Enter();
                        break;

                    case Function.Tab:
                        Tab();
                        break;

                    case Function.Alpha:
                        ActivateSpecificKeyboard(lastKeyboardLayout);
                        break;

                    case Function.Symbol:
                        ActivateSpecificKeyboard(LayoutType.Symbol);
                        break;

                    case Function.Previous:
                        {
                            MoveCaretLeft();
                            break;
                        }

                    case Function.Next:
                        {
                            MoveCaretRight();
                            break;
                        }

                    case Function.Close:
                        Close();
                        break;

                    case Function.Shift:
                        Shift(!IsShifted);
                        break;

                    case Function.CapsLock:
                        CapsLock(!IsCapsLocked);
                        break;

                    case Function.Space:
                        Space();
                        break;

                    case Function.Backspace:
                        Backspace();
                        break;

                    case Function.Undefined:
                    default:
                        Debug.LogErrorFormat("The {0} key on this keyboard hasn't been assigned a function.", functionKey.name);
                        break;
                }
            }
            else
            {
                Debug.LogError("You must set an input field for the NonNativeKeyboard.");
            }
        }

        /// <summary>
        /// Delete the last character.
        /// </summary>
        public void Backspace()
        {
            // check if text is selected
            if (InputField.selectionFocusPosition != InputField.caretPosition || InputField.selectionAnchorPosition != InputField.caretPosition)
            {
                if (InputField.selectionAnchorPosition > InputField.selectionFocusPosition) // right to left
                {
                    Text = Text.Substring(0, InputField.selectionFocusPosition) + Text.Substring(InputField.selectionAnchorPosition);
                    InputField.caretPosition = InputField.selectionFocusPosition;
                }
                else // left to right
                {
                    Text = Text.Substring(0, InputField.selectionAnchorPosition) + Text.Substring(InputField.selectionFocusPosition);
                    InputField.caretPosition = InputField.selectionAnchorPosition;
                }

                m_CaretPosition = InputField.caretPosition;
                InputField.selectionAnchorPosition = m_CaretPosition;
                InputField.selectionFocusPosition = m_CaretPosition;
            }
            else
            {
                m_CaretPosition = InputField.caretPosition;

                if (m_CaretPosition > 0)
                {
                    --m_CaretPosition;
                    Text = Text.Remove(m_CaretPosition, 1);
                    UpdateCaretPosition(m_CaretPosition);
                }
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
                string enterString = "\n";

                m_CaretPosition = InputField.caretPosition;

                Text = Text.Insert(m_CaretPosition, enterString);
                m_CaretPosition += enterString.Length;

                UpdateCaretPosition(m_CaretPosition);
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
                OnKeyboardShifted?.Invoke(IsShifted);
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
            m_CaretPosition = InputField.caretPosition;
            Text = Text.Insert(m_CaretPosition++, " ");

            UpdateCaretPosition(m_CaretPosition);
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void Tab()
        {
            string tabString = "\t";

            m_CaretPosition = InputField.caretPosition;

            Text = Text.Insert(m_CaretPosition, tabString);
            m_CaretPosition += tabString.Length;

            UpdateCaretPosition(m_CaretPosition);
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void MoveCaretRight()
        {
            m_CaretPosition = InputField.caretPosition;

            if (m_CaretPosition < Text.Length)
            {
                ++m_CaretPosition;
                UpdateCaretPosition(m_CaretPosition);
            }
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void MoveCaretLeft()
        {
            m_CaretPosition = InputField.caretPosition;

            if (m_CaretPosition > 0)
            {
                --m_CaretPosition;
                UpdateCaretPosition(m_CaretPosition);
            }
        }

        /// <summary>
        /// Close the keyboard.
        /// </summary>
        public void Close()
        {
            if (InputField != null)
            {
                OnClose.Invoke(Text);
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Clear the text field and reset keyboard state (e.g. Shift and CapsLock).
        /// </summary>
        public void Clear()
        {
            if (InputField != null)
            {
                ResetKeyboardState();
                Text = "";
                UpdateCaretPosition(Text.Length);
                m_CaretPosition = InputField.caretPosition;
            }
        }

        #endregion Keyboard Functions

        #region Keyboard Layout Modes

        private void ShowAlphaKeyboardUpperSection()
        {
            if (alphaKeysSection != null)
            {
                alphaKeysSection.SetActive(true);
            }
        }

        private void ShowAlphaKeyboardDefaultBottomKeysSection()
        {
            if (defaultBottomKeysSection != null && !defaultBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                defaultBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            if (defaultBottomKeysSection != null)
            {
                defaultBottomKeysSection.SetActive(true);
            }
        }

        private void ShowAlphaKeyboardEmailBottomKeysSection()
        {
            if (emailBottomKeysSection != null && !emailBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                emailBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            if (emailBottomKeysSection != null)
            {
                emailBottomKeysSection.SetActive(true);
            }
        }

        private void ShowAlphaKeyboardURLBottomKeysSection()
        {
            if (urlBottomKeysSection != null && !urlBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                urlBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            if (urlBottomKeysSection != null)
            {
                urlBottomKeysSection.SetActive(true);
            }
        }   

        private void ShowSymbolKeyboard()
        {
            if (symbolKeysSection != null)
            {
                symbolKeysSection.gameObject.SetActive(true);
            }
        } 

        /// <summary>
        /// Disable GameObjects for all keyboard elements.
        /// </summary>
        private void DisableAllKeyboards()
        {
            if (alphaKeysSection != null)
            {
                alphaKeysSection.SetActive(false);
            }
            if (defaultBottomKeysSection != null)
            {
                defaultBottomKeysSection.SetActive(false);
            }
            if (urlBottomKeysSection != null)
            {
                urlBottomKeysSection.SetActive(false);
            }
            if (emailBottomKeysSection != null)
            {
                emailBottomKeysSection.SetActive(false);
            }
            if (symbolKeysSection != null)
            {
                symbolKeysSection.gameObject.SetActive(false);
            }
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
