// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Methods that make working with InputAction objects easier.
    /// </summary>
    public static class InputActionHelpers
    {
        /// <summary>
        /// Gets the input action from the specified reference.
        /// </summary>
        /// <param name="actionReference">Unity InputActionReference object.</param>
        /// <returns>The references Unity InputAction, or null.</returns>
        public static InputAction GetInputActionFromReference(InputActionReference actionReference)
        {
            if (actionReference == null) { return null; }
            return actionReference.action;
        }
    }
}
