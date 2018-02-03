// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.Solvers
{
    public class SolverHandler : ControllerFinder
    {
        public enum TrackedObjectToReferenceEnum
        {
            /// <summary>
            /// Calculates position and orientation from the main camera
            /// </summary>
            Head,
            /// <summary>
            /// Calculates position and orientation from the left motion controller
            /// </summary>
            MotionControllerLeft,
            /// <summary>
            /// Calculates position and orientation from the right motion camera
            /// </summary>
            MotionControllerRight
        }

        [SerializeField]
        [Tooltip("Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field")]
        private TrackedObjectToReferenceEnum trackedObjectToReference = TrackedObjectToReferenceEnum.Head;

        public TrackedObjectToReferenceEnum TrackedObjectToReference
        {
            get { return trackedObjectToReference; }
            set { trackedObjectToReference = value; }
        }

        [SerializeField]
        [Tooltip("Manual override for TrackedObjectToReference if you want to use a scene object. Leave empty if you want to use Head or Motion controllers")]
        private Transform transformTarget;

        public Transform TransformTarget
        {
            get { return transformTarget; }
            set { transformTarget = value; }
        }

        public Vector3 GoalPosition { get; set; }

        public Quaternion GoalRotation { get; set; }

        public Vector3 GoalScale { get; set; }

        public Vector3Smoothed AltScale { get; set; }

        public float DeltaTime { get; set; }

        private float LastUpdateTime { get; set; }

        private List<Solver> m_Solvers = new List<Solver>();

        private void Awake()
        {
            m_Solvers.AddRange(GetComponents<Solver>());

            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = 0.0f;
        }

        private void Update()
        {
            DeltaTime = Time.realtimeSinceStartup - LastUpdateTime;
            LastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
            for (int i = 0; i < m_Solvers.Count; ++i)
            {
                Solver solver = m_Solvers[i];

                if (solver.enabled)
                {
                    solver.SolverUpdate();
                }
            }
        }

        [Serializable]
        public struct Vector3Smoothed
        {
            public Vector3 Current { get; set; }
            public Vector3 Goal { get; set; }
            public float SmoothTime { get; set; }

            public Vector3Smoothed(Vector3 value, float smoothingTime) : this()
            {
                Current = value;
                Goal = value;
                SmoothTime = smoothingTime;
            }

            public void Update(float deltaTime)
            {
                Current = Vector3.Lerp(Current, Goal, (Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
            }

            public void SetGoal(Vector3 newGoal)
            {
                Goal = newGoal;
            }
        }

        [Serializable]
        public struct QuaternionSmoothed
        {
            public Quaternion Current { get; set; }
            public Quaternion Goal { get; set; }
            public float SmoothTime { get; set; }

            public QuaternionSmoothed(Quaternion value, float smoothingTime) : this()
            {
                Current = value;
                Goal = value;
                SmoothTime = smoothingTime;
            }

            public void Update(float deltaTime)
            {
                Current = Quaternion.Slerp(Current, Goal, (Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
            }

            public void SetGoal(Quaternion newGoal)
            {
                Goal = newGoal;
            }
        }
    }
}