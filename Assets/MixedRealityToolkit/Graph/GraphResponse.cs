// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Class that represents a Graph response.
    /// </summary>
    public class GraphResponse
    {
        /// <summary>
        /// Gets the HTTP status code for the operation.
        /// </summary>
        public long StatusCode { get; private set; } = 0;

        /// <summary>
        /// Gets the raw data returned for the Graph operation.
        /// </summary>
        public byte[] RawData { get; private set; }

        /// <summary>
        /// Gets the response headers returned for the operation.
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Graph operation succeeded.
        /// </summary>
        public bool Succeeded
        {
            get
            {
                return StatusCode >= 200 && StatusCode < 300;
            }
        }

        /// <summary>
        /// Gets the response header content type.
        /// </summary>
        public string ContentType
        {
            get
            {
                return Headers["Content-Type"];
            }
        }

        /// <summary>
        /// Called to cache the result of the operation.
        /// </summary>
        /// <param name="request">The web request after the operation is complete.</param>
        public virtual void SetResponse(UnityWebRequest request)
        {
            StatusCode = request.responseCode;

            if (!Succeeded)
            {
                Debug.LogErrorFormat("Request failed: {0} - {1}", request.responseCode, request.error);
            }
            else
            {
                RawData = request.downloadHandler.data;
                Headers = request.GetResponseHeaders();
            }
        }
    }
}
