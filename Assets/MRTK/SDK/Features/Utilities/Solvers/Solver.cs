// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// The base abstract class for all Solvers to derive from. It provides state tracking, smoothing parameters
    /// and implementation, automatic solver system integration, and update order. Solvers may be used without a link,
    /// as long as updateLinkedTransform is false.
    /// </summary>
    [RequireComponent(typeof(SolverHandler))]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_Solver.html")]
    public abstract class Solver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If true, the position and orientation will be calculated, but not applied, for other components to use")]
        private bool updateLinkedTransform = false;

        /// <summary>
        /// If true, the position and orientation will be calculated, but not applied, for other components to use
        /// </summary>
        public bool UpdateLinkedTransform
        {
            get => updateLinkedTransform;
            set => updateLinkedTransform = value;
        }

        [SerializeField]
        [Tooltip("If 0, the position will update immediately.  Otherwise, the greater this attribute the slower the position updates")]
        private float moveLerpTime = 0.1f;

        /// <summary>
        /// If 0, the position will update immediately.  Otherwise, the greater this attribute the slower the position updates
        /// </summary>
        public float MoveLerpTime
        {
            get => moveLerpTime;
            set => moveLerpTime = value;
        }

        [SerializeField]
        [Tooltip("If 0, the rotation will update immediately.  Otherwise, the greater this attribute the slower the rotation updates")]
        private float rotateLerpTime = 0.1f;

        /// <summary>
        /// If 0, the rotation will update immediately.  Otherwise, the greater this attribute the slower the rotation updates")]
        /// </summary>
        public float RotateLerpTime
        {
            get => rotateLerpTime;
            set => rotateLerpTime = value;
        }

        [SerializeField]
        [Tooltip("If 0, the scale will update immediately.  Otherwise, the greater this attribute the slower the scale updates")]
        private float scaleLerpTime = 0;

        /// <summary>
        /// If 0, the scale will update immediately.  Otherwise, the greater this attribute the slower the scale updates
        /// </summary>
        public float ScaleLerpTime
        {
            get => scaleLerpTime;
            set => scaleLerpTime = value;
        }

        [SerializeField]
        [Tooltip("If true, the Solver will respect the object's original scale values")]
        private bool maintainScale = true;

        [SerializeField]
        [Tooltip("If true, updates are smoothed to the target. Otherwise, they are snapped to the target")]
        private bool smoothing = true;

        /// <summary>
        /// If true, updates are smoothed to the target. Otherwise, they are snapped to the target
        /// </summary>
        public bool Smoothing
        {
            get => smoothing;
            set => smoothing = value;
        }

        [SerializeField]
        [Tooltip("If > 0, this solver will deactivate after this much time, even if the state is still active")]
        private float lifetime = 0;

        private float currentLifetime;

        /// <summary>
        /// The handler reference for this solver that's attached to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
        /// </summary>
        [HideInInspector]
        protected SolverHandler SolverHandler;

        /// <summary>
        /// The final position to be attained
        /// </summary>
        protected Vector3 GoalPosition
        {
            get { return SolverHandler.GoalPosition; }
            set { SolverHandler.GoalPosition = value; }
        }

        /// <summary>
        /// The final rotation to be attained
        /// </summary>
        protected Quaternion GoalRotation
        {
            get { return SolverHandler.GoalRotation; }
            set { SolverHandler.GoalRotation = value; }
        }

        /// <summary>
        /// The final scale to be attained
        /// </summary>
        protected Vector3 GoalScale
        {
            get { return SolverHandler.GoalScale; }
            set { SolverHandler.GoalScale = value; }
        }

        /// <summary>
        /// Automatically uses the shared position if the solver is set to use the 'linked transform'.
        /// UpdateLinkedTransform may be set to false, and a solver will automatically update the object directly,
        /// and not inherit work done by other solvers to the shared position
        /// </summary>
        public Vector3 WorkingPosition
        {
            get
            {
                return updateLinkedTransform ? GoalPosition : transform.position;
            }
            protected set
            {
                if (updateLinkedTransform)
                {
                    GoalPosition = value;
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
                return updateLinkedTransform ? GoalRotation : transform.rotation;
            }
            protected set
            {
                if (updateLinkedTransform)
                {
                    GoalRotation = value;
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
                return updateLinkedTransform ? GoalScale : transform.localScale;
            }
            protected set
            {
                if (updateLinkedTransform)
                {
                    GoalScale = value;
                }
                else
                {
                    transform.localScale = value;
                }
            }
        }

        #region MonoBehaviour Implementation

        protected virtual void Awake()
        {
            if (SolverHandler == null)
            {
                SolverHandler = GetComponent<SolverHandler>();
            }

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
                SnapGoalTo(GoalPosition, GoalRotation, GoalScale);
            }

            currentLifetime = 0;
        }

        protected virtual void Start()
        {
            if (SolverHandler != null)
            {
                SolverHandler.RegisterSolver(this);
            }
        }
        protected virtual void OnDestroy()
        {
            if (SolverHandler != null)
            {
                SolverHandler.UnregisterSolver(this);
            }
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
            UpdateWorkingToGoal();
        }

        /// <summary>
        /// Snaps the solver to the desired pose.
        /// </summary>
        /// <remarks>
        /// SnapTo may be used to bypass smoothing to a certain position if the object is teleported or spawned.
        /// </remarks>
        public virtual void SnapTo(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            SnapGoalTo(position, rotation, scale);

            WorkingPosition = position;
            WorkingRotation = rotation;
            WorkingScale = scale;
        }

        /// <summary>
        /// SnapGoalTo only sets the goal orientation.  Not really useful.
        /// </summary>
        public virtual void SnapGoalTo(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GoalPosition = position;
            GoalRotation = rotation;
            GoalScale = scale;
        }

        /// <summary>
        /// Snaps the solver to the desired pose.
        /// </summary>
        /// <remarks>
        /// SnapTo may be used to bypass smoothing to a certain position if the object is teleported or spawned.
        /// </remarks>
        [Obsolete("Use SnapTo(Vector3, Quaternion, Vector3) instead.")]
        public virtual void SnapTo(Vector3 position, Quaternion rotation)
        {
            SnapGoalTo(position, rotation);

            WorkingPosition = position;
            WorkingRotation = rotation;
        }

        /// <summary>
        /// SnapGoalTo only sets the goal orientation.  Not really useful.
        /// </summary>
        [Obsolete("Use SnapGoalTo(Vector3, Quaternion, Vector3) instead.")]
        public virtual void SnapGoalTo(Vector3 position, Quaternion rotation)
        {
            GoalPosition = position;
            GoalRotation = rotation;
        }

        /// <summary>
        /// Add an offset position to the target goal position.
        /// </summary>
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
        public static Vector3 SmoothTo(Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
        {
            return Vector3.Lerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        /// Slerps Quaternion source to goal, handles lerpTime of 0
        /// </summary>
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
            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
            UpdateWorkingScaleToGoal();
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
