// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// Test behaviour that simply prints out a message very time a supported event is received from the input module.
    /// This is used to make sure that the input module routes events appropriately to game objects.
    /// </summary>
    public class InputTest : MonoBehaviour, 
                             IInputHandler,
                             IInputClickHandler,
                             IFocusable, 
                             ISourceStateHandler,
                             IHoldHandler,
                             IManipulationHandler,
                             INavigationHandler
    {
        [Tooltip("Set to true if gestures update (ManipulationUpdated, NavigationUpdated) should be logged. Note that this can impact performance." )]
        public bool LogGesturesUpdateEvents = false;

        public void OnInputUp(InputEventData eventData)
        {
            Debug.LogFormat("OnInputUp\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnInputDown(InputEventData eventData)
        {
            Debug.LogFormat("OnInputDown\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            Debug.LogFormat("OnInputClicked\r\nSource: {0}  SourceId: {1} TapCount: {2}", eventData.InputSource, eventData.SourceId, eventData.TapCount);
        }

        public void OnFocusEnter()
        {
            Debug.Log("OnFocusEnter");
        }

        public void OnFocusExit()
        {
            Debug.Log("OnFocusExit");
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.LogFormat("OnSourceDetected\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            Debug.LogFormat("OnSourceLost\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnHoldStarted(HoldEventData eventData)
        {
            Debug.LogFormat("OnHoldStarted\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnHoldCompleted(HoldEventData eventData)
        {
            Debug.LogFormat("OnHoldCompleted\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
            Debug.LogFormat("OnHoldCanceled\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnManipulationStarted(ManipulationEventData eventData)
        {
            Debug.LogFormat("OnManipulationStarted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}", 
                eventData.InputSource, 
                eventData.SourceId, 
                eventData.CumulativeDelta.x,
                eventData.CumulativeDelta.y,
                eventData.CumulativeDelta.z);
        }

        public void OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (LogGesturesUpdateEvents)
            {
                Debug.LogFormat("OnManipulationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
                    eventData.InputSource,
                    eventData.SourceId,
                    eventData.CumulativeDelta.x,
                    eventData.CumulativeDelta.y,
                    eventData.CumulativeDelta.z);
            }
        }

        public void OnManipulationCompleted(ManipulationEventData eventData)
        {
            Debug.LogFormat("OnManipulationCompleted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta.x,
                eventData.CumulativeDelta.y,
                eventData.CumulativeDelta.z);
        }

        public void OnManipulationCanceled(ManipulationEventData eventData)
        {
            Debug.LogFormat("OnManipulationCanceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta.x,
                eventData.CumulativeDelta.y,
                eventData.CumulativeDelta.z);
        }

        public void OnNavigationStarted(NavigationEventData eventData)
        {
            Debug.LogFormat("OnNavigationStarted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta.x,
                eventData.CumulativeDelta.y,
                eventData.CumulativeDelta.z);
        }

        public void OnNavigationUpdated(NavigationEventData eventData)
        {
            if (LogGesturesUpdateEvents)
            {
                Debug.LogFormat("OnNavigationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
                    eventData.InputSource,
                    eventData.SourceId,
                    eventData.CumulativeDelta.x,
                    eventData.CumulativeDelta.y,
                    eventData.CumulativeDelta.z);
            }
        }

        public void OnNavigationCompleted(NavigationEventData eventData)
        {
            Debug.LogFormat("OnNavigationCompleted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta.x,
                eventData.CumulativeDelta.y,
                eventData.CumulativeDelta.z);
        }

        public void OnNavigationCanceled(NavigationEventData eventData)
        {
            Debug.LogFormat("OnNavigationCanceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
                eventData.InputSource,
                eventData.SourceId,
                eventData.CumulativeDelta.x,
                eventData.CumulativeDelta.y,
                eventData.CumulativeDelta.z);
        }
    }
}