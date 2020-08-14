// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Manages constraints for a given object and ensures that Scale/Rotation/Translation 
    /// constraints are executed separately.
    /// </summary>
    internal class ConstraintManager
    {
        private List<TransformConstraint> constraints;
        private MixedRealityTransform initialWorldPose;
        private GameObject host;

        public ConstraintManager(GameObject gameObject)
        {
            host = gameObject;
            constraints = gameObject.GetComponents<TransformConstraint>().ToList();
        }

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

        private void ApplyConstraintsForType(ref MixedRealityTransform transform, bool isOneHanded, bool isNear, TransformFlags transformType)
        {
            EnsureNewConstraintsInitialized();

            ManipulationHandFlags handMode = isOneHanded ? ManipulationHandFlags.OneHanded : ManipulationHandFlags.TwoHanded;
            ManipulationProximityFlags proximityMode = isNear ? ManipulationProximityFlags.Near : ManipulationProximityFlags.Far;

            foreach (var constraint in constraints)
            {
                if (constraint.ConstraintType == transformType &&
                    constraint.HandType.HasFlag(handMode) &&
                    constraint.ProximityType.HasFlag(proximityMode))
                {
                    constraint.ApplyConstraint(ref transform);
                }
            }
        }

        private void EnsureNewConstraintsInitialized()
        {
            var currentConstraints = host.GetComponents<TransformConstraint>();
            bool hasNewConstraint = false;

            foreach (var newConstraint in currentConstraints)
            {
                if (constraints.IndexOf(newConstraint) < 0)
                {
                    newConstraint.Initialize(initialWorldPose);
                    hasNewConstraint = true;
                }
            }

            if (hasNewConstraint)
            {
                constraints = currentConstraints.ToList();
            }
        }
    }
}