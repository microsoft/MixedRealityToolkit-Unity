// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Marks a field as 'not implemented' in editor without hiding it
    // Used for features that are in-progress but not fully completed
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FeatureInProgressAttribute : Attribute
    {
        public  FeatureInProgressAttribute() { }
    }
}