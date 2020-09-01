// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The states with already defined state settting logic within an Interactive Element.
    /// A CoreInteractionState can be added
    /// </summary>
    public enum CoreInteractionState
    {
        /// <summary>
        /// Represents the Default state.  The Default state is only active when all other states are not active.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Represents the Focus state.
        /// </summary>
        Focus,

        /// <summary>
        /// Represents the Touch state.  THIS STATE DOES NOT HAVE LOGIC DEFINED YET.
        /// </summary>
        Touch

        // Add more core states as the state logic is defined: Click, Grab, Speech Keyword, Pressed
    }
}
