// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public delegate void SourceUpEventDelegate(IInputSource inputSource, uint sourceId);
    public delegate void SourceDownEventDelegate(IInputSource inputSource, uint sourceId);
    public delegate void SourceClickedEventDelegate(IInputSource inputSource, uint sourceId);

    public delegate void SourceDetectedEventDelegate(IInputSource inputSource, uint sourceId);
    public delegate void SourceLostEventDelegate(IInputSource inputSource, uint sourceId);

    public delegate void HoldStartedEventDelegate(IInputSource inputSource, uint sourceId);
    public delegate void HoldCompletedEventDelegate(IInputSource inputSource, uint sourceId);
    public delegate void HoldCanceledEventDelegate(IInputSource inputSource, uint sourceId);

    public delegate void ManipulationStartedEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);
    public delegate void ManipulationUpdatedEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);
    public delegate void ManipulationCompletedEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);
    public delegate void ManipulationCanceledEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);

    public delegate void NavigationStartedEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);
    public delegate void NavigationUpdatedEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);
    public delegate void NavigationCompletedEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);
    public delegate void NavigationCanceledEventDelegate(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta);

    /// <summary>
    /// Flags used to indicate which input events are supported by an input source.
    /// </summary>
    [Flags]
    public enum SupportedInputEvents
    {
        SourceUpAndDown = 1,
        SourceClicked = 2,
        Hold = 4,
        Manipulation = 8,
        Navigation = 16
    }

    /// <summary>
    /// Flags used to indicate which input information is supported by an input source.
    /// </summary>
    [Flags]
    public enum SupportedInputInfo
    {
        None = 0,
        Position = 1,
        Orientation = 2,
    }

    /// <summary>
    /// Interface for an input source.
    /// An input source can be anything that a user can use to interact with a device.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>
        /// Event triggered when the input source goes up. This is the equivalent of the pointer up event of a mouse.
        /// </summary>
        event SourceUpEventDelegate SourceUp;

        /// <summary>
        /// Event triggered when the input source goes down. This is the equivalent of the pointer down event of a mouse.
        /// </summary>
        event SourceDownEventDelegate SourceDown;

        /// <summary>
        /// Event triggered when the input source clicks. This is the equivalent of the pointer clicked event of a mouse.
        /// </summary>
        event SourceClickedEventDelegate SourceClicked;

        /// <summary>
        /// Event triggered when the input source is detected. This can happen for input sources that are context-specific.
        /// For example, a "hand" input source is detected when the user's hand is visible.
        /// </summary>
        event SourceDetectedEventDelegate SourceDetected;

        /// <summary>
        /// Event triggered when the input source is lost. This can happen for input sources that are context-specific.
        /// For example, a "hand" input source is lost when the user's hand is no longer visible.
        /// </summary>
        event SourceLostEventDelegate SourceLost;

        /// <summary>
        /// Event triggered when a hold gesture starts.
        /// </summary>
        event HoldStartedEventDelegate HoldStarted;

        /// <summary>
        /// Event triggered when a hold gesture is completed.
        /// </summary>
        event HoldCompletedEventDelegate HoldCompleted;

        /// <summary>
        /// Event triggered when a hold gesture is canceled.
        /// </summary>
        event HoldCanceledEventDelegate HoldCanceled;

        /// <summary>
        /// Event triggered when a manipulation gesture starts.
        /// </summary>
        event ManipulationStartedEventDelegate ManipulationStarted;

        /// <summary>
        /// Event triggered when a manipulation gesture is updated.
        /// </summary>
        event ManipulationUpdatedEventDelegate ManipulationUpdated;

        /// <summary>
        /// Event triggered when a manipulation gesture is completed.
        /// </summary>
        event ManipulationCompletedEventDelegate ManipulationCompleted;

        /// <summary>
        /// Event triggered when a manipulation gesture is canceled.
        /// </summary>
        event ManipulationCanceledEventDelegate ManipulationCanceled;

        /// <summary>
        /// Event triggered when a navigation gesture starts.
        /// </summary>
        event NavigationStartedEventDelegate NavigationStarted;

        /// <summary>
        /// Event triggered when a navigation gesture is updated.
        /// </summary>
        event NavigationUpdatedEventDelegate NavigationUpdated;

        /// <summary>
        /// Event triggered when a navigation gesture is completed.
        /// </summary>
        event NavigationCompletedEventDelegate NavigationCompleted;

        /// <summary>
        /// Event triggered when a navigation gesture is canceled.
        /// </summary>
        event NavigationCanceledEventDelegate NavigationCanceled;

        /// <summary>
        /// Events supported by the input source.
        /// </summary>
        SupportedInputEvents SupportedEvents { get; }

        /// <summary>
        /// Input info that that the input source can provide.
        /// </summary>
        SupportedInputInfo SupportedInputInfo { get; }

        /// <summary>
        /// Returns the position of the input source, if available.
        /// Not all input sources have positional information.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the position should be retrieved.</param>
        /// <param name="position">Out parameter filled with the position if available, otherwise the zero vector.</param>
        /// <returns>True if a position was retrieved, false if not.</returns>
        bool TryGetPosition(uint sourceId, out Vector3 position);

        /// <summary>
        /// Returns the orientation of the input source, if available.
        /// Not all input sources have orientation information.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the position should be retrieved.</param>
        /// <param name="orientation">Out parameter filled with the orientation if available, otherwise the zero vector.</param>
        /// <returns>True if an orientation was retrieved, false if not.</returns>
        bool TryGetOrientation(uint sourceId, out Quaternion orientation);
    }
}
