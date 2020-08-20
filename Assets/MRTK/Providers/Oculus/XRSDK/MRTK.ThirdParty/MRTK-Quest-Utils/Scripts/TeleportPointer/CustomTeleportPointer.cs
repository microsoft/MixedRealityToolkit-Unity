//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Teleport;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Custom teleport pointer, controlled directly by MRTK-Quest input classes.
    /// This exists because the built in teleport pointer has a hard dependency on input actions that do not exist for articulated hands.
    /// </summary>
    public class CustomTeleportPointer : MonoBehaviour, IMixedRealityTeleportHandler, IMixedRealityTeleportPointer
    {
        /// <summary>
        /// True if a teleport request is being raised, false otherwise.
        /// </summary>
        public bool TeleportRequestRaised { get; private set; } = false;

        /// <summary>
        /// The result from the last raycast.
        /// </summary>
        public TeleportSurfaceResult TeleportSurfaceResult { get; private set; } = TeleportSurfaceResult.None;

        /// <inheritdoc />
        public IMixedRealityTeleportHotSpot TeleportHotSpot { get; set; }

        [SerializeField]
        [Tooltip("Teleport Pointer will only respond to input events for teleportation that match this MixedRealityInputAction")]
        private MixedRealityInputAction teleportAction = MixedRealityInputAction.None;

        /// <summary>
        /// Teleport pointer will only respond to input events for teleportation that match this MixedRealityInputAction.
        /// </summary>
        public MixedRealityInputAction TeleportInputAction => teleportAction;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The threshold amount for joystick input (Dead Zone)")]
        private float inputThreshold = 0.5f;

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("If Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like the forward direction, we apply this offset to make navigation feel more natural")]
        private float angleOffset = 0f;

        [SerializeField]
        [Range(5f, 90f)]
        [Tooltip("The angle from the pointer's forward position that will activate the teleport.")]
        private float teleportActivationAngle = 45f;

        [SerializeField]
        [Range(5f, 90f)]
        [Tooltip("The angle from the joystick left and right position that will activate a rotation")]
        private float rotateActivationAngle = 22.5f;

        [SerializeField]
        [Range(5f, 180f)]
        [Tooltip("The amount to rotate the camera when rotation is activated")]
        private float rotationAmount = 90f;

        [SerializeField]
        [Range(5, 90f)]
        [Tooltip("The angle from the joystick down position that will activate a strafe that will move the camera back")]
        private float backStrafeActivationAngle = 45f;

        [SerializeField]
        [Tooltip("The distance to move the camera when the strafe is activated")]
        private float strafeAmount = 0.25f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The up direction threshold to use when determining if a surface is 'flat' enough to teleport to.")]
        private float upDirectionThreshold = 0.2f;

        [SerializeField]
        protected Gradient LineColorHotSpot = new Gradient();

        [SerializeField]
        [Tooltip("Layers that are considered 'valid' for navigation")]
        protected LayerMask ValidLayers = UnityPhysics.DefaultRaycastLayers;

        private LayerMask[] validRayCastLayers;

        [SerializeField]
        [Tooltip("Layers that are considered 'invalid' for navigation")]
        protected LayerMask InvalidLayers = UnityPhysics.IgnoreRaycastLayer;

        [SerializeField]
        private DistorterGravity gravityDistorter = null;


        /// <summary>
        /// The Gravity Distorter that is affecting the <see cref="BaseMixedRealityLineDataProvider"/> attached to this pointer.
        /// </summary>
        public DistorterGravity GravityDistorter => gravityDistorter;

        private float cachedInputThreshold;
        private float inputThresholdSquared;

        /// <summary>
        /// The square of the InputThreshold value.
        /// </summary>
        private float InputThresholdSquared
        {
            get
            {
                if (!Mathf.Approximately(cachedInputThreshold, inputThreshold))
                {
                    inputThresholdSquared = Mathf.Pow(inputThreshold, 2f);
                    cachedInputThreshold = inputThreshold;
                }
                return inputThresholdSquared;
            }
        }

        private Vector2 currentInputPosition = Vector2.zero;

        protected bool isTeleportRequestActive;

        private bool lateRegisterTeleport = true;

        private bool canTeleport;

        private bool canMove;

        // private Handedness pointerHandedness = Handedness.None;

        #region Line Management
        [SerializeField]
        protected Gradient LineColorSelected = new Gradient();

        [SerializeField]
        protected Gradient LineColorValid = new Gradient();

        [SerializeField]
        protected Gradient LineColorInvalid = new Gradient();

        [SerializeField]
        protected Gradient LineColorNoTarget = new Gradient();

        [SerializeField]
        protected Gradient LineColorLockFocus = new Gradient();

        [SerializeField]
        private BaseMixedRealityLineDataProvider lineBase;

        /// <summary>
        /// The Line Data Provider driving this pointer.
        /// </summary>
        public BaseMixedRealityLineDataProvider LineBase => lineBase;

        [SerializeField]
        [Tooltip("If no line renderers are specified, this array will be auto-populated on startup.")]
        private BaseMixedRealityLineRenderer[] lineRenderers;

        /// <summary>
        /// The current line renderers that this pointer is utilizing.
        /// </summary>
        /// <remarks>
        /// If no line renderers are specified, this array will be auto-populated on startup.
        /// </remarks>
        public BaseMixedRealityLineRenderer[] LineRenderers => lineRenderers;

        [Range(1, 50)]
        [SerializeField]
        [Tooltip("Number of rayStep steps to utilize in raycast operation along curve defined in LineBase. This setting has a high performance cost. Values above 20 are not recommended.")]
        protected int LineCastResolution = 10;

        protected Gradient GetLineGradient(TeleportSurfaceResult targetResult)
        {
            switch (targetResult)
            {
                case TeleportSurfaceResult.None:
                    return LineColorNoTarget;
                case TeleportSurfaceResult.Valid:
                    return LineColorValid;
                case TeleportSurfaceResult.Invalid:
                    return LineColorInvalid;
                case TeleportSurfaceResult.HotSpot:
                    return LineColorHotSpot;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetResult), targetResult, null);
            }
        }

        #endregion

        #region Parabollic Pointer Management

        [SerializeField]
        private float minParabolaVelocity = 1f;

        [SerializeField]
        private float maxParabolaVelocity = 5f;

        [SerializeField]
        private float minDistanceModifier = 1f;

        [SerializeField]
        private float maxDistanceModifier = 5f;

        [SerializeField]
        private ParabolaPhysicalLineDataProvider parabolicLineData;

        #endregion

        #region Base Pointer Management


        private IMixedRealityController controller;

        /// <inheritdoc />
        public IMixedRealityController Controller
        {
            get => controller;
            set
            {
                controller = value;
                if (value != null)
                {
                    PointerName = nameof(CustomTeleportPointer) + " " + Controller.ControllerHandedness;
                    InputSourceParent = value.InputSource;
                }
            }
        }

        public IMixedRealityInputSource InputSourceParent { get; private set; }

        private uint pointerId;

        /// <inheritdoc />
        public uint PointerId
        {
            get
            {
                if (pointerId == 0)
                {
                    pointerId = CoreServices.InputSystem.FocusProvider.GenerateNewPointerId();
                }

                return pointerId;
            }
        }

        private string pointerName = string.Empty;

        /// <inheritdoc />
        public string PointerName
        {
            get => pointerName;
            set
            {
                pointerName = value;
                if (this != null)
                {
                    gameObject.name = value;
                }
            }
        }

        public IMixedRealityCursor BaseCursor { get; set; }
        public ICursorModifier CursorModifier { get; set; }

        /// <inheritdoc />
        public bool IsInteractionEnabled => IsActive && !isTeleportRequestActive && TeleportRequestRaised && MixedRealityToolkit.IsTeleportSystemEnabled;

        public bool IsActive { get; set; }

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("The Y orientation of the pointer - used for rotation and navigation")]
        private float pointerOrientation = 0f;


        /// <inheritdoc />
        public float PointerOrientation
        {
            get
            {
                if (TeleportHotSpot != null &&
                    TeleportHotSpot.OverrideTargetOrientation &&
                    TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                {
                    return TeleportHotSpot.TargetOrientation;
                }

                return pointerOrientation + Rotation.eulerAngles.y;
            }
            set
            {
                pointerOrientation = value < 0
                    ? Mathf.Clamp(value, -360f, 0f)
                    : Mathf.Clamp(value, 0f, 360f);
            }
        }

        public bool IsFocusLocked { get; set; }

        /// <summary>
        /// Specifies whether the pointer's target position (cursor) is locked to the target object when focus is locked.
        /// Most pointers want the cursor to "stick" to the object when manipulating, so set this to true by default.
        /// </summary>
        public virtual bool IsTargetPositionLockedOnFocusLock { get; set; } = true;

        [SerializeField]
        private bool overrideGlobalPointerExtent = false;

        [SerializeField]
        [Tooltip("Maximum distance at which all pointers can collide with a GameObject, unless it has an override extent.")]
        private float pointerExtent = 10f;

        /// <summary>
        /// Maximum distance at which all pointers can collide with a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>, unless it has an override extent.
        /// </summary>
        public float PointerExtent
        {
            get
            {
                if (overrideGlobalPointerExtent)
                {
                    if (CoreServices.InputSystem?.FocusProvider != null)
                    {
                        return CoreServices.InputSystem.FocusProvider.GlobalPointingExtent;
                    }
                }

                return pointerExtent;
            }
            set
            {
                pointerExtent = value;
                overrideGlobalPointerExtent = false;
            }
        }

        [SerializeField]
        [Tooltip("The length of the pointer when nothing is hit")]
        private float defaultPointerExtent = 10f;

        /// <summary>
        /// The length of the pointer when nothing is hit.
        /// </summary>
        public float DefaultPointerExtent
        {
            get => Mathf.Min(defaultPointerExtent, PointerExtent);
            set => defaultPointerExtent = value;
        }

        /// <inheritdoc />
        public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

        /// <inheritdoc />
        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        /// <inheritdoc />
        public IMixedRealityFocusHandler FocusTarget { get; set; }

        /// <inheritdoc />
        public IPointerResult Result { get; set; }

        /// <summary>
        /// RayStep stabilizer used when calculating position of pointer end point.
        /// </summary>
        public IBaseRayStabilizer RayStabilizer { get; set; }

        /// <inheritdoc />
        public virtual SceneQueryType SceneQueryType { get; set; } = SceneQueryType.SimpleRaycast;

        public float SphereCastRadius { get; set; }
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        #endregion


        #region Audio Management
        [Header("Audio management")]
        [SerializeField]
        private AudioSource pointerAudioSource = null;

        [SerializeField]
        private AudioClip teleportRequestedClip = null;

        [SerializeField]
        private AudioClip teleportCompletedClip = null;
        #endregion

        #region Lifecycle Manageament

        protected void OnEnable()
        {
            // Disable renderers so that they don't display before having been processed (which manifests as a flash at the origin).
            var renderers = GetComponents<Renderer>();
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.enabled = false;
                }
            }

            CheckInitialization();

            if (gravityDistorter == null)
            {
                gravityDistorter = GetComponent<DistorterGravity>();
            }

            if (!lateRegisterTeleport)
            {
                CoreServices.TeleportSystem?.RegisterHandler<IMixedRealityTeleportHandler>(this);
            }
        }

        private void CheckInitialization()
        {
            validRayCastLayers = new LayerMask[] { ValidLayers };

            if (parabolicLineData == null)
            {
                parabolicLineData = gameObject.GetComponent<ParabolaPhysicalLineDataProvider>();
            }

            if (parabolicLineData.LineTransform == transform)
            {
                UnityEngine.Debug.LogWarning("Missing Parabolic line helper.\nThe Parabolic Teleport Pointer requires an empty GameObject child for calculating the parabola arc. Creating one now.");

                var pointerHelper = transform.Find("ParabolicLinePointerHelper");

                if (pointerHelper == null)
                {
                    pointerHelper = new GameObject("ParabolicLinePointerHelper").transform;
                    pointerHelper.transform.SetParent(transform);
                }

                pointerHelper.transform.localPosition = Vector3.zero;
                parabolicLineData.LineTransform = pointerHelper.transform;
            }

            if (lineBase == null)
            {
                lineBase = GetComponent<BaseMixedRealityLineDataProvider>();
            }

            if (lineBase == null)
            {
                UnityEngine.Debug.LogError($"No Mixed Reality Line Data Provider found on {gameObject.name}. Did you forget to add a Line Data provider?");
            }

            if (lineBase != null && (lineRenderers == null || lineRenderers.Length == 0))
            {
                lineRenderers = lineBase.GetComponentsInChildren<BaseMixedRealityLineRenderer>();
            }

            if (lineRenderers == null || lineRenderers.Length == 0)
            {
                UnityEngine.Debug.LogError($"No Mixed Reality Line Renderers found on {gameObject.name}. Did you forget to add a Mixed Reality Line Renderer?");
            }

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].enabled = true;
            }
        }

        protected IEnumerator Start()
        {
            if (lateRegisterTeleport)
            {
                if (CoreServices.TeleportSystem == null)
                {
                    yield return new WaitUntil(() => CoreServices.TeleportSystem != null);

                    // We've been destroyed during the await.
                    if (this == null)
                    {
                        yield break;
                    }

                    // The pointer's input source was lost during the await.
                    if (Controller == null)
                    {
                        // Since we manually manage this pointer, we dont need a specific controller to point to.
                        yield break;
                    }
                }

                if (CoreServices.InputSystem == null)
                {
                    yield return new WaitUntil(() => CoreServices.InputSystem != null);
                }

                lateRegisterTeleport = false;

                CoreServices.TeleportSystem.RegisterHandler<IMixedRealityTeleportHandler>(this);
            }
        }

        protected void OnDisable()
        {
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].enabled = false;
            }
            CoreServices.TeleportSystem?.UnregisterHandler<IMixedRealityTeleportHandler>(this);
        }

        #endregion


        #region IMixedRealityPointer Implementation


        private StabilizedRay stabilizedRay = new StabilizedRay(0.5f);
        private Ray stabilizationRay = new Ray();
        /// <inheritdoc />
        public void OnPreSceneQuery()
        {
            stabilizationRay.origin = transform.position;
            stabilizationRay.direction = transform.forward;
            stabilizedRay.AddSample(stabilizationRay);

            parabolicLineData.LineTransform.rotation = Quaternion.identity;
            parabolicLineData.Direction = stabilizedRay.StabilizedDirection;

            // when pointing straight up, upDot should be close to 1.
            // when pointing straight down, upDot should be close to -1.
            // when pointing straight forward in any direction, upDot should be 0.
            var upDot = Mathf.Clamp(Vector3.Dot(stabilizedRay.StabilizedDirection, Vector3.up) + 0.5f, -1f, 1f);

            var velocity = minParabolaVelocity;
            var distance = minDistanceModifier;

            // If we're pointing below the horizon, always use the minimum modifiers.
            if (upDot > 0f)
            {
                // Increase the modifier multipliers the higher we point.
                velocity = Mathf.Lerp(minParabolaVelocity, maxParabolaVelocity, upDot);
                distance = Mathf.Lerp(minDistanceModifier, maxDistanceModifier, upDot);
            }

            parabolicLineData.Velocity = velocity;
            parabolicLineData.DistanceMultiplier = distance;

            // Set up our rays
            // Turn off gravity so we get accurate rays
            GravityDistorter.enabled = false;

            PreUpdateLineRenderers();
            UpdateRays();

            // Re-enable gravity if we're looking at a hotspot
            GravityDistorter.enabled = (TeleportSurfaceResult == TeleportSurfaceResult.HotSpot);
        }

        protected virtual void UpdateRays()
        {
            // Make sure our array will hold
            if (Rays == null || Rays.Length != LineCastResolution)
            {
                Rays = new RayStep[LineCastResolution];
            }

            float stepSize = 1f / Rays.Length;
            Vector3 lastPoint = LineBase.GetUnClampedPoint(0f);
            for (int i = 0; i < Rays.Length; i++)
            {
                Vector3 currentPoint = LineBase.GetUnClampedPoint(stepSize * (i + 1));
                Rays[i].UpdateRayStep(ref lastPoint, ref currentPoint);
                lastPoint = currentPoint;
            }
        }

        protected virtual void PreUpdateLineRenderers()
        {
            lineBase.UpdateMatrix();

            // Set our first and last points
            if (IsFocusLocked && IsTargetPositionLockedOnFocusLock && Result != null)
            {
                // Make the final point 'stick' to the target at the distance of the target
                SetLinePoints(Position, Result.Details.Point);
            }
            else
            {
                SetLinePoints(Position, Position + Rotation * Vector3.forward * DefaultPointerExtent);
            }
        }

        protected void SetLinePoints(Vector3 startPoint, Vector3 endPoint)
        {
            lineBase.FirstPoint = startPoint;
            lineBase.LastPoint = endPoint;
        }

        /// <inheritdoc />
        public void OnPostSceneQuery()
        {
            if (currentInputPosition != Vector2.zero && Controller != null)
            {
                CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Controller.ControllerHandedness);
            }

            // Use the results from the last update to set our NavigationResult
            TeleportSurfaceResult = TeleportSurfaceResult.None;
            GravityDistorter.enabled = false;

            if (IsInteractionEnabled && Result != null)
            {
                LineBase.enabled = true;

                // If we hit something
                if (Result.CurrentPointerTarget != null)
                {
                    // Check if it's in our valid layers
                    if (((1 << Result.CurrentPointerTarget.layer) & ValidLayers.value) != 0)
                    {
                        // See if it's a hot spot
                        if (TeleportHotSpot != null && TeleportHotSpot.IsActive)
                        {
                            TeleportSurfaceResult = TeleportSurfaceResult.HotSpot;
                            // Turn on gravity, point it at hotspot
                            GravityDistorter.WorldCenterOfGravity = TeleportHotSpot.Position;
                            GravityDistorter.enabled = true;
                        }
                        else
                        {
                            // If it's NOT a hotspot, check if the hit normal is too steep
                            // (Hotspots override dot requirements)
                            TeleportSurfaceResult = Vector3.Dot(Result.Details.LastRaycastHit.normal, Vector3.up) > upDirectionThreshold
                                ? TeleportSurfaceResult.Valid
                                : TeleportSurfaceResult.Invalid;
                        }
                    }
                    else if (((1 << Result.CurrentPointerTarget.layer) & InvalidLayers) != 0)
                    {
                        TeleportSurfaceResult = TeleportSurfaceResult.Invalid;
                    }
                    else
                    {
                        TeleportSurfaceResult = TeleportSurfaceResult.None;
                    }

                    var clearWorldLength = Result.Details.RayDistance;

                    // Clamp the end of the parabola to the result hit's point
                    LineBase.LineEndClamp = LineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, LineCastResolution);
                    BaseCursor?.SetVisibility(TeleportSurfaceResult == TeleportSurfaceResult.Valid || TeleportSurfaceResult == TeleportSurfaceResult.HotSpot);
                }
                else
                {
                    BaseCursor?.SetVisibility(false);
                    LineBase.LineEndClamp = 1f;
                }

                // Set the line color
                for (int i = 0; i < LineRenderers.Length; i++)
                {
                    LineRenderers[i].LineColor = GetLineGradient(TeleportSurfaceResult);
                }
            }
            else
            {
                LineBase.enabled = false;
            }
        }

        public void OnPreCurrentPointerTargetChange() { }

        public void Reset()
        {
            if (gameObject == null) return;

            if (TeleportHotSpot != null)
            {
                CoreServices.TeleportSystem?.RaiseTeleportCanceled(this, TeleportHotSpot);
                TeleportHotSpot = null;
            }
            OnInputChanged(Vector2.zero, false);
            IsActive = false;
            IsFocusLocked = false;
            Controller = null;
            gameObject.SetActive(false);
        }

        protected void DoSceneQuery()
        {
            if (!IsActive) return;
            TeleportPointerData pointerData = new TeleportPointerData();
            TeleportPointerHitResult hitResult = new TeleportPointerHitResult();

            float rayStartDistance = 0;
            for (int i = 0; i < Rays.Length; i++)
            {
                if (CoreServices.InputSystem.RaycastProvider.Raycast(Rays[i], validRayCastLayers, true, out MixedRealityRaycastHit hitInfo))
                {
                    hitResult.Set(hitInfo, Rays[i], i, rayStartDistance + hitInfo.distance, true);
                    break;
                }
                rayStartDistance += Rays[i].Length;
            }

            pointerData.UpdateHit(this, hitResult);
            Result = pointerData;

            if (Result.CurrentPointerTarget != null)
            {
                TeleportHotSpot = new CustomTeleportHotspot
                {
                    Normal = Result.Details.Normal,
                    Position = Result.Details.Point
                };
            }
            else if (TeleportHotSpot != null)
            {
                TeleportHotSpot = null;
            }
        }

        #endregion IMixedRealityPointer Implementation

        #region IMixedRealityInputHandler Implementation

        /// <summary>
        /// Updates
        /// </summary>
        /// <param name="isActive">Whether the pointer is active.</param>
        /// <param name="teleportDirection">Direction to teleport.</param>
        public void UpdatePointer(bool isActive, Vector2 teleportDirection, bool autoActivate = true)
        {
            if (IsActive && !isActive && TeleportHotSpot != null)
            {
                CoreServices.TeleportSystem?.RaiseTeleportCanceled(this, TeleportHotSpot);
            }
            IsActive = isActive;

            // Call the pointer's OnPreSceneQuery function
            // This will give it a chance to prepare itself for raycasts
            // e.g., by building its Rays array
            OnPreSceneQuery();

            DoSceneQuery();

            // Call the pointer's OnPostSceneQuery function.
            // This will give it a chance to respond to raycast results
            // e.g., by updating its appearance.
            OnPostSceneQuery();

            OnInputChanged(teleportDirection, autoActivate);
        }

        private void OnInputChanged(Vector2 newInputSource, bool autoActivate)
        {
            // Don't process input if we've got an active teleport request in progress.
            if (isTeleportRequestActive || CoreServices.TeleportSystem == null)
            {
                return;
            }

            currentInputPosition = newInputSource;

            if (currentInputPosition.sqrMagnitude > InputThresholdSquared)
            {
                // Get the angle of the pointer input
                float angle = Mathf.Atan2(currentInputPosition.x, currentInputPosition.y) * Mathf.Rad2Deg;

                // Offset the angle so it's 'forward' facing
                angle += angleOffset;
                PointerOrientation = angle;

                if (!TeleportRequestRaised)
                {
                    float absoluteAngle = Mathf.Abs(angle);

                    if (absoluteAngle < teleportActivationAngle)
                    {
                        TeleportRequestRaised = true;

                        CoreServices.TeleportSystem?.RaiseTeleportRequest(this, TeleportHotSpot);
                        if (pointerAudioSource != null && teleportCompletedClip != null)
                        {
                            pointerAudioSource.PlayOneShot(teleportRequestedClip);
                        }
                    }
                    else if (canMove)
                    {
                        // wrap the angle value.
                        if (absoluteAngle > 180f)
                        {
                            absoluteAngle = Mathf.Abs(absoluteAngle - 360f);
                        }

                        // Calculate the offset rotation angle from the 90 degree mark.
                        // Half the rotation activation angle amount to make sure the activation angle stays centered at 90.
                        float offsetRotationAngle = 90f - rotateActivationAngle;

                        // subtract it from our current angle reading
                        offsetRotationAngle = absoluteAngle - offsetRotationAngle;

                        // if it's less than zero, then we don't have activation
                        if (offsetRotationAngle > 0)
                        {
                            // check to make sure we're still under our activation threshold.
                            if (offsetRotationAngle < 2 * rotateActivationAngle)
                            {
                                canMove = false;
                                // Rotate the camera by the rotation amount.  If our angle is positive then rotate in the positive direction, otherwise in the opposite direction.
                                MixedRealityPlayspace.RotateAround(CameraCache.Main.transform.position, Vector3.up, angle >= 0.0f ? rotationAmount : -rotationAmount);
                            }
                            else // We may be trying to strafe backwards.
                            {
                                // Calculate the offset rotation angle from the 180 degree mark.
                                // Half the strafe activation angle to make sure the activation angle stays centered at 180f
                                float offsetStrafeAngle = 180f - backStrafeActivationAngle;
                                // subtract it from our current angle reading
                                offsetStrafeAngle = absoluteAngle - offsetStrafeAngle;

                                // Check to make sure we're still under our activation threshold.
                                if (offsetStrafeAngle > 0 && offsetStrafeAngle <= backStrafeActivationAngle)
                                {
                                    canMove = false;
                                    var height = MixedRealityPlayspace.Position.y;
                                    var newPosition = -CameraCache.Main.transform.forward * strafeAmount + MixedRealityPlayspace.Position;
                                    newPosition.y = height;
                                    MixedRealityPlayspace.Position = newPosition;
                                }
                            }
                        }
                    }
                }
            }
            else if (autoActivate)
            {
                if (!canTeleport && !TeleportRequestRaised)
                {
                    // Reset the move flag when the user stops moving the joystick
                    // but hasn't yet started teleport request.
                    canMove = true;
                }

                if (canTeleport)
                {
                    canTeleport = false;
                    TeleportRequestRaised = false;

                    if (TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                        TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                    {
                        CoreServices.TeleportSystem?.RaiseTeleportStarted(this, TeleportHotSpot);
                        if (pointerAudioSource != null && teleportCompletedClip != null)
                        {
                            pointerAudioSource.PlayOneShot(teleportCompletedClip);
                        }
                    }
                }

                if (TeleportRequestRaised)
                {
                    canTeleport = false;
                    TeleportRequestRaised = false;
                    CoreServices.TeleportSystem?.RaiseTeleportCanceled(this, TeleportHotSpot);
                }
            }

            if (TeleportRequestRaised &&
                TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
            {
                canTeleport = true;
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region IMixedRealityTeleportHandler Implementation

        /// <inheritdoc />
        public virtual void OnTeleportRequest(TeleportEventData eventData)
        {
            // Only turn off the pointer if we're not the one sending the request
            if (eventData.Pointer.PointerId == PointerId)
            {
                isTeleportRequestActive = false;
                BaseCursor?.SetVisibility(true);
            }
            else
            {
                isTeleportRequestActive = true;
                BaseCursor?.SetVisibility(false);
            }
        }


        /// <inheritdoc />
        public virtual void OnTeleportStarted(TeleportEventData eventData)
        {
            // Turn off all pointers while we teleport.
            isTeleportRequestActive = true;
            BaseCursor?.SetVisibility(false);
        }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(TeleportEventData eventData)
        {
            isTeleportRequestActive = false;
            BaseCursor?.SetVisibility(false);
        }


        /// <inheritdoc />
        public virtual void OnTeleportCanceled(TeleportEventData eventData)
        {
            isTeleportRequestActive = false;
            BaseCursor?.SetVisibility(false);
        }

        #endregion IMixedRealityTeleportHandler Implementation

        #region IEquality Implementation
        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left != null && left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityPointer)obj);
        }

        private bool Equals(IMixedRealityPointer other)
        {
            return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)PointerId;
                hashCode = (hashCode * 397) ^ (PointerName != null ? PointerName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation


        #region Helper Classes

        /// <summary>
        /// Given that hotspots are the only way to teleport, we use a custom hotspot that moves along with the hit point.
        /// </summary>
        private class CustomTeleportHotspot : IMixedRealityTeleportHotSpot
        {
            public Vector3 Position { get; set; }
            public Vector3 Normal { get; set; }
            public bool IsActive { get; } = true;
            public bool OverrideTargetOrientation { get; } = false;
            public float TargetOrientation { get; } = 0f;
            public GameObject GameObjectReference { get; set; } = null;
        }

        /// <summary>
        /// Data result from pointer hit test.
        /// </summary>
        private class TeleportPointerData : IPointerResult
        {
            public Vector3 StartPoint { get; private set; }
            public FocusDetails Details { get; private set; }
            public GameObject CurrentPointerTarget { get; private set; }
            public GameObject PreviousPointerTarget { get; private set; }
            public int RayStepIndex { get; private set; }

            public void UpdateHit(CustomTeleportPointer teleportPointer, TeleportPointerHitResult hitResult)
            {
                if (hitResult.hitObject != CurrentPointerTarget)
                {
                    teleportPointer.OnPreCurrentPointerTargetChange();
                }

                PreviousPointerTarget = CurrentPointerTarget;

                float rayDistance;
                Vector3 hitPointOnObject;
                Vector3 hitNormalOnObject;

                if (hitResult.RayStepIndex >= 0)
                {
                    RayStepIndex = hitResult.RayStepIndex;
                    StartPoint = hitResult.RayStep.Origin;

                    rayDistance = hitResult.RayDistance;
                    hitPointOnObject = hitResult.hitPointOnObject;
                    hitNormalOnObject = hitResult.hitNormalOnObject;
                }
                else
                {
                    // If we don't have a valid rayStep cast, use the whole pointer rayStep.
                    RayStep firstStep = teleportPointer.Rays[0];
                    RayStep finalStep = teleportPointer.Rays[teleportPointer.Rays.Length - 1];
                    RayStepIndex = 0;

                    StartPoint = firstStep.Origin;

                    float rayDist = 0.0f;
                    for (int i = 0; i < teleportPointer.Rays.Length; i++)
                    {
                        rayDist += teleportPointer.Rays[i].Length;
                    }

                    rayDistance = rayDist;
                    hitPointOnObject = finalStep.Terminus;
                    hitNormalOnObject = -finalStep.Direction;
                }

                Vector3 pointInLocalSpace;
                Vector3 normalInLocalSpace;
                if (hitResult.hitObject != null)
                {
                    pointInLocalSpace = hitResult.hitObject.transform.InverseTransformPoint(hitPointOnObject);
                    normalInLocalSpace = hitResult.hitObject.transform.InverseTransformDirection(hitNormalOnObject);
                }
                else
                {
                    pointInLocalSpace = Vector3.zero;
                    normalInLocalSpace = Vector3.zero;
                }

                CurrentPointerTarget = hitResult.hitObject;
                PreviousPointerTarget = null;

                Details = new FocusDetails
                {
                    Object = hitResult.hitObject,

                    LastRaycastHit = hitResult.raycastHit,
                    LastGraphicsRaycastResult = hitResult.graphicsRaycastResult,

                    Point = hitPointOnObject,
                    PointLocalSpace = pointInLocalSpace,

                    Normal = hitNormalOnObject,
                    NormalLocalSpace = normalInLocalSpace,

                    RayDistance = rayDistance
                };
            }
        }

        /// <summary>
        /// Helper class for storing intermediate hit results. Should be applied to the PointerData once all
        /// possible hits of a pointer have been processed.
        /// </summary>
        private class TeleportPointerHitResult
        {
            public MixedRealityRaycastHit raycastHit;
            public RaycastResult graphicsRaycastResult;

            public GameObject hitObject;
            public Vector3 hitPointOnObject = Vector3.zero;
            public Vector3 hitNormalOnObject = Vector3.zero;

            public RayStep RayStep;
            public int RayStepIndex = -1;
            public float RayDistance;

            /// <summary>
            /// Set hit focus information from a physics raycast.
            /// </summary>
            public void Set(MixedRealityRaycastHit hit, RayStep rayStep, int rayStepIndex, float rayDistance, bool focusIndividualCompoundCollider)
            {
                raycastHit = hit;
                graphicsRaycastResult = default(RaycastResult);

                hitObject = focusIndividualCompoundCollider ? hit.collider.gameObject : hit.transform.gameObject;
                hitPointOnObject = hit.point;
                hitNormalOnObject = hit.normal;

                RayStep = rayStep;
                RayStepIndex = rayStepIndex;
                RayDistance = rayDistance;
            }
        }

        #endregion
    }
}
