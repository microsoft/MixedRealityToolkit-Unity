// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Abstract class defining all logics that define the logic by which an object
    /// is manipulated by ObjectManipulator.
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new target value for the object.
    /// </summary>
    public abstract class ManipulationLogic<T>
    {
        protected int NumInteractors { get; private set; }

        /// <summary>
        /// Setup the manipulation logic. Called automatically by Update if the number of interactor points has changed.
        /// </summary>
        /// <param name="interactors">
        /// List of all <see cref="IXRSelectInteractor"/>s selecting this object.
        ///</param>
        /// <param name= "interactable">
        /// The <see cref="IXRSelectInteractable"/> that is being manipulated.
        /// </param>
        /// <param name= "currentTarget">
        /// The current manipulation target position/rotation/scale. This is the shared target that each ManipulationLogic modifies.
        /// The result from Update will be applied to this transform by the ObjectManipulator, in the order of Scale, Rotate, Move.
        /// </param>
        public virtual void Setup(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable, MixedRealityTransform currentTarget)
        {
            NumInteractors = interactors.Count;
        }

        /// <summary>
        /// Calculate the new manipulation result, of type T, based on input. If the number of interactor points is
        /// different than the last time Update was called, Setup will be called automatically to re-initialize the manipulation.
        /// </summary>
        /// <param name="interactors">
        /// List of all <see cref="IXRSelectInteractor"/>s selecting this object.
        ///</param>
        /// <param name= "interactable">
        /// The <see cref="IXRSelectInteractable"/> that is being manipulated.
        /// </param>
        /// <param name= "currentTarget">
        /// The current manipulation target position/rotation/scale. This is the shared target that each ManipulationLogic modifies.
        /// The result from Update will be applied to this transform by the ObjectManipulator, in the order of Scale, Rotate, Move.
        /// </param>
        /// <param name= "centeredAnchor">
        /// Should the manipulationLogic anchor the object around its center, or around the manipulation?
        /// </param>
        public virtual T Update(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable, MixedRealityTransform currentTarget, bool centeredAnchor)
        {
            Debug.Assert(interactors.Count != 0, "ManipulationLogic.Update called with zero interactors.");

            if (interactors.Count != NumInteractors)
            {
                Setup(interactors, interactable, currentTarget);
            }

            return default;
        }
    }
}
