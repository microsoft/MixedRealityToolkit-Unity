// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input Manager is responsible for managing input sources and dispatching relevant events
    /// to the appropriate input handlers. 
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        /// <summary>
        /// To tap on a hologram even when not focused on,
        /// set OverrideFocusedObject to desired game object.
        /// If it's null, then focused object will be used.
        /// </summary>
        public GameObject OverrideFocusedObject { get; set; }

        public event Action InputEnabled;
        public event Action InputDisabled;

        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <summary>
        /// Global listeners listen to all events and ignore the fact that other components might have consumed them.
        /// </summary>
        private readonly List<GameObject> globalListeners = new List<GameObject>();

        private bool isRegisteredToGazeChanges;
        private int disabledRefCount;

        private InputEventData inputEventData;
        private InputClickedEventData sourceClickedEventData;
        private SourceStateEventData sourceStateEventData;
        private ManipulationEventData manipulationEventData;
        private HoldEventData holdEventData;
        private NavigationEventData navigationEventData;

#if UNITY_WSA || UNITY_STANDALONE_WIN
        private SpeechKeywordRecognizedEventData speechKeywordRecognizedEventData;
        private DictationEventData dictationEventData;
#endif

        /// <summary>
        /// Indicates if input is currently enabled or not.
        /// </summary>
        public bool IsInputEnabled
        {
            get { return disabledRefCount <= 0; }
        }

        /// <summary>
        /// Should the Unity UI events be fired?
        /// </summary>
        public bool ShouldSendUnityUiEvents { get { return GazeManager.Instance.UnityUIPointerEvent != null && EventSystem.current != null; } }

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushModalInputHandler(GameObject inputHandler)
        {
            modalInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        public void PopModalInputHandler()
        {
            modalInputStack.Pop();
        }

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        public void ClearModalInputStack()
        {
            modalInputStack.Clear();
        }

        /// <summary>
        /// Adds a global listener that will receive all input events, regardless
        /// of which other game objects might have handled the event beforehand.
        /// </summary>
        /// <param name="listener">Listener to add.</param>
        public void AddGlobalListener(GameObject listener)
        {
            globalListeners.Add(listener);
        }

        /// <summary>
        /// Removes a global listener.
        /// </summary>
        /// <param name="listener">Listener to remove.</param>
        public void RemoveGlobalListener(GameObject listener)
        {
            globalListeners.Remove(listener);
        }

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushFallbackInputHandler(GameObject inputHandler)
        {
            fallbackInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        public void PopFallbackInputHandler()
        {
            fallbackInputStack.Pop();
        }

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        public void ClearFallbackInputStack()
        {
            fallbackInputStack.Clear();
        }

        /// <summary>
        /// Push a disabled input state onto the input manager.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        public void PushInputDisable()
        {
            ++disabledRefCount;

            if (disabledRefCount == 1)
            {
                InputDisabled.RaiseEvent();
            }
        }

        /// <summary>
        /// Pop disabled input state. When the last disabled state is 
        /// popped off the stack input will be re-enabled.
        /// </summary>
        public void PopInputDisable()
        {
            --disabledRefCount;
            Debug.Assert(disabledRefCount >= 0, "Tried to pop more input disable than the amount pushed.");

            if (disabledRefCount == 0)
            {
                InputEnabled.RaiseEvent();
            }
        }

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable input.
        /// </summary>
        public void ClearInputDisableStack()
        {
            bool wasInputDisabled = disabledRefCount > 0;
            disabledRefCount = 0;

            if (wasInputDisabled)
            {
                InputEnabled.RaiseEvent();
            }
        }

        private void InitializeEventDatas()
        {
            inputEventData = new InputEventData(EventSystem.current);
            sourceClickedEventData = new InputClickedEventData(EventSystem.current);
            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);
            navigationEventData = new NavigationEventData(EventSystem.current);
            holdEventData = new HoldEventData(EventSystem.current);
#if UNITY_WSA || UNITY_STANDALONE_WIN
            speechKeywordRecognizedEventData = new SpeechKeywordRecognizedEventData(EventSystem.current);
            dictationEventData = new DictationEventData(EventSystem.current);
#endif
        }

        #region Unity Methods

        private void Start()
        {
            InitializeEventDatas();

            if (GazeManager.Instance == null)
            {
                Debug.LogError("InputManager requires an active GazeManager in the scene");
            }

            RegisterGazeManager();
        }

        private void OnEnable()
        {
            RegisterGazeManager();
        }

        private void OnDisable()
        {
            UnregisterGazeManager();
        }

        protected override void OnDestroy()
        {
            UnregisterGazeManager();
        }

        #endregion // Unity Methods

        public void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
            where T : IEventSystemHandler
        {
            if (!Instance.enabled || disabledRefCount > 0)
            {
                return;
            }

            // Use focused object when OverrideFocusedObject is null.
            GameObject focusedObject = (OverrideFocusedObject == null) ? GazeManager.Instance.HitObject : OverrideFocusedObject;

            // Send the event to global listeners
            for (int i = 0; i < globalListeners.Count; i++)
            {
                // Global listeners should only get events on themselves, as opposed to their hierarchy
                ExecuteEvents.Execute(globalListeners[i], eventData, eventHandler);
            }

            // Handle modal input if one exists
            if (modalInputStack.Count > 0)
            {
                GameObject modalInput = modalInputStack.Peek();

                // If there is a focused object in the hierarchy of the modal handler, start the event
                // bubble there
                if (focusedObject != null && modalInput != null && focusedObject.transform.IsChildOf(modalInput.transform))
                {
                    if (ExecuteEvents.ExecuteHierarchy(focusedObject, eventData, eventHandler))
                    {
                        return;
                    }
                }
                // Otherwise, just invoke the event on the modal handler itself
                else
                {
                    if (ExecuteEvents.ExecuteHierarchy(modalInput, eventData, eventHandler))
                    {
                        return;
                    }
                }
            }

            // If event was not handled by modal, pass it on to the current focused object
            if (focusedObject != null)
            {
                bool eventHandled = ExecuteEvents.ExecuteHierarchy(focusedObject, eventData, eventHandler);
                if (eventHandled)
                {
                    return;
                }
            }

            // If event was not handled by the focused object, pass it on to any fallback handlers
            if (fallbackInputStack.Count > 0)
            {
                GameObject fallbackInput = fallbackInputStack.Peek();
                ExecuteEvents.ExecuteHierarchy(fallbackInput, eventData, eventHandler);
            }
        }

        /// <summary>
        /// Register to gaze manager events.
        /// </summary>
        private void RegisterGazeManager()
        {
            if (!isRegisteredToGazeChanges && GazeManager.Instance != null)
            {
                GazeManager.Instance.FocusedObjectChanged += GazeManager_FocusedChanged;
                isRegisteredToGazeChanges = true;
            }
        }

        /// <summary>
        /// Unregister from gaze manager events.
        /// </summary>
        private void UnregisterGazeManager()
        {
            if (isRegisteredToGazeChanges && GazeManager.Instance != null)
            {
                GazeManager.Instance.FocusedObjectChanged -= GazeManager_FocusedChanged;
                isRegisteredToGazeChanges = false;
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusable> OnFocusEnterEventHandler =
            delegate (IFocusable handler, BaseEventData eventData)
            {
                handler.OnFocusEnter();
            };

        private static readonly ExecuteEvents.EventFunction<IFocusable> OnFocusExitEventHandler =
            delegate (IFocusable handler, BaseEventData eventData)
            {
                handler.OnFocusExit();
            };

        private void GazeManager_FocusedChanged(GameObject previousObject, GameObject newObject)
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            if (previousObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(previousObject, null, OnFocusExitEventHandler);
                if (ShouldSendUnityUiEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(previousObject, GazeManager.Instance.UnityUIPointerEvent, ExecuteEvents.pointerExitHandler);
                }
            }

            if (newObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newObject, null, OnFocusEnterEventHandler);
                if (ShouldSendUnityUiEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(newObject, GazeManager.Instance.UnityUIPointerEvent, ExecuteEvents.pointerEnterHandler);
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IInputClickHandler> OnInputClickedEventHandler =
            delegate (IInputClickHandler handler, BaseEventData eventData)
            {
                InputClickedEventData casted = ExecuteEvents.ValidateEventData<InputClickedEventData>(eventData);
                handler.OnInputClicked(casted);
            };

        public void RaiseInputClicked(IInputSource source, uint sourceId, int tapCount)
        {
            // Create input event
            sourceClickedEventData.Initialize(source, sourceId, tapCount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceClickedEventData, OnInputClickedEventHandler);

            // UI events
            if (ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerClickHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnSourceUpEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                InputEventData casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        public void RaiseSourceUp(IInputSource source, uint sourceId)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnSourceUpEventHandler);

            // UI events
            if (ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerUpHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnSourceDownEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                InputEventData casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        public void RaiseSourceDown(IInputSource source, uint sourceId)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnSourceDownEventHandler);

            // UI events
            if (ShouldSendUnityUiEvents)
            {
                PointerEventData unityUiPointerEvent = GazeManager.Instance.UnityUIPointerEvent;

                unityUiPointerEvent.eligibleForClick = true;
                unityUiPointerEvent.delta = Vector2.zero;
                unityUiPointerEvent.dragging = false;
                unityUiPointerEvent.useDragThreshold = true;
                unityUiPointerEvent.pressPosition = unityUiPointerEvent.position;
                unityUiPointerEvent.pointerPressRaycast = unityUiPointerEvent.pointerCurrentRaycast;

                HandleEvent(unityUiPointerEvent, ExecuteEvents.pointerDownHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceDetectedEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                SourceStateEventData casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        public void RaiseSourceDetected(IInputSource source, uint sourceId)
        {
            // Create input event
            sourceStateEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceLostEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                SourceStateEventData casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceLost(casted);
            };

        public void RaiseSourceLost(IInputSource source, uint sourceId)
        {
            // Create input event
            sourceStateEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);
        }

        #region Manipulation Events

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationStartedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationStarted(casted);
            };

        public void RaiseManipulationStarted(IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationUpdatedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationUpdated(casted);
            };

        public void RaiseManipulationUpdated(IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCompletedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCompleted(casted);
            };

        public void RaiseManipulationCompleted(IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCanceledEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCanceled(casted);
            };

        public void RaiseManipulationCanceled(IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        #endregion // Manipulation Events

        #region Hold Events

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldStartedEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldStarted(casted);
            };

        public void RaiseHoldStarted(IInputSource source, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, OnHoldStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCompletedEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldCompleted(casted);
            };

        public void RaiseHoldCompleted(IInputSource source, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, OnHoldCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCanceledEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldCanceled(casted);
            };

        public void RaiseHoldCanceled(IInputSource source, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, OnHoldCanceledEventHandler);
        }

        #endregion // Hold Events

        #region Navigation Events

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationStartedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationStarted(casted);
            };

        public void RaiseNavigationStarted(IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationUpdatedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationUpdated(casted);
            };

        public void RaiseNavigationUpdated(IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCompletedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCompleted(casted);
            };

        public void RaiseNavigationCompleted(IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCanceledEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCanceled(casted);
            };

        public void RaiseNavigationCanceled(IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

        #endregion // Navigation Events

#if UNITY_WSA || UNITY_STANDALONE_WIN
        #region Speech Events

        private static readonly ExecuteEvents.EventFunction<ISpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (ISpeechHandler handler, BaseEventData eventData)
            {
                SpeechKeywordRecognizedEventData casted = ExecuteEvents.ValidateEventData<SpeechKeywordRecognizedEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        public void RaiseSpeechKeywordPhraseRecognized(IInputSource source, uint sourceId, ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text)
        {
            // Create input event
            speechKeywordRecognizedEventData.Initialize(source, sourceId, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechKeywordRecognizedEventData, OnSpeechKeywordRecognizedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        #endregion // Speech Events

        #region Dictation Events

        public void RaiseDictationHypothesis(IInputSource source, uint sourceId, string dictationHypothesis, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, dictationHypothesis, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationHypothesisEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationResultEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        public void RaiseDictationResult(IInputSource source, uint sourceId, string dictationResult, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationResultEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationCompleteEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        public void RaiseDictationComplete(IInputSource source, uint sourceId, string dictationResult, AudioClip dictationAudioClip)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationCompleteEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationErrorEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };

        public void RaiseDictationError(IInputSource source, uint sourceId, string dictationResult, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion // Dictation Events
#endif
    }
}
