// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class SolverExampleManager : MonoBehaviour
    {
        private SolverHandler handler;
        private Solver currentSolver;

        private void Awake()
        {
            SetRadialView();
        }

        public void SetRadialView()
        {
            DestroySolver();

            AddSolver<RadialView>();

            // Configur radial view here
        }

        public void SetOrbital()
        {
            DestroySolver();

            AddSolver<Orbital>();

            // Configur radial view here
        }

        public void SetSurfaceMagnetism()
        {
            DestroySolver();

            AddSolver<SurfaceMagnetism>();

            // Configur radial view here
        }

        private void AddSolver<T>() where T : Solver
        {
            currentSolver = this.gameObject.AddComponent<T>();
            handler = this.GetComponent<SolverHandler>();
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