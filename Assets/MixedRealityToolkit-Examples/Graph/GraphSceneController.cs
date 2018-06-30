// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Graph;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Graph
{
    /// <summary>
    /// Controller class for GraphScene.
    /// </summary>
    public class GraphSceneController : MonoBehaviour
    {
        [SerializeField]
        private Text displayName = null;

        [SerializeField]
        private Text jobTitle = null;

        [SerializeField]
        private Image profileImage = null;

        /// <summary>
        /// Called by Unity on the frame when the script is enabled just before any of the Update methods is called the first time.
        /// </summary>
        private async void Start()
        {
            var graphConnector = new MsalGraphConnector();

            // Gets "me" user data as a object.
            var graphResponseObject = await graphConnector.MakeRequestGetAsync(GraphConnector.GraphEndpoint, "/me");
            if (graphResponseObject.Successful)
            {
                GraphProfile graphProfile = JsonUtility.FromJson<GraphProfile>(graphResponseObject.ResponseBody);

                displayName.text = graphProfile.displayName;

                jobTitle.text = graphProfile.jobTitle;
            }
            else
            {
                Debug.LogErrorFormat("Getting user data as object failed - {0}", graphResponseObject.ResponseCode);
            }

            // Gets "me" profile picture as an image.
            var graphResponseImage = await graphConnector.MakeRequestGetAsync(GraphConnector.GraphEndpoint, "/me/photo/$value");
            if (graphResponseImage.Successful)
            {
                Texture2D texture = new Texture2D(2, 2); // Creates empty texture

                if (texture.LoadImage(graphResponseImage.ResponseData))
                {
                    profileImage.sprite = Sprite.Create(
                        texture,
                        new Rect(0.0f, 0.0f, texture.width, texture.height),
                        Vector2.zero,
                        profileImage.pixelsPerUnit);
                }
            }
            else
            {
                Debug.LogErrorFormat("Getting profile picture as image failed - {0}", graphResponseImage.ResponseCode);
            }
        }
    }
}
