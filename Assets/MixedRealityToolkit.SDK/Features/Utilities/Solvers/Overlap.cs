// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that overlaps with the tracked object.
    /// </summary>
    public class Overlap : Solver
    {
        /// <inheritdoc />
        public override void SolverUpdate()
        {
            var target = SolverHandler.TransformTarget;
            if (target != null)
            {
                GoalPosition = target.position;
                GoalRotation = target.rotation;
            }
        }
    }
}