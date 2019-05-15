// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class AssetCacheEntry<T> where T : class
    {
        public StringGuid AssetId;
        public T Asset;
    }
}