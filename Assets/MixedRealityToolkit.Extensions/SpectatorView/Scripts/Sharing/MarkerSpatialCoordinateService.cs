// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using UnityEngine.XR.ARFoundation;

using Microsoft.MixedReality.Toolkit.Extensions.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Sharing
{
    public enum MarkerSpatialCoordinateServiceOverlayState
    {
        waitingForUser,
        locatingLocalOrigin,
        locatingMarker,
        none
    }

    public interface IMarkerSpatialCoordinateServiceOverlayVisual
    {
        void UpdateVisualState(MarkerSpatialCoordinateServiceOverlayState state);
        void ShowVisual();
        void HideVisual();
    }

    public delegate void ResetSpatialCoordinatesHandler();
    public interface IMarkerSpatialCoordinateServiceResetVisual
    {
        event ResetSpatialCoordinatesHandler ResetSpatialCoordinates;
    }

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
                Position = GetPosition(transform);
                Rotation = GetRotation(transform);
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
            public bool RequestingReset;

            public Spectator()
            {
                Id = "";
                MarkerId = -1;
                UserOriginToSpectatorMarker = new TransformData();
                SpectatorMarkerToSpectatorCamera = new TransformData();
                SpectatorCameraToSpectatorOriginWhenDetected = new TransformData();
                SpectatorOriginToSpectatorCamera = new TransformData();
                RequestingReset = false;
            }

            public Spectator(string id)
            {
                Id = id;
                MarkerId = -1;
                UserOriginToSpectatorMarker = new TransformData();
                SpectatorMarkerToSpectatorCamera = new TransformData();
                SpectatorCameraToSpectatorOriginWhenDetected = new TransformData();
                SpectatorOriginToSpectatorCamera = new TransformData();
                RequestingReset = false;
            }

            public static bool IsValidMarkerId(int marker)
            {
                return marker >= 0;
            }

            public bool HasValidMarkerId()
            {
                return IsValidMarkerId(MarkerId);
            }

            public void ResetTransformData()
            {
                UserOriginToSpectatorMarker = new TransformData();
                SpectatorMarkerToSpectatorCamera = new TransformData();
                SpectatorCameraToSpectatorOriginWhenDetected = new TransformData();
                SpectatorOriginToSpectatorCamera = new TransformData();
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
                    ", SpectatorOriginToSpectatorcamera:" + SpectatorOriginToSpectatorCamera.ToString() +
                    ", RequestingReset: " + RequestingReset.ToString();
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

        enum VisualState
        {
            locatingLocalOrigin,
            locatingMarker,
            showingMarker,
            waitingForUser,
            none
        }

        [SerializeField] Camera _aRFoundationCamera;
        [SerializeField] ARPointCloudManager _aRPointCloudManager;
        [SerializeField] bool _showDebugVisual;
        [SerializeField] DebugVisualHelper _debugVisualHelper;
        [SerializeField] float _markerSize = 0.04f;

        bool _actAsUser = false;
        bool _initialized = false;
        bool _localOriginEstablished = false;
        bool _listeningToPointCloudChanges = false;
        VisualState _visualState = VisualState.none;
        GameObject _sharedOriginVisual;

        // User specific fields
        string _userPlayerId = "";
        Dictionary<string, string> _spectatorIds;
        User _cachedSelfUser;
        [SerializeField] MonoBehaviour MarkerDetector;
        IMarkerDetector _markerDetector;
        bool _detectingMarkers = false;
        Dictionary<string, GameObject> _markerVisuals = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> _cameraVisuals = new Dictionary<string, GameObject>();

        // Spectator specific fields
        Spectator _cachedSelfSpectator = new Spectator();
        [SerializeField] int _pointsRequiredForValidLocalOrigin = 4;
        [SerializeField] MonoBehaviour MarkerVisual;
        IMarkerVisual _markerVisual;
        User _cachedUser;
        GameObject _userCameraVisual;

        [SerializeField] bool _useMarkerSpatialCoordinateVisual;
        [SerializeField] MonoBehaviour HoloLensOverlayVisual;
        [SerializeField] MonoBehaviour MobileOverlayVisual;
        [SerializeField] MonoBehaviour MobileResetVisual;
        IMarkerSpatialCoordinateServiceOverlayVisual _markerSpatialCoordinateOverlayVisual;
        IMarkerSpatialCoordinateServiceResetVisual _markerSpatialCoordinateResetVisual;
        bool _needUIUpdate = true;

        void OnValidate()
        {
#if UNITY_EDITOR
            FieldHelper.ValidateType<IMarkerDetector>(MarkerDetector);
            FieldHelper.ValidateType<IMarkerVisual>(MarkerVisual);
            FieldHelper.ValidateType<IMarkerSpatialCoordinateServiceOverlayVisual>(HoloLensOverlayVisual);
            FieldHelper.ValidateType<IMarkerSpatialCoordinateServiceOverlayVisual>(MobileOverlayVisual);
            FieldHelper.ValidateType<IMarkerSpatialCoordinateServiceResetVisual>(MobileResetVisual);
#endif
        }

        void Awake()
        {
            // TODO - update here if future scenario requires device other than hololens to act as user
#if UNITY_WSA
            _actAsUser = true;
            _markerSpatialCoordinateOverlayVisual = HoloLensOverlayVisual as IMarkerSpatialCoordinateServiceOverlayVisual;
            _markerSpatialCoordinateResetVisual = null;
#elif UNITY_ANDROID || UNITY_IOS
            _actAsUser = false;
            _markerSpatialCoordinateOverlayVisual = MobileOverlayVisual as IMarkerSpatialCoordinateServiceOverlayVisual;
            _markerSpatialCoordinateResetVisual = MobileResetVisual as IMarkerSpatialCoordinateServiceResetVisual;
#endif
            if (_markerSpatialCoordinateResetVisual != null)
            {
                _markerSpatialCoordinateResetVisual.ResetSpatialCoordinates += OnResetSpatialCoordinates;
            }

            _markerDetector = MarkerDetector as IMarkerDetector;
            if (_markerDetector != null)
                _markerDetector.SetMarkerSize(_markerSize);

            _markerVisual = MarkerVisual as IMarkerVisual;
            if (_markerVisual != null)
                _markerVisual.SetMarkerSize(_markerSize);

            SetVisualState(VisualState.none);

            Initialize();
        }

        void Update()
        {
            if (NeedsToPopulate())
            {
                Populate();
            }
            else if (WaitingOnUser())
            {
                SetVisualState(VisualState.waitingForUser);
            }

            var state = GenerateStatePayload();
            if (state != null)
            {
                SpatialCoordinateStateUpdated?.Invoke(state);
            }

            if (_needUIUpdate)
            {
                Debug.Log("Updating UI visual state: " + _visualState.ToString());
                UpdateVisualStateUI();
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
                    _spectatorIds[playerId] = spectator.Id;

                    if (!_cachedSelfUser.Spectators.ContainsKey(spectator.Id))
                    {
                        Debug.Log("Caching new spectator: " + spectator.ToString());
                        _cachedSelfUser.Spectators[spectator.Id] = new Spectator(spectator.Id);
                    }

                    if (!_cachedSelfUser.Spectators[spectator.Id].HasValidMarkerId() &&
                        !spectator.HasValidMarkerId())
                    {
                        _cachedSelfUser.Spectators[spectator.Id].MarkerId = _cachedSelfUser.AvailableMarkerId;
                        _cachedSelfUser.AvailableMarkerId += 1;
                        Debug.Log("Assigned marker to spectator: " + _cachedSelfUser.Spectators[spectator.Id].ToString());
                    }

                    if (spectator.RequestingReset)
                    {
                        // If the spectator is requesting a reset, we clear its cached transform data
                        _cachedSelfUser.Spectators[spectator.Id].ResetTransformData();
                        Debug.Log("Resetting spectator transforms: " + _cachedSelfUser.Spectators[spectator.Id].ToString());
                    }
                    else
                    {
                        _cachedSelfUser.Spectators[spectator.Id].SpectatorMarkerToSpectatorCamera = spectator.SpectatorMarkerToSpectatorCamera;
                        _cachedSelfUser.Spectators[spectator.Id].SpectatorCameraToSpectatorOriginWhenDetected = spectator.SpectatorCameraToSpectatorOriginWhenDetected;
                        _cachedSelfUser.Spectators[spectator.Id].SpectatorOriginToSpectatorCamera = spectator.SpectatorOriginToSpectatorCamera;
                    }
                }
            }
            else
            {
                User user;
                if (User.TryDeserializeUser(payload, out user))
                {
                    _cachedUser = user;

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

                        if (_cachedSelfSpectator.RequestingReset &&
                            !self.UserOriginToSpectatorMarker.Valid)
                        {
                            // The main user registered that we requested a reset
                            _cachedSelfSpectator.ResetTransformData();
                            _cachedSelfSpectator.RequestingReset = false;
                        }
                        else if (!_cachedSelfSpectator.RequestingReset &&
                            self.UserOriginToSpectatorMarker.Valid)
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
                Matrix4x4 spectatorOriginToUserOrigin;
                if (TryCalculateSpectatorOriginToUserOriginTransform(_cachedSelfSpectator, out spectatorOriginToUserOrigin))
                {
                    localOriginToSharedOrigin = spectatorOriginToUserOrigin;
                    return true;
                }
            }

            return false;
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
                if (_visualState == VisualState.showingMarker)
                {
                    SetVisualState(VisualState.none);
                }

                var id = _cachedSelfSpectator.Id;
                _cachedSelfSpectator = new Spectator();
                _cachedSelfSpectator.Id = id;

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
                    _cachedUser = new User();
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
                // A spectator needs to populate if it has a valid marker and hasn't been located
                return _cachedSelfSpectator.HasValidMarkerId() &&
                    (!_cachedSelfSpectator.UserOriginToSpectatorMarker.Valid || (_visualState == VisualState.showingMarker));
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
                if (!_localOriginEstablished)
                {
                    Debug.Log("Local origin not yet established");
                    SetVisualState(VisualState.locatingLocalOrigin);
                }
                else if (!_cachedSelfSpectator.HasValidMarkerId())
                {
                    Debug.Log("Marker not yet assigned");
                    SetVisualState(VisualState.waitingForUser);
                }
                else if (!_cachedSelfSpectator.UserOriginToSpectatorMarker.Valid ||
                    _cachedSelfSpectator.RequestingReset)
                {
                    SetVisualState(VisualState.showingMarker);
                }
                else
                {
                    SetVisualState(VisualState.none);
                }
            }
        }

        bool WaitingOnUser()
        {
            // A spectator is waiting on the user if it does not have a valid marker id
            return !_actAsUser && !_cachedSelfSpectator.HasValidMarkerId();
        }

        void EstablishLocalOrigin()
        {
#if UNITY_WSA
            _localOriginEstablished = true;
#elif UNITY_ANDROID || UNITY_IOS
            if (!_localOriginEstablished &&
                !_listeningToPointCloudChanges)
            {
                if (_aRPointCloudManager == null)
                {
                    Debug.LogError("Point cloud manager not defined for ar foundation device");
                    return;
                }

                Debug.Log("Started observing point cloud changes");
                _aRPointCloudManager.pointCloudUpdated += OnPointCloudUpdated;
                _listeningToPointCloudChanges = true;
            }
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
                if (points.Count > _pointsRequiredForValidLocalOrigin)
                {
                    _localOriginEstablished = true;

                    // TODO - there may be a follow up task to shift inbetween showing the marker/hiding the marker based on whether or not these points are found
                    if (_aRPointCloudManager == null)
                    {
                        Debug.LogError("Point cloud manager not defined for ar foundation device");
                        return;
                    }

                    if (_listeningToPointCloudChanges)
                    {
                        Debug.Log("Stopped observing point cloud changes");
                        _aRPointCloudManager.pointCloudUpdated -= OnPointCloudUpdated;
                        _listeningToPointCloudChanges = false;
                    }
                }
                else
                {
                    Debug.Log("Point cloud did not contain enough points to establish a local origin, current size: " + points.Count);
                }
            }
        }
