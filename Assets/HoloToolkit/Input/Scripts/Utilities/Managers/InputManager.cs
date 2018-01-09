// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

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
        /// To tap on a hologram even when not focused on,
        /// set OverrideFocusedObject to desired game object.
        /// If it's null, then focused object will be used.
        /// </summary>
        public GameObject OverrideFocusedObject { get; set; }

        /// <summary>
        /// Global listeners listen to all events and ignore the fact that other components might have consumed them.
        /// </summary>
        private readonly List<GameObject> globalListeners = new List<GameObject>(0);

        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();

        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        private static readonly HashSet<IInputSource> detectedInputSources = new HashSet<IInputSource>();

        /// <summary>
        /// List of the Interaction Input Sources as detected by the input manager like hands or motion controllers.
        /// </summary>
        public static HashSet<IInputSource> DetectedInputSources { get { return detectedInputSources; } }

        /// <summary>
        /// Indicates if input is currently enabled or not.
        /// </summary>
        public bool IsInputEnabled
        {
            get { return disabledRefCount <= 0; }
        }

        private int disabledRefCount;

        private SourceStateEventData sourceStateEventData;
        private SourcePositionEventData sourcePositionEventData;
        private SourceRotationEventData sourceRotationEventData;

        private ClickEventData clickEventData;
        private FocusEventData focusEventData;

        private InputEventData inputEventData;
        private InputPressedEventData inputPressedEventData;
        private InputPositionEventData inputPositionEventData;

        private NavigationEventData navigationEventData;
        private ManipulationEventData manipulationEventData;

#if UNITY_WSA || UNITY_STANDALONE_WIN
        private SpeechEventData speechEventData;
        private DictationEventData dictationEventData;
#endif

        private void InitializeEventDatas()
        {
            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            sourcePositionEventData = new SourcePositionEventData(EventSystem.current);
            sourceRotationEventData = new SourceRotationEventData(EventSystem.current);

            clickEventData = new ClickEventData(EventSystem.current);
            focusEventData = new FocusEventData(EventSystem.current);

            inputEventData = new InputEventData(EventSystem.current);
            inputPressedEventData = new InputPressedEventData(EventSystem.current);
            inputPositionEventData = new InputPositionEventData(EventSystem.current);

            navigationEventData = new NavigationEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);

#if UNITY_WSA || UNITY_STANDALONE_WIN
            speechEventData = new SpeechEventData(EventSystem.current);
            dictationEventData = new DictationEventData(EventSystem.current);
