// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Describes a source change event.
    /// </summary>
    /// <remarks>Source State events do not have an associated <see cref="MixedRealityInputAction"/>.</remarks>
    public class SourcePoseEventData<T> : SourceStateEventData
    {
        /// <summary>
        /// The new position of the input source.
        /// </summary>
        public T SourceData { get; private set; }

        /// <inheritdoc />
        public SourcePoseEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, T data)
        {
            Initialize(inputSource, controller);
            SourceData = data;
        }
    }
}