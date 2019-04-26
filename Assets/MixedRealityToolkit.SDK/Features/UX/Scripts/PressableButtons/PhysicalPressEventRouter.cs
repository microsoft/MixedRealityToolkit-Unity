// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    ///<summary>
    /// This class exists to route <see cref="Microsoft.MixedReality.Toolkit.UI.PressableButton"/> events through to Interactable.
    /// The result is being able to have physical touch call Interactable.OnPointerClicked.
    ///</summary>
    public class PhysicalPressEventRouter : MonoBehaviour
    {
        [Tooltip("Interactable to which the press events are being routed. Defaults to the object of the component.")]
        public Interactable routingTarget;

        /// Enum specifying which button event causes a Click to be raised.
        public enum PhysicalPressEventBehavior
        {
            EventOnClickCompletion = 0,
            EventOnPress,
            EventOnTouch
        }
        public PhysicalPressEventBehavior InteractableOnClick = PhysicalPressEventBehavior.EventOnClickCompletion;

        private void Awake()
        {
            if (routingTarget == null)
            {
                routingTarget = GetComponent<Interactable>();
            }
        }

        public void OnHandPressTouched()
        {
            if (routingTarget != null)
            {
                routingTarget.SetPhysicalTouch(true);
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnTouch)
                {
                    routingTarget.SetPress(true);
                    routingTarget.OnPointerClicked(null);
                    routingTarget.SetPress(false);
                }
            }
        }

        public void OnHandPressUntouched()
        {
            if (InteractableOnClick == PhysicalPressEventBehavior.EventOnTouch &&
                routingTarget != null)
            {
                routingTarget.SetPhysicalTouch(false);
                routingTarget.SetPress(true);
            }
        }

        public void OnHandPressTriggered()
        {
            if (routingTarget != null)
            {
                routingTarget.SetPhysicalTouch(true);
                routingTarget.SetPress(true);
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnPress)
                {
                    routingTarget.OnPointerClicked(null);
                }
            }
        }

        public void OnHandPressCompleted()
        {
            if (routingTarget != null)
            {
                routingTarget.SetPhysicalTouch(true);
                routingTarget.SetPress(true);
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnClickCompletion)
                {
                    routingTarget.OnPointerClicked(null);
                }
                routingTarget.SetPress(false);
            }
        }
    }
}