#endif

        byte[] GenerateStatePayload()
        {
            if (_actAsUser)
            {
                var output = SerializationHelper.Serialize(_cachedSelfUser);
                return output;
            }
            else
            {
                var output = SerializationHelper.Serialize(_cachedSelfSpectator);
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
                SetVisualState(VisualState.locatingMarker);
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
                SetVisualState(VisualState.none);
            }
        }

        void ShowMarker()
        {
            if (!_cachedSelfSpectator.HasValidMarkerId())
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

        static bool TryCalculateUserOriginToSpectatorOriginTransform(Spectator spectator, out Matrix4x4 userOriginToSpectatorOrigin)
        {
            userOriginToSpectatorOrigin = Matrix4x4.identity;

            if (spectator.SpectatorCameraToSpectatorOriginWhenDetected.Valid &&
                spectator.SpectatorMarkerToSpectatorCamera.Valid &&
                spectator.UserOriginToSpectatorMarker.Valid)
            {
                var spectatorCameraToSpectatorOriginWhenDetected = spectator.SpectatorCameraToSpectatorOriginWhenDetected.GetTransform();
                var spectatorMarkerToSpectatorCamera = spectator.SpectatorMarkerToSpectatorCamera.GetTransform();
                var userOriginToSpectatorMarker = spectator.UserOriginToSpectatorMarker.GetTransform();
                userOriginToSpectatorOrigin = userOriginToSpectatorMarker * spectatorMarkerToSpectatorCamera * spectatorCameraToSpectatorOriginWhenDetected;
                return true;
            }

            return false;
        }

        static bool TryCalculateSpectatorOriginToUserOriginTransform(Spectator spectator, out Matrix4x4 spectatorOriginToUserOrigin)
        {
            spectatorOriginToUserOrigin = Matrix4x4.identity;

            Matrix4x4 userOriginToSpectatorOrigin;
            if (TryCalculateUserOriginToSpectatorOriginTransform(spectator, out userOriginToSpectatorOrigin))
            {
                spectatorOriginToUserOrigin = userOriginToSpectatorOrigin.inverse;
                return true;
            }

            return false;
        }

        static Vector4 GetPosition(Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        static Quaternion GetRotation(Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        void SetVisualState(VisualState visualState)
        {
            if (_visualState != visualState)
            {
                _visualState = visualState;

                // This setter may get called from different threads, but we want to always update UI on the main thread
                // So we flag the ui as needing an update for the next update pass
                _needUIUpdate = true;
            }
        }

        void OnToggleDebugVisuals()
        {
            _showDebugVisual = !_showDebugVisual;
        }

        void OnResetSpatialCoordinates()
        {
            if (_actAsUser)
            {
                Debug.LogWarning("Resetting spatial coordinates not currently supported for main user");
                return;
            }

            Debug.Log("Resetting spatial coordinates");
            _cachedSelfSpectator.RequestingReset = true;
            _cachedSelfSpectator.ResetTransformData();
        }

        void UpdateVisualStateUI()
        {
            var showMarker = false;
            var showVisual = false;
            var state = MarkerSpatialCoordinateServiceOverlayState.none;

            switch (_visualState)
            {
                case VisualState.locatingLocalOrigin:
                    showMarker = false;
                    showVisual = true;
                    state = MarkerSpatialCoordinateServiceOverlayState.locatingLocalOrigin;
                    break;

                case VisualState.locatingMarker:
                    showMarker = false;
                    showVisual = true;
                    state = MarkerSpatialCoordinateServiceOverlayState.locatingMarker;
                    break;

                case VisualState.showingMarker:
                    showMarker = true;
                    showVisual = false;
                    state = MarkerSpatialCoordinateServiceOverlayState.none;
                    break;

                case VisualState.waitingForUser:
                    showMarker = false;
                    showVisual = true;
                    state = MarkerSpatialCoordinateServiceOverlayState.waitingForUser;
                    break;

                case VisualState.none:
                default:
                    showMarker = false;
                    showVisual = false;
                    state = MarkerSpatialCoordinateServiceOverlayState.none;
                    break;
            }

            if (_useMarkerSpatialCoordinateVisual &&
                _markerSpatialCoordinateOverlayVisual != null)
            {
                _markerSpatialCoordinateOverlayVisual.UpdateVisualState(state);

                if (showVisual)
                {
                    _markerSpatialCoordinateOverlayVisual.ShowVisual();
                }
                else
                {
                    _markerSpatialCoordinateOverlayVisual.HideVisual();
                }
            }

            if (showMarker)
            {
                ShowMarker();
            }
            else
            {
                HideMarker();
            }

            _needUIUpdate = false;
        }

        void ShowDebugVisuals()
        {
            // Note: This show debug visual functionality assumes that no parent transforms are applied to this game object

            if (_actAsUser &&
                _cachedSelfUser.Spectators != null)
            {
                // Place a debug visual at the scene origin to better understand the shared origin across devices
                _debugVisualHelper.CreateOrUpdateVisual(ref _sharedOriginVisual, Vector3.zero, Quaternion.identity);

                foreach (var spectatorPair in _cachedSelfUser.Spectators)
                {
                    // Place a debug visual where the spectator's marker was detected
                    if (spectatorPair.Value.UserOriginToSpectatorMarker.Valid)
                    {
                        var position = spectatorPair.Value.UserOriginToSpectatorMarker.Position;
                        var rotation = spectatorPair.Value.UserOriginToSpectatorMarker.Rotation;

                        GameObject visual = null;
                        if (_markerVisuals.ContainsKey(spectatorPair.Value.Id))
                        {
                            visual = _markerVisuals[spectatorPair.Value.Id];
                        }

                        _debugVisualHelper.CreateOrUpdateVisual(ref visual, position, rotation);
                        _markerVisuals[spectatorPair.Value.Id] = visual;

                        // If there was a known marker location for the spectator, we also attempt to place a debug visual at the spectator camera location
                        Matrix4x4 spectatorOriginToUserOrigin;
                        if (spectatorPair.Value.SpectatorOriginToSpectatorCamera.Valid &&
                            TryCalculateSpectatorOriginToUserOriginTransform(spectatorPair.Value, out spectatorOriginToUserOrigin))
                        {
                            var spectatorCameraToSpectatorOrigin = spectatorPair.Value.SpectatorOriginToSpectatorCamera.GetTransform().inverse;
                            var spectatorCameraToUserOrigin = spectatorCameraToSpectatorOrigin * spectatorOriginToUserOrigin;
                            var userOriginToSpectatorCamera = spectatorCameraToUserOrigin.inverse;

                            position = GetPosition(userOriginToSpectatorCamera);
                            rotation = GetRotation(userOriginToSpectatorCamera);

                            visual = null;
                            if (_cameraVisuals.ContainsKey(spectatorPair.Value.Id))
                            {
                               visual = _cameraVisuals[spectatorPair.Value.Id];
                            }

                            _debugVisualHelper.CreateOrUpdateVisual(ref visual, position, rotation);
                            _cameraVisuals[spectatorPair.Value.Id] = visual;
                        }
                    }
                }

                // Cleanup camera visuals for any lost spectators
                AssessAndCleanUpDebugVisuals(_cachedSelfUser.Spectators, _cameraVisuals);

                // Cleanup marker visuals for any lost spectators
                AssessAndCleanUpDebugVisuals(_cachedSelfUser.Spectators, _markerVisuals);
            }
            else
            {
                Matrix4x4 spectatorOriginToUserOrigin;
                if (_sharedOriginVisual == null &&
                    TryCalculateSpectatorOriginToUserOriginTransform(_cachedSelfSpectator, out spectatorOriginToUserOrigin))
                {
                    // Place a marker to show where the spectator has registered the shared origin
                    var position = GetPosition(spectatorOriginToUserOrigin);
                    var rotation = GetRotation(spectatorOriginToUserOrigin);

                    _debugVisualHelper.CreateOrUpdateVisual(ref _sharedOriginVisual, position, rotation);
                }
                else if (_sharedOriginVisual != null)
                {
                    Destroy(_sharedOriginVisual);
                    _sharedOriginVisual = null;
                }

                if (_cachedUser != null &&
                    _cachedUser.UserOriginToUserCamera.Valid &&
                    TryCalculateSpectatorOriginToUserOriginTransform(_cachedSelfSpectator, out spectatorOriginToUserOrigin))
                {
                    // Place a marker showing where the user camera is in the shared space
                    var userOriginToUserCamera = _cachedUser.UserOriginToUserCamera.GetTransform();
                    var spectatorOriginToUserCamera = spectatorOriginToUserOrigin * userOriginToUserCamera;

                    var position = GetPosition(spectatorOriginToUserCamera);
                    var rotation = GetRotation(spectatorOriginToUserCamera);

                    _debugVisualHelper.CreateOrUpdateVisual(ref _userCameraVisual, position, rotation);
                }
                else if (_userCameraVisual)
                {
                    Destroy(_userCameraVisual);
                    _userCameraVisual = null;
                }
            }
        }

        static void AssessAndCleanUpDebugVisuals(Dictionary<string, Spectator> knownSpectators, Dictionary<string, GameObject> debugVisuals)
        {
            if (knownSpectators.Count != debugVisuals.Count)
            {
                List<KeyValuePair<string, GameObject>> visualsToRemove = new List<KeyValuePair<string, GameObject>>();
                foreach (var visualPair in debugVisuals)
                {
                    if (!knownSpectators.ContainsKey(visualPair.Key))
                    {
                        visualsToRemove.Add(visualPair);
                    }
                }

                foreach (var visualPair in visualsToRemove)
                {
                    debugVisuals.Remove(visualPair.Key);
                    Destroy(visualPair.Value);
                }
            }
        }
    }
}
