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
        protected override void Awake()
        {
            base.Awake();

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
                var result = new RaycastResult
                {
                    // We need an arbitrary GameObject to feed it; null will prevent
                    // the event from being processed by the system, strangely enough.
                    gameObject = gameObject, 
                    module = this,
                    distance = ray.direction.magnitude, // arbitrary
                    worldPosition = ray.origin + ray.direction, // arbitrary distance from camera
                    worldNormal = -ray.direction, // arbitrary; reasonable enough
                    screenPosition = eventData.position,
                    displayIndex = displayIndex,
                    index = resultAppendList.Count,
                    sortingLayer = 0,
                    sortingOrder = 0
                };
                resultAppendList.Add(result);
            }
        }
    }
}