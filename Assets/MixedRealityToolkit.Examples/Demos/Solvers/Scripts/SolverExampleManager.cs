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
            this.TrackedType = TrackedObjectType.Head;
        }

        public void SetTrackedHands()
        {
            this.TrackedType = TrackedObjectType.HandJoint;
        }

        public void SetTrackedCustom()
        {
            this.TrackedType = TrackedObjectType.CustomOverride;
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
            var orbital = this.currentSolver as Orbital;
            orbital.LocalOffset = new Vector3(0.0f, -0.5f, 1.0f);
        }

        public void SetSurfaceMagnetism()
        {
            DestroySolver();

            AddSolver<SurfaceMagnetism>();

            // Modify properties of solver custom to this example
            var surfaceMagnetism = this.currentSolver as SurfaceMagnetism;
            surfaceMagnetism.SurfaceNormalOffset = 0.2f;
        }

        private void AddSolver<T>() where T : Solver
        {
            currentSolver = this.gameObject.AddComponent<T>();
            handler = this.GetComponent<SolverHandler>();
            RefreshSolverHandler();
        }

        private void RefreshSolverHandler()
        {
            if (handler != null)
            {
                this.handler.TrackedTargetType = this.TrackedType;
                this.handler.TrackedHandness = Handedness.Both;
                if (this.CustomTrackedObject != null)
                {
                    this.handler.TransformOverride = this.CustomTrackedObject.transform;
                }
            }
        }

        private void DestroySolver()
        {
            if (currentSolver != null)
            {
                DestroyImmediate(currentSolver);
                currentSolver = null;
            }

            if (handler != null)
            {
                DestroyImmediate(handler);
                handler = null;
            }
        }
    }
}