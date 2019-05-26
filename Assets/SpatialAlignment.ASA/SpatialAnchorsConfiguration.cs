// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// Azure Spatial Anchors configuration.
    /// </summary>
    [Serializable]
    public class SpatialAnchorsConfiguration
    {
        /// <summary>
        /// The AAD Access Token to use for Azure Spatial Anchors.
        /// </summary>
        [Tooltip("The AAD Access Token to use for Azure Spatial Anchors.")]
        public string AccessToken;

        /// <summary>
        /// The Account Domain URL to use for Azure Spatial Anchors.
        /// </summary>
        [Tooltip("The Account Domain URL to use for Azure Spatial Anchors.")]
        public string AccountDomain;

        /// <summary>
        /// The Azure Spatial Anchors service Account Id from Azure portal.
        /// </summary>
        [Tooltip("The Azure Spatial Anchors service Account Id from Azure portal.")]
        public string AccountId;

        /// <summary>
        /// The Azure Spatial Anchors service Account Key from Azure portal.
        /// </summary>
        [Tooltip("The Azure Spatial Anchors service Account Key from Azure portal.")]
        public string AccountKey;

        /// <summary>
        /// The AAD Authentication Token to use for Azure Spatial Anchors.
        /// </summary>
        [Tooltip("The AAD Authentication Token to use for Azure Spatial Anchors.")]
        public string AuthenticationToken;

        public SpatialAnchorsConfiguration() { }
    }
}
