//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

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
        private List<GameObject> globalListeners = new List<GameObject>();

        private bool isRegisteredToGazeChanges;
        private int disabledRefCount;

        /// <summary>
        /// Should the Unity UI events be fired?
        /// </summary>
        public bool ShouldSendUnityUIEvents { get { return GazeManager.Instance.UnityUIPointerEvent != null && EventSystem.current != null; } }

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
        }

        private void Start()
        {
            if (GazeManager.Instance == null)
            {
                Debug.LogError("InputManager requires an active GazeManager in the scene");
            }

            RegisterGazeManager();
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
                if (ShouldSendUnityUIEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(previousObject, GazeManager.Instance.UnityUIPointerEvent, ExecuteEvents.pointerExitHandler);
                }
            }

            if (newObject != null)
            {
                ExecuteEvents.ExecuteHierarchy<IFocusable>(newObject, null, (inputHandler, eventData) => { inputHandler.OnFocusEnter(); });
                if (ShouldSendUnityUIEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(newObject, GazeManager.Instance.UnityUIPointerEvent, ExecuteEvents.pointerEnterHandler);
                }
            }
        }

        private void InputSource_SourceClicked(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            InputEventData inputEvent = new InputEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IInputHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnInputClicked(inputEvent); };
            
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);

            // UI events
            if (ShouldSendUnityUIEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerClickHandler);
            }
        }

        private void InputSource_SourceUp(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            InputEventData inputEvent = new InputEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IInputHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnInputUp(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);

            // UI events
            if (ShouldSendUnityUIEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerUpHandler);
            }
        }

        private void InputSource_SourceDown(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            InputEventData inputEvent = new InputEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IInputHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnInputDown(inputEvent); };
            
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);

            // UI events
            if (ShouldSendUnityUIEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;

                unityUIPointerEvent.eligibleForClick = true;
                unityUIPointerEvent.delta = Vector2.zero;
                unityUIPointerEvent.dragging = false;
                unityUIPointerEvent.useDragThreshold = true;
                unityUIPointerEvent.pressPosition = unityUIPointerEvent.position;
                unityUIPointerEvent.pointerPressRaycast = unityUIPointerEvent.pointerCurrentRaycast;

                HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerDownHandler);
            }
        }

        private void InputSource_SourceDetected(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            SourceStateEventData inputEvent = new SourceStateEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<ISourceStateHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnSourceDetected(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_SourceLost(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            SourceStateEventData inputEvent = new SourceStateEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<ISourceStateHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnSourceLost(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_ManipulationUpdated(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            ManipulationEventData inputEvent = new ManipulationEventData(EventSystem.current, inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationUpdated(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_ManipulationStarted(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            ManipulationEventData inputEvent = new ManipulationEventData(EventSystem.current, inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationStarted(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_ManipulationCompleted(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            ManipulationEventData inputEvent = new ManipulationEventData(EventSystem.current, inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationCompleted(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_ManipulationCanceled(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            ManipulationEventData inputEvent = new ManipulationEventData(EventSystem.current, inputSource, sourceId, cumulativeDelta);

            // Handler for execute events
            ExecuteEvents.EventFunction<IManipulationHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnManipulationCanceled(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_HoldStarted(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            HoldEventData inputEvent = new HoldEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldStarted(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_HoldCompleted(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            HoldEventData inputEvent = new HoldEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldCompleted(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }

        private void InputSource_HoldCanceled(IInputSource inputSource, uint sourceId)
        {
            // Create input event
            HoldEventData inputEvent = new HoldEventData(EventSystem.current, inputSource, sourceId);

            // Handler for execute events
            ExecuteEvents.EventFunction<IHoldHandler> eventHandler = (inputHandler, eventData) => { inputHandler.OnHoldCanceled(inputEvent); };

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEvent, eventHandler);
        }
    }
}