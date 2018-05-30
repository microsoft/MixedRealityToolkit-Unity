// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Graph.Internal;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Unity script that enables access to the Graph.
    /// </summary>
    public class GraphConnector : MonoBehaviour
    {
        /// <summary>
        /// Official v1 Graph endpoint.
        /// </summary>
        public static readonly string GraphEndpoint = "https://graph.microsoft.com/v1.0/";

        /// <summary>
        /// Beta Graph endpoint.
        /// </summary>
        public static readonly string GraphBetaEndpoint = "https://graph.microsoft.com/beta/";

        /// <summary>
        /// The application id registered to access the Graph.
        /// </summary>
        [SerializeField]
        [Tooltip("The App Id registered in https://apps.dev.microsoft.com to access the Graph.")]
        private string graphAppId = "83f5c300-1163-48b5-9790-f7a86d02349c";

        /// <summary>
        /// The list of permissions required to access the Graph.
        /// </summary>
        [SerializeField]
        [Tooltip("List of all access scopes required for each Graph API used. It should be a subset (or match) the scopes registered in https://apps.dev.microsoft.com.")]
        private string[] graphAccessScopes = { "User.Read" };

        /// <summary>
        /// Token for testing in the Unity editor.
        /// </summary>
        [SerializeField]
        [Tooltip("Auth token to test Graph access in the Unity editor.")]
        private string testAuthToken = null;

        /// <summary>
        /// The interface used to authenticate the user.
        /// </summary>
        private IGraphAuthentication graphAuthentication;

        /// <summary>
        /// Get an access token for the graph scopes and app id. An attempt is first made to 
        /// acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
        /// </summary>
        /// <returns>The async task.</returns>
        public Task SignInAsync()
        {
            return graphAuthentication.SignInAsync(graphAccessScopes);
        }

        /// <summary>
        /// Method to sign out the current user.
        /// </summary>
        public void SignOut()
        {
            graphAuthentication.SignOut();
        }

        /// <summary>
        /// Makes GET request via Graph's REST APIs. 
        /// </summary>
        /// <typeparam name="T">The returned type for graph response.</typeparam>
        /// <param name="endpoint">The Graph endpoint to call.</param>
        /// <param name="api">The Graph API to call.</param>
        /// <returns>The async task.</returns>
        public async Task<T> MakeRequestGetAsync<T>(string endpoint, string api) where T : GraphResponse, new()
        {
            string authToken = string.Empty;

            if (Application.isEditor)
            {
                authToken = testAuthToken;

                // Passthrough all HTTPS traffic in the Unity Editor. Should not be used in production code.
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            }

            if (graphAuthentication != null && string.IsNullOrEmpty(authToken))
            {
                if (!graphAuthentication.IsAuthenticated)
                {
                    await graphAuthentication.SignInAsync(graphAccessScopes);
                }

                authToken = graphAuthentication.AuthToken;
            }

            GraphRequest<T> graphRequest = new GraphRequest<T>(authToken, endpoint, api);

            return await graphRequest.GetAsync();
        }

        /// <summary>
        /// Method invoked to create the authentication provider.
        /// </summary>
        /// <param name="graphAppId">The app id to access the Graph.</param>
        /// <returns>The authentication provider to use.</returns>
        protected virtual IGraphAuthentication CreateAuthenticationProvider(string graphAppId)
        {
            return null;
        }

        /// <summary>
        /// Called by Unity when initializing a MonoBehaviour.
        /// </summary>
        private void Awake()
        {
            graphAuthentication = CreateAuthenticationProvider(graphAppId);
        }
    }
}
