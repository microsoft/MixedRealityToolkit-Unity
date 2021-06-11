// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// An abstract data consumer that will fetch any supported image type from a URL
    /// and create a Texture.
    /// </summary>
    /// <remarks>
    /// This can be used as a base implementation for updating sprites, quads or any other element that
    /// expects an image texture
    /// 
    /// There are 2 methods to implement:
    /// 
    ///     internal abstract void InitializeForComponent(Type componentType, Component component);
    ///     internal abstract void PlaceImageTexture(Texture imageTexture);
    /// </remarks>
    /// 


    /// </summary>

    [Serializable]
    public abstract class DataConsumerImageTextureFromUrl : DataConsumerGOBase
    {
        [Tooltip("Manage images in child game objects as well as this one.")]
        [SerializeField] private bool manageChildren = true;

        [Tooltip("Key path of where to retrieve image URL within the data source data set.")]
        [SerializeField] private string keyPath = "imageUrl";

        [Tooltip("(Optional) Random load delay to load balance when multiple sprites are requested at same time, such as during the instantiation of an entire list.")]
        [SerializeField] private int maxRandomLoadBalancingDelayInMilliseconds = 0;


        internal bool _fetchInProgress = false;
        internal string _waitingUrlToFetch = null;
        internal int _frameDelayCountdown = 0;
        internal System.Random _random;
        internal Material _imageMaterial;

        internal readonly float FramesPerMillisecond = 60.0f / 1000.0f; // 60 frames per 1000 milliscends

        void Update()
        {
            CheckForWaitingUrlToFetch();

        }

        internal override bool ManageChildren()
        {
            return manageChildren;
        }

        internal override void InitializeDataConsumer()
        {
            _random = new System.Random();
        }

        internal override void AddVariableKeyPathsForComponent(Type componentType, Component component)
        {
            InitializeForComponent(componentType, component);

            AddKeyPath(this.keyPath);
        }


        internal abstract void InitializeForComponent(Type componentType, Component component);
        internal abstract void PlaceImageTexture(Texture2D imageTexture);


        internal override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object newValue)
        {
            // Debug.Log("ProcessDataChanged " + resolvedKeyPath + ",  local: " + localKeyPath);

            if (localKeyPath == this.keyPath)
            {
                string newUrl = newValue.ToString();
                _frameDelayCountdown = (int)((float)maxRandomLoadBalancingDelayInMilliseconds * FramesPerMillisecond);
                _frameDelayCountdown = _random.Next(0, _frameDelayCountdown);

                if (newUrl != null)
                {
                    if (_fetchInProgress || _frameDelayCountdown > 0)
                    {
                        _waitingUrlToFetch = newUrl;
                    }
                    else
                    {
                        _fetchInProgress = true;

                        StartCoroutine(FetchImageTexture(newUrl));
                    }
                }
            }
        }

        internal void CheckForWaitingUrlToFetch()
        {
            if (_waitingUrlToFetch != null && _fetchInProgress == false && --_frameDelayCountdown <= 0)
            {
                _fetchInProgress = true;
                StartCoroutine(FetchImageTexture(_waitingUrlToFetch));
                _waitingUrlToFetch = null;
            }
        }


        internal IEnumerator FetchImageTexture(string uri)
        {
            bool doneWithSuccess = false;
            const int kMaxRetries = 3;

            // UnityEngine.Debug.Log("SendWebRequest: " + uri);
            for (int retries = 0; !doneWithSuccess && retries < kMaxRetries; retries++)
            {
                using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
                {
                    yield return webRequest.SendWebRequest();

                    if (webRequest.isHttpError || webRequest.isNetworkError)
                    {
                        UnityEngine.Debug.LogError("SendWebRequest error: " + webRequest.error + " for URL " + uri );
                    }
                    else
                    {
                        bool saveAllowThreaded = Texture.allowThreadedTextureCreation;
                        Texture.allowThreadedTextureCreation = true;

                        Texture2D imageTexture = DownloadHandlerTexture.GetContent(webRequest);

                        PlaceImageTexture(imageTexture);

                        Texture.allowThreadedTextureCreation = saveAllowThreaded;
                        doneWithSuccess = true;
                    }
                    _fetchInProgress = false;
                }
            }
        }

    }
}
