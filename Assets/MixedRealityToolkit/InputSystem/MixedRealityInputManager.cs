// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.InputSystem.Gaze;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.InputSystem
{
    /// <summary>
    /// The Input system controls the orchestration of input events in a scene
    /// </summary>
    public class MixedRealityInputManager : MixedRealityEventManager, IMixedRealityInputSystem
    {
        /// <inheritdoc />
        public event Action InputEnabled;

        /// <inheritdoc />
        public event Action InputDisabled;

        /// <inheritdoc />
        public HashSet<IMixedRealityInputSource> DetectedInputSources { get; } = new HashSet<IMixedRealityInputSource>();

        /// <inheritdoc />
        public IMixedRealityFocusProvider FocusProvider { get; private set; }

        /// <inheritdoc />
        public IMixedRealityGazeProvider GazeProvider { get; private set; }

        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <inheritdoc />
        public bool IsInputEnabled => disabledRefCount <= 0;

        private int disabledRefCount;

        private SourceStateEventData sourceStateEventData;

        private FocusEventData focusEventData;

        private InputEventData inputEventData;
        private InputClickEventData inputClickEventData;
        private InputPressedEventData inputPressedEventData;

        private TwoDoFInputEventData twoDoFInputEventData;
        private ThreeDoFInputEventData threeDoFInputEventData;
        private SixDoFInputEventData sixDoFInputEventData;

        private NavigationEventData navigationEventData;
        private ManipulationEventData manipulationEventData;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        private SpeechEventData speechEventData;
        private DictationEventData dictationEventData;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #region IMixedRealityManager Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        private void InitializeInternal()
        {
            FocusProvider = CameraCache.Main.gameObject.EnsureComponent<FocusProvider>();
            GazeProvider = CameraCache.Main.gameObject.EnsureComponent<GazeProvider>();
            FocusProvider.UIRaycastCamera.gameObject.EnsureComponent<EventSystem>();
            FocusProvider.UIRaycastCamera.gameObject.EnsureComponent<StandaloneInputModule>();

            sourceStateEventData = new SourceStateEventData(EventSystem.current);

            focusEventData = new FocusEventData(EventSystem.current);

            inputEventData = new InputEventData(EventSystem.current);
            inputClickEventData = new InputClickEventData(EventSystem.current);
            inputPressedEventData = new InputPressedEventData(EventSystem.current);

            twoDoFInputEventData = new TwoDoFInputEventData(EventSystem.current);
            threeDoFInputEventData = new ThreeDoFInputEventData(EventSystem.current);
            sixDoFInputEventData = new SixDoFInputEventData(EventSystem.current);

            navigationEventData = new NavigationEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            speechEventData = new SpeechEventData(EventSystem.current);
            dictationEventData = new DictationEventData(EventSystem.current);
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        }

        /// <inheritdoc />
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        #endregion IMixedRealityManager Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            var baseInputEventData = ExecuteEvents.ValidateEventData<BaseInputEventData>(eventData);

            Debug.Assert(baseInputEventData != null);
            Debug.Assert(!baseInputEventData.used);
            Debug.Assert(eventData != null);

            GameObject focusedObject = FocusProvider?.GetFocusedObject(baseInputEventData);

            // Send the event to global listeners
            for (int i = 0; i < EventListeners.Count; i++)
            {
                // Global listeners should only get events on themselves, as opposed to their hierarchy.
                ExecuteEvents.Execute(EventListeners[i], baseInputEventData, eventHandler);
            }

            if (baseInputEventData.used)
            {
                // All global listeners get a chance to see the event, but if any of them marked it used, we stop
                // the event from going any further.
                return;
            }

            // Handle modal input if one exists
            if (modalInputStack.Count > 0)
            {
                GameObject modalInput = modalInputStack.Peek();

                // If there is a focused object in the hierarchy of the modal handler, start the event bubble there
                if (focusedObject != null && modalInput != null && focusedObject.transform.IsChildOf(modalInput.transform))
                {
                    if (ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler) && baseInputEventData.used)
                    {
                        return;
                    }
                }
                // Otherwise, just invoke the event on the modal handler itself
                else
                {
                    if (ExecuteEvents.ExecuteHierarchy(modalInput, baseInputEventData, eventHandler) && baseInputEventData.used)
                    {
                        return;
                    }
                }
            }

            // If event was not handled by modal, pass it on to the current focused object
            if (focusedObject != null)
            {
                if (ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler) && baseInputEventData.used)
                {
                    return;
                }
            }

            // If event was not handled by the focused object, pass it on to any fallback handlers
            if (fallbackInputStack.Count > 0)
            {
                GameObject fallbackInput = fallbackInputStack.Peek();
                if (ExecuteEvents.ExecuteHierarchy(fallbackInput, baseInputEventData, eventHandler) && baseInputEventData.used)
                {
                    // return;
                }
            }
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> to listen to events that will receive all input events, regardless
        /// of which other <see cref="GameObject"/>s might have handled the event beforehand.
        /// </summary>
        /// <param name="listener">Listener to add.</param>
        public override void Register(GameObject listener)
        {
            Debug.Assert(!EventListeners.Contains(listener), $"{listener.name} is already registered to receive input events!");
            EventListeners.Add(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to input events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            Debug.Assert(EventListeners.Contains(listener), $"{listener.name} was never registered for input events!");
            EventListeners.Remove(listener);
        }

        #endregion IEventSystemManager Implementation

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
                InputDisabled?.Invoke();
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
                InputEnabled?.Invoke();
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
                InputEnabled?.Invoke();
            }
        }

        #endregion Input Disabled Options

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

        #endregion Modal Input Options

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

        #region Input Source Events

        /// <inheritdoc />
        public uint GenerateNewSourceId()
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

        /// <inheritdoc />
        public void RaiseSourceDetected(IMixedRealityInputSource source)
        {
            // Create input event
            sourceStateEventData.Initialize(source);

            AddSource(source);
        }

        private void AddSource(IMixedRealityInputSource source)
        {
            Debug.Assert(!DetectedInputSources.Contains(source), $"{source.SourceName} has already been registered with the Input Manager!");

            DetectedInputSources.Add(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourceStateHandler> OnSourceDetectedEventHandler =
            delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        /// <inheritdoc />
        public void RaiseSourceLost(IMixedRealityInputSource source)
        {
            // Create input event
            sourceStateEventData.Initialize(source);

            RemoveSource(source);
        }

        private void RemoveSource(IMixedRealityInputSource source)
        {
            Debug.Assert(DetectedInputSources.Contains(source), $"{source.SourceName} was never registered with the Input Manager!");

            DetectedInputSources.Remove(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourceStateHandler> OnSourceLostEventHandler =
                delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                    handler.OnSourceLost(casted);
                };

        #endregion Input Source State Events

        #region Focus Events

        /// <inheritdoc />
        public void RaisePreFocusChangedEvent(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
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
                ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GetGameObjectReference(), focusEventData, OnPreFocusChangedHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusChangedHandler> OnPreFocusChangedHandler =
                delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnBeforeFocusChange(casted);
                };

        /// <inheritdoc />
        public void OnFocusChangedEvent(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
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
                ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GetGameObjectReference(), focusEventData, OnFocusChangedHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusChangedHandler> OnFocusChangedHandler =
            delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, OnFocusEnterEventHandler);

            var graphicEventData = FocusProvider.GetSpecificPointerGraphicEventData(pointer);
            if (graphicEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicEventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusEnterEventHandler =
                delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnFocusEnter(casted);
                };

        /// <inheritdoc />
        public void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, OnFocusExitEventHandler);

            var graphicEventData = FocusProvider.GetSpecificPointerGraphicEventData(pointer);
            if (graphicEventData != null)
            {
                ExecuteEvents.ExecuteHierarchy(unfocusedObject, graphicEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusExitEventHandler =
                delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnFocusExit(casted);
                };

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerDownEventHandler =
            delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputClickEventData>(eventData);
                handler.OnPointerDown(casted);
            };

        private static void ExecutePointerDown(GraphicInputEventData graphicInputEventData)
        {
            if (graphicInputEventData != null && graphicInputEventData.selectedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(graphicInputEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerDownHandler);
            }
        }

        private GraphicInputEventData HandlePointerDown(IMixedRealityPointer pointingSource)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputClickEventData, OnPointerDownEventHandler);

            return FocusProvider.GetSpecificPointerGraphicEventData(pointingSource);
        }

        /// <inheritdoc />
        public void RaisePointerDown(IMixedRealityPointer pointer)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent);

            ExecutePointerDown(HandlePointerDown(pointer));
        }

        /// <inheritdoc />
        public void RaisePointerDown(IMixedRealityPointer pointer, Handedness handedness)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent, handedness);

            ExecutePointerDown(HandlePointerDown(pointer));
        }

        /// <inheritdoc />
        public void RaisePointerDown(IMixedRealityPointer pointer, Handedness handedness, InputAction inputAction)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent, handedness, inputAction);

            if (MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.PointerAction.Id == inputAction.Id)
            {
                ExecutePointerDown(HandlePointerDown(pointer));
            }
        }

        #endregion Pointer Down

        #region Pointer Click

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnInputClickedEventHandler =
                delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<InputClickEventData>(eventData);
                    handler.OnPointerClicked(casted);
                };

        private void HandleClick()
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputClickEventData, OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaisePointerUp call the pointerClickHandler.
        }

        /// <inheritdoc />
        public void RaiseInputClicked(IMixedRealityPointer pointer, int count)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent, count);

            HandleClick();
        }

        /// <inheritdoc />
        public void RaiseInputClicked(IMixedRealityPointer pointer, Handedness handedness, int count)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent, count, handedness);

            HandleClick();
        }

        /// <inheritdoc />
        public void RaiseInputClicked(IMixedRealityPointer pointer, Handedness handedness, InputAction inputAction, int count)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent, count, inputAction, handedness);

            if (MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.PointerAction.Id == inputAction.Id)
            {
                HandleClick();
            }
        }

        #endregion Pointer Click

        #region Pointer Up

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerUpEventHandler =
            delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputClickEventData>(eventData);
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

        private GraphicInputEventData HandlePointerUp(IMixedRealityPointer pointingSource)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputClickEventData, OnPointerUpEventHandler);

            return FocusProvider.GetSpecificPointerGraphicEventData(pointingSource);
        }

        /// <inheritdoc />
        public void RaisePointerUp(IMixedRealityPointer pointer)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent);

            ExecutePointerUp(HandlePointerUp(pointer));
        }

        /// <inheritdoc />
        public void RaisePointerUp(IMixedRealityPointer pointer, Handedness handedness)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent, handedness);

            ExecutePointerUp(HandlePointerUp(pointer));
        }

        /// <inheritdoc />
        public void RaisePointerUp(IMixedRealityPointer pointer, Handedness handedness, InputAction inputAction)
        {
            // Create input event
            inputClickEventData.Initialize(pointer.InputSourceParent, handedness, inputAction);

            if (MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.PointerAction.Id == inputAction.Id)
            {
                ExecutePointerUp(HandlePointerUp(pointer));
            }
        }

        #endregion Pointer Up

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputDownEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source)
        {
            // Create input event
            inputEventData.Initialize(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, KeyCode keyCode)
        {
            // Create input event
            inputEventData.Initialize(source, keyCode);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            inputEventData.Initialize(source, handedness);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, keyCode);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        #endregion Input Down

        #region Input Pressed

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputPressedEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputPressedEventData>(eventData);
                handler.OnInputPressed(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source)
        {
            // Create input event
            inputPressedEventData.Initialize(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, KeyCode keyCode)
        {
            // Create input event
            inputPressedEventData.Initialize(source, keyCode, 1D);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, float pressAmount)
        {
            // Create input event
            inputPressedEventData.Initialize(source, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, KeyCode keyCode, float pressAmount)
        {
            // Create input event
            inputPressedEventData.Initialize(source, keyCode, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, InputAction inputAction, float pressAmount)
        {
            // Create input event
            inputPressedEventData.Initialize(source, inputAction, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, float pressAmount)
        {
            // Create input event
            inputPressedEventData.Initialize(source, handedness, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode, float pressAmount)
        {
            // Create input event
            inputPressedEventData.Initialize(source, handedness, keyCode, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, float pressAmount)
        {
            // Create input event
            inputPressedEventData.Initialize(source, handedness, inputAction, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputPressedEventData, OnInputPressedEventHandler);
        }

        #endregion Input Pressed

        #region Input Up

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputUpEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source)
        {
            // Create input event
            inputEventData.Initialize(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, KeyCode keyCode)
        {
            // Create input event
            inputEventData.Initialize(source, keyCode);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            inputEventData.Initialize(source, handedness);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, keyCode);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        #endregion Input Up

        #region Input 2DoF Changed

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnTwoDoFInputChanged =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TwoDoFInputEventData>(eventData);
                handler.On2DoFInputChanged(casted);
            };

        /// <inheritdoc />
        public void Raise2DoFInputChanged(IMixedRealityInputSource source, InputAction inputAction, Vector2 inputPosition)
        {
            // Create input event
            twoDoFInputEventData.Initialize(source, inputAction, inputPosition);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(twoDoFInputEventData, OnTwoDoFInputChanged);
        }

        /// <inheritdoc />
        public void Raise2DoFInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Vector2 inputPosition)
        {
            // Create input event
            twoDoFInputEventData.Initialize(source, inputAction, inputPosition, handedness);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(twoDoFInputEventData, OnTwoDoFInputChanged);
        }

        #endregion Input 2Dof Changed

        #region Input 3DoF Changed

        private static readonly ExecuteEvents.EventFunction<IMixedReality3DoFInputHandler> OnThreeDoFInputChanged =
            delegate (IMixedReality3DoFInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ThreeDoFInputEventData>(eventData);
                handler.On3DoFInputChanged(casted);
            };

        /// <inheritdoc />
        public void Raise3DoFInputChanged(IMixedRealityInputSource source, InputAction inputAction, Vector3 position)
        {
            // Create input event
            threeDoFInputEventData.Initialize(source, inputAction, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(threeDoFInputEventData, OnThreeDoFInputChanged);
        }

        /// <inheritdoc />
        public void Raise3DoFInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Vector3 position)
        {
            // Create input event
            threeDoFInputEventData.Initialize(source, handedness, inputAction, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(threeDoFInputEventData, OnThreeDoFInputChanged);
        }

        /// <inheritdoc />
        public void Raise3DoFInputChanged(IMixedRealityInputSource source, InputAction inputAction, Quaternion rotation)
        {
            // Create input event
            threeDoFInputEventData.Initialize(source, inputAction, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(threeDoFInputEventData, OnThreeDoFInputChanged);
        }

        /// <inheritdoc />
        public void Raise3DoFInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Quaternion rotation)
        {
            // Create input event
            threeDoFInputEventData.Initialize(source, handedness, inputAction, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(threeDoFInputEventData, OnThreeDoFInputChanged);
        }

        #endregion Input 3DoF Changed

        #region Input 6DoF Changed

        private static readonly ExecuteEvents.EventFunction<IMixedReality6DoFInputHandler> OnSixDoFInputChanged =
            delegate (IMixedReality6DoFInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SixDoFInputEventData>(eventData);
                handler.On6DoFInputChanged(casted);
            };

        /// <inheritdoc />
        public void Raise6DofInputChanged(IMixedRealityInputSource source, InputAction inputAction, Tuple<Vector3, Quaternion> inputData)
        {
            // Create input event
            sixDoFInputEventData.Initialize(source, inputAction, inputData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(threeDoFInputEventData, OnSixDoFInputChanged);
        }

        /// <inheritdoc />
        public void Raise6DofInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Tuple<Vector3, Quaternion> inputData)
        {
            // Create input event
            sixDoFInputEventData.Initialize(source, handedness, inputAction, inputData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(threeDoFInputEventData, OnSixDoFInputChanged);
        }

        #endregion Input 6DoF Changed

        #endregion Generic Input Events

        #region Hold Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityHoldHandler> OnHoldStartedEventHandler =
            delegate (IMixedRealityHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseHoldStarted(IMixedRealityInputSource source)
        {
            // Create input event
            inputEventData.Initialize(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldStartedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseHoldStarted(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            inputEventData.Initialize(source, handedness);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityHoldHandler> OnHoldCompletedEventHandler =
            delegate (IMixedRealityHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseHoldCompleted(IMixedRealityInputSource source)
        {
            // Create input event
            inputEventData.Initialize(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCompletedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseHoldCompleted(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            inputEventData.Initialize(source, handedness);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityHoldHandler> OnHoldCanceledEventHandler =
            delegate (IMixedRealityHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseHoldCanceled(IMixedRealityInputSource source)
        {
            // Create input event
            inputEventData.Initialize(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCanceledEventHandler);
        }

        /// <inheritdoc />
        public void RaiseHoldCanceled(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            inputEventData.Initialize(source, handedness);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCanceledEventHandler);
        }

        #endregion Hold Events

        #region Navigation Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationStartedEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationStarted(IMixedRealityInputSource source)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationStarted(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            navigationEventData.Initialize(source, handedness, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationUpdatedEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationUpdated(IMixedRealityInputSource source, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationUpdated(IMixedRealityInputSource source, Handedness handedness, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, handedness, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationCompletedEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationCompleted(IMixedRealityInputSource source, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationCompleted(IMixedRealityInputSource source, Handedness handedness, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, handedness, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationCanceledEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationCanceled(IMixedRealityInputSource source)
        {
            // Create input event
            navigationEventData.Initialize(source, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationCanceled(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            navigationEventData.Initialize(source, handedness, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

        #endregion Navigation Events

        #region Manipulation Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationStartedEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationStarted(IMixedRealityInputSource source)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationStarted(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            manipulationEventData.Initialize(source, handedness, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationUpdatedEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationUpdated(IMixedRealityInputSource source, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationUpdated(IMixedRealityInputSource source, Handedness handedness, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, handedness, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationCompletedEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationCompleted(IMixedRealityInputSource source, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationCompleted(IMixedRealityInputSource source, Handedness handedness, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, handedness, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationCanceledEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationCanceled(IMixedRealityInputSource source)
        {
            // Create input event
            manipulationEventData.Initialize(source, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationCanceled(IMixedRealityInputSource source, Handedness handedness)
        {
            // Create input event
            manipulationEventData.Initialize(source, handedness, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        #endregion Manipulation Events

        #region Teleport Events

        //private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportIntentHandler =
        //        delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportIntent(casted);
        //        };

        //public void RaiseTeleportIntent(TeleportPointer pointer)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportIntentHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportStartedHandler =
        //        delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportStarted(casted);
        //        };

        //public void RaiseTeleportStarted(TeleportPointer pointer)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportStartedHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCompletedHandler =
        //        delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportCompleted(casted);
        //        };

        //public void RaiseTeleportCompleted(TeleportPointer pointer)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportCompletedHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCanceledHandler =
        //        delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
        //        {
        //            var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
        //            handler.OnTeleportCanceled(casted);
        //        };

        //public void RaiseTeleportCanceled(TeleportPointer pointer)
        //{
        //    // Create input event
        //    teleportEventData.Initialize(pointer.InputSourceParent);

        //    // Pass handler through HandleEvent to perform modal/fallback logic
        //    HandleEvent(teleportEventData, OnTeleportCanceledHandler);
        //}

        #endregion Teleport Events

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #region Speech Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (IMixedRealitySpeechHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        /// <inheritdoc />
        public void RaiseSpeechCommandRecognized(IMixedRealityInputSource source, UnityEngine.Windows.Speech.ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, UnityEngine.Windows.Speech.SemanticMeaning[] semanticMeanings, string text)
        {
            // Create input event
            speechEventData.Initialize(source, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechEventData, OnSpeechKeywordRecognizedEventHandler);
        }

        #endregion Speech Events

        #region Dictation Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationHypothesis(IMixedRealityInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationHypothesis, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationHypothesisEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationResultEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationResult(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationResultEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationCompleteEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationComplete(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationCompleteEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationErrorEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationError(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion Dictation Events

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #endregion Input Handlers

    }
}
