// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

#if UNITY_WSA
using UnityEngine.Windows.Speech;
#endif
using UnityEngine.EventSystems;

namespace HoloToolkit.UI.Keyboard
{
	/// <summary>
	/// A simple general use keyboard that is ideal for AR/VR applications.
	/// </summary>
	/// 
	/// NOTE: This keyboard will not automatically appear when you select an InputField in your
	///       Canvas. In order for the keyboard to appear you must call Keyboard.Instance.PresentKeyboard(string).
	///       To retrieve the input from the Keyboard, subscribe to the textEntered event. Note that
	///       tapping 'Close' on the Keyboard will not fire the textEntered event. You must tap 'Enter' to
	///       get the textEntered event.
	public class Keyboard : MonoBehaviour
	{
		public enum LayoutType
		{
			Alpha,
			Symbol,
			URL,
			email,
		}

		#region Callbacks

		/// <summary>
		/// Sent when the 'Enter' button is pressed. To retrieve the text from the event,
		/// cast the sender to 'Keyboard' and get the text from the TextInput field.
		/// (Cleared when keyboard is closed.)
		/// </summary>
		public event EventHandler onTextSubmitted = delegate { };

		/// <summary>
		/// Fired every time the text in the InputField changes.
		/// (Cleared when keyboard is closed.)
		/// </summary>
		public event Action<string> onTextUpdated = delegate { };

		/// <summary>
		/// Fired every time the Close button is pressed.
		/// (Cleared when keyboard is closed.)
		/// </summary>
		public event EventHandler onClosed = delegate { };

		/// <summary>
		/// Sent when the 'Previous' button is pressed. Ideally you would use this event
		/// to set your targeted text input to the previous text field in your document.
		/// (Cleared when keyboard is closed.)
		/// </summary>
		public event EventHandler onPrevious = delegate { };

		/// <summary>
		/// Sent when the 'Next' button is pressed. Ideally you would use this event
		/// to set your targeted text input to the next text field in your document.
		/// (Cleared when keyboard is closed.)
		/// </summary>
		public event EventHandler onNext = delegate { };

		/// <summary>
		/// Sent when the keyboard is placed.  This allows listener to know when someone else is co-opting the keyboard.
		/// </summary>
		public event EventHandler onPlacement = delegate { };

		#endregion Callbacks

		/// <summary>
		/// The InputField that the keyboard uses to show the currently edited text.
		/// If you are using the Keyboard prefab you can ignore this field as it will
		/// be already assigned.
		/// </summary>
		public InputField m_InputField = null;

		// TODO Update comment when InputFieldSlide is implemented
		/// <summary>
		/// Currently not used. The idea is to move the input field back and forth in sync
		/// with your gaze's horizontal position on the keyboard.
		/// </summary>
		public AxisSlider m_InputFieldSlide = null;

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

		[Header("Positioning")]
		/// <summary>
		/// The scale the keyboard should be at its maximum distance.
		/// </summary>
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
		/// Event fired when shift key on keyboard is pressed.
		/// </summary>
		public event Action<bool> onKeyboardShifted = delegate { };

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

#if UNITY_WSA
		/// <summary>
		/// Reference to dictation recognizer.
		/// </summary>
		private DictationRecognizer m_Dictation = null;
#endif

		/// <summary>
		/// The starting scale of the keyboard.
		/// </summary>
		private Vector3 m_StartingScale = Vector3.one;

		/// <summary>
		/// The default bounds of the keyboard.
		/// </summary>
		private Vector3 m_ObjectBounds;

		/// <summary>
		/// Reference to instance of keyboard script.
		/// </summary>
		static private Keyboard s_Keyboard;

        /// <summary>
        /// Gets or Creates the Keyboard to use.
        /// </summary>
        static public Keyboard Instance
		{
			get
			{
				if (s_Keyboard == null)
				{
					Debug.LogError("Keyboard does not exist in the scene.  Drag the Keyboard prefab into the scene for the keyboard to work.");
				}

				return s_Keyboard;
			}
		}

		/// <summary>
		/// Deactivate on Awake.
		/// </summary>
		private void Awake()
		{
			if (s_Keyboard == null)
			{
				s_Keyboard = this;
			}

			m_StartingScale = this.transform.localScale;
			Bounds canvasBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this.transform);

			RectTransform rect = GetComponent<RectTransform>();
			m_ObjectBounds = new Vector3(canvasBounds.size.x * rect.localScale.x, canvasBounds.size.y * rect.localScale.y, canvasBounds.size.z * rect.localScale.z);

            // Keep keyboard deactivated until needed
            gameObject.SetActive(false);
		}

