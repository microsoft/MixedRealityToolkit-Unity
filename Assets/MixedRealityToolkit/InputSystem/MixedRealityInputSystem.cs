// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.InputSystem.EventData;
using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.InputSystem.InputHandlers;
using Microsoft.MixedReality.Toolkit.InputSystem.InputSources;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows.Speech;

namespace Microsoft.MixedReality.Toolkit.InputSystem
{
    /// <summary>
    /// The Input system controls the orchestration of input events in a scene
    /// </summary>
    public class MixedRealityInputManager : BaseManager, IMixedRealityInputSystem
    {
        public static event Action InputEnabled;
        public static event Action InputDisabled;

        /// <summary>
        /// List of the Interaction Input Sources as detected by the input manager like hands or motion controllers.
        /// </summary>
        public static HashSet<IInputSource> DetectedInputSources { get; } = new HashSet<IInputSource>();

        /// <summary>
        /// Global listeners listen to all events and ignore the fact that other components might have consumed them.
        /// </summary>
        public static List<GameObject> GlobalListeners { get; } = new List<GameObject>(0);

        private static readonly Stack<GameObject> ModalInputStack = new Stack<GameObject>();
        private static readonly Stack<GameObject> FallbackInputStack = new Stack<GameObject>();

        /// <summary>
        /// Indicates if input is currently enabled or not.
        /// </summary>
        public static bool IsInputEnabled => disabledRefCount <= 0;

        private static int disabledRefCount;

        private static SourceStateEventData sourceStateEventData;
        private static SourcePositionEventData sourcePositionEventData;
        private static SourceRotationEventData sourceRotationEventData;

        private static ClickEventData clickEventData;
        private static FocusEventData focusEventData;

        private static InputEventData inputEventData;
        private static InputPressedEventData inputPressedEventData;
        private static InputPositionEventData inputPositionEventData;

        private static PlacementEventData placementEventData;

        private static NavigationEventData navigationEventData;
        private static ManipulationEventData manipulationEventData;

        private static TeleportEventData teleportEventData;

#if UNITY_WSA || UNITY_STANDALONE_WIN
        private static SpeechEventData speechEventData;
        private static DictationEventData dictationEventData;
#endif // UNITY_WSA || UNITY_STANDALONE_WIN

        #region IMixedRealityManager Implementation

        /// <summary>
        /// IMixedRealityManager Initialize function, called once the Mixed Reality Manager has finished registering all managers
        /// </summary>
        public override void Initialize()
        {
            InitializeEventDatas();
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        public override void Reset()
        {
            InitializeEventDatas();
        }

        #endregion IMixedRealityManager Implementation

        private static void InitializeEventDatas()
        {
            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            sourcePositionEventData = new SourcePositionEventData(EventSystem.current);
            sourceRotationEventData = new SourceRotationEventData(EventSystem.current);

            clickEventData = new ClickEventData(EventSystem.current);
            focusEventData = new FocusEventData(EventSystem.current);

            inputEventData = new InputEventData(EventSystem.current);
            inputPressedEventData = new InputPressedEventData(EventSystem.current);
            inputPositionEventData = new InputPositionEventData(EventSystem.current);

            placementEventData = new PlacementEventData(EventSystem.current);

            navigationEventData = new NavigationEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);

            teleportEventData = new TeleportEventData(EventSystem.current);

#if UNITY_WSA || UNITY_STANDALONE_WIN
            speechEventData = new SpeechEventData(EventSystem.current);
            dictationEventData = new DictationEventData(EventSystem.current);
#endif // UNITY_WSA || UNITY_STANDALONE_WIN
        }

        #region Input Disabled Options

        /// <summary>
        /// Push a disabled input state onto the input manager.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        public static void PushInputDisable()
        {
            ++disabledRefCount;

            if (disabledRefCount == 1)
            {
                InputDisabled?.Invoke();
            }
        }

