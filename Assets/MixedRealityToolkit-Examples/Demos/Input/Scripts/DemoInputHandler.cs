using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Input
{
    public class DemoInputHandler : BaseInputHandler, IMixedRealityInputHandler<Vector2>, IMixedRealitySourceStateHandler
    {
        [SerializeField]
        [Tooltip("The action that will be used for selecting objects.")]
        private MixedRealityInputAction selectAction;

        [SerializeField]
        [Tooltip("The action that will move the camera forward, back, left, and right.")]
        private MixedRealityInputAction movementAction;

        [SerializeField]
        [Tooltip("The action that will pivot the camera on it's axis.")]
        private MixedRealityInputAction rotateAction;

        [SerializeField]
        [Tooltip("The action that will move the camera up or down vertically.")]
        private MixedRealityInputAction heightAction;

        private Vector3 newPosition = Vector3.zero;

        private Vector3 newRotation = Vector3.zero;

        public void OnInputUp(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == selectAction)
            {
                Debug.Log($"OnInputUp {eventData.MixedRealityInputAction.Description}");
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == selectAction)
            {
                Debug.Log($"OnInputDown {eventData.MixedRealityInputAction.Description}");
            }
        }

        public void OnInputPressed(InputEventData<float> eventData)
        {
            if (eventData.MixedRealityInputAction == heightAction)
            {
                Debug.Log($"OnInputPressed {eventData.MixedRealityInputAction.Description} | Value: {eventData.InputData}");
                newPosition.x = 0f;
                newPosition.y = eventData.InputData;
                newPosition.z = 0f;
                gameObject.transform.position += newPosition;
            }
        }

        public void OnInputChanged(InputEventData<Vector2> eventData)
        {
            if (eventData.MixedRealityInputAction == movementAction)
            {
                Debug.Log($"OnInputChanged {eventData.MixedRealityInputAction.Description} | Value: {eventData.InputData}");
                newPosition.x = eventData.InputData.x;
                newPosition.y = 0f;
                newPosition.z = eventData.InputData.y;
                gameObject.transform.position += newPosition;
            }
            else if (eventData.MixedRealityInputAction == rotateAction)
            {
                Debug.Log($"OnInputChanged {eventData.MixedRealityInputAction.Description} | Value: {eventData.InputData}");
                newRotation.x = eventData.InputData.x;
                newRotation.y = eventData.InputData.y;
            }
        }

        [Obsolete]
        public void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            // Obsolete
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.Log($"OnSourceDetected {eventData.InputSource.SourceName}");
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            Debug.Log($"OnSourceLost {eventData.InputSource.SourceName}");
        }
    }
}