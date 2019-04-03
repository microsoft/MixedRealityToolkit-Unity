// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Describes an source state event that has a source id.
    /// <remarks>Source State events do not have an associated <see cref="Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction"/>.</remarks>
    /// </summary>
    public class HandPanEventData : BaseInputEventData
    {
        public Vector2 PanPosition
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public HandPanEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        public void Initialize(IMixedRealityInputSource source, Vector2 pos)
        {
            PanPosition = pos;
        }
    }
}
