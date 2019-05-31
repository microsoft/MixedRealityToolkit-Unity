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
        [Header("Azure Spatial Anchor Prototyping Properties")]
        /// <summary>
        /// The Account Domain URL to use for Azure Spatial Anchors. NOTE: These values should be used for development/prototyping.
        /// In the shipping application its strongly recommended to use user-based or server-based AAD authentication approaches.
        /// </summary>
        [Tooltip("The Account Domain URL to use for Azure Spatial Anchors. NOTE: These values should be used for development/prototyping. In a shipping application its strongly recommended to use user-based or server-based AAD authentication approaches.")]
        public string AccountDomain;

        /// <summary>
        /// The Azure Spatial Anchors service Account Id from Azure portal. NOTE: These values should be used for development/prototyping.
        /// In the shipping application its strongly recommended to use user-based or server-based AAD authentication approaches.
        /// </summary>
        [Tooltip("The Azure Spatial Anchors service Account Id from Azure portal. NOTE: These values should be used for development/prototyping. In a shipping application its strongly recommended to use user-based or server-based AAD authentication approaches.")]
        public string AccountId;

        /// <summary>
        /// The Azure Spatial Anchors service Account Key from Azure portal. NOTE: These values should be used for development/prototyping.
        /// In the shipping application its strongly recommended to use user-based or server-based AAD authentication approaches.
        /// </summary>
        [Tooltip("The Azure Spatial Anchors service Account Key from Azure portal. NOTE: These values should be used for development/prototyping. In a shipping application its strongly recommended to use user-based or server-based AAD authentication approaches.")]
        public string AccountKey;

        [Header("Azure Active Directory Properties")]
        /// <summary>
        /// The AAD Access Token to use for Azure Spatial Anchors. NOTE: An AccessToken is not needed if the AccountDomain, AccountId and AccountKey have been populated.
        /// </summary>
        [Tooltip("The AAD Access Token to use for Azure Spatial Anchors. NOTE: An AccessToken is not needed if the AccountDomain, AccountId and AccountKey have been populated.")]
        public string AccessToken;

        /// <summary>
        /// The AAD Authentication Token to use for Azure Spatial Anchors. NOTE: An Authentication Token is not needed if the AccountDomain, AccountId and AccountKey have been populated.
        /// </summary>
        [Tooltip("The AAD Authentication Token to use for Azure Spatial Anchors. NOTE: An Authentication Token is not needed if the AccountDomain, AccountId and AccountKey have been populated.")]
        public string AuthenticationToken;

        public SpatialAnchorsConfiguration() { }
    }
}
