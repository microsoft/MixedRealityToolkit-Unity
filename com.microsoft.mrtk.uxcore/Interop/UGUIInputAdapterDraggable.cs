// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// An extension of the <see cref="UGUIInputAdapter"/> that allows the interception of drag events.
    /// Add to any XRI interactable you wish to receive drags, such as sliders, handles, or even ObjectManipulators.
    /// </summary>
    [AddComponentMenu("MRTK/UX/UGUI Input Adapter Draggable")]
    public class UGUIInputAdapterDraggable : UGUIInputAdapter, IDragHandler, IInitializePotentialDragHandler
    {
        // Cache a reference to the last known good event camera.
        // We do this because rolled-off drags don't report the event camera (??),
        // and we need to reconstruct off-target drag rays in order to project them
        // back into 3D space.
        private Camera eventCamera;

        // Plane along which drags will be projected into worldspace.
        // Normal to the camera's forward vector, and offset by the initial hit point.
        private Plane plane;

        // Map Drag to moving the interactor's attachTransform (or whatever else it decides to do in UpdateSelect)
        public virtual void OnDrag(PointerEventData pointerEventData)
        {
            // Only left-click-drags are XRI drags.
            if (pointerEventData.button != PointerEventData.InputButton.Left) { return; }

            // Reject all incoming UnityUI input if it originates from
            // an XRI interactor. The Interactable itself will handle those inputs,
            // and we don't want to duplicate them.
            if (IsXRUIEvent(pointerEventData)) { return; }

            // We only adapt drags for selectable interactables,
            // and if we have a valid proxy interactor.
            if (!(ThisInteractable is IXRSelectInteractable selectInteractable) ||
                ProxyInteractor == null)
            { return; }

            // If we have no event camera at all, abort!
            if (eventCamera == null) { return; }

            // Reconstruct the ray that would have generated this drag event.            
            Ray ray = eventCamera.ScreenPointToRay(pointerEventData.position);
            if (plane.Raycast(ray, out float distance)) 
            {
                ProxyInteractor.UpdateSelect(selectInteractable, ray.origin + ray.direction * distance);
            }
        }

        // Construct the plane for planar drag projection during
        // OnPointerDown. Future OnDrag events may not pass event cameras
        // or valid hit information.
        public override void OnPointerDown(PointerEventData pointerEventData)
        {
            base.OnPointerDown(pointerEventData);

            if (pointerEventData.pointerCurrentRaycast.isValid)
            {
                // Save a reference to the last known event camera. We'll use this to reconstruct
                // drag rays in the future, when the input system might not pass us a camera.
                eventCamera = pointerEventData.pressEventCamera;

                // Construct a new planar projection mapping based on the current valid hit's
                // hit point and normal. For subsequent rolled off drags, we'll project against this plane.
                Vector3 normal = eventCamera.transform.forward;
                plane = new Plane(normal, pointerEventData.pointerCurrentRaycast.worldPosition);
            }
        }

        // No drag thresholds in 3D, please :)
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }
}
