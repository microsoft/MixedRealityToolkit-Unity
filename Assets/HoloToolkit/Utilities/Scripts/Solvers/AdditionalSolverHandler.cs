// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    [RequireComponent(typeof(SolverHandler))]
    public class SecondaryTrackedObjectSolverHandler : SolverHandler
    {
        private Solver relatedSolver;

        protected void Start()
        {
            // We need to prevent this SolverHandler from updating all the other solvers, so they don't get updated twice every update.
            m_Solvers.Clear();
        }

        /// <summary>
        /// This function prevents this additional solver from updating all the other solvers that don't care about this additional solver.
        /// </summary>
        /// <param name="solver"></param>
        public void SetRelatedSolver(Solver solver)
        {
            relatedSolver = solver;
        }
    }
}