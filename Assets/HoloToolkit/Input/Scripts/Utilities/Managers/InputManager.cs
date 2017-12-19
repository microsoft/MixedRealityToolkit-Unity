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
        public event Action InputEnabled;
        public event Action InputDisabled;

        /// <summary>
        /// Global listeners listen to all events and ignore the fact that other components might have consumed them.
        /// </summary>
        private readonly List<GameObject> globalListeners = new List<GameObject>(0);
        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <summary>
        /// To tap on a hologram even when not focused on,
        /// set OverrideFocusedObject to desired game object.
        /// If it's null, then focused object will be used.
        /// </summary>
        public GameObject OverrideFocusedObject { get; set; }

        private int disabledRefCount;

        private FocusEventData focusEventData;
        private InputEventData inputEventData;
        private InputClickedEventData sourceClickedEventData;
        private SourceStateEventData sourceStateEventData;
        private ManipulationEventData manipulationEventData;
        private HoldEventData holdEventData;
        private NavigationEventData navigationEventData;
        private GamePadEventData gamePadEventData;
        private XboxControllerEventData xboxControllerEventData;
        private SourceRotationEventData sourceRotationEventData;
        private SourcePositionEventData sourcePositionEventData;
        private FocusChangedEventData focusChangedEventData;
        private InputPositionEventData inputPositionEventData;
        private SelectPressedEventData selectPressedEventData;
        private PointerInputEventData pointerInputEventData;
#if UNITY_WSA || UNITY_STANDALONE_WIN
        private SpeechEventData speechEventData;
        private DictationEventData dictationEventData;
#endif

        /// <summary>
        /// List of the input sources as detected by the input manager like hands or motion controllers.
        /// </summary>
        private readonly List<InputSourceInfo> detectedInputSources = new List<InputSourceInfo>(0);
        public List<InputSourceInfo> DetectedInputSources { get { return detectedInputSources; } }

        #region event origin

        /// <summary>
        /// Applies a string to the device associated with inputSourceId
        /// This string will accompany events raised by this device until RemoveEventOrigin is called
        /// </summary>
        /// <param name="inputSourceId"></param>
        /// <param name="eventOrigin"></param>
        public void ApplyEventOrigin(uint inputSourceId, string eventOrigin) {
            if (!userEventOriginDictionary.ContainsKey(inputSourceId)) {
                userEventOriginDictionary.Add(inputSourceId, eventOrigin);
            } else {
                userEventOriginDictionary[inputSourceId] = eventOrigin;
            }
        }

        /// <summary>
        /// Removes an event origin string applied via ApplyEventOrigin
        /// If leaveIfOriginIsDifferent is true, event origin string will not be removed unless it matches the original string
        /// (This is to prevent pointers from removing tags that don't belong to them)
        /// </summary>
        /// <param name="inputSourceId"></param>
        /// <param name="eventOrigin"></param>
        /// <param name="leaveIfOriginIsDifferent"></param>
        public void RemoveEventOrigin(uint inputSourceId, string eventOrigin, bool leaveIfOriginIsDifferent = true) {
            if (userEventOriginDictionary.ContainsKey(inputSourceId)) {
                if (!leaveIfOriginIsDifferent || userEventOriginDictionary[inputSourceId] == eventOrigin) {
                    userEventOriginDictionary.Remove(inputSourceId);
                }
            }
        }

        /// <summary>
        /// Returns the event origin associated with this input source, or the default if none is found
        /// </summary>
        /// <param name="inputSourceId"></param>
        /// <returns></returns>
        private string GetEventOrigin(uint inputSourceId) {
            if (userEventOriginDictionary.ContainsKey(inputSourceId)) {
                return userEventOriginDictionary[inputSourceId];
            } else {
                return defaultEventOrigin;
            }
        }

        /// <summary>
        /// User-defined strings set in ApplyEventOrigin
        /// </summary>
        private Dictionary<uint, string> userEventOriginDictionary = new Dictionary<uint, string>();

        [SerializeField]
        private string defaultEventOrigin = string.Empty;

        #endregion

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
        [Obsolete("Will be removed in a future release.  If you need to know if a specific pointer should send Unity UI Events use FocusManager.Instance.GetSpecificPointerEventData()!=null")]
        public bool ShouldSendUnityUiEvents { get { return FocusManager.Instance.GetGazePointerEventData() != null && EventSystem.current != null; } }

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
            if (modalInputStack.Count > 0)
            {
                modalInputStack.Pop();

            }
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
            focusEventData = new FocusEventData(EventSystem.current);
            inputEventData = new InputEventData(EventSystem.current);
            sourceClickedEventData = new InputClickedEventData(EventSystem.current);
            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);
            navigationEventData = new NavigationEventData(EventSystem.current);
            holdEventData = new HoldEventData(EventSystem.current);
            focusChangedEventData = new FocusChangedEventData(EventSystem.current);
            inputPositionEventData = new InputPositionEventData(EventSystem.current);
            selectPressedEventData = new SelectPressedEventData(EventSystem.current);
            sourceRotationEventData = new SourceRotationEventData(EventSystem.current);
            sourcePositionEventData = new SourcePositionEventData(EventSystem.current);
            gamePadEventData = new GamePadEventData(EventSystem.current);
            xboxControllerEventData = new XboxControllerEventData(EventSystem.current);
            pointerInputEventData = new PointerInputEventData(EventSystem.current);
