// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.UX.BoundingBoxes
{
    /// <summary>
    /// Base class for bounding box objects
    /// </summary>
    public class BoundingBox : MonoBehaviour
    {
        public enum BoundsCalculationMethodEnum
        {
            MeshFilterBounds,   // Better for flattened objects - this mode also treats RectTransforms as quad meshes
            RendererBounds,     // Better for objects with non-mesh renderers
            Colliders,          // Better if you want precise control
            Default,            // Use the default method (RendererBounds)
        }

        public enum FlattenModeEnum
        {
            DoNotFlatten,   // Always use XYZ axis
            FlattenX,       // Flatten the X axis
            FlattenY,       // Flatten the Y axis
            FlattenZ,       // Flatten the Z axis
            FlattenAuto,    // Flatten the smallest relative axis if it falls below threshold
        }

        #region public

        public Action OnFlattenedAxisChange;

        public virtual FlattenModeEnum FlattenPreference {
            get { return flattenPreference; }
            set { flattenPreference = value; }
        }

        public virtual BoundsCalculationMethodEnum BoundsCalculationMethod {
            get { return boundsCalculationMethod; }
            set { boundsCalculationMethod = value; }
        }

        /// <summary>
        /// The target object being manipulated
        /// </summary>
        public virtual GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value)
                {
                    // Send a message to the new / old targets
                    // TODO send OnTargetSelected / Deselected events
                    target = value;
                }

                if (!isActiveAndEnabled) {
                    return;
                }

                if (target != null)
                {
                    CreateTransforms();
                    // Set our transforms to the target immediately
                    RefreshTargetBounds();
                }
            }
        }

        /// <summary>
        /// The world-space center of the target object's bounds
        /// </summary>
        public Vector3 TargetBoundsCenter
        {
            get
            {
                return targetBoundsWorldCenter;
            }
        }

        /// <summary>
        /// The world scale of the target object's bounds
        /// Use this if you want to get the size of the target object
        /// </summary>
        public Vector3 TargetBoundsScale
        {
            get
            {
                return scaleTransform.localScale;
            }
        }

        /// <summary>
        /// The local scale of the target object's bounds
        /// </summary>
        public Vector3 TargetBoundsLocalScale
        {
            get
            {
                return targetBoundsLocalScale;
            }
        }

        /// <summary>
        /// Sets the bounding box invisible while not interrupting
        /// the computation of bounds points.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                if (rendererForVisibility == null)
                {
                    Transform scale = transform.GetChild(0);
                    Transform rig = scale.GetChild(0);
                    GameObject rigobject = rig.gameObject;
                    rendererForVisibility = rigobject.gameObject.GetComponent<Renderer>();
                }

                rendererForVisibility.enabled = value;
                isVisible = value;
            }
        }

        /// <summary>
        /// The current flattened axis, if any
        /// </summary>
        public virtual FlattenModeEnum FlattenedAxis
        {
            get
            {
                return flattenedAxis;
            }
            protected set
            {
                if (flattenedAxis != value)
                {
                    flattenedAxis = value;
                    if (OnFlattenedAxisChange != null) {
                        OnFlattenedAxisChange();
                    }
                }
            }
        }

        #endregion

        #region protected

        [Header("Objects")]
        [Tooltip("The target object")]
        [SerializeField]
        protected GameObject target;

        [Tooltip("The transform used to scale the bounding box (will be auto-generated)")]
        [SerializeField]
        protected Transform scaleTransform = null;

        [Header("Flattening & Padding")]
        [Tooltip("Flattening behavior setting.")]
        [SerializeField]
        protected FlattenModeEnum flattenPreference = FlattenModeEnum.FlattenAuto;

        [Tooltip("The relative % size of an axis must meet before being auto-flattened")]
        [SerializeField]
        protected float flattenAxisThreshold = 0.025f;

        [Tooltip("The relative % size of a flattened axis")]
        [SerializeField]
        protected float flattenedAxisThickness = 0.01f;

        [Tooltip("How much to pad the scale of the box to fit around objects (as % of largest dimension)")]
        [SerializeField]
        protected float scalePadding = 0.05f;

        [Tooltip("How much to pad the scale of the box on an axis that's flattened")]
        [SerializeField]
        protected float flattenedScalePadding = 0f;

        [Header("Bounds Calculation")]
        [Tooltip("Method used to calculate the bounds of the object.")]
        [SerializeField]
        protected BoundsCalculationMethodEnum boundsCalculationMethod = BoundsCalculationMethodEnum.MeshFilterBounds;

        [Tooltip("Any renderers on this layer will be ignored when calculating object bounds")]
        [SerializeField]
        protected LayerMask ignoreLayers = (1 << 2); // Ignore Raycast Layer

        protected Vector3 targetBoundsWorldCenter = Vector3.zero;

        protected Vector3 targetBoundsLocalScale = Vector3.zero;

        protected Bounds localTargetBounds = new Bounds();

        protected List<Vector3> boundsPoints = new List<Vector3>();

        protected FlattenModeEnum flattenedAxis = FlattenModeEnum.DoNotFlatten;
   
        protected bool isVisible = true;

        protected Renderer rendererForVisibility;
        #endregion

        #region
        /// <summary>
        /// Override so we're not overwhelmed by button gizmos
        /// </summary>
