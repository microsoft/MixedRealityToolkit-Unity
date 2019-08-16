// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityPhysics = UnityEngine.Physics;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_BoundingBox.html")]
    public class BoundingBox : MonoBehaviour,
        IMixedRealitySourceStateHandler,
        IMixedRealityFocusChangedHandler,
        IMixedRealityFocusHandler
    {
        #region Serialized Fields and Properties
        [SerializeField]
        [Tooltip("The object that the bounding box rig will be modifying.")]
        private GameObject targetObject;

        [Tooltip("For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of Bounding Box.")]
        [SerializeField]
        [FormerlySerializedAs("BoxColliderToUse")]
        private BoxCollider boundsOverride = null;

        /// <summary>
        /// For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of Bounding Box.
        /// </summary>
        public BoxCollider BoundsOverride
        {
            get { return boundsOverride; }
            set
            {
                if (boundsOverride != value)
                {
                    boundsOverride = value;

                    if (boundsOverride == null)
                    {
                        prevBoundsOverride = new Bounds();
                    }
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Defines the volume type and the priority for the bounds calculation")]
        private BoundsCalculationMethod boundsCalculationMethod = BoundsCalculationMethod.RendererOverCollider;

        /// <summary>
        /// Defines the volume type and the priority for the bounds calculation
        /// </summary>
        public BoundsCalculationMethod CalculationMethod
        {
            get { return boundsCalculationMethod; }
            set
            {
                if (boundsCalculationMethod != value)
                {
                    boundsCalculationMethod = value;
                    CreateRig();
                }
            }
        }

        [Header("Behavior")]
        [SerializeField]
        [Tooltip("Type of activation method for showing/hiding bounding box handles and controls")]
        private BoundingBoxActivationType activation = BoundingBoxActivationType.ActivateOnStart;

        /// <summary>
        /// Type of activation method for showing/hiding bounding box handles and controls
        /// </summary>
        public BoundingBoxActivationType BoundingBoxActivation
        {
            get { return activation; }
            set
            {
                if (activation != value)
                {
                    activation = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Obsolete("Use a TransformScaleHandler script rather than setting minimum on BoundingBox directly", false)]
        [Tooltip("Minimum scaling allowed relative to the initial size")]
        private float scaleMinimum = 0.2f;

        [SerializeField]
        [Obsolete("Use a TransformScaleHandler script rather than setting maximum on BoundingBox directly")]
        [Tooltip("Maximum scaling allowed relative to the initial size")]
        private float scaleMaximum = 2.0f;


        /// <summary>
        /// Public property for the scale minimum, in the target's local scale.
        /// Set this value with SetScaleLimits.
        /// </summary>
        [Obsolete("Use a TransformScaleHandler.ScaleMinimum as it is the authoritative value for min scale")]
        public float ScaleMinimum
        {
            get
            {
                if (scaleHandler != null)
                {
                    return scaleHandler.ScaleMinimum;
                }
                return 0.0f;
            }
        }

        /// <summary>
        /// Public property for the scale maximum, in the target's local scale.
        /// Set this value with SetScaleLimits.
        /// </summary>
        [Obsolete("Use a TransformScaleHandler.ScaleMinimum as it is the authoritative value for max scale")]
        public float ScaleMaximum
        {
            get
            {
                if (scaleHandler != null)
                {
                    return scaleHandler.ScaleMaximum;
                }
                return 0.0f;
            }
        }

        [Header("Box Display")]
        [SerializeField]
        [Tooltip("Flatten bounds in the specified axis or flatten the smallest one if 'auto' is selected")]
        private FlattenModeType flattenAxis = FlattenModeType.DoNotFlatten;

        /// <summary>
        /// Flatten bounds in the specified axis or flatten the smallest one if 'auto' is selected
        /// </summary>
        public FlattenModeType FlattenAxis
        {
            get { return flattenAxis; }
            set
            {
                if (flattenAxis != value)
                {
                    flattenAxis = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("When an axis is flattened what value to set that axis's scale to for display.")]
        private float flattenAxisDisplayScale = 0.0f;

        /// <summary>
        /// When an axis is flattened what value to set that axis's scale to for display.
        /// </summary>
        public float FlattenAxisDisplayScale
        {
            get { return flattenAxisDisplayScale; }
            set
            {
                if (flattenAxisDisplayScale != value)
                {
                    flattenAxisDisplayScale = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [FormerlySerializedAs("wireframePadding")]
        [Tooltip("Extra padding added to the actual Target bounds")]
        private Vector3 boxPadding = Vector3.zero;

        /// <summary>
        /// Extra padding added to the actual Target bounds
        /// </summary>
        public Vector3 BoxPadding
        {
            get { return boxPadding; }
            set
            {
                if (Vector3.Distance(boxPadding, value) > float.Epsilon)
                {
                    boxPadding = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material used to display the bounding box. If set to null no bounding box will be displayed")]
        private Material boxMaterial = null;

        /// <summary>
        /// Material used to display the bounding box. If set to null no bounding box will be displayed
        /// </summary>
        public Material BoxMaterial
        {
            get { return boxMaterial; }
            set
            {
                if (boxMaterial != value)
                {
                    boxMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material used to display the bounding box when grabbed. If set to null no change will occur when grabbed.")]
        private Material boxGrabbedMaterial = null;

        /// <summary>
        /// Material used to display the bounding box when grabbed. If set to null no change will occur when grabbed.
        /// </summary>
        public Material BoxGrabbedMaterial
        {
            get { return boxGrabbedMaterial; }
            set
            {
                if (boxGrabbedMaterial != value)
                {
                    boxGrabbedMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Show a wireframe around the bounding box when checked. Wireframe parameters below have no effect unless this is checked")]
        private bool showWireframe = true;

        /// <summary>
        /// Show a wireframe around the bounding box when checked. Wireframe parameters below have no effect unless this is checked
        /// </summary>
        public bool ShowWireFrame
        {
            get { return showWireframe; }
            set
            {
                if (showWireframe != value)
                {
                    showWireframe = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Shape used for wireframe display")]
        private WireframeType wireframeShape = WireframeType.Cubic;

        /// <summary>
        /// Shape used for wireframe display
        /// </summary>
        public WireframeType WireframeShape
        {
            get { return wireframeShape; }
            set
            {
                if (wireframeShape != value)
                {
                    wireframeShape = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material used for wireframe display")]
        private Material wireframeMaterial;

        /// <summary>
        /// Material used for wireframe display
        /// </summary>
        public Material WireframeMaterial
        {
            get { return wireframeMaterial; }
            set
            {
                if (wireframeMaterial != value)
                {
                    wireframeMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [FormerlySerializedAs("linkRadius")]
        [Tooltip("Radius for wireframe edges")]
        private float wireframeEdgeRadius = 0.001f;

        /// <summary>
        /// Radius for wireframe edges
        /// </summary>
        public float WireframeEdgeRadius
        {
            get { return wireframeEdgeRadius; }
            set
            {
                if (wireframeEdgeRadius != value)
                {
                    wireframeEdgeRadius = value;
                    CreateRig();
                }
            }
        }

        [Header("Handles")]
        [SerializeField]
        [Tooltip("Material applied to handles when they are not in a grabbed state")]
        private Material handleMaterial;

        /// <summary>
        /// Material applied to handles when they are not in a grabbed state
        /// </summary>
        public Material HandleMaterial
        {
            get { return handleMaterial; }
            set
            {
                if (handleMaterial != value)
                {
                    handleMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material applied to handles while they are a grabbed")]
        private Material handleGrabbedMaterial;

        /// <summary>
        /// Material applied to handles while they are a grabbed
        /// </summary>
        public Material HandleGrabbedMaterial
        {
            get { return handleGrabbedMaterial; }
            set
            {
                if (handleGrabbedMaterial != value)
                {
                    handleGrabbedMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display scale handles in corners. If not set, boxes will be displayed instead")]
        GameObject scaleHandlePrefab = null;

        /// <summary>
        /// Prefab used to display scale handles in corners. If not set, boxes will be displayed instead
        /// </summary>
        public GameObject ScaleHandlePrefab
        {
            get { return scaleHandlePrefab; }
            set
            {
                if (scaleHandlePrefab != value)
                {
                    scaleHandlePrefab = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display scale handles in corners for 2D slate. If not set, boxes will be displayed instead")]
        GameObject scaleHandleSlatePrefab = null;

        /// <summary>
        /// Prefab used to display scale handles in corners for 2D slate. If not set, boxes will be displayed instead
        /// </summary>
        public GameObject ScaleHandleSlatePrefab
        {
            get { return scaleHandleSlatePrefab; }
            set
            {
                if (scaleHandleSlatePrefab != value)
                {
                    scaleHandleSlatePrefab = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [FormerlySerializedAs("cornerRadius")]
        [Tooltip("Size of the cube collidable used in scale handles")]
        private float scaleHandleSize = 0.016f; // 1.6cm default handle size

        /// <summary>
        /// Size of the cube collidable used in scale handles
        /// </summary>
        public float ScaleHandleSize
        {
            get { return scaleHandleSize; }
            set
            {
                if (scaleHandleSize != value)
                {
                    scaleHandleSize = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Additional padding to apply to the collider on scale handle to make handle easier to hit")]
        private Vector3 scaleHandleColliderPadding = new Vector3(0.016f, 0.016f, 0.016f);

        /// <summary>
        /// Additional padding to apply to the collider on scale handle to make handle easier to hit
        /// </summary>
        public Vector3 ScaleHandleColliderPadding
        {
            get { return scaleHandleColliderPadding; }
            set
            {
                if (scaleHandleColliderPadding != value)
                {
                    scaleHandleColliderPadding = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display rotation handles in the midpoint of each edge. Aligns the Y axis of the prefab with the pivot axis, and the X and Z axes pointing outward. If not set, spheres will be displayed instead")]
        private GameObject rotationHandlePrefab = null;

        /// <summary>
        /// Prefab used to display rotation handles in the midpoint of each edge. Aligns the Y axis of the prefab with the pivot axis, and the X and Z axes pointing outward. If not set, spheres will be displayed instead
        /// </summary>
        public GameObject RotationHandleSlatePrefab
        {
            get { return rotationHandlePrefab; }
            set
            {
                if (rotationHandlePrefab != value)
                {
                    rotationHandlePrefab = value;
                    CreateRig();
                }
            }
        }
        [SerializeField]
        [FormerlySerializedAs("ballRadius")]
        [Tooltip("Radius of the handle geometry of rotation handles")]
        private float rotationHandleSize = 0.016f; // 1.6cm default handle size

        /// <summary>
        /// Radius of the handle geometry of rotation handles
        /// </summary>
        public float RotationHandleSize
        {
            get { return rotationHandleSize; }
            set
            {
                if (rotationHandleSize != value)
                {
                    rotationHandleSize = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Additional padding to apply to the collider on rotate handle to make handle easier to hit")]
        private Vector3 rotateHandleColliderPadding = new Vector3(0.016f, 0.016f, 0.016f);

        /// <summary>
        /// Additional padding to apply to the collider on rotate handle to make handle easier to hit
        /// </summary>
        public Vector3 RotateHandleColliderPadding
        {
            get { return rotateHandleColliderPadding; }
            set
            {
                if (rotateHandleColliderPadding != value)
                {
                    rotateHandleColliderPadding = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Determines the type of collider that will surround the rotation handle prefab.")]
        private RotationHandlePrefabCollider rotationHandlePrefabColliderType = RotationHandlePrefabCollider.Box;

        /// <summary>
        /// Determines the type of collider that will surround the rotation handle prefab.
        /// </summary>
        public RotationHandlePrefabCollider RotationHandlePrefabColliderType
        {
            get
            {
                return rotationHandlePrefabColliderType;
            }
            set
            {
                if (rotationHandlePrefabColliderType != value)
                {
                    rotationHandlePrefabColliderType = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show scale handles")]
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
        [Tooltip("Check to show rotation handles for the X axis")]
        private bool showRotationHandleForX = true;

        /// <summary>
        /// Check to show rotation handles for the X axis
        /// </summary>
        public bool ShowRotationHandleForX
        {
            get
            {
                return showRotationHandleForX;
            }
            set
            {
                if (showRotationHandleForX != value)
                {
                    showRotationHandleForX = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the Y axis")]
        private bool showRotationHandleForY = true;

        /// <summary>
        /// Check to show rotation handles for the Y axis
        /// </summary>
        public bool ShowRotationHandleForY
        {
            get
            {
                return showRotationHandleForY;
            }
            set
            {
                if (showRotationHandleForY != value)
                {
                    showRotationHandleForY = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the Z axis")]
        private bool showRotationHandleForZ = true;

        /// <summary>
        /// Check to show rotation handles for the Z axis
        /// </summary>
        public bool ShowRotationHandleForZ
        {
            get
            {
                return showRotationHandleForZ;
            }
            set
            {
                if (showRotationHandleForZ != value)
                {
                    showRotationHandleForZ = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to draw a tether point from the handles to the hand when manipulating.")]
        private bool drawTetherWhenManipulating = true;

        /// <summary>
        /// Check to draw a tether point from the handles to the hand when manipulating.
        /// </summary>
        public bool DrawTetherWhenManipulating
        {
            get
            {
                return drawTetherWhenManipulating;
            }
            set
            {
                drawTetherWhenManipulating = value;
            }
        }

        [Header("Proximity")]
        [SerializeField]
        [Tooltip("Determines whether proximity feature (scaling and material toggling) for bounding box handles is activated")]
        private bool proximityEffectActive = false;

        /// <summary>
        /// Determines whether proximity feature (scaling and material toggling) for bounding box handles is activated
        /// </summary>
        public bool ProximityEffectActive
        {
            get
            {
                return proximityEffectActive;
            }
            set
            {
                proximityEffectActive = value;
            }
        }

        [SerializeField]
        [Tooltip("How far away should the hand be from a handle before it starts scaling the handle?")]
        [Range(0.005f, 0.2f)]
        private float handleMediumProximity = 0.1f;

        [SerializeField]
        [Tooltip("How far away should the hand be from a handle before it activates the close-proximity scaling effect?")]
        [Range(0.001f, 0.1f)]
        private float handleCloseProximity = 0.03f;

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves out of range. Default is 0, invisible handle.")]
        private float farScale = 0.0f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves out of range. Default is 0, invisible handle.
        /// </summary>
        public float FarScale
        {
            get
            {
                return farScale;
            }
            set
            {
                farScale = value;
            }
        }

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original handle size.")]
        private float mediumScale = 1.0f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original handle size.
        /// </summary>
        public float MediumScale
        {
            get
            {
                return mediumScale;
            }
            set
            {
                mediumScale = value;
            }
        }

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger handle size.")]
        private float closeScale = 1.5f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger handle size
        /// </summary>
        public float CloseScale
        {
            get
            {
                return closeScale;
            }
            set
            {
                closeScale = value;
            }
        }

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves from Medium proximity to Far proximity?")]
        [Range(0.0f, 1.0f)]
        private float farGrowRate = 0.3f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves to a distance that activates Medium Scale ?")]
        [Range(0.0f, 1.0f)]
        private float mediumGrowRate = 0.2f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves to a distance that activates Close Scale ?")]
        [Range(0.0f, 1.0f)]
        private float closeGrowRate = 0.3f;

        [SerializeField]
        [Tooltip("Add a Collider here if you do not want the handle colliders to interact with another object's collider.")]
        private Collider handlesIgnoreCollider = null;

        /// <summary>
        /// Add a Collider here if you do not want the handle colliders to interact with another object's collider.
        /// </summary>
        public Collider HandlesIgnoreCollider
        {
            get
            {
                return handlesIgnoreCollider;
            }
            set
            {
                handlesIgnoreCollider = value;
            }
        }

        [Header("Debug")]
        [Tooltip("Debug only. Component used to display debug messages")]
        public TextMesh debugText;

        [SerializeField]
        [Tooltip("Determines whether to hide GameObjects (i.e handles, links etc) created and managed by this component in the editor")]
        private bool hideElementsInInspector = true;

        /// <summary>
        /// Determines whether to hide GameObjects (i.e handles, links etc) created and managed by this component in the editor
        /// </summary>
        public bool HideElementsInInspector
        {
            get { return hideElementsInInspector; }
            set
            {
                if (hideElementsInInspector != value)
                {
                    hideElementsInInspector = value;
                    UpdateRigVisibilityInInspector();
                }
            }
        }

        

        [Header("Events")]
        public UnityEvent RotateStarted = new UnityEvent();
        public UnityEvent RotateStopped = new UnityEvent();
        public UnityEvent ScaleStarted = new UnityEvent();
        public UnityEvent ScaleStopped = new UnityEvent();
        #endregion Serialized Fields

        #region Private Fields

        // Whether we should be displaying just the wireframe (if enabled) or the handles too
        private bool wireframeOnly = false;

        // Pointer that is being used to manipulate the bounding box
        private IMixedRealityPointer currentPointer;

        private Transform rigRoot;

        // Game object used to display the bounding box. Parented to the rig root
        private GameObject boxDisplay;

        

        // Half the size of the current bounds
        private Vector3 currentBoundsExtents;

        private readonly List<IMixedRealityInputSource> touchingSources = new List<IMixedRealityInputSource>();

        
        private List<IMixedRealityController> sourcesDetected;
        

        // Current axis of rotation about the center of the rig root
        private Vector3 currentRotationAxis;

        // Scale of the target at the beginning of the current manipulation
        private Vector3 initialScaleOnGrabStart;

        // Position of the target at the beginning of the current manipulation
        private Vector3 initialPositionOnGrabStart;

        // Point that was initially grabbed in OnPointerDown()
        private Vector3 initialGrabPoint;

        // Current position of the grab point
        private Vector3 currentGrabPoint;

        private TransformScaleHandler scaleHandler;

        // Grab point position in pointer space. Used to calculate the current grab point from the current pointer pose.
        private Vector3 grabPointInPointer;

        private CardinalAxisType[] edgeAxes;
        private int[] flattenedHandles;

        // Corner opposite to the grabbed one. Scaling will be relative to it.
        private Vector3 oppositeCorner;

        // Direction of the diagonal from the opposite corner to the grabbed one.
        private Vector3 diagonalDir;

        private HandleType currentHandleType;

        // The size, position of boundsOverride object in the previous frame
        // Used to determine if boundsOverride size has changed.
        private Bounds prevBoundsOverride = new Bounds();

        // True if this game object is a child of the Target one
        private bool isChildOfTarget = false;
        private static readonly string rigRootName = "rigRoot";

        // Cache for the corner points of either renderers or colliders during the bounds calculation phase
        private static List<Vector3> totalBoundsCorners = new List<Vector3>();

        private HashSet<IMixedRealityPointer> proximityPointers = new HashSet<IMixedRealityPointer>();
        private List<Vector3> proximityPoints = new List<Vector3>();

        #endregion

        #region public Properties
        // TODO Review this, it feels like we should be using Behaviour.enabled instead.
        private bool active = false;
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                if (active != value)
                {
                    active = value;
                    rigRoot?.gameObject.SetActive(value);
                    ResetHandleVisibility();

                    if (value && proximityEffectActive)
                    {
                        ResetHandleProximityScale();
                    }
                }
            }
        }

       

        

        #endregion Public Properties


        #region Public Methods

        private BoundingBoxTarget target;
        public void Awake()
        {
            target = new BoundingBoxTarget(targetObject ? targetObject : gameObject);
        }

        

        /// <summary>
        /// Sets the minimum/maximum scale for the bounding box at runtime.
        /// </summary>
        /// <param name="min">Minimum scale</param>
        /// <param name="max">Maximum scale</param>
        /// <param name="relativeToInitialState">If true the values will be multiplied by scale of target at startup. If false they will be in absolute local scale.</param>
        [Obsolete("Use a TransformScaleHandler script rather than setting min/max scale on BoundingBox directly")]
        public void SetScaleLimits(float min, float max, bool relativeToInitialState = true)
        {
            scaleMinimum = min;
            scaleMaximum = max;
        }

        

        #endregion


        #region MonoBehaviour Methods

        private void OnEnable()
        {
            CreateRig();
            CaptureInitialState();

            if (activation == BoundingBoxActivationType.ActivateByProximityAndPointer ||
                activation == BoundingBoxActivationType.ActivateByProximity ||
                activation == BoundingBoxActivationType.ActivateByPointer)
            {
                wireframeOnly = true;
                Active = true;
            }
            else if (activation == BoundingBoxActivationType.ActivateOnStart)
            {
                Active = true;
            }
            else if (activation == BoundingBoxActivationType.ActivateManually)
            {
                //activate to create handles etc. then deactivate. 
                Active = true;
                Active = false;
            }
        }

        private void OnDisable()
        {
            DestroyRig();
        }

        private void Update()
        {
            if (active)
            {
                if (currentPointer != null)
                {
                    target.TransformTarget(currentHandleType);
                    UpdateBounds();
                    UpdateRigHandles();
                }
                else if (!isChildOfTarget && target.HasChanged())
                {
                    UpdateBounds();
                    UpdateRigHandles();
                    target.ClearChanged();
                }


                // Only update proximity scaling of handles if they are visible which is when
                // active is true and wireframeOnly is false
                if (proximityEffectActive && !wireframeOnly)
                {
                    // If any handle type is visible, then update
                    if (ShowScaleHandles || ShowRotationHandleForX || ShowRotationHandleForY || ShowRotationHandleForZ)
                    {
                        HandleProximityScaling();
                    }
                }
            }
            else if (boundsOverride != null && HasBoundsOverrideChanged())
            {
                UpdateBounds();
                UpdateRigHandles();
            }
        }

        /// <summary>
        /// Assumes that boundsOverride is not null
        /// Returns true if the size / location of boundsOverride has changed.
        /// If boundsOverride gets set to null, rig is re-created in BoundsOverride
        /// property setter.
        /// </summary>
        private bool HasBoundsOverrideChanged()
        {
            Debug.Assert(boundsOverride != null, "HasBoundsOverrideChanged called but boundsOverride is null");
            Bounds curBounds = boundsOverride.bounds;
            bool result = curBounds != prevBoundsOverride;
            prevBoundsOverride = curBounds;
            return result;
        }

        #endregion MonoBehaviour Methods


        #region Private Methods

        

       

        private Vector3 GetRotationAxis(Transform handle)
        {
            for (int i = 0; i < balls.Count; ++i)
            {
                if (handle == balls[i])
                {
                    if (edgeAxes[i] == CardinalAxisType.X)
                    {
                        return rigRoot.transform.right;
                    }
                    else if (edgeAxes[i] == CardinalAxisType.Y)
                    {
                        return rigRoot.transform.up;
                    }
                    else
                    {
                        return rigRoot.transform.forward;
                    }
                }
            }

            return Vector3.zero;
        }

        private void AddCorners()
        {
            bool isFlattened = (flattenAxis != FlattenModeType.DoNotFlatten);

            for (int i = 0; i < boundsCorners.Length; ++i)
            {
                GameObject corner = new GameObject
                {
                    name = "corner_" + i.ToString()
                };
                corner.transform.parent = rigRoot.transform;
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
                GameObject prefabToInstantiate = isFlattened ? scaleHandleSlatePrefab : scaleHandlePrefab;
                GameObject cornerVisual = null;

                if (prefabToInstantiate == null)
                {
                    // instantiate default prefab, a cube. Remove the box collider from it
                    cornerVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornerVisual.transform.parent = visualsScale.transform;
                    cornerVisual.transform.localPosition = Vector3.zero;
                    Destroy(cornerVisual.GetComponent<BoxCollider>());
                }
                else
                {
                    cornerVisual = Instantiate(prefabToInstantiate, visualsScale.transform);
                }

                if (isFlattened)
                {
                    // Rotate 2D slate handle asset for proper orientation
                    cornerVisual.transform.Rotate(0, 0, -90);
                }

                cornerVisual.name = "visuals";

                // this is the size of the corner visuals
                var cornerbounds = GetMaxBounds(cornerVisual);
                float maxDim = Mathf.Max(Mathf.Max(cornerbounds.size.x, cornerbounds.size.y), cornerbounds.size.z);
                cornerbounds.size = maxDim * Vector3.one;

                // we need to multiply by this amount to get to desired scale handle size
                var invScale = scaleHandleSize / cornerbounds.size.x;
                cornerVisual.transform.localScale = new Vector3(invScale, invScale, invScale);

                ApplyMaterialToAllRenderers(cornerVisual, handleMaterial);

                AddComponentsToAffordance(corner, new Bounds(cornerbounds.center * invScale, cornerbounds.size * invScale), RotationHandlePrefabCollider.Box, CursorContextInfo.CursorAction.Scale, scaleHandleColliderPadding);
                corners.Add(corner.transform);

                handles.Add(new Handle()
                {
                    Type = HandleType.Scale,
                    HandleVisual = cornerVisual.transform,
                    HandleVisualRenderer = cornerVisual.GetComponentInChildren<Renderer>(),
                });
            }
        }

        /// <summary>
        /// Add all common components to a corner or rotate affordance
        /// </summary>
        /// <param name="afford"></param>
        /// <param name="bounds"></param>
        private void AddComponentsToAffordance(GameObject afford, Bounds bounds, RotationHandlePrefabCollider colliderType, CursorContextInfo.CursorAction cursorType, Vector3 colliderPadding)
        {
            if (colliderType == RotationHandlePrefabCollider.Box)
            {
                BoxCollider collider = afford.AddComponent<BoxCollider>();
                collider.size = bounds.size;
                collider.center = bounds.center;
                collider.size += colliderPadding;
            }
            else
            {
                SphereCollider sphere = afford.AddComponent<SphereCollider>();
                sphere.center = bounds.center;
                sphere.radius = bounds.extents.x;
                sphere.radius += Mathf.Max(Mathf.Max(colliderPadding.x, colliderPadding.y), colliderPadding.z);
            }

            // In order for the affordance to be grabbed using near interaction we need
            // to add NearInteractionGrabbable;
            var g = afford.EnsureComponent<NearInteractionGrabbable>();
            g.ShowTetherWhenManipulating = drawTetherWhenManipulating;

            var contextInfo = afford.EnsureComponent<CursorContextInfo>();
            contextInfo.CurrentCursorAction = cursorType;
            contextInfo.ObjectCenter = rigRoot.transform;
        }

        private Bounds GetMaxBounds(GameObject g)
        {
            var b = new Bounds();
            Mesh currentMesh;
            foreach (MeshFilter r in g.GetComponentsInChildren<MeshFilter>())
            {
                if ((currentMesh = r.sharedMesh) == null) { continue; }

                if (b.size == Vector3.zero)
                {
                    b = currentMesh.bounds;
                }
                else
                {
                    b.Encapsulate(currentMesh.bounds);
                }
            }
            return b;
        }

        private void AddLinks()
        {
            edgeCenters = new Vector3[12];

            CalculateEdgeCenters();

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
                GameObject midpoint = new GameObject();
                midpoint.name = "midpoint_" + i.ToString();
                midpoint.transform.position = edgeCenters[i];
                midpoint.transform.parent = rigRoot.transform;

                GameObject midpointVisual;
                if (rotationHandlePrefab != null)
                {
                    midpointVisual = Instantiate(rotationHandlePrefab);
                }
                else
                {
                    midpointVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Destroy(midpointVisual.GetComponent<SphereCollider>());
                }

                // Align handle with its edge assuming that the prefab is initially aligned with the up direction 
                if (edgeAxes[i] == CardinalAxisType.X)
                {
                    Quaternion realignment = Quaternion.FromToRotation(Vector3.up, Vector3.right);
                    midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
                }
                else if (edgeAxes[i] == CardinalAxisType.Z)
                {
                    Quaternion realignment = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                    midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
                }

                Bounds midpointBounds = GetMaxBounds(midpointVisual);
                float maxDim = Mathf.Max(
                    Mathf.Max(midpointBounds.size.x, midpointBounds.size.y),
                    midpointBounds.size.z);
                float invScale = rotationHandleSize / maxDim;

                midpointVisual.transform.parent = midpoint.transform;
                midpointVisual.transform.localScale = new Vector3(invScale, invScale, invScale);
                midpointVisual.transform.localPosition = Vector3.zero;

                AddComponentsToAffordance(midpoint, new Bounds(midpointBounds.center * invScale, midpointBounds.size * invScale), rotationHandlePrefabColliderType, CursorContextInfo.CursorAction.Rotate, rotateHandleColliderPadding);

                balls.Add(midpoint.transform);

                handles.Add(new Handle()
                {
                    Type = HandleType.Rotation,
                    HandleVisual = midpointVisual.transform,
                    HandleVisualRenderer = midpointVisual.GetComponent<Renderer>(),
                });

                if (handleMaterial != null)
                {
                    ApplyMaterialToAllRenderers(midpointVisual, handleMaterial);
                }
            }

            if (links != null)
            {
                GameObject link;
                for (int i = 0; i < edgeCenters.Length; ++i)
                {
                    if (wireframeShape == WireframeType.Cubic)
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(link.GetComponent<BoxCollider>());
                    }
                    else
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        Destroy(link.GetComponent<CapsuleCollider>());
                    }
                    link.name = "link_" + i.ToString();


                    Vector3 linkDimensions = GetLinkDimensions();
                    if (edgeAxes[i] == CardinalAxisType.Y)
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
                    }
                    else if (edgeAxes[i] == CardinalAxisType.Z)
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.z, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                    }
                    else//X
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.x, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
                    }

                    link.transform.position = edgeCenters[i];
                    link.transform.parent = rigRoot.transform;
                    Renderer linkRenderer = link.GetComponent<Renderer>();
                    linkRenderers.Add(linkRenderer);

                    if (wireframeMaterial != null)
                    {
                        linkRenderer.material = wireframeMaterial;
                    }

                    links.Add(link.transform);
                }
            }
        }

       

        private void AddBoxDisplay()
        {
            if (boxMaterial != null)
            {
                bool isFlattened = flattenAxis != FlattenModeType.DoNotFlatten;

                boxDisplay = GameObject.CreatePrimitive(isFlattened ? PrimitiveType.Quad : PrimitiveType.Cube);
                Destroy(boxDisplay.GetComponent<Collider>());
                boxDisplay.name = "bounding box";

                ApplyMaterialToAllRenderers(boxDisplay, boxMaterial);

                boxDisplay.transform.localScale = GetBoxDisplayScale();
                boxDisplay.transform.parent = rigRoot.transform;
            }
        }

        private Vector3 GetBoxDisplayScale()
        {
            // When a box is flattened one axis is normally scaled to zero, this doesn't always work well with visuals so we take 
            // that flattened axis and re-scale it to the flattenAxisDisplayScale.
            Vector3 displayScale = currentBoundsExtents;
            displayScale.x = (flattenAxis == FlattenModeType.FlattenX) ? flattenAxisDisplayScale : displayScale.x;
            displayScale.y = (flattenAxis == FlattenModeType.FlattenY) ? flattenAxisDisplayScale : displayScale.y;
            displayScale.z = (flattenAxis == FlattenModeType.FlattenZ) ? flattenAxisDisplayScale : displayScale.z;

            return 2.0f * displayScale;
        }

        private void SetBoundingBoxCollider()
        {
            // Make sure that the bounds of all child objects are up to date before we compute bounds
            UnityPhysics.SyncTransforms();

            if (boundsOverride != null)
            {
                TargetBounds = boundsOverride;
                TargetBounds.transform.hasChanged = true;
            }
            else
            {
                Bounds bounds = GetTargetBounds();
                TargetBounds = Target.AddComponent<BoxCollider>();

                TargetBounds.center = bounds.center;
                TargetBounds.size = bounds.size;
            }

            CalculateBoxPadding();

            TargetBounds.EnsureComponent<NearInteractionGrabbable>();
        }

        private void CalculateBoxPadding()
        {
            if (boxPadding == Vector3.zero) { return; }

            Vector3 scale = TargetBounds.transform.lossyScale;

            for (int i = 0; i < 3; i++)
            {
                if (scale[i] == 0f) { return; }

                scale[i] = 1f / scale[i];
            }

            TargetBounds.size += Vector3.Scale(boxPadding, scale);
        }

        private Bounds GetTargetBounds()
        {
            KeyValuePair<Transform, Collider> colliderByTransform;
            KeyValuePair<Transform, Bounds> rendererBoundsByTransform;
            totalBoundsCorners.Clear();

            // Collect all Transforms except for the rigRoot(s) transform structure(s)
            // Its possible we have two rigRoots here, the one about to be deleted and the new one
            // Since those have the gizmo structure childed, be need to ommit them completely in the calculation of the bounds
            // This can only happen by name unless there is a better idea of tracking the rigRoot that needs destruction

            List<Transform> childTransforms = new List<Transform>();
            childTransforms.Add(Target.transform);

            foreach (Transform childTransform in Target.transform)
            {
                if (childTransform.name.Equals(rigRootName)) { continue; }
                childTransforms.AddRange(childTransform.GetComponentsInChildren<Transform>());
            }

            // Iterate transforms and collect bound volumes

            foreach (Transform childTransform in childTransforms)
            {
                Debug.Assert(childTransform != rigRoot);

                if (boundsCalculationMethod != BoundsCalculationMethod.RendererOnly)
                {
                    Collider collider = childTransform.GetComponent<Collider>();
                    if (collider != null)
                    {
                        colliderByTransform = new KeyValuePair<Transform, Collider>(childTransform, collider);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
                {
                    MeshFilter meshFilter = childTransform.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>(childTransform, meshFilter.sharedMesh.bounds);
                    }
                    else
                    {
                        continue;
                    }
                }

                // Encapsulate the collider bounds if criteria match

                if (boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly ||
                    boundsCalculationMethod == BoundsCalculationMethod.ColliderOverRenderer)
                {
                    AddColliderBoundsToTarget(colliderByTransform);
                    if (boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly) { continue; }
                }

                // Encapsulate the renderer bounds if criteria match

                if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
                {
                    AddRendererBoundsToTarget(rendererBoundsByTransform);
                    if (boundsCalculationMethod == BoundsCalculationMethod.RendererOnly) { continue; }
                }

                // Do the collider for the one case that we chose RendererOverCollider and did not find a renderer
                AddColliderBoundsToTarget(colliderByTransform);
            }

            if (totalBoundsCorners.Count == 0) { return new Bounds(); }

            Transform targetTransform = Target.transform;

            Bounds finalBounds = new Bounds(targetTransform.InverseTransformPoint(totalBoundsCorners[0]), Vector3.zero);

            for (int i = 1; i < totalBoundsCorners.Count; i++)
            {
                finalBounds.Encapsulate(targetTransform.InverseTransformPoint(totalBoundsCorners[i]));
            }

            return finalBounds;
        }

        private void AddRendererBoundsToTarget(KeyValuePair<Transform, Bounds> rendererBoundsByTarget)
        {
            Vector3[] cornersToWorld = null;
            rendererBoundsByTarget.Value.GetCornerPositions(rendererBoundsByTarget.Key, ref cornersToWorld);
            totalBoundsCorners.AddRange(cornersToWorld);
        }

        private void AddColliderBoundsToTarget(KeyValuePair<Transform, Collider> colliderByTransform)
        {
            BoundsExtensions.GetColliderBoundsPoints(colliderByTransform.Value, totalBoundsCorners, 0);
        }

        

      

        

        private void CaptureInitialState()
        {
            var target = Target;
            if (target != null)
            {
                isChildOfTarget = transform.IsChildOf(target.transform);

                scaleHandler = GetComponent<TransformScaleHandler>();
                if (scaleHandler == null)
                {
                    scaleHandler = gameObject.AddComponent<TransformScaleHandler>();

                    scaleHandler.TargetTransform = Target.transform;
                #pragma warning disable 0618
                    scaleHandler.ScaleMinimum = scaleMinimum;
                    scaleHandler.ScaleMaximum = scaleMaximum;
                #pragma warning restore 0618
                }
            }
        }

        private Vector3 GetLinkDimensions()
        {
            float linkLengthAdjustor = wireframeShape == WireframeType.Cubic ? 2.0f : 1.0f - (6.0f * wireframeEdgeRadius);
            return (currentBoundsExtents * linkLengthAdjustor) + new Vector3(wireframeEdgeRadius, wireframeEdgeRadius, wireframeEdgeRadius);
        }

        private bool ShouldRotateHandleBeVisible(CardinalAxisType axisType)
        {
            return
                (axisType == CardinalAxisType.X && showRotationHandleForX) ||
                (axisType == CardinalAxisType.Y && showRotationHandleForY) ||
                (axisType == CardinalAxisType.Z && showRotationHandleForZ);
        }

       

        

        private void UpdateBounds()
        {
            if (TargetBounds != null)
            {
                // Store current rotation then zero out the rotation so that the bounds
                // are computed when the object is in its 'axis aligned orientation'.
                Quaternion currentRotation = Target.transform.rotation;
                Target.transform.rotation = Quaternion.identity;
                UnityPhysics.SyncTransforms(); // Update collider bounds

                Vector3 boundsExtents = TargetBounds.bounds.extents;

                // After bounds are computed, restore rotation...
                Target.transform.rotation = currentRotation;
                UnityPhysics.SyncTransforms();

                if (boundsExtents != Vector3.zero)
                {
                    if (flattenAxis == FlattenModeType.FlattenAuto)
                    {
                        float min = Mathf.Min(boundsExtents.x, Mathf.Min(boundsExtents.y, boundsExtents.z));
                        flattenAxis = (min == boundsExtents.x) ? FlattenModeType.FlattenX :
                            ((min == boundsExtents.y) ? FlattenModeType.FlattenY : FlattenModeType.FlattenZ);
                    }

                    boundsExtents.x = (flattenAxis == FlattenModeType.FlattenX) ? 0.0f : boundsExtents.x;
                    boundsExtents.y = (flattenAxis == FlattenModeType.FlattenY) ? 0.0f : boundsExtents.y;
                    boundsExtents.z = (flattenAxis == FlattenModeType.FlattenZ) ? 0.0f : boundsExtents.z;
                    currentBoundsExtents = boundsExtents;

                    GetCornerPositionsFromBounds(new Bounds(Vector3.zero, boundsExtents * 2.0f), ref boundsCorners);
                    CalculateEdgeCenters();
                }
            }
        }

        
       


       

        /// <summary>
        /// Determine if passed point is within sphere of radius around this GameObject
        /// To avoid function overhead, request compiler to inline this function since repeatedly called every Update() for every pointer position and result
        /// </summary>
        /// <param name="point">world space position</param>
        /// <param name="radiusSqr">radius of sphere in distance squared for faster comparison</param>
        /// <returns>true if point is within sphere</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPointWithinBounds(Vector3 point, float radiusSqr)
        {
            return (transform.position - point).sqrMagnitude < radiusSqr;
        }



        

        private void GetCornerPositionsFromBounds(Bounds bounds, ref Vector3[] positions)
        {
            int numCorners = 1 << 3;
            if (positions == null || positions.Length != numCorners)
            {
                positions = new Vector3[numCorners];
            }

            // Permutate all axes using minCorner and maxCorner.
            Vector3 minCorner = bounds.center - bounds.extents;
            Vector3 maxCorner = bounds.center + bounds.extents;
            for (int c = 0; c < numCorners; c++)
            {
                positions[c] = new Vector3(
                    (c & (1 << 0)) == 0 ? minCorner[0] : maxCorner[0],
                    (c & (1 << 1)) == 0 ? minCorner[1] : maxCorner[1],
                    (c & (1 << 2)) == 0 ? minCorner[2] : maxCorner[2]);
            }
        }

        

        private bool DoesActivationMatchFocus(FocusEventData eventData)
        {
            switch (activation)
            {
                case BoundingBoxActivationType.ActivateOnStart:
                case BoundingBoxActivationType.ActivateManually:
                    return false;
                case BoundingBoxActivationType.ActivateByProximity:
                    return eventData.Pointer is IMixedRealityNearPointer;
                case BoundingBoxActivationType.ActivateByPointer:
                    return eventData.Pointer is IMixedRealityPointer;
                case BoundingBoxActivationType.ActivateByProximityAndPointer:
                    return true;
                default:
                    return false;
            }
        }

        private void DropController()
        {
            HandleType lastHandleType = currentHandleType;
            currentPointer = null;
            currentHandleType = HandleType.None;
            ResetHandleVisibility();

            if (lastHandleType == HandleType.Scale)
            {
                if (debugText != null) debugText.text = "OnPointerUp:ScaleStopped";
                ScaleStopped?.Invoke();
            }
            else if (lastHandleType == HandleType.Rotation)
            {
                if (debugText != null) debugText.text = "OnPointerUp:RotateStopped";
                RotateStopped?.Invoke();
            }
        }

        #endregion Private Methods


        #region Used Event Handlers

        void IMixedRealityFocusChangedHandler.OnFocusChanged(FocusEventData eventData)
        {
            if (proximityEffectActive && eventData.NewFocusedObject == null)
            {
                ResetHandleProximityScale();
            }

            if (activation == BoundingBoxActivationType.ActivateManually || activation == BoundingBoxActivationType.ActivateOnStart)
            {
                return;
            }

            if (!DoesActivationMatchFocus(eventData))
            {
                return;
            }

            bool handInProximity = eventData.NewFocusedObject != null && eventData.NewFocusedObject.transform.IsChildOf(transform);
            if (handInProximity == wireframeOnly)
            {
                wireframeOnly = !handInProximity;
                ResetHandleVisibility();
            }
        }

        void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
        {
            if (currentPointer != null && eventData.Pointer == currentPointer)
            {
                DropController();
            }
        }

        void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData) { }

        private void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (currentPointer != null && eventData.Pointer == currentPointer)
            {
                DropController();
                eventData.Use();
            }
        }

        private void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (currentPointer == null && !eventData.used)
            {
                GameObject grabbedHandle = eventData.Pointer.Result.CurrentPointerTarget;
                Transform grabbedHandleTransform = grabbedHandle.transform;
                currentHandleType = GetHandleType(grabbedHandleTransform);
                if (currentHandleType != HandleType.None)
                {
                    currentPointer = eventData.Pointer;
                    initialGrabPoint = currentPointer.Result.Details.Point;
                    currentGrabPoint = initialGrabPoint;
                    initialScaleOnGrabStart = Target.transform.localScale;
                    initialPositionOnGrabStart = Target.transform.position;
                    grabPointInPointer = Quaternion.Inverse(eventData.Pointer.Rotation) * (initialGrabPoint - currentPointer.Position);

                    SetHighlighted(grabbedHandleTransform);

                    if (currentHandleType == HandleType.Scale)
                    {
                        // Will use this to scale the target relative to the opposite corner
                        oppositeCorner = rigRoot.transform.TransformPoint(-grabbedHandle.transform.localPosition);
                        diagonalDir = (grabbedHandle.transform.position - oppositeCorner).normalized;

                        ScaleStarted?.Invoke();

                        if (debugText != null)
                        {
                            debugText.text = "OnPointerDown:ScaleStarted";
                        }
                    }
                    else if (currentHandleType == HandleType.Rotation)
                    {
                        currentRotationAxis = GetRotationAxis(grabbedHandleTransform);

                        RotateStarted?.Invoke();

                        if (debugText != null)
                        {
                            debugText.text = "OnPointerDown:RotateStarted";
                        }
                    }

                    eventData.Use();
                }
            }

            if (currentPointer != null)
            {
                // Always mark the pointer data as used to prevent any other behavior to handle pointer events
                // as long as BoundingBox manipulation is active.
                // This is due to us reacting to both "Select" and "Grip" events.
                eventData.Use();
            }
        }

        private void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            if (eventData.Controller != null)
            {
                if (sourcesDetected.Count == 0 || sourcesDetected.Contains(eventData.Controller) == false)
                {
                    sourcesDetected.Add(eventData.Controller);
                }
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            sourcesDetected.Remove(eventData.Controller);

            if (currentPointer != null && currentPointer.InputSourceParent.SourceId == eventData.SourceId)
            {
                HandleType lastHandleType = currentHandleType;

                currentPointer = null;
                currentHandleType = HandleType.None;
                ResetHandleVisibility();

                if (lastHandleType == HandleType.Scale)
                {
                    if (debugText != null) debugText.text = "OnSourceLost:ScaleStopped";
                    ScaleStopped?.Invoke();
                }
                else if (lastHandleType == HandleType.Rotation)
                {
                    if (debugText != null) debugText.text = "OnSourceLost:RotateStopped";
                    RotateStopped?.Invoke();
                }
            }
        }

        #endregion Used Event Handlers


        #region Unused Event Handlers

        void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData) { }

        #endregion Unused Event Handlers
    }
}
