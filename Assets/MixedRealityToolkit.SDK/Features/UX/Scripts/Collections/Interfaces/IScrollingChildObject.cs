// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Definition for a child of a ScrollingObject, used to propigate events deemed correct from the scrolling Object
    /// </summary>
    public interface IScrollingChildObject
    {
        /// <summary>
        /// Sent from ScrollingObject when a press release has occured.
        /// </summary>
        void OnScrollPressRelease();

        /// <summary>
        /// When a Touch motion has occurred, the ScrollingObject will send the handled touch event.
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of the ScrollingObject(transitive).
        /// In this case, the touch motion will be within the bounds of this child object.
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchStarted(HandTrackingInputEventData eventData = null);

        /// <summary>
        /// When a Touch motion ends, the ScrollingObject will send the handled touch event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of the ScrollingObject(transitive).
        /// In this case, the touch motion will be within the bounds of this child object.
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchCompleted(HandTrackingInputEventData eventData = null);

        /// <summary>
        /// When a Touch motion is updated, the ScrollingObject will send the handled touch event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of the ScrollingObject(transitive).
        /// In this case, the touch motion will be within the bounds of this child object.
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchUpdated(HandTrackingInputEventData eventData = null);
    }
}