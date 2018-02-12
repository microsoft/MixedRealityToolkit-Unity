// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Themes
{
    /// <summary>
    /// Generic base theme for buttons
    /// Button feedback can come in any form, scale, position, color, texture, etc...
    /// </summary>
    public class InteractiveTheme<Type> : MonoBehaviour
    {
        [Tooltip("Tag to help distinguish themes")]
        public string Tag = "default";

        [Tooltip("Default button state")]
        public Type Default;
        [Tooltip("Focus button state")]
        public Type Focus;
        [Tooltip("Pressed button state")]
        public Type Press;
        [Tooltip("Selected button state")]
        public Type Selected;
        [Tooltip("Focus Selected button state")]
        public Type FocusSelected;
        [Tooltip("Pressed Selected button state")]
        public Type PressSelected;
        [Tooltip("Disabled button state")]
        public Type Disabled;
        [Tooltip("Disabled Selected button state")]
        public Type DisabledSelected;

        [Tooltip("Current value : read only")]
        public Type CurrentValue;

        [Tooltip("Interactive host : optional")]
        public Interactive Button;
        private void Awake()
        {
            if (Button == null)
            {
                Button = GetComponent<Interactive>();
            }
        }

        public Type GetThemeValue(Interactive.ButtonStateEnum state) {
            switch (state)
            {
                case Interactive.ButtonStateEnum.Default:
                    CurrentValue = Default;
                    break;
                case Interactive.ButtonStateEnum.Focus:
                    CurrentValue = Focus;
                    break;
                case Interactive.ButtonStateEnum.Press:
                    CurrentValue = Press;
                    break;
                case Interactive.ButtonStateEnum.Selected:
                    CurrentValue = Selected;
                    break;
                case Interactive.ButtonStateEnum.FocusSelected:
                    CurrentValue = FocusSelected;
                    break;
                case Interactive.ButtonStateEnum.PressSelected:
                    CurrentValue = PressSelected;
                    break;
                case Interactive.ButtonStateEnum.Disabled:
                    CurrentValue = Disabled;
                    break;
                case Interactive.ButtonStateEnum.DisabledSelected:
                    CurrentValue = DisabledSelected;
                    break;
                default:
                    CurrentValue = Default;
                    break;
            }

            return CurrentValue;
        }
    }
}
