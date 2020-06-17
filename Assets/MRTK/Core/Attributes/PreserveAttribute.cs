// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

/// <summary>
/// Custom preserve attribute that is inheritable.
/// </summary>
[AttributeUsage(
    AttributeTargets.Assembly | 
    AttributeTargets.Class | 
    AttributeTargets.Struct | 
    AttributeTargets.Enum | 
    AttributeTargets.Constructor | 
    AttributeTargets.Method | 
    AttributeTargets.Property | 
    AttributeTargets.Field | 
    AttributeTargets.Event | 
    AttributeTargets.Interface | 
    AttributeTargets.Delegate, 
    Inherited = true)]
public class MixedRealityToolkitPreserveAttribute : UnityEngine.Scripting.PreserveAttribute
{
    // No methods on PreserveAttribue for overriding.
}
