// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement for simple generic input.
    /// </summary>
    public interface IMixedRealityInputHandler : IMixedRealityBaseInputHandler
    {
        /// <summary>
        /// Input Up updates from Interactions, Keys, or any other simple input.
        /// </summary>
        void OnInputUp(InputEventData eventData);

        /// <summary>
        /// Input Down updates from Interactions, Keys, or any other simple input.
        /// </summary>
        void OnInputDown(InputEventData eventData);
    }

    /// <summary>
    /// Interface to implement for more complex generic input.
    /// </summary>
    /// <typeparam name="T">The type of input to listen for.</typeparam>
    /// <remarks>
    /// Valid input types:
    /// </remarks>
    public interface IMixedRealityInputHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Raised input event updates from the type of input specified in the interface handler implementation.
        /// </summary>
        /// <remarks>
        /// The <see cref="Microsoft.MixedReality.Toolkit.Input.InputEventData{T}.InputData"/> is the current input data.
        /// </remarks>
        void OnInputChanged(InputEventData<T> eventData);
    }
}