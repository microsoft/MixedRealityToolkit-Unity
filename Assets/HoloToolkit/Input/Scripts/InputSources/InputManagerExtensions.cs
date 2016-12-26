// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public static partial class InputManagerExtensions
    {
        private static InputEventData inputEventData;
        private static SourceClickedEventData sourceClickedEventData;
        private static SourceStateEventData sourceStateEventData;
        private static ManipulationEventData manipulationEventData;
        private static HoldEventData holdEventData;
        private static NavigationEventData navigationEventData;

        static InputManagerExtensions()
        {
            inputEventData = new InputEventData(EventSystem.current);
            sourceClickedEventData = new SourceClickedEventData(EventSystem.current);
            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            manipulationEventData = new ManipulationEventData(EventSystem.current);
            holdEventData = new HoldEventData(EventSystem.current);
            navigationEventData = new NavigationEventData(EventSystem.current);
        }

        private static readonly ExecuteEvents.EventFunction<IInputClickHandler> OnInputClickedEventHandler =
            delegate (IInputClickHandler handler, BaseEventData eventData)
            {
                SourceClickedEventData casted = ExecuteEvents.ValidateEventData<SourceClickedEventData>(eventData);
                handler.OnInputClicked(casted);
            };

        public static void RaiseSourceClicked(this InputManager inputManager, IInputSource source, uint sourceId, int tapCount)
        {
            // Create input event
            sourceClickedEventData.Initialize(source, sourceId, tapCount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(sourceClickedEventData, OnInputClickedEventHandler);

            // UI events
            if (inputManager.ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                inputManager.HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerClickHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnSourceUpEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                InputEventData casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        public static void RaiseSourceUp(this InputManager inputManager, IInputSource source, uint sourceId)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(inputEventData, OnSourceUpEventHandler);

            // UI events
            if (inputManager.ShouldSendUnityUiEvents)
            {
                PointerEventData unityUIPointerEvent = GazeManager.Instance.UnityUIPointerEvent;
                inputManager.HandleEvent(unityUIPointerEvent, ExecuteEvents.pointerUpHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnSourceDownEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                InputEventData casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        public static void RaiseSourceDown(this InputManager inputManager, IInputSource source, uint sourceId)
        {
            // Create input event
            inputEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(inputEventData, OnSourceDownEventHandler);

            // UI events
            if (inputManager.ShouldSendUnityUiEvents)
            {
                PointerEventData unityUiPointerEvent = GazeManager.Instance.UnityUIPointerEvent;

                unityUiPointerEvent.eligibleForClick = true;
                unityUiPointerEvent.delta = Vector2.zero;
                unityUiPointerEvent.dragging = false;
                unityUiPointerEvent.useDragThreshold = true;
                unityUiPointerEvent.pressPosition = unityUiPointerEvent.position;
                unityUiPointerEvent.pointerPressRaycast = unityUiPointerEvent.pointerCurrentRaycast;

                inputManager.HandleEvent(unityUiPointerEvent, ExecuteEvents.pointerDownHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceDetectedEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                SourceStateEventData casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        public static void RaiseSourceDetected(this InputManager inputManager, IInputSource source, uint sourceId)
        {
            // Create input event
            sourceStateEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceLostEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                SourceStateEventData casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceLost(casted);
            };

        public static void RaiseSourceLost(this InputManager inputManager, IInputSource source, uint sourceId)
        {
            // Create input event
            sourceStateEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(sourceStateEventData, OnSourceLostEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationStartedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationStarted(casted);
            };

        public static void RaiseManipulationStarted(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(manipulationEventData, OnManipulationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationUpdatedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationUpdated(casted);
            };

        public static void RaiseManipulationUpdated(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(manipulationEventData, OnManipulationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCompletedEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCompleted(casted);
            };

        public static void RaiseManipulationCompleted(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(manipulationEventData, OnManipulationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IManipulationHandler> OnManipulationCanceledEventHandler =
            delegate (IManipulationHandler handler, BaseEventData eventData)
            {
                ManipulationEventData casted = ExecuteEvents.ValidateEventData<ManipulationEventData>(eventData);
                handler.OnManipulationCanceled(casted);
            };

        public static void RaiseManipulationCanceled(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 cumulativeDelta)
        {
            // Create input event
            manipulationEventData.Initialize(source, sourceId, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(manipulationEventData, OnManipulationCanceledEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldStartedEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldStarted(casted);
            };

        public static void RaiseHoldStarted(this InputManager inputManager, IInputSource source, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(holdEventData, OnHoldStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCompletedEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldCompleted(casted);
            };

        public static void RaiseHoldCompleted(this InputManager inputManager, IInputSource source, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(holdEventData, OnHoldCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IHoldHandler> OnHoldCanceledEventHandler =
            delegate (IHoldHandler handler, BaseEventData eventData)
            {
                HoldEventData casted = ExecuteEvents.ValidateEventData<HoldEventData>(eventData);
                handler.OnHoldCanceled(casted);
            };

        public static void RaiseHoldCanceled(this InputManager inputManager, IInputSource source, uint sourceId)
        {
            // Create input event
            holdEventData.Initialize(source, sourceId);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(holdEventData, OnHoldCanceledEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationStartedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationStarted(casted);
            };

        public static void RaiseNavigationStarted(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(navigationEventData, OnNavigationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationUpdatedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationUpdated(casted);
            };

        public static void RaiseNavigationUpdated(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(navigationEventData, OnNavigationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCompletedEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCompleted(casted);
            };

        public static void RaiseNavigationCompleted(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(navigationEventData, OnNavigationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<INavigationHandler> OnNavigationCanceledEventHandler =
            delegate (INavigationHandler handler, BaseEventData eventData)
            {
                NavigationEventData casted = ExecuteEvents.ValidateEventData<NavigationEventData>(eventData);
                handler.OnNavigationCanceled(casted);
            };

        public static void RaiseNavigationCanceled(this InputManager inputManager, IInputSource source, uint sourceId, Vector3 normalizedOffset)
        {
            // Create input event
            navigationEventData.Initialize(source, sourceId, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            inputManager.HandleEvent(navigationEventData, OnNavigationCanceledEventHandler);
        }

    }
}
