// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Class used for Graph requests that return a json string.
    /// </summary>
    public class GraphResponseJson : GraphResponse
    {
        /// <summary>
        /// Gets the json string returned for the Graph request.
        /// </summary>
        public string Json { get; private set; }

        /// <summary>
        /// Called to cache the result of the operation.
        /// </summary>
        /// <param name="request">The web request after the operation is complete.</param>
        public override void SetResponse(UnityWebRequest request)
        {
            base.SetResponse(request);

            if (Succeeded)
            {
                Debug.Assert(ContentType.ToLower().Contains("application/json"), "Content type must contain this string.");
                Debug.Assert(ContentType.ToLower().Contains("charset=utf-8"), "Content type must contain this string.");

                Json = Encoding.UTF8.GetString(RawData);
            }
        }
    }
}
