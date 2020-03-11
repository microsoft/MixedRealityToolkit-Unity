// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;
using System;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
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
    public class NonNativeKeyboard : InputSystemGlobalHandlerListener, IMixedRealityDictationHandler
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
        /// Sent when the 'Enter' button is pressed. To retrieve the text from the event,
        /// cast the sender to 'Keyboard' and get the text from the TextInput field.
        /// (Cleared when keyboard is closed.)
        /// </summary>
        public event EventHandler OnTextSubmitted = delegate { };

        /// <summary>
        /// Fired every time the text in the InputField changes.
        /// (Cleared when keyboard is closed.)
        /// </summary>
        public event Action<string> OnTextUpdated = delegate { };

        /// <summary>
        /// Fired every time the close button is pressed.
        /// (Cleared when keyboard is closed.)
        /// </summary>
        public event EventHandler OnClosed = delegate { };

        /// <summary>
        /// Sent when the 'Previous' button is pressed. Ideally you would use this event
        /// to set your targeted text input to the previous text field in your document.
        /// (Cleared when keyboard is closed.)
        /// </summary>
        public event EventHandler OnPrevious = delegate { };

        /// <summary>
        /// Sent when the 'Next' button is pressed. Ideally you would use this event
        /// to set your targeted text input to the next text field in your document.
        /// (Cleared when keyboard is closed.)
        /// </summary>
        public event EventHandler OnNext = delegate { };

        /// <summary>
        /// Sent when the keyboard is placed.  This allows listener to know when someone else is co-opting the keyboard.
        /// </summary>
        public event EventHandler OnPlacement = delegate { };

        #endregion Callbacks

        /// <summary>
        /// The InputField that the keyboard uses to show the currently edited text.
        /// If you are using the Keyboard prefab you can ignore this field as it will
        /// be already assigned.
        /// </summary>
        [Experimental]
        public TMP_InputField InputField = null;

        /// <summary>
        /// Move the axis slider based on the camera forward and the keyboard plane projection.
        /// </summary>
        public AxisSlider InputFieldSlide = null;

        /// <summary>
        /// Bool for toggling the slider being enabled.
        /// </summary>
        public bool SliderEnabled = true;

        /// <summary>
        /// Bool to flag submitting on enter
        /// </summary>
        public bool SubmitOnEnter = true;

        /// <summary>
        /// The panel that contains the alpha keys.
        /// </summary>
        public Image AlphaKeyboard = null;

        /// <summary>
        /// The panel that contains the number and symbol keys.
        /// </summary>
        public Image SymbolKeyboard = null;

        /// <summary>
        /// References abc bottom panel.
        /// </summary>
        public Image AlphaSubKeys = null;

        /// <summary>
        /// References .com bottom panel.
        /// </summary>
        public Image AlphaWebKeys = null;

        /// <summary>
        /// References @ bottom panel.
        /// </summary>
        public Image AlphaMailKeys = null;

        private LayoutType m_LastKeyboardLayout = LayoutType.Alpha;

        /// <summary>
        /// The scale the keyboard should be at its maximum distance.
        /// </summary>
        [Header("Positioning")]
        [SerializeField]
        private float m_MaxScale = 1.0f;

        /// <summary>
        /// The scale the keyboard should be at its minimum distance.
        /// </summary>
        [SerializeField]
        private float m_MinScale = 1.0f;

        /// <summary>
        /// The maximum distance the keyboard should be from the user.
        /// </summary>
        [SerializeField]
        private float m_MaxDistance = 3.5f;

        /// <summary>
        /// The minimum distance the keyboard needs to be away from the user.
        /// </summary>
        [SerializeField]
        private float m_MinDistance = 0.25f;

        /// <summary>
        /// Make the keyboard disappear automatically after a timeout
        /// </summary>
        public bool CloseOnInactivity = true;

        /// <summary>
        /// Inactivity time that makes the keyboard disappear automatically.
        /// </summary>
        public float CloseOnInactivityTime = 15;

        /// <summary>
        /// Time on which the keyboard should close on inactivity
        /// </summary>
        private float _closingTime;

        /// <summary>
        /// Event fired when shift key on keyboard is pressed.
        /// </summary>
        public event Action<bool> OnKeyboardShifted = delegate { };

        /// <summary>
        /// Current shift state of keyboard.
        /// </summary>
        private bool m_IsShifted = false;

        /// <summary>
        /// Current caps lock state of keyboard.
        /// </summary>
        private bool m_IsCapslocked = false;

        /// <summary>
        /// Accessor reporting shift state of keyboard.
        /// </summary>
        public bool IsShifted
        {
            get { return m_IsShifted; }
        }

        /// <summary>
        /// Accessor reporting caps lock state of keyboard.
        /// </summary>
        public bool IsCapsLocked
        {
            get { return m_IsCapslocked; }
        }

        /// <summary>
        /// The position of the caret in the text field.
        /// </summary>
        private int m_CaretPosition = 0;

        /// <summary>
        /// The starting scale of the keyboard.
        /// </summary>
        private Vector3 m_StartingScale = Vector3.one;

        /// <summary>
        /// The default bounds of the keyboard.
        /// </summary>
        private Vector3 m_ObjectBounds;

        /// <summary>
        /// The default color of the mike key.
        /// </summary>        
        private Color _defaultColor;

        /// <summary>
        /// The image on the mike key.
        /// </summary>
        private Image _recordImage;

        /// <summary>
        /// User can add an audio source to the keyboard to have a click be heard on tapping a key 
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// Dictation System
        /// </summary>
        private IMixedRealityDictationSystem dictationSystem;

        /// <summary>
        /// Deactivate on Awake.
        /// </summary>
        void Awake()
        {
            Instance = this;

            m_StartingScale = transform.localScale;
            Bounds canvasBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform);

            RectTransform rect = GetComponent<RectTransform>();
            m_ObjectBounds = new Vector3(canvasBounds.size.x * rect.localScale.x, canvasBounds.size.y * rect.localScale.y, canvasBounds.size.z * rect.localScale.z);

            // Actually find microphone key in the keyboard
            var dictationButton = TransformExtensions.GetChildRecursive(gameObject.transform, "Dictation");
            if (dictationButton != null)
            {
                var dictationIcon = dictationButton.Find("keyboard_closeIcon");
                if (dictationIcon != null)
                {
                    _recordImage = dictationIcon.GetComponentInChildren<Image>();
                    var material = new Material(_recordImage.material);
                    _defaultColor = material.color;
                    _recordImage.material = material;
                }
            }

            // Setting the keyboardType to an undefined TouchScreenKeyboardType,
            // which prevents the MRTK keyboard from triggering the system keyboard itself.
            InputField.keyboardType = (TouchScreenKeyboardType)(int.MaxValue);

            // Keep keyboard deactivated until needed
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Set up Dictation, CanvasEX, and automatically select the TextInput object.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            dictationSystem = CoreServices.GetInputSystemDataProvider<IMixedRealityDictationSystem>();

            // Delegate Subscription
            InputField.onValueChanged.AddListener(DoTextUpdated);
        }

        protected override void RegisterHandlers()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityDictationHandler>(this);
        }

        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityDictationHandler>(this);
        }

        /// <summary>
        /// Intermediary function for text update events.
        /// Workaround for strange leftover reference when unsubscribing.
        /// </summary>
        /// <param name="value">String value.</param>
        private void DoTextUpdated(string value) => OnTextUpdated?.Invoke(value);

        /// <summary>
        /// Makes sure the input field is always selected while the keyboard is up.
        /// </summary>
        private void LateUpdate()
        {
            // Axis Slider
            if (SliderEnabled)
            {
                Vector3 nearPoint = Vector3.ProjectOnPlane(CameraCache.Main.transform.forward, transform.forward);
                Vector3 relPos = transform.InverseTransformPoint(nearPoint);
                InputFieldSlide.TargetPoint = relPos;
            }

            CheckForCloseOnInactivityTimeExpired();
        }

        private void UpdateCaretPosition(int newPos) => InputField.caretPosition = newPos;

        /// <summary>
        /// Called whenever the keyboard is disabled or deactivated.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            m_LastKeyboardLayout = LayoutType.Alpha;
            Clear();
        }


        /// <summary>
        /// Called when dictation hypothesis is found. Not used here
        /// </summary>
        /// <param name="eventData">Dictation event data</param>
        public void OnDictationHypothesis(DictationEventData eventData) { }

        /// <summary>
        /// Called when dictation result is obtained
        /// </summary>
        /// <param name="eventData">Dictation event data</param>
        public void OnDictationResult(DictationEventData eventData)
        {
            if (eventData.used)
            {
                return;
            }
            var text = eventData.DictationResult;
            ResetClosingTime();
            if (text != null)
            {
                m_CaretPosition = InputField.caretPosition;

                InputField.text = InputField.text.Insert(m_CaretPosition, text);
                m_CaretPosition += text.Length;

                UpdateCaretPosition(m_CaretPosition);
                eventData.Use();
            }
        }

        /// <summary>
        /// Called when dictation is completed
        /// </summary>
        /// <param name="eventData">Dictation event data</param>
        public void OnDictationComplete(DictationEventData eventData)
        {
            ResetClosingTime();
            SetMicrophoneDefault();
        }

        /// <summary>
        /// Called on dictation error. Not used here.
        /// </summary>
        /// <param name="eventData">Dictation event data</param>
        public void OnDictationError(DictationEventData eventData) { }

        /// <summary>
        /// Destroy unmanaged memory links.
        /// </summary>
        void OnDestroy()
        {
            if (dictationSystem != null && IsMicrophoneActive())
            {
                dictationSystem.StopRecording();
            }

            Instance = null;
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

            OnPlacement(this, EventArgs.Empty);

            // todo: if the app is built for xaml, our prefab and the system keyboard may be displayed.
            InputField.ActivateInputField();

            SetMicrophoneDefault();
        }


        /// <summary>
        /// Presents the default keyboard to the camera, with start text.
        /// </summary>
        /// <param name="startText">The initial text to show in the keyboard's input field.</param>
        public void PresentKeyboard(string startText)
        {
            PresentKeyboard();
            Clear();
            InputField.text = startText;
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
        /// Function to reposition the Keyboard based on target position and vertical offset 
        /// </summary>
        /// <param name="kbPos">World position for keyboard</param>
        /// <param name="verticalOffset">Optional vertical offset of keyboard</param>
        public void RepositionKeyboard(Vector3 kbPos, float verticalOffset = 0.0f)
        {
            transform.position = kbPos;
            ScaleToSize();
            LookAtTargetOrigin();
        }

        /// <summary>
        /// Function to reposition the keyboard based on target transform and collider information 
        /// </summary>
        /// <param name="objectTransform">Transform of target object to remain relative to</param>
        /// <param name="aCollider">Optional collider information for offset placement</param>
        /// <param name="verticalOffset">Optional vertical offset from the target</param>
        public void RepositionKeyboard(Transform objectTransform, BoxCollider aCollider = null, float verticalOffset = 0.0f)
        {
            transform.position = objectTransform.position;

            if (aCollider != null)
            {
                float yTranslation = -((aCollider.bounds.size.y * 0.5f) + verticalOffset);
                transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }
            else
            {
                float yTranslation = -((m_ObjectBounds.y * 0.5f) + verticalOffset);
                transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }

            ScaleToSize();
            LookAtTargetOrigin();
        }

        /// <summary>
        /// Function to scale keyboard to the appropriate size based on distance
        /// </summary>
        private void ScaleToSize()
        {
            float distance = (transform.position - CameraCache.Main.transform.position).magnitude;
            float distancePercent = (distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
            float scale = m_MinScale + (m_MaxScale - m_MinScale) * distancePercent;

            scale = Mathf.Clamp(scale, m_MinScale, m_MaxScale);
            transform.localScale = m_StartingScale * scale;

            Debug.LogFormat("Setting scale: {0} for distance: {1}", scale, distance);
        }

        /// <summary>
        /// Look at function to have the keyboard face the user
        /// </summary>
        private void LookAtTargetOrigin()
        {
            transform.LookAt(CameraCache.Main.transform.position);
            transform.Rotate(Vector3.up, 180.0f);
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
                        ShowAlphaKeyboard();
                        TryToShowURLSubkeys();
                        break;
                    }

                case LayoutType.Email:
                    {
                        ShowAlphaKeyboard();
                        TryToShowEmailSubkeys();
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
                        TryToShowAlphaSubkeys();
                        break;
                    }
            }
        }

        #region Keyboard Functions

        #region Dictation

        /// <summary>
        /// Initialize dictation mode.
        /// </summary>
        private void BeginDictation()
        {
            ResetClosingTime();
            dictationSystem.StartRecording(gameObject);
            SetMicrophoneRecording();
        }

        private bool IsMicrophoneActive()
        {
            var result = _recordImage.color != _defaultColor;
            return result;
        }

        /// <summary>
        /// Set mike default look
        /// </summary>
        private void SetMicrophoneDefault()
        {
            _recordImage.color = _defaultColor;
        }

        /// <summary>
        /// Set mike recording look (red)
        /// </summary>
        private void SetMicrophoneRecording()
        {
            _recordImage.color = Color.red;
        }

        /// <summary>
        /// Terminate dictation mode.
        /// </summary>
        public void EndDictation()
        {
            dictationSystem.StopRecording();
            SetMicrophoneDefault();
        }

        #endregion Dictation

        /// <summary>
        /// Primary method for typing individual characters to a text field.
        /// </summary>
        /// <param name="valueKey">The valueKey of the pressed key.</param>
        public void AppendValue(KeyboardValueKey valueKey)
        {
            IndicateActivity();
            string value = "";

            // Shift value should only be applied if a shift value is present.
            if (m_IsShifted && !string.IsNullOrEmpty(valueKey.ShiftValue))
            {
                value = valueKey.ShiftValue;
            }
            else
            {
                value = valueKey.Value;
            }

            if (!m_IsCapslocked)
            {
                Shift(false);
            }

            m_CaretPosition = InputField.caretPosition;

            InputField.text = InputField.text.Insert(m_CaretPosition, value);
            m_CaretPosition += value.Length;

            UpdateCaretPosition(m_CaretPosition);
        }

        /// <summary>
        /// Trigger specific keyboard functionality.
        /// </summary>
        /// <param name="functionKey">The functionKey of the pressed key.</param>
        public void FunctionKey(KeyboardKeyFunc functionKey)
        {
            IndicateActivity();
            switch (functionKey.ButtonFunction)
            {
                case KeyboardKeyFunc.Function.Enter:
                    {
                        Enter();
                        break;
                    }

                case KeyboardKeyFunc.Function.Tab:
                    {
                        Tab();
                        break;
                    }

                case KeyboardKeyFunc.Function.ABC:
                    {
                        ActivateSpecificKeyboard(m_LastKeyboardLayout);
                        break;
                    }

                case KeyboardKeyFunc.Function.Symbol:
                    {
                        ActivateSpecificKeyboard(LayoutType.Symbol);
                        break;
                    }

                case KeyboardKeyFunc.Function.Previous:
                    {
                        MoveCaretLeft();
                        break;
                    }

                case KeyboardKeyFunc.Function.Next:
                    {
                        MoveCaretRight();
                        break;
                    }

                case KeyboardKeyFunc.Function.Close:
                    {
                        Close();
                        break;
                    }

                case KeyboardKeyFunc.Function.Dictate:
                    {
                        if (dictationSystem == null) { break; }

                        if (IsMicrophoneActive())
                        {
                            EndDictation();
                        }
                        else
                        {
                            BeginDictation();
                        }
                        break;
                    }

                case KeyboardKeyFunc.Function.Shift:
                    {
                        Shift(!m_IsShifted);
                        break;
                    }

                case KeyboardKeyFunc.Function.CapsLock:
                    {
                        CapsLock(!m_IsCapslocked);
                        break;
                    }

                case KeyboardKeyFunc.Function.Space:
                    {
                        Space();
                        break;
                    }

                case KeyboardKeyFunc.Function.Backspace:
                    {
                        Backspace();
                        break;
                    }

                case KeyboardKeyFunc.Function.UNDEFINED:
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
            // check if text is selected
            if (InputField.selectionFocusPosition != InputField.caretPosition || InputField.selectionAnchorPosition != InputField.caretPosition)
            {
                if (InputField.selectionAnchorPosition > InputField.selectionFocusPosition) // right to left
                {
                    InputField.text = InputField.text.Substring(0, InputField.selectionFocusPosition) + InputField.text.Substring(InputField.selectionAnchorPosition);
                    InputField.caretPosition = InputField.selectionFocusPosition;
                }
                else // left to right
                {
                    InputField.text = InputField.text.Substring(0, InputField.selectionAnchorPosition) + InputField.text.Substring(InputField.selectionFocusPosition);
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
                    InputField.text = InputField.text.Remove(m_CaretPosition, 1);
                    UpdateCaretPosition(m_CaretPosition);
                }
            }
        }

        /// <summary>
        /// Send the "previous" event.
        /// </summary>
        public void Previous()
        {
            OnPrevious(this, EventArgs.Empty);
        }

        /// <summary>
        /// Send the "next" event.
        /// </summary>
        public void Next()
        {
            OnNext(this, EventArgs.Empty);
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
                if (OnTextSubmitted != null)
                {
                    OnTextSubmitted(this, EventArgs.Empty);
                }

                Close();
            }
            else
            {
                string enterString = "\n";

                m_CaretPosition = InputField.caretPosition;

                InputField.text = InputField.text.Insert(m_CaretPosition, enterString);
                m_CaretPosition += enterString.Length;

                UpdateCaretPosition(m_CaretPosition);
            }

        }

        /// <summary>
        /// Set the keyboard to a single action shift state.
        /// </summary>
        /// <param name="newShiftState">value the shift key should have after calling the method</param>
        public void Shift(bool newShiftState)
        {
            m_IsShifted = newShiftState;
            OnKeyboardShifted(m_IsShifted);

            if (m_IsCapslocked && !newShiftState)
            {
                m_IsCapslocked = false;
            }
        }

        /// <summary>
        /// Set the keyboard to a permanent shift state.
        /// </summary>
        /// <param name="newCapsLockState">Caps lock state the method is switching to</param>
        public void CapsLock(bool newCapsLockState)
        {
            m_IsCapslocked = newCapsLockState;
            Shift(newCapsLockState);
        }

        /// <summary>
        /// Insert a space character.
        /// </summary>
        public void Space()
        {
            m_CaretPosition = InputField.caretPosition;
            InputField.text = InputField.text.Insert(m_CaretPosition++, " ");

            UpdateCaretPosition(m_CaretPosition);
        }

        /// <summary>
        /// Insert a tab character.
        /// </summary>
        public void Tab()
        {
            string tabString = "\t";

            m_CaretPosition = InputField.caretPosition;

            InputField.text = InputField.text.Insert(m_CaretPosition, tabString);
            m_CaretPosition += tabString.Length;

            UpdateCaretPosition(m_CaretPosition);
        }

        /// <summary>
        /// Move caret to the left.
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
        /// Move caret to the right.
        /// </summary>
        public void MoveCaretRight()
        {
            m_CaretPosition = InputField.caretPosition;

            if (m_CaretPosition < InputField.text.Length)
            {
                ++m_CaretPosition;
                UpdateCaretPosition(m_CaretPosition);
            }
        }

        /// <summary>
        /// Close the keyboard.
        /// (Clears all event subscriptions.)
        /// </summary>
        public void Close()
        {
            if (IsMicrophoneActive())
            {
                dictationSystem.StopRecording();
            }
            SetMicrophoneDefault();
            OnClosed(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Clear the text input field.
        /// </summary>
        public void Clear()
        {
            ResetKeyboardState();
            if (InputField.caretPosition != 0)
            {
                InputField.MoveTextStart(false);
            }
            InputField.text = "";
            m_CaretPosition = InputField.caretPosition;
        }

        #endregion

        /// <summary>
        /// Method to set the sizes by code, as the properties are private. 
        /// Useful for scaling 'from the outside', for instance taking care of differences between
        /// immersive headsets and HoloLens
        /// </summary>
        /// <param name="minScale">Min scale factor</param>
        /// <param name="maxScale">Max scale factor</param>
        /// <param name="minDistance">Min distance from camera</param>
        /// <param name="maxDistance">Max distance from camera</param>
        public void SetScaleSizeValues(float minScale, float maxScale, float minDistance, float maxDistance)
        {
            m_MinScale = minScale;
            m_MaxScale = maxScale;
            m_MinDistance = minDistance;
            m_MaxDistance = maxDistance;
        }

        #region Keyboard Layout Modes

        /// <summary>
        /// Enable the alpha keyboard.
        /// </summary>
        public void ShowAlphaKeyboard()
        {
            AlphaKeyboard.gameObject.SetActive(true);
            m_LastKeyboardLayout = LayoutType.Alpha;
        }

        /// <summary>
        /// Show the default subkeys only on the Alphanumeric keyboard.
        /// </summary>
        /// <returns>Returns true if default subkeys were activated, false if alphanumeric keyboard isn't active</returns>
        private bool TryToShowAlphaSubkeys()
        {
            if (AlphaKeyboard.IsActive())
            {
                AlphaSubKeys.gameObject.SetActive(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Show the email subkeys only on the Alphanumeric keyboard.
        /// </summary>
        /// <returns>Returns true if the email subkey was activated, false if alphanumeric keyboard is not active and key can't be activated</returns>
        private bool TryToShowEmailSubkeys()
        {
            if (AlphaKeyboard.IsActive())
            {
                AlphaMailKeys.gameObject.SetActive(true);
                m_LastKeyboardLayout = LayoutType.Email;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Show the URL subkeys only on the Alphanumeric keyboard.
        /// </summary>
        /// <returns>Returns true if the URL subkey was activated, false if alphanumeric keyboard is not active and key can't be activated</returns>
        private bool TryToShowURLSubkeys()
        {
            if (AlphaKeyboard.IsActive())
            {
                AlphaWebKeys.gameObject.SetActive(true);
                m_LastKeyboardLayout = LayoutType.URL;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Enable the symbol keyboard.
        /// </summary>
        public void ShowSymbolKeyboard()
        {
            SymbolKeyboard.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disable GameObjects for all keyboard elements.
        /// </summary>
        private void DisableAllKeyboards()
        {
            AlphaKeyboard.gameObject.SetActive(false);
            SymbolKeyboard.gameObject.SetActive(false);

            AlphaWebKeys.gameObject.SetActive(false);
            AlphaMailKeys.gameObject.SetActive(false);
            AlphaSubKeys.gameObject.SetActive(false);
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
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
            if (_audioSource != null)
            {
                _audioSource.Play();
            }
        }

        /// <summary>
        /// Reset inactivity closing timer
        /// </summary>
        private void ResetClosingTime()
        {
            if (CloseOnInactivity)
            {
                _closingTime = Time.time + CloseOnInactivityTime;
            }
        }

        /// <summary>
        /// Check if the keyboard has been left alone for too long and close
        /// </summary>
        private void CheckForCloseOnInactivityTimeExpired()
        {
            if (Time.time > _closingTime && CloseOnInactivity)
            {
                Close();
            }
        }
    }
}
