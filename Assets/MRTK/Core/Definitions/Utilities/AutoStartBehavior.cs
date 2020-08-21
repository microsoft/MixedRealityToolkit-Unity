// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// This enumeration identifies two different ways to handle the startup behavior for a feature. 
    /// Both will warm up the component, ready for its use (e.g. connecting backend services or registering for events. 
    /// The first causes the feature to start immediately. The second allows the feature to be manually started at a later time.
    /// </summary>
    public enum AutoStartBehavior
    {
        /// <summary>
        /// Automatically start the feature
        /// </summary>
        AutoStart = 0,
        /// <summary>
        /// Delay the start of the feature until the user requests it to begin
        /// </summary>
        ManualStart
    }
}