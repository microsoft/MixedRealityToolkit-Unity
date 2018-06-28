// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement input for 3 Degrees of Freedom.
    /// </summary>
    public interface IMixedReality3DoFInputHandler : IMixedRealityInputHandler
    {
        /// <summary>
        /// Raised when the input source's position has changed.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPositionChanged(PositionInputEventData eventData);

        /// <summary>
        /// Raised when the input source's rotation has changed.
        /// </summary>
        /// <param name="eventData"></param>
        void OnRotationChanged(RotationInputEventData eventData);
    }
}