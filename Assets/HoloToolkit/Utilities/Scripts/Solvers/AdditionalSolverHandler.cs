// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class AdditionalSolverHandler : SolverHandler
    {
        private Solver relatedSolver;        

        /// <summary>
        /// This function prevents this additional solver from updating all the other solvers that don't care about this additional solver.
        /// </summary>
        /// <param name="solver"></param>
        public void SetRelatedSolver(Solver solver)
        {
            relatedSolver = solver;
        }

        private void Start()
        {
            // We need to prevent this SolverHandler from updating all the other solvers, so they don't get updated twice.
            m_Solvers.Clear();
        }

        public override void AttachToNewTrackedObject()
        {
            switch (TrackedObjectToReference)
            {
                case TrackedObjectToReferenceEnum.MotionControllerLeft:
                default:
                    Handedness = UnityEngine.XR.WSA.Input.InteractionSourceHandedness.Left;
                    break;
                case TrackedObjectToReferenceEnum.MotionControllerRight:
                    Handedness = UnityEngine.XR.WSA.Input.InteractionSourceHandedness.Right;
                    break;
            }

            // We need to reattach to the appropriate controller
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(Handedness, out ControllerInfo))
            {
                AddControllerTransform(ControllerInfo);
            }
            // This is the key piece that differs from the parent. We don't want to tell all the sovlers to seek, just the one that matters to us.
            relatedSolver.SeekTrackedObject();
        }
    }
}