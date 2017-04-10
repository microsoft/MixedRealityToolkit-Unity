// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
#if UNITY_METRO && !UNITY_EDITOR
using System.Reflection;
#endif

namespace HoloToolkit
{
    public static class TypeUtils
    {
        public static Type GetBaseType(this Type type)
        {
#if UNITY_METRO && !UNITY_EDITOR
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }
    }
}