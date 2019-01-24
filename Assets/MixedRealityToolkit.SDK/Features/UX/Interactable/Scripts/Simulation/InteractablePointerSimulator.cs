// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Simulation
{
    /// <summary>
    /// A way to test button state feedback while in the editor
    /// </summary>
    
    public class InteractablePointerSimulator : MonoBehaviour
    {
        public Interactable Button;
        public bool Focus;
        public bool Down;
        public bool Disabled;
        public bool Clicked;

        private bool? hasFocus;
        private bool? hasDown;
        private bool? isDisabled;
        private bool isClicked = false;

        private void Update()
        {
            if (Button == null)
            {
                return;
            }

            if (hasFocus != Focus)
            {
                Button.SetFocus(Focus);
                hasFocus = Focus;
            }

            if (hasDown != Down)
            {
                Button.SetPress(Down);
                hasDown = Down;
            }

            if (isDisabled != Disabled)
            {
                Button.SetDisabled(Disabled);
                isDisabled = Disabled;
            }

            if (isClicked != Clicked)
            {
                Button.OnPointerClicked(null);
                Clicked = isClicked;
            }
        }
    }
}
