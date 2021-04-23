// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A way to test button state feedback while in the editor
    /// </summary>

    [AddComponentMenu("Scripts/MRTK/SDK/InteractablePointerSimulator")]
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
                Button.HasFocus = Focus;
                hasFocus = Focus;
            }

            if (hasDown != Down)
            {
                Button.HasPress = Down;
                hasDown = Down;
            }

            if (isDisabled != Disabled)
            {
                Button.IsEnabled = !Disabled;
                isDisabled = Disabled;
            }

            if (isClicked != Clicked)
            {
                Button.TriggerOnClick(true);
                Clicked = isClicked;
            }
        }
    }
}