#if UNITY_WSA || UNITY_STANDALONE_WIN
            speechEventData = new SpeechEventData(EventSystem.current);
            dictationEventData = new DictationEventData(EventSystem.current);
#endif
        }

        #region Unity APIs

        protected override void Awake()
        {
            base.Awake();
            InitializeEventDatas();
        }

        private void Start()
        {
            if (!FocusManager.IsInitialized)
            {
                Debug.LogError("InputManager requires an active FocusManager in the scene");
            }
        }

        #endregion // Unity APIs

        public void HandleEvent<T>(BaseInputEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            if (!Instance.enabled || disabledRefCount > 0)
            {
                return;
            }

            Debug.Assert(!eventData.used);

            // Send the event to global listeners
            for (int i = 0; i < globalListeners.Count; i++)
            {
                // Global listeners should only get events on themselves, as opposed to their hierarchy.
                ExecuteEvents.Execute(globalListeners[i], eventData, eventHandler);
            }

            if (eventData.used)
            {
                // All global listeners get a chance to see the event, but if any of them marked it used, we stop
                // the event from going any further.
                return;
            }

            // If we have an override focus object
            if (OverrideFocusedObject != null)
            {
                // Execute focus events on the override object only
                if (ExecuteFocusEvents<T>(eventData, eventHandler, OverrideFocusedObject))
                {
                    return;
                }
            }
            else
            {
                // Otherwise, check if we have focus targets from the focus manager
                List<IFocusTarget> currentFocusTargets = FocusManager.Instance.CurrentFocusTargets;

                if (currentFocusTargets.Count == 0)
                {
                    // If there are no focus targets, execute events on a null target so modal input still receives events
                    ExecuteFocusEvents<T>(eventData, eventHandler, null);
                }
                else
                {
                    // Otherwise execute events on each focus target
                    foreach (IFocusTarget target in currentFocusTargets)
                    {
                        // Skip any that don't have focus
                        // (IFocusTargets may override focus status via FocusEnabled)
                        if (!target.HasFocus)
                        {
                            continue;
                        }

                        // Check whether any of the focusers in the target own this event data
                        bool atLeastOneFocuserOwnsEvent = false;
                        foreach (IFocuser focuser in target.Focusers)
                        {
                            if (focuser.OwnsInput(eventData))
                            {
                                atLeastOneFocuserOwnsEvent = true;
                                break;
                            }
                        }

                        // If none own the event data, don't send the event to this target
                        if (!atLeastOneFocuserOwnsEvent)
                        {
                            continue;
                        }

                        GameObject focusedObject = target.gameObject;

                        if (ExecuteFocusEvents<T>(eventData, eventHandler, focusedObject))
                        {
                            // If executing the focus events consumes the event, we're done
                            return;
                        }
                    }
                }
            }

            // If event was not handled by the focused object, pass it on to any fallback handlers
            if (fallbackInputStack.Count > 0)
            {
                GameObject fallbackInput = fallbackInputStack.Peek();
                if (ExecuteEvents.ExecuteHierarchy(fallbackInput, eventData, eventHandler) && eventData.used)
                {
                    return;
                }
            }
        }

        private bool ExecuteFocusEvents<T>(BaseInputEventData eventData, ExecuteEvents.EventFunction<T> eventHandler, GameObject focusedObject) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);

            // Handle modal input if one exists
            if (modalInputStack.Count > 0)
            {
                GameObject modalInput = modalInputStack.Peek();

                // If there is a focused object in the hierarchy of the modal handler, start the event
                // bubble there
                if (focusedObject != null && modalInput != null && focusedObject.transform.IsChildOf(modalInput.transform))
                {
                    if (ExecuteEvents.ExecuteHierarchy(focusedObject, eventData, eventHandler) && eventData.used)
                    {
                        return true;
                    }
                }
                // Otherwise, just invoke the event on the modal handler itself
                else
                {
                    if (ExecuteEvents.ExecuteHierarchy(modalInput, eventData, eventHandler) && eventData.used)
                    {
                        return true;
                    }
                }
            }

            // If event was not handled by modal, pass it on to the current focused object
            if (focusedObject != null)
            {
                if (ExecuteEvents.ExecuteHierarchy(focusedObject, eventData, eventHandler) && eventData.used)
                {
                    return true;
                }
            }

            return false;
        }

        #region Focus Events

        public struct FocusEvent
        {
            public FocusEvent(IFocuser focuser, GameObject target)
            {
                Focuser = focuser;
                Target = target;
            }

            public IFocuser Focuser { get; private set; }
            public GameObject Target { get; private set; }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusTarget> OnFocusEnterEventHandlerInfo =
            delegate (IFocusTarget handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusEnter(casted);
            };

        public void RaiseFocusEnter(FocusEvent focusedEvent)
        {
            focusEventData.Initialize(focusedEvent.Focuser, focusedEvent.Target);

            ExecuteEvents.ExecuteHierarchy(focusedEvent.Target.gameObject, focusEventData, OnFocusEnterEventHandlerInfo);

            PointerInputEventData pointerInputEventData = FocusManager.Instance.GetGazePointerEventData();

            if (pointerInputEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(focusedEvent.Target.gameObject, pointerInputEventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusTarget> OnFocusExitEventHandlerInfo =
            delegate (IFocusTarget handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusExit(casted);
            };

        public void RaiseFocusExit(FocusEvent deFocusedEvent)
        {
            focusEventData.Initialize(deFocusedEvent.Focuser, deFocusedEvent.Target);

            ExecuteEvents.ExecuteHierarchy(deFocusedEvent.Target.gameObject, focusEventData, OnFocusExitEventHandlerInfo);

            PointerInputEventData pointerInputEventData = FocusManager.Instance.GetGazePointerEventData();

            if (pointerInputEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(deFocusedEvent.Target.gameObject, pointerInputEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusChangedHandler> OnFocusChangedEventHandler =
            delegate (IFocusChangedHandler handler, BaseEventData eventData)
            {
                FocusChangedEventData casted = ExecuteEvents.ValidateEventData<FocusChangedEventData>(eventData);
                handler.OnFocusChanged(casted);
            };

        /// <summary>
        /// Raise focus enter and exit events for when an input (that supports pointing) points to a game object.
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="oldFocusedObject"></param>
        /// <param name="newFocusedObject"></param>
        public void RaiseFocusChangedEvents(IFocuser focuser, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusChangedEventData.Initialize(focuser, oldFocusedObject, newFocusedObject);
            ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusChangedEventData, OnFocusChangedEventHandler);
        }

        #endregion // Focus Events

        #region Generic Input Events

        private static readonly ExecuteEvents.EventFunction<IInputClickHandler> OnInputClickedEventHandler =
            delegate (IInputClickHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputClickedEventData>(eventData);
                handler.OnInputClicked(casted);
            };

        public void RaiseInputClicked(IInputSource source, uint sourceId, InteractionSourcePressInfo pressType, int tapCount, object[] tag = null)
        {
            // Create input event
            sourceClickedEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), pressType, tapCount, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceClickedEventData, OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaiseSourceUp call the pointerClickHandler.
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnSourceUpEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        public void RaiseSourceUp(IInputSource source, uint sourceId, InteractionSourcePressInfo pressType, object[] tag = null)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), pressType, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnSourceUpEventHandler);

            // UI events
            IFocuser focuser;
            if (FocusManager.Instance.TryGetFocuser(inputEventData, out focuser))
            {
                if (pressType == InteractionSourcePressInfo.Select)
                {
                    pointerInputEventData.Reset();
                    pointerInputEventData.InputSource = source;
                    pointerInputEventData.SourceId = sourceId;

                    ExecuteEvents.ExecuteHierarchy(inputEventData.selectedObject, pointerInputEventData, ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.ExecuteHierarchy(inputEventData.selectedObject, pointerInputEventData, ExecuteEvents.pointerClickHandler);
                    pointerInputEventData.Clear();
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnSourceDownEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        public void RaiseSourceDown(IInputSource source, uint sourceId, InteractionSourcePressInfo pressType, object[] tag = null)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), pressType, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnSourceDownEventHandler);

            // UI events
            IFocuser focuser;
            if (FocusManager.Instance.TryGetFocuser(inputEventData, out focuser))
            {
                if (pressType == InteractionSourcePressInfo.Select)
                {
                    pointerInputEventData.Reset();
                    pointerInputEventData.InputSource = source;
                    pointerInputEventData.SourceId = sourceId;
                    pointerInputEventData.pointerId = (int)sourceId;

                    pointerInputEventData.eligibleForClick = true;
                    pointerInputEventData.delta = Vector2.zero;
                    pointerInputEventData.dragging = false;
                    pointerInputEventData.useDragThreshold = true;
                    pointerInputEventData.pressPosition = pointerInputEventData.position;
                    pointerInputEventData.pointerPressRaycast = pointerInputEventData.pointerCurrentRaycast;

                    ExecuteEvents.ExecuteHierarchy(inputEventData.selectedObject, pointerInputEventData, ExecuteEvents.pointerDownHandler);
                }
            }
        }

        #endregion // Generic Input Events

        #region Source State Events

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceDetectedEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        public void RaiseSourceDetected(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Manage list of detected sources
            bool alreadyDetected = false;

            for (int iDetected = 0; iDetected < detectedInputSources.Count; iDetected++)
            {
                if (detectedInputSources[iDetected].Matches(source, sourceId))
                {
                    alreadyDetected = true;
                    break;
                }
            }

            if (!alreadyDetected)
            {
                InputSourceInfo newInputSource = new InputSourceInfo(source, sourceId);
                detectedInputSources.Add(newInputSource);
            }

            // Create input event
            sourceStateEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceLostEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceLost(casted);
            };

        public void RaiseSourceLost(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Manage list of detected sources
            for (int iDetected = 0; iDetected < detectedInputSources.Count; iDetected++)
            {
                if (detectedInputSources[iDetected].Matches(source, sourceId))
                {
                    detectedInputSources.RemoveAt(iDetected);
                    break;
                }
            }

            // Create input event
            sourceStateEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);
        }

        #endregion

        #region Manipulation Events

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationStartedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationStarted(casted);
            };

        public void RaiseManipulationStarted(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), Vector3.zero, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationUpdatedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationUpdated(casted);
            };

        public void RaiseManipulationUpdated(IInputSource source, uint sourceId, Vector3 cumulativeDelta, object[] tag = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), cumulativeDelta, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCompletedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCompleted(casted);
            };

        public void RaiseManipulationCompleted(IInputSource source, uint sourceId, Vector3 cumulativeDelta, object[] tag = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), cumulativeDelta, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCanceledEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCanceled(casted);
            };

        public void RaiseManipulationCanceled(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), Vector3.zero, tag);

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

        public void RaiseHoldStarted(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, OnHoldStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCompletedEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldCompleted(casted);
            };

        public void RaiseHoldCompleted(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, OnHoldCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCanceledEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldCanceled(casted);
            };

        public void RaiseHoldCanceled(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), tag);

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

        public void RaiseNavigationStarted(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), Vector3.zero, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationUpdatedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationUpdated(casted);
            };

        public void RaiseNavigationUpdated(IInputSource source, uint sourceId, Vector3 normalizedOffset, object[] tag = null)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), normalizedOffset, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCompletedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCompleted(casted);
            };

        public void RaiseNavigationCompleted(IInputSource source, uint sourceId, Vector3 normalizedOffset, object[] tag = null)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), normalizedOffset, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCanceledEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCanceled(casted);
            };

        public void RaiseNavigationCanceled(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), Vector3.zero, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

        #endregion // Navigation Events

        #region Controller Events

        private static readonly ExecuteEvents.EventFunction<IControllerInputHandler> OnInputPositionChangedEventHandler =
            delegate (IControllerInputHandler handler, BaseEventData eventData)
            {
                InputPositionEventData casted = ExecuteEvents.ValidateEventData<InputPositionEventData>(eventData);
                handler.OnInputPositionChanged(casted);
            };

        public void RaiseInputPositionChanged(IInputSource source, uint sourceId, InteractionSourcePressInfo pressType, Vector2 position, object[] tag = null)
        {
            // Create input event
            inputPositionEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), pressType, position, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPositionEventData, OnInputPositionChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISelectHandler> OnSelectPressedAmountChangedEventHandler =
            delegate (ISelectHandler handler, BaseEventData eventData)
            {
                SelectPressedEventData casted = ExecuteEvents.ValidateEventData<SelectPressedEventData>(eventData);
                handler.OnSelectPressedAmountChanged(casted);
            };

        public void RaiseSelectPressedAmountChanged(IInputSource source, uint sourceId, double pressedAmount, object[] tag = null)
        {
            // Create input event
            selectPressedEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), pressedAmount, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(selectPressedEventData, OnSelectPressedAmountChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IControllerTouchpadHandler> OnTouchpadTouchedEventHandler =
            delegate (IControllerTouchpadHandler handler, BaseEventData eventData)
            {
                InputEventData casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnTouchpadTouched(casted);
            };

        public void RaiseTouchpadTouched(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), InteractionSourcePressInfo.Touchpad, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnTouchpadTouchedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IControllerTouchpadHandler> OnTouchpadReleasedEventHandler =
            delegate (IControllerTouchpadHandler handler, BaseEventData eventData)
            {
                InputEventData casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnTouchpadReleased(casted);
            };

        public void RaiseTouchpadReleased(IInputSource source, uint sourceId, object[] tag = null)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), InteractionSourcePressInfo.Touchpad, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnTouchpadReleasedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourcePositionHandler> OnSourcePositionChangedEventHandler =
            delegate (ISourcePositionHandler handler, BaseEventData eventData)
            {
                SourcePositionEventData casted = ExecuteEvents.ValidateEventData<SourcePositionEventData>(eventData);
                handler.OnPositionChanged(casted);
            };

        public void RaiseSourcePositionChanged(IInputSource source, uint sourceId, Vector3 pointerPosition, Vector3 gripPosition, object[] tag = null)
        {
            // Create input event
            sourcePositionEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), pointerPosition, gripPosition, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePositionEventData, OnSourcePositionChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceRotationHandler> OnSourceRotationChangedEventHandler =
            delegate (ISourceRotationHandler handler, BaseEventData eventData)
            {
                SourceRotationEventData casted = ExecuteEvents.ValidateEventData<SourceRotationEventData>(eventData);
                handler.OnRotationChanged(casted);
            };

        public void RaiseSourceRotationChanged(IInputSource source, uint sourceId, Quaternion pointerRotation, Quaternion gripRotation, object[] tag = null)
        {
            // Create input event
            sourceRotationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), pointerRotation, gripRotation, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceRotationEventData, OnSourceRotationChangedEventHandler);
        }

        #endregion // Controller Events

        #region GamePad Events

        private static readonly ExecuteEvents.EventFunction<IGamePadHandler> OnGamePadDetectedEventHandler =
            delegate (IGamePadHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<GamePadEventData>(eventData);
                handler.OnGamePadDetected(casted);
            };

        public void RaiseGamePadDetected(IInputSource source, uint sourceId, string gamePadName, object[] tag = null)
        {
            // Create input event
            gamePadEventData.Initialize(source, sourceId, gamePadName, GetEventOrigin(sourceId), tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(gamePadEventData, OnGamePadDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IGamePadHandler> OnGamePadLostEventHandler =
            delegate (IGamePadHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<GamePadEventData>(eventData);
                handler.OnGamePadLost(casted);
            };

        public void RaiseGamePadLost(IInputSource source, uint sourceId, string gamePadName, object[] tag = null)
        {
            // Create input event
            gamePadEventData.Initialize(source, sourceId, gamePadName, GetEventOrigin(sourceId), tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(gamePadEventData, OnGamePadLostEventHandler);
        }

        #region Xbox Controller Events

        private static readonly ExecuteEvents.EventFunction<IXboxControllerHandler> OnXboxAxisUpdateHandler =
            delegate (IXboxControllerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<XboxControllerEventData>(eventData);
                handler.OnXboxAxisUpdate(casted);
            };

        public void RaiseXboxInputUpdate(IInputSource source, uint sourceId, XboxControllerData inputData, object[] tag = null)
        {
            // Create input event
            xboxControllerEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), inputData, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(xboxControllerEventData, OnXboxAxisUpdateHandler);
        }

        #endregion // Xbox Controller Events

        #endregion // GamePad Events

#if UNITY_WSA || UNITY_STANDALONE_WIN
        #region Speech Events

        private static readonly ExecuteEvents.EventFunction<ISpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (ISpeechHandler handler, BaseEventData eventData)
            {
                SpeechEventData casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        public void RaiseSpeechKeywordPhraseRecognized(IInputSource source, uint sourceId, ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text, object[] tag = null)
        {
            // Create input event
            speechEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), confidence, phraseDuration, phraseStartTime, semanticMeanings, text, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechEventData, OnSpeechKeywordRecognizedEventHandler);
        }

        #endregion // Speech Events

        #region Dictation Events

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        public void RaiseDictationHypothesis(IInputSource source, uint sourceId, string dictationHypothesis, AudioClip dictationAudioClip = null, object[] tag = null)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), dictationHypothesis, dictationAudioClip, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationHypothesisEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationResultEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        public void RaiseDictationResult(IInputSource source, uint sourceId, string dictationResult, AudioClip dictationAudioClip = null, object[] tag = null)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), dictationResult, dictationAudioClip, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationResultEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationCompleteEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        public void RaiseDictationComplete(IInputSource source, uint sourceId, string dictationResult, AudioClip dictationAudioClip, object[] tag = null)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), dictationResult, dictationAudioClip, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationCompleteEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationErrorEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                DictationEventData casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };

        public void RaiseDictationError(IInputSource source, uint sourceId, string dictationResult, AudioClip dictationAudioClip = null, object[] tag = null)
        {
            // Create input event
            dictationEventData.Initialize(source, sourceId, GetEventOrigin(sourceId), dictationResult, dictationAudioClip, tag);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion // Dictation Events
#endif
    }
}
