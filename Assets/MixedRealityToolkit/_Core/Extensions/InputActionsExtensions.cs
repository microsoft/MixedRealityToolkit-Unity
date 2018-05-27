// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Extensions
{
    /// <summary>
    /// Extension methods for the Mixed Reality Toolkit InputActions arrray
    /// </summary>
    public static class InputActionsExtensions
    {
        /// <summary>
        /// Get an Input Action by name from the current Array
        /// </summary>
        /// <param name="input">InputAction Array to query</param>
        /// <param name="actionName">Input Action name to search for</param>
        /// <returns>A specific InputAction if found, else null</returns>
        public static InputAction GetActionByName(this InputAction[] input, string actionName)
        {
            for (int i = 0; i < input?.Length; i++)
            {
                if (input[i].Description == actionName)
                {
                    return input[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Get the List of InputActions using a specific AxisType
        /// </summary>
        /// <param name="input">InputAction Array to query</param>
        /// <param name="axis">Axis type to search for</param>
        /// <returns>An array of InputActions if any have the specific AxisType, else null</returns>
        //public static InputAction[] GetActionsByAxis(this InputAction[] input, Definitions.AxisType axis)
        //{
        //    List<InputAction> axisInputActions = new List<InputAction>();
        //    for (int i = 0; i < input?.Length; i++)
        //    {
        //        if (input[i].Axis == axis)
        //        {
        //            axisInputActions.Add(input[i]);
        //        }
        //    }
        //    return axisInputActions.ToArray();
        //}
    }
}
