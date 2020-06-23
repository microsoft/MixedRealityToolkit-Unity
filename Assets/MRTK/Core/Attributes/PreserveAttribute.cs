// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Customized version of Unity's PreserveAttribute. This version makes the attribute inheritable.
    /// </summary>
    /// <remarks>
    /// Applying this attribute to a class, for example, instructs the Unity linker exclude it from
    /// byte code stripping. For more information on byte code stripping, please see
    /// https://docs.unity3d.com/Manual/ManagedCodeStripping.html.
    /// </remarks>
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
}
