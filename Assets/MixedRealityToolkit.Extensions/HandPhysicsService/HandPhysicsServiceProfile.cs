using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Experimental.Extensions
{
	[MixedRealityServiceProfile(typeof(IHandPhysicsService))]
	[CreateAssetMenu(fileName = "HandPhysicsServiceProfile", menuName = "MixedRealityToolkit/Hand Physics Service Configuration Profile")]
	public class HandPhysicsServiceProfile : BaseMixedRealityProfile
	{
        public bool UsePalmKinematicBody => usePalmKinematicBody;
        public GameObject FingerTipKinematicBodyPrefab => fingerTipKinematicBodyPrefab;
        public GameObject PalmKinematicBodyPrefab => palmKinematicBodyPrefab;

        [SerializeField]
        [Tooltip("The prefab to represent each PhysicsJoint")]
        private GameObject fingerTipKinematicBodyPrefab = null;

        [SerializeField]
        [Tooltip("Whether make the Palm a PhysicsJoint")]
        private bool usePalmKinematicBody = false;
       
        [SerializeField]
        [Tooltip("The prefab to represent the Palm PhysicsJoint")]
        private GameObject palmKinematicBodyPrefab = null;
    }
}