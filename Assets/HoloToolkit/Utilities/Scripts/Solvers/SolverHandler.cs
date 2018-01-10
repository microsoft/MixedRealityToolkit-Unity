// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity
{
    public class SolverHandler : ControllerFinder
    {
        #region public enums
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
        #endregion

        #region public members
        [Tooltip("Tracked object to calculate position and orrientation from. If you want to manually override and use a scene object, use the TransformTarget field")]
        public TrackedObjectToReferenceEnum TrackedObjectToReference = TrackedObjectToReferenceEnum.Head;

        [Tooltip("Manual override for TrackedObjectToReference if you want to use a scene object. Leave empty if you want to use Head or Motion controllers")]
        public Transform TransformTarget;
        #endregion

        private List<Solver> m_Solvers = new List<Solver>();
        public Vector3 GoalPosition { get; set; }
        public Quaternion GoalRotation { get; set; }
        public Vector3 GoalScale { get; set; }
        public Vector3Smoothed AltScale { get; set; }
        public float DeltaTime { get; set; }

        private float LastUpdateTime { get; set; }

        private void Awake()
        {
            m_Solvers.AddRange(GetComponents<Solver>());

            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = 0.0f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
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

        [System.Serializable]
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
                Current = Vector3.Lerp(Current, Goal, (System.Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
            }

            public void SetGoal(Vector3 newGoal)
            {
                Goal = newGoal;
            }
        }

        [System.Serializable]
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
                Current = Quaternion.Slerp(Current, Goal, (System.Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
            }

            public void SetGoal(Quaternion newGoal)
            {
                Goal = newGoal;
            }
        }
    }
}