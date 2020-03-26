// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to receive input action events.
    /// </summary>
    public interface IMixedRealityInputActionHandler : IMixedRealityBaseInputHandler
    {
        /// <summary>
        /// Received on action start, e.g when a button is pressed or a gesture starts.
        /// </summary>
        /// <param name="eventData">Input event that triggered the action</param>
        void OnActionStarted(BaseInputEventData eventData);

        /// <summary>
        /// Received on action end, e.g when a button is released or a gesture completed.
        /// </summary>
        /// <param name="eventData">Input event that triggered the action</param>
        void OnActionEnded(BaseInputEventData eventData);
    }
}