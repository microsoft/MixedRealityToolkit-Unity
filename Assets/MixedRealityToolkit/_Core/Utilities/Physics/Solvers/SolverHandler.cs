// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Solvers
{
    public class SolverHandler : MonoBehaviour
    {
        [SerializeField]
        private Transform transformTarget;

        public Transform TransformTarget
        {
            get { return transformTarget; }
            set { transformTarget = value; }
        }

        public Vector3 GoalPosition { get; private set; }

        public Quaternion GoalRotation { get; private set; }

        public Vector3 GoalScale { get; private set; }

        public Vector3 AltScale { get; private set; }

        public float DeltaTime { get; private set; }

        private float lastUpdateTime;

        private readonly List<Solver> solvers = new List<Solver>();

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
    }
}