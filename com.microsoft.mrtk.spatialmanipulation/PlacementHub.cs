using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    public class PlacementHub : MonoBehaviour
    {
        [SerializeReference]
        [InterfaceSelector]
        public List<ITransformation> transformations =new List<ITransformation>();

        (Vector3, Quaternion, Vector3) targetPose;

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

        [SerializeField]
        private bool useForces;
        private Rigidbody rigidBody => transform.GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            if (useForces && rigidBody != null && GetTargetTransform())
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
                if (!useForces)
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

        [SerializeReference, InterfaceSelector(false)]
        [DrawIf("shouldSmooth")]
        private ITransformSmoothingLogic smoothingLogic = new DefaultTransformSmoothingLogic();

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float moveLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float MoveLerpTime
        {
            get => moveLerpTime;
            set => moveLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float rotateLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float RotateLerpTime
        {
            get => rotateLerpTime;
            set => rotateLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float scaleLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float ScaleLerpTime
        {
            get => scaleLerpTime;
            set => scaleLerpTime = value;
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

            bool applySmoothing = UseSmoothing && smoothingLogic != null;

            targetPose.Position = (applySmoothing && !useForces) ? smoothingLogic.SmoothPosition(transform.position, targetPose.Position, moveLerpTime, Time.deltaTime) : targetPose.Position;
            targetPose.Rotation = (applySmoothing && !useForces) ? smoothingLogic.SmoothRotation(transform.rotation, targetPose.Rotation, rotateLerpTime, Time.deltaTime) : targetPose.Rotation;
            targetPose.Scale = applySmoothing ? smoothingLogic.SmoothScale(transform.localScale, targetPose.Scale, scaleLerpTime, Time.deltaTime) : targetPose.Scale;
        }

        #endregion

        #region rigidbodies
        [SerializeField]
        [Tooltip(
                "Apply torque to control orientation of the body")]
        private bool applyTorque = true;

        /// <summary>
        /// Apply torque to control orientation of the body
        /// </summary>
        public bool ApplyTorque
        {
            get => applyTorque;
            set => applyTorque = value;
        }

        [SerializeField]
        [Range(0.001f, 2.0f)]
        [Tooltip("The time scale at which a Rigidbody reacts to input movement defined as oscillation period of the dampened spring force.")]
        private float springForceSoftness = 0.1f;

        /// <summary>
        /// The time scale at which a Rigidbody reacts to input movement defined as oscillation period of the dampened spring force.
        /// </summary>
        public float SpringForceSoftness
        {
            get => springForceSoftness;
            set => springForceSoftness = value;
        }

        [SerializeField]
        [Range(0.001f, 2.0f)]
        [Tooltip("The time scale at which a Rigidbody reacts to input rotation defined as oscillation period of the dampened spring torque.")]
        private float springTorqueSoftness = 0.1f;

        /// <summary>
        /// The time scale at which a Rigidbody reacts to input rotation defined as oscillation period of the dampened angular spring force.
        /// </summary>
        public float SpringTorqueSoftness
        {
            get => springTorqueSoftness;
            set => springTorqueSoftness = value;
        }

        [SerializeField]
        [Range(0, 2.0f)]
        [Tooltip("The damping of the spring force&torque: 1.0f corresponds to critical damping, lower values lead to underdamping (i.e. oscillation).")]
        private float springDamping = 1.0f;

        /// <summary>
        /// The damping of the spring force&torque: 1.0f corresponds to critical damping, lower values lead to underdamping (i.e. oscillation).
        /// </summary>
        public float SpringDamping
        {
            get => springDamping;
            set => springDamping = value;
        }

        [SerializeField]
        [Range(0, 10000f)]
        [Tooltip("The maximum acceleration applied by the spring force to avoid trembling when pushing a body against a static object.")]
        private float springForceLimit = 100.0f;

        /// <summary>
        /// The maximum acceleration applied by the spring force to avoid trembling when pushing a body against a static object.
        /// </summary>
        public float SpringForceLimit
        {
            get => springForceLimit;
            set => springForceLimit = value;
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
            float omega = Mathf.PI / springForceSoftness;  // angular frequency, sqrt(k/m)

            Vector3 distance = transform.position - targetTransform.Position;

            // when player is moving, we need to anticipate where the targetTransform is going to be one time step from now
            distance -= referenceFrameVelocity * Time.fixedDeltaTime;

            var velocity = rigidBody.velocity;

            var acceleration = -distance * omega * omega;  // acceleration caused by spring force

            var accelerationMagnitude = acceleration.magnitude;

            // apply springForceLimit only for slow-moving body (e.g. pressed against wall)
            // when body is already moving fast, also allow strong acceleration
            var maxAcceleration = Mathf.Max(springForceLimit, 10 * velocity.magnitude / Time.fixedDeltaTime);

            if (accelerationMagnitude > maxAcceleration)
            {
                acceleration *= maxAcceleration / accelerationMagnitude;
            }

            // Apply damping - mathematically, we need e^(-2 * omega * dt)
            // To compensate for the finite time step, this is split in two equal factors,
            // one applied before, the other after the spring force
            // equivalent with applying damping as well as spring force continuously
            float halfDampingFactor = Mathf.Exp(-springDamping * omega * Time.fixedDeltaTime);

            velocity -= referenceFrameVelocity;  // change to the player's frame of reference before damping

            velocity *= halfDampingFactor;  // 1/2 damping
            velocity += acceleration * Time.fixedDeltaTime; // integration step of spring force
            velocity *= halfDampingFactor;  // 1/2 damping

            velocity += referenceFrameVelocity;  // change back to global frame of reference

            rigidBody.velocity = velocity;

            if (applyTorque)
            {
                // Torque calculations: same calculation & parameters as for linear velocity
                // skipping referenceFrameVelocity and springForceLimit which do not exactly apply here

                // implement critically dampened spring force, scaled to mass-independent frequency
                float angularOmega = Mathf.PI / springTorqueSoftness;  // angular frequency, sqrt(k/m)

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

/// <summary>
/// Interface which describes a transformation that is applied to MixedRealityTransform
/// </summary>
public interface ITransformation
{
    public (Vector3, Quaternion, Vector3) ApplyTransformation(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialLocalScale);

    /// <summary>
    /// Execution order priority of this constraint. Lower numbers will be executed before higher numbers.
    /// </summary>
    public int ExecutionPriority { get; }
}

/// <summary>
/// A transformation which restricts the scale of of the MixedRealityTransform
/// </summary>
public class MinMaxConstraintTransformation: ITransformation
{
    public Vector3 minScale = Vector3.one * 0.2f;

    public Vector3 maxScale = Vector3.one * 2.0f;

    protected const int scale_priority = -1000;
    protected const int rotation_priority = -1000;
    protected const int position_priority = -1000;
    protected const int constraint_priority_modifier = 1;

    public int ExecutionPriority => throw new NotImplementedException();

    public (Vector3, Quaternion, Vector3) ApplyTransformation(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialLocalScale)
    {
        initialLocalScale.x = Mathf.Clamp(initialLocalScale.x, minScale.x, maxScale.x);
        initialLocalScale.y = Mathf.Clamp(initialLocalScale.y, minScale.y, maxScale.y);
        initialLocalScale.z = Mathf.Clamp(initialLocalScale.z, minScale.z, maxScale.z);
        return (initialPosition, initialRotation, initialLocalScale);
    }
}
