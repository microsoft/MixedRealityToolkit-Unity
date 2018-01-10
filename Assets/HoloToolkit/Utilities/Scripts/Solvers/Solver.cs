//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HoloToolkit.Unity.InputModule;
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    ///   SolverBase is the base abstract class for all Solvers to derive from.  It provides state tracking, smoothing parameters
    ///   and implementation, automatic solver system integration, and update order.  Solvers may be used without a link,
    ///   as long as UpdateLinkedTransform is false.
    /// </summary>
    [RequireComponent(typeof(SolverHandler))]
    public abstract class Solver : MonoBehaviour
    {
        #region public members
        [Tooltip("If true, the position and orientation will be calculated, but not applied, for other components to use")]
        public bool UpdateLinkedTransform = false;

        [Tooltip("Position lerp multiplier")]
        public float MoveLerpTime = 0.1f;
        [Tooltip("Rotation lerp multiplier")]
        public float RotateLerpTime = 0.1f;
        [Tooltip("Scale lerp multiplier")]
        public float ScaleLerpTime = 0;

        [Tooltip("If true, the Solver will respect the object's original scale values")]
        public bool MaintainScale = true;

        // Field that may be used to represent final position to be smoothly attained
        [HideInInspector]
        public Vector3 GoalPosition;
        // Field that may be used to represent final rotation to be smoothly attained
        [HideInInspector]
        public Quaternion GoalRotation;
        // Field that may be used to represent final scale to be smoothly attained
        [HideInInspector]
        public Vector3 GoalScale;

        [Tooltip("Working output is smoothed if true.  Otherwise snapped")]
        public bool Smoothing = true;

        [Tooltip("If > 0, this solver will deactivate after this much time, even if the state is still active")]
        public float Lifetime = 0;
        #endregion

        #region private members
        protected SolverHandler solverHandler;
        private float lifetime;
        #endregion

        protected virtual void Awake()
        {
            solverHandler = GetComponent<SolverHandler>();

            if (UpdateLinkedTransform && solverHandler == null)
            {
                Debug.LogError("No SolverHandler component found on " + name + " when UpdateLinkedTransform was set to true!  Disabling UpdateLinkedTransform");
                UpdateLinkedTransform = false;
            }

            GoalScale = MaintainScale == true ? this.transform.localScale : Vector3.one;
        }

        /// <summary>
        ///   Typically when a solver becomes enabled, it should update its internal state to the system, in case it was disabled far away
        /// </summary>
        protected virtual void OnEnable()
        {
            //Ensure the Camera helper component exists
            CameraCache.Main.gameObject.EnsureComponent<CameraMotionInfo>();

            if (solverHandler != null)
            {
                SnapGoalTo(solverHandler.GoalPosition, solverHandler.GoalRotation);
            }

            lifetime = 0;
        }

        protected virtual void Start()
        {
            //TransformTarget overrides ObjectToReferenceEnum
            if (!solverHandler.TransformTarget)
            {
                StartCoroutine(CoStart());
            }
        }

        protected IEnumerator CoStart()
        {
                switch (solverHandler.TrackedObjectToReference)
                {
                    case SolverHandler.TrackedObjectToReferenceEnum.Head:
                        while (CameraCache.Main == null || CameraCache.Main.transform == null)
                        {
                            yield return null;
                        }
                        //Base transform target to camera transform
                        solverHandler.TransformTarget = CameraCache.Main.transform;
                        break;

                    case SolverHandler.TrackedObjectToReferenceEnum.MotionControllerLeft:
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                        solverHandler.Handedness = InteractionSourceHandedness.Left;
                        while (solverHandler.ElementTransform == null)
                        {
                            yield return null;
                        }
                        //Base transform target to Motion controller transform
                        solverHandler.TransformTarget = solverHandler.ElementTransform;
#endif
                        break;

                    case SolverHandler.TrackedObjectToReferenceEnum.MotionControllerRight:
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                        solverHandler.Handedness = InteractionSourceHandedness.Right;
                        while (solverHandler.ElementTransform != null)
                        {
                            yield return null;
                        }
                        //Base transform target to Motion controller transform
                        solverHandler.TransformTarget = solverHandler.ElementTransform;
#endif
                        break;
                }
        }

        // SolverLink will pass transform through
        // Should be implemented in derived classes, but SolverBase can be used to flush shared transform to real transform
        public abstract void SolverUpdate();

        /// <summary>
        ///    Tracks lifetime of the solver, disabling it when expired, and finally runs the orientation update logic
        /// </summary>
        public void SolverUpdateEntry()
        {
            lifetime += solverHandler.DeltaTime;
            if (Lifetime > 0 && lifetime >= Lifetime)
            {
                enabled = false;
                return;
            }

            SolverUpdate();
        }

        /// <summary>
        ///   SnapTo may be used to bypass smoothing to a certain position if the object is teleported or spawned
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapTo(Vector3 position, Quaternion rotation)
        {
            SnapGoalTo(position, rotation);

            WorkingPos = position;
            WorkingRot = rotation;
        }

        /// <summary>
        ///   SnapGoalTo only sets the goal orientation.  Not really useful.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void SnapGoalTo(Vector3 position, Quaternion rotation)
        {
            GoalPosition = position;
            GoalRotation = rotation;
        }

        public virtual void AddOffset(Vector3 offset)
        {
            GoalPosition += offset;
        }

        /// <summary>
        ///   WorkingPos automatically uses the shared position if the solver is set to use the 'linked transform'.
        ///   UpdateLinkedTransform may be set to false, and a solver will automatically update the object directly,
        ///   and not inherit work done by other solvers to the shared position
        /// </summary>
        public Vector3 WorkingPos
        {
            get
            {
                if (UpdateLinkedTransform)
                {
                    return solverHandler.GoalPosition;
                }
                else
                {
                    return transform.position;
                }
            }

            set
            {
                if (UpdateLinkedTransform)
                {
                    solverHandler.GoalPosition = value;
                }
                else
                {
                    this.transform.position = value;
                }
            }
        }

        /// <summary>
        ///   Rotation version of WorkingPos
        /// </summary>
        public Quaternion WorkingRot
        {
            get
            {
                if (UpdateLinkedTransform)
                {
                    return solverHandler.GoalRotation;
                }
                else
                {
                    return transform.rotation;
                }
            }

            set
            {
                if (UpdateLinkedTransform)
                {
                    solverHandler.GoalRotation = value;
                }
                else
                {
                    this.transform.rotation = value;
                }
            }
        }

        /// <summary>
        ///    Scale version of WorkingPos
        /// </summary>
        public Vector3 WorkingScale
        {
            get
            {
                if (UpdateLinkedTransform)
                {
                    return solverHandler.GoalScale;
                }
                else
                {
                    return transform.localScale;
                }
            }

            set
            {
                if (UpdateLinkedTransform)
                {
                    solverHandler.GoalScale = value;
                }
                else
                {
                    this.transform.localScale = value;
                }
            }
        }

        /// <summary>
        ///    Lerps Vector3 source to goal, handles lerpTime of 0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Vector3 SmoothTo(Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
        {
            return Vector3.Lerp(source, goal, lerpTime == 0 ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        ///   Slerps Quaternion source to goal, handles lerpTime of 0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Quaternion SmoothTo(Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
        {
            return Quaternion.Slerp(source, goal, lerpTime == 0 ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        ///    Updates all object orientations to the goal orientation for this solver, with smoothing accounted for (smoothing may be off)
        /// </summary>
        protected void UpdateTransformToGoal()
        {
            if (Smoothing)
            {
                Vector3 pos = this.transform.position;
                Quaternion rot = this.transform.rotation;
                Vector3 scale = this.transform.localScale;

                float dt = solverHandler.DeltaTime;
                pos = SmoothTo(pos, GoalPosition, dt, MoveLerpTime);
                rot = SmoothTo(rot, GoalRotation, dt, RotateLerpTime);
                scale = SmoothTo(scale, GoalScale, dt, ScaleLerpTime);

                this.transform.position = pos;
                this.transform.rotation = rot;
                this.transform.localScale = scale;
            }
            else
            {
                this.transform.position = GoalPosition;
                this.transform.rotation = GoalRotation;
                this.transform.localScale = GoalScale;
            }
        }

        /// <summary>
        ///   Updates the Working orientation (which may be the object, or the shared orientation) to the goal, with smoothing accounted for
        /// </summary>
        public void UpdateWorkingToGoal()
        {
            if (Smoothing)
            {
                float dt = solverHandler.DeltaTime;
                WorkingPos = SmoothTo(WorkingPos, GoalPosition, dt, MoveLerpTime);
                WorkingRot = SmoothTo(WorkingRot, GoalRotation, dt, RotateLerpTime);
                WorkingScale = SmoothTo(WorkingScale, GoalScale, dt, ScaleLerpTime);
            }
            else
            {
                WorkingPos = GoalPosition;
                WorkingRot = GoalRotation;
                WorkingScale = GoalScale;
            }
        }

        /// <summary>
        ///    Updates only the working position to goal witih smoothing
        /// </summary>
        public void UpdateWorkingPosToGoal()
        {
            if (Smoothing)
            {
                WorkingPos = SmoothTo(WorkingPos, GoalPosition, solverHandler.DeltaTime, MoveLerpTime);
            }
            else
            {
                WorkingPos = GoalPosition;
            }
        }

        /// <summary>
        ///    Updates only the working rotation to goal witih smoothing
        /// </summary>
        public void UpdateWorkingRotToGoal()
        {
            if (Smoothing)
            {
                WorkingRot = SmoothTo(WorkingRot, GoalRotation, solverHandler.DeltaTime, RotateLerpTime);
            }
            else
            {
                WorkingRot = GoalRotation;
            }
        }

        /// <summary>
        ///    Updates only the working scale to goal witih smoothing
        /// </summary>
        public void UpdateWorkingScaleToGoal()
        {
            if (Smoothing)
            {
                WorkingScale = SmoothTo(WorkingScale, GoalScale, solverHandler.DeltaTime, ScaleLerpTime);
            }
            else
            {
                WorkingScale = GoalScale;
            }
        }
    }

}
