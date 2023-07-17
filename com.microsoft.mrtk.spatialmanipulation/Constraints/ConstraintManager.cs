// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Manages constraints for a given object and ensures that scale, rotation, and translation 
    /// constraints are executed separately.
    /// </summary>
    /// <remarks>
    /// The constraint system might be reworked in the future. In such a case, these existing components will be deprecated.
    /// </remarks>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/constraint-manager")]
    [AddComponentMenu("MRTK/Spatial Manipulation/Constraint Manager")]
    public class ConstraintManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Per default, constraint manager will apply all to this gameobject attached constraint components." +
            "If this flag is enabled only the selected constraint list will be applied.")]
        private bool autoConstraintSelection = true;
        /// <summary>
        /// Per default, constraint manager will apply all to this gameobject attached constraint components automatically.
        /// If this flag is enabled, only the selected constraint list will be applied.
        /// </summary>
        public bool AutoConstraintSelection
        {
            get => autoConstraintSelection;
            set => autoConstraintSelection = value;
        }

        [SerializeField]
        [Tooltip("Manually selected list of transform constraints. Note that this list will only be processed by the " +
            "manager if AutoConstraintSelection is disabled.")]
        private List<TransformConstraint> selectedConstraints = new List<TransformConstraint>();
        /// <summary>
        /// Manually selected list of transform constraints. Note that this list will only be processed by the
        /// manager if AutoConstraintSelection is disabled.
        /// Note that this is a read only property. To add new constraints to the list call RegisterConstraint.
        /// </summary>
        public List<TransformConstraint> SelectedConstraints
        {
            get => selectedConstraints;
        }

        private List<TransformConstraint> constraints = new List<TransformConstraint>();
        private MixedRealityTransform initialWorldPose;

        /// <summary>	
        /// Adds a constraint to the manual selection list.
        /// Note that only unique components will be added to the list.
        /// </summary>	
        /// <param name="constraint">Constraint to add to the managers manual constraint list.</param>
        /// <returns>Returns true if insertion was successful. If the component was already in the list the insertion will fail.</returns>	
        public bool AddConstraintToManualSelection(TransformConstraint constraint)
        {
            var existingConstraint = selectedConstraints.Find(t => t == constraint);
            if (existingConstraint == null)
            {
                ConstraintUtils.AddWithPriority(ref selectedConstraints, constraint, new ConstraintExecOrderComparer());
            }

            return existingConstraint == null;
        }

        /// <summary>
        /// Removes the given component from the manually selected constraint list.
        /// </summary>
        /// <param name="constraint">Constraint to remove.</param>
        public void RemoveConstraintFromManualSelection(TransformConstraint constraint)
        {
            selectedConstraints.Remove(constraint);
        }

        /// <summary>
        /// Apply scale constraints to the given transform.
        /// </summary>
        /// <param name="transform">The transform to be adjusted.</param>
        /// <param name="isOneHanded">This value should be <see langword="true"/> if the scale is being executed with one hand.</param>
        /// <param name="isNear">The value should be <see langword="true"/> if the scale is being executed with a near interaction.</param>
        public void ApplyScaleConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ApplyConstraintsForType(ref transform, isOneHanded, isNear, TransformFlags.Scale);
        }

        /// <summary>
        /// Apply rotation constraints to the given transform.
        /// </summary>
        /// <param name="transform">The transform to be adjusted.</param>
        /// <param name="isOneHanded">This value should be <see langword="true"/> if the rotation is being executed with one hand.</param>
        /// <param name="isNear">The value should be <see langword="true"/> if the rotation is being executed with a near interaction.</param>
        public void ApplyRotationConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ApplyConstraintsForType(ref transform, isOneHanded, isNear, TransformFlags.Rotate);
        }

        /// <summary>
        /// Apply move or translation constraints to the given transform.
        /// </summary>
        /// <param name="transform">The transform to be adjusted.</param>
        /// <param name="isOneHanded">This value should be <see langword="true"/> if the move is being executed with one hand.</param>
        /// <param name="isNear">The value should be <see langword="true"/> if the move is being executed with a near interaction.</param>
        public void ApplyTranslationConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ApplyConstraintsForType(ref transform, isOneHanded, isNear, TransformFlags.Move);
        }

        /// <summary>
        /// Call once before any manipulation occurs.
        /// </summary>
        public void Setup(MixedRealityTransform worldPose)
        {
            initialWorldPose = worldPose;
            foreach (var constraint in constraints)
            {
                constraint.Setup(worldPose);
            }
        }

        /// <summary>
        /// Call when manipulation begins.
        /// </summary>
        public void OnManipulationStarted(MixedRealityTransform worldPose)
        {
            foreach (var constraint in constraints)
            {
                constraint.OnManipulationStarted(worldPose);
            }
        }

        /// <summary>
        /// This function is obsolete.
        /// </summary>
        /// <remarks>
        ///  Use <see cref="Setup"/> instead for first-time initialization, and <see cref="OnManipulationStarted"/> for subsequent manipulation.
        /// </remarks>
        [Obsolete("Use Setup instead for first-time initialization, and OnManipulationStarted for subsequent manipulation.")]
        public void Initialize(MixedRealityTransform worldPose) { }

        /// <summary>
        /// Re-sort list of constraints. Triggered by constraints
        /// when their execution order is modified at runtime.
        /// </summary>
        internal void RefreshPriorities()
        {
            constraints.Sort(new ConstraintExecOrderComparer());
        }

        /// <summary>
        /// Registering of a constraint during runtime. This method gets called by the constraint
        /// components to auto register in their OnEnable method.
        /// </summary>
        /// <param name="constraint">Constraint to add to the manager.</param>
        internal void AutoRegisterConstraint(TransformConstraint constraint)
        {
            // add to auto component list
            if (constraint.isActiveAndEnabled)
            {
                ConstraintUtils.AddWithPriority(ref constraints, constraint, new ConstraintExecOrderComparer());
                constraint.Setup(initialWorldPose);
            }
        }

        /// <summary>
        /// Unregister a constraint from the manager.
        /// Removes the constraint from the manual list if auto mode is disabled.
        /// </summary>
        /// <param name="constraint">Constraint to remove from the manager.</param>
        internal void AutoUnregisterConstraint(TransformConstraint constraint)
        {
            constraints.Remove(constraint);
        }

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        protected void Awake()
        {
            var constraintComponents = gameObject.GetComponents<TransformConstraint>();
            foreach (var constraint in constraintComponents)
            {
                if (constraint.isActiveAndEnabled)
                {
                    ConstraintUtils.AddWithPriority(ref constraints, constraint, new ConstraintExecOrderComparer());
                    constraint.Setup(initialWorldPose);
                }
            }
        }

        private void ApplyConstraintsForType(ref MixedRealityTransform transform, bool isOneHanded, bool isNear, TransformFlags transformType)
        {
            ManipulationHandFlags handMode = isOneHanded ? ManipulationHandFlags.OneHanded : ManipulationHandFlags.TwoHanded;
            ManipulationProximityFlags proximityMode = isNear ? ManipulationProximityFlags.Near : ManipulationProximityFlags.Far;

            foreach (var constraint in constraints)
            {
                // If on manual mode, filter executed constraints by which have been manually selected
                if (!autoConstraintSelection && !selectedConstraints.Contains(constraint))
                {
                    continue;
                }

                if (constraint.isActiveAndEnabled &&
                    constraint.ConstraintType == transformType &&
                    (constraint.HandType & handMode) == handMode &&
                    (constraint.ProximityType & proximityMode) == proximityMode)
                {
                    constraint.ApplyConstraint(ref transform);
                }
            }
        }
    }
}
