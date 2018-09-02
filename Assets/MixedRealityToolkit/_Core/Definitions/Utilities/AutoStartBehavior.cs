// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities
{
    /// <summary>
    /// This enumeration gives the manager two different ways to handle the recognizer. Both will
    /// set up the recognizer and add all keywords. The first causes the recognizer to start
    /// immediately. The second allows the recognizer to be manually started at a later time.
    /// </summary>
    public enum AutoStartBehavior
    {
        /// <summary>
        /// Automatically start the speech recognizer
        /// </summary>
        AutoStart = 0,
        /// <summary>
        /// Delay the start of the speech recognizer until the user requests recognition to begin
        /// </summary>
        ManualStart
    }
}