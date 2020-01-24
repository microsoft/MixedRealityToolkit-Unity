// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Shareable configuration for a proximity effect that can be used with BoundsControl
    /// ProximityEffect scales and switches out materials for registered objects whenever a pointer is in proximity.
    /// Scaling is done on three different stages: far / medium and close proximity whereas material switching 
    /// will only be done on close proximity.
    /// </summary>
    [CreateAssetMenu(fileName = "ProximityEffect", menuName = "Mixed Reality Toolkit/Experimental/Bounds Control/Proximity Effect")]
    public class ProximityEffectConfiguration : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Determines whether proximity feature (scaling and material toggling) is activated")]
        private bool proximityEffectActive = false;

        /// <summary>
        /// Determines whether proximity feature (scaling and material toggling) is activated
        /// </summary>
        public bool ProximityEffectActive { get => proximityEffectActive; set => proximityEffectActive = value; }

        [SerializeField]
        [Tooltip("How far away should the hand be from an object before it starts scaling the object?")]
        [Range(0.005f, 0.2f)]
        private float objectMediumProximity = 0.1f;
        /// <summary>
        /// Distance the hand has to be in to start scaling objects.
        /// </summary>
        public float ObjectMediumProximity { get => objectMediumProximity; set => objectMediumProximity = value; }

        [SerializeField]
        [Tooltip("How far away should the hand be from an object before it activates the close-proximity scaling effect?")]
        [Range(0.001f, 0.1f)]
        private float objectCloseProximity = 0.03f;
        /// <summary>
        /// Distance the hand has to be in to start the close proximity scaling effect.
        /// </summary>
        public float ObjectCloseProximity { get => objectCloseProximity; set => objectCloseProximity = value; }

        [SerializeField]
        [Tooltip("A Proximity-enabled object scales by this amount when a hand moves out of range. Default is 0, invisible object.")]
        private float farScale = 0.0f;

        /// <summary>
        /// A Proximity-enabled object scales by this amount when a hand moves out of range. Default is 0, invisible object.
        /// </summary>
        public float FarScale { get => farScale; set => farScale = value; }

        [SerializeField]
        [Tooltip("A Proximity-enabled object scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original object size.")]
        private float mediumScale = 1.0f;

        /// <summary>
        /// A Proximity-enabled object scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original object size.
        /// </summary>
        public float MediumScale { get => mediumScale; set => mediumScale = value; }

        [SerializeField]
        [Tooltip("A Proximity-enabled object scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger object size.")]
        private float closeScale = 1.5f;

        /// <summary>
        /// A Proximity-enabled object scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger object size
        /// </summary>
        public float CloseScale { get => closeScale; set => closeScale = value; }

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled object scale when the Hand moves from Medium proximity to Far proximity?")]
        [Range(0.0f, 1.0f)]
        private float farGrowRate = 0.3f;
        /// <summary>
        /// Rate a proximity scaled object scales when the hand moves from medium to far proximity.
        /// </summary>
        public float FarGrowRate { get => farGrowRate; set => farGrowRate = value; }

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Object scale when the Hand moves to a distance that activates Medium Scale ?")]
        [Range(0.0f, 1.0f)]
        private float mediumGrowRate = 0.2f;
        /// <summary>
        /// Rate a proximity scaled object scales when the hand moves from medium to close proximity.
        /// </summary>
        public float MediumGrowRate { get => mediumGrowRate; set => mediumGrowRate = value; }

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled object scale when the Hand moves to a distance that activates Close Scale ?")]
        [Range(0.0f, 1.0f)]
        private float closeGrowRate = 0.3f;
        /// <summary>
        /// Rate a proximity scaled object scales when the hand moves from close proximity to object center
        /// </summary>
        public float CloseGrowRate { get => closeGrowRate; set => closeGrowRate = value; }
    }
}
