// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class DragEnabler : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusChangedHandler
    {
        [SerializeField]
        private ManipulationHandler manipulatorToDisable = null;

        public void OnBeforeFocusChange(FocusEventData eventData) { }

        public void OnFocusChanged(FocusEventData eventData) { }

        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (manipulatorToDisable != null)
            {
                manipulatorToDisable.enabled = false;
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (manipulatorToDisable != null)
            {
                manipulatorToDisable.enabled = true;
            }
        }
    }
}