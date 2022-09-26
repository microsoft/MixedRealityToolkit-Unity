// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A wrapper around <see cref="UnityEngine.EventSystems.PhysicsRaycaster"/>, which
    /// will automatically disable itself if it detects the application is running on an
    /// XR device (i.e., a <see cref="UnityEngine.XR.XRDisplaySubsystem"/> is present and running).
    /// It also slightly modifies the underlying raycast behavior to return empty raycast hits when
    /// no other hits are detected, which improves dragging/manipulation on 3D objects.
    /// </summary>
    /// <remarks>
    /// This is useful for automatically enabling UGUI-event-based UI with mouse/touchscreen
    /// input on flat/2D platforms, while saving performance on XR devices that don't need
    /// 2D input processing.
    /// </remarks>
    internal class PlatformAwarePhysicsRaycaster : PhysicsRaycaster
    {
        [SerializeField]
        [Tooltip("For which layers should the interactor use a planar projection " +
            "instead of spherical projection?")]
        private LayerMask planarLayers;

        /// <summary>
        /// For which layers should the raycast use a planar projection
        /// instead of spherical projection when dragging off of a 3D object?
        /// </summary>
        /// <remarks>
        /// This is useful for UI elements, which are typically flat. Spherical
        /// projection (default) is more intuitive for manipulating 3D objects, but
        /// can cause issues when working with flat UI panels.
        /// </remarks>
        public LayerMask PlanarLayers { get => planarLayers; set => planarLayers = value; }

        // Last known good hit distance, measured from the ray origin to the hit point.
        // (note: not the same as from the camera position to the hit point!)
        private float lastDistance = 1.0f;

        private GameObject lastGameObject = null;

        // Are we using a planar projection for the current hit?
        private bool isPlanar = false;

        // The plane used for planar projection.
        private Plane plane;

        protected override void Awake()
        {
            base.Awake();

            if (planarLayers == 0)
            {
                // If no mask is specified, default to UI.
                planarLayers = LayerMask.GetMask("UI");
            }

            // Are we on an XR device? If so, we don't want to
            // use camera raycasting at all.
            if (XRDisplaySubsystemHelpers.AreAnyActive())
            {
                enabled = false;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// We inject a "blank" raycast hit into the result list when the base class's raycast
        /// doesn't hit anything. This allows us to keep firing OnDrag events on Selectables, even
        /// when the object is smoothed/lags behind the cursor's position.
        /// </remarks>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            base.Raycast(eventData, resultAppendList);

            Ray ray = new Ray();
            int displayIndex = 0;
            float distanceToClipPlane = 0;

            // We have to call this again, unfortunately, instead of reusing the base impl's
            // ray result. It doesn't actually perform any raycasting, but it computes the ray's
            // direction (along with performing some interesting multi-display validation/logic)
            if (!ComputeRayAndDistance(eventData, ref ray, ref displayIndex, ref distanceToClipPlane))
            {
                return;
            }

            // If the base class's Raycast implementation didn't hit anything,
            // we fill the raycast list with an empty raycast hit. The event system
            // will continue firing OnDrags on the currently-selected Selectable.
            if (resultAppendList.Count == 0)
            {
                // Debug.Log("rayOrigin = " + ray.origin.ToString("F5") + ", direction = " + ray.direction);
                // Debug.Log("Near plane offset: " + (ray.origin - Camera.main.transform.position).magnitude.ToString("F3"));
                // Debug.Log("ray magnitude: " + ray.direction.magnitude.ToString("F3"));
                // Debug.Log("distanceToClipPlane: " + distanceToClipPlane.ToString("F3"));

                float newDistance;
                Vector3 newPoint;
                Vector3 newNormal;

                if (!plane.Raycast(ray, out newDistance))
                {
                    // Planar projection; intersects the new ray with the
                    // plane formed by the orginal hit point and the hit normal.
                    newPoint = ray.origin + ray.direction * newDistance;
                    newNormal = plane.normal;
                }
                else
                {
                    Debug.Log("no intersection, plane normal = " + plane.normal.ToString("F3") + ", closest point = " + plane.ClosestPointOnPlane(ray.origin).ToString("F3"));
                    Debug.Log("ray origin = " + ray.origin.ToString("F3") + ", direction = " + ray.direction.ToString("F3"));
                    // Spherical projection. Equidistant from ray origin,
                    // same distance as the last known good hit.
                    newDistance = lastDistance;
                    newPoint = ray.origin + ray.direction * newDistance;
                    newNormal = -ray.direction;
                }

                var result = new RaycastResult
                {
                    // We need an arbitrary GameObject to feed it; null will prevent
                    // the event from being processed by the system, strangely enough.
                    gameObject = gameObject, 
                    module = this,
                    distance = newDistance,
                    worldPosition = ray.origin + ray.direction * lastDistance, // arbitrary distance from camera
                    worldNormal = -ray.direction, // arbitrary; reasonable enough
                    screenPosition = eventData.position,
                    displayIndex = displayIndex,
                    index = resultAppendList.Count,
                    sortingLayer = 0,
                    sortingOrder = 0
                };
                resultAppendList.Add(result);
            }
            else
            {
                // Compute distance from the hit to the start of the ray.
                // This will be used to construct to
                lastDistance = (eventData.pointerCurrentRaycast.worldPosition - ray.origin).magnitude;
                lastGameObject = eventData.pointerCurrentRaycast.gameObject;

                // Should we use a planar projection or a spherical projection?
                // if (lastGameObject != null)
                // {
                //     // isPlanar = ((1 << lastGameObject.layer) & planarLayers) != 0;
                    isPlanar = true;
                    plane = new Plane(-ray.direction.normalized, eventData.pointerCurrentRaycast.worldPosition);
                // }
                // else
                // {
                //     isPlanar = true;
                // }
                
            }
        }
    }
}