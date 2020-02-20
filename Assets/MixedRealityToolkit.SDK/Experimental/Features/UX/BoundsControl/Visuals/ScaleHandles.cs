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

                // figure out which prefab to instantiate
                GameObject cornerVisual = null;
                GameObject prefabType = isFlattened ? config.HandleSlatePrefab : config.HandlePrefab;
                if (prefabType == null)
                {
                    // instantiate default prefab, a cube. Remove the box collider from it
                    cornerVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornerVisual.transform.parent = visualsScale.transform;
                    cornerVisual.transform.localPosition = Vector3.zero;
                    GameObject.Destroy(cornerVisual.GetComponent<BoxCollider>());
                }
                else
                {
                    cornerVisual = GameObject.Instantiate(prefabType, visualsScale.transform);
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

                VisualUtils.AddComponentsToAffordance(corner, new Bounds(cornerbounds.center * invScale, cornerbounds.size * invScale), 
                    RotationHandlePrefabCollider.Box, CursorContextInfo.CursorAction.Scale, config.ColliderPadding, parent, drawManipulationTether);
                handles.Add(corner.transform);       
            }
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
