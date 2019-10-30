// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityPhysics = UnityEngine.Physics;
using Microsoft.MixedReality.Toolkit.UI.Experimental.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    /// <summary>
    /// Bounds Control allows to transform objects (rotate and scale) and draws a cube around the object to visualize 
    /// the possibility of user triggered transform manipulation. 
    /// Bounds Control provides scale and rotation handles that can be used for far and near interaction manipulation
    /// of the object. It further provides a proximity effect for scale and rotation handles that alters scaling and material. 
    /// Todo: replace doc link - point to BoundsControl docs
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_BoundingBox.html")]
    public class BoundsControl : MonoBehaviour,
        IMixedRealitySourceStateHandler,
        IMixedRealityFocusChangedHandler,
        IMixedRealityFocusHandler
    {
        #region Serialized Fields and Properties
        [SerializeField]
        [Tooltip("The object that the bounds control rig will be modifying.")]
        private GameObject targetObject;
        /// <summary>
        /// The object that the bounds control rig will be modifying.
        /// </summary>
        public GameObject Target
        {
            get
            {
                if (targetObject == null)
                {
                    targetObject = gameObject;
                }

                return targetObject;
            }
        }

        [Tooltip("For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of bounds control.")]
        [SerializeField]
        [FormerlySerializedAs("BoxColliderToUse")]
        private BoxCollider boundsOverride = null;

        /// <summary>
        /// For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of bounds control.
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
        [Tooltip("Type of activation method for showing/hiding bounds control handles and controls")]
        private BoundsControlActivationType activation = BoundsControlActivationType.ActivateOnStart;

        /// <summary>
        /// Type of activation method for showing/hiding bounds control handles and controls
        /// </summary>
        public BoundsControlActivationType BoundsControlActivation
        {
            get { return activation; }
            set
            {
                if (activation != value)
                {
                    activation = value;
                    ResetVisuals();
                }
            }
        }

        [Header("Visuals")]

        [SerializeField]
        [Tooltip("Check to draw a tether point from the handles to the hand when manipulating.")]
        private bool drawTetherWhenManipulating = true;

        /// <summary>
        /// Check to draw a tether point from the handles to the hand when manipulating.
        /// </summary>
        public bool DrawTetherWhenManipulating => drawTetherWhenManipulating;

        [SerializeField]
        [Tooltip("Add a Collider here if you do not want the handle colliders to interact with another object's collider.")]
        private Collider handlesIgnoreCollider = null;

        /// <summary>
        /// Add a Collider here if you do not want the handle colliders to interact with another object's collider.
        /// </summary>
        public Collider HandlesIgnoreCollider => handlesIgnoreCollider;

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
                    Flatten(flattenAxis);
                    CreateRig();
                }
            }
        }

        private int[] flattenedHandles = null;
        private void Flatten(FlattenModeType flattenAxis)
        {
            if (flattenAxis == FlattenModeType.FlattenX)
            {
                flattenedHandles = new int[] { 0, 4, 2, 6 };
            }
            else if (flattenAxis == FlattenModeType.FlattenY)
            {
                flattenedHandles = new int[] { 1, 3, 5, 7 };
            }
            else if (flattenAxis == FlattenModeType.FlattenZ)
            {
                flattenedHandles = new int[] { 9, 10, 8, 11 };
            }
        }

        [SerializeField]
        // [FormerlySerializedAs("wireframePadding")]
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
        [Tooltip("Bounds control box display configuration section.")]
        private BoundsControlBoxDisplay boxDisplay = new BoundsControlBoxDisplay();
        /// <summary>
        /// Bounds control box display configuration section.
        /// </summary>
        public BoundsControlBoxDisplay BoxDisplay => boxDisplay;

        [SerializeField]
        [Tooltip("This section defines the links / lines that are drawn between the corners of the control.")]
        private BoundsControlLinks links = new BoundsControlLinks();
        /// <summary>
        /// This section defines the links / lines that are drawn between the corners of the control.
        /// </summary>
        public BoundsControlLinks Links => links;

        [SerializeField]
        [Tooltip("Configuration of the scale handles.")]
        private BoundsControlScaleHandles scaleHandles = new BoundsControlScaleHandles();
        /// <summary>
        /// Configuration of the scale handles.
        /// </summary>
        public BoundsControlScaleHandles ScaleHandles => scaleHandles;

        [SerializeField]
        [Tooltip("Configuration of the rotation handles.")]
        private BoundsControlRotationHandles rotationHandles = new BoundsControlRotationHandles();
        /// <summary>
        /// Configuration of the rotation handles.
        /// </summary>
        public BoundsControlRotationHandles RotationHandles => rotationHandles;

        [SerializeField]
        [Tooltip("Configuration for Proximity Effect to scale handles or change materials on proximity.")]
        private ProximityEffect proximityEffect = new ProximityEffect();
        /// <summary>
        /// Configuration for Proximity Effect to scale handles or change materials on proximity.
        /// </summary>
        public ProximityEffect ProximityEffect => proximityEffect;

        [Header("Debug")]
        [Tooltip("Debug only. Component used to display debug messages.")]
        /// <summary>
        /// Component used to display debug messages.
        /// </summary>
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
        /// <summary>
        /// Event that gets fired when interaction with a rotation handle starts.
        /// </summary>
        public UnityEvent RotateStarted = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a rotation handle stops.
        /// </summary>
        public UnityEvent RotateStopped = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a scale handle starts.
        /// </summary>
        public UnityEvent ScaleStarted = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a scale handle stops.
        /// </summary>
        public UnityEvent ScaleStopped = new UnityEvent();

        #endregion Serialized Fields

        #region Private Fields

        // Whether we should be displaying just the wireframe (if enabled) or the handles too
        private bool wireframeOnly = false;

        // Pointer that is being used to manipulate the bounds control
        private IMixedRealityPointer currentPointer;

        // parent/root game object for all bounding box visuals (like handles, edges, boxdisplay,..)
        private Transform rigRoot;

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

        private MinMaxScaleConstraint scaleConstraint;

        // Grab point position in pointer space. Used to calculate the current grab point from the current pointer pose.
        private Vector3 grabPointInPointer;

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

        //public class BoundsChangedEvent: UnityEvent<Vector3[]> {}
        //internal BoundsChangedEvent boundsUpdated = new BoundsChangedEvent();
        private UnityEvent boundsUpdated = new UnityEvent();

        private Vector3[] boundsCorners = new Vector3[8];
        public Vector3[] BoundsCorners { get; private set; }

        #endregion

        #region public Properties

        /// <summary>
        /// The collider reference tracking the bounds utilized by this component during runtime
        /// </summary>
        public BoxCollider TargetBounds { get; private set; }

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
                    ResetVisuals();

                    if (active)
                    {
                        proximityEffect.ResetProximityScale();
                    }
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Allows to manually enable wire (edge) highlighting (edges) of the bounds control.
        /// This is useful if connected to the Manipulation events of a
        /// <see cref="Microsoft.MixedReality.Toolkit.UI.ManipulationHandler"/> 
        /// when used in conjunction with this MonoBehavior.
        /// </summary>
        public void HighlightWires()
        {
            SetHighlighted(null);
        }

        public void UnhighlightWires()
        {
            ResetVisuals();
        }

        /// <summary>
        /// Destroys and re-creates the rig around the bounds control
        /// TODO: this shouldn't be called every time we change a param - be more specific with what we recreate
        /// </summary>
        public void CreateRig()
        {
            DestroyRig();
            InitializeRigRoot();
            InitializeDataStructures();
            SetBoundsControlCollider();
            UpdateBounds();
            CreateVisuals();
            ResetVisuals();
            rigRoot.gameObject.SetActive(active);
            UpdateRigVisibilityInInspector();
        }


        /// <summary>
        /// Register a transform scale handler to bounding box to limit the scaling range
        /// This is useful for adding/switching your scale handler during runtime
        /// </summary>
        /// <param name="transformScaleHandler">scale handler you want to switch to - can be null if scaling shouldn't be constrained</param>
        public void RegisterTransformScaleHandler(MinMaxScaleConstraint transformScaleHandler)
        {
            scaleConstraint = transformScaleHandler;
        }

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (targetObject == null)
                targetObject = gameObject;

            // subscribe to visual changes
            scaleHandles.configurationChanged.AddListener(CreateRig);
            scaleHandles.visibilityChanged.AddListener(ResetVisuals);
            rotationHandles.configurationChanged.AddListener(CreateRig);
            boxDisplay.configurationChanged.AddListener(CreateRig);
            links.configurationChanged.AddListener(CreateRig);

            // subscribe visuals to bounds changes
            boundsUpdated.AddListener(() => rotationHandles.CalculateEdgeCenters(ref boundsCorners));
        }

        private void OnEnable()
        {
            Flatten(flattenAxis);
            CreateRig();
            CaptureInitialState();

            if (activation == BoundsControlActivationType.ActivateByProximityAndPointer ||
                activation == BoundsControlActivationType.ActivateByProximity ||
                activation == BoundsControlActivationType.ActivateByPointer)
            {
                wireframeOnly = true;
                Active = true;
            }
            else if (activation == BoundsControlActivationType.ActivateOnStart)
            {
                Active = true;
            }
            else if (activation == BoundsControlActivationType.ActivateManually)
            {
                //activate to create handles etc. then deactivate. 
                Active = true;
                Active = false;
            }
        }

        private void OnDisable()
        {
            DestroyRig();

            if (currentPointer != null)
            {
                DropController();
            }
        }

        private void Update()
        {
            if (active)
            {
                if (currentPointer != null)
                {
                    TransformTarget(currentHandleType);
                    UpdateBounds();
                    UpdateVisuals();
                }
                else if ((!isChildOfTarget && Target.transform.hasChanged)
                    || (boundsOverride != null && HasBoundsOverrideChanged()))
                {
                    UpdateBounds();
                    UpdateVisuals();
                    Target.transform.hasChanged = false;
                }


                // Only update proximity scaling of handles if they are visible which is when
                // active is true and wireframeOnly is false
                // also only use proximity effect if nothing is being dragged or grabbed
                if (!wireframeOnly && currentPointer == null)
                {
                    proximityEffect.Update(transform.position, currentBoundsExtents);
                }
            }
        }

        #endregion MonoBehaviour Methods

        #region Private Methods

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

        private void SetBoundsControlCollider()
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
            // Since those have the gizmo structure childed, be need to omit them completely in the calculation of the bounds
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
                        colliderByTransform = new KeyValuePair<Transform, Collider>();
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
                        rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>();
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
            if (rendererBoundsByTarget.Key == null) { return; }

            Vector3[] cornersToWorld = null;
            rendererBoundsByTarget.Value.GetCornerPositions(rendererBoundsByTarget.Key, ref cornersToWorld);
            totalBoundsCorners.AddRange(cornersToWorld);
        }

        private void AddColliderBoundsToTarget(KeyValuePair<Transform, Collider> colliderByTransform)
        {
            if (colliderByTransform.Key != null)
            {
                BoundsExtensions.GetColliderBoundsPoints(colliderByTransform.Value, totalBoundsCorners, 0);
            }
        }

        private HandleType GetHandleType(Transform handle)
        {
            if (rotationHandles.IsHandleType(handle))
            {
                return rotationHandles.GetHandleType();
            }
            else if (scaleHandles.IsHandleType(handle))
            {
                return scaleHandles.GetHandleType();
            }
            else
            {
                return HandleType.None;
            }
        }

        private void CaptureInitialState()
        {
            if (Target != null)
            {
                isChildOfTarget = transform.IsChildOf(Target.transform);

                RegisterTransformScaleHandler(GetComponent<MinMaxScaleConstraint>());
            }
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
                    boundsUpdated.Invoke();
                }
            }
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
                case BoundsControlActivationType.ActivateOnStart:
                case BoundsControlActivationType.ActivateManually:
                    return false;
                case BoundsControlActivationType.ActivateByProximity:
                    return eventData.Pointer is IMixedRealityNearPointer;
                case BoundsControlActivationType.ActivateByPointer:
                    return eventData.Pointer is IMixedRealityPointer;
                case BoundsControlActivationType.ActivateByProximityAndPointer:
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
            ResetVisuals();

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

        private void DestroyRig()
        {
            if (boundsOverride == null)
            {
                Destroy(TargetBounds);
            }
            else
            {
                boundsOverride.size -= boxPadding;

                if (TargetBounds != null)
                {
                    if (TargetBounds.gameObject.GetComponent<NearInteractionGrabbable>())
                    {
                        Destroy(TargetBounds.gameObject.GetComponent<NearInteractionGrabbable>());
                    }
                }
            }

            // todo: move this out?
            DestroyVisuals();

            if (rigRoot != null)
            {
                Destroy(rigRoot.gameObject);
                rigRoot = null;
            }

        }

        private void UpdateRigVisibilityInInspector()
        {
            HideFlags desiredFlags = hideElementsInInspector ? HideFlags.HideInHierarchy | HideFlags.HideInInspector : HideFlags.None;
            scaleHandles.UpdateVisibilityInInspector(desiredFlags);
            links.UpdateVisibilityInInspector(desiredFlags);
            boxDisplay.UpdateVisibilityInInspector(desiredFlags);

            if (rigRoot != null)
            {
                rigRoot.hideFlags = desiredFlags;
            }

        }

        private Vector3 GetRotationAxis(Transform handle)
        {
            CardinalAxisType axisType = rotationHandles.GetAxisType(handle);
            if (axisType == CardinalAxisType.X)
            {
                return rigRoot.transform.right;
            }
            else if (axisType == CardinalAxisType.Y)
            {
                return rigRoot.transform.up;
            }
            else
            {
                return rigRoot.transform.forward;
            }
        }

        private void InitializeRigRoot()
        {
            var rigRootObj = new GameObject(rigRootName);
            rigRoot = rigRootObj.transform;
            rigRoot.parent = transform;

            var pH = rigRootObj.AddComponent<PointerHandler>();
            pH.OnPointerDown.AddListener(OnPointerDown);
            pH.OnPointerDragged.AddListener(OnPointerDragged);
            pH.OnPointerUp.AddListener(OnPointerUp);
        }

        private void InitializeDataStructures()
        {
            sourcesDetected = new List<IMixedRealityController>();
        }

        private void TransformTarget(HandleType transformType)
        {
            if (transformType != HandleType.None)
            {
                Vector3 prevGrabPoint = currentGrabPoint;
                currentGrabPoint = (currentPointer.Rotation * grabPointInPointer) + currentPointer.Position;

                if (transformType == HandleType.Rotation)
                {
                    Vector3 prevDir = Vector3.ProjectOnPlane(prevGrabPoint - rigRoot.transform.position, currentRotationAxis).normalized;
                    Vector3 currentDir = Vector3.ProjectOnPlane(currentGrabPoint - rigRoot.transform.position, currentRotationAxis).normalized;
                    Quaternion q = Quaternion.FromToRotation(prevDir, currentDir);
                    q.ToAngleAxis(out float angle, out Vector3 axis);

                    Target.transform.RotateAround(rigRoot.transform.position, axis, angle);
                }
                else if (transformType == HandleType.Scale)
                {
                    float initialDist = Vector3.Dot(initialGrabPoint - oppositeCorner, diagonalDir);
                    float currentDist = Vector3.Dot(currentGrabPoint - oppositeCorner, diagonalDir);
                    float scaleFactor = 1 + (currentDist - initialDist) / initialDist;

                    Vector3 newScale = initialScaleOnGrabStart * scaleFactor;

                    MixedRealityTransform clampedTransform = MixedRealityTransform.NewScale(newScale);
                    if (scaleConstraint != null)
                    {
                        scaleConstraint.ApplyConstraint(ref clampedTransform);
                        if (clampedTransform.Scale != newScale)
                        {
                            scaleFactor = clampedTransform.Scale[0] / initialScaleOnGrabStart[0];
                        }
                    }

                    Target.transform.localScale = clampedTransform.Scale;
                    Target.transform.position = initialPositionOnGrabStart * scaleFactor + (1 - scaleFactor) * oppositeCorner;
                }
            }
        }

        #endregion Private Methods

        #region Used Event Handlers

        void IMixedRealityFocusChangedHandler.OnFocusChanged(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject == null)
            {
                proximityEffect.ResetProximityScale();
            }

            if (activation == BoundsControlActivationType.ActivateManually || activation == BoundsControlActivationType.ActivateOnStart)
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
                // todo: move this out?
                ResetVisuals();
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

                    // todo: move this out?
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
                // as long as bounds control manipulation is active.
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
                // todo: move this out?
                ResetVisuals();

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


        #region BoundsControl Visuals Private Methods

        private void SetHighlighted(Transform activeHandle)
        {
            scaleHandles.SetHighlighted(activeHandle);
            rotationHandles.SetHighlighted(activeHandle);
            boxDisplay.SetHighlighted();
        }

        private void ResetVisuals()
        {
            if (currentPointer != null)
            {
                return;
            }

            //set link visibility
            links.ResetVisibility(active);
            links.Flatten(ref flattenedHandles);

            boxDisplay.ResetVisibility(active);

            bool isVisible = (active == true && wireframeOnly == false);
            //set corner visibility
            scaleHandles.ResetHandleVisibility(isVisible);
            // set rotation handle visibility
            rotationHandles.ResetHandleVisibility(isVisible);
            rotationHandles.FlattenHandles(ref flattenedHandles);
        }

        private void CreateVisuals()
        {
            // add corners
            bool isFlattened = flattenAxis != FlattenModeType.DoNotFlatten;
            scaleHandles.Create(ref boundsCorners, rigRoot, drawTetherWhenManipulating, isFlattened);
            proximityEffect.AddObjects(scaleHandles);

            // add links
            rotationHandles.Create(ref boundsCorners, rigRoot, drawTetherWhenManipulating);
            proximityEffect.AddObjects(rotationHandles);
            links.CreateLinks(rotationHandles, rigRoot, currentBoundsExtents);

            // add box display
            boxDisplay.AddBoxDisplay(rigRoot.transform, currentBoundsExtents, flattenAxis);

            // setup colliders
            // Make the handle colliders ignore specified collider. (e.g. spatial mapping's floor collider to avoid the object get lifted up)
            scaleHandles.HandleIgnoreCollider(handlesIgnoreCollider);
            rotationHandles.HandleIgnoreCollider(handlesIgnoreCollider);

            // update visuals
            UpdateVisuals();
        }

        private void DestroyVisuals()
        {
            proximityEffect.ClearObjects();
            links.Clear();
            scaleHandles.DestroyHandles();
            rotationHandles.DestroyHandles();
        }

        private void UpdateVisuals()
        {
            if (rigRoot != null && Target != null && TargetBounds != null)
            {
                // We move the rigRoot to the scene root to ensure that non-uniform scaling performed
                // anywhere above the rigRoot does not impact the position of rig corners / edges
                rigRoot.parent = null;

                rigRoot.rotation = Quaternion.identity;
                rigRoot.position = Vector3.zero;
                rigRoot.localScale = Vector3.one;

                scaleHandles.UpdateHandles(ref boundsCorners);
                rotationHandles.UpdateHandles();
                links.Update(rotationHandles, rigRoot, currentBoundsExtents);

                boxDisplay.Update(rigRoot, currentBoundsExtents, flattenAxis);

                // move rig into position and rotation
                rigRoot.position = TargetBounds.bounds.center;
                rigRoot.rotation = Target.transform.rotation;
                rigRoot.parent = transform;
            }
        }

        #endregion BoundsControl Visuals Private Methods

    }
}
