//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Core.Extensions;


namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Base class for bounding box objects
    /// </summary>
    public class BoundingBox : MonoBehaviour
    {
        /// <summary>
        /// The target object. GameObject that the BoundingBox surrounds.
        /// </summary>
        [Header("Objects")]
        [Tooltip("The target object")]
        [SerializeField]
        protected GameObject target;

        /// <summary>
        /// "The transform used to scale the bounding box (will be auto-generated)
        /// </summary>
        [Tooltip("The transform used to scale the bounding box (will be auto-generated)")]
        [SerializeField]
        protected Transform scaleTransform = null;

        /// <summary>
        /// Flattening behavior setting. Which axis will be considered flat?
        /// </summary>
        [Header("Flattening & Padding")]
        [Tooltip("Flattening behavior setting.")]
        [SerializeField]
        protected FlattenModeEnum flattenPreference = FlattenModeEnum.FlattenAuto;

        /// <summary>
        /// Public property describing axis intended to be regarded as flat.
        /// </summary>
        public virtual FlattenModeEnum FlattenPreference
        {
            get
            {
                return flattenPreference;
            }
            set
            {
                flattenPreference = value;
            }
        }

        private bool manualUpdateActive = false;

        /// <summary>
        ///setting this to true reduces computation by only recomputed when requested
        /// </summary>
        public bool ManualUpdateActive
        {
            get
            {
                return manualUpdateActive;
            }

            set
            {
                manualUpdateActive = value;
            }
        }

        /// <summary>
        /// The relative % size of an axis must meet before being auto-flattened.
        /// </summary>
        [Tooltip("The relative % size of an axis must meet before being auto-flattened")]
        [SerializeField]
        protected float flattenAxisThreshold = 0.025f;

        /// <summary>
        /// The relative % size of a flattened axis
        /// </summary>
        [Tooltip("The relative % size of a flattened axis")]
        [SerializeField]
        protected float flattenedAxisThickness = 0.01f;

        /// <summary>
        /// How much to pad the scale of the box to fit around objects (as % of largest dimension)
        /// </summary>
        [Tooltip("How much to pad the scale of the box to fit around objects (as % of largest dimension)")]
        [SerializeField]
        protected float scalePadding = 0.05f;

        /// <summary>
        /// How much to pad the scale of the box on an axis that's flattened
        /// </summary>
        [Tooltip("How much to pad the scale of the box on an axis that's flattened")]
        [SerializeField]
        protected float flattenedScalePadding = 0f;

        /// <summary>
        /// Method used to calculate the bounds of the object.
        /// </summary>
        [Header("Bounds Calculation")]
        [Tooltip("Method used to calculate the bounds of the object.")]
        [SerializeField]
        protected BoundsCalculationMethodEnum boundsCalculationMethod = BoundsCalculationMethodEnum.MeshFilterBounds;

        /// <summary>
        /// Any renderers on this layer will be ignored when calculating object bounds
        /// </summary>
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

        /// <summary>
        /// Event Handler- called when the FlattenedAxis property is changed.
        /// </summary>
        public Action OnFlattenedAxisChange;

        /// <summary>
        /// instruction to boundingBox which determines which of several methods
        /// to use to calculate the bounds of the gameObject it surrounds.
        /// </summary>
        public virtual BoundsCalculationMethodEnum BoundsCalculationMethod
        {
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

                if (!isActiveAndEnabled)
                {
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
                    if (OnFlattenedAxisChange != null)
                    {
                        OnFlattenedAxisChange();
                    }
                }
            }
        }

        #region Protected Virtual Functions
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

        /// <summary>
        /// private Update function provided by MonoBehaviour
        /// </summary>
        protected virtual void Update()
        {
            CreateTransforms();

            if (manualUpdateActive == false)
            {
                RefreshTargetBounds();
                UpdateScaleTransform();
            }
        }

        /// <summary>
        /// Method which instantiates new Transforms 
        /// if they have not been declared earlier.
        /// Method assigns the variable scaleTransform 
        /// which represents the transform 
        /// to which the boundingbox aligns itself. It can be assigned
        /// directly if 'this' already has a Transform. If not
        /// it gets set by instantiating a new Transform.
        /// Once it is set- the new transform becomes the parent of scaleTransform.
        /// </summary>
        protected virtual void CreateTransforms()
        {
            if (scaleTransform == null)
            {
                scaleTransform = transform;
            }

            if (scaleTransform == null)
            {
                scaleTransform = new GameObject("Scale").transform;
            }

            scaleTransform.parent = transform;
        }

        /// <summary>
        /// re-calculates the Bounding box extrema (corners)
        /// of the axis aligned cube that bounds the Target gameObject.
        /// This method takes into account flattening.
        /// </summary>
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
                    BoundsExtensions.GetRenderBoundsPoints(target, boundsPoints, ignoreLayers);
                    break;

                case BoundsCalculationMethodEnum.Colliders:
                    BoundsExtensions.GetColliderBoundsPoints(target, boundsPoints, ignoreLayers);
                    break;

                case BoundsCalculationMethodEnum.MeshFilterBounds:
                    BoundsExtensions.GetMeshFilterBoundsPoints(target, boundsPoints, ignoreLayers);
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

        /// <summary>
        /// recomputes flattening if axis to be flattened has changed.
        /// </summary>
        protected virtual void UpdateFlattenedAxis()
        {
            // Find the maximum size of the new bounds
            float maxAxisThickness = Mathf.Max(Mathf.Max(targetBoundsLocalScale.x, targetBoundsLocalScale.y), targetBoundsLocalScale.z);

            FlattenModeEnum newFlattenedAxis = FlattenModeEnum.DoNotFlatten;
            switch (flattenPreference)
            {
                case FlattenModeEnum.DoNotFlatten:
                    // Do nothing
                    break;

                case FlattenModeEnum.FlattenAuto:
                    // Flattening order of preference - z, y, x
                    if (Mathf.Abs(targetBoundsLocalScale.z / maxAxisThickness) < flattenAxisThreshold)
                    {
                        newFlattenedAxis = FlattenModeEnum.FlattenZ;
                        targetBoundsLocalScale.z = flattenedAxisThickness * maxAxisThickness;
                    }
                    else if (Mathf.Abs(targetBoundsLocalScale.y / maxAxisThickness) < flattenAxisThreshold)
                    {
                        newFlattenedAxis = FlattenModeEnum.FlattenY;
                        targetBoundsLocalScale.y = flattenedAxisThickness * maxAxisThickness;
                    }
                    else if (Mathf.Abs(targetBoundsLocalScale.x / maxAxisThickness) < flattenAxisThreshold)
                    {
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
            if (target == null)
            {
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
                case FlattenModeEnum.DoNotFlatten:
                default:
                    scale.x += (largestDimension * scalePadding);
                    scale.y += (largestDimension * scalePadding);
                    scale.z += (largestDimension * scalePadding);
                    break;

                case FlattenModeEnum.FlattenX:
                    scale.x += (largestDimension * flattenedScalePadding);
                    scale.y += (largestDimension * scalePadding);
                    scale.z += (largestDimension * scalePadding);
                    break;

                case FlattenModeEnum.FlattenY:
                    scale.x += (largestDimension * scalePadding);
                    scale.y += (largestDimension * flattenedScalePadding);
                    scale.z += (largestDimension * scalePadding);
                    break;

                case FlattenModeEnum.FlattenZ:
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

        public void ManualUpdate()
        {
            RefreshTargetBounds();
            UpdateScaleTransform();
        }
    }
}