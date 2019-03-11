// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.MarkerDetection
{
    public class UnityArUcoMarkerDetectorHoloLens : MonoBehaviour,
        IMarkerDetector
    {
        [SerializeField] float _timeBetweenCapture = 0.250f;
        [SerializeField] float _markerSize = 0.03f; // meters

        private UnityArUcoMarkerDetectorPluginAPI _api;
        private bool _detectingMarkers = false;
        private float _dt = 0;

        public event MarkersUpdatedHandler MarkersUpdated;

        public void StartDetecting()
        {
            this.gameObject.SetActive(true);

#if UNITY_WSA
            _api = new UnityArUcoMarkerDetectorPluginAPI();
            if (!_api.Initialize(_markerSize))
            {
                Debug.LogError("Failed to initialize api");
                return;
            }

            _detectingMarkers = true;
#endif
        }

        public void StopDetecting()
        {
            CleanUp();
            this.gameObject.SetActive(false);
        }

        public void SetMarkerSize(float size)
        {
            _markerSize = size;
        }

        void Update()
        {
            _dt += Time.deltaTime;
            if (_detectingMarkers &&
                _dt > _timeBetweenCapture)
            {
                _api.DetectMarkers();
                var markers = _api.GetKnownMarkers();
                MarkersUpdated?.Invoke(markers);
                _dt = 0;
            }
        }

        void OnDisable()
        {
            CleanUp();
        }

        void CleanUp()
        {
#if UNITY_WSA
            if (_api != null)
            {
                _api.Destroy();
            }
#endif
        }
    }
}