        /// <summary>
        /// Pop disabled input state. When the last disabled state is 
        /// popped off the stack input will be re-enabled.
        /// </summary>
        public static void PopInputDisable()
        {
            --disabledRefCount;
            Debug.Assert(disabledRefCount >= 0, "Tried to pop more input disable than the amount pushed.");

            if (disabledRefCount == 0)
            {
                InputEnabled?.Invoke();
            }
        }

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable input.
        /// </summary>
        public static void ClearInputDisableStack()
        {
            bool wasInputDisabled = disabledRefCount > 0;
            disabledRefCount = 0;

            if (wasInputDisabled)
            {
                InputEnabled?.Invoke();
            }
        }

        #endregion Input Disabled Options

        #region Global Listener Options

        /// <summary>
        /// Adds a global listener that will receive all input events, regardless
        /// of which other game objects might have handled the event beforehand.
        /// </summary>
        /// <param name="listener">Listener to add.</param>
        public static void AddGlobalListener(GameObject listener)
        {
            GlobalListeners.Add(listener);
        }

        /// <summary>
        /// Removes a global listener.
        /// </summary>
        /// <param name="listener">Listener to remove.</param>
        public static void RemoveGlobalListener(GameObject listener)
        {
            GlobalListeners.Remove(listener);
        }

        #endregion

