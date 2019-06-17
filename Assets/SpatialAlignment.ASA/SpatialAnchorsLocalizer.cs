// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// This is the localization mechanism for enabling anchor exchange/localization through Azure Spatial Anchors.
    /// </summary>
    public class SpatialAnchorsLocalizer : SpatialLocalizer<SpatialAnchorsConfiguration>
    {
        internal static readonly Guid Id = new Guid("FB077E49-B855-453F-9FB5-77187B1AF784");

        public override Guid SpatialLocalizerId => Id;

        /// <summary>
        /// Location of the anchor used for localization.
        /// </summary>
        [SerializeField]
        [Tooltip("Rotation of the anchor used for localization.")]
        private Vector3 anchorPosition = Vector3.zero;

        /// <summary>
        /// Location of the anchor used for localization.
        /// </summary>
        [SerializeField]
        [Tooltip("Rotation of the anchor used for localization.")]
        private Vector3 anchorRotation = Vector3.zero;

        public override bool TryDeserializeSettings(BinaryReader reader, out SpatialAnchorsConfiguration settings)
        {
            return SpatialAnchorsConfiguration.TryDeserialize(reader, out settings);
        }

        public override bool TryCreateLocalizationSession(IPeerConnection peerConnection, SpatialAnchorsConfiguration settings, out ISpatialLocalizationSession session)
        {
            if ((string.IsNullOrWhiteSpace(settings.AccountId) || string.IsNullOrWhiteSpace(settings.AccountKey)) && string.IsNullOrWhiteSpace(settings.AuthenticationToken) && string.IsNullOrWhiteSpace(settings.AccessToken))
            {
                Debug.LogError("Authentication method not configured for Azure Spatial Anchors, ensure you configured AccountID and AccountKey, or Authentication Token, or Access Token.", this);
                session = null;
                return false;
            }

#if !SPATIALALIGNMENT_ASA
            Debug.LogError("Attempting to use SpatialAnchorLocalizer but ASA is not enabled for this build");
            session = null;
            return false;
#elif UNITY_WSA && SPATIALALIGNMENT_ASA
            session = new SpatialCoordinateLocalizationSession(this, new SpatialAnchorsUWPCoordinateService(settings), settings, peerConnection);
            return true;
#elif UNITY_ANDROID && SPATIALALIGNMENT_ASA
            session = new SpatialCoordinateLocalizationSession(this, new SpatialAnchorsAndroidCoordinateService(settings), settings, peerConnection);
            return true;
#elif UNITY_IOS && SPATIALALIGNMENT_ASA
            Debug.LogError("SpatialAnchorLocalizer does not yet support iOS");
            session = null;
            return false;
#endif
        }

#if SPATIALALIGNMENT_ASA
        private event Action Updated;

        private void Update()
        {
            Updated?.Invoke();
        }

        private class SpatialCoordinateLocalizationSession : DisposableBase, ISpatialLocalizationSession
        {
            private readonly IPeerConnection peerConnection;
            private readonly SpatialAnchorsCoordinateService coordinateService;
            private readonly SpatialAnchorsConfiguration configuration;
            private readonly SpatialAnchorsLocalizer localizer;
            private readonly TaskCompletionSource<string> coordinateIdentifierTaskSource;

            public SpatialCoordinateLocalizationSession(SpatialAnchorsLocalizer localizer, SpatialAnchorsCoordinateService coordinateService, SpatialAnchorsConfiguration configuration, IPeerConnection peerConnection)
            {
                this.localizer = localizer;
                this.coordinateService = coordinateService;
                this.configuration = configuration;
                this.coordinateIdentifierTaskSource = new TaskCompletionSource<string>();
                this.peerConnection = peerConnection;

                localizer.Updated += OnUpdated;
            }

            private void OnUpdated()
            {
                coordinateService.FrameUpdate();
            }

            public async Task<ISpatialCoordinate> LocalizeAsync(CancellationToken cancellationToken)
            {
                ISpatialCoordinate coordinateToReturn = null;
                if (configuration.IsCoordinateCreator)
                {
                    localizer.DebugLog("User getting initialized coordinate");
                    coordinateToReturn = await coordinateService.TryCreateCoordinateAsync(localizer.anchorPosition, Quaternion.Euler(localizer.anchorRotation), cancellationToken);

                    localizer.DebugLog($"Sending coordinate id: {coordinateToReturn.Id}");
                    peerConnection.SendData(writer => writer.Write(coordinateToReturn.Id));

                    localizer.DebugLog("Message sent.");
                }
                else
                {
                    localizer.DebugLog("Non-host waiting for coord id to be sent over");
                    string coordinateIdentifier = await coordinateIdentifierTaskSource.Task.Unless(cancellationToken);

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        localizer.DebugLog($"Coordinate id: {coordinateIdentifier}, starting discovery.");
                        if (await coordinateService.TryDiscoverCoordinatesAsync(cancellationToken, coordinateIdentifier))
                        {
                            localizer.DebugLog("Discovery complete, retrieving reference to ISpatialCoordinate");
                            if (!coordinateService.TryGetKnownCoordinate(coordinateIdentifier, out coordinateToReturn))
                            {
                                Debug.LogError("We discovered, but for some reason failed to get coordinate from service.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Failed to discover spatial coordinate.");
                        }
                    }
                }

                return coordinateToReturn;
            }

            protected override void OnManagedDispose()
            {
                base.OnManagedDispose();

                this.coordinateService.Dispose();
                localizer.Updated -= OnUpdated;
            }

            public void OnDataReceived(BinaryReader reader)
            {
                coordinateIdentifierTaskSource.SetResult(reader.ReadString());
            }
        }
#endif
    }
}
