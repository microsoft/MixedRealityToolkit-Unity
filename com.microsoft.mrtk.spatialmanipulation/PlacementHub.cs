// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    public class PlacementHub : MonoBehaviour
    {
        [SerializeReference]
        [InterfaceSelector]
        private List<ITransformation> transformations = new List<ITransformation>();

        internal List<ITransformation> Transformations => transformations;

        /// <summary>
        /// The target pose 
        /// </summary>
        private (Vector3, Quaternion, Vector3) targetPose;

        // Make this it's own class so we can potentially allow for alternate implementations which "blend" transformations or
        // use a transformation's execution order differently.
        private bool GetTargetTransform()
        {
            if (transformations.Count == 0)
            {
                return false;
            }

            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;
            Vector3 currentLocalScale = transform.localScale;

            for (int i = 0; i < transformations.Count; i++)
            {
                var t = transformations[i];
                (currentPosition, currentRotation, currentLocalScale) = t.ApplyTransformation(currentPosition, currentRotation, currentLocalScale);
            }

            targetPose = (currentPosition, currentRotation, currentLocalScale);
            return true;
        }

        // Update is called once per frame
        void Update()
        {
            if (GetTargetTransform())
            {
                ApplyTargetTransform();
            }
        }


        // When the player is carrying a Rigidbody, the physics damping of interaction should act within the moving frame of reference of the player.
        // The reference frame logic allows compensating for that 
        private Transform referenceFrameTransform = null;
        private bool referenceFrameHasLastPos = false;
        private Vector3 referenceFrameLastPos;
        public MixedRealityTransform targetTransform;
        public bool isManipulated;

        private Rigidbody rigidBody => transform.GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            if (UseForces && rigidBody != null && GetTargetTransform())
            {
                ApplyForcesToRigidbody();
            }
        }

        /// <summary>
        /// Once the <paramref name="targetTransform"/> has been determined, this method is called
        /// to apply the target pose to the object. Calls <see cref="SmoothTargetPose"/> before
        /// applying, to adjust the pose with smoothing, constraints, etc.
        /// </summary>
        private void ApplyTargetTransform()
        {
            targetTransform = new MixedRealityTransform(targetPose.Item1, targetPose.Item2, targetPose.Item3);

            // modifiedTransformFlags currently unused.
            TransformFlags modifiedTransformFlags = TransformFlags.None;
            SmoothTargetPose(ref targetTransform, ref modifiedTransformFlags);

            if (rigidBody == null)
            {
                transform.SetPositionAndRotation(targetTransform.Position, targetTransform.Rotation);
                transform.localScale = targetTransform.Scale;
            }
            else
            {
                // There is a Rigidbody. Potential different paths for near vs far manipulation
                if (!UseForces)
                {
                    rigidBody.MovePosition(targetTransform.Position);
                    rigidBody.MoveRotation(targetTransform.Rotation);
                }

                transform.localScale = targetTransform.Scale;
            }
        }

        #region smoothing
        [SerializeField]
        private bool useSmoothing = true;

        public bool UseSmoothing
        {
            get { return useSmoothing; }
            set { useSmoothing = value; }
        }

        [SerializeField]
        [DrawIf("useSmoothing")]
        private SmoothingSettings smoothingSettings;

        [Serializable]
        private struct SmoothingSettings
        {
            [SerializeReference, InterfaceSelector(false)]
            internal ITransformSmoothingLogic smoothingLogic;

            [SerializeField]
            [Range(0, 1)]
            [DefaultValue("0.001f")]
            [Tooltip("Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.")]
            internal float moveLerpTime;

            [SerializeField]
            [Range(0, 1)]
            [DefaultValue("0.001f")]
            [Tooltip("Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
            internal float rotateLerpTime;

            [SerializeField]
            [Range(0, 1)]
            [DefaultValue("0.001f")]
            [Tooltip("Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.")]
            internal float scaleLerpTime;
        }

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float MoveLerpTime
        {
            get => smoothingSettings.moveLerpTime;
            set => smoothingSettings.moveLerpTime = value;
        }

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float RotateLerpTime
        {
            get => smoothingSettings.rotateLerpTime;
            set => smoothingSettings.rotateLerpTime = value;
        }

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float ScaleLerpTime
        {
            get => smoothingSettings.scaleLerpTime;
            set => smoothingSettings.scaleLerpTime = value;
        }

        /// <summary>
        /// Called by ApplyTargetPose to modify the target pose with the relevant constraints, smoothing,
        /// elastic, or any other derived/overridden behavior.
        /// </summary>
        /// <param name="targetPose">
        /// The target position, rotation, and scale, pre-smoothing, but post-input and post-constraints. Modified by-reference.
        /// <param/>
        /// <param name="modifiedTransformFlags">
        /// Flags which parts of the transform (position, rotation, scale) have been altered by an external source (like Elastics).
        /// Modified by-reference.
        /// <param/>
        protected virtual void SmoothTargetPose(ref MixedRealityTransform targetPose, ref TransformFlags modifiedTransformFlags)
        {
            // TODO: Elastics. Compute elastics here and apply to modifiedTransformFlags.

            bool applySmoothing = UseSmoothing && smoothingSettings.smoothingLogic != null;

            targetPose.Position = (applySmoothing && !UseForces) ? smoothingSettings.smoothingLogic.SmoothPosition(transform.position, targetPose.Position, MoveLerpTime, Time.deltaTime) : targetPose.Position;
            targetPose.Rotation = (applySmoothing && !UseForces) ? smoothingSettings.smoothingLogic.SmoothRotation(transform.rotation, targetPose.Rotation, RotateLerpTime, Time.deltaTime) : targetPose.Rotation;
            targetPose.Scale = applySmoothing ? smoothingSettings.smoothingLogic.SmoothScale(transform.localScale, targetPose.Scale, ScaleLerpTime, Time.deltaTime) : targetPose.Scale;
        }

        #endregion

        #region rigidbodies
        [SerializeField]
        private bool useForces;

        // A little unsure of how the placement hub should manage UseForces, since
        // its rigidbody can be set to IsKinematic during manipulation.
        internal bool UseForces
        {
            get { return useForces; }
            set { useForces = value; }
        }

        [SerializeField]
        [DrawIf("useForces")]
        private PhysicsSettings physicsSettings;

        [Serializable]
        private struct PhysicsSettings
        {
            [SerializeField]
            [Range(0.001f, 2.0f)]
            [DefaultValue("0.1f")]
            [Tooltip("The time scale at which a Rigidbody reacts to input movement defined as oscillation period of the dampened spring force.")]
            internal float springForceSoftness;

            [SerializeField]
            [DefaultValue(true)]
            [Tooltip("Apply torque to control orientation of the body")]
            internal bool applyTorque;

            [SerializeField]
            [Range(0.001f, 2.0f)]
            [DefaultValue(0.1f)]
            [Tooltip("The time scale at which a Rigidbody reacts to input rotation defined as oscillation period of the dampened spring torque.")]
            internal float springTorqueSoftness;

            [SerializeField]
            [Range(0, 2.0f)]
            [DefaultValue(1.0f)]
            [Tooltip("The damping of the spring force&torque: 1.0f corresponds to critical damping, lower values lead to underdamping (i.e. oscillation).")]
            internal float springDamping;

            [SerializeField]
            [Range(0, 10000f)]
            [DefaultValue(100.0f)]
            [Tooltip("The maximum acceleration applied by the spring force to avoid trembling when pushing a body against a static object.")]
            internal float springForceLimit;
        }

        /// <summary>
        /// The time scale at which a Rigidbody reacts to input movement defined as oscillation period of the dampened spring force.
        /// </summary>
        public float SpringForceSoftness
        {
            get => physicsSettings.springForceSoftness;
            set => physicsSettings.springForceSoftness = value;
        }

        /// <summary>
        /// Apply torque to control orientation of the body
        /// </summary>
        public bool ApplyTorque
        {
            get => physicsSettings.applyTorque;
            set => physicsSettings.applyTorque = value;
        }

        /// <summary>
        /// The time scale at which a Rigidbody reacts to input rotation defined as oscillation period of the dampened angular spring force.
        /// </summary>
        public float SpringTorqueSoftness
        {
            get => physicsSettings.springTorqueSoftness;
            set => physicsSettings.springTorqueSoftness = value;
        }

        /// <summary>
        /// The damping of the spring force&torque: 1.0f corresponds to critical damping, lower values lead to underdamping (i.e. oscillation).
        /// </summary>
        public float SpringDamping
        {
            get => physicsSettings.springDamping;
            set => physicsSettings.springDamping = value;
        }

        /// <summary>
        /// The maximum acceleration applied by the spring force to avoid trembling when pushing a body against a static object.
        /// </summary>
        public float SpringForceLimit
        {
            get => physicsSettings.springForceLimit;
            set => physicsSettings.springForceLimit = value;
        }

        /// <summary>
        /// Override this class to provide the transform of the reference frame (e.g. the camera) against which to compute the damping.
        ///
        /// This intended for the situation of FPS-style controllers moving forward at constant speed while holding an object,
        /// to prevent damping from pushing the body towards the player.
        ///
        /// The transform that used will be used to define the reference frame or null to use the global reference frame</returns>
        public void SetReferenceFrameTransform(Transform t)
        {
            referenceFrameTransform = t;
            referenceFrameHasLastPos = false;
        }

        /// <summary>
        /// In case a Rigidbody gets the targetTransform applied using physical forcees, this function is called within the
        /// FixedUpdate() routine with physics-conforming time stepping.
        /// </summary>
        private void ApplyForcesToRigidbody()
        {
            var referenceFrameVelocity = Vector3.zero;

            if (referenceFrameTransform != null)
            {
                if (referenceFrameHasLastPos)
                {
                    referenceFrameVelocity = (referenceFrameTransform.position - referenceFrameLastPos) / Time.fixedDeltaTime;
                }

                referenceFrameLastPos = referenceFrameTransform.position;
                referenceFrameHasLastPos = true;
            }

            // implement critically dampened spring force, scaled to mass-independent frequency
            float omega = Mathf.PI / SpringForceSoftness;  // angular frequency, sqrt(k/m)

            Vector3 distance = transform.position - targetTransform.Position;

            // when player is moving, we need to anticipate where the targetTransform is going to be one time step from now
            distance -= referenceFrameVelocity * Time.fixedDeltaTime;

            var velocity = rigidBody.velocity;

            var acceleration = -distance * omega * omega;  // acceleration caused by spring force

            var accelerationMagnitude = acceleration.magnitude;

            // apply springForceLimit only for slow-moving body (e.g. pressed against wall)
            // when body is already moving fast, also allow strong acceleration
            var maxAcceleration = Mathf.Max(SpringForceLimit, 10 * velocity.magnitude / Time.fixedDeltaTime);

            if (accelerationMagnitude > maxAcceleration)
            {
                acceleration *= maxAcceleration / accelerationMagnitude;
            }

            // Apply damping - mathematically, we need e^(-2 * omega * dt)
            // To compensate for the finite time step, this is split in two equal factors,
            // one applied before, the other after the spring force
            // equivalent with applying damping as well as spring force continuously
            float halfDampingFactor = Mathf.Exp(-SpringDamping * omega * Time.fixedDeltaTime);

            velocity -= referenceFrameVelocity;  // change to the player's frame of reference before damping

            velocity *= halfDampingFactor;  // 1/2 damping
            velocity += acceleration * Time.fixedDeltaTime; // integration step of spring force
            velocity *= halfDampingFactor;  // 1/2 damping

            velocity += referenceFrameVelocity;  // change back to global frame of reference

            rigidBody.velocity = velocity;

            if (ApplyTorque)
            {
                // Torque calculations: same calculation & parameters as for linear velocity
                // skipping referenceFrameVelocity and springForceLimit which do not exactly apply here

                // implement critically dampened spring force, scaled to mass-independent frequency
                float angularOmega = Mathf.PI / SpringTorqueSoftness;  // angular frequency, sqrt(k/m)

                var angularDistance = transform.rotation * Quaternion.Inverse(targetTransform.Rotation);
                angularDistance.ToAngleAxis(out float angle, out Vector3 axis);

                if (!axis.IsValidVector())
                {
                    // ToAngleAxis is numerically unstable, returning NaN axis for near-zero angles
                    angle = 0;
                    axis = Vector3.up;
                }

                if (angle > 180f)
                {
                    angle -= 360f;
                }

                var angularVelocity = rigidBody.angularVelocity;

                var angularAcceleration = -angle * angularOmega * angularOmega;  // acceleration caused by spring force

                angularVelocity *= halfDampingFactor;  // 1/2 damping
                angularVelocity += angularAcceleration * Time.fixedDeltaTime * Mathf.Deg2Rad * axis.normalized; // integration step of spring force
                angularVelocity *= halfDampingFactor;  // 1/2 damping

                rigidBody.angularVelocity = angularVelocity;
            }
        }
        #endregion
    }
}