		/// <summary>
		/// Set up Dictation, CanvasEX, and automatically select the TextInput object.
		/// </summary>
		private void Start()
		{
			// Delegate Subscription
			m_InputField.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(DoTextUpdated));
		}

		/// <summary>
		/// Intermediary function for text update events.
		/// Workaround for strange leftover reference when unsubscribing.
		/// </summary>
		/// <param name="value">String value.</param>
		private void DoTextUpdated(string value)
		{
			if (onTextUpdated != null)
			{
				onTextUpdated(value);
			}
		}

		/// <summary>
		/// Makes sure the input field is always selected while the keyboard is up.
		/// </summary>
		private void Update()
		{
			if (EventSystem.current.currentSelectedGameObject != m_InputField.gameObject)
			{
				m_InputField.Select();
				m_InputField.ActivateInputField();
				m_InputField.OnPointerClick(new PointerEventData(EventSystem.current));

				StartCoroutine(WaitToUpdateInputField());
			}

            // Axis Slider
            bool updateSliderPos = GazeManager.Instance.HitObject == this.gameObject;
			if (updateSliderPos)
			{
				Vector3 relPos = this.transform.InverseTransformPoint(GazeManager.Instance.HitPosition);
				m_InputFieldSlide.TargetPoint = relPos;
			}
		}

		/// <summary>
		/// Called whenever the keyboard is disabled or deactivated.
		/// </summary>
		private void OnDisable()
		{
			m_LastKeyboardLayout = LayoutType.Alpha;
			Clear();
		}

		/// <summary>
		/// Workaround for unity bug in InputField.
		/// Waits for a frame before updating the caret position to remove highlight.
		/// </summary>
		/// <returns></returns>
		private IEnumerator WaitToUpdateInputField()
		{
			yield return null;

			m_InputField.caretPosition = m_CaretPosition;
			m_InputField.ForceLabelUpdate();
		}

#if UNITY_WSA
		/// <summary>
		/// Event fired when dictation completed.
		/// </summary>
		/// <param name="text">Speech to Text</param>
		/// <param name="confidence">Confidence dictation has in it's translation.</param>
		private void OnDictationResult(string text, ConfidenceLevel confidence)
		{
			if (text != null)
			{
				m_InputField.text.Insert(m_InputField.caretPosition, text);
				m_InputField.caretPosition += text.Length;
			}
		}
