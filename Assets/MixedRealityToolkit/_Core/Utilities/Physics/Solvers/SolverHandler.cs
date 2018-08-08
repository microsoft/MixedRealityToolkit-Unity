// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Solvers
{
    /// <summary>
    /// This class handles the solver components that are attached to this <see cref="GameObject"/>
    /// </summary>
    [DisallowMultipleComponent]
    public class SolverHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The target transform that the solvers will act upon.")]
        private Transform transformTarget;

        /// <summary>
        /// The target transform that the solvers will act upon.
        /// </summary>
        public Transform TransformTarget
        {
            get { return transformTarget; }
            set { transformTarget = value; }
        }

        /// <summary>
        /// The position the solver is trying to move to.
        /// </summary>
        public Vector3 GoalPosition { get; private set; }

        /// <summary>
        /// The rotation the solver is trying to rotate to.
        /// </summary>
        public Quaternion GoalRotation { get; private set; }

        /// <summary>
        /// The scale the solver is trying to scale to.
        /// </summary>
        public Vector3 GoalScale { get; private set; }

        /// <summary>
        /// Alternate scale.
        /// </summary>
        public Vector3 AltScale { get; private set; }

        /// <summary>
        /// The timestamp the solvers will use to calculate with.
        /// </summary>
        public float DeltaTime { get; private set; }

        private float lastUpdateTime;

        private readonly List<Solver> solvers = new List<Solver>();

        #region MonoBehaviour Implementation

        private void OnValidate()
        {
            solvers.Clear();
            solvers.AddRange(GetComponents<Solver>());
        }

        private void Awake()
        {
            if (transformTarget == null)
            {
                transformTarget = transform;
            }

            GoalScale = Vector3.one;
            AltScale = Vector3.one;
            DeltaTime = 0.0f;
        }

        private void Update()
        {
            DeltaTime = Time.realtimeSinceStartup - lastUpdateTime;
            lastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
            for (int i = 0; i < solvers.Count; ++i)
            {
                Solver solver = solvers[i];

                if (solver.enabled)
                {
                    solver.SolverUpdate();
                }
            }
        }

        #endregion MonoBehaviour Implementation
    }
}