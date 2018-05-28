// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Graph;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Class to test GraphConnector using Unity coroutine model.
    /// </summary>
    public class GraphConnectorTest : MonoBehaviour
    {
        /// <summary>
        /// Called by Unity on the frame when the script is enabled just before any of the Update methods is called the first time.
        /// </summary>
        /// <returns>The Unity coroutine.</returns>
        private IEnumerator Start()
        {
            var graphConnector = GetComponent<GraphConnector>();

            // Gets "me" profile picture as an image.
            using (var graphRequestImage = new GraphRequest<GraphResponseImage>(GraphConnector.GraphEndpoint, "/me/photo/$value"))
            {
                yield return graphConnector.MakeRequestGet(graphRequestImage);
                if (graphRequestImage.Result.Succeeded)
                {
                    Debug.LogFormat("Getting profile picture as image succeeded - {0}", graphRequestImage.Result.Image);
                }
                else
                {
                    Debug.LogErrorFormat("Getting profile picture as image failed - {0}", graphRequestImage.Result.StatusCode);
                }
            }

            // Gets "me" user data as a json string.
            using (var graphRequestJson = new GraphRequest<GraphResponseJson>(GraphConnector.GraphEndpoint, "/me"))
            {
                yield return graphConnector.MakeRequestGet(graphRequestJson);
                if (graphRequestJson.Result.Succeeded)
                {
                    Debug.LogFormat("Getting user data as json string succeeded - {0}", graphRequestJson.Result.Json);
                }
                else
                {
                    Debug.LogErrorFormat("Getting user data as json string failed - {0}", graphRequestJson.Result.StatusCode);
                }
            }

            // Gets "me" user data as a object.
            using (var graphRequestObject = new GraphRequest<GraphResponseObject<GraphProfile>>(GraphConnector.GraphEndpoint, "/me"))
            {
                yield return graphConnector.MakeRequestGet(graphRequestObject);
                if (graphRequestObject.Result.Succeeded)
                {
                    Debug.LogFormat("Getting user data as object succeeded - {0}", graphRequestObject.Result.Object);
                }
                else
                {
                    Debug.LogErrorFormat("Getting user data as object failed - {0}", graphRequestObject.Result.StatusCode);
                }
            }
        }
    }
}
