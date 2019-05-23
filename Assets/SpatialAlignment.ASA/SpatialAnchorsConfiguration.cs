// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    [Serializable]
    public class SpatialAnchorsConfiguration
    {
        public string AccessToken;

        public string AccountDomain;

        public string AccountId;

        public string AccountKey;

        public string AuthenticationToken;

        public SpatialAnchorsConfiguration() { }
    }
}
