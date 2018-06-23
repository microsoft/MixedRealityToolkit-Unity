// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Identity.Client;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.WebRequestRest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Profile that implements authentication using Microsoft Authentication Library (MSAL).
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Graph/Graph Connector Profile", fileName = "GraphConnectorProfile")]
    public class GraphConnectorProfile : ScriptableObject, IGraphAuthentication
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
        /// The <see cref="PublicClientApplication"/> for authentication with MSAL.
        /// </summary>
        private PublicClientApplication ClientApp => clientApp ?? (clientApp = new PublicClientApplication(graphAppId));
        private PublicClientApplication clientApp = null;

        /// <summary>
        /// The cached authentication result for the current user.
        /// </summary>
        private AuthenticationResult authResult;

        /// <inheritdoc />
        public string AuthToken => authResult?.AccessToken;

        /// <inheritdoc />
        public bool IsAuthenticated => authResult != null;

        private readonly Dictionary<string, string> authenticationHeaders = new Dictionary<string, string>
        {
            {"User-Agent", "MixedRealityToolkit-Unity"}
        };

        /// <inheritdoc />
        public async Task SignInAsync()
        {
            var user = ClientApp.Users.FirstOrDefault();

            try
            {
                authResult = await ClientApp.AcquireTokenSilentAsync(graphAccessScopes, user);
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    authResult = await ClientApp.AcquireTokenAsync(graphAccessScopes);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <inheritdoc />
        public void SignOut()
        {
            if (IsAuthenticated)
            {
                ClientApp.Remove(authResult.User);
            }

            authResult = null;
        }

        /// <summary>
        /// Makes GET request via Graph's REST APIs.
        /// </summary>
        /// <param name="endpoint">The Graph endpoint to call.</param>
        /// <param name="api">The Graph API to call.</param>
        /// <returns>The <see cref="Response"/></returns>
        public async Task<Response> GetAsync(string endpoint, string api)
        {
            string authToken = string.Empty;

            if (Application.isEditor)
            {
                authToken = testAuthToken;

                // Pass through all HTTPS traffic in the Unity Editor. Should not be used in production code.
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            }

            if (string.IsNullOrEmpty(authToken))
            {
                if (!IsAuthenticated)
                {
                    await SignInAsync();
                }
            }

            authToken = $"Bearer {authToken}";

            if (authenticationHeaders.ContainsKey("Authorization"))
            {
                authenticationHeaders["Authorization"] = authToken;
            }
            else
            {
                authenticationHeaders.Add("Authorization", authToken);
            }

            return await Rest.GetAsync($"{endpoint}{api}", authenticationHeaders);
        }
    }
}
