// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Implements a generic movelogic that works for most/all XRI interactors,
    /// assuming a well-defined attachTransform. 
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    public class MoveLogic : ManipulationLogic<Vector3>
    {
        private Vector3 attachToObject;
        private Vector3 objectLocalAttachPoint;

        /// <inheritdoc />
        public override void Setup(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable, MixedRealityTransform currentTarget)
        {
            base.Setup(interactors, interactable, currentTarget);

            Vector3 attachCentroid = GetAttachCentroid(interactors, interactable);

            attachToObject = currentTarget.Position - attachCentroid;
            objectLocalAttachPoint = Quaternion.Inverse(currentTarget.Rotation) * (attachCentroid - currentTarget.Position);
            objectLocalAttachPoint = objectLocalAttachPoint.Div(currentTarget.Scale);
        }

        /// <inheritdoc />
        public override Vector3 Update(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable, MixedRealityTransform currentTarget, bool centeredAnchor)
        {
            base.Update(interactors, interactable, currentTarget, centeredAnchor);

            Vector3 attachCentroid = GetAttachCentroid(interactors, interactable);

            if (centeredAnchor)
            {
                return attachCentroid + attachToObject;
            }
            else
            {
                Vector3 scaledLocalAttach = Vector3.Scale(objectLocalAttachPoint, currentTarget.Scale);
                Vector3 worldAttachPoint = currentTarget.Rotation * scaledLocalAttach + currentTarget.Position;
                return currentTarget.Position + (attachCentroid - worldAttachPoint);
            }
        }

        private Vector3 GetAttachCentroid(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable)
        {
            // TODO: This uses the attachTransform ONLY, which can possibly be
            // unstable/imprecise (see GrabInteractor, etc.) Old version used to use the interactor
            // transform in the case where there was only one interactor, and the attachTransform
            // when there were 2+. The interactor should stabilize its attachTransform
            // to get a similar effect. Possibly, we should stabilize grabs on the thumb, or some
            // other technique.

            Vector3 sumPos = Vector3.zero;
            int count = 0;
            foreach (IXRSelectInteractor interactor in interactors)
            {
                sumPos += interactor.GetAttachTransform(interactable).position;
                count++;
            }

            return sumPos / Mathf.Max(1, count);
        }
    }
}