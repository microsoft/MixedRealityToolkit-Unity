// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Networking;

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
        /// Called to cache the result of the operation.
        /// </summary>
        /// <param name="request">The web request after the operation is complete.</param>
        public override void SetResponse(UnityWebRequest request)
        {
            base.SetResponse(request);

            if (Succeeded)
            {
                Debug.Assert(ContentType.ToLower().Contains("image/"), "Content type must contain this string.");

                Image = new Texture2D(2, 2); // Creates empty texture
                Image.LoadImage(RawData);
            }
        }
    }
}
