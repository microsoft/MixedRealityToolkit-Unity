// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for handling mouse pointers.
    /// </summary>
    public interface IMixedRealityMousePointer : IMixedRealityPointer
    {
        /// <summary>
        /// Current Mouse Screen Position.
        /// </summary>
        Vector3 ScreenPosition { get; }
    }
}