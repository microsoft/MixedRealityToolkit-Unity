// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Describes an source state event that has a source id.
    /// </summary>
    /// <remarks>Source State events do not have an associated <see cref="MixedRealityInputAction"/>.</remarks>
    public class SourceStateEventData : BaseInputEventData
    {
        public IMixedRealityController Controller { get; private set; }

        /// <inheritdoc />
        public SourceStateEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller)
        {
            // NOTE: Source State events do not have an associated Input Action.
            BaseInitialize(inputSource, MixedRealityInputAction.None);
            Controller = controller;
        }
    }
}
