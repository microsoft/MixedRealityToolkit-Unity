// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// A state with already defined state settting logic within an Interactive Element is considered a Core Interaction State.
    /// </summary>
    public enum CoreInteractionState
    {
        /// <summary>
        /// Represents the Default state. The Default state is only active when all other states are not active.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Represents the Focus state. This state supports both near and far interaction.
        /// </summary>
        Focus,

        /// <summary>
        /// Represents the Focus Near state. This is a near interaction state
        /// </summary>
        FocusNear,

        /// <summary>
        /// Represents the Focus state. This is a far interaction state. 
        /// </summary>
        FocusFar,

        /// <summary>
        /// Represents the Touch state. This is a near interaction state that also requires the attachment of a NearInteractionTouchable 
        /// component to register touch input. 
        /// </summary>
        Touch,

        /// <summary>
        /// Represents the Select Far state. This is a far interaction state. 
        /// </summary>
        SelectFar,

        /// <summary>
        /// Represents the Speech Keyword state.  This state is set when a speech keyword is recognized.
        /// </summary>
        SpeechKeyword,

        /// <summary>
        /// Represents the Clicked state. By default, this state is set through a far interaction selection.
        /// </summary>
        Clicked,

        /// <summary>
        /// Represents the Toggle On state. By default, this state is set through a far interaction selection.
        /// </summary>
        ToggleOn,

        /// <summary>
        /// Represents the Toggle Off state. By default, this state is set through a far interaction selection.
        /// </summary>
        ToggleOff
    }
}
