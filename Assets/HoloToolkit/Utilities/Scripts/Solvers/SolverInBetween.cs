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
    ///   InBetween solver positions an object inbetween two tracked transforms.
    /// </summary>
    public class SolverInBetween : Solver
    {
        [SerializeField]
        [Tooltip("Distance along the center line the object will be located. 0.5 is halfway, 1.0 is at the second transform, 0.0 is at the first transform.")]
        private float partwayOffset = 0.5f;

        public float PartwayOffset
        {
            get
            {
                return partwayOffset;
            }

            set
            {
                partwayOffset = value;
            }
        }

        [SerializeField]
        [Tooltip("Tracked object to calculate position and orientation for the second object. If you want to manually override and use a scene object, use the TransformTarget field")]
        [HideInInspector]
        private SolverHandler.TrackedObjectToReferenceEnum trackedObjectForSecondTransform = SolverHandler.TrackedObjectToReferenceEnum.Head;

        public SolverHandler.TrackedObjectToReferenceEnum TrackedObjectForSecondTransform
        {
            get { return trackedObjectForSecondTransform; }
            set
            {
                trackedObjectForSecondTransform = value;
                if (secondSolverHandler != null)
                {
                    secondSolverHandler.TrackedObjectToReference = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("This transform overrides any Tracked Object as the second point in the In Between.")]
        private Transform secondTransformOverride;

        private SecondaryTrackedObjectSolverHandler secondSolverHandler;

        private void OnValidate()
        {
            if (secondSolverHandler != null && trackedObjectForSecondTransform != secondSolverHandler.TrackedObjectToReference)
            {
                secondSolverHandler.TrackedObjectToReference = trackedObjectForSecondTransform;
            }
            if ( secondTransformOverride != null)
            {
                secondSolverHandler.TransformTarget = secondTransformOverride;
            }
        }

        protected override void Start()
        {
            // We need to get the secondSolverHandler ready before we tell them both to seek a tracked object.
            secondSolverHandler = gameObject.AddComponent<SecondaryTrackedObjectSolverHandler>();
            secondSolverHandler.SetRelatedSolver(this);
            secondSolverHandler.TrackedObjectToReference = TrackedObjectForSecondTransform;
            secondSolverHandler.TransformTarget = secondTransformOverride;

            base.Start();
        }

        public override void SeekTrackedObject()
        {
            base.SeekTrackedObject();

            if (!secondSolverHandler.TransformTarget)
            {
                StartCoroutine(CoStartSecond());
            }
        }

        public override void SolverUpdate()
        {
            if (solverHandler != null && secondSolverHandler != null)
            {
                if (solverHandler.TransformTarget != null && secondSolverHandler.TransformTarget != null)
                {
                    AdjustPositionForOffset(solverHandler.TransformTarget, secondSolverHandler.TransformTarget);
                }
            }
            
        }


        private void AdjustPositionForOffset(Transform targTransform, Transform secondTransform)
        {
            if (targTransform != null && secondTransform != null)
            {

                Vector3 centerline = targTransform.position - secondTransform.position;

                GoalPosition = secondTransform.position + (centerline * PartwayOffset);


                UpdateWorkingPosToGoal();
            }
        }

        /// <summary>
        /// We need to run another coroutine for the second transform so we can assign it to the SecondTransform. This code mirrors what's in the base Solver class.
        /// </summary>
        /// <returns></returns>
        // TODO: When we refactor Solver to have the co-routine in SolverHandler, we should use that instead of this.
        protected IEnumerator CoStartSecond()
        {
            if (secondTransformOverride != null)
            {
                secondSolverHandler.TransformTarget = secondTransformOverride;
            }
            else
            {
                switch (TrackedObjectForSecondTransform)
                {
                    case SolverHandler.TrackedObjectToReferenceEnum.Head:
                        while (CameraCache.Main == null || CameraCache.Main.transform == null)
                        {
                            yield return null;
                        }
                        //Base transform target to camera transform
                        secondSolverHandler.TransformTarget = CameraCache.Main.transform;
                        break;

                    case SolverHandler.TrackedObjectToReferenceEnum.MotionControllerLeft:
#if UNITY_WSA && UNITY_2017_2_OR_NEWER

                        secondSolverHandler.Handedness = InteractionSourceHandedness.Left;

                        while (secondSolverHandler.ElementTransform == null)
                        {
                            yield return null;
                        }
                        //Base transform target to Motion controller transform
                        secondSolverHandler.TransformTarget = secondSolverHandler.ElementTransform;
#endif
                        break;

                    case SolverHandler.TrackedObjectToReferenceEnum.MotionControllerRight:
#if UNITY_WSA && UNITY_2017_2_OR_NEWER

                        secondSolverHandler.Handedness = InteractionSourceHandedness.Right;
                        while (secondSolverHandler.ElementTransform == null)
                        {
                            yield return null;
                        }
                        //Base transform target to Motion controller transform
                        secondSolverHandler.TransformTarget = secondSolverHandler.ElementTransform;
#endif
                        break;
                }
            }
        }

        /// <summary>
        /// This should only be called from the SolverInBetweenEditor to cause the secondary solverhandler to reattach this solver.
        /// </summary>
        public void AttachSecondTransformToNewTrackedObject()
        {
            secondSolverHandler.TransformTarget = secondTransformOverride;
            secondSolverHandler.AttachToNewTrackedObject();
        }
    }
}