#endif
        }

        #region Monobehavior Implementations

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

        #endregion Monobehavior Implementations

        #region Input Disabled Options

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

        #endregion Input Disabled Options

        #region Global Listener Options

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

        #endregion

        #region Modal Input Options

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

        #endregion

        #region Fallback Input Handler Options

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

        #endregion Fallback Input Handler Options

        #region Input Handlers

        /// <summary>
        /// The main function for handling and forwarding all events to their intended recipients.
        /// <para><remarks>See: https://docs.unity3d.com/Manual/MessagingSystem.html </remarks></para>
        /// </summary>
        /// <typeparam name="T">Event Handler Interface Type</typeparam>
        /// <param name="eventData">Event Data</param>
        /// <param name="eventHandler">Event Handler delegate</param>
        public void HandleEvent<T>(BaseInputEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            if (!Instance.enabled || disabledRefCount > 0)
            {
                return;
            }

            Debug.Assert(!eventData.used);

            // Use focused object when OverrideFocusedObject is null.
            GameObject focusedObject = (OverrideFocusedObject == null)
                    ? FocusManager.Instance.TryGetFocusedObject(eventData)
                    : OverrideFocusedObject;

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

            // TODO: robertes: consider whether modal and fallback input should flow to each handler until used
            //       or it should flow to just the topmost handler on the stack as it does today.

            // Handle modal input if one exists
            if (modalInputStack.Count > 0)
            {
                GameObject modalInput = modalInputStack.Peek();

                // If there is a focused object in the hierarchy of the modal handler, start the event bubble there
                if (focusedObject != null && modalInput != null && focusedObject.transform.IsChildOf(modalInput.transform))
                {
                    if (ExecuteEvents.ExecuteHierarchy(focusedObject, eventData, eventHandler) && eventData.used)
                    {
                        return;
                    }
                }
                // Otherwise, just invoke the event on the modal handler itself
                else
                {
                    if (ExecuteEvents.ExecuteHierarchy(modalInput, eventData, eventHandler) && eventData.used)
                    {
                        return;
                    }
                }
            }

            // If event was not handled by modal, pass it on to the current focused object
            if (focusedObject != null)
            {
                if (ExecuteEvents.ExecuteHierarchy(focusedObject, eventData, eventHandler) && eventData.used)
                {
                    return;
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

        #region Input Source Events

        /// <summary>
        /// Generates a new unique source id.
        /// </summary>
        /// <returns></returns>
        public static uint GenerateNewSourceId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var inputSource in detectedInputSources)
            {
                if (inputSource.SourceId == newId)
                {
                    return GenerateNewSourceId();
                }
            }

            return newId;
        }

        public void RaiseSourceDetected(IInputSource source, object[] tags = null)
        {
            Debug.Assert(!detectedInputSources.Contains(source), "This Input Source has already been registered!");

            detectedInputSources.Add(source);

            // Create input event
            sourceStateEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceDetectedEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        public void RaiseSourceLost(IInputSource source, object[] tags = null)
        {
            Debug.Assert(detectedInputSources.Contains(source), "This Input Source was never registered!");

            detectedInputSources.Remove(source);

            // Create input event
            sourceStateEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceLostEventHandler =
                delegate (ISourceStateHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                    handler.OnSourceLost(casted);
                };

        public void RaiseSourcePositionChanged(IInputSource source, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null)
        {
            // Create input event
            sourcePositionEventData.Initialize(source, pointerPosition, gripPosition, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePositionEventData, OnSourcePositionChangedEventHandler);
        }

        public void RaiseSourcePositionChanged(IInputSource source, Vector3 pointerPosition, Vector3 gripPosition, Handedness sourceHandedness, object[] tags = null)
        {
            // Create input event
            sourcePositionEventData.Initialize(source, pointerPosition, gripPosition, sourceHandedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePositionEventData, OnSourcePositionChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourcePositionChangedEventHandler =
                delegate (ISourceStateHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<SourcePositionEventData>(eventData);
                    handler.OnSourcePositionChanged(casted);
                };

        public void RaiseSourceRotationChanged(IInputSource source, Quaternion pointerRotation, Quaternion gripRotation, Handedness sourceHandedness, object[] tags = null)
        {
            // Create input event
            sourceRotationEventData.Initialize(source, pointerRotation, gripRotation, sourceHandedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceRotationEventData, OnSourceRotationChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceRotationChangedEventHandler =
                delegate (ISourceStateHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<SourceRotationEventData>(eventData);
                    handler.OnSourceRotationChanged(casted);
                };

        #endregion Input Source State Events

        #region Focus Events

        /// <summary>
        /// Raise the event OnFocusChanged to the game object when focus enters it.
        /// </summary>
        /// <param name="pointer">The pointer that focused the GameObject.</param>
        /// <param name="focusedObject">The GameObject that is focused.</param>
        public void RaiseFocusEnter(IPointingSource pointer, GameObject focusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, OnFocusEnterEventHandler);

            var graphicEventData = FocusManager.Instance.GetSpecificPointerGraphicEventData(pointer);
            if (graphicEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicEventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusHandler> OnFocusEnterEventHandler =
                delegate (IFocusHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnFocusEnter(casted);
                };

        /// <summary>
        /// Raise the event OnFocusExit to the game object when focus exists it.
        /// </summary>
        /// <param name="pointer">The pointer that unfocused the GameObject.</param>
        /// <param name="unfocusedObject">The GameObject that is unfocused.</param>
        public void RaiseFocusExit(IPointingSource pointer, GameObject unfocusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, OnFocusExitEventHandler);

            var graphicEventData = FocusManager.Instance.GetSpecificPointerGraphicEventData(pointer);
            if (graphicEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(unfocusedObject, graphicEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusHandler> OnFocusExitEventHandler =
                delegate (IFocusHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnFocusExit(casted);
                };

        /// <summary>
        /// Raise focus enter and exit events for when an input (that supports pointing) points to a game object.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="oldFocusedObject"></param>
        /// <param name="newFocusedObject"></param>
        public void RaisePointerSpecificFocusChangedEvents(IPointingSource pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.Cursor != null)
            {
                ExecuteEvents.ExecuteHierarchy(pointer.Cursor.gameObject, focusEventData, OnPointerSpecificFocusChangedEventHandler);
            }

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnPointerSpecificFocusChangedEventHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnPointerSpecificFocusChangedEventHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusHandler> OnPointerSpecificFocusChangedEventHandler =
                delegate (IFocusHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnFocusChanged(casted);
                };

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        private static readonly ExecuteEvents.EventFunction<IPointerHandler> OnPointerDownEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ClickEventData>(eventData);
                handler.OnPointerDown(casted);
            };

        private void ExecutePointerDown(GraphicInputEventData graphicInputEventData)
        {
            if (graphicInputEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(clickEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerDownHandler);
            }
        }

        private GraphicInputEventData HandlePointerDown(IPointingSource pointingSource)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(clickEventData, OnPointerDownEventHandler);

            return FocusManager.Instance.GetSpecificPointerGraphicEventData(pointingSource);
        }

        public void RaisePointerDown(IPointingSource source, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, tags);

            ExecutePointerDown(HandlePointerDown(source));
        }

        public void RaisePointerDown(IPointingSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, handedness, tags);

            ExecutePointerDown(HandlePointerDown(source));
        }

#if UNITY_WSA
        public void RaisePointerDown(IPointingSource source, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, pressType, handedness, tags);

            if (pressType == InteractionSourcePressType.Select)
            {
                ExecutePointerDown(HandlePointerDown(source));
            }
        }
#endif

        #endregion Pointer Down

        #region Pointer Click

        private static readonly ExecuteEvents.EventFunction<IPointerHandler> OnInputClickedEventHandler =
                delegate (IPointerHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<ClickEventData>(eventData);
                    handler.OnPointerClicked(casted);
                };

        private void HandleClick()
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(clickEventData, OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaisePointerUp call the pointerClickHandler.
        }

        public void RaiseInputClicked(IPointingSource source, int tapCount, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, tapCount, tags);

            HandleClick();
        }

        public void RaiseInputClicked(IPointingSource source, int tapCount, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, tapCount, handedness, tags);

            HandleClick();
        }

#if UNITY_WSA
        public void RaiseInputClicked(IPointingSource source, int tapCount, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, tapCount, pressType, handedness, tags);

            HandleClick();
        }
#endif

        #endregion Pointer Click

        #region Pointer Up

        private static readonly ExecuteEvents.EventFunction<IPointerHandler> OnPointerUpEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ClickEventData>(eventData);
                handler.OnPointerUp(casted);
            };

        private void ExecutePointerUp(GraphicInputEventData graphicInputEventData)
        {
            if (graphicInputEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(clickEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.ExecuteHierarchy(clickEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerClickHandler);
                graphicInputEventData.Clear();
            }
        }

        private GraphicInputEventData HandlePointerUp(IPointingSource pointingSource)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(clickEventData, OnPointerUpEventHandler);

            return FocusManager.Instance.GetSpecificPointerGraphicEventData(pointingSource);
        }

        public void RaisePointerUp(IPointingSource source, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, tags);

            ExecutePointerUp(HandlePointerUp(source));
        }

        public void RaisePointerUp(IPointingSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, handedness, tags);

            ExecutePointerUp(HandlePointerUp(source));
        }

