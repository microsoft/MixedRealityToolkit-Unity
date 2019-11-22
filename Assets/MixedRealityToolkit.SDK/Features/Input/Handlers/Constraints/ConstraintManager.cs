// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

        public ConstraintManager(GameObject gameObject)
        {
            constraints = gameObject.GetComponents<TransformConstraint>().ToList();
        }

        public void ApplyScaleConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ManipulationHandFlags handMode = isOneHanded ? ManipulationHandFlags.OneHanded : ManipulationHandFlags.TwoHanded;
            ManipulationProximityFlags proximityMode = isNear ? ManipulationProximityFlags.Near : ManipulationProximityFlags.Far;
            
            foreach (var constraint in constraints)
            {
                if (constraint.ConstraintType == Utilities.TransformFlags.Scale &&
                    constraint.HandType.HasFlag(handMode) &&
                    constraint.ProximityType.HasFlag(proximityMode))
                {
                    constraint.ApplyConstraint(ref transform);
                }
            }
        }

        public void ApplyRotationConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ManipulationHandFlags handMode = isOneHanded ? ManipulationHandFlags.OneHanded : ManipulationHandFlags.TwoHanded;
            ManipulationProximityFlags proximityMode = isNear ? ManipulationProximityFlags.Near : ManipulationProximityFlags.Far;
            
            foreach (var constraint in constraints)
            {
                if (constraint.ConstraintType == Utilities.TransformFlags.Rotate &&
                    constraint.HandType.HasFlag(handMode) &&
                    constraint.ProximityType.HasFlag(proximityMode))
                {
                    constraint.ApplyConstraint(ref transform);
                }
            }
        }

        public void ApplyTranslationConstraints(ref MixedRealityTransform transform, bool isOneHanded, bool isNear)
        {
            ManipulationHandFlags handMode = isOneHanded ? ManipulationHandFlags.OneHanded : ManipulationHandFlags.TwoHanded;
            ManipulationProximityFlags proximityMode = isNear ? ManipulationProximityFlags.Near : ManipulationProximityFlags.Far;
            
            foreach (var constraint in constraints)
            {
                if (constraint.ConstraintType == Utilities.TransformFlags.Move &&
                    constraint.HandType.HasFlag(handMode) &&
                    constraint.ProximityType.HasFlag(proximityMode))
                {
                    constraint.ApplyConstraint(ref transform);
                }
            }
        }

        public void Initialize(MixedRealityPose worldPose)
        {
            foreach (var constraint in constraints)
            {
                constraint.Initialize(worldPose);
            }
        }
    }
}