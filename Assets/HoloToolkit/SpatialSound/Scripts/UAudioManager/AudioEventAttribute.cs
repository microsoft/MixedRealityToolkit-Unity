// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Attribute to mark up a string field to be drawn using the
    /// AudioEventPropertyDrawer
    /// This allows the UI to display a dropdown instead of a
    /// text entry field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AudioEventAttribute : PropertyAttribute
    {
        // Nothing to see Here, This only acts as a marker to help the editor.
    }
}