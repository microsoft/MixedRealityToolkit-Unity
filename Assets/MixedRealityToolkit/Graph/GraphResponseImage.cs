// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.WebRequestRest;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Class used for Graph requests that return a image.
    /// </summary>
    public class GraphResponseImage : GraphResponse
    {
        /// <summary>
        /// Gets the image returned for the Graph request.
        /// </summary>
        public Texture2D Image { get; private set; }

        /// <summary>
        /// Called to cache the result of the REST operation.
        /// </summary>
        /// <param name="webResponse">The response return for the REST call.</param>
        public override void SetResponse(Response webResponse)
        {
            base.SetResponse(webResponse);

            if (webResponse.Successful)
            {
                Image = new Texture2D(2, 2); // Creates empty texture
                Image.LoadImage(webResponse.ResponseData);
            }
        }
    }
}
