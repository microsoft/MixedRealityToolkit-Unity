// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Bounds Control allows to transform objects (rotate and scale) and draws a cube around the object to visualize 
    /// the possibility of user triggered transform manipulation. 
    /// Bounds Control provides scale and rotation handles that can be used for far and near interaction manipulation
    /// of the object. It further provides a proximity effect for scale and rotation handles that alters scaling and material. 
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/bounds-control")]
    [RequireComponent(typeof(ConstraintManager))]
    [AddComponentMenu("Scripts/MRTK/SDK/BoundsControl")]
    public class BoundsControl : MonoBehaviour,
        IMixedRealitySourceStateHandler,
        IMixedRealityFocusChangedHandler,
        IMixedRealityFocusHandler,
        IBoundsTargetProvider
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

            set
            {
                if (targetObject != value)
                {
                    targetObject = value;
                    isChildOfTarget = transform.IsChildOf(targetObject.transform);
                    // reparent rigroot
                    if (rigRoot != null)
                    {
                        rigRoot.parent = targetObject.transform;
                        UpdateBounds();
                    }
                }
            }
        }

        [Tooltip("For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of bounds control.")]
        [SerializeField]
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
                    UpdateBounds();
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
                    UpdateBounds();
                }
            }
        }

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
                    SetActivationFlags();
                    ResetVisuals();
                }
            }
        }

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
                    UpdateExtents();
                    UpdateVisuals();
                    ResetVisuals();
                }
            }
        }

        [SerializeField]
        [Tooltip("Whether scale the flattened axis when uniform scale is used.")]
        private bool uniformScaleOnFlattenedAxis = true;

        /// <summary>
        /// Whether scale the flattened axis when uniform scale is used.
        /// </summary>
        public bool UniformScaleOnFlattenedAxis
        {
            get => uniformScaleOnFlattenedAxis;
            set => uniformScaleOnFlattenedAxis = value;
        }

        [SerializeField]
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
                    UpdateBounds();
                }
            }
        }

        [SerializeField]
        [Tooltip("Bounds control box display configuration section.")]
        private BoxDisplayConfiguration boxDisplayConfiguration;
        /// <summary>
        /// Bounds control box display configuration section.
        /// </summary>
        public BoxDisplayConfiguration BoxDisplayConfig
        {
            get => boxDisplayConfiguration;
            set
            {
                boxDisplayConfiguration = value;
                boxDisplay = new BoxDisplay(boxDisplayConfiguration);
                CreateRig();
            }
        }

        [SerializeField]
        [Tooltip("This section defines the links / lines that are drawn between the corners of the control.")]
        private LinksConfiguration linksConfiguration;
        /// <summary>
        /// This section defines the links / lines that are drawn between the corners of the control.
        /// </summary>
        public LinksConfiguration LinksConfig
        {
            get => linksConfiguration;
            set
            {
                linksConfiguration = value;
                links = new Links(linksConfiguration);
                CreateRig();
            }
        }

        [SerializeField]
        [Tooltip("Configuration of the scale handles.")]
        private ScaleHandlesConfiguration scaleHandlesConfiguration;
        /// <summary>
        /// Configuration of the scale handles.
        /// </summary>
        public ScaleHandlesConfiguration ScaleHandlesConfig
        {
            get => scaleHandlesConfiguration;
            set
            {
                scaleHandlesConfiguration = value;
                scaleHandles = scaleHandlesConfiguration.ConstructInstance();
                CreateRig();
            }
        }

        [SerializeField]
        [Tooltip("Configuration of the rotation handles.")]
        private RotationHandlesConfiguration rotationHandlesConfiguration;
        /// <summary>
        /// Configuration of the rotation handles.
        /// </summary>
        public RotationHandlesConfiguration RotationHandlesConfig
        {
            get => rotationHandlesConfiguration;
            set
            {
                rotationHandlesConfiguration = value;
                rotationHandles = rotationHandlesConfiguration.ConstructInstance();
                CreateRig();
            }
        }

        [SerializeField]
        [Tooltip("Configuration of the translation handles.")]
        private TranslationHandlesConfiguration translationHandlesConfiguration;
        /// <summary>
        /// Configuration of the translation handles.
        /// </summary>
        public TranslationHandlesConfiguration TranslationHandlesConfig
        {
            get => translationHandlesConfiguration;
            set
            {
                translationHandlesConfiguration = value;
                translationHandles = translationHandlesConfiguration.ConstructInstance();
                CreateRig();
            }
        }

        [SerializeField]
        [Tooltip("Configuration for Proximity Effect to scale handles or change materials on proximity.")]
        private ProximityEffectConfiguration handleProximityEffectConfiguration;
        /// <summary>
        /// Configuration for Proximity Effect to scale handles or change materials on proximity.
        /// </summary>
        public ProximityEffectConfiguration HandleProximityEffectConfig
        {
            get => handleProximityEffectConfiguration;
            set
            {
                handleProximityEffectConfiguration = value;
                proximityEffect = new ProximityEffect(handleProximityEffectConfiguration);
                CreateRig();
            }
        }

        [Tooltip("Debug only. Component used to display debug messages.")]
        private TextMesh debugText;
        /// <summary>
        /// Component used to display debug messages.
        /// </summary>
        public TextMesh DebugText
        {
            get => debugText;
            set => debugText = value;
        }

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

        [SerializeField]
        [Tooltip("Check to enable frame-rate independent smoothing.")]
        private bool smoothingActive = false;

        /// <summary>
        /// Check to enable frame-rate independent smoothing.
        /// </summary>
        public bool SmoothingActive
        {
            get => smoothingActive;
            set => smoothingActive = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float rotateLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float RotateLerpTime
        {
            get => rotateLerpTime;
            set => rotateLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float scaleLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float ScaleLerpTime
        {
            get => scaleLerpTime;
            set => scaleLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the translation. " +
            "Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float translateLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the translation. Smoothing of 0 
        /// means no smoothing. Max value means no change to value.
        /// </summary>
        public float TranslateLerpTime
        {
            get => translateLerpTime;
            set => translateLerpTime = value;
        }

        [SerializeField]
        [Tooltip("Enable or disable constraint support of this component. When enabled, transform " +
            "changes will be post processed by the linked constraint manager.")]
        private bool enableConstraints = true;
        /// <summary>
        /// Enable or disable constraint support of this component. When enabled, transform
        /// changes will be post processed by the linked constraint manager.
        /// </summary>
        public bool EnableConstraints
        {
            get => enableConstraints;
            set => enableConstraints = value;
        }

        [SerializeField]
        [Tooltip("Constraint manager slot to enable constraints when manipulating the object.")]
        private ConstraintManager constraintsManager;
        /// <summary>
        /// Constraint manager slot to enable constraints when manipulating the object.
        /// </summary>
        public ConstraintManager ConstraintsManager
        {
            get => constraintsManager;
            set => constraintsManager = value;
        }

        [SerializeField]
        [Tooltip("Event that gets fired when interaction with a rotation handle starts.")]
        private UnityEvent rotateStarted = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a rotation handle starts.
        /// </summary>
        public UnityEvent RotateStarted
        {
            get => rotateStarted;
            set => rotateStarted = value;
        }

        [SerializeField]
        [Tooltip("Event that gets fired when interaction with a rotation handle stops.")]
        private UnityEvent rotateStopped = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a rotation handle stops.
        /// </summary>
        public UnityEvent RotateStopped
        {
            get => rotateStopped;
            set => rotateStopped = value;
        }

        [SerializeField]
        [Tooltip("Event that gets fired when interaction with a scale handle starts.")]
        private UnityEvent scaleStarted = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a scale handle starts.
        /// </summary>
        public UnityEvent ScaleStarted
        {
            get => scaleStarted;
            set => scaleStarted = value;
        }

        [SerializeField]
        [Tooltip("Event that gets fired when interaction with a scale handle stops.")]
        private UnityEvent scaleStopped = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a scale handle stops.
        /// </summary>
        public UnityEvent ScaleStopped
        {
            get => scaleStopped;
            set => scaleStopped = value;
        }

        [SerializeField]
        [Tooltip("Event that gets fired when interaction with a translation handle starts.")]
        private UnityEvent translateStarted = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a translation handle starts.
        /// </summary>
        public UnityEvent TranslateStarted
        {
            get => translateStarted;
            set => translateStarted = value;
        }

        [SerializeField]
        [Tooltip("Event that gets fired when interaction with a translation handle stops.")]
        private UnityEvent translateStopped = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a translation handle stops.
        /// </summary>
        public UnityEvent TranslateStopped
        {
            get => translateStopped;
            set => translateStopped = value;
        }

        [SerializeField]
        [Tooltip("Elastics Manager slot to enable elastics simulation when manipulating the object.")]
        private ElasticsManager elasticsManager;
        /// <summary>
        /// Elastics Manager slot to enable elastics simulation when manipulating the object.
        /// </summary>
        public ElasticsManager ElasticsManager
        {
            get => elasticsManager;
            set => elasticsManager = value;
        }

        #endregion Serialized Fields

        #region Private Fields

        // runtime instantiated visuals of bounding box 
        private Links links;
        private ScaleHandles scaleHandles;
        private RotationHandles rotationHandles;
        private TranslationHandles translationHandles;
        private BoxDisplay boxDisplay;
        private ProximityEffect proximityEffect;

        // Whether we should be displaying just the wireframe (if enabled) or the handles too
        public bool WireframeOnly { get => wireframeOnly; }
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

        // Current axis of translation about the center of the rig root
        private Vector3 currentTranslationAxis;

        // Scale of the target at the beginning of the current manipulation
        private Vector3 initialScaleOnGrabStart;

        // Rotation of the target at the beginning of the current manipulation
        private Quaternion initialRotationOnGrabStart;

        // Position of the target at the beginning of the current manipulation
        private Vector3 initialPositionOnGrabStart;

        // Point that was initially grabbed in OnPointerDown()
        private Vector3 initialGrabPoint;

        // Current position of the grab point
        private Vector3 currentGrabPoint;

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

        // Used to record the initial size of the bounds override, if it exists.
        // Necessary because BoxPadding will destructively edit the size of the
        // override BoxCollider, and repeated calls to BoxPadding will result
        // in the override bounds growing continually larger/smaller.
        private Vector3? initialBoundsOverrideSize = null;

        // True if this game object is a child of the Target one
        private bool isChildOfTarget = false;
        private const string RigRootName = "rigRoot";

        // Cache for the corner points of either renderers or colliders during the bounds calculation phase
        private static readonly List<Vector3> TotalBoundsCorners = new List<Vector3>();

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
                    if (rigRoot != null)
                    {
                        rigRoot.gameObject.SetActive(value);
                    }
                    ResetVisuals();

                    if (active)
                    {
                        proximityEffect?.ResetProximityScale();
                    }
                }
            }
        }

        #endregion Public Properties

        #region Private Properties
        private bool IsInitialized
        {
            get
            {
                return rotationHandles != null &&
                    scaleHandles != null &&
                    translationHandles != null &&
                    boxDisplay != null &&
                    links != null &&
                    proximityEffect != null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Allows to manually enable wire (edge) highlighting (edges) of the bounds control.
        /// This is useful if connected to the Manipulation events of a
        /// <see cref="Microsoft.MixedReality.Toolkit.UI.ObjectManipulator"/> 
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
        /// </summary>
        public void CreateRig()
        {
            if (!IsInitialized)
            {
                return;
            }

            // Record what the initial size of the bounds override
            // was when we constructed the rig, so we can restore
            // it after we destructively edit the size with the
            // BoxPadding (https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7997)
            if (boundsOverride != null)
            {
                initialBoundsOverrideSize = boundsOverride.size;
            }

            DestroyRig();
            InitializeRigRoot();
            InitializeDataStructures();
            DetermineTargetBounds();
            UpdateExtents();
            CreateVisuals();
            ResetVisuals();
            rigRoot.gameObject.SetActive(active);
            UpdateRigVisibilityInInspector();
        }

        /// <summary>
        /// Update the bounds.
        /// Call this function after modifying the transform of the target externally to make sure the bounds are also updated accordingly.
        /// </summary>
        public void UpdateBounds()
        {
            DetermineTargetBounds();
            UpdateExtents();
            UpdateVisuals();
        }

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (targetObject == null)
                targetObject = gameObject;

            // ensure we have a default configuration in case there's none set by the user
            scaleHandlesConfiguration = EnsureScriptable(scaleHandlesConfiguration);
            rotationHandlesConfiguration = EnsureScriptable(rotationHandlesConfiguration);
            translationHandlesConfiguration = EnsureScriptable(translationHandlesConfiguration);
            boxDisplayConfiguration = EnsureScriptable(boxDisplayConfiguration);
            linksConfiguration = EnsureScriptable(linksConfiguration);
            handleProximityEffectConfiguration = EnsureScriptable(handleProximityEffectConfiguration);

            // instantiate runtime classes for visuals
            scaleHandles = scaleHandlesConfiguration.ConstructInstance();
            rotationHandles = rotationHandlesConfiguration.ConstructInstance();
            translationHandles = translationHandlesConfiguration.ConstructInstance();

            boxDisplay = new BoxDisplay(boxDisplayConfiguration);
            links = new Links(linksConfiguration);
            proximityEffect = new ProximityEffect(handleProximityEffectConfiguration);

            if (constraintsManager == null && EnableConstraints)
            {
                constraintsManager = gameObject.EnsureComponent<ConstraintManager>();
            }
        }

        private static T EnsureScriptable<T>(T instance) where T : ScriptableObject
        {
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<T>();
            }

            return instance;
        }

        private void OnEnable()
        {
            SetActivationFlags();
            CreateRig();
            CaptureInitialState();
        }

        private void SetActivationFlags()
        {
            wireframeOnly = false;

            if (activation == BoundsControlActivationType.ActivateByProximityAndPointer ||
                activation == BoundsControlActivationType.ActivateByProximity ||
                activation == BoundsControlActivationType.ActivateByPointer)
            {
                Active = true;
                if (currentPointer == null || !DoesActivationMatchPointer(currentPointer))
                {
                    wireframeOnly = true;
                }
            }
            else if (activation == BoundsControlActivationType.ActivateOnStart)
            {
                Active = true;
            }
            else if (activation == BoundsControlActivationType.ActivateManually)
            {
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
                    UpdateExtents();
                    UpdateVisuals();
                }
                else if ((!isChildOfTarget && Target.transform.hasChanged)
                    || (boundsOverride != null && HasBoundsOverrideChanged()))
                {
                    UpdateExtents();
                    UpdateVisuals();
                    Target.transform.hasChanged = false;
                }


                // Only update proximity scaling of handles if they are visible which is when
                // active is true and wireframeOnly is false
                // also only use proximity effect if nothing is being dragged or grabbed
                if (!wireframeOnly && currentPointer == null)
                {
                    proximityEffect.UpdateScaling(TargetBounds.transform.TransformPoint(TargetBounds.center), currentBoundsExtents);
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

        private void DetermineTargetBounds()
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
                // first remove old collider if there is any so we don't accumulate any 
                // box padding on consecutive calls of this method
                if (TargetBounds != null)
                {
                    Destroy(TargetBounds);
                }
                TargetBounds = Target.AddComponent<BoxCollider>();
                Bounds bounds = GetTargetBounds();

                TargetBounds.center = bounds.center;
                TargetBounds.size = bounds.size;
            }

            // add box padding
            if (boxPadding == Vector3.zero) { return; }

            Vector3 scale = TargetBounds.transform.lossyScale;

            for (int i = 0; i < 3; i++)
            {
                if (scale[i] == 0f) { return; }

                scale[i] = 1f / scale[i];
            }

            TargetBounds.size += Vector3.Scale(boxPadding, scale);

            TargetBounds.EnsureComponent<NearInteractionGrabbable>();
        }

        private readonly List<Transform> childTransforms = new List<Transform>();

        private Bounds GetTargetBounds()
        {
            TotalBoundsCorners.Clear();

            // Collect all Transforms except for the rigRoot(s) transform structure(s)
            // Its possible we have two rigRoots here, the one about to be deleted and the new one
            // Since those have the gizmo structure childed, be need to omit them completely in the calculation of the bounds
            // This can only happen by name unless there is a better idea of tracking the rigRoot that needs destruction

            childTransforms.Clear();
            if (Target != gameObject)
            {
                childTransforms.Add(Target.transform);
            }

            foreach (Transform childTransform in Target.transform)
            {
                if (childTransform.name.Equals(RigRootName)) { continue; }
                childTransforms.AddRange(childTransform.GetComponentsInChildren<Transform>());
            }

            // Iterate transforms and collect bound volumes

            foreach (Transform childTransform in childTransforms)
            {
                Debug.Assert(childTransform != rigRoot);

                ExtractBoundsCorners(childTransform, boundsCalculationMethod);
            }

            Transform targetTransform = Target.transform;

            // In case we found nothing and this is the Target, we add its inevitable collider's bounds
            if (TotalBoundsCorners.Count == 0 && Target == gameObject)
            {
                ExtractBoundsCorners(targetTransform, BoundsCalculationMethod.ColliderOnly);
            }

            Bounds finalBounds = new Bounds(targetTransform.InverseTransformPoint(TotalBoundsCorners[0]), Vector3.zero);

            for (int i = 1; i < TotalBoundsCorners.Count; i++)
            {
                finalBounds.Encapsulate(targetTransform.InverseTransformPoint(TotalBoundsCorners[i]));
            }

            return finalBounds;
        }

        private void ExtractBoundsCorners(Transform childTransform, BoundsCalculationMethod boundsCalculationMethod)
        {
            KeyValuePair<Transform, Collider> colliderByTransform;
            KeyValuePair<Transform, Bounds> rendererBoundsByTransform;

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
                if (AddColliderBoundsCornersToTarget(colliderByTransform) && boundsCalculationMethod == BoundsCalculationMethod.ColliderOverRenderer ||
                    boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly) { return; }
            }

            // Encapsulate the renderer bounds if criteria match

            if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
            {
                if (AddRendererBoundsCornersToTarget(rendererBoundsByTransform) && boundsCalculationMethod == BoundsCalculationMethod.RendererOverCollider ||
                    boundsCalculationMethod == BoundsCalculationMethod.RendererOnly) { return; }
            }

            // Do the collider for the one case that we chose RendererOverCollider and did not find a renderer
            AddColliderBoundsCornersToTarget(colliderByTransform);
        }

        private bool AddRendererBoundsCornersToTarget(KeyValuePair<Transform, Bounds> rendererBoundsByTarget)
        {
            if (rendererBoundsByTarget.Key == null) { return false; }

            Vector3[] cornersToWorld = null;
            rendererBoundsByTarget.Value.GetCornerPositions(rendererBoundsByTarget.Key, ref cornersToWorld);
            TotalBoundsCorners.AddRange(cornersToWorld);
            return true;
        }

        private bool AddColliderBoundsCornersToTarget(KeyValuePair<Transform, Collider> colliderByTransform)
        {
            if (colliderByTransform.Key != null)
            {
                BoundsExtensions.GetColliderBoundsPoints(colliderByTransform.Value, TotalBoundsCorners, 0);
            }

            return colliderByTransform.Key != null;
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
            else if (translationHandles.IsHandleType(handle))
            {
                return translationHandles.GetHandleType();
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
            }
        }


        private Vector3 CalculateBoundsExtents()
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

            // apply flattening
            return VisualUtils.FlattenBounds(boundsExtents, flattenAxis);
        }

        private void UpdateExtents()
        {
            if (TargetBounds != null)
            {
                Vector3 newExtents = CalculateBoundsExtents();
                if (newExtents != Vector3.zero)
                {
                    currentBoundsExtents = newExtents;
                    VisualUtils.GetCornerPositionsFromBounds(new Bounds(Vector3.zero, currentBoundsExtents * 2.0f), ref boundsCorners);
                }
            }
        }
        private bool DoesActivationMatchPointer(IMixedRealityPointer pointer)
        {
            switch (activation)
            {
                case BoundsControlActivationType.ActivateOnStart:
                case BoundsControlActivationType.ActivateManually:
                    return false;
                case BoundsControlActivationType.ActivateByProximity:
                    return pointer is IMixedRealityNearPointer;
                case BoundsControlActivationType.ActivateByPointer:
                    return (pointer is IMixedRealityPointer && !(pointer is IMixedRealityNearPointer));
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
                if (debugText != null) debugText.text = "DropController:ScaleStopped";
                ScaleStopped?.Invoke();
            }
            else if (lastHandleType == HandleType.Rotation)
            {
                if (debugText != null) debugText.text = "DropController:RotateStopped";
                RotateStopped?.Invoke();
            }
            else if (lastHandleType == HandleType.Translation)
            {
                if (debugText != null) debugText.text = "DropController:TranslateStopped";
                TranslateStopped?.Invoke();
            }

            if (elasticsManager != null)
            {
                elasticsManager.EnableElasticsUpdate = true;
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
                // If we have previously logged an initial bounds size,
                // reset the boundsOverride BoxCollider to the initial size.
                // This is because the CalculateBoxPadding
                if (initialBoundsOverrideSize.HasValue)
                {
                    boundsOverride.size = initialBoundsOverrideSize.Value;
                }

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
            if (!IsInitialized)
            {
                return;
            }

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
        private Vector3 GetTranslationAxis(Transform handle)
        {
            CardinalAxisType axisType = translationHandles.GetAxisType(handle);
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
            var rigRootObj = new GameObject(RigRootName);
            rigRoot = rigRootObj.transform;
            rigRoot.parent = Target.transform;

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
                currentGrabPoint = (currentPointer.Rotation * grabPointInPointer) + currentPointer.Position;
                bool isNear = currentPointer is IMixedRealityNearPointer;


                TransformFlags transformUpdated = 0;
                if (transformType == HandleType.Rotation)
                {
                    Vector3 initDir = Vector3.ProjectOnPlane(initialGrabPoint - Target.transform.position, currentRotationAxis).normalized;
                    Vector3 currentDir = Vector3.ProjectOnPlane(currentGrabPoint - Target.transform.position, currentRotationAxis).normalized;
                    Quaternion goal = Quaternion.FromToRotation(initDir, currentDir) * initialRotationOnGrabStart;
                    MixedRealityTransform constraintRotation = MixedRealityTransform.NewRotate(goal);
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyRotationConstraints(ref constraintRotation, true, isNear);
                    }
                    if (elasticsManager != null)
                    {
                        transformUpdated = elasticsManager.ApplyTargetTransform(constraintRotation, TransformFlags.Rotate);
                    }
                    if (!transformUpdated.HasFlag(TransformFlags.Rotate))
                    {
                        Target.transform.rotation = smoothingActive ?
                            Smoothing.SmoothTo(Target.transform.rotation, constraintRotation.Rotation, rotateLerpTime, Time.deltaTime) :
                            constraintRotation.Rotation;
                    }
                }
                else if (transformType == HandleType.Scale)
                {
                    Vector3 scaleFactor = Target.transform.localScale;
                    if (ScaleHandlesConfig.ScaleBehavior == HandleScaleMode.Uniform)
                    {
                        float initialDist = Vector3.Dot(initialGrabPoint - oppositeCorner, diagonalDir);
                        float currentDist = Vector3.Dot(currentGrabPoint - oppositeCorner, diagonalDir);
                        float scaleFactorUniform = 1 + (currentDist - initialDist) / initialDist;
                        scaleFactor = new Vector3(scaleFactorUniform, scaleFactorUniform, scaleFactorUniform);
                    }
                    else // non-uniform scaling
                    {
                        // get diff from center point of box
                        Vector3 initialDist = (initialGrabPoint - oppositeCorner);
                        Vector3 currentDist = (currentGrabPoint - oppositeCorner);
                        Vector3 grabDiff = (currentDist - initialDist);
                        scaleFactor = Vector3.one + grabDiff.Div(initialDist);
                    }

                    // If non-uniform scaling or uniform scaling only on the non-flattened axes
                    if (ScaleHandlesConfig.ScaleBehavior != HandleScaleMode.Uniform || !UniformScaleOnFlattenedAxis)
                    {
                        FlattenModeType determinedType = FlattenAxis == FlattenModeType.FlattenAuto ? VisualUtils.DetermineAxisToFlatten(TargetBounds.bounds.extents) : FlattenAxis;
                        if (determinedType == FlattenModeType.FlattenX)
                        {
                            scaleFactor.x = 1;
                        }
                        if (determinedType == FlattenModeType.FlattenY)
                        {
                            scaleFactor.y = 1;
                        }
                        if (determinedType == FlattenModeType.FlattenZ)
                        {
                            scaleFactor.z = 1;
                        }
                    }

                    Vector3 newScale = initialScaleOnGrabStart.Mul(scaleFactor);
                    MixedRealityTransform clampedTransform = MixedRealityTransform.NewScale(newScale);
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyScaleConstraints(ref clampedTransform, true, isNear);
                    }

                    if (elasticsManager != null)
                    {
                        transformUpdated = elasticsManager.ApplyTargetTransform(clampedTransform, TransformFlags.Scale);
                    }
                    if (!transformUpdated.HasFlag(TransformFlags.Scale))
                    {
                        Target.transform.localScale = smoothingActive ?
                            Smoothing.SmoothTo(Target.transform.localScale, clampedTransform.Scale, scaleLerpTime, Time.deltaTime) :
                            clampedTransform.Scale;
                    }

                    var originalRelativePosition = initialPositionOnGrabStart - oppositeCorner;
                    var newPosition = originalRelativePosition.Div(initialScaleOnGrabStart).Mul(Target.transform.localScale) + oppositeCorner;
                    Target.transform.position = smoothingActive ? Smoothing.SmoothTo(Target.transform.position, newPosition, scaleLerpTime, Time.deltaTime) : newPosition;
                }
                else if (transformType == HandleType.Translation)
                {
                    Vector3 translateVectorAlongAxis = Vector3.Project(currentGrabPoint - initialGrabPoint, currentTranslationAxis);

                    var goal = initialPositionOnGrabStart + translateVectorAlongAxis;
                    MixedRealityTransform constraintTranslate = MixedRealityTransform.NewTranslate(goal);
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyTranslationConstraints(ref constraintTranslate, true, isNear);
                    }
                    if (elasticsManager != null)
                    {
                        transformUpdated = elasticsManager.ApplyTargetTransform(constraintTranslate, TransformFlags.Move);
                    }
                    if (!transformUpdated.HasFlag(TransformFlags.Move))
                    {
                        Target.transform.position = smoothingActive ?
                            Smoothing.SmoothTo(Target.transform.position, constraintTranslate.Position, translateLerpTime, Time.deltaTime) :
                            constraintTranslate.Position;
                    }
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

            if (!DoesActivationMatchPointer(eventData.Pointer))
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
                    initialRotationOnGrabStart = Target.transform.rotation;
                    initialPositionOnGrabStart = Target.transform.position;
                    grabPointInPointer = Quaternion.Inverse(eventData.Pointer.Rotation) * (initialGrabPoint - currentPointer.Position);

                    // todo: move this out?
                    SetHighlighted(grabbedHandleTransform, eventData.Pointer);

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
                    else if (currentHandleType == HandleType.Translation)
                    {
                        currentTranslationAxis = GetTranslationAxis(grabbedHandleTransform);

                        TranslateStarted?.Invoke();

                        if (debugText != null)
                        {
                            debugText.text = "OnPointerDown:TranslateStarted";
                        }
                    }

                    if (elasticsManager != null)
                    {
                        // Initialize elastic systems.
                        elasticsManager.InitializeElastics(Target.transform);
                        // disable auto update (elastics are going to be queried through applyTargetTransform when manipulating)
                        elasticsManager.EnableElasticsUpdate = false;
                    }

                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.Initialize(new MixedRealityTransform(Target.transform));
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
                DropController();
            }
        }

        #endregion Used Event Handlers

        #region Unused Event Handlers

        void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData) { }

        #endregion Unused Event Handlers

        #region BoundsControl Visuals Private Methods

        private void SetHighlighted(Transform activeHandle, IMixedRealityPointer pointer = null)
        {
            scaleHandles.SetHighlighted(activeHandle, pointer);
            rotationHandles.SetHighlighted(activeHandle, pointer);
            translationHandles.SetHighlighted(activeHandle, pointer);
            boxDisplay.SetHighlighted();
        }

        private void ResetVisuals()
        {
            if (currentPointer != null || !IsInitialized)
            {
                return;
            }

            boxDisplay.Reset(active);
            boxDisplay.UpdateFlattenAxis(flattenAxis);

            bool isVisible = (active == true && wireframeOnly == false);

            rotationHandles.Reset(isVisible, flattenAxis);
            links.Reset(active, flattenAxis);
            scaleHandles.Reset(isVisible, flattenAxis);
            translationHandles.Reset(isVisible, flattenAxis);
        }

        private void CreateVisuals()
        {
            // add corners
            bool isFlattened = flattenAxis != FlattenModeType.DoNotFlatten;

            // Add scale handles
            scaleHandles.Create(ref boundsCorners, rigRoot, isFlattened);
            proximityEffect.RegisterObjectProvider(scaleHandles);

            // Add rotation handles
            rotationHandles.Create(ref boundsCorners, rigRoot);
            proximityEffect.RegisterObjectProvider(rotationHandles);

            links.CreateLinks(ref boundsCorners, rigRoot, currentBoundsExtents);

            // Add translation handles
            translationHandles.Create(ref boundsCorners, rigRoot);
            proximityEffect.RegisterObjectProvider(translationHandles);

            // add box display
            boxDisplay.AddBoxDisplay(rigRoot.transform, currentBoundsExtents, flattenAxis);

            // update visuals
            UpdateVisuals();
        }

        private void DestroyVisuals()
        {
            proximityEffect.ClearObjects();
            links.Clear();

            scaleHandles.DestroyHandles();
            rotationHandles.DestroyHandles();
            translationHandles.DestroyHandles();
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

                rotationHandles.CalculateHandlePositions(ref boundsCorners);

                // Links depend on rotation handles for position calculations.
                links.UpdateLinkPositions(ref boundsCorners);
                links.UpdateLinkScales(currentBoundsExtents);

                translationHandles.CalculateHandlePositions(ref boundsCorners);
                scaleHandles.UpdateHandles(ref boundsCorners);

                boxDisplay.UpdateDisplay(currentBoundsExtents, flattenAxis);

                // move rig into position and rotation
                rigRoot.position = TargetBounds.bounds.center;
                rigRoot.rotation = Target.transform.rotation;
                rigRoot.parent = Target.transform;
            }
        }

        #endregion BoundsControl Visuals Private Methods
    }
}
