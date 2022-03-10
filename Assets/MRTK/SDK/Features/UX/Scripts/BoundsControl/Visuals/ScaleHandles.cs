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
        private FlattenModeType currentFlattenAxis = FlattenModeType.DoNotFlatten;

        /// <summary>
        /// Cached handle positions - we keep track of handle positions in this array
        /// in case we have to reload the handles due to configuration changes.
        /// </summary>
        protected Vector3[] HandlePositions { get; private set; }
        private const int NumHandles = 8;
        internal ScaleHandles(ScaleHandlesConfiguration configuration)
        {
            HandlePositions = new Vector3[NumHandles];

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

        internal void UpdateHandles()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                handles[i].position = HandlePositions[i];
            }
        }

        internal void CalculateHandlePositions(ref Vector3[] boundsCorners)
        {
            if (boundsCorners != null && HandlePositions != null)
            {
                for (int i = 0; i < HandlePositions.Length; ++i)
                {
                    HandlePositions[i] = boundsCorners[i];
                }
                UpdateHandles();
            }
        }

        internal void Create(ref Vector3[] boundsCorners, Transform parent, bool isFlattened)
        {
            CalculateHandlePositions(ref boundsCorners);

            // create corners
            for (int i = 0; i < boundsCorners.Length; ++i)
            {
                GameObject corner = new GameObject
                {
                    name = "corner_" + i.ToString()
                };
                corner.transform.parent = parent;
                corner.transform.localPosition = HandlePositions[i];

                Bounds visualBounds = CreateVisual(i, corner, isFlattened);
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
                Transform obsoleteChild = handles[i].Find(visualsName);
                if (obsoleteChild)
                {
                    obsoleteChild.parent = null;
                    Object.Destroy(obsoleteChild.gameObject);
                }
                else
                {
                    Debug.LogError("Couldn't find corner visual on recreating visuals");
                }

                // create new visual
                Bounds visualBounds = CreateVisual(i, handles[i].gameObject, areHandlesFlattened);

                // update handle collider bounds
                UpdateColliderBounds(handles[i], visualBounds.size);
            }

            objectsChangedEvent.Invoke(this);
        }

        protected override void UpdateColliderBounds(Transform handle, Vector3 visualSize)
        {
            float maxDim = VisualUtils.GetMaxComponent(visualSize);
            float invScale = maxDim == 0.0f ? 0.0f : config.HandleSize / maxDim;
            GetVisual(handle).transform.localScale = new Vector3(invScale, invScale, invScale);
            BoxCollider collider = handle.gameObject.GetComponent<BoxCollider>();
            Vector3 colliderSize = visualSize * invScale;
            collider.size = colliderSize;
            collider.size += BaseConfig.ColliderPadding;
            collider.center = Vector3.zero;
        }

        /// <summary>
        /// Creates the corner visual and returns the bounds of the created visual
        /// </summary>
        /// <param name="handleIndex">cornerIndex</param>
        /// <param name="parent">parent of visual</param>
        /// <param name="isFlattened">instantiate in flattened mode - slate</param>
        /// <returns>bounds of the created visual</returns>
        private Bounds CreateVisual(int handleIndex, GameObject parent, bool isFlattened)
        {
            // figure out which prefab to instantiate
            GameObject handleVisual = null;
            areHandlesFlattened = isFlattened;
            GameObject prefabType = isFlattened ? config.HandleSlatePrefab : config.HandlePrefab;
            if (prefabType == null)
            {
                // instantiate default prefab, a cube with box collider
                handleVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // deactivate collider on visuals and register for deletion - actual collider
                // of handle is attached to the handle gameobject, not the visual
                var collider = handleVisual.GetComponent<BoxCollider>();
                collider.enabled = false;
                UnityEngine.Object.Destroy(collider);
            }
            else
            {
                handleVisual = GameObject.Instantiate(prefabType, parent.transform);
            }

            // this is the size of the corner visuals
            var handleVisualBounds = VisualUtils.GetMaxBounds(handleVisual);
            float maxDim = VisualUtils.GetMaxComponent(handleVisualBounds.size);
            float invScale = maxDim == 0.0f ? 0.0f : config.HandleSize / maxDim;

            handleVisual.name = visualsName;
            handleVisual.transform.parent = parent.transform;
            handleVisual.transform.localScale = new Vector3(invScale, invScale, invScale);
            handleVisual.transform.localPosition = Vector3.zero;
            handleVisual.transform.localRotation = Quaternion.identity;

            Quaternion realignment = GetRotationRealignment(handleIndex, isFlattened);
            parent.transform.localRotation = realignment;

            if (config.HandleMaterial != null)
            {
                VisualUtils.ApplyMaterialToAllRenderers(handleVisual, config.HandleMaterial);
            }

            return handleVisualBounds;
        }

        protected Quaternion GetRotationRealignment(int handleIndex, bool isFlattened)
        {
            // Helper lambda to sign a vector.
            Vector3 signVector(Vector3 i) => new Vector3(Mathf.Sign(i.x), Mathf.Sign(i.y), Mathf.Sign(i.z));

            // The neutral handle is the handle position at which
            // the corner handle model is appropriately aligned, sans any rotation
            Vector3 neutralHandle = signVector(HandlePositions[6]);
            Vector3 handlePos = signVector(HandlePositions[handleIndex]);

            if (isFlattened)
            {
                Vector3 axis = Vector3.forward;
                switch (currentFlattenAxis)
                {
                    case FlattenModeType.FlattenAuto:
                        Debug.LogError("ScaleHandles should never receive FlattenAuto. BoundsControl should pass ActualFlattenAxis");
                        break;
                    case FlattenModeType.FlattenX:
                        axis = Vector3.right;
                        break;
                    case FlattenModeType.FlattenY:
                        axis = -Vector3.up;
                        break;
                    case FlattenModeType.FlattenZ:
                        axis = Vector3.forward;
                        break;
                }

                Vector3 neutralProjected = Vector3.ProjectOnPlane(neutralHandle, axis);
                Vector3 handleProjected = Vector3.ProjectOnPlane(handlePos, axis);

                float angleAroundAxis = Vector3.SignedAngle(neutralProjected,
                                                                handleProjected,
                                                                axis);

                return Quaternion.AngleAxis(angleAroundAxis, axis) * Quaternion.LookRotation(axis, Vector3.up);
            }
            else
            {
                // Flip the handle if it's on the underside of the bounds.
                Quaternion flip = Quaternion.Euler(0, 0, handlePos.y > 0 ? 0 : 90);

                float angleAroundVertical = Vector3.SignedAngle(new Vector3(neutralHandle.x, 0, neutralHandle.z),
                                                                new Vector3(handlePos.x, 0, handlePos.z),
                                                                Vector3.up);
                return Quaternion.Euler(0, angleAroundVertical, 0) * flip;
            }
        }

        internal void Reset(bool areHandlesActive, FlattenModeType flattenAxis)
        {
            IsActive = areHandlesActive;
            ResetHandles();
            bool isFlattened = flattenAxis != FlattenModeType.DoNotFlatten;
            currentFlattenAxis = flattenAxis;
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
            Transform visual = handle.GetChild(0);
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
