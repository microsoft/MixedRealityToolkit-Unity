// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{
    public class XboxControllerHandlerBase : GamePadHandlerBase, IXboxControllerHandler
    {
        public virtual void OnXboxInputUpdate(XboxControllerEventData eventData)
        {
        }

        [Obsolete("Use XboxControllerMapping.GetButton_Up")]
        protected static bool OnButton_Up(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            return XboxControllerMapping.GetButton_Up(buttonType, eventData);
        }

        [Obsolete("Use XboxControllerMapping.GetButton_Pressed")]
        protected static bool OnButton_Pressed(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            return XboxControllerMapping.GetButton_Pressed(buttonType, eventData);
        }

        [Obsolete("Use XboxControllerMapping.GetButton_Down")]
        protected static bool OnButton_Down(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            return XboxControllerMapping.GetButton_Down(buttonType, eventData);
        }
    }
}
