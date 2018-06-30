// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Configurations to access MS Graph.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Graph Access Profile", fileName = "MixedRealityGraphAccessProfile", order = 5)]
    public class MixedRealityGraphAccessProfile : ScriptableObject
    {        
        [SerializeField]
        [Tooltip("The App Id registered in https://apps.dev.microsoft.com to access the Graph.")]
        private string graphAppId = null;
                
        [SerializeField]
        [Tooltip("List of all access scopes required for each MS Graph API used. It should be a subset (or match) the scopes registered in https://apps.dev.microsoft.com.")]
        private string[] graphAccessScopes = { "User.Read" };
                
        [SerializeField]
        [Tooltip("Auth token to test MS Graph access in the Unity editor.")]
        private string testAuthToken = null;

        /// <summary>
        /// The application id registered to access the Graph.
        /// </summary>
        public string GraphAppId => graphAppId;

        /// <summary>
        /// The list of permissions required to access the Graph.
        /// </summary>
        public string[] GraphAccessScopes => graphAccessScopes;

        /// <summary>
        /// Token for testing in the Unity editor.
        /// </summary>
        public string TestAuthToken => testAuthToken;
    }
}
