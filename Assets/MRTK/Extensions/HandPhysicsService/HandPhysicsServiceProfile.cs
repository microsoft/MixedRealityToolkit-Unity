// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.HandPhysics
{
    /// <summary>
    /// Configuration profile for <see cref="HandPhysicsService"/> extension service.
    /// </summary>
	[MixedRealityServiceProfile(typeof(IHandPhysicsService))]
    [CreateAssetMenu(fileName = "HandPhysicsServiceProfile", menuName = "Mixed Reality Toolkit/Extensions/Hand Physics Service/Hand Physics Service Configuration Profile")]
    public class HandPhysicsServiceProfile : BaseMixedRealityProfile
    {
        /// <summary>
        /// Whether make the Palm a physics joint
        /// </summary>
        public bool UsePalmKinematicBody => usePalmKinematicBody;

        /// <summary>
        /// The prefab to represent each physics joint
        /// </summary>
        public GameObject FingerTipKinematicBodyPrefab => fingerTipKinematicBodyPrefab;

        /// <summary>
        /// The prefab to represent the palm physics joint
        /// </summary>
        public GameObject PalmKinematicBodyPrefab => palmKinematicBodyPrefab;

        /// <summary>
        /// The Layer the physics joints will be on
        /// </summary>
        public int HandPhysicsLayer => handPhysicsLayer;

        [SerializeField]
        [Tooltip("The Layer the physics joints will be on")]
        private int handPhysicsLayer = 0;

        [SerializeField]
        [Tooltip("The prefab to represent each physics joint")]
        private GameObject fingerTipKinematicBodyPrefab = null;

        [SerializeField]
        [Tooltip("Whether make the Palm a physics joint")]
        private bool usePalmKinematicBody = false;

        [SerializeField]
        [Tooltip("The prefab to represent the palm physics joint")]
        private GameObject palmKinematicBodyPrefab = null;
    }
}