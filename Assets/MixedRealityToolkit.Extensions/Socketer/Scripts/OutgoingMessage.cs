// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    /// <summary>
    /// Wrapper for an outgoing message's payload
    /// </summary>
    public struct OutgoingMessage
    {
        public float time;
        public byte[] data;
    }
}
