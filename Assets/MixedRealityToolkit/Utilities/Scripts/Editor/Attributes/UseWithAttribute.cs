// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace MixedRealityToolkit.Utilities.Attributes
{
    // Indicates which components this class ought to be used with (though are not strictly required)
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UseWithAttribute : Attribute
    {
        public Type[] UseWithTypes { get; private set; }

        // IL2CPP doesn't support attributes with object arguments that are array types
        public UseWithAttribute(Type useWithType1, Type useWithType2 = null, Type useWithType3 = null, Type useWithType4 = null, Type useWithType5 = null)
        {
            List<Type> types = new List<Type>() { useWithType1 };

            if (useWithType2 != null)
                types.Add(useWithType2);

            if (useWithType3 != null)
                types.Add(useWithType3);

            if (useWithType4 != null)
                types.Add(useWithType4);

            if (useWithType5 != null)
                types.Add(useWithType5);

            UseWithTypes = types.ToArray();
        }
    }
}