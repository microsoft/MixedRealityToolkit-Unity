// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Microsoft.MixedReality.Toolkit.UX.Experimental.NonNativeFunctionKey;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// A simple general use keyboard that provides an alternative to the native keyboard offered by each device.
    /// </summary>
    /// <remarks>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// 
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
        [field: SerializeField, Experimental, Tooltip("Fired when the user submits the text (i.e. when 'Enter' button is pressed and SubmitOnEnter is true).")]
        public NonNativeKeyboardTextEvent OnTextSubmit { get; private set; }

        /// <summary>
        /// Fired every time the text changes.
        /// </summary>
        [field: SerializeField, Tooltip("Fired every time the text changes.")]
        public NonNativeKeyboardTextEvent OnTextUpdate { get; private set; }

        /// <summary>
        /// Fired every time the caret index changes.
        /// </summary>
        [field: SerializeField, Tooltip("Fired every time the caret index changes.")]
        public NonNativeKeyboardIntEvent OnCaretIndexUpdate { get; private set; }

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
        /// The preview component that the keyboard uses to show the currently edited text.
        /// </summary>
        /// <remarks>
        /// Text will be stored in this component, so this component must to assigned for the keyboard to function.
        /// The default Non-native Keyboard prefab sets this field by default.
        /// </remarks>
        [field: SerializeField, Tooltip("The preview component that the keyboard uses to show the currently edited text.")]
        public KeyboardPreview Preview { get; set; }

        /// <summary>
        /// The text entered in the current keyboard session.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                if (Preview != null)
                {
                    Preview.Text = value;
                }

                if (text != value)
                {
                    text = value;
                    DoTextUpdated(value);
                }
            }
        }

        /// <summary>
        /// The caret index for the current keyboard session.
        /// </summary>
        public int CaretIndex
        {
            get => caretIndex;

            private set
            {
                if (Preview != null)
                {
                    Preview.CaretIndex = value;
                }

                if (caretIndex != value)
                {
                    caretIndex = value;
                    DoCaretIndexUpdated(value);
                }
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
        /// Get if the dictation services are available for this device.
        /// </summary>
        private bool IsDictationAvailable => XRSubsystemHelpers.DictationSubsystem != null;

        /// <summary>
        /// The panel that contains the alpha keys.
        /// </summary>
        [field: SerializeField, Tooltip("The panel that contains the alpha keys.")]
        public GameObject AlphaKeysSection { get; set; }

        /// <summary>
        /// The panel that contains the number and symbol keys.
        /// </summary>
        [field: SerializeField, Tooltip("The panel that contains the number and symbol keys.")]
        public GameObject SymbolKeysSection { get; set; }

        /// <summary>
        /// References the default bottom panel.
        /// </summary>
        [field: SerializeField, Tooltip("References the default bottom panel.")]
        public GameObject DefaultBottomKeysSection { get; set; }

        /// <summary>
        /// References the .com bottom panel.
        /// </summary>
        [field: SerializeField, Tooltip("References the .com bottom panel.")]
        public GameObject UrlBottomKeysSection { get; set; }

        /// <summary>
        /// References the @ bottom panel.
        /// </summary>
        [field: SerializeField, Tooltip("References the @ bottom panel.")]
        public GameObject EmailBottomKeysSection { get; set; }

        /// <summary>
        /// Used for changing the color of the icon to indicate if recording is active.
        /// </summary>
        [field: SerializeField, Tooltip("Used for changing the color of the icon to indicate if recording is active.")]
        public Image DictationRecordIcon { get; set; }
        #endregion Properties

        #region Private fields
        /// <summary>
        /// The keyword recognition subsystem that was stopped by this component.
        /// </summary>
        private IKeywordRecognitionSubsystem keywordRecognitionSubsystem = null;

        /// <summary>
        /// The inner text value set via the `Text` property
        /// </summary>
        private string text = string.Empty;

        /// <summary>
        /// The default color of the mic key.
        /// </summary>        
        private Color dictationRecordIconDefaultColor;

        /// <summary>
        /// Tracks whether or not dictation is actively recording.
        /// </summary>        
        private bool isRecording = false;

        /// <summary>
        /// Tracking the previous keyboard layout.
        /// </summary>
        /// <summary>
        /// The inner caret index set via the `CaretIndex` property
        /// </summary>
        private int caretIndex = 0;

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

            if (DictationRecordIcon != null)
            {
                dictationRecordIconDefaultColor = DictationRecordIcon.material.color;
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
            if (DictationRecordIcon != null)
            {
                DictationRecordIcon.gameObject.SetActive(IsDictationAvailable);
            }
        }

        /// <summary>
        /// Intermediary function for text update events.
        /// Workaround for strange leftover reference when unsubscribing.
        /// </summary>
        /// <param name="value">String value.</param>
        private void DoTextUpdated(string value) => OnTextUpdate?.Invoke(value);

        /// <summary>
        /// Intermediary function for caret index update events.
        /// </summary>
        /// <param name="value">Integer value.</param>
        private void DoCaretIndexUpdated(int value) => OnCaretIndexUpdate?.Invoke(value);

        private void LateUpdate()
        {
            CheckForCloseOnInactivityTimeExpired();
        }

        private void OnDisable()
        {
            // Reset the keyboard layout for next use
            lastKeyboardLayout = LayoutType.Alpha;
            Clear();
            StopDictation();
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
            Text = startText;
            Open();
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
            CaretIndex = Text.Length;
        }
        #endregion Open Functions

        #region Keyboard Functions

        /// <summary>
        /// Process key presses from <see cref="NonNativeValueKey"/>.
        /// </summary>
        public void ProcessValueKeyPress(NonNativeValueKey valueKey)
        {
            ResetClosingTime();
            OnKeyPressed?.Invoke(valueKey);
            Text = Text.Insert(CaretIndex, valueKey.CurrentValue);
            CaretIndex += valueKey.CurrentValue.Length;

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
                     MoveCaretLeft();
                    break;
                case Function.Next:
                    MoveCaretRight();
                    break;
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

                case Function.Dictate:
                    ToggleDictation();
                    break;

                case Function.Undefined:
                default:
                    Debug.LogErrorFormat("The {0} key on this keyboard hasn't been assigned a function.", functionKey.name);
                    break;
            }
        }

        /// <summary>
        /// Delete the last character.
        /// </summary>
        public void Backspace()
        {
            int caretPosition = CaretIndex;
            if (caretPosition > 0)
            {
                --caretPosition;
                Text = Text.Remove(caretPosition, 1);
                CaretIndex = caretPosition;
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
                Text = Text.Insert(CaretIndex, enterString);
                CaretIndex += enterString.Length;
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
            int caretPosition = CaretIndex;
            Text = Text.Insert(caretPosition++, " ");
            CaretIndex = caretPosition;
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void Tab()
        {
            string tabString = "\t";
            Text = Text.Insert(CaretIndex, tabString);
            CaretIndex += tabString.Length;
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void MoveCaretRight()
        {
            if (CaretIndex < Text.Length)
            {
                CaretIndex++;
            }
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void MoveCaretLeft()
        {
            if (CaretIndex > 0)
            {
                --CaretIndex;
            }
        }

        /// <summary>
        /// Toggle dictation on or off,
        /// </summary>
        public void ToggleDictation()
        {
            if (isRecording)
            {
                StopDictation();
            }
            else
            {
                StartDictation();
            }
        }

        /// <summary>
        /// Start dictation on a DictationSubsystem.
        /// </summary>
        public void StartDictation()
        {
            var dictationSubsystem = XRSubsystemHelpers.DictationSubsystem;
            if (dictationSubsystem != null && !isRecording)
            {
                isRecording = true;
                UpdateDictationRecordIconColor();

                keywordRecognitionSubsystem = XRSubsystemHelpers.KeywordRecognitionSubsystem;
                if (keywordRecognitionSubsystem != null)
                {
                    keywordRecognitionSubsystem.Stop();
                }

                ResetClosingTime();
                dictationSubsystem.Recognized += OnDictationRecognizedResult;
                dictationSubsystem.RecognitionFinished += OnDictationFinished;
                dictationSubsystem.RecognitionFaulted += OnDictationFaulted;
                dictationSubsystem.StartDictation();
            }
        }

        /// <summary>
        /// Stop dictation on the current DictationSubsystem.
        /// </summary>
        public void StopDictation()
        {
            var dictationSubsystem = XRSubsystemHelpers.DictationSubsystem;
            if (dictationSubsystem != null)
            {
                dictationSubsystem.StopDictation();
            }
        }

        /// <summary>
        /// Close the keyboard.
        /// </summary>
        public void Close()
        {
            StopDictation();
            OnClose.Invoke(Text);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Clear the text field and reset keyboard state (e.g. Shift and CapsLock).
        /// </summary>
        public void Clear()
        {
            ResetKeyboardState();
            Text = string.Empty;
            CaretIndex = Text.Length;
        }

        #endregion Keyboard Functions

        #region Keyboard Layout Modes

        private void ShowAlphaKeyboardUpperSection()
        {
            if (AlphaKeysSection != null)
            {
                AlphaKeysSection.SetActive(true);
            }
        }

        private void ShowAlphaKeyboardDefaultBottomKeysSection()
        {
            if (DefaultBottomKeysSection != null && !DefaultBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                DefaultBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            if (DefaultBottomKeysSection != null)
            {
                DefaultBottomKeysSection.SetActive(true);
            }
        }

        private void ShowAlphaKeyboardEmailBottomKeysSection()
        {
            if (EmailBottomKeysSection != null && !EmailBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                EmailBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            if (EmailBottomKeysSection != null)
            {
                EmailBottomKeysSection.SetActive(true);
            }
        }

        private void ShowAlphaKeyboardURLBottomKeysSection()
        {
            if (UrlBottomKeysSection != null && !UrlBottomKeysSection.transform.parent.gameObject.activeSelf)
            {
                UrlBottomKeysSection.transform.parent.gameObject.SetActive(true);
            }
            if (UrlBottomKeysSection != null)
            {
                UrlBottomKeysSection.SetActive(true);
            }
        }   

        private void ShowSymbolKeyboard()
        {
            if (SymbolKeysSection != null)
            {
                SymbolKeysSection.gameObject.SetActive(true);
            }
        } 

        /// <summary>
        /// Disable GameObjects for all keyboard elements.
        /// </summary>
        private void DisableAllKeyboards()
        {
            if (AlphaKeysSection != null)
            {
                AlphaKeysSection.SetActive(false);
            }
            if (DefaultBottomKeysSection != null)
            {
                DefaultBottomKeysSection.SetActive(false);
            }
            if (UrlBottomKeysSection != null)
            {
                UrlBottomKeysSection.SetActive(false);
            }
            if (EmailBottomKeysSection != null)
            {
                EmailBottomKeysSection.SetActive(false);
            }
            if (SymbolKeysSection != null)
            {
                SymbolKeysSection.gameObject.SetActive(false);
            }
        }

        #endregion Keyboard Layout Modes

        #region Private Functions
        /// <summary>
        /// Set mike recording look (red)
        /// </summary>
        private void UpdateDictationRecordIconColor()
        {
            if (IsDictationAvailable && DictationRecordIcon != null)
            {
                DictationRecordIcon.color = isRecording ? Color.red : dictationRecordIconDefaultColor;
            }
        }

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

        /// <summary>
        /// Called when dictation result is obtained
        /// </summary>
        /// <param name="eventData">Dictation event data</param>
        private void OnDictationRecognizedResult(DictationResultEventArgs eventData)
        {
            var text = eventData.Result;
            ResetClosingTime();

            if (!string.IsNullOrEmpty(text))
            {
                Text = Text.Insert(CaretIndex, text);
                CaretIndex += text.Length;
            }
        }

        /// <summary>
        /// Called when dictation is completed
        /// </summary>
        /// <param name="eventData">Dictation event data</param>
        private void OnDictationFinished(DictationSessionEventArgs eventData)
        {
            HandleDictationShutdown();
        }

        /// <summary>
        /// Called when dictation is faulted
        /// </summary>
        /// <param name="eventData">Dictation event data</param>
        private void OnDictationFaulted(DictationSessionEventArgs eventData)
        {
            Debug.LogError("Dictation faulted. Reason: " + eventData.Reason);
            HandleDictationShutdown();
        }

        /// <summary>
        /// Release references to dictation events
        /// </summary>
        private void HandleDictationShutdown()
        {
            var dictationSubsystem = XRSubsystemHelpers.DictationSubsystem;
            if (dictationSubsystem != null)
            {
                dictationSubsystem.RecognitionFinished -= OnDictationFinished;
                dictationSubsystem.RecognitionFaulted -= OnDictationFaulted;
                dictationSubsystem.Recognized -= OnDictationRecognizedResult;

                isRecording = false;
                UpdateDictationRecordIconColor();
            }

            if (keywordRecognitionSubsystem != null)
            {
                keywordRecognitionSubsystem.Start();
                keywordRecognitionSubsystem = null;
            }
        }
        #endregion Private Functions
    }
}
