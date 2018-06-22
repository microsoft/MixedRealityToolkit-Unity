// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an source state event that has a source id.
    /// <remarks>Source State events do not have an associated Input Action.</remarks>
    /// </summary>
    public class SourceStateEventData : BaseInputEventData
    {
        /// <inheritdoc />
        public SourceStateEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        public void Initialize(IMixedRealityInputSource inputSource)
        {
            // NOTE: Source State events do not have an associated Input Action.
            BaseInitialize(inputSource, Definitions.InputSystem.InputAction.None);
        }
    }
}
