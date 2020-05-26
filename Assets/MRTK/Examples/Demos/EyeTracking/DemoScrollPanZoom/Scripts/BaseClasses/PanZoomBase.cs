// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public abstract class PanZoomBase : MonoBehaviour,
        IMixedRealityPointerHandler,
        IMixedRealityFocusHandler,
        IMixedRealitySourceStateHandler,
        IMixedRealityHandJointHandler
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
        internal Vector3? origColliderSize = null;

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
        internal float skimproof_UpdateSpeedFromUser;

        // Private variables        
        protected Vector2 cursorPos = new Vector2(0.5f, 0.5f);
        protected Vector2 scale;
        protected Vector2 offset = Vector2.zero;
        protected Vector2 offsetRate_Zoom = Vector2.zero;
        protected Vector2 offsetRate_Pan = Vector2.zero;
        public bool limitPanning = true;

        private float skimproof_normFixator = 0;
        private float skimproof_UpdateSpeed = 5f; // Speed for how fast the current fixation will trigger a scroll after looking around. 

        private BoxCollider myCollider;
        protected EyeTrackingTarget myEyeTarget;

        protected float zoomSpeed = 0.01f;
        protected float zoomDir = 0; // 1: Zoom in, -1: Zoom out
        protected int dynaZoomInvert = 1; // To invert when to zoom in/out for the manipulation gesture
        private bool wasLookedAtBefore = false;
        private bool isNavigating = false;
        private Vector3 navPos = Vector3.zero;
        private bool isFocused = false;
        internal bool isZooming = false;
        internal bool ZoomGestureEnabledOnStartup = false;
        private bool handZoomEnabled = false;

        protected Vector3 originalRatio;
        protected Vector2 originalPivot;
        protected Vector2 originalScale;
        protected Vector2 originalOffset;

        private IMixedRealityEyeSaccadeProvider eyeSaccadeProvider = null;

        protected IMixedRealityEyeSaccadeProvider EyeSaccadeProvider
        {
            get
            {
                if (eyeSaccadeProvider == null)
                {
                    eyeSaccadeProvider = CoreServices.GetInputSystemDataProvider<IMixedRealityEyeGazeDataProvider>()?.SaccadeProvider;
                }

                return eyeSaccadeProvider;
            }
        }

        #endregion

        public abstract void Initialize();
        public abstract float ComputePanSpeed(float cursorPosInOneDir, float maxSpeed, float minDistFromCenterForAutoPan);
        public abstract int ZoomDir(bool zoomIn);
        public abstract void ZoomIn();
        public abstract void ZoomOut();
        public abstract void UpdatePanZoom();
        public abstract bool UpdateCursorPosInHitBox();

        protected virtual void Start()
        {
            // Init children
            Initialize();
            handZoomEnabled = ZoomGestureEnabledOnStartup;
            Initialize();

            // Init eye tracking target
            if (myEyeTarget == null)
            {
                myEyeTarget = GetComponent<EyeTrackingTarget>();
            }

            if (EyeSaccadeProvider != null)
            {
                EyeSaccadeProvider.OnSaccadeY += ResetScroll_OnSaccade;
            }
        }
        private Vector3 CustomColliderSizeOnLookAt
        {
            get
            {
                if (customColliderSizeOnLookAt.HasValue)
                {
                    return customColliderSizeOnLookAt.Value;
                }
                else
                {
                    return myCollider.size;
                }
            }
        }

        public void AutoPan()
        {
            PanHorizontally(ComputePanSpeed(cursorPos.x, panSpeedLeftRight, minDistFromCenterForAutoPan.x));
            PanVertically(ComputePanSpeed(cursorPos.y, panSpeedUpDown, minDistFromCenterForAutoPan.y));
        }

        /// <summary>
        /// Scroll sideways.
        /// </summary>
        public void PanHorizontally(float speed)
        {
            offsetRate_Pan = new Vector2(Time.deltaTime * speed, offsetRate_Pan.y);
        }

        /// <summary>
        /// Scroll from top to bottom
        /// </summary>
        public void PanVertically(float speed)
        {
            offsetRate_Pan = new Vector2(offsetRate_Pan.x, Time.deltaTime * speed);
        }

        public void EnableHandZoom()
        {
            handZoomEnabled = true;
        }

        public void DisableHandZoom()
        {
            handZoomEnabled = false;
        }

        /// <summary>
        /// Resets the zoom speed and sets the correct zoom direction when first engaging with "zoom in/out"
        /// </summary>
        private void ZoomStart(bool zoomIn)
        {
            zoomSpeed = 0;
            zoomDir = ZoomDir(zoomIn);
        }

        public void ZoomInStart()
        {
            Debug.Log($"[{gameObject.name}] ZoomInStart: {scale.ToString()}");
            ZoomStart(true);
        }

        public void ZoomOutStart()
        {
            Debug.Log($"[{gameObject.name}] ZoomOutStart: {scale.ToString()}");
            ZoomStart(false);
        }

        /// <summary>
        /// No matter if the user is still looking at the currently active pan-zoom panel, once the "zoom stop" 
        /// action has been triggered, reset the active target.
        /// </summary>
        public void ZoomStop()
        {
            if (zoomSpeed != 0)
            {
                zoomSpeed = 0;
                zoomDir = 0;
            }
            isZooming = false;
        }

        private void NavigationStart()
        {
            isNavigating = true;
            navPos = Vector3.zero;
        }

        private void NavigationStop()
        {
            isNavigating = false;
            ZoomStop();
            ResetNormFixator();
            navPos = Vector3.zero;
        }

        private void NavigationUpdate(Vector3 normalizedOffset)
        {
            if (!isNavigating)
            {
                NavigationStart();
            }

            // Let's rotate the normalized gesture offset based on camera rotation. Let's check if the zoom direction changed
            Vector3 transfPnt = normalizedOffset;
            // Rotate around the y axis
            transfPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.down) * transfPnt;

            // Rotate around the x axis
            transfPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.left) * transfPnt;

            // Rotate around the z axis
            transfPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.back) * transfPnt;

            if ((navPos.z >= 0) && (transfPnt.z < 0))
            {
                ZoomOutStart();
            }
            else if ((navPos.z <= 0) && (transfPnt.z > 0))
            {
                ZoomInStart();
            }

            navPos = transfPnt * 5f;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // Let's make sure that the correct GameObject is targeted and update the pan and zoom parameters.
            if (UpdateCursorPosInHitBox())
            {
                // Dynamically increase hit box size once user looks at this target
                if (!wasLookedAtBefore)
                {
                    wasLookedAtBefore = true;

                    if ((origColliderSize.HasValue) && (customColliderSizeOnLookAt.HasValue))
                    {
                        MyCollider.size = CustomColliderSizeOnLookAt;
                    }
                }

                // Pan
                AutomaticGazePanning();

                // Zoom
                UpdateZoom();
            }
            else
            {
                // Dynamically decrease hit box size back to original once user isn't looking at this target anymore
                if (wasLookedAtBefore)
                {
                    wasLookedAtBefore = false;
                    if (origColliderSize.HasValue)
                    {
                        MyCollider.size = origColliderSize.Value;
                    }
                }
            }
        }

        #region Skim proofing
        internal void SetSkimProofUpdateSpeed(float newSpeed)
        {
            skimproof_UpdateSpeed = newSpeed / 1000f;
        }

        public void ResetNormFixator()
        {
            skimproof_normFixator = 0;
        }

        public void IncrementNormFixator()
        {
            skimproof_normFixator = Mathf.Clamp(skimproof_normFixator + skimproof_UpdateSpeed, 0, 1);
        }

        private void ResetScroll_OnSaccade()
        {
            ResetNormFixator();
        }
        #endregion

        /// <summary>
        /// Updating continuous pan and zoom based on the previously assigned values in Update().
        /// </summary>
        private void LateUpdate()
        {
            // Update offset based on the animation rate and previous offset
            if (useSkimProof)
            {
                if (isFocused)
                {
                    IncrementNormFixator();
                }
                offset += (offsetRate_Pan * skimproof_normFixator + offsetRate_Zoom);
            }
            else
            {
                offset += (offsetRate_Pan + offsetRate_Zoom);
            }

            UpdatePanZoom();

            // Reset rate of change
            offsetRate_Pan = new Vector2(0, 0);
            offsetRate_Zoom = new Vector2(0, 0);
        }

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
            offsetRate_Pan = new Vector2(offsetRate_Pan.x, Time.deltaTime * speed);
        }

        /// <summary>
        /// Update the horizontal pan offset.
        /// </summary>
        /// <param name="speed">Pan speed</param>
        private void PanLeftRight(float speed)
        {
            offsetRate_Pan = new Vector2(Time.deltaTime * speed, offsetRate_Pan.y);
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
                dynamicZoom = Mathf.Abs(navPos.z);
                zoomDir = Mathf.Sign(navPos.z) * dynaZoomInvert;
            }

            if (zoomDir != 0)
            {
                // Following a logistic function; -0.5 because dynamicZoom [0,1];
                zoomSpeed = ZoomSpeedMax / (1 + Mathf.Pow(2.71828f, -ZoomAcceleration * (dynamicZoom - 0.5f))) * Time.deltaTime;
                zoomSpeed = Mathf.Clamp(zoomSpeed, 0, ZoomSpeedMax);

                // Zoom in: Zoom toward zoom pivot + corrective pan motions
                if (zoomDir < 0)
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

        public void ZoomIn_Timed()
        {
            StartCoroutine(ZoomAndStop(true));
        }

        public void ZoomOut_Timed()
        {
            StartCoroutine(ZoomAndStop(false));
        }

        private IEnumerator ZoomAndStop(bool zoomIn)
        {
            // Start zoom
            isZooming = true;
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


        public void StartFocusing()
        {
            isFocused = true;
        }

        public void StopFocusing()
        {
            // Stop navigation if not focusing this window anymore
            isFocused = false;
            NavigationStop();
        }

        #region Handle MixedRealityToolkit input events
        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // Stop zoom
            isZooming = false;
            NavigationStop();
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            isZooming = true;
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData)
        {
            isFocused = true;
        }

        void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
        {
            // Stop zoom
            StopFocusing();
            eventData.Pointer.IsFocusLocked = false;
        }

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            foreach (var pointer in CoreServices.InputSystem.GazeProvider.GazeInputSource.Pointers)
            {
                pointer.IsFocusLocked = false;
            }
        }

        private bool zoomUsingHandsActive = false;
        private Vector3 initialPalmPos;
        private Handedness handUsedToZoom;
        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            if (handZoomEnabled && isZooming)
            {
                MixedRealityPose pose;
                eventData.InputData.TryGetValue(TrackedHandJoint.Palm, out pose);

                if (pose != null)
                {
                    if (!zoomUsingHandsActive)
                    {
                        zoomUsingHandsActive = true;
                        initialPalmPos = new Vector3(pose.Position.x, pose.Position.y, pose.Position.z);
                        handUsedToZoom = eventData.Handedness;
                    }

                    if (handUsedToZoom == eventData.Handedness)
                    {
                        Vector3 deltaPalm = pose.Position - initialPalmPos;
                        NavigationUpdate(deltaPalm);
                    }
                }
            }
            else
            {
                zoomUsingHandsActive = false;
            }
        }
        #endregion
    }
}