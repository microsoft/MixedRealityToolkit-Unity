// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Class that represents a Graph request.
    /// </summary>
    /// <typeparam name="T">The returned type for graph response.</typeparam>
    public class GraphRequest<T> : IDisposable where T : GraphResponse, new()
    {
        /// <summary>
        /// Event that signals when the request is completed.
        /// </summary>
        private ManualResetEvent requestCompleted = new ManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphRequest{T}" /> class.
        /// </summary>
        /// <param name="endpoint">The Graph endpoint to call.</param>
        /// <param name="api">The Graph API to call.</param>
        /// <param name="body">The request body used for POST or PUT requests.</param>
        public GraphRequest(string endpoint, string api, string body = null)
        {
            Uri = string.Format("{0}{1}", endpoint, api);
            Body = body;
        }

        /// <summary>
        /// Gets the Graph URI to call.
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Gets the request body used for POST or PUT requests.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Gets the result of the request operation.
        /// </summary>
        public T Result { get; private set; }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            requestCompleted.Dispose();
        }

        /// <summary>
        /// Called to cache the result of the operation.
        /// </summary>
        /// <param name="request">The web request after the operation is complete.</param>
        public void SetResult(UnityWebRequest request)
        {
            Result = new T();
            Result.SetResponse(request);

            requestCompleted.Set();
        }

        /// <summary>
        /// Blocks until the request is completed. Should only be invoked from background threads.
        /// </summary>
        public void WaitForCompletion()
        {
            requestCompleted.WaitOne();
        }
    }
}
