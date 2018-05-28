// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Class used for Graph requests that return a de-serializable object.
    /// </summary>
    /// <typeparam name="T">The returned object type.</typeparam>
    public class GraphResponseObject<T> : GraphResponse
    {
        /// <summary>
        /// Gets the object returned for the Graph request.
        /// </summary>
        public T Object { get; private set; }

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

                var json = Encoding.UTF8.GetString(RawData);
                Object = JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
