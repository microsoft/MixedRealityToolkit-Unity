// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <summary>
        /// Global listeners listen to all events and ignore the fact that other components might have consumed them.
        /// </summary>
        private readonly List<GameObject> globalListeners = new List<GameObject>();

        private int disabledRefCount;

        private FocusEventData focusEventData;
        private InputEventData inputEventData;
        private SourceStateEventData sourceStateEventData;
        private ManipulationEventData manipulationEventData;
        private NavigationEventData navigationEventData;
        private HoldEventData holdEventData;

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

        /// <summary>
        /// Register an input source with the input manager to make it start listening
        /// to events from it.
        /// </summary>
        /// <param name="inputSource">The input source to register</param>
        public void RegisterInputSource(IInputSource inputSource)
        {
            inputSource.FocusChanged += InputSource_FocusChanged;
            inputSource.HoldCanceled += InputSource_HoldCanceled;
            inputSource.HoldCompleted += InputSource_HoldCompleted;
            inputSource.HoldStarted += InputSource_HoldStarted;
            inputSource.ManipulationCanceled += InputSource_ManipulationCanceled;
            inputSource.ManipulationCompleted += InputSource_ManipulationCompleted;
            inputSource.ManipulationStarted += InputSource_ManipulationStarted;
            inputSource.ManipulationUpdated += InputSource_ManipulationUpdated;
            inputSource.SourceDown += InputSource_SourceDown;
            inputSource.SourceUp += InputSource_SourceUp;
            inputSource.SourceClicked += InputSource_SourceClicked;
            inputSource.SourceDetected += InputSource_SourceDetected;
            inputSource.SourceLost += InputSource_SourceLost;
            inputSource.NavigationCanceled += InputSource_NavigationCanceled;
            inputSource.NavigationCompleted += InputSource_NavigationCompleted;
            inputSource.NavigationStarted += InputSource_NavigationStarted;
            inputSource.NavigationUpdated += InputSource_NavigationUpdated;
        }

        /// <summary>
        /// Unregister an input source from the input manager. Will stop listening to events
        /// from the source.
        /// </summary>
        /// <param name="inputSource">The input source to unregister</param>
        public void UnregisterInputSource(IInputSource inputSource)
        {
            inputSource.FocusChanged -= InputSource_FocusChanged;
            inputSource.HoldCanceled -= InputSource_HoldCanceled;
            inputSource.HoldCompleted -= InputSource_HoldCompleted;
            inputSource.HoldStarted -= InputSource_HoldStarted;
            inputSource.ManipulationCanceled -= InputSource_ManipulationCanceled;
            inputSource.ManipulationCompleted -= InputSource_ManipulationCompleted;
            inputSource.ManipulationStarted -= InputSource_ManipulationStarted;
            inputSource.ManipulationUpdated -= InputSource_ManipulationUpdated;
            inputSource.SourceDown -= InputSource_SourceDown;
            inputSource.SourceUp -= InputSource_SourceUp;
            inputSource.SourceClicked -= InputSource_SourceClicked;
            inputSource.SourceDetected -= InputSource_SourceDetected;
            inputSource.SourceLost -= InputSource_SourceLost;
            inputSource.NavigationCanceled -= InputSource_NavigationCanceled;
            inputSource.NavigationCompleted -= InputSource_NavigationCompleted;
            inputSource.NavigationStarted -= InputSource_NavigationStarted;
            inputSource.NavigationUpdated -= InputSource_NavigationUpdated;
        }

        private void Start()
        {
            InitializeEventDatas();
        }

        private void InitializeEventDatas()
        {
            focusEventData = new FocusEventData(EventSystem.current);
            inputEventData = new InputEventData(EventSystem.current);
            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);
            navigationEventData = new NavigationEventData(EventSystem.current);
            holdEventData = new HoldEventData(EventSystem.current);
        }

        private void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
            where T : IEventSystemHandler
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            // Send the event to global listeners
            for (int i = 0; i < globalListeners.Count; i++)
            {
                // Global listeners should only get events on themselves, as opposed to their hierarchy
                ExecuteEvents.Execute(globalListeners[i], eventData, eventHandler);
            }

            // Handle modal input if one exists
            if (modalInputStack.Count > 0)
            {
                var modalInput = modalInputStack.Peek();

                // If there is a focused object in the hierarchy of the modal handler, start the event
                // bubble there
                GameObject focusedObject = GazeManager.Instance.HitObject;
                if (focusedObject != null && focusedObject.transform.IsChildOf(modalInput.transform))
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
            if (GazeManager.Instance.HitObject != null)
            {
                bool eventHandled = ExecuteEvents.ExecuteHierarchy(GazeManager.Instance.HitObject, eventData, eventHandler);
                if (eventHandled)
                {
                    return;
                }
            }

            // If event was not handled by the focused object, pass it on to any fallback handlers
            if (fallbackInputStack.Count > 0)
            {
                var fallbackInput = fallbackInputStack.Peek();
                ExecuteEvents.ExecuteHierarchy(fallbackInput, eventData, eventHandler);
            }
        }

        private void InputSource_FocusChanged(object sender, FocusChangedEventArgs e)
        {
            // Create input event
            focusEventData.Initialize(e.InputSource, e.SourceId, e.PreviousObject, e.NewObject);

            // Handler for execute events
            ExecuteEvents.EventFunction<IFocusHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnFocusChanged(focusEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(focusEventData, eventHandler);

            // UI events
            if (ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerClickHandler);
            }
        }

        private void InputSource_SourceClicked(object sender, SourceClickEventArgs e)
        {
            // Create input event
            inputEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IInputClickHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnInputClicked(inputEventData); };
            
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, eventHandler);

            // UI events
            if (ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerClickHandler);
            }
        }

        private void InputSource_SourceUp(object sender, InputSourceEventArgs e)
        {
            // Create input event
            inputEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IInputHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnInputUp(inputEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, eventHandler);

            // UI events
            if (ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerUpHandler);
            }
        }

        private void InputSource_SourceDown(object sender, InputSourceEventArgs e)
        {
            // Create input event
            inputEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IInputHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnInputDown(inputEventData); };
            
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, eventHandler);

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

        private void InputSource_SourceDetected(object sender, InputSourceEventArgs e)
        {
            // Create input event
            sourceStateEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<ISourceStateHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnSourceDetected(sourceStateEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, eventHandler);
        }

        private void InputSource_SourceLost(object sender, InputSourceEventArgs e)
        {
            // Create input event
            sourceStateEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<ISourceStateHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnSourceLost(sourceStateEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, eventHandler);
        }

        private void InputSource_ManipulationUpdated(object sender, ManipulationEventArgs e)
        {
            // Create input event
            manipulationEventData.Initialize(e.InputSource, e.SourceId, e.CumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationUpdated(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_ManipulationStarted(object sender, ManipulationEventArgs e)
        {
            // Create input event
            manipulationEventData.Initialize(e.InputSource, e.SourceId, e.CumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationStarted(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_ManipulationCompleted(object sender, ManipulationEventArgs e)
        {
            // Create input event
            manipulationEventData.Initialize(e.InputSource, e.SourceId, e.CumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationCompleted(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_ManipulationCanceled(object sender, ManipulationEventArgs e)
        {
            // Create input event
            manipulationEventData.Initialize(e.InputSource, e.SourceId, e.CumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationCanceled(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_HoldStarted(object sender, HoldEventArgs e)
        {
            // Create input event
            holdEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldStarted(holdEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, eventHandler);
        }

        private void InputSource_HoldCompleted(object sender, HoldEventArgs e)
        {
            // Create input event
            holdEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldCompleted(holdEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, eventHandler);
        }

        private void InputSource_HoldCanceled(object sender, HoldEventArgs e)
        {
            // Create input event
            holdEventData.Initialize(e.InputSource, e.SourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldCanceled(holdEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, eventHandler);
        }

        private void InputSource_NavigationUpdated(object sender, NavigationEventArgs e)
        {
            // Create input event
            navigationEventData.Initialize(e.InputSource, e.SourceId, e.NormalizedOffset);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationUpdated(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }

        private void InputSource_NavigationStarted(object sender, NavigationEventArgs e)
        {
            // Create input event
            navigationEventData.Initialize(e.InputSource, e.SourceId, e.NormalizedOffset);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationStarted(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }

        private void InputSource_NavigationCompleted(object sender, NavigationEventArgs e)
        {
            // Create input event
            navigationEventData.Initialize(e.InputSource, e.SourceId, e.NormalizedOffset);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationCompleted(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }

        private void InputSource_NavigationCanceled(object sender, NavigationEventArgs e)
        {
            // Create input event
            navigationEventData.Initialize(e.InputSource, e.SourceId, e.NormalizedOffset);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationCanceled(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }
    }
}