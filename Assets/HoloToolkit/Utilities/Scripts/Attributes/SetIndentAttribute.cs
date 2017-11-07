// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Sets the indent level for custom formatting
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SetIndentAttribute : Attribute
    {
        public int Indent { get; private set; }

        public SetIndentAttribute(int indent)
        {
            Indent = indent;
        }
    }
}