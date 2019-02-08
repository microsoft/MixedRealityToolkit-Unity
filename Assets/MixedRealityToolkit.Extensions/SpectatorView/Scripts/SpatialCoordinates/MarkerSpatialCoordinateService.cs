// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces;
using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.SpatialCoordinates
{
    public class MarkerSpatialCoordinateService : MonoBehaviour,
        ISpatialCoordinateService,
        IPlayerStateObserver
    {
        #region Serializable  Classes
        [Serializable]
        class TransformData
        {
            public bool Valid;
            public Vector3 Position;
            public Quaternion Rotation;

            public TransformData()
            {
                Valid = false;
                Position = Vector3.zero;
                Rotation = Quaternion.identity;
            }

            public void SetTransform(Matrix4x4 transform)
            {
                Position = transform.GetColumn(3);
                Rotation = Quaternion.LookRotation(transform.GetColumn(2), transform.GetColumn(1));
            }

            public Matrix4x4 GetTransform()
            {
                return Matrix4x4.TRS(Position, Rotation, Vector3.one);
            }

            public override string ToString()
            {
                return "{" + Valid.ToString() + ", (" + Position.ToString() + "), (" + Rotation.ToString() + ")}";
            }
        };

        [Serializable]
        class Spectator
        {
            public string Id;
            public int MarkerId;
            public TransformData UserOriginToSpectatorMarker;
            public TransformData SpectatorMarkerToSpectatorCamera;
            public TransformData SpectatorCameraToSpectatorOriginWhenDetected;
            public TransformData SpectatorOriginToSpectatorCamera;

            public Spectator()
            {
                Id = "";
                MarkerId = -1;
                UserOriginToSpectatorMarker = new TransformData();
                SpectatorMarkerToSpectatorCamera = new TransformData();
                SpectatorCameraToSpectatorOriginWhenDetected = new TransformData();
                SpectatorOriginToSpectatorCamera = new TransformData();
            }

            public Spectator(string id)
            {
                Id = id;
                MarkerId = -1;
                UserOriginToSpectatorMarker = new TransformData();
                SpectatorMarkerToSpectatorCamera = new TransformData();
                SpectatorCameraToSpectatorOriginWhenDetected = new TransformData();
                SpectatorOriginToSpectatorCamera = new TransformData();
            }

            public static bool IsValidMarkerId(int marker)
            {
                return marker >= 0;
            }

            public bool HasValidMarkerId()
            {
                return IsValidMarkerId(MarkerId);
            }

            public static bool TryDeserializeSpectator(byte[] payload, out Spectator spectator)
            {
                spectator = new Spectator();
                try
                {
                    spectator = SerializationHelper.Deserialize<Spectator>(payload);
                    return true;
                }
                catch { }

                return false;
            }

            public override string ToString()
            {
                return "Spectator:" + Id +
                    ", Marker:" + MarkerId +
                    ", UserOriginToSpectatorMarker:" + UserOriginToSpectatorMarker.ToString() +
                    ", SpectatorMarkerToSpectatorCamera:" + SpectatorMarkerToSpectatorCamera.ToString() +
                    ", SpectatorCameraToSpectatorOriginWhenDetected:" + SpectatorCameraToSpectatorOriginWhenDetected.ToString() + 
                    ", SpectatorOriginToSpectatorcamera:" + SpectatorOriginToSpectatorCamera.ToString();
            }
        }

        [Serializable]
        class User
        {
            public int AvailableMarkerId;
            public Dictionary<string, Spectator> Spectators;
            public TransformData UserOriginToUserCamera;

            // TODO - figure out a better process for serialization than this additional list
            public List<Spectator> SpectatorList;

            private StringBuilder _stringBuilder;

            public User()
            {
                AvailableMarkerId = 0;
                Spectators = new Dictionary<string, Spectator>();
                UserOriginToUserCamera = new TransformData();
                SpectatorList = new List<Spectator>();
            }

            public void SyncList()
            {
                if (SpectatorList == null)
                    SpectatorList = new List<Spectator>();

                SpectatorList.Clear();
                foreach (var pair in Spectators)
                {
                    SpectatorList.Add(pair.Value);
                }
            }

            protected void SyncDictionaryFromList()
            {
                if (Spectators == null)
                    Spectators = new Dictionary<string, Spectator>();

                Spectators.Clear();
                foreach (var spectator in SpectatorList)
                {
                    Spectators.Add(spectator.Id, spectator);
                }
            }

            public static bool TryDeserializeUser(byte[] payload, out User user)
            {
                user = new User();
                try
                {
                    user = SerializationHelper.Deserialize<User>(payload);
                    user.SyncDictionaryFromList();
                    return true;
                }
                catch { }

                return false;
            }

            public override string ToString()
            {
                if (_stringBuilder == null)
                    _stringBuilder = new StringBuilder();
                else
                    _stringBuilder.Clear();

                _stringBuilder.Append("User: AvailableMarkerId:" + AvailableMarkerId + " " + UserOriginToUserCamera.ToString() + " ");
                foreach (var spectatorPair in Spectators)
                {
                    _stringBuilder.Append(spectatorPair.Value.ToString() + ", ");
                }

                return _stringBuilder.ToString();
            }
        }

        class SerializationHelper
        {
            public static byte[] Serialize(User user)
            {
                user.SyncList();
                var json = JsonUtility.ToJson(user);
                return ToBytes(json);
            }

            public static byte[] Serialize<T>(T obj)
            {
                var json = JsonUtility.ToJson(obj);
                return ToBytes(json);
            }

            public static T Deserialize<T>(byte[] data)
            {
                var json = Encoding.ASCII.GetString(data);
                return JsonUtility.FromJson<T>(json);
            }

            static byte[] ToBytes(string serialized)
            {
                var bytes = Encoding.ASCII.GetBytes(serialized);
                return bytes;
            }
        }
        #endregion

        [SerializeField] GameObject _aRFoundationGameObject;
        [SerializeField] Camera _aRFoundationCamera;
        [SerializeField] ARPointCloudManager _aRPointCloudManager;
        [SerializeField] bool _showDebugVisual;
        [SerializeField] DebugVisualHelper _debugVisualHelper;

        bool _actAsUser = false;
        bool _initialized = false;
        bool _localOriginEstablished = false;

        // User specific fields
        string _userPlayerId = "";
        Dictionary<string, string> _spectatorIds;
        User _cachedSelfUser;
        [SerializeField] MonoBehaviour MarkerDetector;
        IMarkerDetector _markerDetector;
        bool _detectingMarkers = false;
        Dictionary<string, GameObject> _originVisuals = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> _cameraVisuals = new Dictionary<string, GameObject>();

        // Spectator specific fields
        int _observedAvailableMarkerId = -1;
        Spectator _cachedSelfSpectator = new Spectator();
        [SerializeField] MonoBehaviour MarkerVisual;
        IMarkerVisual _markerVisual;
        bool _showingMarker = false;

        void OnValidate()
        {
            FieldHelper.ValidateType<IMarkerDetector>(MarkerDetector);
            FieldHelper.ValidateType<IMarkerVisual>(MarkerVisual);
        }

        void Awake()
        {
            // TODO - update here if future scenario requires device other than hololens to act as user
#if UNITY_WSA
            _actAsUser = true;
#elif UNITY_ANDROID || UNITY_IOS
            _actAsUser = false;
#endif
            _markerDetector = MarkerDetector as IMarkerDetector;
            _markerVisual = MarkerVisual as IMarkerVisual;

            Initialize();
        }

        void Update()
        {
            if (NeedsToPopulate())
            {
                Populate();
            }

            var state = GenerateStatePayload();
            if (state != null)
            {
                SpatialCoordinateStateUpdated?.Invoke(state);
            }

            if (_showDebugVisual)
            {
                ShowDebugVisuals();
            }

            if (_actAsUser)
            {
                _cachedSelfUser.UserOriginToUserCamera = GetLocalCameraTransform();
            }
            else
            {
                _cachedSelfSpectator.SpectatorOriginToSpectatorCamera = GetLocalCameraTransform();

                // TODO - assess whether assuming the camera is behind the marker is accurate enough
                _cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.Position = Vector3.zero;
                _cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.Rotation = Quaternion.Euler(0, 180, 0);
                _cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.Valid = true;
            }
        }

        public event SpatialCoordinateStateUpdateHandler SpatialCoordinateStateUpdated;

        public void Sync(string playerId, byte[] payload)
        {
            if (_actAsUser)
            {
                Spectator spectator;
                if (Spectator.TryDeserializeSpectator(payload, out spectator))
                {
                    Debug.Log("Received spectator data: " + spectator.ToString());
                    _spectatorIds[playerId] = spectator.Id;

                    if (!_cachedSelfUser.Spectators.ContainsKey(spectator.Id))
                    {
                        Debug.Log("Caching new spectator: " + spectator.ToString());
                        _cachedSelfUser.Spectators[spectator.Id] = new Spectator(spectator.Id);
                    }

                    if ((_cachedSelfUser.Spectators[spectator.Id].MarkerId != spectator.MarkerId) &&
                        (spectator.HasValidMarkerId()))
                    {
                        int potentialMarkerId = spectator.MarkerId;
                        int maxMarkerId = potentialMarkerId;
                        bool markerUsed = false;
                        foreach (var cachedSpectatorPair in _cachedSelfUser.Spectators)
                        {
                            if (cachedSpectatorPair.Value.MarkerId == potentialMarkerId)
                            {
                                markerUsed = true;
                                break;
                            }

                            if (cachedSpectatorPair.Value.MarkerId > maxMarkerId)
                            {
                                maxMarkerId = cachedSpectatorPair.Value.MarkerId;
                            }
                        }

                        if (!markerUsed)
                        {
                            _cachedSelfUser.Spectators[spectator.Id].MarkerId = potentialMarkerId;
                        }

                        _cachedSelfUser.AvailableMarkerId = maxMarkerId + 1;
                    }

                    _cachedSelfUser.Spectators[spectator.Id].SpectatorMarkerToSpectatorCamera = spectator.SpectatorMarkerToSpectatorCamera;
                    _cachedSelfUser.Spectators[spectator.Id].SpectatorCameraToSpectatorOriginWhenDetected = spectator.SpectatorCameraToSpectatorOriginWhenDetected;
                    _cachedSelfUser.Spectators[spectator.Id].SpectatorOriginToSpectatorCamera = spectator.SpectatorOriginToSpectatorCamera;
                }
            }
            else
            {
                User user;
                if (User.TryDeserializeUser(payload, out user))
                {
                    Debug.Log("Received user data: " + user.ToString());

                    if (!String.IsNullOrEmpty(_userPlayerId) &&
                        _userPlayerId != playerId)
                    {
                        Debug.Log("Observed a second server instance: " + playerId + ", ignoring for now");
                        return;
                    }

                    _userPlayerId = playerId;
                    Spectator self = null;
                    foreach (var spectator in user.Spectators)
                    {
                        if (spectator.Value.Id == _cachedSelfSpectator.Id)
                        {
                            self = spectator.Value;
                            break;
                        }
                    }

                    if (self != null)
                    {
                        _cachedSelfSpectator.MarkerId = self.MarkerId;

                        if (self.UserOriginToSpectatorMarker.Valid)
                        {
                            // Cache that the marker was located
                            _cachedSelfSpectator.UserOriginToSpectatorMarker = self.UserOriginToSpectatorMarker;

                            if (!_cachedSelfSpectator.SpectatorCameraToSpectatorOriginWhenDetected.Valid)
                            {
                                // Cache camera to origin transform when the marker was located
                                var spectatorOriginToSpectatorCamera = GetLocalCameraTransform();
                                var spectatorCameraTospectatorOrigin = spectatorOriginToSpectatorCamera;
                                spectatorCameraTospectatorOrigin.SetTransform(spectatorCameraTospectatorOrigin.GetTransform().inverse);
                                _cachedSelfSpectator.SpectatorCameraToSpectatorOriginWhenDetected = spectatorCameraTospectatorOrigin;
                            }
                        }
                    }

                    _observedAvailableMarkerId = user.AvailableMarkerId;
                }
            }
        }

        public bool TryGetLocalOriginToSharedOrigin(out Matrix4x4 localOriginToSharedOrigin)
        {
            localOriginToSharedOrigin = Matrix4x4.identity;

            if (_actAsUser)
            {
                return true;
            }
            else
            {
                if (!_cachedSelfSpectator.UserOriginToSpectatorMarker.Valid ||
                    !_cachedSelfSpectator.SpectatorCameraToSpectatorOriginWhenDetected.Valid ||
                    !_cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.Valid ||
                    !_cachedSelfSpectator.SpectatorOriginToSpectatorCamera.Valid)
                {
                    return false;
                }

                var userOriginToMarker = _cachedSelfSpectator.UserOriginToSpectatorMarker.GetTransform();
                var markerToCamera = _cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.GetTransform();
                var cameraToSpectatorOriginWhenDetected = _cachedSelfSpectator.SpectatorCameraToSpectatorOriginWhenDetected.GetTransform();
                var userOriginToSpectatorOrigin = cameraToSpectatorOriginWhenDetected * userOriginToMarker * markerToCamera;
                var spectatorOriginToUserOrigin = userOriginToSpectatorOrigin.inverse;

                localOriginToSharedOrigin = spectatorOriginToUserOrigin;
                return true;
            }
        }

        public void PlayerConnected(string playerId)
        {
        }

        public void PlayerDisconnected(string playerId)
        {
            if (playerId == _userPlayerId)
            {
                Debug.Log("User disconnected");
                // Tear down any cached self spectator state
                if (_showingMarker)
                {
                    _markerVisual.HideMarker();
                }

                _cachedSelfUser.AvailableMarkerId = -1;
                _cachedSelfSpectator.MarkerId = -1;
                _cachedSelfSpectator.UserOriginToSpectatorMarker = new TransformData();
                _userPlayerId = "";
            }

            if (_spectatorIds.ContainsKey(playerId))
            {
                Debug.Log("Spectator disconnected: " + _spectatorIds.ToString());

                // Remove the associated spectator and assess whether to stop detecting markers
                var spectatorId = _spectatorIds[playerId];
                _cachedSelfUser.Spectators.Remove(spectatorId);
                _spectatorIds.Remove(playerId);

                if (!NeedsToPopulate())
                {
                    StopDetectingMarkers();
                }
            }
        }

        void Initialize()
        {
            if (!_initialized)
            {
                if (_actAsUser)
                {
                    _spectatorIds = new Dictionary<string, string>();
                    _cachedSelfUser = new User();
                }
                else
                {
                    _cachedSelfSpectator = new Spectator(Guid.NewGuid().ToString());
                }

                _initialized = true;
            }
        }

        bool NeedsToPopulate()
        {
            if (_actAsUser)
            {
                // The user needs to populate if any spectators with valid markers have not been located
                bool needsToPopulate = false;
                foreach (var spectatorPair in _cachedSelfUser.Spectators)
                {
                    if (spectatorPair.Value.HasValidMarkerId() &&
                        !spectatorPair.Value.UserOriginToSpectatorMarker.Valid)
                    {
                        needsToPopulate = true;
                        break;
                    }
                }

                return needsToPopulate;
            }
            else
            {
                // An spectator needs to populate if it has a valid marker and hasn't been located
                return Spectator.IsValidMarkerId(_cachedSelfSpectator.MarkerId) && (!_cachedSelfSpectator.UserOriginToSpectatorMarker.Valid || _showingMarker);
            }
        }

        void Populate()
        {
            EstablishLocalOrigin();

            if (_actAsUser)
            {
                if (_localOriginEstablished)
                {
                    DetectMarkers();
                }
            }
            else
            {
                // The spectator may populate by showing or finding a marker
                if (!_localOriginEstablished)
                {
                    Debug.Log("Local origin not yet established");
                }
                else if (!_cachedSelfSpectator.UserOriginToSpectatorMarker.Valid)
                {
                    ShowMarker();
                }
                else
                {
                    HideMarker();
                }
            }
        }

        void EstablishLocalOrigin()
        {
#if UNITY_WSA
        _localOriginEstablished = true;
#elif UNITY_ANDROID || UNITY_IOS
            if (_aRFoundationGameObject == null)
            {
                Debug.LogError("AR Foundation game object not defined");
                return;
            }
            _aRFoundationGameObject.SetActive(true);

            if (_aRPointCloudManager == null)
            {
                Debug.LogError("Point cloud manager not defined for ar foundation device");
                return;
            }
            _aRPointCloudManager.pointCloudUpdated += OnPointCloudUpdated;
#endif
        }

#if UNITY_ANDROID || UNITY_IOS
        private void OnPointCloudUpdated(ARPointCloudUpdatedEventArgs args)
        {
            if (args != null &&
                args.pointCloud != null)
            {
                List<Vector3> points = new List<Vector3>();
                args.pointCloud.GetPoints(points);
                if (points.Count > 4)
                {
                    _localOriginEstablished = true;

                    // TODO - there may be a follow up tasks to shift inbetween showing the marker/hiding the marker based on whether or not these points are found
                    if (_aRPointCloudManager == null)
                    {
                        Debug.LogError("Point cloud manager not defined for ar foundation device");
                        return;
                    }
                    _aRPointCloudManager.pointCloudUpdated -= OnPointCloudUpdated;
                }
            }
        }
#endif

        byte[] GenerateStatePayload()
        {
            if (_actAsUser)
            {
                Debug.Log("Creating user payload: " + _cachedSelfUser.ToString());
                var output = SerializationHelper.Serialize(_cachedSelfUser);
                return output;
            }
            else
            {
                var marker = Spectator.IsValidMarkerId(_cachedSelfSpectator.MarkerId) ? _cachedSelfSpectator.MarkerId : _observedAvailableMarkerId;
                var self = _cachedSelfSpectator;
                self.MarkerId = marker;
                Debug.Log("Creating spectator payload: " + self.ToString());
                var output = SerializationHelper.Serialize(self);
                return output;
            }
        }

        void DetectMarkers()
        {
            if (_markerDetector == null)
            {
                Debug.LogError("Marker detector not set for Marker Anchor Sharing Manager");
                return;
            }

            if (!_detectingMarkers)
            {
                Debug.Log("Starting marker detection");

                _markerDetector.MarkersUpdated += OnMarkersUpdated;
                _markerDetector.StartDetecting();

                _detectingMarkers = true;
            }

        }

        void StopDetectingMarkers()
        {
            if (_markerDetector == null)
            {
                Debug.LogError("Marker detector not set for Marker Anchor Sharing Manager");
                return;
            }

            if (_detectingMarkers)
            {
                Debug.Log("Stopping marker detection");

                _markerDetector.StopDetecting();
                _markerDetector.MarkersUpdated -= OnMarkersUpdated;

                _detectingMarkers = false;
            }
        }

        void ShowMarker()
        {
            if (!Spectator.IsValidMarkerId(_cachedSelfSpectator.MarkerId))
            {
                Debug.Log("Marker id not yet assigned");
                return;
            }

            Debug.Log("Attempting to show marker: " + _cachedSelfSpectator.MarkerId);

            if (_markerVisual == null)
            {
                Debug.LogError("Marker visual not defined.");
                return;
            }

            _showingMarker = true;
            _markerVisual.ShowMarker(_cachedSelfSpectator.MarkerId);
        }

        void HideMarker()
        {
            Debug.Log("Hiding marker");

            if (_markerVisual == null)
            {
                Debug.LogError("Marker visual not defined.");
                return;
            }

            if (_markerVisual != null)
            {
                _markerVisual.HideMarker();
                _showingMarker = false;
            }
        }

        void OnMarkersUpdated(Dictionary<int, Marker> markerDictionary)
        {
            Debug.Log("Markers updated, observed " + markerDictionary.Count + " markers");

            foreach (var spectatorPair in _cachedSelfUser.Spectators)
            {
                if (spectatorPair.Value.HasValidMarkerId() &&
                    !spectatorPair.Value.UserOriginToSpectatorMarker.Valid)
                {
                    int markerId = spectatorPair.Value.MarkerId;
                    if (markerDictionary.ContainsKey(markerId))
                    {
                        spectatorPair.Value.UserOriginToSpectatorMarker.Position = markerDictionary[markerId].Position;
                        spectatorPair.Value.UserOriginToSpectatorMarker.Rotation = markerDictionary[markerId].Rotation;
                        spectatorPair.Value.UserOriginToSpectatorMarker.Valid = true;
                    }
                }
            }

            if (!NeedsToPopulate())
            {
                StopDetectingMarkers();
            }
        }

        TransformData GetLocalCameraTransform()
        {
            var localCameraTransform = new TransformData();
#if UNITY_ANDROID || UNITY_IOS
            if (_aRFoundationCamera != null)
            {
                localCameraTransform.Position = _aRFoundationCamera.transform.position;
                localCameraTransform.Rotation = _aRFoundationCamera.transform.rotation;
                localCameraTransform.Valid = true;
            }
            else
            {
                Debug.LogError("AR Camera not declared for scene");
            }
#else
            localCameraTransform.Position = Camera.main.transform.position;
            localCameraTransform.Rotation = Camera.main.transform.rotation;
            localCameraTransform.Valid = true;
#endif
            return localCameraTransform;
        }

        void ShowDebugVisuals()
        {
            Debug.Log("Attempting to show debug visual");
            if (_actAsUser &&
                _cachedSelfUser.Spectators != null)
            {
                foreach (var spectatorPair in _cachedSelfUser.Spectators)
                {
                    if (spectatorPair.Value.UserOriginToSpectatorMarker.Valid)
                    {
                        var position = spectatorPair.Value.UserOriginToSpectatorMarker.Position;
                        var rotation = spectatorPair.Value.UserOriginToSpectatorMarker.Rotation;

                        Debug.Log("Placing marker visual: " + spectatorPair.Value.Id + ", " + position.ToString() + ", " + rotation.ToString());

                        if (_originVisuals.ContainsKey(spectatorPair.Value.Id))
                        {
                            var visual = _originVisuals[spectatorPair.Value.Id];
                            _debugVisualHelper.UpdateVisual(ref visual, position, rotation);
                        }
                        else
                        {
                            _originVisuals[spectatorPair.Value.Id] = _debugVisualHelper.CreateVisual(position, rotation);
                        }

                        if (spectatorPair.Value.SpectatorMarkerToSpectatorCamera.Valid &&
                            spectatorPair.Value.SpectatorCameraToSpectatorOriginWhenDetected.Valid &&
                            spectatorPair.Value.SpectatorOriginToSpectatorCamera.Valid)
                        {
                            var spectatorCameraToSpectatorOriginWhenDetected = spectatorPair.Value.SpectatorCameraToSpectatorOriginWhenDetected.GetTransform();
                            var spectatorMarkerToSpectatorCamera = spectatorPair.Value.SpectatorMarkerToSpectatorCamera.GetTransform();
                            var userOriginToSpectatorMarker = spectatorPair.Value.UserOriginToSpectatorMarker.GetTransform();

                            var userOriginToSpectatorOrigin = spectatorCameraToSpectatorOriginWhenDetected * userOriginToSpectatorMarker * spectatorMarkerToSpectatorCamera;
                            var spectatorOriginToUserOrigin = userOriginToSpectatorOrigin.inverse;

                            var spectatorCameraToSpectatorOrigin = spectatorPair.Value.SpectatorOriginToSpectatorCamera.GetTransform().inverse;
                            var spectatorCameraToUserOrigin = spectatorOriginToUserOrigin * spectatorCameraToSpectatorOrigin;
                            var userOriginToSpectatorCamera = spectatorCameraToUserOrigin.inverse;

                            position = userOriginToSpectatorCamera.GetColumn(3);
                            rotation = Quaternion.LookRotation(userOriginToSpectatorCamera.GetColumn(2), userOriginToSpectatorCamera.GetColumn(1));

                            Debug.Log("Placing camera visual: " + spectatorPair.Value.Id + ", " + position.ToString() + ", " + rotation.ToString());

                            if (_cameraVisuals.ContainsKey(spectatorPair.Value.Id))
                            {
                                var visual = _cameraVisuals[spectatorPair.Value.Id];
                                _debugVisualHelper.UpdateVisual(ref visual, position, rotation);
                            }
                            else
                            {
                                _cameraVisuals[spectatorPair.Value.Id] = _debugVisualHelper.CreateVisual(position, rotation);
                            }
                        }
                    }
                }
            }
        }
    }
}
