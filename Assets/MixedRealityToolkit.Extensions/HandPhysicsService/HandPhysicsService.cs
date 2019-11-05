using Microsoft.MixedReality.Toolkit.Experimental.Input;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Extensions
{
    [MixedRealityExtensionService(SupportedPlatforms.WindowsUniversal)]
    public class HandPhysicsService : BaseExtensionService, IHandPhysicsService, IMixedRealityExtensionService
    {
        private HandPhysicsServiceProfile handPhysicsServiceProfile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public HandPhysicsService(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority, profile)
        {
            handPhysicsServiceProfile = (HandPhysicsServiceProfile)profile;
        }

        private IMixedRealityHandJointService HandJointService => handJointService ?? (MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>() as MixedRealityInputSystem).GetDataProvider<IMixedRealityHandJointService>();

        /// <inheritdoc />
        public GameObject HandPhysicsServiceRoot { get; private set; }

        /// <inheritdoc />
        public bool UsePalmKinematicBody { get; set; }

        /// <inheritdoc />
        public GameObject FingerTipKinematicBodyPrefab
        {
            get { return fingerTipKinematicBodyPrefab; }
            set
            {
                if (value != null)
                {
                    CreateKinematicBodies();
                }
                fingerTipKinematicBodyPrefab = value;
            }
        }

        private GameObject fingerTipKinematicBodyPrefab;

        /// <inheritdoc />
        public GameObject PalmKinematicBodyPrefab
        {
            get { return palmKinematicBodyPrefab; }
            set
            {
                if(value != null)
                {
                    CreateKinematicBodies();
                }

                palmKinematicBodyPrefab = value;
            }
        }

        private GameObject palmKinematicBodyPrefab;

        private IMixedRealityHandJointService handJointService = null;

        private static readonly Handedness[] handednessTypes = new Handedness[]
        {
            Handedness.Left,
            Handedness.Right
        };

        private static readonly TrackedHandJoint[] fingerTipTypes = new TrackedHandJoint[]
        {
            TrackedHandJoint.ThumbTip,
            TrackedHandJoint.IndexTip,
            TrackedHandJoint.MiddleTip,
            TrackedHandJoint.RingTip,
            TrackedHandJoint.PinkyTip
        };

        private List<JointKinematicBody> jointKinematicBodies = new List<JointKinematicBody>();

        #region BaseExtensionService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            UsePalmKinematicBody = handPhysicsServiceProfile.UsePalmKinematicBody;
            FingerTipKinematicBodyPrefab = handPhysicsServiceProfile.FingerTipKinematicBodyPrefab;
            PalmKinematicBodyPrefab = handPhysicsServiceProfile.PalmKinematicBodyPrefab;
        }

        /// <inheritdoc />
        public override void Enable()
        {
            HandPhysicsServiceRoot = new GameObject("Hand Physics Service");

            CreateKinematicBodies();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (HandPhysicsServiceRoot != null)
            {
                UnityEngine.Object.Destroy(HandPhysicsServiceRoot);
                HandPhysicsServiceRoot = null;
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            foreach (JointKinematicBody jointKinematicBody in jointKinematicBodies)
            {
                if (HandJointService.IsHandTracked(jointKinematicBody.HandednessType))
                {
                    jointKinematicBody.Joint = jointKinematicBody.Joint ?? HandJointService.RequestJointTransform(jointKinematicBody.JointType, jointKinematicBody.HandednessType);
                    jointKinematicBody.UpdateState(jointKinematicBody.Joint != null);
                }
                else
                {
                    jointKinematicBody.UpdateState(false);
                }
            }
        }

        #endregion BaseExtensionService Implementation

        #region HandPhysicsService Implementation

        private void CreateKinematicBodies()
        {
            if(jointKinematicBodies.Count > 0)
            {
                //Tear down the old kinematicBodies
                foreach (JointKinematicBody jointKinematicBody in jointKinematicBodies)
                {
                    UnityEngine.Object.Destroy(jointKinematicBody.gameObject);
                }
            }

            // Create joint kinematic bodies.
            for (int i = 0; i < handednessTypes.Length; ++i)
            {
                for (int j = 0; j < fingerTipTypes.Length; ++j)
                {
                    if(FingerTipKinematicBodyPrefab == null) { continue; }
                    if (TryCreateJointKinematicBody(FingerTipKinematicBodyPrefab, handednessTypes[i], fingerTipTypes[j], HandPhysicsServiceRoot.transform, out JointKinematicBody jointKinematicBody))
                    {
                        jointKinematicBodies.Add(jointKinematicBody);
                    }
                }

                if (UsePalmKinematicBody)
                {
                    if (PalmKinematicBodyPrefab == null) { continue; }
                    if (TryCreateJointKinematicBody(PalmKinematicBodyPrefab, handednessTypes[i], TrackedHandJoint.Palm, HandPhysicsServiceRoot.transform, out JointKinematicBody jointKinematicBody))
                    {
                        jointKinematicBodies.Add(jointKinematicBody);
                    }
                }
            }
        }

        private static bool TryCreateJointKinematicBody(GameObject rbPrefab, Handedness handednessType, TrackedHandJoint jointType, Transform parent, out JointKinematicBody jointKinematicBody)
        {
            jointKinematicBody = null;

            GameObject gO = GameObject.Instantiate(rbPrefab, parent);

            JointKinematicBody jkb = gO.GetComponent<JointKinematicBody>();

            if (jkb == null)
            {
                Debug.LogError("The HandPhysicsService assumes the FingerTipKinematicBodyPrefab has a JointKinematicBody component.");
                UnityEngine.Object.Destroy(gO);
                return false;
            }

            jkb.JointType = jointType;
            jkb.HandednessType = handednessType;
            gO.name = handednessType + " " + jointType;

            if (gO.GetComponent<Collider>() == null)
            {
                Debug.LogError("The HandPhysicsService assumes the FingerTipKinematicBodyPrefab has a Collder component.");
                UnityEngine.Object.Destroy(gO);
                return false;
            }

            Rigidbody rigidbody = gO.GetComponent<Rigidbody>();

            if (rigidbody == null)
            {
                Debug.LogError("The HandPhysicsService assumes the FingerTipKinematicBodyPrefab has a Rigidbody component.");
                UnityEngine.Object.Destroy(gO);
                return false;
            }

            if (!rigidbody.isKinematic)
            {
                Debug.LogWarning("The HandPhysicsService FingerTipKinematicBodyPrefab rigidbody should be marked as kinematic, making kinematic.");
                rigidbody.isKinematic = true;
            }

            jointKinematicBody = jkb;
            return true;
        }

        #endregion HandPhysicsService Implementation

    }
}