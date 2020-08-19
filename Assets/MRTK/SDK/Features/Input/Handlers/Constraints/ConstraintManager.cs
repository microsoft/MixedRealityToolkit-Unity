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
    internal class ConstraintManager : MonoBehaviour
    {
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
        internal void RegisterConstraint(TransformConstraint constraint)
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
        internal void UnregisterConstraint(TransformConstraint constraint)
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
    }
}