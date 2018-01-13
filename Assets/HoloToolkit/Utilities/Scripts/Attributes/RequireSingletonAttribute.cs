// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Indicates which components this class ought to be used with (though are not strictly required)
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequireSingletonAttribute : Attribute
    {
        public Type SingletonType { get; private set; }
        
        public RequireSingletonAttribute(Type singletonType)
        {
            SingletonType = singletonType;
        }
    }
}