// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Manages constraints for a given object and ensures that Scale/Rotation/Translation 
    /// constraints are executed separately.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_ConstraintManager.html")]
    public class ConstraintManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Per default constraint manager will apply all to this gameobject attached constraint components." +
            "If this flag is enabled only the selected constraint list will be applied.")]
        private bool autoConstraintSelection = true;
        /// <summary>
        /// Per default constraint manager will apply all to this gameobject attached constraint components automatically.
        /// If this flag is enabled only the selected constraint list will be applied.
        /// </summary>
        public bool AutoConstraintSelection
        {
            get => autoConstraintSelection;
            set => autoConstraintSelection = value;
        }

        [SerializeField]
        [Tooltip("Manually selected list of transform constraints. Note that this list will only be processed by the" +
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

        private HashSet<TransformConstraint> constraints = new HashSet<TransformConstraint>();
        private MixedRealityTransform initialWorldPose;

        public void ApplyScaleConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ApplyConstraintsForType(ref transform, isOneHanded, isNear, TransformFlags.Scale);
        }

        public void ApplyRotationConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ApplyConstraintsForType(ref transform, isOneHanded, isNear, TransformFlags.Rotate);
        }

        public void ApplyTranslationConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ApplyConstraintsForType(ref transform, isOneHanded, isNear, TransformFlags.Move);
        }

        public void Initialize(MixedRealityTransform worldPose)
        {
            initialWorldPose = worldPose;
            foreach (var constraint in constraints)
            {
                constraint.Initialize(worldPose);
            }
        }

        /// <summary>
        /// Registering of a constraint in case a constraint gets added during runtime.
        /// If auto mode is disabled the component will be added to the manually selected
        /// constraint list as well.
        /// Constraints in either of the lists (auto or manual) are unique. The same component 
        /// can't be added twice.
        /// </summary>
        /// <param name="constraint">Constraint to add to the manager.</param>
        public void RegisterConstraint(TransformConstraint constraint)
        {
            // don't add anything that isn't attached to this gameobject
            if (gameObject != constraint.gameObject)
            {
                return;
            }

            // add to auto component list
            if (constraint.isActiveAndEnabled)
            {
                constraints.Add(constraint);
                constraint.Initialize(initialWorldPose);
            }

            // if we are in manual mode add to manual list as well
            if (AutoConstraintSelection == false)
            {
                // check if unique here (in case of manual list we have to operate on a list (serialization)
                // which doesn't do unique checks automatically for us)
                var existingConstraint = selectedConstraints.Find(t => t == constraint);
                if (existingConstraint == null)
                {
                    selectedConstraints.Add(constraint);
                }
            }
        }

        /// <summary>
        /// Unregister a constraint from the manager.
        /// Removes the constraint from the manual list if auto mode is disabled.
        /// </summary>
        /// <param name="constraint">Constraint to remove from the manager.</param>
        public void UnregisterConstraint(TransformConstraint constraint)
        {
            constraints.Remove(constraint);
            if (AutoConstraintSelection == false)
            {
                selectedConstraints.Remove(constraint);
            }
        }

        protected void Awake()
        {
            var constraintComponents = gameObject.GetComponents<TransformConstraint>();
            foreach (var constraint in constraintComponents)
            {
                if (constraint.isActiveAndEnabled)
                {
                    constraints.Add(constraint);
                }
            }
        }

        private void ApplyConstraintsForType(ref MixedRealityTransform transform, bool isOneHanded, bool isNear, TransformFlags transformType)
        {
            ManipulationHandFlags handMode = isOneHanded ? ManipulationHandFlags.OneHanded : ManipulationHandFlags.TwoHanded;
            ManipulationProximityFlags proximityMode = isNear ? ManipulationProximityFlags.Near : ManipulationProximityFlags.Far;

            if (autoConstraintSelection)
            {
                foreach (var constraint in constraints)
                {
                    if (constraint.isActiveAndEnabled &&
                        constraint.ConstraintType == transformType &&
                        constraint.HandType.HasFlag(handMode) &&
                        constraint.ProximityType.HasFlag(proximityMode))
                    {
                        constraint.ApplyConstraint(ref transform);
                    }
                }
            }
            else
            {
                foreach (var constraint in selectedConstraints)
                {
                    if (constraint.isActiveAndEnabled &&
                        constraint.ConstraintType == transformType &&
                        constraint.HandType.HasFlag(handMode) &&
                        constraint.ProximityType.HasFlag(proximityMode))
                    {
                        constraint.ApplyConstraint(ref transform);
                    }
                }
            }
        }
    }
}