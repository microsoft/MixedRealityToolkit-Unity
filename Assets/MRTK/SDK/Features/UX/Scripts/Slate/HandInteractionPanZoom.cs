// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [AddComponentMenu("Scripts/MRTK/SDK/HandInteractionPanZoom")]
    public class HandInteractionPanZoom :
        BaseFocusHandler, IMixedRealityTouchHandler, IMixedRealityPointerHandler, IMixedRealitySourceStateHandler
    {
        /// <summary>
        /// Internal data stored for each hand or pointer.
        /// </summary>
        protected class HandPanData
        {
            public bool IsActive = true;
            public bool IsSourceNear = false;
            public Vector2 uvOffset = Vector2.zero;
            public Vector2 touchingQuadCoord = Vector2.zero;
            public Vector2 uvTotalOffset = Vector2.zero;
            public Vector3 touchingPoint = Vector3.zero;
            public Vector3 touchingPointSmoothed = Vector3.zero;
            public Vector3 touchingInitialPt = Vector3.zero;
            public Vector3 touchingRayOffset = Vector3.zero;
            public Vector2 touchingInitialUV = Vector2.zero;
            public Vector2 touchingUVOffset = Vector2.zero;
            public Vector2 touchingUVTotalOffset = Vector2.zero;
            public Vector3 initialProjectedOffset = Vector3.zero;
            public IMixedRealityInputSource touchingSource = null;
            public IMixedRealityController currentController = null;
            public IMixedRealityPointer currentPointer = null;
        }

        #region Serialized Fields

        [SerializeField]
        [FormerlySerializedAs("enabled")]
        private bool isEnabled = true;
        /// <summary>
        /// This Property sets and gets whether a the pan/zoom behavior is active.
        /// </summary>
        public bool Enabled { get => isEnabled; set => isEnabled = value; }

        [Header("Behavior")]
        [SerializeField]
        private bool enableZoom = false;

        [SerializeField]
        private bool lockHorizontal = false;

        [SerializeField]
        private bool lockVertical = false;

        [SerializeField]
        [Tooltip("If this is checked, Max Pan Horizontal and Max Pan Vertical are ignored.")]
        private bool unlimitedPan = true;

        [SerializeField]
        [Range(1.0f, 20.0f)]
        private float maxPanHorizontal = 2;

        [SerializeField]
        [Range(1.0f, 20.0f)]
        private float maxPanVertical = 2;

        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float minScale = 0.2f;

        [SerializeField]
        [Range(1.0f, 10.0f)]
        private float maxScale = 1.5f;

        [SerializeField]
        [Range(0.0f, 0.99f)]
        [Tooltip("a value of 0 results in panning coming to a complete stop when released.")]
        private float momentumHorizontal = 0.9f;

        [SerializeField]
        [Tooltip("a value of 0 results in panning coming to a complete stop when released.")]
        [Range(0.0f, 0.99f)]
        private float momentumVertical = 0.9f;

        [SerializeField]
        [Range(0.0f, 99.0f)]
        private float panZoomSmoothing = 80.0f;

        [Header("Visual affordance")]
        [SerializeField]
        [Tooltip("If affordance geometry is desired to emphasize the touch points(leftPoint and rightPoint) and the center point between them (reticle), assign them here.")]
        [FormerlySerializedAs("reticle")]
        private GameObject centerPoint = null;

        [SerializeField]
        private GameObject leftPoint = null;

        [SerializeField]
        private GameObject rightPoint = null;

        [Tooltip("When the slate is touched, what color to change on the ProximityLight center color override to. (Assumes the target material uses a proximity light and proximity light color override)")]
        [SerializeField]
        private Color proximityLightCenterColor = new Color(0.25f, 0.25f, 0.25f, 0.0f);

        [SerializeField]
        [Tooltip("Current scale value. 1 is the original 100%.")]
        private float currentScale;
        /// <summary>
        /// Current scale value. 1 is the original 100%.
        /// </summary>
        public float CurrentScale
        {
            get { return currentScale; }
        }

        /// <summary>
        /// Returns the current pan delta (pan value - previous pan value)
        /// in UV coordinates (0 being no pan, 1, being pan of the entire ) 
        /// </summary>
        public Vector2 CurrentPanDelta
        {
            get { return totalUVOffset; }
        }

        [Header("Events")]
        public PanUnityEvent PanStarted = new PanUnityEvent();
        public PanUnityEvent PanStopped = new PanUnityEvent();
        public PanUnityEvent PanUpdated = new PanUnityEvent();

        #endregion Serialized Fields

        #region Private Properties

        private Mesh mesh;
        private MeshFilter meshFilter;
        private BoxCollider boxCollider;
        private bool touchActive
        {
            get
            {
                return handDataMap.Count > 0;
            }
        }
        private bool scaleActive
        {
            get
            {
                return enableZoom && handDataMap.Count > 1;
            }
        }
        private float previousContactRatio = 1.0f;
        private float initialTouchDistance = 0.0f;
        private float lastTouchDistance = 0.0f;
        private Vector2 totalUVOffset = Vector2.zero;
        private Vector2 totalUVScale = Vector2.one;
        private bool affordancesVisible = false;
        private float runningAverageSmoothing = 0.0f;
        private const float percentToDecimal = 0.01f;
        private Material currentMaterial;
        private int proximityLightCenterColorID;
        private Color defaultProximityLightCenterColor;
        private List<Vector2> unTransformedUVs = new List<Vector2>();
        private Dictionary<uint, HandPanData> handDataMap = new Dictionary<uint, HandPanData>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<Vector2> uvsOrig = new List<Vector2>();
        private bool oldIsTargetPositionLockedOnFocusLock;

#if UNITY_2019_3_OR_NEWER
        // Quad meshes by default (in 2019 and higher) appear to follow the vertex order
        // specified here: https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
        // That is, LowerLeft->LowerRight->UpperLeft->UpperRight
        // Note that even though the example on that page is one that creates a quad manually
        // using a specific order of vertices, this order seems to be what a quad mesh defaults to.
        // This was discovered when looking into an issue on the SlateTests, which depend on the
        // projection math within this to be using the correct right and up vectors.
        private const int UpperLeftQuadIndex = 2;
        private const int UpperRightQuadIndex = 3;
        private const int LowerLeftQuadIndex = 0;
#else // !UNITY_2019_3_OR_NEWER
        // Quad meshes in 2018 and lower appear to follow a vertex order that looks like this:
        // [0] "(-0.5, -0.5, 0.0)"
        // [1] "(0.5, 0.5, 0.0)"
        // [2] "(0.5, -0.5, 0.0)"
        // [3] "(-0.5, 0.5, 0.0)"
        // That is, LowerLeft->UpperRight->LowerRight->UpperLeft
        // Note that the ifdefs only cover +/- 2019.3 because that was the min tested version
        // for Unity 2019 - this could very well be needed for 2019.2 and 2019.1, but with 2019.4
        // out at this point, support is mainly on the LTS release.
        private const int UpperLeftQuadIndex = 3;
        private const int UpperRightQuadIndex = 1;
        private const int LowerLeftQuadIndex = 0;
#endif

        #endregion Private Properties

        /// <summary>
        /// This function sets the pan and zoom back to their starting settings.
        /// </summary>
        public void Reset()
        {
            mesh.SetUVs(0, unTransformedUVs);
            totalUVOffset = Vector2.zero;
            totalUVScale = Vector2.one;
            initialTouchDistance = 0.0f;
        }

        #region MonoBehaviour Handlers

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (isEnabled)
            {
                if (touchActive)
                {
                    foreach (uint key in handDataMap.Keys)
                    {
                        if (UpdateHandTouchingPoint(key))
                        {
                            MoveTouch(key);
                        }
                    }

                    totalUVOffset = GetUvOffset();
                }

                UpdateIdle();
                UpdateUVMapping();

                if (!touchActive && affordancesVisible)
                {
                    SetAffordancesActive(false);
                }

                if (affordancesVisible)
                {
                    if (centerPoint != null)
                    {
                        centerPoint.transform.position = GetContactCenter();
                    }
                    if (leftPoint != null)
                    {
                        leftPoint.transform.position = GetContactForHand(Handedness.Left);
                    }
                    if (rightPoint != null)
                    {
                        rightPoint.transform.position = GetContactForHand(Handedness.Right);
                    }
                }
            }
        }

        #endregion MonoBehaviour Handlers

        #region Private Methods

        private bool TryGetMRControllerRayPoint(HandPanData data, out Vector3 rayPoint)
        {

            if (data.currentPointer != null && data.currentController != null && data.currentController.IsPositionAvailable)
            {
                rayPoint = data.touchingInitialPt + (SnapFingerToQuad(data.currentPointer.Position) - data.initialProjectedOffset);
                return true;
            }

            rayPoint = Vector3.zero;
            return false;
        }

        private bool UpdateHandTouchingPoint(uint sourceId)
        {
            Vector3 tryHandPoint = Vector3.zero;
            bool tryGetSucceeded = false;
            if (handDataMap.ContainsKey(sourceId))
            {
                HandPanData data = handDataMap[sourceId];

                if (data.IsActive)
                {
                    if (data.IsSourceNear)
                    {
                        tryGetSucceeded = TryGetHandPositionFromController(data.currentController, TrackedHandJoint.IndexTip, out tryHandPoint);
                    }
                    else
                    {
                        tryGetSucceeded = TryGetHandPositionFromController(data.currentController, TrackedHandJoint.Palm, out tryHandPoint);
                    }
                    if (!tryGetSucceeded)
                    {
                        tryGetSucceeded = TryGetMRControllerRayPoint(data, out tryHandPoint);
                    }

                    if (tryGetSucceeded)
                    {
                        tryHandPoint = SnapFingerToQuad(tryHandPoint);
                        Vector3 unfilteredTouchPt = data.IsSourceNear ? tryHandPoint : tryHandPoint + data.touchingRayOffset;
                        runningAverageSmoothing = panZoomSmoothing * percentToDecimal;
                        unfilteredTouchPt *= (1.0f - runningAverageSmoothing);
                        data.touchingPointSmoothed = (data.touchingPointSmoothed * runningAverageSmoothing) + unfilteredTouchPt;
                        data.touchingPoint = data.touchingPointSmoothed;
                    }
                }
            }

            return true;
        }

        private bool TryGetHandRayPoint(IMixedRealityController controller, out Vector3 handRayPoint)
        {
            if (controller != null &&
                 controller.InputSource != null &&
                 controller.InputSource.Pointers != null &&
                 controller.InputSource.Pointers.Length > 0 &&
                 controller.InputSource.Pointers[0].Result != null)
            {
                handRayPoint = controller.InputSource.Pointers[0].Result.Details.Point;
                return true;
            }

            handRayPoint = Vector3.zero;
            return false;
        }

        private void Initialize()
        {
            SetAffordancesActive(false);

            // Check for boxcollider
            boxCollider = GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                Debug.Log("The GameObject that runs this script must have a BoxCollider attached.");
            }
            else
            {
                Renderer renderer = this.GetComponent<Renderer>();
                Material material = (renderer != null) ? renderer.material : null;
                if ((material != null) && (material.mainTexture != null))
                {
                    material.mainTexture.wrapMode = TextureWrapMode.Repeat;
                }
            }

            // Get material
            currentMaterial = this.gameObject.GetComponent<Renderer>().material;
            proximityLightCenterColorID = Shader.PropertyToID("_ProximityLightCenterColorOverride");
            bool materialValid = currentMaterial != null && currentMaterial.HasProperty(proximityLightCenterColorID);
            defaultProximityLightCenterColor = materialValid ?
                currentMaterial.GetColor(proximityLightCenterColorID) :
                new Color(0.0f, 0.0f, 0.0f, 0.0f);

            // Precache references
            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.Log("The GameObject: " + this.gameObject.name + " " + "does not have a Mesh component.");
            }
            else
            {
                mesh = meshFilter.mesh;
            }

            mesh.GetUVs(0, unTransformedUVs);
        }

        private void UpdateIdle()
        {
            if (!touchActive)
            {
                if (Mathf.Abs(totalUVOffset.x) < 0.01f && Mathf.Abs(totalUVOffset.y) < 0.01f)
                {
                    totalUVOffset = Vector2.zero;
                }
                else
                {
                    totalUVOffset = new Vector2(totalUVOffset.x * momentumHorizontal, totalUVOffset.y * momentumVertical);
                    RaisePanning(0);
                }
            }
        }

        private void UpdateUVMapping()
        {
            Vector2 tiling = currentMaterial != null ? currentMaterial.mainTextureScale : new Vector2(1.0f, 1.0f);
            Vector2 uvTestValue;
            mesh.GetUVs(0, uvs);
            uvsOrig.Clear();
            uvsOrig.AddRange(uvs);
            float scaleUVDelta = 0.0f;
            Vector2 scaleUVCentroid = Vector2.zero;
            float currentContactRatio = 0.0f;

            if (scaleActive)
            {
                scaleUVCentroid = GetDisplayedUVCentroid(uvs);
                currentContactRatio = GetUVScaleFromTouches();
                scaleUVDelta = currentContactRatio / previousContactRatio;
                previousContactRatio = currentContactRatio;

                currentScale = totalUVScale.x / scaleUVDelta;

                // Test for scale limits
                if (currentScale > minScale && currentScale < maxScale)
                {
                    // Track total scale
                    totalUVScale /= scaleUVDelta;
                    for (int i = 0; i < uvs.Count; ++i)
                    {
                        // This is where zoom is applied if Active
                        uvs[i] = ((uvs[i] - scaleUVCentroid) / scaleUVDelta) + scaleUVCentroid;
                    }
                }
            }

            // Test for pan limits
            Vector2 uvDelta = new Vector2(totalUVOffset.x, -totalUVOffset.y);
            if (!unlimitedPan)
            {
                bool xLimited = false;
                bool yLimited = false;
                for (int i = 0; i < uvs.Count; ++i)
                {
                    uvTestValue = uvs[i] - uvDelta;
                    if (uvTestValue.x > tiling.x * maxPanHorizontal || uvTestValue.x < -(tiling.x * maxPanHorizontal))
                    {
                        xLimited = true;
                    }
                    if (uvTestValue.y > tiling.y * maxPanVertical || uvTestValue.y < -(tiling.y * maxPanVertical))
                    {
                        yLimited = true;
                    }
                }

                for (int i = 0; i < uvs.Count; ++i)
                {
                    uvs[i] = new Vector2(xLimited ? uvs[i].x : uvs[i].x - uvDelta.x, yLimited ? uvs[i].y : uvs[i].y - uvDelta.y);
                }
            }
            else
            {
                for (int i = 0; i < uvs.Count; ++i)
                {
                    uvs[i] -= uvDelta;
                }
            }

            mesh.SetUVs(0, uvs);
        }

        private float GetUVScaleFromTouches()
        {
            if (!scaleActive || initialTouchDistance == 0)
            {
                return 0.0f;
            }

            float uvScaleFromTouches = GetContactDistance() / initialTouchDistance;
            return uvScaleFromTouches;
        }

        private void UpdateTouchUVOffset(uint sourceId)
        {
            HandPanData data = handDataMap[sourceId];
            Vector2 currentQuadCoord = GetQuadCoordFromPoint(data.touchingPoint);
            data.uvOffset = currentQuadCoord - data.touchingQuadCoord;
            data.touchingQuadCoord = currentQuadCoord;
        }

        private Vector2 GetUvOffset()
        {
            if (touchActive && AreSourcesCompatible())
            {
                Vector2 offset = Vector2.zero;
                foreach (uint key in handDataMap.Keys)
                {
                    offset += handDataMap[key].uvOffset;
                }
                offset /= (float)handDataMap.Count;
                return offset;
            }
            return totalUVOffset;
        }

        private Vector3 GetContactCenter()
        {
            Vector3 center = Vector3.zero;

            if (handDataMap.Keys.Count > 0)
            {
                foreach (uint key in handDataMap.Keys)
                {
                    center += handDataMap[key].touchingPoint;
                }

                center /= (float)handDataMap.Keys.Count;
            }

            return center;
        }

        private void SetAffordancesActive(bool active)
        {
            affordancesVisible = active;
            if (centerPoint != null)
            {
                centerPoint.SetActive(affordancesVisible);
            }
            if (leftPoint != null)
            {
                leftPoint.SetActive(affordancesVisible);
            }
            if (rightPoint != null)
            {
                rightPoint.SetActive(affordancesVisible);
            }

            if (currentMaterial != null)
            {
                currentMaterial.SetColor(proximityLightCenterColorID, active ? proximityLightCenterColor : defaultProximityLightCenterColor);
            }
        }

        private Vector3 GetContactForHand(Handedness hand)
        {
            Vector3 handPoint = Vector3.zero;
            if (handDataMap.Keys.Count > 0)
            {
                foreach (uint key in handDataMap.Keys)
                {
                    if (handDataMap[key].currentController.ControllerHandedness == hand)
                    {
                        return handDataMap[key].touchingPoint;
                    }
                }
            }

            return handPoint;
        }

        private bool AreSourcesCompatible()
        {
            int score = 0;
            foreach (uint key in handDataMap.Keys)
            {
                score += handDataMap[key].IsSourceNear ? 1 : 0;
            }
            return (score == 0 || score == handDataMap.Keys.Count);
        }

        private bool AreSourcesNear()
        {
            foreach (uint key in handDataMap.Keys)
            {
                if (handDataMap[key].IsSourceNear == false)
                {
                    return false;
                }
            }
            return true;
        }

        private Vector3 GetTouchPoint()
        {
            if (touchActive)
            {
                Vector3 touchingPoint = Vector3.zero;
                foreach (uint key in handDataMap.Keys)
                {
                    touchingPoint += handDataMap[key].touchingPoint;
                }
                touchingPoint /= (float)handDataMap.Count;
                return touchingPoint;
            }
            return Vector3.zero;
        }

        private Vector2 GetScaleUVCentroid()
        {
            return GetUVFromPoint(GetTouchPoint());
        }

        private Vector2 GetDisplayedUVCentroid(List<Vector2> uvs)
        {
            Vector2 centroid = Vector2.zero;
            for (int i = 0; i < uvs.Count; ++i)
            {
                centroid += uvs[i];
            }

            return centroid /= (float)uvs.Count;
        }

        private float GetContactDistance()
        {
            if (!scaleActive || handDataMap.Keys.Count < 2)
            {
                return 0.0f;
            }

            int index = 0;
            Vector3 a = Vector3.zero;
            Vector3 b = Vector3.zero;
            foreach (uint key in handDataMap.Keys)
            {
                if (index == 0)
                {
                    a = handDataMap[key].touchingPoint;
                }
                else if (index == 1)
                {
                    b = handDataMap[key].touchingPoint;
                }

                index++;
            }

            return (b - a).magnitude;
        }

        private Vector2 GetQuadCoordFromPoint(Vector3 point)
        {
            Vector2 quadCoord = GetQuadCoord(point);
            quadCoord = new Vector2(lockHorizontal ? 0.0f : quadCoord.x, lockVertical ? 0.0f : quadCoord.y);
            return quadCoord;
        }

        private Vector2 GetUVFromQuadCoord(Vector2 coord)
        {
            Vector2 uvCoord = Vector2.zero;
            Vector2[] uvs = mesh.uv;
            Vector2 upperLeft = uvs[UpperLeftQuadIndex];
            Vector2 upperRight = uvs[UpperRightQuadIndex];
            Vector2 lowerLeft = uvs[LowerLeftQuadIndex];

            float magVertical = (lowerLeft - upperLeft).magnitude;
            float magHorizontal = (upperRight - upperLeft).magnitude;

            if (!Mathf.Approximately(0, magVertical) && !Mathf.Approximately(0, magHorizontal))
            {
                // Get coord projection on uv coordinates then divide by length to get quad coord 0 to 1
                uvCoord.x = Vector2.Dot(coord - upperLeft, upperRight - upperLeft) / (magHorizontal * magHorizontal);
                uvCoord.y = Vector2.Dot(coord - upperLeft, lowerLeft - upperLeft) / (magVertical * magVertical);
            }

            return uvCoord;
        }

        private Vector2 GetUVFromPoint(Vector3 point)
        {
            Vector2 quadCoord = GetQuadCoordFromPoint(point);
            return GetUVFromQuadCoord(quadCoord);
        }

        private Vector2 GetQuadCoord(Vector3 point)
        {
            Vector2 quadCoord = Vector2.zero;
            Vector3[] vertices = mesh.vertices;
            Vector3 upperLeft = transform.TransformPoint(vertices[UpperLeftQuadIndex]);
            Vector3 upperRight = transform.TransformPoint(vertices[UpperRightQuadIndex]);
            Vector3 lowerLeft = transform.TransformPoint(vertices[LowerLeftQuadIndex]);

            float magVertical = (lowerLeft - upperLeft).magnitude;
            float magHorizontal = (upperRight - upperLeft).magnitude;

            if (!Mathf.Approximately(0, magVertical) && !Mathf.Approximately(0, magHorizontal))
            {
                // Get point projection on vertices coordinates then divide by length to get quad coord 0 to 1
                quadCoord.x = Vector3.Dot(point - upperLeft, upperRight - upperLeft) / (magHorizontal * magHorizontal);
                quadCoord.y = Vector3.Dot(point - upperLeft, lowerLeft - upperLeft) / (magVertical * magVertical);
            }

            return quadCoord;
        }

        private Vector3 SnapFingerToQuad(Vector3 pointToSnap)
        {
            Vector3 planePoint = this.transform.TransformPoint(mesh.vertices[0]);
            Vector3 planeNormal = gameObject.transform.forward;

            return Vector3.ProjectOnPlane(pointToSnap - planePoint, planeNormal) + planePoint;
        }

        private void SetHandDataFromController(IMixedRealityController controller, IMixedRealityPointer pointer, bool isNear)
        {
            HandPanData data = new HandPanData();
            data.IsSourceNear = isNear;
            data.IsActive = true;
            data.touchingSource = controller.InputSource;
            data.currentController = controller;
            data.currentPointer = pointer;

            if (isNear)
            {
                if (TryGetHandPositionFromController(data.currentController, TrackedHandJoint.IndexTip, out Vector3 touchPosition))
                {
                    data.touchingInitialPt = SnapFingerToQuad(touchPosition);
                    data.touchingPointSmoothed = data.touchingInitialPt;
                    data.touchingPoint = data.touchingInitialPt;
                }
            }
            else // Is far
            {
                if (data.currentPointer is GGVPointer)
                {
                    data.touchingInitialPt = SnapFingerToQuad(data.currentPointer.Position);
                    data.touchingPoint = data.touchingInitialPt;
                    data.touchingPointSmoothed = data.touchingInitialPt;
                }
                else if (TryGetHandRayPoint(controller, out Vector3 handRayPt))
                {
                    data.touchingInitialPt = SnapFingerToQuad(handRayPt);
                    data.touchingPoint = data.touchingInitialPt;
                    data.touchingPointSmoothed = data.touchingInitialPt;
                    if (TryGetHandPositionFromController(data.currentController, TrackedHandJoint.Palm, out Vector3 touchPosition))
                    {
                        data.touchingRayOffset = handRayPt - SnapFingerToQuad(touchPosition);
                    }
                }
            }

            // Store value in case of MRController
            if (data.currentPointer != null)
            {
                Vector3 pt = data.currentPointer.Position;
                data.initialProjectedOffset = SnapFingerToQuad(pt);
            }

            data.touchingQuadCoord = GetUVFromPoint(data.touchingPoint);
            data.touchingInitialUV = data.touchingQuadCoord;
            data.touchingUVTotalOffset = totalUVOffset;
            data.touchingUVOffset = data.touchingUVTotalOffset;
            handDataMap.Add(data.touchingSource.SourceId, data);
            initialTouchDistance = GetContactDistance();
            lastTouchDistance = initialTouchDistance;
            totalUVOffset = Vector2.zero;

            if (handDataMap.Keys.Count > 1)
            {
                if (initialTouchDistance == 0)
                {
                    initialTouchDistance = GetContactDistance();
                }
                else
                {
                    float contactDist = GetContactDistance();
                    initialTouchDistance = contactDist + (initialTouchDistance - contactDist);
                }
                previousContactRatio = 1.0f;
            }

            SetAffordancesActive(isNear);

            StartTouch(data.touchingSource.SourceId);
        }

        private bool TryGetHandPositionFromController(IMixedRealityController controller, TrackedHandJoint joint, out Vector3 position)
        {
            var hand = controller as IMixedRealityHand;
            if (hand != null)
            {
                if (hand.TryGetJoint(joint, out MixedRealityPose pose))
                {
                    position = pose.Position;
                    return true;
                }
            }

            position = Vector3.zero;
            return false;
        }

        #endregion Private Methods

        #region Internal State Handlers

        private void StartTouch(uint sourceId)
        {
            UpdateTouchUVOffset(sourceId);
            RaisePanStarted(sourceId);
        }

        private void EndTouch(uint sourceId)
        {
            if (handDataMap.ContainsKey(sourceId))
            {
                handDataMap.Remove(sourceId);
                RaisePanEnded(0);
            }
        }

        private void EndAllTouches()
        {
            handDataMap.Clear();
            RaisePanEnded(0);
        }

        private void MoveTouch(uint sourceId)
        {
            UpdateTouchUVOffset(sourceId);
            RaisePanning(sourceId);
        }

        #endregion Internal State Handlers

        #region Fire Events to Listening Objects

        private void RaisePanStarted(uint sourceId)
        {
            HandPanEventData eventData = new HandPanEventData();
            eventData.PanDelta = GetUvOffset();
            PanStarted?.Invoke(eventData);
        }

        private void RaisePanEnded(uint sourceId)
        {
            HandPanEventData eventData = new HandPanEventData();
            eventData.PanDelta = Vector2.zero;
            PanStopped?.Invoke(eventData);
        }

        private void RaisePanning(uint sourceId)
        {
            HandPanEventData eventData = new HandPanEventData();
            eventData.PanDelta = GetUvOffset();
            PanUpdated?.Invoke(eventData);
        }

        #endregion Fire Events to Listening Objects

        #region BaseFocusHandler Methods

        /// <inheritdoc />
        public override void OnFocusEnter(FocusEventData eventData) { }

        /// <inheritdoc />
        public override void OnFocusExit(FocusEventData eventData)
        {
            EndAllTouches();
        }

        #endregion

        #region IMixedRealityTouchHandler
        /// <summary>
        /// In order to receive Touch Events from the IMixedRealityTouchHandler
        /// remember to add a NearInteractionTouchable script to the object that has this script.
        /// </summary>
        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            EndTouch(eventData.SourceId);
            SetHandDataFromController(eventData.Controller, null, true);
            eventData.Use();
        }

        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            EndTouch(eventData.SourceId);
            eventData.Use();
        }

        public void OnTouchUpdated(HandTrackingInputEventData eventData) { }

        #endregion IMixedRealityTouchHandler

        #region IMixedRealityInputHandler Methods

        /// <summary>
        /// The Input Event handlers receive Hand Ray events.
        /// </summary>
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            oldIsTargetPositionLockedOnFocusLock = eventData.Pointer.IsTargetPositionLockedOnFocusLock;
            if (!(eventData.Pointer is IMixedRealityNearPointer) && eventData.Pointer.Controller.IsRotationAvailable)
            {
                eventData.Pointer.IsTargetPositionLockedOnFocusLock = false;
            }
            SetAffordancesActive(false);
            EndTouch(eventData.SourceId);
            SetHandDataFromController(eventData.Pointer.Controller, eventData.Pointer, false);
            eventData.Use();
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            eventData.Pointer.IsTargetPositionLockedOnFocusLock = oldIsTargetPositionLockedOnFocusLock;
            EndTouch(eventData.SourceId);
            eventData.Use();
        }

        #endregion IMixedRealityInputHandler Methods

        #region IMixedRealitySourceStateHandler Methods
        public void OnSourceLost(SourceStateEventData eventData)
        {
            EndTouch(eventData.SourceId);
            eventData.Use();
        }

        #endregion IMixedRealitySourceStateHandler Methods

        #region Unused Methods

        public void OnSourceDetected(SourceStateEventData eventData) { }

        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

        #endregion Unused Methods
    }
}
