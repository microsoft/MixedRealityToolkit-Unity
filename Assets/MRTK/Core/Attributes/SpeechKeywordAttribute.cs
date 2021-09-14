// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute used to display a dropdown of registered keywords from the speech profile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SpeechKeywordAttribute : PropertyAttribute { }
}
