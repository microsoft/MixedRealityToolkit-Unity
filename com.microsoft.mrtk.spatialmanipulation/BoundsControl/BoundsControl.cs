// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// BoundsControl uses an `IBoundsVisuals` prefab to create a bounds visual around the specified object.
    /// Any <see cref="BoundsHandleInteractable"/>s on the bounds prefab will forward their manipulation events
    /// to this script, allowing for handle-based manipulation of the target object.
    /// </summary>
    [RequireComponent(typeof(ConstraintManager))]
    [AddComponentMenu("MRTK/Spatial Manipulation/Bounds Control")]
    public class BoundsControl : MonoBehaviour
    {
        #region Serialized Fields/Properties

        [Header("Bounds")]

        [SerializeField]
        [Tooltip("This prefab will be instantiated as the bounds visuals. Consider making your own prefab to modify how the visuals are drawn.")]
        private GameObject boundsVisualsPrefab;

        /// <summary>
        /// This prefab will be instantiated as the bounds visuals.
        /// </summary>
        /// <remarks>
        /// Consider making your own prefab to modify how the visuals are drawn.
        /// </remarks>
        public GameObject BoundsVisualsPrefab
        {
            get => boundsVisualsPrefab;
            set
            {
                if (value != boundsVisualsPrefab)
                {
                    boundsVisualsPrefab = value;
                    CreateBoundsVisuals();
                }
            }
        }

        [SerializeField]
        [Tooltip("How should the bounds be automatically calculated?")]
        private BoundsCalculator.BoundsCalculationMethod boundsCalculationMethod = BoundsCalculator.BoundsCalculationMethod.RendererOverCollider;

        /// <summary>
        /// How should the bounds be automatically calculated?
        /// </summary>
        public BoundsCalculator.BoundsCalculationMethod BoundsCalculationMethod
        {
            get => boundsCalculationMethod;
            set
            {
                boundsCalculationMethod = value;
                needsBoundsRecompute = ComputeBounds();
            }
        }

        [SerializeField]
        [Tooltip("Should BoundsControl include inactive objects when it traverses the hierarchy to calculate bounds?")]
        private bool includeInactiveObjects = false;

        /// <summary>
        /// Should BoundsControl include inactive objects when it traverses the hierarchy to calculate bounds?
        /// </summary>
        public bool IncludeInactiveObjects
        {
            get => includeInactiveObjects;
            set
            {
                if (includeInactiveObjects != value)
                {
                    includeInactiveObjects = value;
                    needsBoundsRecompute = ComputeBounds();
                }
            }
        }

        [SerializeField]
        [Tooltip("Should BoundsControl use a specific object to calculate bounds, instead of the entire hierarchy?")]
        private bool overrideBounds = false;

        /// <summary>
        /// Should BoundsControl use a specific object to calculate bounds, instead of the entire hierarchy?
        /// </summary>
        public bool OverrideBounds
        {
            get => overrideBounds;
            set
            {
                if (overrideBounds != value)
                {
                    overrideBounds = value;
                    needsBoundsRecompute = ComputeBounds();
                }
            }
        }

        [SerializeField]
        [DrawIf("overrideBounds")]
        [Tooltip("The bounds will be calculated from this object and this object only, instead of the entire hierarchy.")]
        private Transform boundsOverride;

        /// <summary>
        /// The bounds will be calculated from this object and this object only, instead of the entire hierarchy.
        /// </summary>
        public Transform BoundsOverride
        {
            get => boundsOverride;
            set
            {
                if (value != boundsOverride)
                {
                    boundsOverride = value;
                    needsBoundsRecompute = true;
                }
            }
        }

        [SerializeField]
        [Tooltip("How should this BoundsControl flatten?")]
        private FlattenMode flattenMode = FlattenMode.Auto;

        /// <summary>
        /// How should this BoundsControl flatten?
        /// </summary>
        public FlattenMode FlattenMode
        {
            get => flattenMode;
            set
            {
                flattenMode = value;
                needsBoundsRecompute = true;
            }
        }

        [SerializeField]
        [Tooltip("The bounds will be padded around the extent of the object by this amount, in world units.")]
        private float boundsPadding = 0.01f;

        /// <summary>
        /// The bounds will be padded around the extent of the object by this amount, in world units.
        /// </summary>
        public float BoundsPadding
        {
            get => boundsPadding;
            set
            {
                boundsPadding = value;
                needsBoundsRecompute = true;
            }
        }

        [Header("Interactable Connection")]

        [SerializeField]
        [Tooltip("Reference to the interactable (such as ObjectManipulator) in charge of the wrapped object")]
        private StatefulInteractable interactable;

        /// <summary>
        /// Reference to the interactable (such as ObjectManipulator) in charge of the wrapped object
        /// </summary>
        public StatefulInteractable Interactable
        {
            get => interactable;
            set
            {
                if (interactable != value)
                {
                    UnsubscribeFromInteractable();
                    interactable = value;
                    SubscribeToInteractable();
                }
            }
        }

        [SerializeField]
        [Tooltip("Toggle the handles when the interactable is selected, not moved, and then released.")]
        private bool toggleHandlesOnClick = true;

        /// <summary>
        /// Toggle the handles when the interactable is selected, not moved, and then released.
        /// </summary>
        public bool ToggleHandlesOnClick
        {
            get => toggleHandlesOnClick;
            set => toggleHandlesOnClick = value;
        }

        [SerializeField]
        [DrawIf("toggleHandlesOnClick")]
        [Tooltip("During a selection of the associated interactable, if the interactable is dragged/moved a smaller distance than this value, the handles will be activated/deactivated.")]
        private float dragToggleThreshold = 0.005f;

        /// <summary>
        /// During a selection of the associated interactable, if the interactable is
        /// dragged/moved a smaller distance than this value, the handles will be activated/deactivated.
        /// </summary>
        public float DragToggleThreshold { get => dragToggleThreshold; set => dragToggleThreshold = value; }

        [Header("Manipulation")]

        [SerializeField]
        [Tooltip("The transform to be manipulated.")]
        private Transform target;

        /// <summary>
        /// The transform to be manipulated. If null, it is automatically set
        /// to the transform that this BoundsControl is on.
        /// </summary>
        public Transform Target
        {
            get
            {
                if (target == null)
                {
                    target = transform;
                }
                return target;
            }
            set
            {
                target = value;
            }
        }

        [SerializeField]
        [Tooltip("Should any handles be visible?")]
        private bool handlesActive = false;

        /// <summary>
        /// Should any handles be visible?
        /// </summary>
        public bool HandlesActive
        {
            get => handlesActive;
            set => handlesActive = value;
        }

        [SerializeField]
        [Tooltip("Which type of handles should be visible?")]
        private HandleType enabledHandles = HandleType.Rotation | HandleType.Scale;

        /// <summary>
        /// Which type of handles should be visible?
        /// </summary>
        public HandleType EnabledHandles
        {
            get => enabledHandles;
            set => enabledHandles = value;
        }

        [EnumFlags]
        [SerializeField]
        [Tooltip("Specifies whether the rotate handles will rotate the object around its origin, or the center of its calculated bounds.")]
        private RotateAnchorType rotateAnchor = RotateAnchorType.BoundsCenter;

        /// <summary>
        /// Specifies whether the rotate handles will rotate the object around its origin, or the center of its calculated bounds.
        /// </summary>
        public RotateAnchorType RotateAnchor
        {
            get => rotateAnchor;
            set
            {
                if (rotateAnchor != value)
                {
                    rotateAnchor = value;
                }
            }
        }

        [EnumFlags]
        [SerializeField]
        [Tooltip("Specifies whether the scale handles will rotate the object around their opposing corner, or the center of its calculated bounds.")]
        private ScaleAnchorType scaleAnchor = ScaleAnchorType.OppositeCorner;

        /// <summary>
        /// Specifies whether the scale handles will rotate the scale around the opposing corner, or the center of its calculated bounds.
        /// </summary>
        public ScaleAnchorType ScaleAnchor
        {
            get => scaleAnchor;
            set
            {
                if (scaleAnchor != value)
                {
                    scaleAnchor = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Scale mode that is applied when interacting with scale handles - default is uniform scaling. Non uniform mode scales the control according to hand / controller movement in space.")]
        private HandleScaleMode scaleBehavior = HandleScaleMode.Uniform;

        /// <summary>
        /// Scale behavior that is applied when interacting with scale handles - default is uniform scaling. Non uniform mode scales the control according to hand / controller movement in space.
        /// </summary>
        public HandleScaleMode ScaleBehavior
        {
            get => scaleBehavior;
            set
            {
                if (scaleBehavior != value)
                {
                    scaleBehavior = value;
                }
            }
        }

        [Header("Modifiers")]

        [SerializeField]
        [Tooltip("Check to enable frame-rate independent smoothing.")]
        private bool smoothingActive = true;

        /// <summary>
        /// Check to enable frame-rate independent smoothing.
        /// </summary>
        public bool SmoothingActive
        {
            get => smoothingActive;
            set => smoothingActive = value;
        }

        [SerializeField]
        [DrawIf("smoothingActive")]
        [Tooltip("Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float rotateLerpTime = 0.00001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float RotateLerpTime
        {
            get => rotateLerpTime;
            set => rotateLerpTime = value;
        }

        [SerializeField]
        [DrawIf("smoothingActive")]
        [Tooltip("Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float scaleLerpTime = 0.00001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float ScaleLerpTime
        {
            get => scaleLerpTime;
            set => scaleLerpTime = value;
        }

        [SerializeField]
        [DrawIf("smoothingActive")]
        [Tooltip("Enter amount representing amount of smoothing to apply to the translation. " +
            "Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float translateLerpTime = 0.00001f;

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
        [DrawIf("enableConstraints")]
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

        [Header("Events")]

        [SerializeField]
        SelectEnterEvent manipulationStarted = new SelectEnterEvent();

        /// <summary>
        /// Fired when manipulation on a handle begins.
        /// </summary>
        public SelectEnterEvent ManipulationStarted
        {
            get => manipulationStarted;
            set => manipulationStarted = value;
        }

        [SerializeField]
        SelectExitEvent manipulationEnded = new SelectExitEvent();

        /// <summary>
        /// Fired when manipulation on a handle ends.
        /// </summary>
        public SelectExitEvent ManipulationEnded
        {
            get => manipulationEnded;
            set => manipulationEnded = value;
        }

        #endregion Serialized Fields/Properties

        /// <summary>
        /// Is this BoundsControl currently being manipulated?
        /// </summary>
        public bool IsManipulated => currentHandle != null;

        /// <summary>
        /// Is this BoundsControl actively flattening along its thinnest axis?
        /// </summary>
        public bool IsFlat { get; protected set; }

        private Bounds currentBounds = new Bounds();

        // The box visuals GameObject instantiated at Awake.
        private GameObject boxInstance;

        // Used to determine whether the associated interactable was moved between select/deselect,
        // which drives whether the handles get toggled on/off. If the interactable was moved less than a
        // certain threshold, we toggle the handles on/off. If the interactable was moved further than the
        // threshold, we don't toggle the handles (as it was probably an intentional ObjectManipulation!)
        private Vector3 startMovePosition;

        // The interactor selecting the currentHandle's attachTransform position at time of selection.
        private Vector3 initialGrabPoint;

        // The handle that is currently being manipulated.
        private BoundsHandleInteractable currentHandle;

        // A unit vector, relative to the transform target, that represents the flattening axis.
        private Vector3 flattenVector;

        // The transform of the object when the manipulation was initiated.
        private MixedRealityTransform initialTransformOnGrabStart;

        // The corner opposite from the current scale handle (if a scale handle is being selected)
        private Vector3 oppositeCorner;

        // The vector representing the diagonal (from the current scale handle to the opposite corner)
        private Vector3 diagonalDir;

        // Position of the anchor when manipulation started.
        private Vector3 initialAnchorOnGrabStart;

        // Delta from the anchor to the object's center when manipulation started.
        private Vector3 initialAnchorDeltaOnGrabStart;

        // Rotate axis during a rotation, translation axis during a translation, etc
        private Vector3 currentManipulationAxis;

        // If we calculate the bounds at Awake and discover a UGUI autolayout group,
        // we need to queue up a second bounds computation pass to take the newly computed
        // autolayout into account.
        private bool needsBoundsRecompute = false;

        // Number of frames to wait until we re-compute bounds for UGUI autolayout.
        private int waitForFrames = 1;

        // An absolute minimum scale to prevent the object from collapsing/inverting.
        private Vector3 minimumScale;

        // BC cannot scale below this "epsilon" value
        private const float lowerAbsoluteClamp = 0.001f;

        // Is Bounds Control host selected?
        private bool isHostSelected = false;

        // Has the bounds control moved past the toggle threshold throughout the time it was selected?
        private bool hasPassedToggleThreshold = false;

        private void Awake()
        {
            if (Interactable == null)
            {
                Interactable = GetComponentInParent<StatefulInteractable>();
            }
            else
            {
                SubscribeToInteractable();
            }

            // Clamp all scaling operations to a tiny fraction the initial scale,
            // regardless if the user has applied a MinMaxScaleConstraint or not.
            minimumScale = Target.transform.localScale * lowerAbsoluteClamp;

            // Spawn our bounds visuals.
            CreateBoundsVisuals();

            // See if we have a constraints manager.
            if (constraintsManager == null)
            {
                constraintsManager = GetComponent<ConstraintManager>();
            }
            // Setup constraints with the initial pose.
            if (constraintsManager != null)
            {
                constraintsManager.Setup(new MixedRealityTransform(Target.transform));
            }
        }

        private void Update()
        {
            // If we need to recompute bounds (usually because we found a
            // UGUI element), make sure we've waited enough frames since
            // startup, then recompute.
            if (needsBoundsRecompute && waitForFrames-- <= 0)
            {
                ComputeBounds(true);
                needsBoundsRecompute = false;
            }

            TransformTarget();
            CheckToggleThreshold();
        }

        private void OnDestroy()
        {
            UnsubscribeFromInteractable();
        }

        private void OnHostSelected(SelectEnterEventArgs args)
        {
            isHostSelected = true;
            hasPassedToggleThreshold = false;
            // Track where the interactable was when it was selected.
            // We compare against this when the selection ends.
            startMovePosition = Target.localPosition;
        }

        private void OnHostDeselected(SelectExitEventArgs args)
        {
            if (!hasPassedToggleThreshold && toggleHandlesOnClick)
            {
                HandlesActive = !HandlesActive;
            }
            hasPassedToggleThreshold = false;
            isHostSelected = false;
        }

        private void SubscribeToInteractable()
        {
            if (Interactable != null)
            {
                Interactable.firstSelectEntered.AddListener(OnHostSelected);
                Interactable.lastSelectExited.AddListener(OnHostDeselected);
            }
        }

        private void UnsubscribeFromInteractable()
        {
            if (Interactable != null)
            {
                Interactable.firstSelectEntered.RemoveListener(OnHostSelected);
                Interactable.lastSelectExited.RemoveListener(OnHostDeselected);
            }
        }

        private void CreateBoundsVisuals()
        {
            // Teardown the existing bounds visuals if we have any.
            if (boxInstance != null)
            {
                Destroy(boxInstance);
                boxInstance = null;
            }

            if (boundsVisualsPrefab != null)
            {
                boxInstance = Instantiate(boundsVisualsPrefab, transform);

                // Compute bounds, but we might need to run it again one frame later,
                // to take UGUI autolayout computation into account.
                needsBoundsRecompute = ComputeBounds();
            }
        }


        /// <summary>
        /// Recomputes the bounds of the BoundsControl and updates the current bounds visuals to match.
        /// </summary>
        public void RecomputeBounds()
        {
            // Any subsequent/public calls to RecomputeBounds are considered a second pass.
            ComputeBounds(true);
        }

        /// <summary>
        /// Computes the bounds of the BoundsControl and updates the current bounds visuals to match.
        /// </summary>
        /// <param name="isSecondPass">
        /// Is this the second pass? If not, we'll abort early if we find the need to queue up a second pass.
        /// </param>
        /// <returns>
        /// True if a second computation pass is required (usually because we found a Canvas element somewhere.)
        /// You should call this function again (and pass in true) at least one frame from the current frame.
        /// This allows Canvas elements to compute their layouts.
        /// </returns>
        private bool ComputeBounds(bool isSecondPass = false)
        {
            // currentBounds are local to Target.
            // needsBoundsRecompute will be set to true iff we find a UGUI autolayout.

            // Use the bounds override if we have one.
            Transform searchStart = (overrideBounds && boundsOverride != null) ? boundsOverride : Target;

            currentBounds = BoundsCalculator.CalculateBounds(Target, searchStart, boxInstance.transform, out bool foundCanvas, boundsCalculationMethod, includeInactiveObjects, !isSecondPass);

            // Immediately give up if we know we have a UGUI layout.
            // We re-queue a second pass next frame.
            if (foundCanvas && isSecondPass == false)
            {
                return foundCanvas;
            }

            // Transform bounds to world-scale. (Still Target-axis-aligned.)
            Vector3 globalBoundsSize = Vector3.Scale(currentBounds.size, Target.lossyScale);

            // Compute the flatten vector.
            flattenVector = BoundsCalculator.CalculateFlattenVector(globalBoundsSize);

            // Are we flattened?
            bool isThinEnough = globalBoundsSize.x < 0.01f || globalBoundsSize.y < 0.01f || globalBoundsSize.z < 0.01f;
            IsFlat = (isThinEnough && FlattenMode == FlattenMode.Auto) || FlattenMode == FlattenMode.Always;

            // If flattened, flatten the padding by the flatten vector.
            Vector3 padding = IsFlat ? Vector3.Scale(Vector3.one * BoundsPadding, Vector3.one - flattenVector) : Vector3.one * BoundsPadding;

            // Rescale the padding back to local space, because we need to add it back onto the bounds.
            Vector3 localPadding = Vector3.Scale(padding, new Vector3(1.0f / Target.lossyScale.x, 1.0f / Target.lossyScale.y, 1.0f / Target.lossyScale.z));
            currentBounds.size += localPadding;

            // Initialize the box instance to the correct size/position.
            boxInstance.transform.localScale = currentBounds.size;
            boxInstance.transform.localPosition = currentBounds.center;

            return foundCanvas;
        }

        /// <summary>
        /// Called by <see cref="BoundsHandleInteractable"/> from its OnSelectExited.
        /// Routes the XRI event data through, as well as a reference to itself, the selected handle.
        /// </summary>
        internal void OnHandleSelectExited(BoundsHandleInteractable handle, SelectExitEventArgs args)
        {
            if (currentHandle == handle)
            {
                currentHandle = null;

                // Notify listeners of manipulation end.
                manipulationEnded?.Invoke(args);
            }
        }

        /// <summary>
        /// Called by <see cref="BoundsHandleInteractable"/> from its OnSelectEntered.
        /// Routes the XRI event data through, as well as a reference to itself, the selected handle.
        /// </summary>
        internal void OnHandleSelectEntered(BoundsHandleInteractable handle, SelectEnterEventArgs args)
        {
            if (currentHandle != null)
            {
                return;
            }

            if ((handle.HandleType & EnabledHandles) == handle.HandleType)
            {
                // Notify listeners of manipulation start.
                manipulationStarted?.Invoke(args);

                currentHandle = handle;
                initialGrabPoint = args.interactorObject.GetAttachTransform(handle).position;
                initialTransformOnGrabStart = new MixedRealityTransform(Target.transform);
                Vector3 anchorPoint = RotateAnchor == RotateAnchorType.BoundsCenter ? Target.transform.TransformPoint(currentBounds.center) : Target.transform.position;
                initialAnchorOnGrabStart = anchorPoint;
                initialAnchorDeltaOnGrabStart = Target.transform.position - anchorPoint;
                // todo: move this out?
                if (currentHandle.HandleType == HandleType.Scale)
                {
                    // Will use this to scale the target relative to the opposite corner
                    oppositeCorner = boxInstance.transform.TransformPoint(-currentHandle.transform.localPosition);
                    diagonalDir = (currentHandle.transform.position - oppositeCorner).normalized;
                    // ScaleStarted?.Invoke();
                }
                else if (currentHandle.HandleType == HandleType.Rotation)
                {
                    currentManipulationAxis = handle.transform.forward;
                    // RotateStarted?.Invoke();
                }
                else if (currentHandle.HandleType == HandleType.Translation)
                {
                    // currentTranslationAxis = GetTranslationAxis(handle);
                    // TranslateStarted?.Invoke();
                }

                if (EnableConstraints && constraintsManager != null)
                {
                    constraintsManager.OnManipulationStarted(new MixedRealityTransform(Target.transform));
                }
            }
        }

        private static readonly ProfilerMarker TransformTargetPerfMarker =
            new ProfilerMarker("[MRTK] BoundsControl.TransformTarget");

        private void TransformTarget()
        {
            using (TransformTargetPerfMarker.Auto())
            {
                if (currentHandle != null)
                {
                    Vector3 currentGrabPoint = currentHandle.interactorsSelecting[0].GetAttachTransform(currentHandle).position;
                    // bool isNear = currentInteractor is IGrabInteractor;

                    TransformFlags transformUpdated = 0;
                    if (currentHandle.HandleType == HandleType.Rotation)
                    {
                        // Compute the anchor around which we will be rotating the object, based
                        // on the desired RotateAnchorType.
                        Vector3 anchorPoint = RotateAnchor == RotateAnchorType.BoundsCenter ? Target.transform.TransformPoint(currentBounds.center) : Target.transform.position;

                        Vector3 initDir = Vector3.ProjectOnPlane(initialGrabPoint - anchorPoint, currentManipulationAxis).normalized;
                        Vector3 currentDir = Vector3.ProjectOnPlane(currentGrabPoint - anchorPoint, currentManipulationAxis).normalized;
                        Quaternion initQuat = Quaternion.LookRotation(initDir, currentManipulationAxis);
                        Quaternion currentQuat = Quaternion.LookRotation(currentDir, currentManipulationAxis);
                        Quaternion goalRotation = (currentQuat * Quaternion.Inverse(initQuat)) * initialTransformOnGrabStart.Rotation;

                        Quaternion rotationDelta = goalRotation * Quaternion.Inverse(initialTransformOnGrabStart.Rotation);
                        Vector3 goalPosition = initialAnchorOnGrabStart + (rotationDelta * initialAnchorDeltaOnGrabStart);

                        MixedRealityTransform constraintRotation = MixedRealityTransform.NewRotate(goalRotation);

                        if (EnableConstraints && constraintsManager != null)
                        {
                            constraintsManager.ApplyRotationConstraints(ref constraintRotation, true, currentHandle.IsGrabSelected);
                        }

                        // TODO: Elastics integration (soon!)

                        // if (elasticsManager != null)
                        // {
                        //     transformUpdated = elasticsManager.ApplyTargetTransform(constraintRotation, TransformFlags.Rotate);
                        // }

                        if (!transformUpdated.IsMaskSet(TransformFlags.Rotate))
                        {
                            // Here, we apply smoothing to the new goal position using the rotateLerpTime, because this
                            // position modification is specifically for adjusting the object's origin based on the RotateAnchorType.
                            // This offset needs to match the smoothing on the rotation.
                            Target.transform.SetPositionAndRotation(
                                smoothingActive ?
                                    Smoothing.SmoothTo(Target.transform.position, goalPosition, rotateLerpTime, Time.deltaTime) :
                                    goalPosition,

                                smoothingActive ?
                                    Smoothing.SmoothTo(Target.transform.rotation, constraintRotation.Rotation, rotateLerpTime, Time.deltaTime) :
                                    constraintRotation.Rotation
                                );
                        }
                    }
                    else if (currentHandle.HandleType == HandleType.Scale)
                    {
                        Vector3 anchorPoint = ScaleAnchor == ScaleAnchorType.BoundsCenter ? Target.transform.TransformPoint(currentBounds.center) : oppositeCorner;
                        Vector3 scaleFactor = Target.transform.localScale;
                        if (ScaleBehavior == HandleScaleMode.Uniform)
                        {
                            float initialDist = Vector3.Dot(initialGrabPoint - anchorPoint, diagonalDir);
                            float currentDist = Vector3.Dot(currentGrabPoint - anchorPoint, diagonalDir);
                            float scaleFactorUniform = 1 + (currentDist - initialDist) / initialDist;
                            scaleFactor = new Vector3(scaleFactorUniform, scaleFactorUniform, scaleFactorUniform);
                        }
                        else // non-uniform scaling
                        {
                            // get diff from center point of box
                            Vector3 initialDist = Target.transform.InverseTransformVector(initialGrabPoint - anchorPoint);
                            Vector3 currentDist = Target.transform.InverseTransformVector(currentGrabPoint - anchorPoint);
                            Vector3 grabDiff = (currentDist - initialDist);

                            scaleFactor = Vector3.one + grabDiff.Div(initialDist);
                        }

                        Vector3 newScale = initialTransformOnGrabStart.Scale.Mul(scaleFactor);
                        MixedRealityTransform clampedTransform = MixedRealityTransform.NewScale(newScale);
                        if (EnableConstraints && constraintsManager != null)
                        {
                            constraintsManager.ApplyScaleConstraints(ref clampedTransform, true, currentHandle.IsGrabSelected);
                        }

                        // Clamp to an absolute minimum, regardless of whether a MinMaxScaleConstraint is being used or not.
                        // Prevents object from collapsing/inverting.
                        clampedTransform.Scale = Vector3.Max(clampedTransform.Scale, minimumScale);

                        // TODO: Elastics integration (soon!)

                        // if (elasticsManager != null)
                        // {
                        //     transformUpdated = elasticsManager.ApplyTargetTransform(clampedTransform, TransformFlags.Scale);
                        // }

                        if (!transformUpdated.IsMaskSet(TransformFlags.Scale))
                        {
                            Target.transform.localScale = smoothingActive ?
                                Smoothing.SmoothTo(Target.transform.localScale, clampedTransform.Scale, scaleLerpTime, Time.deltaTime) :
                                clampedTransform.Scale;
                        }

                        var originalRelativePosition = Target.transform.InverseTransformDirection(initialTransformOnGrabStart.Position - anchorPoint);
                        var newPosition = Target.transform.TransformDirection(originalRelativePosition.Mul(clampedTransform.Scale.Div(initialTransformOnGrabStart.Scale))) + anchorPoint;
                        Target.transform.position = smoothingActive ? Smoothing.SmoothTo(Target.transform.position, newPosition, scaleLerpTime, Time.deltaTime) : newPosition;
                    }
                    else if (currentHandle.HandleType == HandleType.Translation)
                    {
                        Vector3 translateVectorAlongAxis = Vector3.Project(currentGrabPoint - initialGrabPoint, currentHandle.transform.forward);

                        var goal = initialTransformOnGrabStart.Position + translateVectorAlongAxis;
                        MixedRealityTransform constraintTranslate = MixedRealityTransform.NewTranslate(goal);
                        if (EnableConstraints && constraintsManager != null)
                        {
                            constraintsManager.ApplyTranslationConstraints(ref constraintTranslate, true, currentHandle.IsGrabSelected);
                        }

                        // TODO: Elastics integration (soon!)

                        // if (elasticsManager != null)
                        // {
                        //     transformUpdated = elasticsManager.ApplyTargetTransform(constraintTranslate, TransformFlags.Move);
                        // }

                        if (!transformUpdated.IsMaskSet(TransformFlags.Move))
                        {
                            Target.transform.position = smoothingActive ?
                                Smoothing.SmoothTo(Target.transform.position, constraintTranslate.Position, translateLerpTime, Time.deltaTime) :
                                constraintTranslate.Position;
                        }
                    }
                }
            }
        }

        private void CheckToggleThreshold()
        {
            if (isHostSelected && !hasPassedToggleThreshold && Vector3.Distance(startMovePosition, Target.localPosition) >= dragToggleThreshold)
            {
                hasPassedToggleThreshold = true;
            }
        }
    }
}
