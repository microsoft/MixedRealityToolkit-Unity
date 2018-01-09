// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// Test MonoBehaviour that simply prints out a message very time a supported event is received from the input module.
    /// This is used to make sure that the input module routes events appropriately to game objects.
    /// </summary>
    public class InputTest : MonoBehaviour, IInputHandler, IPointerHandler, IFocusHandler, ISourceStateHandler, IHoldHandler, IManipulationHandler, INavigationHandler
    {
        [Tooltip("Set to true if gestures update (ManipulationUpdated, NavigationUpdated) should be logged. Note that this can impact performance.")]
        public bool LogGesturesUpdateEvents = false;

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            Debug.LogFormat("RaiseOnInputUp\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            Debug.LogFormat("OnInputDown\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IInputHandler.OnInputPressed(InputPressedEventData eventData)
        {
            Debug.LogFormat("OnInputPressed\r\nSource: {0}  SourceId: {1}  PressedAmount: {2}", eventData.InputSource, eventData.SourceId, eventData.PressedAmount);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IInputHandler.OnInputPositionChanged(InputPositionEventData eventData)
        {
            Debug.LogFormat("OnInputPressed\r\nSource: {0}  SourceId: {1}  InputPosition: {2}", eventData.InputSource, eventData.SourceId, eventData.InputPosition);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IPointerHandler.OnPointerUp(ClickEventData eventData)
        {
            Debug.LogFormat("OnPointerUp\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IPointerHandler.OnPointerDown(ClickEventData eventData)
        {
            Debug.LogFormat("OnPointerDown\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IPointerHandler.OnPointerClicked(ClickEventData eventData)
        {
            Debug.LogFormat("OnPointerClicked\r\nSource: {0}  SourceId: {1}  ClickCount: {2}", eventData.InputSource, eventData.SourceId, eventData.ClickCount);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IFocusHandler.OnFocusEnter(FocusEventData eventData)
        {
            Debug.LogFormat("OnFocusEnter: {0}\r\nPointer: {0} | Pointer: {1} | SourceId: {2}", gameObject.name, eventData.Pointer, eventData.Pointer.SourceId);
        }

        void IFocusHandler.OnFocusExit(FocusEventData eventData)
        {
            Debug.LogFormat("OnFocusExit: {0}\r\nPointer: {0} | Pointer: {1} | SourceId: {2}", gameObject.name, eventData.Pointer, eventData.Pointer.SourceId);
        }

        void IFocusHandler.OnFocusChanged(FocusEventData eventData)
        {
            Debug.LogFormat("OnFocusChanged\r\nPointer: {0} | Pointer SourceId: {1} | Old Focused Object: {2} | New Focused Object: {3}",
                            eventData.Pointer,
                            eventData.Pointer.SourceId,
                            eventData.OldFocusedObject == null ? "None" : eventData.OldFocusedObject.name,
                            eventData.NewFocusedObject == null ? "None" : eventData.NewFocusedObject.name);
        }

        void ISourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.LogFormat("OnSourceDetected\r\nSource: {0} | SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void ISourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            Debug.LogFormat("OnSourceLost\r\nSource: {0} | SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void ISourceStateHandler.OnSourcePositionChanged(SourcePositionEventData eventData)
        {
            Debug.LogFormat("OnSourcePositionChanged\r\nSource: {0} | SourceId: {1} | Pointer Position: {2} | Grip Position: {3}", eventData.InputSource, eventData.SourceId, eventData.PointerPosition, eventData.GripPosition);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void ISourceStateHandler.OnSourceRotationChanged(SourceRotationEventData eventData)
        {
            Debug.LogFormat("OnSourceRotationChanged\r\nSource: {0} | SourceId: {1} | Pointer Rotation: {2} | Grip Rotation: {3}", eventData.InputSource, eventData.SourceId, eventData.PointerRotation, eventData.GripRotation);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IHoldHandler.OnHoldStarted(InputEventData eventData)
        {
            Debug.LogFormat("OnHoldStarted\r\nSource: {0} | SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IHoldHandler.OnHoldCompleted(InputEventData eventData)
        {
            Debug.LogFormat("OnHoldCompleted\r\nSource: {0} | SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IHoldHandler.OnHoldCanceled(InputEventData eventData)
        {
            Debug.LogFormat("OnHoldCanceled\r\nSource: {0} | SourceId: {1}", eventData.InputSource, eventData.SourceId);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
        {
            Debug.LogFormat("OnManipulationStarted\r\nSource: {0} | SourceId: {1}\r\nCumulativeDelta: {2}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta);

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (LogGesturesUpdateEvents)
            {
                Debug.LogFormat("OnManipulationUpdated\r\nSource: {0} | SourceId: {1}\r\nCumulativeDelta: {2}",
                    eventData.InputSource,
                    eventData.SourceId,
                    eventData.CumulativeDelta);

                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
            }
        }

        void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
        {
            Debug.LogFormat("OnManipulationCompleted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta);

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
        {
            Debug.LogFormat("OnManipulationCanceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta);

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
        {
            Debug.LogFormat("OnNavigationStarted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.NormalizedOffset);

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void INavigationHandler.OnNavigationUpdated(NavigationEventData eventData)
        {
            if (LogGesturesUpdateEvents)
            {
                Debug.LogFormat("OnNavigationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2}",
                    eventData.InputSource,
                    eventData.SourceId,
                    eventData.NormalizedOffset);

                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
            }
        }

        void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
        {
            Debug.LogFormat("OnNavigationCompleted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.NormalizedOffset);

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
        {
            Debug.LogFormat("OnNavigationCanceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.NormalizedOffset);

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}