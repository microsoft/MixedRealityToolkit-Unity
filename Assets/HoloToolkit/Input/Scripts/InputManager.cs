// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        public Cursor ActiveCursor;

        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <summary>
        /// Global listeners listen to all events and ignore the fact that other components might have consumed them.
        /// </summary>
        private readonly List<GameObject> globalListeners = new List<GameObject>();

        private bool isRegisteredToGazeChanges;
        private int disabledRefCount;

        private InputEventData inputEventData;
        private SourceStateEventData sourceStateEventData;
        private ManipulationEventData manipulationEventData;
        private NavigationEventData navigationEventData;
        private HoldEventData holdEventData;

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

            if (ActiveCursor != null)
            {
                ActiveCursor.DisableInput();
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

            if (ActiveCursor != null && disabledRefCount == 0)
            {
                ActiveCursor.EnableInput();
            }
        }

        /// <summary>
        /// Clear the input disable stack, which will immediately
        /// re enable input.
        /// </summary>
        public void ClearInputDisableStack()
        {
            disabledRefCount = 0;
            ActiveCursor.EnableInput();
        }

        /// <summary>
        /// Register an input source with the input manager to make it start listening
        /// to events from it.
        /// </summary>
        /// <param name="inputSource">The input source to register</param>
        public void RegisterInputSource(IInputSource inputSource)
        {
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

            if (GazeManager.Instance == null)
            {
                Debug.LogError("InputManager requires an active GazeManager in the scene");
            }

            RegisterGazeManager();
        }

        private void InitializeEventDatas()
        {
            inputEventData = new InputEventData(EventSystem.current);
            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);
            navigationEventData = new NavigationEventData(EventSystem.current);
            holdEventData = new HoldEventData(EventSystem.current);
        }

        protected override void OnDestroy()
        {
            UnregisterGazeManager();
        }

        private void OnEnable()
        {
            RegisterGazeManager();
        }

        private void OnDisable()
        {
            UnregisterGazeManager();
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

        private void GazeManager_FocusedChanged(GameObject previousObject, GameObject newObject)
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            if (previousObject != null)
            {
                ExecuteEvents.ExecuteHierarchy<IFocusable>(previousObject, null, (inputHandler, eventData) => { inputHandler.OnFocusExit(); });
                if (ShouldSendUnityUiEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(previousObject, GazeManager.Instance.UnityUIPointerEvent, ExecuteEvents.pointerExitHandler);
                }
            }

            if (newObject != null)
            {
                ExecuteEvents.ExecuteHierarchy<IFocusable>(newObject, null, (inputHandler, eventData) => { inputHandler.OnFocusEnter(); });
                if (ShouldSendUnityUiEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(newObject, GazeManager.Instance.UnityUIPointerEvent, ExecuteEvents.pointerEnterHandler);
                }
            }
        }

        private void InputSource_SourceClicked(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            inputEventData.Initialize(inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IInputHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnInputClicked(inputEventData); };
            
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, eventHandler);

            // UI events
            if (ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerClickHandler);
            }
        }

        private void InputSource_SourceUp(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            inputEventData.Initialize(inputSource, sourceId);

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

        private void InputSource_SourceDown(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            inputEventData.Initialize(inputSource, sourceId);

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

        private void InputSource_SourceDetected(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            sourceStateEventData.Initialize(inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<ISourceStateHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnSourceDetected(sourceStateEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, eventHandler);
        }

        private void InputSource_SourceLost(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            sourceStateEventData.Initialize(inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<ISourceStateHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnSourceLost(sourceStateEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, eventHandler);
        }

        private void InputSource_ManipulationUpdated(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationUpdated(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_ManipulationStarted(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationStarted(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_ManipulationCompleted(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationCompleted(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_ManipulationCanceled(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationCanceled(manipulationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(manipulationEventData, eventHandler);
        }

        private void InputSource_HoldStarted(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldStarted(holdEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, eventHandler);
        }

        private void InputSource_HoldCompleted(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldCompleted(holdEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, eventHandler);
        }

        private void InputSource_HoldCanceled(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldCanceled(holdEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(holdEventData, eventHandler);
        }

        private void InputSource_NavigationUpdated(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            navigationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationUpdated(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }

        private void InputSource_NavigationStarted(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            navigationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationStarted(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }

        private void InputSource_NavigationCompleted(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            navigationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationCompleted(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }

        private void InputSource_NavigationCanceled(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            navigationEventData.Initialize(inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<INavigationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnNavigationCanceled(navigationEventData); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(navigationEventData, eventHandler);
        }
    }
}