        #region Modal Input Options

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public static void PushModalInputHandler(GameObject inputHandler)
        {
            ModalInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        public static void PopModalInputHandler()
        {
            if (ModalInputStack.Count > 0)
            {
                ModalInputStack.Pop();

            }
        }

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        public static void ClearModalInputStack()
        {
            ModalInputStack.Clear();
        }

        #endregion

        #region Fallback Input Handler Options

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public static void PushFallbackInputHandler(GameObject inputHandler)
        {
            FallbackInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        public static void PopFallbackInputHandler()
        {
            FallbackInputStack.Pop();
        }

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        public static void ClearFallbackInputStack()
        {
            FallbackInputStack.Clear();
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
        public static void HandleEvent<T>(BaseInputEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            Debug.Assert(!eventData.used);

            GameObject focusedObject = FocusManager.GetFocusedObject(eventData);

            // Send the event to global listeners
            for (int i = 0; i < GlobalListeners.Count; i++)
            {
                // Global listeners should only get events on themselves, as opposed to their hierarchy.
                ExecuteEvents.Execute(GlobalListeners[i], eventData, eventHandler);
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
            if (ModalInputStack.Count > 0)
            {
                GameObject modalInput = ModalInputStack.Peek();

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
            if (FallbackInputStack.Count > 0)
            {
                GameObject fallbackInput = FallbackInputStack.Peek();
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

            foreach (var inputSource in DetectedInputSources)
            {
                if (inputSource.SourceId == newId)
                {
                    return GenerateNewSourceId();
                }
            }

            return newId;
        }

        public static void RaiseSourceDetected(IInputSource source, object[] tags = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, tags);

            AddSource(source);
        }

        private static void AddSource(IInputSource source)
        {
            Debug.Assert(!DetectedInputSources.Contains(source), $"{source.SourceName} has already been registered with the Input Manager!");

            DetectedInputSources.Add(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceDetectedEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        public static void RaiseSourceLost(IInputSource source, object[] tags = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, tags);

            RemoveSource(source);
        }

        private static void RemoveSource(IInputSource source)
        {
            Debug.Assert(DetectedInputSources.Contains(source), $"{source.SourceName} was never registered with the Input Manager!");

            DetectedInputSources.Remove(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceLostEventHandler =
                delegate (ISourceStateHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                    handler.OnSourceLost(casted);
                };

        public static void RaiseSourcePositionChanged(IInputSource source, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null)
        {
            // Create input event
            sourcePositionEventData.Initialize(source, pointerPosition, gripPosition, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePositionEventData, OnSourcePositionChangedEventHandler);
        }

        public static void RaiseSourcePositionChanged(IInputSource source, Vector3 pointerPosition, Vector3 gripPosition, Handedness sourceHandedness, object[] tags = null)
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

        public static void RaiseSourceRotationChanged(IInputSource source, Quaternion pointerRotation, Quaternion gripRotation, Handedness sourceHandedness, object[] tags = null)
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
        /// Raise the event OnBeforeFocusChange to the game object when focus enters it.
        /// </summary>
        /// <param name="pointer">The pointer that focused the GameObject.</param>
        /// <param name="focusedObject">The GameObject that is focused.</param>
        public static void RaiseFocusEnter(IPointer pointer, GameObject focusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, OnFocusEnterEventHandler);

            var graphicEventData = FocusManager.GetSpecificPointerGraphicEventData(pointer);
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
        public static void RaiseFocusExit(IPointer pointer, GameObject unfocusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, OnFocusExitEventHandler);

            var graphicEventData = FocusManager.GetSpecificPointerGraphicEventData(pointer);
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
        /// Raise the Pre Focus Changed Event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="oldFocusedObject"></param>
        /// <param name="newFocusedObject"></param>
        public static void RaisePreFocusChangedEvent(IPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.gameObject, focusEventData, OnPreFocusChangedHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusChangedHandler> OnPreFocusChangedHandler =
                delegate (IFocusChangedHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnBeforeFocusChange(casted);
                };

        /// <summary>
        /// Raise focus enter and exit events for when an input (that supports pointing) points to a game object.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="oldFocusedObject"></param>
        /// <param name="newFocusedObject"></param>
        public static void OnFocusChangedEvent(IPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.gameObject, focusEventData, OnFocusChangedHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusChangedHandler> OnFocusChangedHandler =
            delegate (IFocusChangedHandler handler, BaseEventData eventData)
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

        private static void ExecutePointerDown(GraphicInputEventData graphicInputEventData)
        {
            if (graphicInputEventData != null && graphicInputEventData.selectedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(graphicInputEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerDownHandler);
            }
        }

        private static GraphicInputEventData HandlePointerDown(IPointer pointingSource)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(clickEventData, OnPointerDownEventHandler);

            return FocusManager.GetSpecificPointerGraphicEventData(pointingSource);
        }

        public static void RaisePointerDown(IPointer pointer, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, tags);

            ExecutePointerDown(HandlePointerDown(pointer));
        }

        public static void RaisePointerDown(IPointer pointer, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, handedness, tags);

            ExecutePointerDown(HandlePointerDown(pointer));
        }

#if UNITY_WSA
        public static void RaisePointerDown(IPointer pointer, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, pressType, handedness, tags);

            if (pressType == InteractionSourcePressType.Select)
            {
                ExecutePointerDown(HandlePointerDown(pointer));
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

        private static void HandleClick()
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(clickEventData, OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaisePointerUp call the pointerClickHandler.
        }

        public static void RaiseInputClicked(IPointer pointer, int tapCount, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, tapCount, tags);

            HandleClick();
        }

        public static void RaiseInputClicked(IPointer pointer, int tapCount, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, tapCount, handedness, tags);

            HandleClick();
        }

#if UNITY_WSA
        public static void RaiseInputClicked(IPointer pointer, int tapCount, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, tapCount, pressType, handedness, tags);

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

        private static void ExecutePointerUp(GraphicInputEventData graphicInputEventData)
        {
            if (graphicInputEventData != null)
            {
                if (graphicInputEventData.selectedObject != null)
                {
                    ExecuteEvents.ExecuteHierarchy(graphicInputEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.ExecuteHierarchy(graphicInputEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerClickHandler);
                }

                graphicInputEventData.Clear();
            }
        }

        private static GraphicInputEventData HandlePointerUp(IPointer pointingSource)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(clickEventData, OnPointerUpEventHandler);

            return FocusManager.GetSpecificPointerGraphicEventData(pointingSource);
        }

        public static void RaisePointerUp(IPointer pointer, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, tags);

            ExecutePointerUp(HandlePointerUp(pointer));
        }

        public static void RaisePointerUp(IPointer pointer, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, handedness, tags);

            ExecutePointerUp(HandlePointerUp(pointer));
        }

#if UNITY_WSA
        public static void RaisePointerUp(IPointer pointer, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            clickEventData.Initialize(pointer.InputSourceParent, pressType, handedness, tags);

            if (pressType == InteractionSourcePressType.Select)
            {
                ExecutePointerUp(HandlePointerUp(pointer));
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

        public static void RaiseOnInputDown(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        public static void RaiseOnInputDown(IInputSource source, KeyCode keyCode, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, keyCode, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        public static void RaiseOnInputDown(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        public static void RaiseOnInputDown(IInputSource source, KeyCode keyCode, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, keyCode, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

#if UNITY_WSA
        public static void RaiseOnInputDown(IInputSource source, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
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
                handler.OnInputPressed(casted);
            };

        public static void RaiseOnInputPressed(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        public static void RaiseOnInputPressed(IInputSource source, KeyCode keyCode, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, keyCode, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        public static void RaiseOnInputPressed(IInputSource source, double pressAmount, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, pressAmount, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        public static void RaiseOnInputPressed(IInputSource source, KeyCode keyCode, double pressAmount, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, keyCode, pressAmount, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        public static void RaiseOnInputPressed(IInputSource source, double pressAmount, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, pressAmount, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        public static void RaiseOnInputPressed(IInputSource source, KeyCode keyCode, double pressAmount, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, keyCode, pressAmount, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

#if UNITY_WSA
        public static void RaiseOnInputPressed(IInputSource source, double pressAmount, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPressedEventData.Initialize(source, pressAmount, pressType, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }
#endif
        #endregion Input Up

        #region Input Up

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputUpEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        public static void RaiseOnInputUp(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        public static void RaiseOnInputUp(IInputSource source, KeyCode keyCode, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, keyCode, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        public static void RaiseOnInputUp(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        public static void RaiseOnInputUp(IInputSource source, KeyCode keyCode, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, keyCode, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

#if UNITY_WSA
        public static void RaiseOnInputUp(IInputSource source, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
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

        public static void RaiseInputPositionChanged(IInputSource source, InputPositionType inputType, Vector2 inputPosition, object[] tags = null)
        {
            // Create input event
            inputPositionEventData.Initialize(source, inputType, inputPosition, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPositionEventData, OnInputPositionChanged);
        }

        public static void RaiseInputPositionChanged(IInputSource source, InputPositionType inputType, Vector2 inputPosition, Handedness handedness, object[] tags = null)
        {
            // Create input event
            inputPositionEventData.Initialize(source, inputType, inputPosition, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPositionEventData, OnInputPositionChanged);
        }

#if UNITY_WSA
        public static void RaiseInputPositionChanged(IInputSource source, Vector2 inputPosition, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
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

        public static void RaiseHoldStarted(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldStartedEventHandler);
        }

        public static void RaiseHoldStarted(IInputSource source, Handedness handedness, object[] tags = null)
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

        public static void RaiseHoldCompleted(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCompletedEventHandler);
        }

        public static void RaiseHoldCompleted(IInputSource source, Handedness handedness, object[] tags = null)
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

        public static void RaiseHoldCanceled(IInputSource source, object[] tags = null)
        {
            // Create input event
            inputEventData.Initialize(source, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCanceledEventHandler);
        }

        public static void RaiseHoldCanceled(IInputSource source, Handedness handedness, object[] tags = null)
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

        public static void RaiseNavigationStarted(IInputSource source, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        public static void RaiseNavigationStarted(IInputSource source, Handedness handedness, object[] tags = null)
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

        public static void RaiseNavigationUpdated(IInputSource source, Vector3 normalizedOffset, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        public static void RaiseNavigationUpdated(IInputSource source, Vector3 normalizedOffset, Handedness handedness, object[] tags = null)
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

        public static void RaiseNavigationCompleted(IInputSource source, Vector3 normalizedOffset, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        public static void RaiseNavigationCompleted(IInputSource source, Vector3 normalizedOffset, Handedness handedness, object[] tags = null)
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

        public static void RaiseNavigationCanceled(IInputSource source, object[] tags = null)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

        public static void RaiseNavigationCanceled(IInputSource source, Handedness handedness, object[] tags = null)
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

        public static void RaiseManipulationStarted(IInputSource source, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        public static void RaiseManipulationStarted(IInputSource source, Handedness handedness, object[] tags = null)
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

        public static void RaiseManipulationUpdated(IInputSource source, Vector3 cumulativeDelta, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        public static void RaiseManipulationUpdated(IInputSource source, Vector3 cumulativeDelta, Handedness handedness, object[] tags = null)
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

        public static void RaiseManipulationCompleted(IInputSource source, Vector3 cumulativeDelta, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        public static void RaiseManipulationCompleted(IInputSource source, Vector3 cumulativeDelta, Handedness handedness, object[] tags = null)
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

        public static void RaiseManipulationCanceled(IInputSource source, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        public static void RaiseManipulationCanceled(IInputSource source, Handedness handedness, object[] tags = null)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero, handedness, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        #endregion Manipulation Events

        //#region Teleport Events

        //private static readonly ExecuteEvents.EventFunction<ITeleportHandler> OnTeleportIntentHandler =
        //        delegate (ITeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportIntent(casted);
        //        };

        //public static void RaiseTeleportIntent(TeleportPointer pointer, object[] tags = null)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent, tags);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportIntentHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<ITeleportHandler> OnTeleportStartedHandler =
        //        delegate (ITeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportStarted(casted);
        //        };

        //public static void RaiseTeleportStarted(TeleportPointer pointer, object[] tags = null)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent, tags);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportStartedHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<ITeleportHandler> OnTeleportCompletedHandler =
        //        delegate (ITeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportCompleted(casted);
        //        };

        //public static void RaiseTeleportCompleted(TeleportPointer pointer, object[] tags = null)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent, tags);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportCompletedHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<ITeleportHandler> OnTeleportCanceledHandler =
        //        delegate (ITeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportCanceled(casted);
        //        };

        //public static void RaiseTeleportCanceled(TeleportPointer pointer, object[] tags = null)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent, tags);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportCanceledHandler);
        //}

        //#endregion Teleport Events

#if UNITY_WSA || UNITY_STANDALONE_WIN

        #region Speech Events

        private static readonly ExecuteEvents.EventFunction<ISpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (ISpeechHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        public static void RaiseSpeechKeywordPhraseRecognized(IInputSource source, ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text, object[] tags = null)
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

        public static void RaiseDictationHypothesis(IInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null, object[] tags = null)
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

        public static void RaiseDictationResult(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null)
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

        public static void RaiseDictationComplete(IInputSource source, string dictationResult, AudioClip dictationAudioClip, object[] tags = null)
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

        public static void RaiseDictationError(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip, tags);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion Dictation Events

#endif // UNITY_WSA || UNITY_STANDALONE_WIN

        #region Placement Events

        private static readonly ExecuteEvents.EventFunction<IPlacementHandler> OnPlacingStartedEventHandler =
            delegate (IPlacementHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PlacementEventData>(eventData);
                handler.OnPlacingStarted(casted);
            };

        public static void RaisePlacingStarted(IInputSource source, GameObject objectBeingPlaced, object[] tags = null)
        {
            // Create input event
            placementEventData.Initialize(source, tags, objectBeingPlaced);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(placementEventData, OnPlacingStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IPlacementHandler> OnPlacingCompletedEventHandler =
            delegate (IPlacementHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PlacementEventData>(eventData);
                handler.OnPlacingCompleted(casted);
            };

        public static void RaisePlacingCompleted(IInputSource source, GameObject objectBeingPlaced, object[] tags = null)
        {
            // Create input event
            placementEventData.Initialize(source, tags, objectBeingPlaced);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(placementEventData, OnPlacingCompletedEventHandler);
        }

        #endregion Placement Events

        #endregion Input Handlers
    }
}