#if UNITY_WSA
        public void RaisePointerUp(IPointingSource source, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(source, pressType, handedness, tags);

            if (pressType == InteractionSourcePressType.Select)
            {
                ExecutePointerUp(HandlePointerUp(source));
            }
        }
#endif
        #endregion Pointer Up

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputDownEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        public void RaiseOnInputDown(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        public void RaiseOnInputDown(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

#if UNITY_WSA
        public void RaiseOnInputDown(IInputSource source, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, pressType, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }
#endif
        #endregion Input Down

        #region Input Pressed

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputPressedEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputPressedEventData>(eventData);
                handler.OnInputDown(casted);
            };

        public void RaiseOnInputPressed(IInputSource source, double pressAmount, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, pressAmount, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputPressedEventHandler);
        }

        public void RaiseOnInputPressed(IInputSource source, double pressAmount, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, pressAmount, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputPressedEventHandler);
        }

#if UNITY_WSA
        public void RaiseOnInputPressed(IInputSource source, double pressAmount, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, pressAmount, pressType, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputPressedEventHandler);
        }
#endif
        #endregion Input Up

        #region Input Up

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputUpEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        public void RaiseOnInputUp(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        public void RaiseOnInputUp(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

#if UNITY_WSA
        public void RaiseOnInputUp(IInputSource source, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, pressType, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }
#endif
        #endregion Input Up

        #region Input Position Changed

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputPositionChanged =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputPositionEventData>(eventData);
                handler.OnInputPositionChanged(casted);
            };

        public void RaiseInputPositionChanged(IInputSource source, Vector2 inputPosition, object[] tags = null)
        {
            // Create input event
            inputPositionEventData.Initialize(source, inputPosition, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPositionEventData, OnInputPositionChanged);
        }

        public void RaiseInputPositionChanged(IInputSource source, Vector2 inputPosition, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPositionEventData.Initialize(source, inputPosition, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPositionEventData, OnInputPositionChanged);
        }

#if UNITY_WSA
        public void RaiseInputPositionChanged(IInputSource source, Vector2 inputPosition, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPositionEventData.Initialize(source, inputPosition, pressType, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPositionEventData, OnInputPositionChanged);
        }
#endif
        #endregion Input Position Changed

        #endregion Generic Input Events

        #region Hold Events

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldStartedEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldStarted(casted);
            };

        public void RaiseHoldStarted(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldStartedEventHandler);
        }

        public void RaiseHoldStarted(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCompletedEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldCompleted(casted);
            };

        public void RaiseHoldCompleted(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCompletedEventHandler);
        }

        public void RaiseHoldCompleted(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCanceledEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldCanceled(casted);
            };

        public void RaiseHoldCanceled(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCanceledEventHandler);
        }

        public void RaiseHoldCanceled(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCanceledEventHandler);
        }

        #endregion Hold Events

        #region Navigation Events

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationStartedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationStarted(casted);
            };

        public void RaiseNavigationStarted(IInputSource source, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        public void RaiseNavigationStarted(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationUpdatedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationUpdated(casted);
            };

        public void RaiseNavigationUpdated(IInputSource source, Vector3 normalizedOffset, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        public void RaiseNavigationUpdated(IInputSource source, Vector3 normalizedOffset, Handedness handedness, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCompletedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCompleted(casted);
            };

        public void RaiseNavigationCompleted(IInputSource source, Vector3 normalizedOffset, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        public void RaiseNavigationCompleted(IInputSource source, Vector3 normalizedOffset, Handedness handedness, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCanceledEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCanceled(casted);
            };

        public void RaiseNavigationCanceled(IInputSource source, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

        public void RaiseNavigationCanceled(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

        #endregion Navigation Events

        #region Manipulation Events

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationStartedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationStarted(casted);
            };

        public void RaiseManipulationStarted(IInputSource source, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        public void RaiseManipulationStarted(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationUpdatedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationUpdated(casted);
            };

        public void RaiseManipulationUpdated(IInputSource source, Vector3 cumulativeDelta, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        public void RaiseManipulationUpdated(IInputSource source, Vector3 cumulativeDelta, Handedness handedness, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCompletedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCompleted(casted);
            };

        public void RaiseManipulationCompleted(IInputSource source, Vector3 cumulativeDelta, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        public void RaiseManipulationCompleted(IInputSource source, Vector3 cumulativeDelta, Handedness handedness, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCanceledEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCanceled(casted);
            };

        public void RaiseManipulationCanceled(IInputSource source, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        public void RaiseManipulationCanceled(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        #endregion Manipulation Events

#if UNITY_WSA || UNITY_STANDALONE_WIN

        #region Speech Events

        private static readonly ExecuteEvents.EventFunction<ISpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (ISpeechHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        public void RaiseSpeechKeywordPhraseRecognized(IInputSource source, ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text, object[] tags = null)
        {
            // Create input event
            speechEventData.Initialize(source, confidence, phraseDuration, phraseStartTime, semanticMeanings, text, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechEventData, OnSpeechKeywordRecognizedEventHandler);
        }

        #endregion Speech Events

        #region Dictation Events

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        public void RaiseDictationHypothesis(IInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null, object[] tags = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationHypothesis, dictationAudioClip, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationHypothesisEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationResultEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        public void RaiseDictationResult(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationResultEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationCompleteEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        public void RaiseDictationComplete(IInputSource source, string dictationResult, AudioClip dictationAudioClip, object[] tags = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationCompleteEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationErrorEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };

        public void RaiseDictationError(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion Dictation Events

#endif

        #endregion Input Handlers
    }
}
