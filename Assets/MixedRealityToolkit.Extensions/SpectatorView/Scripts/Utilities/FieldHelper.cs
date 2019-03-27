// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public class FieldHelper
    {
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
