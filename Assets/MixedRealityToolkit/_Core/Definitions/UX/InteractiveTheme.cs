// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{

    /// <summary>
    /// Interactive Theme base object, used for styling the Interactive Object themes for each type
    /// </summary>
    /// <typeparam name="Type"></typeparam>
    public class InteractiveTheme<Type> : ScriptableObject
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

        public Type GetThemeValue(ButtonStateEnum state)
        {
            switch (state)
            {
                case ButtonStateEnum.Default:
                    CurrentValue = Default;
                    break;
                case ButtonStateEnum.Focus:
                    CurrentValue = Focus;
                    break;
                case ButtonStateEnum.Press:
                    CurrentValue = Press;
                    break;
                case ButtonStateEnum.Selected:
                    CurrentValue = Selected;
                    break;
                case ButtonStateEnum.FocusSelected:
                    CurrentValue = FocusSelected;
                    break;
                case ButtonStateEnum.PressSelected:
                    CurrentValue = PressSelected;
                    break;
                case ButtonStateEnum.Disabled:
                    CurrentValue = Disabled;
                    break;
                case ButtonStateEnum.DisabledSelected:
                    CurrentValue = DisabledSelected;
                    break;
                default:
                    CurrentValue = Default;
                    break;
            }

            return CurrentValue;
        }
    }

    /// <summary>
    /// This Scriptable Object defines a theme / style for your interactive control
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/UX/InteractiveThemeColor", fileName = "MixedRealityInteractiveThemeColor", order = 0)]
    public class InteractiveThemeColor : InteractiveTheme<Color> { }

    /// <summary>
    /// This Scriptable Object defines a theme / style for your interactive control
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/UX/InteractiveThemeString", fileName = "MixedRealityInteractiveThemeString", order = 0)]
    public class InteractiveThemeString : InteractiveTheme<string> { }

    /// <summary>
    /// This Scriptable Object defines a theme / style for your interactive control
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/UX/InteractiveThemeTexture", fileName = "MixedRealityInteractiveThemeTexture", order = 0)]
    public class InteractiveThemeTexture : InteractiveTheme<Texture> { }

    /// <summary>
    /// This Scriptable Object defines a theme / style for your interactive control
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/UX/InteractiveThemeVector3", fileName = "MixedRealityInteractiveThemeVector3", order = 0)]
    public class InteractiveThemeVector3 : InteractiveTheme<Vector3> { }
}
