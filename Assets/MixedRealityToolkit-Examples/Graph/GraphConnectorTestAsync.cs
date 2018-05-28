// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Graph;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Class to test GraphConnector using async model.
    /// </summary>
    public class GraphConnectorTestAsync : MonoBehaviour
    {
        /// <summary>
        /// Called by Unity on the frame when the script is enabled just before any of the Update methods is called the first time.
        /// </summary>
        private async void Start()
        {
            var graphConnector = GetComponent<GraphConnector>();

            // Gets "me" profile picture as an image.
            var graphResponseImage = await graphConnector.MakeRequestGetAsync<GraphResponseImage>(GraphConnector.GraphEndpoint, "/me/photo/$value");
            if (graphResponseImage.Succeeded)
            {
                Debug.LogFormat("Getting profile picture as image succeeded - {0}", graphResponseImage.Image);
            }
            else
            {
                Debug.LogErrorFormat("Getting profile picture as image failed - {0}", graphResponseImage.StatusCode);
            }

            // Gets "me" user data as a json string.
            var graphResponseJson = await graphConnector.MakeRequestGetAsync<GraphResponseJson>(GraphConnector.GraphEndpoint, "/me");
            if (graphResponseJson.Succeeded)
            {
                Debug.LogFormat("Getting user data as json string succeeded - {0}", graphResponseJson.Json);
            }
            else
            {
                Debug.LogErrorFormat("Getting user data as json string failed - {0}", graphResponseJson.StatusCode);
            }

            // Gets "me" user data as a object.
            var graphResponseObject = await graphConnector.MakeRequestGetAsync<GraphResponseObject<GraphProfile>>(GraphConnector.GraphEndpoint, "/me");
            if (graphResponseObject.Succeeded)
            {
                Debug.LogFormat("Getting user data as object succeeded - {0}", graphResponseObject.Object);
            }
            else
            {
                Debug.LogErrorFormat("Getting user data as object failed - {0}", graphResponseObject.StatusCode);
            }
        }
    }
}
