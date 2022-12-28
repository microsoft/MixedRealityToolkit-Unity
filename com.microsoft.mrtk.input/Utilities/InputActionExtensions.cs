// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Extensions that simplify the task of interacting with a Unity InputAction object.
    /// </summary>
    public static class InputActionExtensions
    {
        /// <summary>
        /// Checks if any active controls match this InputAction's bindings.
        /// </summary>
        /// <returns>True if <paramref name="action"/> is non-null and there are any number of controls matching its bindings.</returns>
        public static bool HasAnyControls(this InputAction action)
        {
            return action?.controls.Count > 0;
        }

        /* todo: as needed
        public static bool RaisedByGameController(this InputAction action)
        {
            return CheckForDeviceType(action, "mouse");
        }
        */

        /// <summary>
        /// Checks to see if the action was raised using a keyboard control.
        /// </summary>
        /// <param name="action">The action that was raised.</param>
        /// <returns>
        /// True if at least one of the controls used to raise the action was on a keyboard, or false.
        /// </returns>
        public static bool RaisedByKeyboard(this InputAction action)
        {
            return CheckDeviceType(action, "keyboard");
        }

        /// <summary>
        /// Checks to see if the action was raised using a mouse control.
        /// </summary>
        /// <param name="action">The action that was raised.</param>
        /// <returns>
        /// True if at least one of the controls used to raise the action was on a mouse, or false.
        /// </returns>
        public static bool RaisedByMouse(this InputAction action)
        {
            return CheckDeviceType(action, "mouse");
        }

        /// <summary>
        /// Checks to see if at least one of the devices used to raise a Unity InputAction
        /// matches the requested device name.
        /// </summary>
        /// <param name="action">The action that was raised.</param>
        /// <param name="deviceName">The name (ex: "mouse") of the desired device type.</param>
        /// <returns>
        /// True if at least one of the controls which raised the action was on the requested device type, or false.
        /// </returns>
        private static bool CheckDeviceType(InputAction action, string deviceName)
        {
            bool isUsed = false;

            foreach (InputControl control in action.controls)
            {
                if (control.IsActuated() && deviceName.ToLower().Equals(control.device.name.ToLower()))
                {
                    isUsed = true;
                    break;
                }
            }

            return isUsed;
        }
    }
}
