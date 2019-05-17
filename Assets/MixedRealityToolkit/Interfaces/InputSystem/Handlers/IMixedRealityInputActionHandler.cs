// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement for simple generic input.
    /// </summary>
    public interface IMixedRealityInputActionHandler : IMixedRealityBaseInputHandler
    {
        void OnActionStarted(BaseInputEventData eventData);
        void OnActionEnded(BaseInputEventData eventData);
    }
}