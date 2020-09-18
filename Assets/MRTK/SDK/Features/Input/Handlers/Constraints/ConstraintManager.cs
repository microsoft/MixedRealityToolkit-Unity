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
            "If this flag is enabled only the selected list of constraint components will be applied.")]
        private bool autoConstraintSelection = true;
        /// <summary>
        /// Per default constraint manager will apply all to this gameobject attached constraint components automatically.
        /// If this flag is disabled only the selected list of constraint components will be applied.
        /// </summary>
        public bool AutoConstraintSelection
        {
            get => autoConstraintSelection;
            set => autoConstraintSelection = value;
        }

        [SerializeField]
        [Tooltip("Optional set of transform constraints if applied constraints should be limited / user selected.")]
        private List<TransformConstraint> selectedConstraints = new List<TransformConstraint>();
        /// <summary>
        /// Optional set of transform constraints if applied constraints should be limited / user selected.
        /// </summary>
        public List<TransformConstraint> SelectedConstraints
        {
            get => selectedConstraints;
        }

        /// <summary>
        /// Adds a constraint
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public bool AddConstraint(TransformConstraint constraint)
        {
            var existingConstraint = selectedConstraints.Find(t => t == constraint);
            if (existingConstraint == null)
            {
                selectedConstraints.Add(constraint);
            }

            return existingConstraint == null;
        }

        /// <summary>
        /// Returns the currently processed constraints by this manager.
        /// </summary>
        public TransformConstraint[] ProcessedConstraints
        {
            get
            {
                return autoConstraintSelection ? gameObject.GetComponents<TransformConstraint>() : selectedConstraints.ToArray();
            }
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
        /// Manual registering of a constraint in case a constraint gets added during runtime.
        /// Only active and enabled constraints can be registered to the constraint manager.
        /// </summary>
        public void RegisterConstraint(TransformConstraint constraint)
        {
            if (constraint.isActiveAndEnabled)
            {
                constraints.Add(constraint);
                constraint.Initialize(initialWorldPose);
            }
        }

        /// <summary>
        /// Unregister a constraint from the manager.
        /// </summary>
        public void UnregisterConstraint(TransformConstraint constraint)
        {
            constraints.Remove(constraint);
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