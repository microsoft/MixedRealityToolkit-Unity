// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// An extension of the <see cref="UGUIInputAdapter"/> that allows the interception of drag events.
    /// Add to any XRI interactable you wish to recieve drags, such as sliders, handles, or even ObjectManipulators.
    /// </summary>
    public class UGUIInputAdapterDraggable : UGUIInputAdapter, IDragHandler
    {
        // Map Drag to moving the interactor's attachTransform (or whatever else it decides to do in UpdateSelect)
        public virtual void OnDrag(PointerEventData pointerEventData)
        {
            // Only left-click-drags are XRI drags.
            if (pointerEventData.button != PointerEventData.InputButton.Left) { return; }

            // Reject all incoming UnityUI input if it originates from
            // an XRI interactor. The Interactable itself will handle those inputs,
            // and we don't want to duplicate them.
            if (IsXRUIEvent(pointerEventData)) { return; }

            if (ThisInteractable is IXRSelectInteractable selectInteractable && pointerEventData.pointerCurrentRaycast.isValid)
            {
                ProxyInteractor.UpdateSelect(selectInteractable, pointerEventData.pointerCurrentRaycast.worldPosition);
            }
        }

    }
}
