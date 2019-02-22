// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public class BoundingBox : BaseFocusHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<MixedRealityPose>,
        IMixedRealityPointerHandler,
        IMixedRealitySourceStateHandler
    {
        #region Enums
        /// <summary>
        /// Enum which describes how an object's boundingbox is to be flattened.
        /// </summary>
        private enum FlattenModeType
        {
            DoNotFlatten = 0,
            /// <summary>
            /// Flatten the X axis
            /// </summary>
            FlattenX,
            /// <summary>
            /// Flatten the Y axis
            /// </summary>
            FlattenY,
            /// <summary>
            /// Flatten the Z axis
            /// </summary>
            FlattenZ,
            /// <summary>
            /// Flatten the smallest relative axis if it falls below threshold
            /// </summary>
            FlattenAuto,
        }

        /// <summary>
        /// Enum which describes whether a boundingbox handle which has been grabbed, is 
        /// a Rotation Handle (sphere) or a Scale Handle( cube)
        /// </summary>
        private enum HandleType
        {
            None = 0,
            Rotation,
            Scale
        }

        /// <summary>
        /// This enum describes which primitive type the wireframe portion of the boundingbox
        /// consists of. 
        /// </summary>
        /// <remarks>
        /// Wireframe refers to the thin linkage between the handles. When the handles are invisible
        /// the wireframe looks like an outline box around an object.
        /// </remarks> 
        private enum WireframeType
        {
            Cubic = 0,
            Cylindrical
        }

        /// <summary>
        /// This enum defines which of the axes a given rotation handle revolves about.
        /// </summary>
        private enum CardinalAxisType
        {
            X = 0,
            Y,
            Z
        }

        /// <summary>
        /// This enum is used internally to define how an object's bounds are calculated in order to fit the boundingbox
        /// to it.
        /// </summary>
        private enum BoundsCalculationMethod
        {
            Collider = 0,
            Colliders,
            Renderers,
            MeshFilters
        }

        /// <summary>
        /// This enum defines how a particular controller rotates an object when a Rotate handle has been grabbed.
        /// </summary>
        /// <remarks>
        /// a Controller feels more natural when rotation of the controller rotates the object.
        /// the wireframe looks like an outline box around an object.
        /// </remarks> 
        private enum HandleMoveType
        {
            Ray = 0,
            Point
        }
        #endregion Enums

        #region Serialized Fields
        [Header("Bounds Calculation")]
        [Tooltip("For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of Bounding Box.")]
        [SerializeField]
        private BoxCollider boxColliderToUse = null;

        [Header("Behavior")]
        [SerializeField]
        private bool activateOnStart = false;

        [SerializeField]
        private float scaleMaximum = 2.0f;

        [SerializeField]
        private float scaleMinimum = 0.2f;

        [Header("Wireframe")]
        [SerializeField]
        private bool wireframeOnly = false;

        /// <summary>
        /// Public Property that displays simple wireframe around an object with no scale or rotate handles.
        /// </summary>
        /// <remarks>
        /// this is useful when outlining an object without being able to edit it is desired.
        /// </remarks>
        public bool WireframeOnly
        {
            get { return wireframeOnly; }
            set
            {
                if (wireframeOnly != value)
                {
                    wireframeOnly = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        private Vector3 wireframePadding = Vector3.zero;

        [SerializeField]
        private FlattenModeType flattenAxis = FlattenModeType.DoNotFlatten;

        [SerializeField]
        private WireframeType wireframeShape = WireframeType.Cubic;

        [SerializeField]
        private Material wireframeMaterial;

        [Header("Handles")]
        [Tooltip("Default materials will be created for Handles and Wireframe if none is specified.")]
        [SerializeField]
        private Material handleMaterial;

        [SerializeField]
        private Material handleGrabbedMaterial;

        [SerializeField]
        private bool showScaleHandles = true;

        /// <summary>
        /// Public property to Set the visibility of the corner cube Scaling handles.
        /// This property can be set independent of the Rotate handles.
        /// </summary>
        public bool ShowScaleHandles
        {
            get
            {
                return showScaleHandles;
            }
            set
            {
                if (showScaleHandles != value)
                {
                    showScaleHandles = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        private bool showRotateHandles = true;

        /// <summary>
        /// Public property to Set the visibility of the sphere rotating handles.
        /// This property can be set independent of the Scaling handles.
        /// </summary>
        public bool ShowRotateHandles
        {
            get
            {
                return showRotateHandles;
            }
            set
            {
                if (showRotateHandles != value)
                {
                    showRotateHandles = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        private float linkRadius = 0.005f;

        [SerializeField]
        private float ballRadius = 0.035f;

        [SerializeField]
        private float cornerRadius = 0.03f;
        #endregion Serialized Fields

        #region Constants
        private const int LeftTopBack = 0;
        private const int LeftTopFront = 1;
        private const int LeftBottomFront = 2;
        private const int LeftBottomBack = 3;
        private const int RightTopBack = 4;
        private const int RightTopFront = 5;
        private const int RightBottonFront = 6;
        private const int RightBottomBack = 7;
        private const int CORNER_COUNT = 8;
        #endregion Constants

        #region Private Properties
        private bool isActive = false;
        /// <summary>
        /// This Public property sets whether the BoundingBox is active (visible)
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if (isActive != value)
                {
                    if (value)
                    {
                        CreateRig();
                        rigRoot.gameObject.SetActive(true);
                    }
                    else
                    {
                        DestroyRig();
                    }

                    isActive = value;
                }
            }
        }
        private IMixedRealityPointer currentPointer;
        private IMixedRealityInputSource currentInputSource;
        private Vector3 initialGazePoint = Vector3.zero;
        private GameObject targetObject;
        private Transform rigRoot;
        private BoxCollider cachedTargetCollider;
        private Vector3[] boundsCorners;
        private Vector3 currentBoundsSize;
        private BoundsCalculationMethod boundsMethod;
        private HandleMoveType handleMoveType = HandleMoveType.Point;
        private List<Transform> links;
        private List<Transform> corners;
        private List<Transform> balls;
        private List<Renderer> cornerRenderers;
        private List<Renderer> ballRenderers;
        private List<Renderer> linkRenderers;
        private List<Collider> cornerColliders;
        private List<Collider> ballColliders;
        private Vector3[] edgeCenters;
        private Ray initialGrabRay;
        private Ray currentGrabRay;
        private float initialGrabMag;
        private Vector3 currentRotationAxis;
        private Vector3 initialScale;
        private Vector3 initialGrabbedPosition;
        private Vector3 initialGrabbedCentroid;
        private Vector3 initialGrabPoint;
        private CardinalAxisType[] edgeAxes;
        private int[] flattenedHandles;
        private Vector3 boundsCentroid;
        private GameObject grabbedHandle;
        private bool usingPose = false;
        private Vector3 currentPosePosition = Vector3.zero;
        private HandleType currentHandleType;
        #endregion Private Properties

        #region MonoBehaviour Methods
        private void Start()
        {
            targetObject = this.gameObject;

            if (MixedRealityToolkit.IsInitialized && MixedRealityToolkit.InputSystem != null)
            {
                MixedRealityToolkit.InputSystem.Register(targetObject);
            }

            if (activateOnStart == true)
            {
                IsActive = true;
            }
        }

        private void Update()
        {
            if (currentInputSource == null)
            {
                UpdateBounds();
            }
            else
            {
                UpdateBounds();
                TransformRig();
            }

            UpdateRigHandles();
        }
        #endregion MonoBehaviour Methods

        #region Private Methods
        private void CreateRig()
        {
            DestroyRig();
            SetMaterials();
            InitializeDataStructures();

            SetBoundingBoxCollider();

            UpdateBounds();
            AddCorners();
            AddLinks();
            UpdateRigHandles();
            Flatten();
            ResetHandleVisibility();
            rigRoot.gameObject.SetActive(false);
        }

        private void DestroyRig()
        {
            if (boxColliderToUse == null)
            {
                Destroy(cachedTargetCollider);
            }
            else
            {
                boxColliderToUse.size -= wireframePadding;
            }

            if (balls != null)
            {
                for (var i = 0; i < balls.Count; i++)
                {
                    Destroy(balls[i]);
                }

                balls.Clear();
            }

            if (links != null)
            {
                for (int i = 0; i < links.Count; i++)
                {
                    Destroy(links[i]);
                }

                links.Clear();
            }

            if (corners != null)
            {
                for (var i = 0; i < corners.Count; i++)
                {
                    Destroy(corners[i]);
                }

                corners.Clear();
            }

            if (rigRoot != null)
            {
                Destroy(rigRoot);
            }
        }

        private void TransformRig()
        {
            if (usingPose)
            {
                TransformHandleWithPoint();
            }
            else
            {
                switch (handleMoveType)
                {
                    case HandleMoveType.Ray:
                        TransformHandleWithRay();
                        break;
                    case HandleMoveType.Point:
                        TransformHandleWithPoint();
                        break;
                    default:
                        Debug.LogWarning($"Unexpected handle move type {handleMoveType}");
                        break;
                }
            }
        }

        private void TransformHandleWithRay()
        {
            if (currentHandleType != HandleType.None)
            {
                currentGrabRay = GetHandleGrabbedRay();
                Vector3 grabRayPt = currentGrabRay.origin + (currentGrabRay.direction * initialGrabMag);

                switch (currentHandleType)
                {
                    case HandleType.Rotation:
                        RotateByHandle(grabRayPt);
                        break;
                    case HandleType.Scale:
                        ScaleByHandle(grabRayPt);
                        break;
                    default:
                        Debug.LogWarning($"Unexpected handle type {currentHandleType}");
                        break;
                }
            }
        }

        private void TransformHandleWithPoint()
        {
            if (currentHandleType != HandleType.None)
            {
                Vector3 newGrabbedPosition;

                if (usingPose == false)
                {
                    Vector3 newRemotePoint;
                    currentPointer.TryGetPointerPosition(out newRemotePoint);
                    newGrabbedPosition = initialGrabbedPosition + (newRemotePoint - initialGrabPoint);
                }
                else
                {
                    if (initialGazePoint == Vector3.zero)
                    {
                        return;
                    }

                    newGrabbedPosition = currentPosePosition;
                }

                if (currentHandleType == HandleType.Rotation)
                {
                    RotateByHandle(newGrabbedPosition);
                }
                else if (currentHandleType == HandleType.Scale)
                {
                    ScaleByHandle(newGrabbedPosition);
                }
            }
        }

        private void RotateByHandle(Vector3 newHandlePosition)
        {
            Vector3 projPt = Vector3.ProjectOnPlane((newHandlePosition - rigRoot.transform.position).normalized, currentRotationAxis);
            Quaternion rotation = Quaternion.FromToRotation((grabbedHandle.transform.position - rigRoot.transform.position).normalized, projPt.normalized);
            Vector3 axis;
            float angle;
            rotation.ToAngleAxis(out angle, out axis);
            targetObject.transform.RotateAround(rigRoot.transform.position, axis, angle);
        }

        private void ScaleByHandle(Vector3 newHandlePosition)
        {
            Vector3 correctedPt = PointToRay(rigRoot.transform.position, grabbedHandle.transform.position, newHandlePosition);
            Vector3 rigCentroid = rigRoot.transform.position;
            float startMag = (initialGrabbedPosition - rigCentroid).magnitude;
            float newMag = (correctedPt - rigCentroid).magnitude;

            bool isClamped;
            float ratio = newMag / startMag;
            Vector3 newScale = ClampScale(initialScale * ratio, out isClamped);
            //scale from object center
            targetObject.transform.localScale = newScale;
        }

        private Vector3 GetRotationAxis(GameObject handle)
        {
            for (int i = 0; i < balls.Count; ++i)
            {
                if (handle == balls[i])
                {
                    switch (edgeAxes[i])
                    {
                        case CardinalAxisType.X:
                            return rigRoot.transform.right;
                        case CardinalAxisType.Y:
                            return rigRoot.transform.up;
                        default:
                            return rigRoot.transform.forward;
                    }
                }
            }

            return Vector3.zero;
        }

        private void AddCorners()
        {
            for (int i = 0; i < boundsCorners.Length; ++i)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = $"corner_{i}";
                cube.transform.localScale = new Vector3(cornerRadius, cornerRadius, cornerRadius);
                cube.transform.position = boundsCorners[i];
                cube.transform.parent = rigRoot.transform;

                var cubeRenderer = cube.GetComponent<Renderer>();
                cornerRenderers.Add(cubeRenderer);
                cornerColliders.Add(cube.GetComponent<Collider>());
                corners.Add(cube.transform);

                if (handleMaterial != null)
                {
                    cubeRenderer.material = handleMaterial;
                }
            }
        }

        private void AddLinks()
        {
            edgeCenters = new Vector3[12];

            CalculateEdgeCenters();

            for (int i = 0; i < edgeCenters.Length; ++i)
            {
                GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.name = $"midpoint_{i}";
                ball.transform.localScale = new Vector3(ballRadius, ballRadius, ballRadius);
                ball.transform.position = edgeCenters[i];
                ball.transform.parent = rigRoot.transform;

                var ballRenderer = ball.GetComponent<Renderer>();
                ballRenderers.Add(ballRenderer);
                ballColliders.Add(ball.GetComponent<Collider>());
                balls.Add(ball.transform);

                if (handleMaterial != null)
                {
                    ballRenderer.material = handleMaterial;
                }
            }

            edgeAxes = new CardinalAxisType[12];

            edgeAxes[0] = CardinalAxisType.X;
            edgeAxes[1] = CardinalAxisType.Y;
            edgeAxes[2] = CardinalAxisType.X;
            edgeAxes[3] = CardinalAxisType.Y;
            edgeAxes[4] = CardinalAxisType.X;
            edgeAxes[5] = CardinalAxisType.Y;
            edgeAxes[6] = CardinalAxisType.X;
            edgeAxes[7] = CardinalAxisType.Y;
            edgeAxes[8] = CardinalAxisType.Z;
            edgeAxes[9] = CardinalAxisType.Z;
            edgeAxes[10] = CardinalAxisType.Z;
            edgeAxes[11] = CardinalAxisType.Z;

            for (int i = 0; i < edgeCenters.Length; ++i)
            {
                var link = GameObject.CreatePrimitive(wireframeShape == WireframeType.Cubic
                    ? PrimitiveType.Cube
                    : PrimitiveType.Cylinder);
                link.name = $"link_{i}";

                Vector3 linkDimensions = GetLinkDimensions();

                switch (edgeAxes[i])
                {
                    case CardinalAxisType.Y:
                        link.transform.localScale = new Vector3(linkRadius, linkDimensions.y, linkRadius);
                        link.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
                        break;
                    case CardinalAxisType.Z:
                        link.transform.localScale = new Vector3(linkRadius, linkDimensions.z, linkRadius);
                        link.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                        break;
                    default: //X
                        link.transform.localScale = new Vector3(linkRadius, linkDimensions.x, linkRadius);
                        link.transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
                        break;
                }

                link.transform.position = edgeCenters[i];
                link.transform.parent = rigRoot.transform;

                var linkRenderer = link.GetComponent<Renderer>();
                linkRenderers.Add(linkRenderer);

                if (wireframeMaterial != null)
                {
                    linkRenderer.material = wireframeMaterial;
                }

                links.Add(link.transform);
            }
        }

        private void SetBoundingBoxCollider()
        {
            //Collider.bounds is world space bounding volume.
            //Mesh.bounds is local space bounding volume
            //Renderer.bounds is the same as mesh.bounds but in world space coords

            if (boxColliderToUse != null)
            {
                cachedTargetCollider = boxColliderToUse;
                cachedTargetCollider.transform.hasChanged = true;
            }
            else
            {
                Bounds bounds = GetTargetBounds();
                cachedTargetCollider = targetObject.AddComponent<BoxCollider>();
                switch (boundsMethod)
                {
                    case BoundsCalculationMethod.Renderers:
                        cachedTargetCollider.center = bounds.center;
                        cachedTargetCollider.size = bounds.size;
                        break;
                    case BoundsCalculationMethod.Colliders:
                        cachedTargetCollider.center = bounds.center;
                        cachedTargetCollider.size = bounds.size;
                        break;
                    default:
                        Debug.LogWarning($"Unexpected Bounds Calculation Method {boundsMethod}");
                        break;
                }
            }

            cachedTargetCollider.size += wireframePadding;
        }

        private Bounds GetTargetBounds()
        {
            var bounds = new Bounds();

            if (targetObject.transform.childCount == 0)
            {
                bounds = GetSingleObjectBounds(targetObject);
                boundsMethod = BoundsCalculationMethod.Collider;
                return bounds;
            }

            for (int i = 0; i < targetObject.transform.childCount; ++i)
            {
                if (bounds.size == Vector3.zero)
                {
                    bounds = GetSingleObjectBounds(targetObject.transform.GetChild(i).gameObject);
                }
                else
                {
                    Bounds childBounds = GetSingleObjectBounds(targetObject.transform.GetChild(i).gameObject);

                    if (childBounds.size != Vector3.zero)
                    {
                        bounds.Encapsulate(childBounds);
                    }
                }
            }

            if (bounds.size != Vector3.zero)
            {
                boundsMethod = BoundsCalculationMethod.Colliders;
                return bounds;
            }

            //simple case: sum of existing colliders
            Collider[] colliders = targetObject.GetComponentsInChildren<Collider>();
            if (colliders.Length > 0)
            {
                //Collider.bounds is in world space.
                bounds = colliders[0].bounds;

                for (int i = 0; i < colliders.Length; ++i)
                {
                    Bounds colliderBounds = colliders[i].bounds;
                    if (colliderBounds.size != Vector3.zero)
                    {
                        bounds.Encapsulate(colliderBounds);
                    }
                }

                if (bounds.size != Vector3.zero)
                {
                    boundsMethod = BoundsCalculationMethod.Colliders;
                    return bounds;
                }
            }

            //Renderer bounds is local. Requires transform to global coord system.
            Renderer[] childRenderers = targetObject.GetComponentsInChildren<Renderer>();
            if (childRenderers.Length > 0)
            {
                bounds = childRenderers[0].bounds;
                var _corners = new Vector3[CORNER_COUNT];

                for (int i = 0; i < childRenderers.Length; ++i)
                {
                    bounds.Encapsulate(childRenderers[i].bounds);
                }

                GetCornerPositionsFromBounds(bounds, ref boundsCorners);

                for (int cornerIndex = 0; cornerIndex < _corners.Length; ++cornerIndex)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = cornerIndex.ToString();
                    cube.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                    cube.transform.position = boundsCorners[cornerIndex];
                }

                boundsMethod = BoundsCalculationMethod.Renderers;
                return bounds;
            }

            MeshFilter[] meshFilters = targetObject.GetComponentsInChildren<MeshFilter>();

            if (meshFilters.Length > 0)
            {
                //Mesh.bounds is local space bounding volume
                bounds.size = meshFilters[0].mesh.bounds.size;
                bounds.center = meshFilters[0].mesh.bounds.center;

                for (int i = 0; i < meshFilters.Length; ++i)
                {
                    bounds.Encapsulate(meshFilters[i].mesh.bounds);
                }

                if (bounds.size != Vector3.zero)
                {
                    bounds.center = targetObject.transform.position;
                    boundsMethod = BoundsCalculationMethod.MeshFilters;
                    return bounds;
                }
            }

            var boxCollider = targetObject.AddComponent<BoxCollider>();
            bounds = boxCollider.bounds;
            Destroy(boxCollider);
            boundsMethod = BoundsCalculationMethod.Collider;
            return bounds;
        }

        private Bounds GetSingleObjectBounds(GameObject boundsObject)
        {
            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            Component[] components = boundsObject.GetComponents<Component>();

            if (components.Length < 3)
            {
                return bounds;
            }

            var boxCollider = boundsObject.GetComponent<BoxCollider>();

            if (boxCollider == null)
            {
                boxCollider = boundsObject.AddComponent<BoxCollider>();
                bounds = boxCollider.bounds;
                Destroy(boxCollider);
            }
            else
            {
                bounds = boxCollider.bounds;
            }

            return bounds;
        }

        private void SetMaterials()
        {
            if (wireframeMaterial == null)
            {
                Shader.EnableKeyword("_InnerGlow");
                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                wireframeMaterial = new Material(shader);
                wireframeMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
            }

            if (handleMaterial == null && handleMaterial != wireframeMaterial)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                Shader.EnableKeyword("_InnerGlow");
                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                handleMaterial = new Material(shader);
                handleMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleMaterial.SetFloat("_InnerGlow", 1.0f);
                handleMaterial.SetFloatArray("_InnerGlowColor", color);
            }

            if (handleGrabbedMaterial == null && handleGrabbedMaterial != handleMaterial && handleGrabbedMaterial != wireframeMaterial)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                Shader.EnableKeyword("_InnerGlow");
                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                handleGrabbedMaterial = new Material(shader);
                handleGrabbedMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleGrabbedMaterial.SetFloat("_InnerGlow", 1.0f);
                handleGrabbedMaterial.SetFloatArray("_InnerGlowColor", color);
            }
        }

        private void InitializeDataStructures()
        {
            rigRoot = new GameObject("rigRoot").transform;
            rigRoot.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

            boundsCorners = new Vector3[8];

            corners = new List<Transform>();
            cornerColliders = new List<Collider>();
            cornerRenderers = new List<Renderer>();
            balls = new List<Transform>();
            ballRenderers = new List<Renderer>();
            ballColliders = new List<Collider>();
            links = new List<Transform>();
            linkRenderers = new List<Renderer>();
        }

        private void CalculateEdgeCenters()
        {
            if (boundsCorners != null && edgeCenters != null)
            {
                edgeCenters[0] = (boundsCorners[0] + boundsCorners[1]) * 0.5f;
                edgeCenters[1] = (boundsCorners[1] + boundsCorners[2]) * 0.5f;
                edgeCenters[2] = (boundsCorners[2] + boundsCorners[3]) * 0.5f;
                edgeCenters[3] = (boundsCorners[3] + boundsCorners[0]) * 0.5f;

                edgeCenters[4] = (boundsCorners[4] + boundsCorners[5]) * 0.5f;
                edgeCenters[5] = (boundsCorners[5] + boundsCorners[6]) * 0.5f;
                edgeCenters[6] = (boundsCorners[6] + boundsCorners[7]) * 0.5f;
                edgeCenters[7] = (boundsCorners[7] + boundsCorners[4]) * 0.5f;

                edgeCenters[8] = (boundsCorners[0] + boundsCorners[4]) * 0.5f;
                edgeCenters[9] = (boundsCorners[1] + boundsCorners[5]) * 0.5f;
                edgeCenters[10] = (boundsCorners[2] + boundsCorners[6]) * 0.5f;
                edgeCenters[11] = (boundsCorners[3] + boundsCorners[7]) * 0.5f;
            }
        }

        private Vector3 ClampScale(Vector3 scale, out bool clamped)
        {
            Vector3 finalScale = scale;
            Vector3 maximumScale = initialScale * scaleMaximum;
            clamped = false;

            if (scale.x > maximumScale.x || scale.y > maximumScale.y || scale.z > maximumScale.z)
            {
                finalScale = maximumScale;
                clamped = true;
            }

            Vector3 minimumScale = initialScale * scaleMinimum;

            if (finalScale.x < minimumScale.x || finalScale.y < minimumScale.y || finalScale.z < minimumScale.z)
            {
                finalScale = minimumScale;
                clamped = true;
            }

            return finalScale;
        }

        private Vector3 GetLinkDimensions()
        {
            float linkLengthAdjustor = wireframeShape == WireframeType.Cubic ? 2.0f : 1.0f - (6.0f * linkRadius);
            return (currentBoundsSize * linkLengthAdjustor) + new Vector3(linkRadius, linkRadius, linkRadius);
        }

        private void ResetHandleVisibility()
        {
            bool isVisible;

            //set balls visibility
            if (balls != null)
            {
                isVisible = (!wireframeOnly && showRotateHandles);

                for (int i = 0; i < ballRenderers.Count; ++i)
                {
                    ballRenderers[i].material = handleMaterial;
                    ballRenderers[i].enabled = isVisible;
                }
            }

            //set corner visibility
            if (corners != null)
            {
                isVisible = (!wireframeOnly && showScaleHandles);

                for (int i = 0; i < cornerRenderers.Count; ++i)
                {
                    cornerRenderers[i].material = handleMaterial;
                    cornerRenderers[i].enabled = isVisible;
                }
            }

            SetHiddenHandles();
        }

        private void ShowOneHandle(GameObject handle)
        {
            //turn off all balls
            if (balls != null)
            {
                for (int i = 0; i < ballRenderers.Count; ++i)
                {
                    ballRenderers[i].enabled = false;
                }
            }

            //turn off all corners
            if (corners != null)
            {
                for (int i = 0; i < cornerRenderers.Count; ++i)
                {
                    cornerRenderers[i].enabled = false;
                }
            }

            //turn on one handle
            if (handle != null)
            {
                var handleRenderer = handle.GetComponent<Renderer>();
                handleRenderer.material = handleGrabbedMaterial;
                handleRenderer.enabled = true;
            }
        }

        private void UpdateBounds()
        {
            Vector3 boundsSize = Vector3.zero;
            Vector3 centroid = Vector3.zero;

            //store current rotation then zero out the rotation so that the bounds
            //are computed when the object is in its 'axis aligned orientation'.
            Quaternion currentRotation = targetObject.transform.rotation;
            targetObject.transform.rotation = Quaternion.identity;

            if (cachedTargetCollider != null)
            {
                Bounds colliderBounds = cachedTargetCollider.bounds;
                boundsSize = colliderBounds.extents;
                centroid = colliderBounds.center;
            }

            //after bounds are computed, restore rotation...
            targetObject.transform.rotation = currentRotation;

            if (boundsSize != Vector3.zero)
            {
                if (flattenAxis == FlattenModeType.FlattenAuto)
                {
                    float min = Mathf.Min(boundsSize.x, Mathf.Min(boundsSize.y, boundsSize.z));
                    flattenAxis = min.Equals(boundsSize.x) ? FlattenModeType.FlattenX : (min.Equals(boundsSize.y) ? FlattenModeType.FlattenY : FlattenModeType.FlattenZ);
                }

                boundsSize.x = flattenAxis == FlattenModeType.FlattenX ? 0.0f : boundsSize.x;
                boundsSize.y = flattenAxis == FlattenModeType.FlattenY ? 0.0f : boundsSize.y;
                boundsSize.z = flattenAxis == FlattenModeType.FlattenZ ? 0.0f : boundsSize.z;

                currentBoundsSize = boundsSize;
                boundsCentroid = centroid;

                boundsCorners[0] = centroid - new Vector3(centroid.x - currentBoundsSize.x, centroid.y - currentBoundsSize.y, centroid.z - currentBoundsSize.z);
                boundsCorners[1] = centroid - new Vector3(centroid.x + currentBoundsSize.x, centroid.y - currentBoundsSize.y, centroid.z - currentBoundsSize.z);
                boundsCorners[2] = centroid - new Vector3(centroid.x + currentBoundsSize.x, centroid.y + currentBoundsSize.y, centroid.z - currentBoundsSize.z);
                boundsCorners[3] = centroid - new Vector3(centroid.x - currentBoundsSize.x, centroid.y + currentBoundsSize.y, centroid.z - currentBoundsSize.z);

                boundsCorners[4] = centroid - new Vector3(centroid.x - currentBoundsSize.x, centroid.y - currentBoundsSize.y, centroid.z + currentBoundsSize.z);
                boundsCorners[5] = centroid - new Vector3(centroid.x + currentBoundsSize.x, centroid.y - currentBoundsSize.y, centroid.z + currentBoundsSize.z);
                boundsCorners[6] = centroid - new Vector3(centroid.x + currentBoundsSize.x, centroid.y + currentBoundsSize.y, centroid.z + currentBoundsSize.z);
                boundsCorners[7] = centroid - new Vector3(centroid.x - currentBoundsSize.x, centroid.y + currentBoundsSize.y, centroid.z + currentBoundsSize.z);

                CalculateEdgeCenters();
            }
        }

        private void UpdateRigHandles()
        {
            if (rigRoot != null && targetObject != null)
            {
                rigRoot.rotation = Quaternion.identity;
                rigRoot.position = Vector3.zero;

                for (int i = 0; i < corners.Count; ++i)
                {
                    corners[i].position = boundsCorners[i];
                }

                Vector3 linkDimensions = GetLinkDimensions();

                for (int i = 0; i < edgeCenters.Length; ++i)
                {
                    balls[i].position = edgeCenters[i];
                    links[i].position = edgeCenters[i];

                    if (edgeAxes[i] == CardinalAxisType.X)
                    {
                        links[i].localScale = new Vector3(linkRadius, linkDimensions.x, linkRadius);
                    }
                    else if (edgeAxes[i] == CardinalAxisType.Y)
                    {
                        links[i].localScale = new Vector3(linkRadius, linkDimensions.y, linkRadius);
                    }
                    else
                    {
                        links[i].localScale = new Vector3(linkRadius, linkDimensions.z, linkRadius);
                    }
                }

                //move rig into position and rotation
                rigRoot.position = cachedTargetCollider.bounds.center;
                rigRoot.rotation = targetObject.transform.rotation;
            }
        }

        private HandleType GetHandleType(GameObject handle)
        {
            for (int i = 0; i < balls.Count; ++i)
            {
                if (handle == balls[i])
                {
                    return HandleType.Rotation;
                }
            }

            for (int i = 0; i < corners.Count; ++i)
            {
                if (handle == corners[i])
                {
                    return HandleType.Scale;
                }
            }

            return HandleType.None;
        }

        private Collider GetGrabbedCollider(Ray ray, out float distance)
        {
            Collider closestCollider = null;
            float currentDistance;
            float closestDistance = float.MaxValue;


            for (int i = 0; i < cornerColliders.Count; ++i)
            {
                if (cornerRenderers[i].enabled && cornerColliders[i].bounds.IntersectRay(ray, out currentDistance))
                {
                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        closestCollider = cornerColliders[i];
                    }
                }
            }

            for (int i = 0; i < ballColliders.Count; ++i)
            {
                if (ballRenderers[i].enabled && ballColliders[i].bounds.IntersectRay(ray, out currentDistance))
                {
                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        closestCollider = ballColliders[i];
                    }
                }
            }

            distance = closestDistance;
            return closestCollider;
        }

        private Ray GetHandleGrabbedRay()
        {
            Ray pointerRay = new Ray();

            if (currentInputSource.Pointers.Length > 0)
            {
                currentInputSource.Pointers[0].TryGetPointingRay(out pointerRay);
            }

            return pointerRay;
        }

        private void Flatten()
        {
            switch (flattenAxis)
            {
                case FlattenModeType.FlattenX:
                    flattenedHandles = new[] { 0, 4, 2, 6 };
                    break;
                case FlattenModeType.FlattenY:
                    flattenedHandles = new[] { 1, 3, 5, 7 };
                    break;
                case FlattenModeType.FlattenZ:
                    flattenedHandles = new[] { 9, 10, 8, 11 };
                    break;
            }

            if (flattenedHandles != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    linkRenderers[flattenedHandles[i]].enabled = false;
                }
            }
        }

        private void SetHiddenHandles()
        {
            if (flattenedHandles != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    ballRenderers[flattenedHandles[i]].enabled = false;
                }
            }
        }

        private void GetCornerPositionsFromBounds(Bounds bounds, ref Vector3[] positions)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            if (positions == null || positions.Length != CORNER_COUNT)
            {
                positions = new Vector3[CORNER_COUNT];
            }

            positions[LeftBottomFront] = new Vector3(leftEdge, bottomEdge, frontEdge);
            positions[LeftBottomBack] = new Vector3(leftEdge, bottomEdge, backEdge);
            positions[LeftTopFront] = new Vector3(leftEdge, topEdge, frontEdge);
            positions[LeftTopBack] = new Vector3(leftEdge, topEdge, backEdge);
            positions[RightBottonFront] = new Vector3(rightEdge, bottomEdge, frontEdge);
            positions[RightBottomBack] = new Vector3(rightEdge, bottomEdge, backEdge);
            positions[RightTopFront] = new Vector3(rightEdge, topEdge, frontEdge);
            positions[RightTopBack] = new Vector3(rightEdge, topEdge, backEdge);
        }

        private static Vector3 PointToRay(Vector3 origin, Vector3 end, Vector3 closestPoint)
        {
            Vector3 originToPoint = closestPoint - origin;
            Vector3 originToEnd = end - origin;
            float magnitudeAb = originToEnd.sqrMagnitude;
            float dotProduct = Vector3.Dot(originToPoint, originToEnd);
            float distance = dotProduct / magnitudeAb;
            return origin + (originToEnd * distance);
        }
        #endregion Private Methods

        #region Used Event Handlers
        public void OnInputDown(InputEventData eventData)
        {
            if (currentInputSource == null)
            {
                IMixedRealityPointer pointer = eventData.InputSource.Pointers[0];
                Ray ray;
                if (pointer.TryGetPointingRay(out ray))
                {
                    handleMoveType = HandleMoveType.Ray;
                    float distance;
                    Collider grabbedCollider = GetGrabbedCollider(ray, out distance);

                    if (grabbedCollider != null)
                    {
                        currentInputSource = eventData.InputSource;
                        currentPointer = pointer;
                        grabbedHandle = grabbedCollider.gameObject;
                        currentHandleType = GetHandleType(grabbedHandle);
                        currentRotationAxis = GetRotationAxis(grabbedHandle);
                        currentPointer.TryGetPointingRay(out initialGrabRay);
                        initialGrabMag = distance;
                        initialGrabbedPosition = grabbedHandle.transform.position;
                        initialGrabbedCentroid = targetObject.transform.position;
                        initialScale = targetObject.transform.localScale;
                        pointer.TryGetPointerPosition(out initialGrabPoint);
                        ShowOneHandle(grabbedHandle);
                        initialGazePoint = Vector3.zero;
                    }
                }
            }
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (currentInputSource != null && eventData.InputSource.SourceId == currentInputSource.SourceId)
            {
                currentInputSource = null;
                currentHandleType = HandleType.None;
                currentPointer = null;
                grabbedHandle = null;
                ResetHandleVisibility();
            }
        }

        public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (currentInputSource != null && eventData.InputSource.SourceId == currentInputSource.SourceId)
            {
                Vector3 pos = eventData.InputData.Position;
                usingPose = true;
                if (initialGazePoint == Vector3.zero)
                {
                    initialGazePoint = pos;
                }
                currentPosePosition = initialGrabbedPosition + (pos - initialGazePoint);
            }
            else
            {
                usingPose = false;
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (currentInputSource != null && eventData.InputSource.SourceId == currentInputSource.SourceId)
            {
                currentInputSource = null;
                currentHandleType = HandleType.None;
                currentPointer = null;
                grabbedHandle = null;
                ResetHandleVisibility();
            }
        }
        #endregion Used Event Handlers

        #region Unused Event Handlers
        public void OnPointerDown(MixedRealityPointerEventData eventData) { }
        public void OnPointerUp(MixedRealityPointerEventData eventData) { }
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
        public void OnInputPressed(InputEventData<float> eventData) { }
        public void OnPositionInputChanged(InputEventData<Vector2> eventData) { }
        public void OnPositionChanged(InputEventData<Vector3> eventData) { }
        public void OnRotationChanged(InputEventData<Quaternion> eventData) { }
        public void OnSourceDetected(SourceStateEventData eventData) { }
        #endregion Unused Event Handlers
    }
}
