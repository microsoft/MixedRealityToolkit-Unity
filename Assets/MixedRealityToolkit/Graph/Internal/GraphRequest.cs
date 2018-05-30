// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.WebRequestRest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Graph.Internal
{
    /// <summary>
    /// Class that represents a Graph request.
    /// </summary>
    /// <typeparam name="T">The returned type for graph response.</typeparam>
    public class GraphRequest<T> where T : GraphResponse, new()
    {
        /// <summary>
        /// The bearer token to use.
        /// </summary>
        private string authToken;

        /// <summary>
        /// The Graph URI to call.
        /// </summary>
        private string uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphRequest{T}" /> class.
        /// </summary>
        /// <param name="authToken">The bearer token to use.</param>
        /// <param name="endpoint">The Graph endpoint to call.</param>
        /// <param name="api">The Graph API to call.</param>
        public GraphRequest(string authToken, string endpoint, string api)
        {
            this.authToken = authToken;
            this.uri = $"{endpoint}{api}";
        }

        /// <summary>
        /// Makes GET request via Graph's REST APIs.
        /// </summary>
        /// <returns>The response data.</returns>
        public async Task<T> GetAsync()
        {
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {authToken}" },
                { "User-Agent", "MRTK" }
            };

            var response = await Rest.GetAsync(uri, headers);

            var result = new T();
            result.SetResponse(response);

            return result;
        }
    }
}
