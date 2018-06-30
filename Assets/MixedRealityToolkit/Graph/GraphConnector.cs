// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.WebRequestRest;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Class that enables access to MS Graph.
    /// </summary>
    public abstract class GraphConnector
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
        /// The interface used to authenticate the user.
        /// </summary>
        private IGraphAuthentication graphAuthentication;

        /// <summary>
        /// GraphConnector constructor.
        /// </summary>
        protected GraphConnector()
        {
            graphAuthentication = CreateAuthenticationProvider(MixedRealityManager.Instance.ActiveProfile.GraphAccessProfile.GraphAppId);
        }

        /// <summary>
        /// Get an access token for the graph scopes and app id. An attempt is first made to 
        /// acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
        /// </summary>
        /// <returns>The async task.</returns>
        public Task SignInAsync()
        {
            return graphAuthentication.SignInAsync(MixedRealityManager.Instance.ActiveProfile.GraphAccessProfile.GraphAccessScopes);
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
        public async Task<Response> MakeRequestGetAsync(string endpoint, string api)
        {
            string authToken = string.Empty;

            if (Application.isEditor)
            {
                authToken = MixedRealityManager.Instance.ActiveProfile.GraphAccessProfile.TestAuthToken;
            }

            if (graphAuthentication != null && string.IsNullOrEmpty(authToken))
            {
                if (!graphAuthentication.IsAuthenticated)
                {
                    await graphAuthentication.SignInAsync(MixedRealityManager.Instance.ActiveProfile.GraphAccessProfile.GraphAccessScopes);
                }

                authToken = graphAuthentication.AuthToken;
            }

            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {authToken}" },
                { "User-Agent", "MRTK" }
            };

            return await Rest.GetAsync($"{endpoint}{api}", headers);
        }

        /// <summary>
        /// Method invoked to create the authentication provider.
        /// </summary>
        /// <param name="graphAppId">The app id to access the Graph.</param>
        /// <returns>The authentication provider to use.</returns>
        protected abstract IGraphAuthentication CreateAuthenticationProvider(string graphAppId);
    }
}
