// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.WebRequestRest;

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
        /// Called to cache the result of the REST operation.
        /// </summary>
        /// <param name="webResponse">The response return for the REST call.</param>
        public override void SetResponse(Response webResponse)
        {
            base.SetResponse(webResponse);

            if (webResponse.Successful)
            {
                Json = webResponse.ResponseBody;
            }
        }
    }
}
