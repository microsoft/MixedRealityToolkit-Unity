// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Applies acceleration/velocity/friction to simulate momentum for an object being moved by other solvers/components
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/Momentum")]
    public class Momentum : Solver
    {
        #region Momentum Parameters
        [SerializeField]
        [Tooltip("Friction to slow down the current velocity")]
        private float resistance = 0.99f;

        /// <summary>
        /// Friction to slow down the current velocity
        /// </summary>
        public float Resistance
        {
            get { return resistance; }
            set { resistance = value; }
        }

        [SerializeField]
        [Tooltip("Apply more resistance when going faster- applied resistance is resistance * (velocity ^ resistanceVelocityPower)")]
        private float resistanceVelocityPower = 1.5f;

        /// <summary>
        /// Apply more resistance when going faster- applied resistance is resistance * (velocity ^ resistanceVelocityPower)
        /// </summary>
        public float ResistanceVelocityPower
        {
            get { return resistanceVelocityPower; }
            set { resistanceVelocityPower = value; }
        }

        [SerializeField]
        [Tooltip("Accelerate to goal position at this rate")]
        private float accelerationRate = 10f;

        /// <summary>
        /// Accelerate to goal position at this rate
        /// </summary>
        public float AccelerationRate
        {
            get { return accelerationRate; }
            set { accelerationRate = value; }
        }

        [SerializeField]
        [Tooltip("Apply more acceleration if farther from target- applied acceleration is accelerationRate + springiness * distance")]
        private float springiness = 0;

        /// <summary>
        /// Apply more acceleration if farther from target- applied acceleration is accelerationRate + springiness * distance
        /// </summary>
        public float Springiness
        {
            get { return springiness; }
            set { springiness = value; }
        }

        [SerializeField]
        [Tooltip("Instantly maintain a constant depth from the view point instead of simulating Z-velocity")]
        private bool snapZ = true;

        /// <summary>
        /// Instantly maintain a constant depth from the view point instead of simulating Z-velocity
        /// </summary>
        public bool SnapZ
        {
            get { return snapZ; }
            set { snapZ = value; }
        }

        #endregion

        private Vector3 velocity;

        private Vector3 ReferencePosition => SolverHandler.TransformTarget == null ? Vector3.zero : SolverHandler.TransformTarget.position;

        protected override void OnEnable()
        {
            base.OnEnable();

            velocity = Vector3.zero;
        }

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            // Start with snapZ
            if (SnapZ)
            {
                // Snap the current depth to the goal depth
                var referencePosition = ReferencePosition;
                float goalDepth = (SolverHandler.GoalPosition - referencePosition).magnitude;
                Vector3 currentDelta = transform.position - referencePosition;
                float currentDeltaMagnitude = currentDelta.magnitude;

                if (!Mathf.Approximately(currentDeltaMagnitude, 0))
                {
                    Vector3 currentDeltaNorm = currentDelta / currentDeltaMagnitude;
                    transform.position += currentDeltaNorm * (goalDepth - currentDeltaMagnitude);
                }
            }

            // Determine and apply acceleration
            Vector3 delta = SolverHandler.GoalPosition - transform.position;
            float deltaMagnitude = delta.magnitude;

            if (deltaMagnitude > 0.01f)
            {
                Vector3 deltaNorm = delta / deltaMagnitude;
                velocity += deltaNorm * (SolverHandler.DeltaTime * (AccelerationRate + Springiness * deltaMagnitude));
            }

            // Resistance
            float velocityMagnitude = velocity.magnitude;

            if (!Mathf.Approximately(velocityMagnitude, 0))
            {
                Vector3 velocityNormal = velocity / velocityMagnitude;
                float powFactor = velocityMagnitude > 1f ? Mathf.Pow(velocityMagnitude, ResistanceVelocityPower) : velocityMagnitude;
                velocity -= velocityNormal * (powFactor * Resistance * SolverHandler.DeltaTime);
            }

            if (velocity.sqrMagnitude < 0.001f)
            {
                velocity = Vector3.zero;
            }

            // Apply velocity to the solver... no wait, the actual transform
            transform.position += velocity * SolverHandler.DeltaTime;
        }

        /// <inheritdoc />
        public override void SnapTo(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            base.SnapTo(position, rotation, scale);
            velocity = Vector3.zero;
        }
    }
}
