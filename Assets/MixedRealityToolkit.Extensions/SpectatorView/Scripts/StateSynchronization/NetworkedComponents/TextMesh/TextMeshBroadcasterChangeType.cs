// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Flags]
    internal enum TextMeshBroadcasterChangeType : byte
    {
        None = 0x0,
        Text = 0x1,
        FontAndPlacement = 0x2
    }
}