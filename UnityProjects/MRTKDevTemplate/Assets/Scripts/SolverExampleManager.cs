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
        /// Method to change to the <see cref="RadialView"/> solver
        /// </summary>
        public void SetRadialView()
        {
            DestroySolver();

            AddSolver<RadialView>();
        }

        /// <summary>
        /// Method to change to the <see cref="Orbital"/> solver
        /// </summary>
        public void SetOrbital()
        {
            DestroySolver();

            AddSolver<Orbital>();

            // Modify properties of solver custom to this example
            var orbital = currentSolver as Orbital;
            orbital.LocalOffset = new Vector3(0.0f, -0f, 1.0f);
        }

        /// <summary>
        /// Method to change to the <see cref="SurfaceMagnetism"/> solver
        /// </summary>
        public void SetSurfaceMagnetism()
        {
            DestroySolver();

            AddSolver<SurfaceMagnetism>();

            // Modify properties of solver custom to this example
            var surfaceMagnetism = currentSolver as SurfaceMagnetism;
            surfaceMagnetism.SurfaceNormalOffset = 0.2f;
        }

        private void AddSolver<T>() where T : Solver
        {
            currentSolver = gameObject.AddComponent<T>();
            handler = GetComponent<SolverHandler>();
            RefreshSolverHandler();
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
                    handler.LeftInteractor = RightInteractor;
                }
            }
        }

        private void DestroySolver()
        {
            if (currentSolver != null)
            {
                Destroy(currentSolver);
                currentSolver = null;
            }
        }
    }
}
