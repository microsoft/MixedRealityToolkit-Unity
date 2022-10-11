// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The abstract interaction events that all proxy interactors support.
    /// Proxy interactors are used to map foreign interaction systems (like UnityUI)
    /// onto XRI interaction primitives.
    /// Generally, input shims will call these functions to request the proxy to
    /// hover/select/etc the object on which the shim is placed.
    /// </summary>
    public interface IProxyInteractor : IXRSelectInteractor, IXRHoverInteractor
    {
        /// <summary>
        /// Begin hovering the interactable. The interactable will receive
        /// an OnHoverEntering/Entered, and the proxy interactor will include it in
        /// its list of valid targets.
        /// </summary>
        void StartHover(IXRHoverInteractable interactable);

        /// <summary>
        /// Begin hovering the interactable. The interactable will receive
        /// an OnHoverEntering/Entered, and the proxy interactor will include it in
        /// its list of valid targets. Also includes the worldPosition of the pointer.
        /// </summary>
        void StartHover(IXRHoverInteractable interactable, Vector3 worldPosition);

        /// <summary>
        /// End hovering the interactable. The interactable will receive
        /// an OnHoverExiting/Exited, and the proxy interactor will remove it from
        /// its list of valid targets.
        /// </summary>
        void EndHover(IXRHoverInteractable interactable);

        /// <summary>
        /// Begin selecting the interactable. The interactable will receive
        /// an OnSelectEntering/Entered.
        /// </summary>
        void StartSelect(IXRSelectInteractable interactable);

        // <summary>
        /// Begin selecting the interactable. The interactable will receive
        /// an OnSelectEntering/Entered. Also includes the worldPosition of the pointer.
        /// </summary>
        void StartSelect(IXRSelectInteractable interactable, Vector3 worldPosition);

        /// <summary>
        /// Call to periodically update an in-progress selection. Typically
        /// used for drags; worldPosition specifies the world position of the pointer's drag.
        /// </summary>
        void UpdateSelect(IXRSelectInteractable interactable, Vector3 worldPosition);

        /// <summary>
        /// End selecting the interactable. The interactable will receive
        /// an OnSelectExiting/Exited. SuppressEvents will prevent StatefulInteractables
        /// from receiving click or toggle events.
        /// </summary>
        void EndSelect(IXRSelectInteractable interactable, bool suppressEvents = false);
    }
}
