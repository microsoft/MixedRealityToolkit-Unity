// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the Touch InteractionState.
    /// </summary>
    public class TouchEvents : BaseInteractionEventConfiguration
    {
        /// <summary>
        /// A Unity event with HandTrackingInputEventData. This event is fired when Touch enters an object.
        /// </summary>
        public TouchInteractionEvent OnTouchStarted = new TouchInteractionEvent();

        /// <summary>
        /// A Unity event with HandTrackingInputEventData. This event is fired when Touch exits an object.
        /// </summary>
        public TouchInteractionEvent OnTouchCompleted = new TouchInteractionEvent();

        /// <summary>
        /// A Unity event with HandTrackingInputEventData. This event is fired when Touch is updated.
        /// </summary>
        public TouchInteractionEvent OnTouchUpdated = new TouchInteractionEvent();
    }
}
