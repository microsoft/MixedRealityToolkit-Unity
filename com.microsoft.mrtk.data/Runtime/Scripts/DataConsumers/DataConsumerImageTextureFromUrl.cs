// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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
    ///     protected abstract void InitializeForComponent(Component component);
    ///     protected abstract void PlaceImageTexture(Texture imageTexture);
    /// </remarks>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Image Texture From Url")]
    public abstract class DataConsumerImageTextureFromUrl : DataConsumerGOBase
    {
        [Tooltip("Manage images in child game objects as well as this one.")]
        [SerializeField]
        private bool manageChildren = true;

        [Tooltip("Key path of where to retrieve image URL within the data source data set.")]
        [SerializeField]
        private string keyPath = "imageUrl";

        [Tooltip("(Optional) Random load delay to load balance when multiple sprites are requested at same time, such as during the instantiation of an entire list.")]
        [SerializeField]
        private int maxRandomLoadBalancingDelayInMilliseconds = 0;

        protected bool _fetchInProgress = false;
        protected string _waitingUrlToFetch = null;
        protected int _frameDelayCountdown = 0;
        protected System.Random _random = new System.Random();
        private IEnumerator _coroutine;

        protected const float FramesPerMillisecond = 60.0f / 1000.0f; // 60 frames per 1000 milliseconds

        private void Update()
        {
            CheckForWaitingUrlToFetch();
        }

        /// </inheritdoc/>
        protected override bool ManageChildren()
        {
            return manageChildren;
        }

        /// </inheritdoc/>
        protected override void DetachDataConsumer()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _fetchInProgress = false;
            _waitingUrlToFetch = null;
        }

        /// </inheritdoc/>
        protected override void AddVariableKeyPathsForComponent(Component component)
        {
            InitializeForComponent(component);

            AddKeyPathListener(keyPath);
        }

        /// <summary>
        /// Initialize any state specific to each managed component found.
        /// </summary>
        /// <param name="component">The component instance of that component type.</param>
        protected abstract void InitializeForComponent(Component component);

        /// <summary>
        /// Place the fetched image texture.
        /// </summary>
        /// <param name="imageTexture">The retrieved image as a Texture2D object.</param>
        protected abstract void PlaceImageTexture(Texture2D imageTexture);

        /// </inheritdoc/>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType)
        {
            if (localKeyPath == keyPath)
            {
                if (value == null)
                {
                    Debug.LogError("Value should not be null for resolvedKeyPath " + resolvedKeyPath);
                }
                string newUrl = value.ToString();
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
                        _coroutine = FetchImageTexture(newUrl);
                        StartCoroutine(_coroutine);
                    }
                }
            }
        }

        protected void CheckForWaitingUrlToFetch()
        {
            if (_waitingUrlToFetch != null && _fetchInProgress == false && --_frameDelayCountdown <= 0)
            {
                _fetchInProgress = true;
                _coroutine = FetchImageTexture(_waitingUrlToFetch);
                StartCoroutine(_coroutine);
                _waitingUrlToFetch = null;
            }
        }

        protected IEnumerator FetchImageTexture(string uri)
        {
            bool doneWithSuccess = false;
            const int kMaxRetries = 3;

            for (int retries = 0; !doneWithSuccess && retries < kMaxRetries; retries++)
            {
                using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
                {
                    yield return webRequest.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
                    if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
#else
                    if (webRequest.isHttpError || webRequest.isNetworkError)
#endif
                    {
                        Debug.LogError("SendWebRequest error: " + webRequest.error + " for URL " + uri);
                    }
                    else
                    {
#if UNITY_2019_1_OR_NEWER
                        bool saveAllowThreaded = Texture.allowThreadedTextureCreation;
                        Texture.allowThreadedTextureCreation = true;
#endif
                        Texture2D imageTexture = DownloadHandlerTexture.GetContent(webRequest);

                        PlaceImageTexture(imageTexture);

#if UNITY_2019_1_OR_NEWER
                        Texture.allowThreadedTextureCreation = saveAllowThreaded;
#endif
                        doneWithSuccess = true;
                    }
                    _fetchInProgress = false;
                }
            }
            _coroutine = null;
        }
    }
}
