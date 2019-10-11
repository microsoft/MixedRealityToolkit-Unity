// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    internal class ConstraintManager
    {
        private List<TransformConstraint> constraints;

        public ConstraintManager(GameObject gameObject)
        {
            constraints = gameObject.GetComponents<TransformConstraint>().ToList();
        }

        public void ApplyScaleConstraints(ref MixedRealityPose pose, ref Vector3 scale)
        {
            foreach (var constraint in constraints)
            {
                if (constraint.ConstraintType == Utilities.TransformFlags.Scale)
                {
                    constraint.ApplyConstraint(ref pose, ref scale);
                }
            }
        }

        public void ApplyRotationConstraints(ref MixedRealityPose pose, ref Vector3 scale)
        {
            foreach (var constraint in constraints)
            {
                if (constraint.ConstraintType == Utilities.TransformFlags.Rotate)
                {
                    constraint.ApplyConstraint(ref pose, ref scale);
                }
            }
        }

        public void ApplyTranslationConstraints(ref MixedRealityPose pose, ref Vector3 scale)
        {
            foreach (var constraint in constraints)
            {
                if (constraint.ConstraintType == Utilities.TransformFlags.Move)
                {
                    constraint.ApplyConstraint(ref pose, ref scale);
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