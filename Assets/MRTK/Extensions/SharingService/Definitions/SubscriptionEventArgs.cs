// Copyright(c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Struct describing a subscription event.
    /// </summary>
    public struct SubscriptionEventArgs
    {
        /// <summary>
        /// The device's current subscription mode
        /// </summary>
        public SubscriptionMode Mode;

        /// <summary>
        /// The manual types specified for SubscriptionMode.Manual.
        /// </summary>
        public IEnumerable<short> Types;
    }
}