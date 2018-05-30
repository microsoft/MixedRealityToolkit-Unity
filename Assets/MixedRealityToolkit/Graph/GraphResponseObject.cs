// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.WebRequestRest;
using UnityEngine;

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
        /// Called to cache the result of the REST operation.
        /// </summary>
        /// <param name="webResponse">The response return for the REST call.</param>
        public override void SetResponse(Response webResponse)
        {
            base.SetResponse(webResponse);

            if (webResponse.Successful)
            {
                Object = JsonUtility.FromJson<T>(WebResponse.ResponseBody);
            }
        }
    }
}
