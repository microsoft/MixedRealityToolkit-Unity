// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Implements common logic for rotating holograms with direct interaction. Will either perform
    /// a direct rotation from a single interactor, or will form a "handlebar" between two interactors
    /// for two-handed rotations.
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    public class RotateLogic : ManipulationLogic<Quaternion>
    {
        private Vector3 startHandlebar;
        private Quaternion startInputRotation;
        private Quaternion startRotation;

        /// <inheritdoc />
        public override void Setup(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable, MixedRealityTransform currentTarget)
        {
            base.Setup(interactors, interactable, currentTarget);

            if (NumInteractors >= 2)
            {
                startHandlebar = GetHandlebarDirection(interactors, interactable);
            }

            startInputRotation = interactors[0].GetAttachTransform(interactable).rotation;
            startRotation = currentTarget.Rotation;
        }

        /// <inheritdoc />
        public override Quaternion Update(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable, MixedRealityTransform currentTarget, bool centeredAnchor)
        {
            base.Update(interactors, interactable, currentTarget, centeredAnchor);

            if (NumInteractors == 1)
            {
                return interactors[0].GetAttachTransform(interactable).rotation * Quaternion.Inverse(startInputRotation) * startRotation;
            }
            else
            {
                // TODO: This gimbal locks if you hold an object with two hands and then rotate your body a full 180 degrees.
                return Quaternion.FromToRotation(startHandlebar, GetHandlebarDirection(interactors, interactable)) * startRotation;
            }
        }

        private static Vector3 GetHandlebarDirection(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable)
        {
            Debug.Assert(interactors.Count >= 2, $"GetHandlebarDirection called with less than 2 interactors ({interactors.Count}).");
            return interactors[1].GetAttachTransform(interactable).position - interactors[0].GetAttachTransform(interactable).position;
        }
    }
}