#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            // nothing
            if (!Application.isPlaying)
            {
                // Do this here to ensure continuous updates in editor
                CreateTransforms();
                RefreshTargetBounds();
                UpdateScaleTransform();
            }

            if (target != null)
            {
                foreach (Vector3 point in boundsPoints)
                {
                    Gizmos.DrawSphere(target.transform.TransformPoint(point), 0.01f);
                }
            }
        }
#endif

        protected virtual void Update()
        {
            CreateTransforms();
            RefreshTargetBounds();
            UpdateScaleTransform();
        }

        protected virtual void CreateTransforms()
        {
            if (scaleTransform == null) {
                scaleTransform = transform.Find("Scale");
            }

            if (scaleTransform == null) {
                scaleTransform = new GameObject("Scale").transform;
            }

            scaleTransform.parent = transform;
        }

        protected virtual void RefreshTargetBounds()
        {
            if (target == null)
            {
                targetBoundsWorldCenter = Vector3.zero;
                targetBoundsLocalScale = Vector3.one;
                return;
            }

            // Get the new target bounds
            boundsPoints.Clear();

            switch (boundsCalculationMethod)
            {
                case BoundsCalculationMethodEnum.RendererBounds:
                default:
                    GetRenderBoundsPoints(target, boundsPoints, ignoreLayers);
                    break;

                case BoundsCalculationMethodEnum.Colliders:
                    GetColliderBoundsPoints(target, boundsPoints, ignoreLayers);
                    break;

                case BoundsCalculationMethodEnum.MeshFilterBounds:
                    GetMeshFilterBoundsPoints(target, boundsPoints, ignoreLayers);
                    break;
            }

            if (boundsPoints.Count > 0)
            {
                // We now have a list of all points in world space
                // Translate them all to local space
                for (int i = 0; i < boundsPoints.Count; i++)
                {
                    boundsPoints[i] = target.transform.InverseTransformPoint(boundsPoints[i]);
                }

                // Encapsulate the points with a local bounds
                localTargetBounds.center = boundsPoints[0];
                localTargetBounds.size = Vector3.zero;
                foreach (Vector3 point in boundsPoints)
                {
                    localTargetBounds.Encapsulate(point);
                }
            }

            // Store the world center of the target bb
            targetBoundsWorldCenter = target.transform.TransformPoint(localTargetBounds.center);

            // Store the local scale of the target bb
            targetBoundsLocalScale = localTargetBounds.size;
            targetBoundsLocalScale.Scale(target.transform.localScale);

            // Now check our flatten behavior
            UpdateFlattenedAxis();
        }

        protected virtual void UpdateFlattenedAxis() {

            // Find the maximum size of the new bounds
            float maxAxisThickness = Mathf.Max(Mathf.Max(targetBoundsLocalScale.x, targetBoundsLocalScale.y), targetBoundsLocalScale.z);

            FlattenModeEnum newFlattenedAxis = FlattenModeEnum.DoNotFlatten;
            switch (flattenPreference) {
                case FlattenModeEnum.DoNotFlatten:
                    // Do nothing
                    break;

                case FlattenModeEnum.FlattenAuto:
                    // Flattening order of preference - z, y, x
                    if (Mathf.Abs(targetBoundsLocalScale.z / maxAxisThickness) < flattenAxisThreshold) {
                        newFlattenedAxis = FlattenModeEnum.FlattenZ;
                        targetBoundsLocalScale.z = flattenedAxisThickness * maxAxisThickness;
                    } else if (Mathf.Abs(targetBoundsLocalScale.y / maxAxisThickness) < flattenAxisThreshold) {
                        newFlattenedAxis = FlattenModeEnum.FlattenY;
                        targetBoundsLocalScale.y = flattenedAxisThickness * maxAxisThickness;
                    } else if (Mathf.Abs(targetBoundsLocalScale.x / maxAxisThickness) < flattenAxisThreshold) {
                        newFlattenedAxis = FlattenModeEnum.FlattenX;
                        targetBoundsLocalScale.x = flattenedAxisThickness * maxAxisThickness;
                    }
                    break;

                case FlattenModeEnum.FlattenX:
                    newFlattenedAxis = FlattenModeEnum.FlattenX;
                    targetBoundsLocalScale.x = flattenedAxisThickness * maxAxisThickness;
                    break;

                case FlattenModeEnum.FlattenY:
                    newFlattenedAxis = FlattenModeEnum.FlattenY;
                    targetBoundsLocalScale.y = flattenedAxisThickness * maxAxisThickness;
                    break;

                case FlattenModeEnum.FlattenZ:
                    newFlattenedAxis = FlattenModeEnum.FlattenZ;
                    targetBoundsLocalScale.z = flattenedAxisThickness * maxAxisThickness;
                    break;
            }

            FlattenedAxis = newFlattenedAxis;
        }

        /// <summary>
        /// Updates bounding box to match target position etc
        /// </summary>
        protected virtual void UpdateScaleTransform()
        {
            // If we don't have a target, nothing to do here
            if (target == null) {
                return;
            }
            // Get position of object based on renderers
            transform.position = targetBoundsWorldCenter;
            Vector3 scale = targetBoundsLocalScale;

            // Use absolute value when determining smallest axis
            // so we don't get fooled by inverted scales
            float largestDimension = Mathf.Max(Mathf.Max(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.y)),
                Mathf.Abs(scale.z));

            switch (flattenedAxis)
            {
                case BoundingBox.FlattenModeEnum.DoNotFlatten:
                default:
                    scale.x += (largestDimension * scalePadding);
                    scale.y += (largestDimension * scalePadding);
                    scale.z += (largestDimension * scalePadding);
                    break;

                case BoundingBox.FlattenModeEnum.FlattenX:
                    scale.x += (largestDimension * flattenedScalePadding);
                    scale.y += (largestDimension * scalePadding);
                    scale.z += (largestDimension * scalePadding);
                    break;

                case BoundingBox.FlattenModeEnum.FlattenY:
                    scale.x += (largestDimension * scalePadding);
                    scale.y += (largestDimension * flattenedScalePadding);
                    scale.z += (largestDimension * scalePadding);
                    break;

                case BoundingBox.FlattenModeEnum.FlattenZ:
                    scale.x += (largestDimension * scalePadding);
                    scale.y += (largestDimension * scalePadding);
                    scale.z += (largestDimension * flattenedScalePadding);
                    break;
            }
            scaleTransform.localScale = scale;
            Vector3 rotation = target.transform.eulerAngles;
            transform.eulerAngles = rotation;
        }
        #endregion

        #region static utility functions

        public static void GetColliderBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (ignoreLayers == (1 << colliders[i].gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                switch (colliders[i].GetType().Name)
                {
                    case "SphereCollider":
                        SphereCollider sc = colliders[i] as SphereCollider;
                        Bounds sphereBounds = new Bounds(sc.center, Vector3.one * sc.radius * 2);
                        sphereBounds.GetFacePositions(sc.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;

                    case "BoxCollider":
                        BoxCollider bc = colliders[i] as BoxCollider;
                        Bounds boxBounds = new Bounds(bc.center, bc.size);
                        boxBounds.GetCornerPositions(bc.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;

                    case "MeshCollider":
                        MeshCollider mc = colliders[i] as MeshCollider;
                        Bounds meshBounds = mc.sharedMesh.bounds;
                        meshBounds.GetCornerPositions(mc.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;

                    case "CapsuleCollider":
                        CapsuleCollider cc = colliders[i] as CapsuleCollider;
                        Bounds capsuleBounds = new Bounds(cc.center, Vector3.zero);
                        switch (cc.direction)
                        {
                            case 0:
                                capsuleBounds.size = new Vector3(cc.height, cc.radius * 2, cc.radius * 2);
                                break;

                            case 1:
                                capsuleBounds.size = new Vector3(cc.radius * 2, cc.height, cc.radius * 2);
                                break;

                            case 2:
                                capsuleBounds.size = new Vector3(cc.radius * 2, cc.radius * 2, cc.height);
                                break;
                        }
                        capsuleBounds.GetFacePositions(cc.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;

                    default:
                        break;
                }
            }
        }

        public static void GetRenderBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                var rendererObj = renderers[i];
                if (ignoreLayers == (1 << rendererObj.gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                rendererObj.bounds.GetCornerPositionsFromRendererBounds(ref corners);
                boundsPoints.AddRange(corners);
            }
        }

        public static void GetMeshFilterBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                var meshFilterObj = meshFilters[i];
                if (ignoreLayers == (1 << meshFilterObj.gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                Bounds meshBounds = meshFilterObj.sharedMesh.bounds;
                meshBounds.GetCornerPositions(meshFilterObj.transform, ref corners);
                boundsPoints.AddRange(corners);
            }
            RectTransform[] rectTransforms = target.GetComponentsInChildren<RectTransform>();
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                rectTransforms[i].GetWorldCorners(rectTransformCorners);
                boundsPoints.AddRange(rectTransformCorners);
            }
        }

        private static Vector3[] corners = null;
        private static Vector3[] rectTransformCorners = new Vector3[4];

        #endregion

    }
}