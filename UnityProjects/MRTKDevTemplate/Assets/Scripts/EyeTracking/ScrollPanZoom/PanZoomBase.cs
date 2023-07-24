// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for sample. While nice to have, this documentation is not required for samples.
#pragma warning disable CS1591

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using Subsystems;
    using UnityEngine.XR;

    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>
    public abstract class PanZoomBase : StatefulInteractable
    {
        #region Variable declaration
        // PAN
        /// <summary>
        /// Ability to pan using your eye gaze without any additional input (e.g., air tap or 
        /// button presses).  
        /// </summary>
        internal bool autoGazePanIsActive = true;

        /// <summary>
        ///  Horizontal panning speed. For example: 0.1f for slow panning. 0.6f for fast panning.
        /// </summary>
        internal float panSpeedLeftRight; // Comment: This could be improved by using panning step sizes depending on the zoom level. 

        /// <summary>
        /// Vertical panning speed. For example: 0.1f for slow panning. 0.6f for fast panning.  
        /// </summary>
        internal float panSpeedUpDown;

        /// <summary>
        /// Minimal distance in x and y from center of the target (0, 0) to trigger panning. Thus, 
        /// values must range between 0 (always panning) and 0.5 (no panning).
        /// </summary>
        internal Vector2 minDistFromCenterForAutoPan = new Vector2(0.2f, 0.2f);

        // ZOOM
        /// <summary>
        /// Zoom acceleration defining the steepness of logistic speed function mapping.
        /// </summary>
        internal float ZoomAcceleration = 10f;

        /// <summary>
        ///  Maximum zoom speed. 
        /// </summary>
        [Tooltip("Maximum speed when zooming.")]
        internal float ZoomSpeedMax = 0.02f;

        /// <summary>
        ///  Minimum scale of the texture for zoom in - e.g., 0.5f (half the original size).
        /// </summary>
        internal float ZoomMinScale = 0.1f;

        /// <summary>
        ///  Maximum scale of the texture for zoom out - e.g., 1f (the original size) or 2.0f 
        ///  (double the original size).
        /// </summary>
        internal float ZoomMaxScale = 1.0f;

        /// <summary>
        /// Size of the GameObject's collider when being looked at.
        /// </summary>        
        internal Vector3? customColliderSizeOnLookAt = null;

        /// <summary>
        /// The idle size of the GameObject's collider if not being looked at.
        /// </summary>
        internal Vector3? originalColliderSize = null;

        /// <summary>
        /// Timed zoom: Once triggered, a zoom in/out will be performed for the given 
        /// amount of time in seconds.
        /// </summary>
        internal float timeInSecondsToZoom = 0.5f;

        /// <summary>
        /// To avoid sudden scrolling when looking around, you can enable this. This may cause scrolling to feel less responsive though.
        /// </summary>
        internal bool useSkimProof;

        /// <summary>
        /// Speed to indicate how quickly the signal is updated after skimming across the content. 
        /// </summary>
        internal float skimProofUpdateSpeedFromUser;

        // Private variables        
        protected Vector2 cursorPosition = new Vector2(0.5f, 0.5f);
        protected Vector2 scale;
        protected Vector2 offset = Vector2.zero;
        protected Vector2 offsetRateZoom = Vector2.zero;
        protected Vector2 offsetRatePan = Vector2.zero;
        public bool limitPanning = true;

        private float skimProofNormalFixator;
        private float skimproof_UpdateSpeed = 5f; // Speed for how fast the current fixation will trigger a scroll after looking around. 

        private BoxCollider myCollider;

        protected float zoomSpeed = 0.01f;
        protected float zoomDirection; // 1: Zoom in, -1: Zoom out
        protected int dynamicZoomInvert = 1; // To invert when to zoom in/out for the manipulation gesture
        private bool wasLookedAtBefore = false;
        private bool isNavigating = false;
        private Vector3 navigationPosition = Vector3.zero;

        internal bool IsZooming = false;
        internal bool ZoomGestureEnabledOnStartup = false;
        private bool handZoomEnabled = false;

        protected Vector3 originalRatio;
        protected Vector2 originalScale;
        #endregion

        /// <summary>
        /// Initializes the state of the interactable.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Computes the pan speed based on the distance from interactor position to the center of the interactable.
        /// </summary>
        protected abstract float ComputePanSpeed(float cursorPosInOneDir, float maxSpeed, float minDistFromCenterForAutoPan);

        /// <summary>
        /// Determines the sign of the zoom direction for zooming in and out.
        /// </summary>
        protected abstract int ZoomDir(bool zoomIn);

        /// <summary>
        /// Zooms in the GameObject.
        /// </summary>
        protected abstract void ZoomIn();

        /// <summary>
        /// Zooms out the GameObject.
        /// </summary>
        protected abstract void ZoomOut();

        /// <summary>
        /// Update pan zoom
        /// </summary>
        protected abstract void UpdatePanZoom();

        /// <summary>
        /// Determine the position of the cursor within the hitbox. 
        /// </summary>
        protected abstract bool UpdateCursorPosInHitBox(Vector3 hitPosition);

        protected virtual void Start()
        {
            // Init children
            Initialize();
            handZoomEnabled = ZoomGestureEnabledOnStartup;
            Initialize();
        }

        private Vector3 CustomColliderSizeOnLookAt
        {
            get
            {
                return customColliderSizeOnLookAt ?? myCollider.size;
            }
        }

        protected void AutoPan()
        {
            PanHorizontally(ComputePanSpeed(cursorPosition.x, panSpeedLeftRight, minDistFromCenterForAutoPan.x));
            PanVertically(ComputePanSpeed(cursorPosition.y, panSpeedUpDown, minDistFromCenterForAutoPan.y));
        }

        /// <summary>
        /// Scroll sideways.
        /// </summary>
        public void PanHorizontally(float speed)
        {
            offsetRatePan = new Vector2(Time.deltaTime * speed, offsetRatePan.y);
        }

        /// <summary>
        /// Scroll from top to bottom
        /// </summary>
        public void PanVertically(float speed)
        {
            offsetRatePan = new Vector2(offsetRatePan.x, Time.deltaTime * speed);
        }

        /// <summary>
        /// Enables zoom operations using hand based interators.
        /// </summary>
        public void EnableHandZoom()
        {
            handZoomEnabled = true;
        }

        /// <summary>
        /// Disables zoom operations using hand based interators.
        /// </summary>
        public void DisableHandZoom()
        {
            handZoomEnabled = false;
        }

        /// <summary>
        /// Resets the zoom speed and sets the correct zoom direction when first engaging with "zoom in/out"
        /// </summary>
        private void ZoomStart(bool zoomIn)
        {
            zoomSpeed = 0f;
            zoomDirection = ZoomDir(zoomIn);
        }

        private void ZoomInStart()
        {
            Debug.Log($"[{gameObject.name}] ZoomInStart: {scale}");
            ZoomStart(true);
        }

        private void ZoomOutStart()
        {
            Debug.Log($"[{gameObject.name}] ZoomOutStart: {scale}");
            ZoomStart(false);
        }

        /// <summary>
        /// No matter if the user is still looking at the currently active pan-zoom panel, once the "zoom stop" 
        /// action has been triggered, reset the active target.
        /// </summary>
        public void ZoomStop()
        {
            if (zoomSpeed != 0f)
            {
                zoomSpeed = 0f;
                zoomDirection = 0f;
            }
            IsZooming = false;
        }

        private void NavigationStart()
        {
            isNavigating = true;
            navigationPosition = Vector3.zero;
        }

        private void NavigationStop()
        {
            isNavigating = false;
            ZoomStop();
            ResetNormFixator();
            navigationPosition = Vector3.zero;
        }

        private void NavigationUpdate(Vector3 normalizedOffset)
        {
            if (!isNavigating)
            {
                NavigationStart();
            }

            // Let's rotate the normalized gesture offset based on camera rotation. Let's check if the zoom direction changed
            Vector3 transformPoint = normalizedOffset;
            // Rotate around the y axis
            transformPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.down) * transformPoint;

            // Rotate around the x axis
            transformPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.left) * transformPoint;

            // Rotate around the z axis
            transformPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.back) * transformPoint;

            if (navigationPosition.z >= 0f && transformPoint.z < 0f)
            {
                ZoomOutStart();
            }
            else if (navigationPosition.z <= 0f && transformPoint.z > 0f)
            {
                ZoomInStart();
            }

            navigationPosition = 5f * transformPoint;
        }

        private bool zoomUsingHandsActive = false;
        private Vector3 initialPalmPosition;
        private XRNode handUsedToZoom;

        [Tooltip("Toggles the direction of hand based zoom interactions")]
        [SerializeField]
        private bool invertPalmZoomDirection = true;

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (handZoomEnabled && IsZooming)
                {
                    var handsSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsSubsystem>();

                    if (handsSubsystem != null)
                    {
                        XRNode[] hands = { XRNode.RightHand, XRNode.LeftHand };
                        foreach (var hand in hands)
                        {
                            if (handsSubsystem.TryGetJoint(TrackedHandJoint.IndexTip, hand, out var palmJointPose))
                            {
                                if (!zoomUsingHandsActive)
                                {
                                    zoomUsingHandsActive = true;
                                    initialPalmPosition = new Vector3(palmJointPose.Pose.position.x,
                                        palmJointPose.Pose.position.y, palmJointPose.Pose.position.z);
                                    handUsedToZoom = hand;
                                }

                                if (handUsedToZoom == hand)
                                {
                                    Vector3 deltaPalm = invertPalmZoomDirection
                                        ? initialPalmPosition - palmJointPose.Pose.position
                                        : palmJointPose.Pose.position - initialPalmPosition;
                                    NavigationUpdate(deltaPalm);
                                }
                            }
                        }
                    }
                }

                foreach (var interactor in interactorsHovering)
                {
                    if (interactor is FuzzyGazeInteractor gaze)
                    {
                        // Let's make sure that the correct GameObject is targeted and update the pan and zoom parameters.
                        if (UpdateCursorPosInHitBox(gaze.PreciseHitResult.raycastHit.point))
                        {
                            // Dynamically increase hit box size once user looks at this target
                            if (!wasLookedAtBefore)
                            {
                                wasLookedAtBefore = true;

                                if ((originalColliderSize.HasValue) && (customColliderSizeOnLookAt.HasValue))
                                {
                                    MyCollider.size = CustomColliderSizeOnLookAt;
                                }
                            }

                            // Pan
                            AutomaticGazePanning();

                            // Zoom
                            UpdateZoom();
                            return;
                        }
                    }
                }

                // Dynamically decrease hit box size back to original once user isn't looking at this target anymore
                if (wasLookedAtBefore)
                {
                    wasLookedAtBefore = false;
                    if (originalColliderSize.HasValue)
                    {
                        MyCollider.size = originalColliderSize.Value;
                    }
                }
            }
            else if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Late)
            {
                // Update offset based on the animation rate and previous offset
                if (useSkimProof)
                {
                    if (isHovered)
                    {
                        IncrementNormFixator();
                    }
                    offset += offsetRatePan * skimProofNormalFixator + offsetRateZoom;
                }
                else
                {
                    offset += offsetRatePan + offsetRateZoom;
                }

                UpdatePanZoom();

                // Reset rate of change
                offsetRatePan = Vector2.zero;
                offsetRateZoom = Vector2.zero;
            }
        }

        #region Skim proofing
        internal void SetSkimProofUpdateSpeed(float newSpeed)
        {
            skimproof_UpdateSpeed = newSpeed / 1000f;
        }

        protected void ResetNormFixator()
        {
            skimProofNormalFixator = 0f;
        }

        public void IncrementNormFixator()
        {
            skimProofNormalFixator = Mathf.Clamp01(skimProofNormalFixator + skimproof_UpdateSpeed);
        }
        #endregion

        #region Panning
        /// <summary>
        /// If AutoGazePanIsActive is active, currently looked at content toward the border of the panel's border will move toward the panel's center.
        /// </summary>
        private void AutomaticGazePanning()
        {
            if (autoGazePanIsActive)
            {
                AutoPan();
            }
        }
        #endregion

        /// <summary>
        /// Returns the collider of this GameObject.
        /// </summary>
        protected BoxCollider MyCollider
        {
            get
            {
                if (myCollider == null)
                {
                    myCollider = GetComponent<BoxCollider>();
                }
                return myCollider;
            }
            set
            {
                myCollider = value;
            }
        }

        /// <summary>
        /// Update the vertical pan offset.
        /// </summary>
        /// <param name="speed">Pan speed</param>
        private void PanUpDown(float speed)
        {
            offsetRatePan = new Vector2(offsetRatePan.x, Time.deltaTime * speed);
        }

        /// <summary>
        /// Update the horizontal pan offset.
        /// </summary>
        /// <param name="speed">Pan speed</param>
        private void PanLeftRight(float speed)
        {
            offsetRatePan = new Vector2(Time.deltaTime * speed, offsetRatePan.y);
        }

        /// <summary>
        /// Perform zoom if this game object is the current pan-and-zoom target.
        /// </summary>
        private void UpdateZoom()
        {
            // Dynamic zoom speed based on normalized input.
            float dynamicZoom = 0.5f; // Default = 0.5 in case the zoom is triggered by a button (Hand gesture would provide a value between 0 and 1
            if (isNavigating)
            {
                dynamicZoom = Mathf.Abs(navigationPosition.z);
                zoomDirection = Mathf.Sign(navigationPosition.z) * dynamicZoomInvert;
            }

            if (zoomDirection != 0f)
            {
                // Following a logistic function; -0.5 because dynamicZoom [0,1];
                zoomSpeed = ZoomSpeedMax / (1f + Mathf.Pow(2.71828f, -ZoomAcceleration * (dynamicZoom - 0.5f))) * Time.deltaTime;
                zoomSpeed = Mathf.Clamp(zoomSpeed, 0f, ZoomSpeedMax);

                // Zoom in: Zoom toward zoom pivot + corrective pan motions
                if (zoomDirection < 0f)
                {
                    ZoomIn();
                }
                // Zoom out: For zoom out, we don't really need to apply any corrective pan motions as we can just return to the original texture offset.
                else
                {
                    ZoomOut();
                }
            }
        }

        protected Vector2 LimitScaling(Vector2 newScale)
        {
            // Clamp 2D scale vector to specified min and max values.
            if (newScale.x <= ZoomMinScale)
            {
                newScale = new Vector2(ZoomMinScale, (ZoomMinScale / originalRatio.x) * originalRatio.y);
            }
            else if (newScale.x >= ZoomMaxScale)
            {
                newScale = new Vector2(ZoomMaxScale, (ZoomMaxScale / originalRatio.x) * originalRatio.y);
            }

            // Same for the y dimension
            if (newScale.y <= ZoomMinScale)
            {
                newScale = new Vector2((ZoomMinScale / originalRatio.y) * originalRatio.x, ZoomMinScale);
            }
            else if (newScale.y >= ZoomMaxScale)
            {
                newScale = new Vector2((ZoomMaxScale / originalRatio.y) * originalRatio.x, ZoomMaxScale);
            }

            return newScale;
        }

        /// <summary>
        /// Begins a zoom in operation on the GameObject.
        /// </summary>
        public void ZoomIn_Timed()
        {
            StartCoroutine(ZoomAndStop(true));
        }

        /// <summary>
        /// Begins a zoom out operation on the GameObject.
        /// </summary>
        public void ZoomOut_Timed()
        {
            StartCoroutine(ZoomAndStop(false));
        }

        private IEnumerator ZoomAndStop(bool zoomIn)
        {
            // Start zoom
            IsZooming = true;
            if (zoomIn)
            {
                ZoomInStart();
            }
            else
            {
                ZoomOutStart();
            }

            // Wait
            yield return new WaitForSeconds(timeInSecondsToZoom);

            // Stop
            ZoomStop();
        }

        /// <summary>
        /// Wrapper to ease keeping parameters up-to-date
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal bool UpdateValues<T>(ref T objBase, T objLocal)
        {
            if (!EqualityComparer<T>.Default.Equals(objBase, objLocal))
            {
                objBase = objLocal;
                return true;
            }
            return false;
        }

        #region Handle input events
        /// <summary>
        /// Called when a user selects the GameObject.
        /// </summary>
        public void OnSelectEntered()
        {
            IsZooming = true;
        }

        /// <summary>
        /// Called when a user deselects the GameObject.
        /// </summary>
        public void OnSelectExited()
        {
            // Stop zoom
            zoomUsingHandsActive = false;
            IsZooming = false;
            NavigationStop();
        }
        #endregion
    }
}

#pragma warning restore CS1591
