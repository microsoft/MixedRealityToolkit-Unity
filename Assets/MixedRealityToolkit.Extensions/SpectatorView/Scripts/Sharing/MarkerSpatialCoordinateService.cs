// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if !UNITY_WSA && SPATIALALIGNMENT_LEGACY
using UnityEngine.XR.ARFoundation;
#endif

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Sharing
{
    /// <summary>
    /// Enum for various visual states associated with setting up a <see cref="MarkerSpatialCoordinateService"/> shared coordinate system
    /// </summary>
    public enum MarkerSpatialCoordinateServiceOverlayState
    {
        waitingForUser,
        locatingLocalOrigin,
        locatingMarker,
        none
    }

    /// <summary>
    /// Interface for displaying various visual states associated with setting up a <see cref="MarkerSpatialCoordinateService"/> shared coordinate system
    /// </summary>
    public interface IMarkerSpatialCoordinateServiceOverlayVisual
    {
        /// <summary>
        /// Updates the visual state to the requested state
        /// </summary>
        /// <param name="state"></param>
        void UpdateVisualState(MarkerSpatialCoordinateServiceOverlayState state);

        /// <summary>
        /// Shows the visual
        /// </summary>
        void ShowVisual();

        /// <summary>
        /// Hides the visual
        /// </summary>
        void HideVisual();
    }

    /// <summary>
    /// Delegate called to reset the a <see cref="MarkerSpatialCoordinateService"/> shared coordinate system
    /// </summary>
    public delegate void ResetSpatialCoordinatesHandler();

    /// <summary>
    /// Implemented by visuals that want to reset a <see cref="MarkerSpatialCoordinateService"/> shared coordinate system
    /// </summary>
    public interface IMarkerSpatialCoordinateServiceResetVisual
    {
        /// <summary>
        /// Event called when a user specifies to reset the shared spatial coordinate system
        /// </summary>
        event ResetSpatialCoordinatesHandler ResetSpatialCoordinates;
    }

    /// <summary>
    /// Class that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.ISpatialCoordinateServiceOld"/>
    /// and <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IPlayerStateObserver"/> to provide a marker based shared spatial coordinate system
    /// </summary>
    public class MarkerSpatialCoordinateService : MonoBehaviour,
        ISpatialCoordinateServiceOld,
        IPlayerStateObserver
    {
#region Serializable  Classes
        [Serializable]
        private class TransformData
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
        private class Spectator
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
        private class User
        {
            public int AvailableMarkerId;
            public Dictionary<string, Spectator> Spectators;
            public TransformData UserOriginToUserCamera;

            // Future state serialization refactors should avoid maintaining this second data structure for serialization
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

        private class SerializationHelper
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
                var json = Encoding.UTF8.GetString(data);
                return JsonUtility.FromJson<T>(json);
            }

            static byte[] ToBytes(string serialized)
            {
                var bytes = Encoding.UTF8.GetBytes(serialized);
                return bytes;
            }
        }
#endregion

        /// <summary>
        /// Various visual states possible when setting up the <see cref="MarkerSpatialCoordinateService"/> based spatial coordinate system
        /// </summary>
        protected enum VisualState
        {
            locatingLocalOrigin,
            locatingMarker,
            showingMarker,
            waitingForUser,
            none
        }

        /// <summary>
        /// AR Camera used for Mobile Device's AR Foundation experience.
        /// </summary>
        [Tooltip("AR Camera used for Mobile Device's AR Foundation experience.")]
        [SerializeField]
        protected Camera _aRFoundationCamera;

#if !UNITY_WSA && SPATIALALIGNMENT_LEGACY
        /// <summary>
        /// AR Point Cloud Manager defined for mobile device's AR Foundation experience. Said manager will be used to determine whether or not the mobile device has found its local application origin.
        /// </summary>
        [Tooltip("AR Point Cloud Manager defined for mobile device's AR Foundation experience. Said manager will be used to determine whether or not the mobile device has found its local application origin.")]
        [SerializeField]
        protected ARPointCloudManager _aRPointCloudManager;
#endif

        /// <summary>
        /// If true, debug visuals will be added to the unity scene to reflect positions of <see cref="User"/>s and <see cref="Spectator"/>s in the shared application space.
        /// A debug visual will also be shown at the shared application origin.
        /// </summary>
        [Tooltip("If true, debug visuals will be added to the unity scene to reflect positions of Users and Spectators in the shared application space. A debug visual will also be shown at the shared application origin.")]
        [SerializeField]
        protected bool _showDebugVisual;

        /// <summary>
        /// Helper class for showing debug visuals.
        /// </summary>
        [Tooltip("Helper class for showing debug visuals.")]
        [SerializeField]
        protected DebugVisualHelper _debugVisualHelper;

        /// <summary>
        /// Physical marker size in meters to use for showing and detecting markers (default value is 0.04).
        /// </summary>
        [Tooltip("Physical marker size in meters to use for showing and detecting markers (default value is 0.04).")]
        [SerializeField]
        protected float _markerSize = 0.04f;

        /// <summary>
        /// If true, the MarkerSpatialCoordinateService will attempt to show a visual reflecting the shared coordinate system setup state in the application.
        /// If false, no visual is shown.
        /// </summary>
        [Tooltip("If true, the MarkerSpatialCoordinateService will attempt to show a visual reflecting the shared coordinate system setup state in the application. If false, no visual is shown.")]
        [SerializeField]
        protected bool _useMarkerSpatialCoordinateVisual;

        /// <summary>
        /// MonoBehaviour that implements <see cref="IMarkerSpatialCoordinateServiceOverlayVisual"/> for the HoloLens experience. Note: an error is thrown if the MonoBehaviour does not implement IMarkerSpatialCoordinateServiceOverlayVisual.
        /// </summary>
        [Tooltip("MonoBehaviour that implements IMarkerSpatialCoordinateServiceOverlayVisual for the HoloLens experience. This visual is only shown if UseMarkerSpatialCoordinateVisual is set to true. Note: an error is thrown if the MonoBehaviour does not implement IMarkerSpatialCoordinateServiceOverlayVisual.")]
        [SerializeField]
        protected MonoBehaviour HoloLensOverlayVisual;

        /// <summary>
        /// MonoBehaviour that implements <see cref="IMarkerSpatialCoordinateServiceOverlayVisual"/> for the Mobile experience. Note: an error is thrown if the MonoBehaviour does not implement IMarkerSpatialCoordinateServiceOverlayVisual.
        /// </summary>
        [Tooltip("MonoBehaviour that implements IMarkerSpatialCoordinateServiceOverlayVisual for the Mobile experience. This visual is only shown if UseMarkerSpatialCoordinateVisual is set to true. Note: an error is thrown if the MonoBehaviour does not implement IMarkerSpatialCoordinateServiceOverlayVisual.")]
        [SerializeField]
        protected MonoBehaviour MobileOverlayVisual;

        /// <summary>
        /// MonoBehaviour that implements <see cref="IMarkerSpatialCoordinateServiceResetVisual"/> for the Mobile experience. Note: an error is thrown if the MonoBehaviour does not implement IMarkerSpatialCoordinateServiceResetVisual.
        /// </summary>
        [Tooltip("MonoBehaviour that implements IMarkerSpatialCoordinateServiceResetVisual for the Mobile experience. Note: an error is thrown if the MonoBehaviour does not implement IMarkerSpatialCoordinateServiceResetVisual.")]
        [SerializeField]
        protected MonoBehaviour MobileResetVisual;

        protected IMarkerSpatialCoordinateServiceOverlayVisual _markerSpatialCoordinateOverlayVisual;
        protected IMarkerSpatialCoordinateServiceResetVisual _markerSpatialCoordinateResetVisual;
        protected bool _needUIUpdate = true;
        protected bool _actAsUser = false;
        protected bool _initialized = false;
        protected bool _localOriginEstablished = false;
        protected bool _listeningToPointCloudChanges = false;
        protected VisualState _visualState = VisualState.none;
        protected GameObject _sharedOriginVisual;

#region User specific fields
        /// <summary>
        /// Marker detector used by the primary user to locate spectators in the shared scene.
        /// </summary>
        [Tooltip("Marker detector used by the primary user to locate spectators in the scene.")]
        [SerializeField]
        protected MonoBehaviour MarkerDetector;

        protected string _userPlayerId = "";
        protected Dictionary<string, string> _spectatorIds;
        protected IMarkerDetector _markerDetector;
        protected bool _detectingMarkers = false;
        protected Dictionary<string, GameObject> _markerVisuals = new Dictionary<string, GameObject>();
        protected Dictionary<string, GameObject> _cameraVisuals = new Dictionary<string, GameObject>();
        private User _cachedSelfUser;
#endregion

#region Spectator specific fields
        /// <summary>
        /// Number of point cloud points to detect before assessing the mobile device has found in the AR scene (default value is 4).
        /// </summary>
        [Tooltip("Number of point cloud points to detect before assessing the mobile device has found in the AR scene (default value is 4).")]
        [SerializeField]
        protected int _pointsRequiredForValidLocalOrigin = 4;

        /// <summary>
        /// Marker visual displayed by a spectator to the primary user in order to locate the spectator in the shared scene.
        /// </summary>
        [Tooltip("Marker visual displayed by a spectator to the primary user in order to locate the spectator in the shared scene.")]
        [SerializeField]
        protected MonoBehaviour MarkerVisual;

        protected IMarkerVisual _markerVisual;
        protected GameObject _userCameraVisual;
        private Spectator _cachedSelfSpectator = new Spectator();
        private User _cachedUser;
#endregion

        protected void OnValidate()
        {
#if UNITY_EDITOR
            FieldHelper.ValidateType<IMarkerDetector>(MarkerDetector);
            FieldHelper.ValidateType<IMarkerVisual>(MarkerVisual);
            FieldHelper.ValidateType<IMarkerSpatialCoordinateServiceOverlayVisual>(HoloLensOverlayVisual);
            FieldHelper.ValidateType<IMarkerSpatialCoordinateServiceOverlayVisual>(MobileOverlayVisual);
            FieldHelper.ValidateType<IMarkerSpatialCoordinateServiceResetVisual>(MobileResetVisual);
#endif
        }

        protected void Awake()
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
            if (_markerVisual == null ||
                !_markerVisual.TrySetMarkerSize(_markerSize))
            {
                Debug.LogWarning("Failed to set marker visual size.");
            }

            SetVisualState(VisualState.none);

            Initialize();
        }

        protected void Update()
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

                // Note: the SpectatorMarkerToSpectatorCamera defined here may vary in accuracy across devices.
                _cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.Position = Vector3.zero;
                _cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.Rotation = Quaternion.Euler(0, 180, 0);
                _cachedSelfSpectator.SpectatorMarkerToSpectatorCamera.Valid = true;
            }
        }

        /// <inheritdoc/>
        public event SpatialCoordinateStateUpdateHandler SpatialCoordinateStateUpdated;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void PlayerConnected(string playerId)
        {
        }

        /// <inheritdoc/>
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

        protected void Initialize()
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

        protected bool NeedsToPopulate()
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

        protected void Populate()
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

        protected bool WaitingOnUser()
        {
            // A spectator is waiting on the user if it does not have a valid marker id
            return !_actAsUser && !_cachedSelfSpectator.HasValidMarkerId();
        }

        protected void EstablishLocalOrigin()
        {
#if UNITY_WSA
            _localOriginEstablished = true;
#elif (UNITY_ANDROID || UNITY_IOS) && SPATIALALIGNMENT_LEGACY
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

#if (UNITY_ANDROID || UNITY_IOS) && SPATIALALIGNMENT_LEGACY
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

        protected byte[] GenerateStatePayload()
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

        protected void DetectMarkers()
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

        protected void StopDetectingMarkers()
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

        protected void ShowMarker()
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

        protected void HideMarker()
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

        protected void OnMarkersUpdated(Dictionary<int, Marker> markerDictionary)
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

        private TransformData GetLocalCameraTransform()
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

        private static bool TryCalculateUserOriginToSpectatorOriginTransform(Spectator spectator, out Matrix4x4 userOriginToSpectatorOrigin)
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

        private static bool TryCalculateSpectatorOriginToUserOriginTransform(Spectator spectator, out Matrix4x4 spectatorOriginToUserOrigin)
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

        protected static Vector4 GetPosition(Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        protected static Quaternion GetRotation(Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        protected void SetVisualState(VisualState visualState)
        {
            if (_visualState != visualState)
            {
                _visualState = visualState;

                // This setter may get called from different threads, but we want to always update UI on the main thread
                // So we flag the ui as needing an update for the next update pass
                _needUIUpdate = true;
            }
        }

        protected void OnResetSpatialCoordinates()
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

        protected void ShowDebugVisuals()
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

        private static void AssessAndCleanUpDebugVisuals(Dictionary<string, Spectator> knownSpectators, Dictionary<string, GameObject> debugVisuals)
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
