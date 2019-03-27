// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    /// <summary>
    /// Helper class for validating that an object implements or extends a specific type.
    /// </summary>
    public class FieldHelper
    {
        /// <summary>
        /// Used to assess whether an object implements or extends the type T
        /// </summary>
        /// <typeparam name="T">Type that the provided object should impelments or extends</typeparam>
        /// <param name="obj">Object that should implement the provided type</param>
        /// <returns>Returns true if the object implements or extends the provided type, Throws if the object does not implement said type.</returns>
        public static bool ValidateType<T>(object obj) where T : class
        {
            if (obj == null)
                return true;

            var castedObj = obj as T;
            if (castedObj == null)
            {
                throw new Exception("Object did not implement expected type: " + obj.ToString());
            }

            return true;
        }
    }
}
