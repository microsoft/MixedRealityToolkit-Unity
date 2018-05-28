// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Graph.Internal
{
    /// <summary>
    /// Class that implements authentication using MSAL.
    /// </summary>
    public class MsalGraphAuthentication : IGraphAuthentication
    {
        /// <summary>
        /// The MSAL SDK used for auth.
        /// </summary>
        private PublicClientApplication clientApp;

        /// <summary>
        /// The cached authentication result for the current user.
        /// </summary>
        private AuthenticationResult authResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsalGraphAuthentication" /> class.
        /// </summary>
        /// <param name="graphAppId">The application id registered to access the Graph.</param>
        public MsalGraphAuthentication(string graphAppId)
        {
            clientApp = new PublicClientApplication(graphAppId);
        }
        
        /// <summary>
        /// Gets the cached token, if any.
        /// </summary>
        public string AuthToken
        {
            get
            {
                return authResult == null ? null : authResult.AccessToken;
            }
        }

        /// <summary>
        /// Gets a value indicating whether user is currently authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return authResult != null;
            }
        }

        /// <summary>
        /// Get an access token for the graph scopes and app id. An attempt is first made to 
        /// acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
        /// </summary>
        /// <param name="graphScopes">The Graph scopes required for the auth token being requested.</param>
        /// <returns>The async task.</returns>
        public async Task SignInAsync(string[] graphScopes)
        {
            var user = clientApp.Users.FirstOrDefault();

            try
            {
                authResult = await clientApp.AcquireTokenSilentAsync(graphScopes, user);
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    authResult = await clientApp.AcquireTokenAsync(graphScopes);
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

        /// <summary>
        /// Method to sign out the current user.
        /// </summary>
        public void SignOut()
        {
            if (IsAuthenticated)
            {
                clientApp.Remove(authResult.User);
            }

            authResult = null;
        }
    }
}
