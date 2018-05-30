// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.WebRequestRest;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Class that represents a Graph response.
    /// </summary>
    public class GraphResponse
    {
        public Response WebResponse { get; private set; }
        
        /// <summary>
        /// Gets a value indicating whether the Graph operation succeeded.
        /// </summary>
        public bool Successful
        {
            get
            {
                return WebResponse.Successful;
            }
        }

        /// <summary>
        /// Called to cache the result of the REST operation.
        /// </summary>
        /// <param name="webResponse">The response return for the REST call.</param>
        public virtual void SetResponse(Response webResponse)
        {
            WebResponse = webResponse;
        }
    }
}
