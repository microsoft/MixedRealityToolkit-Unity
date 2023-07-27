// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using SpatialManipulation;
    using UnityEngine.XR.Interaction.Toolkit;

    /// <summary>
    /// Manager class for the solver examples scene.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/SolverExampleManager")]
    public class SolverExampleManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject CustomTrackedObject = null;

        [SerializeField]
        [Tooltip("The interactor used when solving for the left hand / controller.")]
        private XRBaseInteractor LeftInteractor = null;


        [SerializeField]
        [Tooltip("The interactor used when solving for the right hand / controller.")]
        private XRBaseInteractor RightInteractor = null;

        private SolverHandler handler;
        private Solver currentSolver;
        private TrackedObjectType trackedType = TrackedObjectType.Head;

        /// <summary>
        /// Get or set the type of object to be tracked.
        /// </summary>
        public TrackedObjectType TrackedType
        {
            get => trackedType;
            set
            {
                if (trackedType != value)
                {
                    trackedType = value;
                    RefreshSolverHandler();
                }
            }
        }

        private readonly Vector3 HandJointRotationFix = new Vector3(90f, 0f, 0f);

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            SetRadialView();
        }

        /// <summary>
        /// Method to change the tracking type to the user's head
        /// </summary>
        public void SetTrackedHead()
        {
            TrackedType = TrackedObjectType.Head;
        }

        /// <summary>
        /// Method to change the tracking type to controller ray
        /// </summary>
        public void SetTrackedController()
        {
            TrackedType = TrackedObjectType.ControllerRay;
        }

        /// <summary>
        /// Method to change the tracking type to the hand joint
        /// </summary>
        public void SetTrackedHands()
        {
            TrackedType = TrackedObjectType.HandJoint;
        }

        /// <summary>
        /// Method to change the tracking type to custom
        /// </summary>
        public void SetTrackedCustom()
        {
            TrackedType = TrackedObjectType.CustomOverride;
        }

        /// <summary>
        /// Method to change to the <see cref="Follow"/> solver.
        /// </summary>
        public void SetFollowSolver()
        {
            EnableSolver<Follow>();
        }

        /// <summary>
        /// Method to change to the <see cref="RadialView"/> solver.
        /// </summary>
        public void SetRadialView()
        {
            EnableSolver<RadialView>();
        }

        /// <summary>
        /// Method to change to the <see cref="Orbital"/> solver.
        /// </summary>
        public void SetOrbital()
        {
            EnableSolver<Orbital>();
        }

        /// <summary>
        /// Method to change to the <see cref="SurfaceMagnetism"/> solver
        /// </summary>
        public void SetSurfaceMagnetism()
        {
            EnableSolver<SurfaceMagnetism>();
        }

        private void EnableSolver<T>() where T : Solver
        {
            DisableCurrentSolver();
            currentSolver = gameObject.EnsureComponent<T>();
            handler = gameObject.EnsureComponent<SolverHandler>();
            RefreshSolverHandler();
            currentSolver.enabled = true;
        }

        private void RefreshSolverHandler()
        {
            if (handler != null)
            {
                handler.TrackedTargetType = TrackedType;

#if WINDOWS_UWP || UNITY_EDITOR
                // If using input simulation in editor or on Windows Mixed Reality platform, forward vector points through the fingers, not the palm.
                // Apply additional rotation to correct this so that forward will be through the palm and outward
                handler.AdditionalRotation = (TrackedType == TrackedObjectType.HandJoint) ? HandJointRotationFix : Vector3.zero;
#endif

                handler.TrackedHandedness = Handedness.Both;
                if (CustomTrackedObject != null)
                {
                    handler.TransformOverride = CustomTrackedObject.transform;
                }

                if (LeftInteractor != null)
                {
                    handler.LeftInteractor = LeftInteractor;
                }
                if (RightInteractor != null)
                {
                    handler.RightInteractor = RightInteractor;
                }
            }
        }

        private void DisableCurrentSolver()
        {
            if (currentSolver != null)
            {
                currentSolver.enabled = false;
                currentSolver = null;
            }
        }
    }
}
