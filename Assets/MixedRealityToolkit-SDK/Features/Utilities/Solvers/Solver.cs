// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Utilities.Solvers
{
    /// <summary>
    /// The base abstract class for all Solvers to derive from. It provides state tracking, smoothing parameters
    /// and implementation, automatic solver system integration, and update order. Solvers may be used without a link,
    /// as long as updateLinkedTransform is false.
    /// </summary>
    [RequireComponent(typeof(SolverHandler))]
    public abstract class Solver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If true, the position and orientation will be calculated, but not applied, for other components to use")]
        private bool updateLinkedTransform = false;

        [SerializeField]
        [Tooltip("Position lerp multiplier")]
        private float moveLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("Rotation lerp multiplier")]
        private float rotateLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("Scale lerp multiplier")]
        private float scaleLerpTime = 0;

        [SerializeField]
        [Tooltip("If true, the Solver will respect the object's original scale values")]
        private bool maintainScale = true;

        [SerializeField]
        [Tooltip("Working output is smoothed if true. Otherwise, snapped")]
        private bool smoothing = true;

        [SerializeField]
        [Tooltip("If > 0, this solver will deactivate after this much time, even if the state is still active")]
        private float lifetime = 0;

        private float currentLifetime;

        /// <summary>
        /// The handler reference for this solver that's attached to this <see cref="GameObject"/>
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected SolverHandler SolverHandler;

        /// <summary>
        /// The final position to be attained
        /// </summary>
        protected Vector3 GoalPosition;

        /// <summary>
        /// The final rotation to be attained
        /// </summary>
        protected Quaternion GoalRotation;

        /// <summary>
        /// The final scale to be attained
        /// </summary>
        protected Vector3 GoalScale;

        /// <summary>
        /// Automatically uses the shared position if the solver is set to use the 'linked transform'.
        /// UpdateLinkedTransform may be set to false, and a solver will automatically update the object directly,
        /// and not inherit work done by other solvers to the shared position
        /// </summary>
        public Vector3 WorkingPosition
        {
            get
            {
                return updateLinkedTransform ? SolverHandler.GoalPosition : transform.position;
            }
            protected set
            {
                if (updateLinkedTransform)
                {
                    SolverHandler.GoalPosition = value;
                }
                else
                {
                    transform.position = value;
                }
            }
        }

        /// <summary>
        /// Rotation version of WorkingPosition
        /// </summary>
        public Quaternion WorkingRotation
        {
            get
            {
                return updateLinkedTransform ? SolverHandler.GoalRotation : transform.rotation;
            }
            protected set
            {
                if (updateLinkedTransform)
                {
                    SolverHandler.GoalRotation = value;
                }
                else
                {
                    transform.rotation = value;
                }
            }
        }

        /// <summary>
        /// Scale version of WorkingPosition
        /// </summary>
        public Vector3 WorkingScale
        {
            get
            {
                return updateLinkedTransform ? SolverHandler.GoalScale : transform.localScale;
            }
            protected set
            {
                if (updateLinkedTransform)
                {
                    SolverHandler.GoalScale = value;
                }
                else
                {
                    transform.localScale = value;
                }
            }
        }

        #region MonoBehaviour Implementation

        protected virtual void OnValidate()
        {
            if (SolverHandler == null)
            {
                SolverHandler = GetComponent<SolverHandler>();
            }
        }

        protected virtual void Awake()
        {
            if (updateLinkedTransform && SolverHandler == null)
            {
                Debug.LogError("No SolverHandler component found on " + name + " when UpdateLinkedTransform was set to true! Disabling UpdateLinkedTransform.");
                updateLinkedTransform = false;
            }

            GoalScale = maintainScale ? transform.localScale : Vector3.one;
        }

        /// <summary>
        /// Typically when a solver becomes enabled, it should update its internal state to the system, in case it was disabled far away
        /// </summary>
        protected virtual void OnEnable()
        {
            if (SolverHandler != null)
            {
                SnapGoalTo(SolverHandler.GoalPosition, SolverHandler.GoalRotation);
            }

            currentLifetime = 0;
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Should be implemented in derived classes, but Solver can be used to flush shared transform to real transform
        /// </summary>
        public abstract void SolverUpdate();

        /// <summary>
        /// Tracks lifetime of the solver, disabling it when expired, and finally runs the orientation update logic
        /// </summary>
        public void SolverUpdateEntry()
        {
            currentLifetime += SolverHandler.DeltaTime;

            if (lifetime > 0 && currentLifetime >= lifetime)
            {
                enabled = false;
                return;
            }

            SolverUpdate();
        }

        /// <summary>
        /// Snaps the solver to the desired pose.
        /// </summary>
        /// <remarks>
        /// SnapTo may be used to bypass smoothing to a certain position if the object is teleported or spawned.
        /// </remarks>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapTo(Vector3 position, Quaternion rotation)
        {
            SnapGoalTo(position, rotation);

            WorkingPosition = position;
            WorkingRotation = rotation;
        }

        /// <summary>
        /// SnapGoalTo only sets the goal orientation.  Not really useful.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapGoalTo(Vector3 position, Quaternion rotation)
        {
            GoalPosition = position;
            GoalRotation = rotation;
        }

        /// <summary>
        /// Add an offset position to the target goal position.
        /// </summary>
        /// <param name="offset"></param>
        public virtual void AddOffset(Vector3 offset)
        {
            GoalPosition += offset;
        }

        /// <summary>
        /// Lerps Vector3 source to goal.
        /// </summary>
        /// <remarks>
        /// Handles lerpTime of 0.
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Vector3 SmoothTo(Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
        {
            return Vector3.Lerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        /// Slerps Quaternion source to goal, handles lerpTime of 0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Quaternion SmoothTo(Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
        {
            return Quaternion.Slerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        /// Updates all object orientations to the goal orientation for this solver, with smoothing accounted for (smoothing may be off)
        /// </summary>
        protected void UpdateTransformToGoal()
        {
            if (smoothing)
            {
                Vector3 pos = transform.position;
                Quaternion rot = transform.rotation;
                Vector3 scale = transform.localScale;

                pos = SmoothTo(pos, GoalPosition, SolverHandler.DeltaTime, moveLerpTime);
                rot = SmoothTo(rot, GoalRotation, SolverHandler.DeltaTime, rotateLerpTime);
                scale = SmoothTo(scale, GoalScale, SolverHandler.DeltaTime, scaleLerpTime);

                transform.position = pos;
                transform.rotation = rot;
                transform.localScale = scale;
            }
            else
            {
                transform.position = GoalPosition;
                transform.rotation = GoalRotation;
                transform.localScale = GoalScale;
            }
        }

        /// <summary>
        /// Updates the Working orientation (which may be the object, or the shared orientation) to the goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingToGoal()
        {
            if (smoothing)
            {
                WorkingPosition = SmoothTo(WorkingPosition, GoalPosition, SolverHandler.DeltaTime, moveLerpTime);
                WorkingRotation = SmoothTo(WorkingRotation, GoalRotation, SolverHandler.DeltaTime, rotateLerpTime);
                WorkingScale = SmoothTo(WorkingScale, GoalScale, SolverHandler.DeltaTime, scaleLerpTime);
            }
            else
            {
                WorkingPosition = GoalPosition;
                WorkingRotation = GoalRotation;
                WorkingScale = GoalScale;
            }
        }

        /// <summary>
        /// Updates only the working position to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingPositionToGoal()
        {
            WorkingPosition = smoothing ? SmoothTo(WorkingPosition, GoalPosition, SolverHandler.DeltaTime, moveLerpTime) : GoalPosition;
        }

        /// <summary>
        /// Updates only the working rotation to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingRotationToGoal()
        {
            WorkingRotation = smoothing ? SmoothTo(WorkingRotation, GoalRotation, SolverHandler.DeltaTime, rotateLerpTime) : GoalRotation;
        }

        /// <summary>
        /// Updates only the working scale to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingScaleToGoal()
        {
            WorkingScale = smoothing ? SmoothTo(WorkingScale, GoalScale, SolverHandler.DeltaTime, scaleLerpTime) : GoalScale;
        }
    }
}
