// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces;
using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView
{
    public class SpectatorView : MonoBehaviour
    {
        [SerializeField] MonoBehaviour MatchMakingService;
        [SerializeField] MonoBehaviour PlayerService;
        [SerializeField] MonoBehaviour NetworkingService;
        [SerializeField] MonoBehaviour SpatialCoordinateService;
        [SerializeField] MonoBehaviour AndroidRecordingService;
        [SerializeField] MonoBehaviour IosRecordingService;
        [SerializeField] MonoBehaviour RecordingServiceVisual;
        [SerializeField] List<MonoBehaviour> PlayerStateObservers;

        [HideInInspector] public Matrix4x4 LocalOriginToSharedOrigin = Matrix4x4.identity;
        [HideInInspector] public GameObject SceneRoot;
        IMatchMakingService _matchMakingService;
        IPlayerService _playerService;
        INetworkingService _networkingService;
        ISpatialCoordinateService _spatialCoordinateService;
        List<IPlayerStateObserver> _playerStateObservers;
        IRecordingService _recordingService;
        IRecordingServiceVisual _recordingServiceVisual;

        bool _validState = true;

        void OnValidate()
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

        void Awake()
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

        void Start()
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

        private void OnDataReceivedEvent(string playerId, byte[] payload)
        {
            _spatialCoordinateService.Sync(playerId, payload);
        }

        private void OnPlayerConnected(string playerId)
        {
            Debug.Log("Observed new player: " + playerId);
            foreach (var observer in _playerStateObservers)
            {
                observer.PlayerConnected(playerId);
            }
        }

        private void OnPlayerDisconnected(string playerId)
        {
            Debug.Log("Player lost: " + playerId);
            foreach (var observer in _playerStateObservers)
            {
                observer.PlayerDisconnected(playerId);
            }
        }

        private void OnSpatialCoordinateStateUpdated(byte[] payload)
        {
            if (_matchMakingService.IsConnected())
            {
                if(!_networkingService.SendData(payload, NetworkPriority.Default))
                {
                    Debug.LogError("Networking service failed to send data");
                }
            }
        }

        void Update()
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

        private void SetupRecordingService()
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
