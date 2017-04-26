// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Color based theme for buttons
    /// </summary>
    public class BaseButtonTheme<Type> : MonoBehaviour
    {
        public string Tag = "default";

        public Type Default;
        public Type Focus;
        public Type Press;
        public Type Selected;
        public Type FocusSelected;
        public Type PressSelected;
        public Type Disabled;
        public Type DisabledSelected;

        public Type CurrentValue;
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
