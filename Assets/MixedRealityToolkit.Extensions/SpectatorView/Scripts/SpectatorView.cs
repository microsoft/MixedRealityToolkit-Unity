// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Class that facilitates the Spectator View experience
    /// </summary>
    public class SpectatorView : MonoBehaviour
    {
        /// <summary>
        /// MonoBehaviour that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IMatchMakingService"/>
        /// </summary>
        [Tooltip("MonoBehaviour that implements Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IMatchMakingService. Note: errors will be generated if the provided MonoBehaviour does not implement IMatchMakingService")]
        [SerializeField]
        protected MonoBehaviour MatchMakingService;

        /// <summary>
        /// MonoBehaviour that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IPlayerService"/>
        /// </summary>
        [Tooltip("MonoBehaviour that implements Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IPlayerService. Note: errors will be generated if the provided MonoBehaviour does not implement IPlayerService")]
        [SerializeField]
        protected MonoBehaviour PlayerService;

        /// <summary>
        /// MonoBehaviour that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.INetworkingService"/>
        /// </summary>
        [Tooltip("MonoBehaviour that implements Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.INetworkingService. Note: errors will be generated if the provided MonoBehaviour does not implement INetworkingService")]
        [SerializeField]
        protected MonoBehaviour NetworkingService;

        /// <summary>
        /// MonoBehaviour that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.ISpatialCoordinateService"/>
        /// </summary>
        [Tooltip("MonoBehaviour that implements Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.ISpatialCoordinateService. Note: errors will be generated if the provided MonoBehaviour does not implement ISpatialCoordinateService")]
        [SerializeField]
        protected MonoBehaviour SpatialCoordinateService;

        /// <summary>
        /// MonoBehaviour that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingService"/> for the Android platform
        /// </summary>
        [Tooltip("MonoBehaviour that implements Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingService for the Android platform. Note: errors will be generated if the provided MonoBehaviour does not implement IRecordingService")]
        [SerializeField]
        protected MonoBehaviour AndroidRecordingService;

        /// <summary>
        /// MonoBehaviour that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingService"/> for the iOS platform
        /// </summary>
        [Tooltip("MonoBehaviour that implements Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingService for the iOS platform. Note: errors will be generated if the provided MonoBehaviour does not implement IRecordingService")]
        [SerializeField]
        protected MonoBehaviour IosRecordingService;

        /// <summary>
        /// MonoBehaviour that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingServiceVisual"/>
        /// </summary>
        [Tooltip("MonoBehaviour that implements Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingServiceVisual. Note: errors will be generated if the provided MonoBehaviour does not implement IRecordingServiceVisual")]
        [SerializeField]
        protected MonoBehaviour RecordingServiceVisual;

        /// <summary>
        /// List of MonoBehaviours that implement <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IPlayerStateObserver"/>
        /// </summary>
        [Tooltip("List of MonoBehaviours that observe player state changes. Note: errors will be generated if any of the provided MonoBehaviours do not implement IPlayerStateObserver")]
        [SerializeField]
        protected List<MonoBehaviour> PlayerStateObservers;

        /// <summary>
        /// Transform from the local application origin to the shared application origin
        /// </summary>
        public Matrix4x4 LocalOriginToSharedOrigin { get; set; }

        /// <summary>
        /// If set, a transform from the local appliation origin to the shared appliation origin will be set to this SceneRoot GameObject
        /// </summary>
        public GameObject SceneRoot { get; set; }

        protected IMatchMakingService _matchMakingService;
        protected IPlayerService _playerService;
        protected INetworkingService _networkingService;
        protected ISpatialCoordinateService _spatialCoordinateService;
        protected List<IPlayerStateObserver> _playerStateObservers;
        protected IRecordingService _recordingService;
        protected IRecordingServiceVisual _recordingServiceVisual;

        protected bool _validState = true;

        protected void OnValidate()
        {
#if UNITY_EDITOR
            FieldHelper.ValidateType<IMatchMakingService>(MatchMakingService);
            FieldHelper.ValidateType<IPlayerService>(PlayerService);
            FieldHelper.ValidateType<INetworkingService>(NetworkingService);
            FieldHelper.ValidateType<ISpatialCoordinateService>(SpatialCoordinateService);
            FieldHelper.ValidateType<IRecordingService>(AndroidRecordingService);
            FieldHelper.ValidateType<IRecordingService>(IosRecordingService);
            FieldHelper.ValidateType<IRecordingServiceVisual>(RecordingServiceVisual);

            foreach (var observer in PlayerStateObservers)
            {
                FieldHelper.ValidateType<IPlayerStateObserver>(observer);
            }
#endif
        }

        protected void Awake()
        {
            _matchMakingService = MatchMakingService as IMatchMakingService;
            _playerService = PlayerService as IPlayerService;
            _networkingService = NetworkingService as INetworkingService;
            _spatialCoordinateService = SpatialCoordinateService as ISpatialCoordinateService;

            if (_matchMakingService == null ||
                _playerService == null ||
                _networkingService == null ||
                _spatialCoordinateService == null)
            {
                Debug.LogError("Invalid spectator view configuration, needed service classes are null or don't implement the correct interfaces");
                _validState = false;
            }

            _playerStateObservers = new List<IPlayerStateObserver>();
            foreach (var monoBehaviour in PlayerStateObservers)
            {
                var observer = monoBehaviour as IPlayerStateObserver;
                if (observer != null)
                    _playerStateObservers.Add(observer);
            }

            SetupRecordingService();
        }

        protected void Start()
        {
            // Allow spectator view logic to exist across multiple scenes
            DontDestroyOnLoad(gameObject);

            if (_validState)
            {
                _networkingService.DataReceived += OnDataReceivedEvent;
                _playerService.PlayerConnected += OnPlayerConnected;
                _playerService.PlayerDisconnected += OnPlayerDisconnected;

                _spatialCoordinateService.SpatialCoordinateStateUpdated += OnSpatialCoordinateStateUpdated;
            }
        }

        protected void OnDataReceivedEvent(string playerId, byte[] payload)
        {
            _spatialCoordinateService.Sync(playerId, payload);
        }

        protected void OnPlayerConnected(string playerId)
        {
            Debug.Log("Observed new player: " + playerId);
            foreach (var observer in _playerStateObservers)
            {
                observer.PlayerConnected(playerId);
            }
        }

        protected void OnPlayerDisconnected(string playerId)
        {
            Debug.Log("Player lost: " + playerId);
            foreach (var observer in _playerStateObservers)
            {
                observer.PlayerDisconnected(playerId);
            }
        }

        protected void OnSpatialCoordinateStateUpdated(byte[] payload)
        {
            if (_matchMakingService.IsConnected())
            {
                if(!_networkingService.SendData(payload))
                {
                    Debug.LogError("Networking service failed to send data");
                }
            }
        }

        protected void Update()
        {
            if (_validState)
            {
                if (!_matchMakingService.IsConnected())
                {
                    // Setup the connection
                    _matchMakingService.Connect();

                    if (!_matchMakingService.IsConnected())
                    {
                        // If no network connection exists, we won't attempt any additional shared anchor logic
                        return;
                    }
                }

                // Update the world origin
                Matrix4x4 localOriginToSharedOrigin;
                if (_spatialCoordinateService.TryGetLocalOriginToSharedOrigin(out localOriginToSharedOrigin))
                {
                    LocalOriginToSharedOrigin = localOriginToSharedOrigin;

                    if (SceneRoot != null)
                    {
                        SceneRoot.transform.position = LocalOriginToSharedOrigin.GetColumn(3);
                        SceneRoot.transform.rotation = Quaternion.LookRotation(LocalOriginToSharedOrigin.GetColumn(2), LocalOriginToSharedOrigin.GetColumn(1));
                    }
                    else
                    {
                        Debug.LogWarning("No Scene Root set, spectator view's shared origin transform wasn't applied to any content");
                    }
                }
            }
        }

        protected void SetupRecordingService()
        {
            _recordingServiceVisual = RecordingServiceVisual as IRecordingServiceVisual;

#if UNITY_IOS
            _recordingService = IosRecordingService as IRecordingService;
#elif UNITY_ANDROID
            _recordingService = AndroidRecordingService as IRecordingService;
#endif

            if (_recordingService != null &&
                _recordingServiceVisual != null)
            {
                _recordingServiceVisual.SetRecordingService(_recordingService);
            }
        }
    }
}
