using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    public class SpatialAnchorsCoordinateLocalizationInitializer : MonoBehaviour
    {
        /// <summary>
        /// Configuration for the Azure Spatial Anchors service.
        /// </summary>
        [SerializeField]
        [Tooltip("Configuration for the Azure Spatial Anchors service.")]
        private SpatialAnchorsConfiguration configuration = null;

        private void Start()
        {
            SpatialCoordinateSystemManager.Instance.ParticipantConnected += Instance_ParticipantConnected;
        }

        private void OnDestroy()
        {
            SpatialCoordinateSystemManager.Instance.ParticipantConnected -= Instance_ParticipantConnected;
        }

        private void Instance_ParticipantConnected(SpatialCoordinateSystemParticipant participant)
        {
            SpatialCoordinateSystemManager.Instance.LocalizeAsync(participant.SocketEndpoint, SpatialAnchorsLocalizer.Id, configuration);

            configuration.IsCoordinateCreator = true;
            SpatialCoordinateSystemManager.Instance.InitiateRemoteLocalization(participant.SocketEndpoint, SpatialAnchorsLocalizer.Id, configuration);
        }
    }
}