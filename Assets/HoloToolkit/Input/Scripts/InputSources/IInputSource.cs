// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
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
        Navigation = 16,
        SpeechKeyword = 32
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
        event EventHandler<InputSourceEventArgs> SourceUp;

        /// <summary>
        /// Event triggered when the input source goes down. This is the equivalent of the pointer down event of a mouse.
        /// </summary>
        event EventHandler<InputSourceEventArgs> SourceDown;

        /// <summary>
        /// Event triggered when the input source clicks. This is the equivalent of the pointer clicked event of a mouse.
        /// </summary>
        event EventHandler<SourceClickEventArgs> SourceClicked;

        /// <summary>
        /// Event triggered when the input source is detected. This can happen for input sources that are context-specific.
        /// For example, a "hand" input source is detected when the user's hand is visible.
        /// </summary>
        event EventHandler<InputSourceEventArgs> SourceDetected;

        /// <summary>
        /// Event triggered when the input source is lost. This can happen for input sources that are context-specific.
        /// For example, a "hand" input source is lost when the user's hand is no longer visible.
        /// </summary>
        event EventHandler<InputSourceEventArgs> SourceLost;

        /// <summary>
        /// Event triggered when a hold gesture starts.
        /// </summary>
        event EventHandler<HoldEventArgs> HoldStarted;

        /// <summary>
        /// Event triggered when a hold gesture is completed.
        /// </summary>
        event EventHandler<HoldEventArgs> HoldCompleted;

        /// <summary>
        /// Event triggered when a hold gesture is canceled.
        /// </summary>
        event EventHandler<HoldEventArgs> HoldCanceled;

        /// <summary>
        /// Event triggered when a manipulation gesture starts.
        /// </summary>
        event EventHandler<ManipulationEventArgs> ManipulationStarted;

        /// <summary>
        /// Event triggered when a manipulation gesture is updated.
        /// </summary>
        event EventHandler<ManipulationEventArgs> ManipulationUpdated;

        /// <summary>
        /// Event triggered when a manipulation gesture is completed.
        /// </summary>
        event EventHandler<ManipulationEventArgs> ManipulationCompleted;

        /// <summary>
        /// Event triggered when a manipulation gesture is canceled.
        /// </summary>
        event EventHandler<ManipulationEventArgs> ManipulationCanceled;

        /// <summary>
        /// Event triggered when a navigation gesture starts.
        /// </summary>
        event EventHandler<NavigationEventArgs> NavigationStarted;

        /// <summary>
        /// Event triggered when a navigation gesture is updated.
        /// </summary>
        event EventHandler<NavigationEventArgs> NavigationUpdated;

        /// <summary>
        /// Event triggered when a navigation gesture is completed.
        /// </summary>
        event EventHandler<NavigationEventArgs> NavigationCompleted;

        /// <summary>
        /// Event triggered when a navigation gesture is canceled.
        /// </summary>
        event EventHandler<NavigationEventArgs> NavigationCanceled;

        /// <summary>
        /// Event triggered when a speech phrase is recognized.
        /// </summary>
        event EventHandler<SpeechKeywordRecognizedEventArgs> SpeechKeywordRecognized;

        /// <summary>
        /// Events supported by the input source.
        /// </summary>
        SupportedInputEvents SupportedEvents { get; }

        /// <summary>
        /// Returns the input info that that the input source can provide.
        /// </summary>
        SupportedInputInfo GetSupportedInputInfo(uint sourceId);

        /// <summary>
        /// Returns whether the input source supports the specified input info type.
        /// </summary>
        /// <param name="sourceId">ID of the source.</param>
        /// <param name="inputInfo">Input info type that we want to get information about.</param>
        bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo);

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
