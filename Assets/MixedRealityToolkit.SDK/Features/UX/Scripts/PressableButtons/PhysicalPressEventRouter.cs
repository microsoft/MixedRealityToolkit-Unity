// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    ///<summary>
    /// This class exists to route <see cref="Microsoft.MixedReality.Toolkit.UI.PressableButton"/> events through to <see cref="Microsoft.MixedReality.Toolkit.UI.Interactable"/>.
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

        private bool CanRouteInput()
        {
            return routingTarget != null && routingTarget.IsEnabled;
        }

        /// <summary>
        /// Gets called when the TouchBegin event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components. When the physical touch with a 
        /// hand has begun, set physical touch state within Interactable. 
        /// </summary>
        public void OnHandPressTouched()
        {
            if (CanRouteInput())
            {
                routingTarget.HasPhysicalTouch = true;
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnTouch)
                {
                    routingTarget.HasPress = true;
                    routingTarget.TriggerOnClick();
                    routingTarget.HasPress = false;
                }
            }
        }

        /// <summary>
        /// Gets called when the TouchEnd event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components. Once the physical touch with a hand is removed, set
        /// the physical touch and possibly press state within Interactable.
        /// </summary>
        public void OnHandPressUntouched()
        {
            if (CanRouteInput())
            {
                routingTarget.HasPhysicalTouch = false;
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnTouch)
                {
                    routingTarget.HasPress = true;
                }
            }
        }

        /// <summary>
        /// Gets called when the ButtonPressed event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components. When the physical press with a hand is triggered, set 
        /// the physical touch and press state within Interactable. 
        /// </summary>
        public void OnHandPressTriggered()
        {
            if (CanRouteInput())
            {
                routingTarget.HasPhysicalTouch = true;
                routingTarget.HasPress = true;
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnPress)
                {
                    routingTarget.TriggerOnClick();
                }
            }
        }

        /// <summary>
        /// Gets called when the ButtonReleased event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components.  Once the physical press with a hand is completed, set
        /// the press and physical touch states within Interactable
        /// </summary>
        public void OnHandPressCompleted()
        {
            if (CanRouteInput())
            {
                routingTarget.HasPhysicalTouch = true;
                routingTarget.HasPress = true;
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnClickCompletion)
                {
                    routingTarget.TriggerOnClick();
                }
                routingTarget.HasPress = false;
                routingTarget.HasPhysicalTouch = false;
            }
        }
    }
}