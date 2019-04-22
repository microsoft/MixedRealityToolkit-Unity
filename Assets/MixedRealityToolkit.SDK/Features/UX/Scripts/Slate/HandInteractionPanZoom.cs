// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class HandInteractionPanZoom : BaseFocusHandler, IMixedRealityTouchHandler, IMixedRealityInputHandler, IMixedRealitySourceStateHandler
    {
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
            public IMixedRealityInputSource touchingSource = null;
            public IMixedRealityController currentController = null;
        }

        #region Serialized Fields
        [SerializeField]
        [FormerlySerializedAs("enabled")]
        private bool isEnabled = true;
        public bool Enabled { get => isEnabled; set => isEnabled = value; }

        [Header("Behavior")]
        [SerializeField]
        private bool enableZoom = false;
        [SerializeField]
        private bool lockHorizontal = false;
        [SerializeField]
        private bool lockVertical = false;
        [SerializeField]
        private bool wrapTexture = false;
        [SerializeField]
        private bool velocityActive = false;
        [SerializeField]
        [Range(0.0f, 0.99f)]
        private float velocityDampingX = 0.9f;
        [SerializeField]
        [Range(0.0f, 0.99f)]
        private float velocityDampingY = 0.9f;
        [SerializeField]
        [Range(0.0f, 99.0f)]
        private float panZoomSmoothing = 80.0f;

        [Header("Receiver Objects")]
        [SerializeField]
        [Tooltip("Each object listed must have a script that implements the IHandPanHandler interface or it will not receive events")]
        private GameObject[] panEventReceivers = null;


        [Header("Geometry")]
        [SerializeField]
        [Tooltip("If affordace geometry is desired to emphasize the touch points(leftPoint and rightPoint) and the center point between them (reticle), assign them here.")]
        private GameObject reticle = null;
        [SerializeField]
        private GameObject leftPoint = null;
        [SerializeField]
        private GameObject rightPoint = null;

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
        private bool affordancesVisible = false;
        private float runningAverageSmoothing = 0.0f;
        private const float percentToDecimal = 0.01f;
        private List<Vector2> unTransformedUVs = new List<Vector2>();
        private Dictionary<uint, HandPanData> handDataMap = new Dictionary<uint, HandPanData>();
        private List<IMixedRealityHandPanHandler> handlerInterfaces = new List<IMixedRealityHandPanHandler>();
        #endregion Private Properties


        #region MonoBehaviour Handlers
        private void Awake()
        {
            ValidityCheck();
        }
        private void Update()
        {
            if (isEnabled == true)
            {
                if (touchActive)
                {
                    if (affordancesVisible == false)
                    {
                        SetAffordancesActive(true);
                    }

                    foreach (uint key in handDataMap.Keys)
                    {
                        if (true == UpdateHandTouchingPoint(key))
                        {
                            MoveTouch(key);
                        }
                    }

                    totalUVOffset = GetUvOffset();
                }

                UpdateIdle();
                UpdateUVMapping();

                if (touchActive == false && affordancesVisible == true)
                {
                    SetAffordancesActive(false);
                }

                if (affordancesVisible)
                {
                    if (reticle != null)
                    {
                        reticle.transform.position = GetContactCenter();
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
        private bool UpdateHandTouchingPoint(uint sourceId)
        {
            Vector3 tryHandPoint = Vector3.zero;
            bool tryGetSucceeded = false;
            if (handDataMap.ContainsKey(sourceId) == true)
            {
                HandPanData data = handDataMap[sourceId];
                if (data.IsActive == true)
                {
                    if (data.IsSourceNear == true)
                    {
                        tryGetSucceeded = TryGetHandPositionFromController(data.currentController, TrackedHandJoint.IndexTip, out tryHandPoint);
                    }
                    else
                    {
                        tryGetSucceeded = TryGetHandPositionFromController(data.currentController, TrackedHandJoint.Palm, out tryHandPoint);
                    }
                    if (tryGetSucceeded == true)
                    {
                        tryHandPoint = SnapFingerToQuad(tryHandPoint);
                        Vector3 unfilteredTouchPt = (data.IsSourceNear == true) ? tryHandPoint : tryHandPoint + data.touchingRayOffset;
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
        private void ValidityCheck()
        {
            SetAffordancesActive(false);

            //check for boxcollider
            boxCollider = GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                Debug.Log("The GameObject that runs this script must have a BoxCollider attached.");
            }
            else
            {
                this.GetComponent<Renderer>().material.mainTexture.wrapMode = TextureWrapMode.Repeat;
            }

            //get event targets
            foreach (GameObject gameObject in panEventReceivers)
            {
                if (gameObject != null)
                {
                    IMixedRealityHandPanHandler handler = gameObject.GetComponent<IMixedRealityHandPanHandler>();
                    if (handler != null)
                    {
                        handlerInterfaces.Add(handler);
                    }
                }
            }

            //precache references
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
            if (touchActive == false)
            {
                if (Mathf.Abs(totalUVOffset.x) < 0.01f && Mathf.Abs(totalUVOffset.y) < 0.01f)
                {
                    totalUVOffset = Vector2.zero;
                }
                else
                {
                    if (velocityActive == true)
                    {
                        totalUVOffset = new Vector2(totalUVOffset.x * velocityDampingX, totalUVOffset.y * velocityDampingY);
                        FirePanning(0);
                    }
                }
            }
        }
        private void UpdateUVMapping()
        {
            if (velocityActive)
            {
                bool oobX = false;
                bool oobY = false;
                Vector2 tiling = new Vector2(1.0f, 1.0f);
                List<Vector2> uvs = new List<Vector2>();
                List<Vector2> uvsOrig = new List<Vector2>();
                mesh.GetUVs(0, uvs);
                uvsOrig.AddRange(uvs);
                float scaleUVDelta = 0.0f;
                Vector2 scaleUVCentroid = Vector2.zero;
                float currentContactRatio = 0.0f;

                if (scaleActive)
                {
                    //scaleUVCentroid = GetScaleUVCentroid();
                    scaleUVCentroid = GetDisplayedUVCentroid();
                    currentContactRatio = GetUVScaleFromTouches();

                    //todo fix this- totalUVScale resets to 1 when second hand touches.

                    //hint: totalUVOffset works correctly- make scaleUVDelta work the same way
                    scaleUVDelta = currentContactRatio / previousContactRatio;
                    previousContactRatio = currentContactRatio;
                }

                for (int i = 0; i < uvs.Count; ++i)
                {
                    if (scaleActive)
                    {
                        //this is where zoom is applied if Active
                        uvs[i] = ((uvs[i] - scaleUVCentroid) / scaleUVDelta) + scaleUVCentroid;
                    }


                    //this is where the Pan is applied
                    uvs[i] = new Vector2(uvs[i].x - (totalUVOffset.x / previousContactRatio), uvs[i].y + (totalUVOffset.y / previousContactRatio));


                    //Pan limits are applied here if specified
                    if (wrapTexture == false)
                    {
                        if (uvs[i].x > (1.0f / tiling.x) || uvs[i].x < -0.001f)
                        {
                            oobX = true;
                            totalUVOffset.x = 0.0f;
                        }

                        if (uvs[i].y > (1.0f / tiling.y) || uvs[i].y < -0.001f)
                        {
                            oobY = true;
                            totalUVOffset.y = 0.0f;
                        }
                    }
                }

                for (int i = 0; i < uvs.Count; ++i)
                {
                    uvs[i] = new Vector2(oobX ? uvsOrig[i].x : uvs[i].x, oobY ? uvsOrig[i].y : uvs[i].y);
                }

                mesh.uv = uvs.ToArray();
            }
        }
        private float GetUVScaleFromTouches()
        {
            if (scaleActive == false || initialTouchDistance == 0)
            {
                return 0.0f;
            }

            float uvScaleFromTouches = GetContactDistance() / initialTouchDistance;
            //uvScaleFromTouches = Mathf.Clamp(uvScaleFromTouches, 0.5f, 2.0f);

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
            if (touchActive && AreSourcesCompatible() == true)
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
            if (reticle != null)
            {
                reticle.SetActive(affordancesVisible);
            }
            if (leftPoint != null)
            {
                leftPoint.SetActive(affordancesVisible);
            }
            if (rightPoint != null)
            {
                rightPoint.SetActive(affordancesVisible);
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
        private Vector2 GetDisplayedUVCentroid()
        {
            List<Vector2> uvs = new List<Vector2>();
            mesh.GetUVs(0, uvs);
            Vector2 centroid = Vector2.zero;
            for (int i = 0; i < uvs.Count; ++i)
            {
                centroid += uvs[i];
            }

            return centroid /= (float)uvs.Count;
        }
        private float GetContactDistance()
        {
            if (scaleActive == false || handDataMap.Keys.Count < 2)
            {
                return 0.0f;
            }

            int index = 0;
            Vector2 a = Vector2.zero;
            Vector2 b = Vector2.zero;
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
            Vector2[] uvs = mesh.uv;
            Vector2 upperLeft = uvs[3];
            Vector2 upperRight = uvs[1];
            Vector2 lowerLeft = uvs[0];

            float magVertical = (lowerLeft - upperLeft).magnitude;
            float magHorizontal = (upperRight - upperLeft).magnitude;

            float v = Vector2.Dot(coord - upperLeft, (lowerLeft - upperLeft) / (magVertical * magVertical));
            float h = Vector2.Dot(coord - upperLeft, (upperRight - upperLeft) / (magHorizontal * magHorizontal));

            return new Vector2(h, v);
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
            Vector3 upperLeft = transform.TransformPoint(vertices[3]);
            Vector3 upperRight = transform.TransformPoint(vertices[1]);
            Vector3 lowerLeft = transform.TransformPoint(vertices[0]);

            float magVertical = (lowerLeft - upperLeft).magnitude;
            float magHorizontal = (upperRight - upperLeft).magnitude;
            if (magVertical != 0.0f && magHorizontal != 0.0f)
            {
                Vector3 verticalEdgeNorm = (lowerLeft - upperLeft) / magVertical;
                Vector3 horizontalEdgeNorm = (upperRight - upperLeft) / magHorizontal;
                //get dotproduct to determine distance ->then divide by length to get quad coord 0 to 1
                float v = Vector3.Dot(point - upperLeft, verticalEdgeNorm) / magVertical;
                float h = Vector3.Dot(point - upperLeft, horizontalEdgeNorm) / magHorizontal;
                quadCoord = new Vector2(h, v);
            }

            return quadCoord;
        }
        private Vector3 SnapFingerToQuad(Vector3 pointToSnap)
        {
            Vector3 planePoint = this.transform.TransformPoint(mesh.vertices[0]);
            Vector3 planeNormal = gameObject.transform.forward;

            return Vector3.ProjectOnPlane(pointToSnap - planePoint, planeNormal) + planePoint;
        }
        private void SetHandDataFromController(IMixedRealityController controller, bool isNear)
        {
            HandPanData data = new HandPanData();
            data.IsSourceNear = isNear;
            data.IsActive = true;
            data.touchingSource = controller.InputSource;
            data.currentController = controller;
            if (isNear == true)
            {
                if (TryGetHandPositionFromController(data.currentController, TrackedHandJoint.IndexTip, out Vector3 touchPosition) == true)
                {
                    data.touchingInitialPt = SnapFingerToQuad(touchPosition);
                    data.touchingPointSmoothed = data.touchingInitialPt;
                    data.touchingPoint = data.touchingInitialPt;
                }
            }
            else
            {
                if (TryGetHandRayPoint(controller, out Vector3 handRayPt) == true)
                {
                    data.touchingInitialPt = SnapFingerToQuad(handRayPt);
                    data.touchingPoint = data.touchingInitialPt;
                    data.touchingPointSmoothed = data.touchingInitialPt;
                    if (TryGetHandPositionFromController(data.currentController, TrackedHandJoint.Palm, out Vector3 touchPosition) == true)
                    {
                        data.touchingRayOffset = handRayPt - SnapFingerToQuad(touchPosition);
                    }
                }
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

            StartTouch(data.touchingSource.SourceId);
        }
        private bool TryGetHandPositionFromController(IMixedRealityController controller, TrackedHandJoint joint, out Vector3 position)
        {
            if (controller != null && controller.Visualizer is IMixedRealityHandVisualizer)
            {
                if ((controller.Visualizer as IMixedRealityHandVisualizer).TryGetJointTransform(joint, out Transform palm) == true)
                {
                    position = palm.position;
                    return true;
                }
            }
            else if (controller != null)
            {
                if (true == HandJointUtils.TryGetJointPose(joint, controller.ControllerHandedness, out MixedRealityPose pose))
                {
                    position = pose.Position;
                    return true;
                }
            }

            position = Vector3.zero;
            return false;
        }
        private IMixedRealityHandPanHandler[] GetInterfaces()
        {
            List<IMixedRealityHandPanHandler> interfaces = new List<IMixedRealityHandPanHandler>();
            GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var gameObject in gameObjects)
            {
                IMixedRealityHandPanHandler[] childrenInterfaces = gameObject.GetComponentsInChildren<IMixedRealityHandPanHandler>();
                foreach (var childInterface in childrenInterfaces)
                {
                    interfaces.Add(childInterface);
                }
            }

            return interfaces.ToArray();

        }
        #endregion Private Methods


        #region Internal State Handlers
        private void StartTouch(uint sourceId)
        {
            UpdateTouchUVOffset(sourceId);
            FirePanStarted(sourceId);
        }
        private void EndTouch(uint sourceId)
        {
            if (handDataMap.ContainsKey(sourceId) == true)
            {
                handDataMap.Remove(sourceId);
                FirePanEnded(0);
            }
        }
        private void EndAllTouches()
        {
            handDataMap.Clear();
            FirePanEnded(0);
        }
        private void MoveTouch(uint sourceId)
        {
            UpdateTouchUVOffset(sourceId);
            FirePanning(sourceId);
        }
        #endregion Internal State Handlers


        #region Fire Events to Listening Objects
        private void FirePanStarted(uint sourceId)
        {
            HandPanEventData eventData = new HandPanEventData(EventSystem.current);
            eventData.Initialize(handDataMap[sourceId].touchingSource, GetUvOffset());

            foreach (IMixedRealityHandPanHandler handler in handlerInterfaces)
            {
                if (handler != null)
                {
                    handler.OnPanStarted(eventData);
                }
            }
        }
        private void FirePanEnded(uint sourceId)
        {
            HandPanEventData eventData = new HandPanEventData(EventSystem.current);
            eventData.Initialize(null, Vector2.zero);

            foreach (IMixedRealityHandPanHandler handler in handlerInterfaces)
            {
                if (handler != null)
                {
                    handler.OnPanEnded(eventData);
                }
            }
        }
        private void FirePanning(uint sourceId)
        {
            if (handlerInterfaces.Count > 0 && handDataMap.ContainsKey(sourceId))
            {
                HandPanEventData eventData = new HandPanEventData(EventSystem.current);
                eventData.Initialize(handDataMap[sourceId].touchingSource, GetUvOffset());

                foreach (IMixedRealityHandPanHandler handler in handlerInterfaces)
                {
                    if (handler != null)
                    {
                        handler.OnPanning(eventData);
                    }
                }
            }
        }
        #endregion Fire Events to Listening Objects


        #region BaseFocusHandler Methods
        public override void OnFocusEnter(FocusEventData eventData) { }
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
            SetHandDataFromController(eventData.Controller, true);
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
        public void OnInputDown(InputEventData eventData)
        {
            EndTouch(eventData.SourceId);
            SetHandDataFromController(eventData.InputSource.Pointers[0].Controller, false);
            eventData.Use();
        }
        public void OnInputUp(InputEventData eventData)
        {
            EndTouch(eventData.SourceId);
            eventData.Use();
        }
        public void OnPositionInputChanged(InputEventData<Vector2> eventData) { }
        public void OnInputPressed(InputEventData<float> eventData) { }
        public void OnSourceDetected(SourceStateEventData eventData) { }
        public void OnSourceLost(SourceStateEventData eventData)
        {
            EndTouch(eventData.SourceId);
            eventData.Use();
        }
        #endregion IMixedRealityInputHandler Methods
    }
}
