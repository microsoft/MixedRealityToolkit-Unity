// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using PokePath = Microsoft.MixedReality.Toolkit.IPokeInteractor.PokePath;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// A simple interactor that can live on inanimate or on objects that
    /// do not contain a <see cref="XRController"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interactor acts as a poking interactor through trigger intersections.
    /// </para>
    /// <para>
    /// The full <see cref="IPokeInteractor"/> implementation used for the user's fingers
    /// uses a much more advanced sphere cast intersection system to ensure that
    /// high speed pokes are always registered. This interactor, however, uses
    /// simple trigger intersections in the interest of simplicity and demonstration.
    /// At high speeds, and low frame rates, this may miss some intersections with
    /// interactables. For higher fidelity intersections, consider using a sphere cast
    /// system like <see cref="IPokeInteractor"/>.
    /// </para>
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Pen Interactor")]
    internal class PenInteractor : XRBaseInteractor, IPokeInteractor
    {
        #region IPokeInteractor Implementation

        /// <inheritdoc />
        public float PokeRadius => 0.001f;

        // The last and current poke points, forming a
        // continuous poking trajectory.
        private PokePath pokeTrajectory;

        /// <inheritdoc />
        public PokePath PokeTrajectory => pokeTrajectory;

        #endregion IPokeInteractor Implementation

        /// <inheritdoc />
        // Always select.
        public override bool isSelectActive => true;

        // Collection of hover targets.
        private HashSet<IXRInteractable> hoveredTargets = new HashSet<IXRInteractable>();

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(hoveredTargets);
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            // Can only select if we've hovered.
            return hoveredTargets.Contains(interactable);
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                // Update the trajectory.
                // The PokeInteractor we use for hands does advanced
                // sphere casting to ensure reliable pokes; as a demonstration,
                // this simple interactor only performs trigger intersections.
                pokeTrajectory.Start = pokeTrajectory.End;
                pokeTrajectory.End = attachTransform.position;
            }
        }

        void OnTriggerStay(Collider c)
        {
            if (interactionManager.TryGetInteractableForCollider(c, out var associatedInteractable))
            {
                hoveredTargets.Add(associatedInteractable);
            }
        }

        /// <summary>
        /// A Unity event function that is called at an framerate independent frequency, and is only called if this object is enabled.
        /// </summary>
        private void FixedUpdate()
        {
            hoveredTargets.Clear();
        }
    }
}
#pragma warning restore CS1591