#endif

		/// <summary>
		/// Destroy unmanaged memory links.
		/// </summary>
		private void OnDestroy()
		{
#if UNITY_WSA
			// HACK Must manually call destroy on DictationRecognizer according to the documentation.
			if (m_Dictation != null)
			{
				m_Dictation.Dispose();
				m_Dictation = null;
			}
#endif
		}

		#region Present Functions

		/// <summary>
		/// Present the default keyboard to the camera.
		/// </summary>
		public void PresentKeyboard()
		{
            this.gameObject.SetActive(true);
            ActivateSpecificKeyboard(LayoutType.Alpha);

			onPlacement(this, EventArgs.Empty);
		}

		/// <summary>
		/// Presents the default keyboard to the camera, with start text.
		/// </summary>
		/// <param name="startText">The initial text to show in the Keyboard's InputField.</param>
		public void PresentKeyboard(string startText)
		{
			PresentKeyboard();
			Clear();
			m_InputField.text = startText;
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
		/// <param name="startText">The initial text to show in the Keyboard's InputField.</param>
		/// <param name="keyboardType">Specify the keyboard type.</param>
		public void PresentKeyboard(string startText, LayoutType keyboardType)
		{
			PresentKeyboard(startText);
			ActivateSpecificKeyboard(keyboardType);
		}

        #endregion Present Functions
        public void RepositionKeyboard(Vector3 kbPos, float verticalOffset = 0.0f)
        {
            this.transform.position = kbPos;
            ScaleToSize();
            LookAtTargetOrigin();
        }


        public void RepositionKeyboard(Transform objectTransform, BoxCollider aCollider = null, float verticalOffset = 0.0f)
        {
            this.transform.position = objectTransform.position;

            if (aCollider != null)
            {
                float yTranslation = -((aCollider.bounds.size.y * 0.5f) + verticalOffset);
                this.transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }
            else
            {
                float yTranslation = -((m_ObjectBounds.y * 0.5f) + verticalOffset);
                this.transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }


            ScaleToSize();

            LookAtTargetOrigin();
        }

        private void ScaleToSize()
        {
            float distance = (this.transform.position - Camera.main.transform.position).magnitude;
            float distancePercent = (distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
            float scale = m_MinScale + (m_MaxScale - m_MinScale) * distancePercent;
            scale = Mathf.Clamp(scale, m_MinScale, m_MaxScale);
            this.transform.localScale = m_StartingScale * scale;

            Debug.LogFormat("Setting scale: {0} for distance: {1}", scale, distance);
        }

        private void LookAtTargetOrigin()
        {
            this.transform.LookAt(Camera.main.transform.position);
            this.transform.Rotate(Vector3.up, 180.0f);
        }

        /// <summary>
        /// Activates a specific keyboard layout, and any sub keys.
        /// </summary>
        /// <param name="keyboardType"></param>
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

				case LayoutType.email:
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
#if UNITY_WSA
            if (m_Dictation == null)
            {
                m_Dictation = new DictationRecognizer();
                m_Dictation.DictationResult += OnDictationResult;
            }

            m_Dictation.Start();
#endif
		}

		/// <summary>
		/// Terminate dictation mode.
		/// </summary>
		/// TODO: Something needs to call this.
		public void EndDictation()
		{
#if UNITY_WSA
			if (m_Dictation.Status == SpeechSystemStatus.Running)
			{
				m_Dictation.Stop();
			}
#endif
		}

		#endregion Dictation

		/// <summary>
		/// Primary method for typing individual characters to a text field.
		/// </summary>
		/// <param name="valueKey">The valuekey of the pressed key.</param>
		public void AppendValue(KeyboardValueKey valueKey)
		{
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

			m_InputField.text = m_InputField.text.Insert(m_CaretPosition, value);
			m_CaretPosition += value.Length;
			m_InputField.caretPosition = m_CaretPosition;
		}

		/// <summary>
		/// Trigger specific keyboard functionality.
		/// </summary>
		/// <param name="functionKey">The functionkey of the pressed key.</param>
		public void FunctionKey(KeyboardKeyFunc functionKey)
		{
			switch (functionKey.m_ButtonFunction)
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
					BeginDictation();
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

				default:
				case KeyboardKeyFunc.Function.UNDEFINED:
				{
					Debug.LogErrorFormat("The {0} key on this keyboard hasn't been assigned a function.", functionKey.name);
					break;
				}
			}
		}

		/// <summary>
		/// Delete the character before the caret.
		/// </summary>
		public void Backspace()
		{
			if (m_CaretPosition > 0)
			{
				--m_CaretPosition;
				m_InputField.text = m_InputField.text.Remove(m_CaretPosition, 1);
				m_InputField.caretPosition = m_CaretPosition;
			}
		}

		/// <summary>
		/// Send the "previous" event.
		/// </summary>
		public void Previous()
		{
			onPrevious(this, EventArgs.Empty);
		}

		/// <summary>
		/// Send the "next" event.
		/// </summary>
		public void Next()
		{
			onNext(this, EventArgs.Empty);
		}

		/// <summary>
		/// Fire the text entered event for objects listening to keyboard.
		/// Immediately closes keyboard.
		/// </summary>
		public void Enter()
		{
			// Send text entered event and close the keyboard
			if (onTextSubmitted != null)
			{
				onTextSubmitted(this, EventArgs.Empty);
			}

			Close();
		}

		/// <summary>
		/// Set the keyboard to a single action sift state.
		/// </summary>
		/// <param name="newShiftState"></param>
		public void Shift(bool newShiftState)
		{
			m_IsShifted = newShiftState;
			onKeyboardShifted(m_IsShifted);

			if (m_IsCapslocked && !newShiftState)
			{
				m_IsCapslocked = false;
			}
		}

		/// <summary>
		/// Set the keyboard to a perminant shift state.
		/// </summary>
		/// <param name="newCapsLockState"></param>
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
			m_InputField.text = m_InputField.text.Insert(m_CaretPosition++, " ");
			m_InputField.caretPosition = m_CaretPosition;
		}

		/// <summary>
		/// Insert a tab character.
		/// </summary>
		public void Tab()
		{
			string tabString = "\t";
			m_InputField.text = m_InputField.text.Insert(m_CaretPosition, tabString);
			m_CaretPosition += tabString.Length;
			m_InputField.caretPosition = m_CaretPosition;
		}

		/// <summary>
		/// Move caret to the left.
		/// </summary>
		public void MoveCaretLeft()
		{
			if (m_CaretPosition > 0)
			{
				--m_CaretPosition;
				m_InputField.caretPosition = m_CaretPosition;
			}
		}

		/// <summary>
		/// Move caret to the right.
		/// </summary>
		public void MoveCaretRight()
		{
			if (m_CaretPosition < m_InputField.text.Length)
			{
				++m_CaretPosition;
				m_InputField.caretPosition = m_CaretPosition;
			}
		}

		/// <summary>
		/// Close the keyboard.
		/// (Clears all event subscriptions.)
		/// </summary>
		public void Close()
		{
			onClosed(this, EventArgs.Empty);

			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// Clear the text input field.
		/// </summary>
		public void Clear()
		{
			ResetKeyboardState();
			m_InputField.MoveTextStart(false);
			m_InputField.text = "";
			m_CaretPosition = m_InputField.caretPosition;
		}

		#endregion

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
		/// <returns></returns>
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
		/// <returns></returns>
		private bool TryToShowEmailSubkeys()
		{
			if (AlphaKeyboard.IsActive())
			{
				AlphaMailKeys.gameObject.SetActive(true);
				m_LastKeyboardLayout = LayoutType.email;
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
		/// <returns></returns>
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
		/// Disable gameobjects for all keyboard elements.
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
	}
}