// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement for 3 Degree of Freedom input.
    /// </summary>
    public interface IMixedReality3DoFInputHandler : IMixedRealityInputHandler
    {
        /// <summary>
        /// 3 Degree of Freedom input update.
        /// </summary>
        /// <param name="eventData"></param>
        void On3DoFInputChanged(ThreeDofInputEventData eventData);
    }
}