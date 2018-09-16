// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.UX.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Utilities.Solvers
{
    /// <summary>
    /// This class handles the solver components that are attached to this <see cref="GameObject"/>
    /// </summary>
    public class SolverHandler : BaseInputHandler, IMixedRealitySourceStateHandler
    {
        [SerializeField]
        [Tooltip("Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.")]
        private TrackedObjectType trackedObjectToReference = TrackedObjectType.None;

        /// <summary>
        /// Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        public TrackedObjectType TrackedObjectToReference
        {
            get { return trackedObjectToReference; }
            set
            {
                if (trackedObjectToReference != value)
                {
                    trackedObjectToReference = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("Add an additional offset of the tracked object to base the solver on. Useful for tracking something like a halo position above your head or off the side of a controller.")]
        private Vector3 additionalOffset;

        /// <summary>
        /// Add an additional offset of the tracked object to base the solver on. Useful for tracking something like a halo position above your head or off the side of a controller.
        /// </summary>
        public Vector3 AdditionalOffset
        {
            get { return additionalOffset; }
            set
            {
                additionalOffset = value;
                TransformTarget = MakeOffsetTransform(TransformTarget);
            }
        }

        [SerializeField]
        [Tooltip("Add an additional rotation on top of the tracked object. Useful for tracking what is essentially the up or right/left vectors.")]
        private Vector3 additionalRotation;

        /// <summary>
        /// Add an additional rotation on top of the tracked object. Useful for tracking what is essentially the up or right/left vectors.
        /// </summary>
        public Vector3 AdditionalRotation
        {
            get { return additionalRotation; }
            set
            {
                additionalRotation = value;
                TransformTarget = MakeOffsetTransform(TransformTarget);
            }
        }

        [SerializeField]
        [Tooltip("Manual override for TrackedObjectToReference if you want to use a scene object. Leave empty if you want to use head or motion-tracked controllers.")]
        private Transform transformTarget;

        /// <summary>
        /// The target transform that the solvers will act upon.
        /// </summary>
        public Transform TransformTarget
        {
            get { return transformTarget; }
            set { transformTarget = value; }
        }

        [SerializeField]
        [Tooltip("Whether or not this SolverHandler calls SolverUpdate() every frame. Only one SolverHandler should manage SolverUpdate(). This setting does not affect whether the Target Transform of this SolverHandler gets updated or not.")]
        private bool updateSolvers = true;

        /// <summary>
        /// Whether or not this SolverHandler calls SolverUpdate() every frame. Only one SolverHandler should manage SolverUpdate(). This setting does not affect whether the Target Transform of this SolverHandler gets updated or not.
        /// </summary>
        public bool UpdateSolvers
        {
            get { return updateSolvers; }
            set { updateSolvers = value; }
        }

        /// <summary>
        /// The position the solver is trying to move to.
        /// </summary>
        public Vector3 GoalPosition { get; set; }

        /// <summary>
        /// The rotation the solver is trying to rotate to.
        /// </summary>
        public Quaternion GoalRotation { get; set; }

        /// <summary>
        /// The scale the solver is trying to scale to.
        /// </summary>
        public Vector3 GoalScale { get; set; }

        /// <summary>
        /// Alternate scale.
        /// </summary>
        public Vector3Smoothed AltScale { get; set; }

        /// <summary>
        /// The timestamp the solvers will use to calculate with.
        /// </summary>
        public float DeltaTime { get; set; }

        private bool RequiresOffset => AdditionalOffset.sqrMagnitude != 0 || AdditionalRotation.sqrMagnitude != 0;

        protected readonly List<Solver> solvers = new List<Solver>();

        private float lastUpdateTime;
        private GameObject transformWithOffset;

        #region MonoBehaviour Implementation

        private async void Awake()
        {
            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = 0.0f;

            solvers.AddRange(GetComponents<Solver>());

            await WaitUntilInputSystemValid;

            // TransformTarget overrides TrackedObjectToReference
            if (trackedObjectToReference != TrackedObjectType.None)
            {
                AttachToNewTrackedObject();
            }
        }

        private void Update()
        {
            DeltaTime = Time.realtimeSinceStartup - lastUpdateTime;
            lastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
            if (UpdateSolvers)
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

        protected void OnDestroy()
        {
            if (transformWithOffset != null)
            {
                Destroy(transformWithOffset);
            }
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Clears the transform target and attaches to the current <see cref="TrackedObjectToReference"/>.
        /// </summary>
        public void RefreshTrackedObject()
        {
            transformTarget = null;
            AttachToNewTrackedObject();
        }

        protected virtual void AttachToNewTrackedObject()
        {
            switch (TrackedObjectToReference)
            {
                case TrackedObjectType.Head:
                    TrackTransform(CameraCache.Main.transform);
                    break;
                case TrackedObjectType.LeftController:
                    var leftController = GetVisualizerForHand(Handedness.Left);
                    if (leftController != null)
                    {
                        TrackTransform(leftController.gameObject.transform);
                    }
                    break;
                case TrackedObjectType.RightController:
                    var rightController = GetVisualizerForHand(Handedness.Right);
                    if (rightController != null)
                    {
                        TrackTransform(rightController.gameObject.transform);
                    }
                    break;

                    // Other cases will come online as ControllerFinder is ported appropriately.
            }
        }

        private MixedRealityControllerVisualizer GetVisualizerForHand(Handedness hand)
        {
            for (var i = 0; i < MixedRealityManager.Instance.MixedRealitySceneObjects.Count; i++)
            {
                var controller = MixedRealityManager.Instance.MixedRealitySceneObjects[i] as MixedRealityControllerVisualizer;

                if (controller != null && controller.Handedness == hand)
                {
                    return controller;
                }
            }

            return null;
        }

        private void TrackTransform(Transform newTrackedTransform)
        {
            TransformTarget = RequiresOffset ? MakeOffsetTransform(newTrackedTransform) : newTrackedTransform;
        }

        private Transform MakeOffsetTransform(Transform parentTransform)
        {
            if (transformWithOffset == null)
            {
                transformWithOffset = new GameObject();
                transformWithOffset.transform.parent = parentTransform;
            }

            transformWithOffset.transform.localPosition = Vector3.Scale(AdditionalOffset, transformWithOffset.transform.localScale);
            transformWithOffset.transform.localRotation = Quaternion.Euler(AdditionalRotation);
            transformWithOffset.name = string.Format("{0} on {1} with offset {2}, {3}", gameObject.name, TrackedObjectToReference.ToString(), AdditionalOffset, AdditionalRotation);
            return transformWithOffset.transform;
        }

        #region IMixedRealitySourceStateHandler implementation

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            if (trackedObjectToReference != TrackedObjectType.None)
            {
                AttachToNewTrackedObject();
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (transformWithOffset != null)
            {
                Destroy(transformWithOffset);
            }
            if (trackedObjectToReference != TrackedObjectType.None)
            {
                TransformTarget = null;
            }
        }

        #endregion IMixedRealitySourceStateHandler implementation
    }
}