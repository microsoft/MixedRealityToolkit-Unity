// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace HoloToolkit.Unity
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
            set
            {
                if (trackedObjectToReference != value)
                {
                    trackedObjectToReference = value;
                    OnControllerLost();
                    AttachToNewTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("Add an additional offset of the tracked object to base the solver on. Useful for tracking something like a halo position above your head or off the side of a controller. Cannot be updated once Play begins.")]
        private Vector3 additionalOffset;

        [SerializeField]
        [Tooltip("Add an additional rotation on top of the tracked object. Useful for tracking what is essentially the up or right/left vectors. Cannot be updated once Play begins.")]
        private Vector3 additionalRotation;

        public Vector3 AdditionalOffset
        {
            get { return additionalOffset; }
            set
            {
                additionalOffset = value;
                TrackTransform(TransformTarget);
            }
        }

        public Vector3 AdditionalRotation
        {
            get { return additionalRotation; }
            set
            {
                additionalRotation = value;
                TrackTransform(TransformTarget);
            }
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

        protected List<Solver> m_Solvers = new List<Solver>();

        [SerializeField]
        private bool updateSolvers = true;
        public bool UpdateSolvers { get { return updateSolvers; } set { updateSolvers = value; } }

        private GameObject transformWithOffset;

        private void Awake()
        {
            m_Solvers.AddRange(GetComponents<Solver>());

            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = 0.0f;

            //TransformTarget overrides TrackedObjectToReference
            if (!TransformTarget)
            {
                AttachToNewTrackedObject();
            }
        }

        private void Update()
        {
            DeltaTime = Time.realtimeSinceStartup - LastUpdateTime;
            LastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
            if (UpdateSolvers)
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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (transformWithOffset != null)
            {
                Destroy(transformWithOffset);
            }
        }

        protected override void OnControllerFound()
        {
            if (!TransformTarget)
            {
                TrackTransform(ElementTransform);
            }
        }

        protected override void OnControllerLost()
        {
            TransformTarget = null;

            if (transformWithOffset != null)
            {
                Destroy(transformWithOffset);
                transformWithOffset = null;
            }
        }

        public virtual void AttachToNewTrackedObject()
        {
            switch (TrackedObjectToReference)
            {
                case TrackedObjectToReferenceEnum.Head:
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                    // No need to search for a controller if we've already attached to the head.
                    Handedness = InteractionSourceHandedness.Unknown;
#endif
                    TrackTransform(CameraCache.Main.transform);
                    break;
                case TrackedObjectToReferenceEnum.MotionControllerLeft:
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                    Handedness = InteractionSourceHandedness.Left;
#endif
                    break;
                case TrackedObjectToReferenceEnum.MotionControllerRight:
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                    Handedness = InteractionSourceHandedness.Right;
#endif
                    break;
            }
        }

        private void TrackTransform(Transform newTrackedTransform)
        {
            TransformTarget = MakeOffsetTransform(newTrackedTransform);
        }

        private Transform MakeOffsetTransform(Transform parentTransform)
        {
            if (transformWithOffset == null)
            {
                transformWithOffset = new GameObject();
                transformWithOffset.transform.parent = parentTransform;
            }

            transformWithOffset.transform.localPosition = AdditionalOffset;
            transformWithOffset.transform.localRotation = Quaternion.Euler(AdditionalRotation);
            transformWithOffset.name = string.Format("{0} on {1} with offset {2}, {3}", gameObject.name, TrackedObjectToReference.ToString(), AdditionalOffset, AdditionalRotation);
            // In order to account for the reversed normals due to the glTF coordinate system change, we need to provide rotated attachment points.
            transformWithOffset.transform.Rotate(0, 180, 0);

            return transformWithOffset.transform;
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