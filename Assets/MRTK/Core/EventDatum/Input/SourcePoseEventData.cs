// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, T data)
        {
            Initialize(inputSource, controller);
            SourceData = data;
        }
    }
}