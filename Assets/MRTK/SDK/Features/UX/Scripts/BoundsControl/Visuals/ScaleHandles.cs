// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Scale handles for <see cref="BoundsControl"/> that are used for scaling the
    /// gameobject BoundsControl is attached to with near or far interaction
    /// </summary>
    public class ScaleHandles : HandlesBase
    {
        protected override HandlesBaseConfiguration BaseConfig => config;
        private ScaleHandlesConfiguration config;
        private bool areHandlesFlattened = false;
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

        internal void Create(ref Vector3[] boundsCorners, Transform parent, bool isFlattened)
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
                var invScale = visualBounds.size.x == 0.0f ? 0.0f : config.HandleSize / visualBounds.size.x;
                VisualUtils.AddComponentsToAffordance(corner, new Bounds(visualBounds.center * invScale, visualBounds.size * invScale),
                    HandlePrefabCollider.Box, CursorContextInfo.CursorAction.Scale, config.ColliderPadding, parent, config.DrawTetherWhenManipulating);
                handles.Add(corner.transform);
            }

            VisualUtils.HandleIgnoreCollider(config.HandlesIgnoreCollider, handles);
            objectsChangedEvent.Invoke(this);
        }

        protected override void RecreateVisuals()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                // get parent of visual
                Transform visualsScaleParent = handles[i].Find("visualsScale");
                if (visualsScaleParent)
                {
                    // get old child and remove it
                    Transform obsoleteChild = visualsScaleParent.Find(visualsName);
                    if (obsoleteChild)
                    {
                        obsoleteChild.parent = null;
                        Object.Destroy(obsoleteChild.gameObject);
                    }
                    else
                    {
                        Debug.LogError("couldn't find corner visual on recreating visuals");
                    }

                    // create new visual
                    Bounds cornerBounds = CreateVisual(visualsScaleParent.gameObject, areHandlesFlattened);

                    // update handle collider bounds
                    UpdateColliderBounds(handles[i], cornerBounds.size);
                }
            }

            objectsChangedEvent.Invoke(this);
        }

        protected override void UpdateColliderBounds(Transform handle, Vector3 visualSize)
        {
            var invScale = visualSize.x == 0.0f ? 0.0f : config.HandleSize / visualSize.x;
            GetVisual(handle).transform.localScale = new Vector3(invScale, invScale, invScale);
            BoxCollider collider = handle.gameObject.GetComponent<BoxCollider>();
            Vector3 colliderSize = visualSize * invScale;
            collider.size = colliderSize;
            collider.size += BaseConfig.ColliderPadding;
            collider.center = new Vector3(collider.size.x, collider.size.y, collider.size.z) * 0.5f;
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
            areHandlesFlattened = isFlattened;
            GameObject prefabType = isFlattened ? config.HandleSlatePrefab : config.HandlePrefab;
            if (prefabType == null)
            {
                // instantiate default prefab, a cube with box collider
                cornerVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cornerVisual.transform.parent = parent.transform;
                cornerVisual.transform.localPosition = Vector3.zero;
                // deactivate collider on visuals and register for deletion - actual collider
                // of handle is attached to the handle gameobject, not the visual
                var collider = cornerVisual.GetComponent<BoxCollider>();
                collider.enabled = false;
                Object.Destroy(collider);
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

            cornerVisual.name = visualsName;

            // this is the size of the corner visuals
            var cornerbounds = VisualUtils.GetMaxBounds(cornerVisual);
            float maxDim = Mathf.Max(Mathf.Max(cornerbounds.size.x, cornerbounds.size.y), cornerbounds.size.z);
            cornerbounds.size = maxDim * Vector3.one;

            // we need to multiply by this amount to get to desired scale handle size
            var invScale = cornerbounds.size.x == 0.0f ? 0.0f : config.HandleSize / cornerbounds.size.x;
            cornerVisual.transform.localScale = new Vector3(invScale, invScale, invScale);

            VisualUtils.ApplyMaterialToAllRenderers(cornerVisual, config.HandleMaterial);

            return cornerbounds;
        }

        internal void Reset(bool areHandlesActive, FlattenModeType flattenAxis)
        {
            IsActive = areHandlesActive;
            ResetHandles();
            bool isFlattened = flattenAxis != FlattenModeType.DoNotFlatten;
            if (areHandlesFlattened != isFlattened)
            {
                areHandlesFlattened = isFlattened;
                // we have to recreate visuals in this case as flattened scale handles will use a different prefab
                RecreateVisuals();
            }
        }

        #region BoundsControlHandlerBase

        protected override Transform GetVisual(Transform handle)
        {
            Transform firstChild = handle.GetChild(0);
            if (firstChild == null)
            {
                return null;
            }

            Transform visual = firstChild.GetChild(0);
            if (visual != null && visual.name == visualsName)
            {
                return visual;
            }

            return null;
        }

        internal override bool IsVisible(Transform handle)
        {
            return IsActive;
        }

        internal override HandleType GetHandleType()
        {
            return HandleType.Scale;
        }

        #endregion BoundsControlHandlerBase

        #region IProximityScaleObjectProvider 
        public override bool IsActive
        {
            get
            {
                return config.ShowScaleHandles && base.IsActive;
            }
        }
        #endregion IProximityScaleObjectProvider
    }
}
