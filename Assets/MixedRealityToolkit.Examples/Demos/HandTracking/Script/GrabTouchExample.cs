// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [System.Obsolete("This component is no longer supported", true)]
    public class GrabTouchExample : MonoBehaviour, IMixedRealityTouchHandler, IMixedRealityInputHandler
    {
        [SerializeField]
        private MixedRealityInputAction grabAction = MixedRealityInputAction.None;

        private void Awake()
        {
            Debug.LogError(this.GetType().Name + " is deprecated");
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == grabAction)
            {
                GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
            }
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == grabAction)
            {
                GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }

        public void OnInputPressed(InputEventData<float> eventData) { }

        public void OnPositionInputChanged(InputEventData<Vector2> eventData) { }

        /// <summary>
        /// This Handler is called by a HandTrackingInputSource when a Touch action for that hand starts.
        /// </summary>
        /// <remarks>    
        /// A Touch action requires a target. a Touch action must occur inside the bounds of a gameObject.
        /// The eventData argument contains.
        /// </remarks>
        /// <param name="eventData">
        /// The argument passed contains information about the InputSource, the point in space where
        /// the Touch action occurred and the status of the Touch action.
        /// </param>
        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
        }

        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
        }

        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
        }
    }
}