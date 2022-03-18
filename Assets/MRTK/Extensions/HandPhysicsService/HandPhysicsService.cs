// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.HandPhysics
{
    /// <summary>
    /// A simple service that creates KinematicRigidbodies on fingertips for physics interactions.
    /// </summary>
    [MixedRealityExtensionService(
        (SupportedPlatforms)(-1),
        "Hand Physics Service",
        "HandPhysicsService/Profiles/DefaultHandPhysicsServiceProfile.asset",
        "MixedRealityToolkit.Extensions",
        true)]
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

        private IMixedRealityHandJointService HandJointService
            => handJointService ?? CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();

        /// <inheritdoc />
        public GameObject HandPhysicsServiceRoot { get; private set; }

        /// <inheritdoc />
        public int HandPhysicsLayer { get; set; }

        /// <inheritdoc />
        public bool UsePalmKinematicBody { get; set; }

        /// <inheritdoc />
        public GameObject FingerTipKinematicBodyPrefab
        {
            get { return fingerTipKinematicBodyPrefab; }
            set
            {
                fingerTipKinematicBodyPrefab = value;
                if (fingerTipKinematicBodyPrefab != null)
                {
                    CreateKinematicBodies();
                }
                else
                {
                    DestroyKinematicBodies();
                }
            }
        }

        private GameObject fingerTipKinematicBodyPrefab;

        /// <inheritdoc />
        public GameObject PalmKinematicBodyPrefab
        {
            get { return palmKinematicBodyPrefab; }
            set
            {
                palmKinematicBodyPrefab = value;
                if (palmKinematicBodyPrefab != null)
                {
                    CreateKinematicBodies();
                }
                else
                {
                    DestroyKinematicBodies();
                }
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
            base.Initialize();
            HandPhysicsLayer = handPhysicsServiceProfile.HandPhysicsLayer;
            UsePalmKinematicBody = handPhysicsServiceProfile.UsePalmKinematicBody;
            FingerTipKinematicBodyPrefab = handPhysicsServiceProfile.FingerTipKinematicBodyPrefab;
            PalmKinematicBodyPrefab = handPhysicsServiceProfile.PalmKinematicBodyPrefab;
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
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
            base.Disable();
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] HandPhysicsService.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                base.Update();
                foreach (JointKinematicBody jointKinematicBody in jointKinematicBodies)
                {
                    if (HandJointService.IsHandTracked(jointKinematicBody.HandednessType))
                    {
                        jointKinematicBody.Joint = jointKinematicBody.Joint != null ? jointKinematicBody.Joint : HandJointService.RequestJointTransform(jointKinematicBody.JointType, jointKinematicBody.HandednessType);
                        jointKinematicBody.UpdateState(jointKinematicBody.Joint != null);
                    }
                    else
                    {
                        jointKinematicBody.UpdateState(false);
                    }
                }
            }
        }

        #endregion BaseExtensionService Implementation

        #region HandPhysicsService Implementation

        /// <summary>
        /// Sets up the service by iterating over joints
        /// </summary>
        private void CreateKinematicBodies()
        {
            DestroyKinematicBodies();

            if (HandPhysicsServiceRoot == null) { return; }

            // Create joint kinematic bodies.
            for (int i = 0; i < handednessTypes.Length; ++i)
            {
                for (int j = 0; j < fingerTipTypes.Length; ++j)
                {
                    if (FingerTipKinematicBodyPrefab == null) { continue; }
                    if (TryCreateJointKinematicBody(FingerTipKinematicBodyPrefab, HandPhysicsLayer, handednessTypes[i], fingerTipTypes[j], HandPhysicsServiceRoot.transform, out JointKinematicBody jointKinematicBody))
                    {
                        jointKinematicBodies.Add(jointKinematicBody);
                    }
                }

                if (UsePalmKinematicBody)
                {
                    if (PalmKinematicBodyPrefab == null) { continue; }
                    if (TryCreateJointKinematicBody(PalmKinematicBodyPrefab, HandPhysicsLayer, handednessTypes[i], TrackedHandJoint.Palm, HandPhysicsServiceRoot.transform, out JointKinematicBody jointKinematicBody))
                    {
                        jointKinematicBodies.Add(jointKinematicBody);
                    }
                }
            }
        }

        /// <summary>
        /// Destroys the existing joints
        /// </summary>
        private void DestroyKinematicBodies()
        {
            if (jointKinematicBodies.Count > 0)
            {
                // Tear down the old kinematicBodies
                foreach (JointKinematicBody jointKinematicBody in jointKinematicBodies)
                {
                    UnityEngine.Object.Destroy(jointKinematicBody.gameObject);
                }
            }
        }

        /// <summary>
        /// Instantiates <see cref="FingerTipKinematicBodyPrefab"/>s for all <see cref="fingerTipTypes"/>.
        /// </summary>
        /// <remarks>
        /// Optionally instantiates <see cref="PalmKinematicBodyPrefab"/>.
        /// </remarks>
        /// <param name="rigidBodyPrefab">The prefab to instantiate.</param>
        /// <param name="layer">the layer to put the prefab on.</param>
        /// <param name="handednessType">the specified <see cref="Handedness"/> for the joint.</param>
        /// <param name="jointType">the specified <see cref="TrackedHandJoint"/> to instantiate against.</param>
        /// <param name="parent">The root <see href="https://docs.unity3d.com/ScriptReference/GameObject.html"> for the joints.</param>
        /// <param name="jointKinematicBody">When successful, the generated <see cref="JointKinematicBody"/>.</param>
        /// <returns>True when able to successfully intantiate and create a <see cref="JointKinematicBody"/>.</returns>
        private static bool TryCreateJointKinematicBody(GameObject rigidBodyPrefab, int layer, Handedness handednessType, TrackedHandJoint jointType, Transform parent, out JointKinematicBody jointKinematicBody)
        {
            jointKinematicBody = null;

            GameObject currentGameObject = GameObject.Instantiate(rigidBodyPrefab, parent);
            currentGameObject.layer = layer;
            JointKinematicBody currentJoint = currentGameObject.GetComponent<JointKinematicBody>();

            if (currentJoint == null)
            {
                Debug.LogError("The HandPhysicsService assumes the FingerTipKinematicBodyPrefab has a JointKinematicBody component.");
                UnityEngine.Object.Destroy(currentGameObject);
                return false;
            }

            currentJoint.JointType = jointType;
            currentJoint.HandednessType = handednessType;
            currentGameObject.name = handednessType + " " + jointType;

            if (currentGameObject.GetComponent<Collider>() == null)
            {
                Debug.LogError("The HandPhysicsService assumes the FingerTipKinematicBodyPrefab has a Collider component.");
                UnityEngine.Object.Destroy(currentGameObject);
                return false;
            }

            Rigidbody rigidbody = currentGameObject.GetComponent<Rigidbody>();

            if (rigidbody == null)
            {
                Debug.LogError("The HandPhysicsService assumes the FingerTipKinematicBodyPrefab has a Rigidbody component.");
                UnityEngine.Object.Destroy(currentGameObject);
                return false;
            }

            if (!rigidbody.isKinematic)
            {
                Debug.LogWarning("The HandPhysicsService FingerTipKinematicBodyPrefab rigidbody should be marked as kinematic, making kinematic.");
                rigidbody.isKinematic = true;
            }

            jointKinematicBody = currentJoint;
            return true;
        }

        #endregion HandPhysicsService Implementation
    }
}