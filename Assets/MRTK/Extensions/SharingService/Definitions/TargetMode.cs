// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Mode describing which devices should receive data.
    /// </summary>
    public enum TargetMode
    {
        /// <summary>
        /// By default, everyone will receive the data including the sender.
        /// Subscription settings will apply.
        /// </summary>
        Default,

        /// <summary>
        /// Everyone except sender will receive the data. Subscription settings will apply.
        /// </summary>
        SkipSender,

        /// <summary>
        /// The Targets array will be used. Subscription settings will apply.
        /// </summary>
        Manual,
    }
}