// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Scale handles for <see cref="BoundsControl"/> that are used for scaling the
    /// gameobject BoundsControl is attached to with near or far interaction
    /// </summary>
    public class ScaleHandles : HandlesBase
    {
        protected override HandlesBaseConfiguration BaseConfig => config;
        private ScaleHandlesConfiguration config;
        internal ScaleHandles(ScaleHandlesConfiguration configuration)
        {
            Debug.Assert(configuration != null, "Can't create BoundsControlScaleHandles without valid configuration");
            config = configuration;
            config.handlesChanged.AddListener(HandlesChanged);
        }

        ~ScaleHandles()
        {
            config.handlesChanged.RemoveListener(HandlesChanged);
        }

        private void HandlesChanged(HandlesBaseConfiguration.HandlesChangedEventType changedType)
        {
            switch (changedType)
            {
                case HandlesBaseConfiguration.HandlesChangedEventType.MATERIAL:
                    UpdateBaseMaterial();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.MATERIAL_GRABBED:
                    UpdateGrabbedMaterial();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.PREFAB:
                    RecreateVisuals();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.COLLIDER_SIZE:
                case HandlesBaseConfiguration.HandlesChangedEventType.COLLIDER_PADDING:
                    UpdateColliderBounds();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.VISIBILITY:
                    //TODO
                    break;
            }
        }

        private void UpdateColliderBounds()
        {
            foreach (var handle in handles)
            {
                var cornerbounds = VisualUtils.GetMaxBounds(GetVisual(handle).gameObject);
                UpdateColliderBounds(handle, cornerbounds);
            }
        }

        internal void UpdateVisibilityInInspector(HideFlags flags)
        {
            if (handles != null)
            {
                foreach (var cube in handles)
                {
                    cube.hideFlags = flags;
                }
            }
        }

        internal void UpdateHandles(ref Vector3[] boundsCorners)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                handles[i].position = boundsCorners[i];
            }
        }

        internal void Create(ref Vector3[] boundsCorners, Transform parent, bool drawManipulationTether, bool isFlattened)
        {
            // create corners
            for (int i = 0; i < boundsCorners.Length; ++i)
            {
                GameObject corner = new GameObject
                {
                    name = "corner_" + i.ToString()
                };
                corner.transform.parent = parent;
                corner.transform.localPosition = boundsCorners[i];

                GameObject visualsScale = new GameObject();
                visualsScale.name = "visualsScale";
                visualsScale.transform.parent = corner.transform;
                visualsScale.transform.localPosition = Vector3.zero;

                // Compute mirroring scale
                {
                    Vector3 p = boundsCorners[i];
                    visualsScale.transform.localScale = new Vector3(Mathf.Sign(p[0]), Mathf.Sign(p[1]), Mathf.Sign(p[2]));
                }

                Bounds visualBounds = CreateVisual(visualsScale, isFlattened);

                var invScale = config.HandleSize / visualBounds.size.x;
                VisualUtils.AddComponentsToAffordance(corner, new Bounds(visualBounds.center * invScale, visualBounds.size * invScale), 
                    HandlePrefabCollider.Box, CursorContextInfo.CursorAction.Scale, config.ColliderPadding, parent, drawManipulationTether);
                handles.Add(corner.transform);       
            }
        }

        private void RecreateVisuals()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                // get parent of visual
                Transform visualsScaleParent = handles[i].Find("visualsScale");
                if (visualsScaleParent)
                {
                    // get old child and remove it
                    Transform obsoleteChild = visualsScaleParent.Find("visuals");
                    if (obsoleteChild)
                    {
                        obsoleteChild.parent = null;
                        Object.Destroy(obsoleteChild);
                    }
                    else
                    {
                        Debug.LogError("couldn't find corner visual on recreating visuals");
                    }

                    // create new visual
                    Bounds cornerBounds = CreateVisual(visualsScaleParent.gameObject, true);

                    // update handle collider bounds
                    UpdateColliderBounds(handles[i], cornerBounds);
                }
            }
        }

        private void UpdateColliderBounds(Transform handle, Bounds cornerBounds)
        {
            var invScale = config.HandleSize / cornerBounds.size.x;
            BoxCollider collider = handle.gameObject.GetComponent<BoxCollider>();
            Vector3 colliderSize = cornerBounds.size * invScale;
            collider.size = colliderSize;
            collider.size += BaseConfig.ColliderPadding;
        }

        /// <summary>
        /// Creates the corner visual and returns the bounds of the created visual
        /// </summary>
        /// <param name="parent">parent of visual</param>
        /// <param name="isFlattened">instantiate in flattened mode - slate</param>
        /// <returns>bounds of the created visual</returns>
        private Bounds CreateVisual(GameObject parent, bool isFlattened)
        {
            // figure out which prefab to instantiate
            GameObject cornerVisual = null;
            GameObject prefabType = isFlattened ? config.HandleSlatePrefab : config.HandlePrefab;
            if (prefabType == null)
            {
                // instantiate default prefab, a cube. Remove the box collider from it
                cornerVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cornerVisual.transform.parent = parent.transform;
                cornerVisual.transform.localPosition = Vector3.zero;
                GameObject.Destroy(cornerVisual.GetComponent<BoxCollider>());
            }
            else
            {
                cornerVisual = GameObject.Instantiate(prefabType, parent.transform);
            }

            if (isFlattened)
            {
                // Rotate 2D slate handle asset for proper orientation
                cornerVisual.transform.Rotate(0, 0, -90);
            }

            cornerVisual.name = "visuals";

            // this is the size of the corner visuals
            var cornerbounds = VisualUtils.GetMaxBounds(cornerVisual);
            float maxDim = Mathf.Max(Mathf.Max(cornerbounds.size.x, cornerbounds.size.y), cornerbounds.size.z);
            cornerbounds.size = maxDim * Vector3.one;

            // we need to multiply by this amount to get to desired scale handle size
            var invScale = config.HandleSize / cornerbounds.size.x;
            cornerVisual.transform.localScale = new Vector3(invScale, invScale, invScale);

            VisualUtils.ApplyMaterialToAllRenderers(cornerVisual, config.HandleMaterial);

            return cornerbounds;
        }

        #region BoundsControlHandlerBase
        protected override Transform GetVisual(Transform handle)
        {
            Transform visual = handle.GetChild(0)?.GetChild(0);
            if (visual != null && visual.name == "visuals")
            {
                return visual;
            }

            return null;
        }

        internal override bool IsVisible(Transform handle)
        {
            return config.ShowScaleHandles;
        }

        internal override HandleType GetHandleType()
        {
            return HandleType.Scale;
        }
        #endregion BoundsControlHandlerBase

        #region IProximityScaleObjectProvider 
        public override bool IsActive()
        {
            return config.ShowScaleHandles;
        }
        #endregion IProximityScaleObjectProvider
    }
}
