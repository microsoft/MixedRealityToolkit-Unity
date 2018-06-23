// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Graph;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Graph
{
    /// <summary>
    /// Class to test GraphConnector using async model.
    /// </summary>
    public class GraphConnectorTestAsync : MonoBehaviour
    {
        [SerializeField]
        private GraphConnectorProfile graphConnector;

        /// <summary>
        /// Called by Unity on the frame when the script is enabled just before any of the Update methods is called the first time.
        /// </summary>
        private async void Start()
        {
            if (graphConnector == null)
            {
                Debug.LogError("No Graph Profile found!");
                return;
            }

            // Gets "me" user data as a json string.
            var graphResponseJson = await graphConnector.GetAsync(GraphConnectorProfile.GraphEndpoint, "/me");
            if (graphResponseJson.Successful)
            {
                Debug.LogFormat("Getting user data as json string succeeded - {0}", graphResponseJson.ResponseBody);
            }
            else
            {
                Debug.LogErrorFormat("Getting user data as json string failed - {0}", graphResponseJson.ResponseCode);
            }

            // Gets "me" profile picture as an image.
            var graphResponseImage = await graphConnector.GetAsync(GraphConnectorProfile.GraphEndpoint, "/me/photo/$value");
            if (graphResponseImage.Successful)
            {
                var image = new Texture2D(2, 2); // Creates empty texture
                image.LoadImage(graphResponseImage.ResponseData);
                Debug.LogFormat("Getting profile picture as image succeeded - {0}", image);
            }
            else
            {
                Debug.LogErrorFormat("Getting profile picture as image failed - {0}", graphResponseImage.ResponseCode);
            }

            // Gets "me" user data as a object.
            var graphResponseObject = await graphConnector.GetAsync(GraphConnectorProfile.GraphEndpoint, "/me");
            if (graphResponseObject.Successful)
            {
                var profile = JsonUtility.FromJson<GraphProfileTestData>(graphResponseObject.ResponseBody);
                Debug.LogFormat("Getting user data as object succeeded - {0},{1}", profile.displayName, profile.jobTitle);
            }
            else
            {
                Debug.LogErrorFormat("Getting user data as object failed - {0}", graphResponseObject.ResponseCode);
            }
        }
    }
}
