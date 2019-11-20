// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class SolverExampleManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject CustomTrackedObject = null;

        private SolverHandler handler;
        private Solver currentSolver;

        private TrackedObjectType trackedType = TrackedObjectType.Head;
        public TrackedObjectType TrackedType
        {
            get { return trackedType; }
            set
            {
                if (trackedType != value)
                {
                    trackedType = value;
                    RefreshSolverHandler();
                }
            }
        }

        private void Awake()
        {
            SetRadialView();
        }

        public void SetTrackedHead()
        {
            TrackedType = TrackedObjectType.Head;
        }

        public void SetTrackedHands()
        {
            TrackedType = TrackedObjectType.HandJoint;
        }

        public void SetTrackedCustom()
        {
            TrackedType = TrackedObjectType.CustomOverride;
        }

        public void SetRadialView()
        {
            DestroySolver();

            AddSolver<RadialView>();
        }

        public void SetOrbital()
        {
            DestroySolver();

            AddSolver<Orbital>();

            // Modify properties of solver custom to this example
            var orbital = currentSolver as Orbital;
            orbital.LocalOffset = new Vector3(0.0f, -0.5f, 1.0f);
        }

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
                handler.TrackedHandness = Handedness.Both;
                if (CustomTrackedObject != null)
                {
                    handler.TransformOverride = CustomTrackedObject.transform;
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