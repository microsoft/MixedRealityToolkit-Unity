// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class HandInteractionPan : BaseFocusHandler, IMixedRealityTouchHandler, IMixedRealityPointerHandler
    {
        private enum TouchType
        {
            IndexFinger = 0,
            HandRay
        }

        #region Serialized Fields
        [SerializeField]
        private TouchType touchType = TouchType.IndexFinger;
        [SerializeField]
        [Tooltip("Each object listed must have a script that implements the IHandPanHandler interface or it will not receive events")]
        private GameObject[] panEventReceivers = null;
        [SerializeField]
        private bool lockHorizontal = false;
        [SerializeField]
        private bool lockVertical = false;
        [SerializeField]
        private bool wrapTexture = false;
        [SerializeField]
        private bool velocityActive = false;
        [SerializeField]
        private Vector2 velocityDamping = new Vector2(0.9f, 0.9f);
        [SerializeField]
        private bool springActive = false;
        [SerializeField]
        [FormerlySerializedAs("enabled")]
        private bool isEnabled = true;
        [SerializeField]
        private GameObject touchVisual = null;
        public bool Enabled { get => isEnabled; set => isEnabled = value; }
        #endregion Serialized Fields

        #region Private Properties
        private List<Vector2> uvs = new List<Vector2>();
        private List<Vector2> uvsTouchStart = new List<Vector2>();
        private MeshFilter meshFilter;
        private Mesh mesh;
        private Vector2 uvOffset = Vector2.zero;
        private Vector2 uvTotalOffset = Vector2.zero;
        private Vector3 touchingInitialPt;
        private Vector3 touchingPoint;
        private Vector2 touchingUV;
        private Vector2 touchingInitialUV;
        private Vector2 touchingUVOffset;
        private Vector2 touchingUVTotalOffset;
        private List<IMixedRealityHandPanHandler> handlerInterfaces = new List<IMixedRealityHandPanHandler>();
        private IMixedRealityPointer currentPointer = null;

        private bool TouchActive { get => (currentPointer != null); }

        #endregion Private Properties

        #region MonoBehaviour Handlers
        private void Awake()
        {
            ValidityCheck();
        }

        private void Update()
        {
            if (isEnabled == false)
            {
                return;
            }

            touchingPoint = Vector3.zero;
            if (currentPointer != null)
            {
                if (false == TryGetPointerPosition(currentPointer, out touchingPoint))
                {
                    return;
                }

                MoveTouch();
            }

            if (touchVisual != null)
            {
                touchVisual.transform.position = touchingPoint;
            }

            UpdateIdle();
            UpdateUVMapping();
        }

        #endregion MonoBehaviour Handlers

        #region Private Methods

        private bool TryGetPointerPosition(IMixedRealityPointer pointer, out Vector3 handRayPoint)
        {
            Vector3 planePoint = transform.TransformPoint(
                (mesh != null && mesh.vertices.Length > 0) ? mesh.vertices[0] : Vector3.zero);
            Vector3 planeNormal = transform.forward;

            if (touchType == TouchType.IndexFinger)
            {
                handRayPoint = Vector3.ProjectOnPlane(pointer.Position - planePoint, planeNormal) + pointer.Position;
                return true;
            }
            else if (touchType == TouchType.HandRay)
            {
                if (pointer.Result != null)
                {
                    Plane plane = new Plane(planeNormal, planePoint);

                    // Compute intersection of ray with quad plane.
                    // The ray may be outside of the object since we disable target position focus locking.
                    foreach (RayStep step in pointer.Rays)
                    {
                        if (MixedRealityRaycaster.RaycastPlanePhysicsStep(step, plane, out handRayPoint))
                        {
                            return true;
                        }
                    }

                    handRayPoint = pointer.Result.Details.Point;
                    return true;
                }
            }

            handRayPoint = Vector3.zero;
            return false;
        }

        private void ValidityCheck()
        {
            // check for box collider
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogWarning("The GameObject that runs this script must have a Collider attached.");
            }
            else
            {
                GetComponent<Renderer>().material.mainTexture.wrapMode = TextureWrapMode.Repeat;
            }

            // get event targets
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

            // precache references
            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                mesh = null;
                Debug.LogWarning($"The GameObject: '{gameObject.name}' does not have a Mesh component.");
            }
            else
            {
                mesh = meshFilter.mesh;
            }
        }

        private void UpdateIdle()
        {
            if (TouchActive == false)
            {
                if (Mathf.Abs(uvOffset.x) < 0.01f && Mathf.Abs(uvOffset.y) < 0.01f)
                {
                    uvOffset = Vector2.zero;
                }
                else
                {
                    if (velocityActive == true)
                    {
                        uvOffset = new Vector2(uvOffset.x * velocityDamping.x, uvOffset.y * velocityDamping.y);
                        FirePanning();
                    }

                    if (springActive == true)
                    {
                        List<Vector2> uvCurrent = new List<Vector2>();
                        GetMeshUVs(uvCurrent);
                        uvOffset = -(uvsTouchStart[0] - uvCurrent[0]) * 0.1f;
                        FirePanning();
                    }
                }
            }
        }

        private void UpdateTouchPoint()
        {
            Vector2 newUV = GetUVFromPoint(touchingPoint);

            uvOffset = newUV - touchingUV;
            uvOffset.y = -uvOffset.y;
            touchingUV = newUV;
        }

        private void UpdateUVMapping()
        {
            if (velocityActive)
            {
                Vector2 tiling = new Vector2(1.0f, 1.0f);
                List<Vector2> uvs = new List<Vector2>();
                List<Vector2> uvsOrig = new List<Vector2>();

                GetMeshUVs(uvs);
                uvsOrig.AddRange(uvs);

                bool oobX = false;
                bool oobY = false;

                //adjust all uv coordinates using uvOffset value
                for (int i = 0; i < uvs.Count; ++i)
                {
                    //this is the key line
                    uvs[i] -= uvOffset;

                    if (wrapTexture == false)
                    {
                        if (uvs[i].x > (1.0f / tiling.x) || uvs[i].x < -0.001f)
                        {
                            oobX = true;
                            uvOffset.x = 0.0f;
                        }

                        if (uvs[i].y > (1.0f / tiling.y) || uvs[i].y < -0.001f)
                        {
                            oobY = true;
                            uvOffset.y = 0.0f;
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

        private void GetMeshUVs(List<Vector2> uvs)
        {
            if (mesh != null)
            {
                mesh.GetUVs(0, uvs);
            }
            else
            {
                uvs.Clear();
            }
        }

        private Vector2 GetUVFromPoint(Vector3 point)
        {
            Vector2 uv = GetPlanePoint(point);
            uv = new Vector2(lockHorizontal ? 0.0f : uv.x, lockVertical ? 0.0f : uv.y);
            return uv;
        }

        private Vector2 GetPlanePoint(Vector3 point)
        {
            if (mesh == null)
            {
                return Vector2.zero;
            }

            Vector3[] vertices = mesh.vertices;
            Vector3 upperLeft = transform.TransformPoint(vertices[3]);
            Vector3 upperRight = transform.TransformPoint(vertices[1]);
            Vector3 lowerLeft = transform.TransformPoint(vertices[0]);

            float magVertical = (lowerLeft - upperLeft).magnitude;
            float magHorizontal = (upperRight - upperLeft).magnitude;

            float v = Vector3.Dot(point - upperLeft, (lowerLeft - upperLeft) / magVertical);
            float h = Vector3.Dot(point - upperLeft, (upperRight - upperLeft) / magHorizontal);
            v /= magVertical;
            h /= magHorizontal;
            return new Vector2(h, v);
        }

        private void DisconnectTouch()
        {
            currentPointer = null;
            touchingPoint = Vector3.zero;
            touchingInitialPt = Vector3.zero;
            touchingUV = Vector2.zero;
            touchingInitialUV = Vector2.zero;
            touchingUVOffset = Vector2.zero;
            EndTouch();
        }

        #endregion Private Methods

        #region Internal State Handlers

        private void StartTouch()
        {
            UpdateTouchPoint();
            GetMeshUVs(uvsTouchStart);
            FirePanStarted();
        }

        private void EndTouch()
        {
            touchingUVTotalOffset += uvOffset;
            FirePanEnded();
        }

        private void MoveTouch()
        {
            UpdateTouchPoint();
            FirePanning();
        }

        #endregion Internal State Handlers

        #region Fire Events to Listening Objects

        private void FirePanStarted()
        {
            HandPanEventData eventData = new HandPanEventData(EventSystem.current);
            eventData.Initialize(currentPointer == null ? null : currentPointer.InputSourceParent, uvOffset);

            foreach (IMixedRealityHandPanHandler handler in handlerInterfaces)
            {
                if (handler != null)
                {
                    handler.OnPanStarted(eventData);
                }
            }
        }

        private void FirePanEnded()
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

        private void FirePanning()
        {
            HandPanEventData eventData = new HandPanEventData(EventSystem.current);
            eventData.Initialize(currentPointer == null ? null : currentPointer.InputSourceParent, uvOffset);

            foreach (IMixedRealityHandPanHandler handler in handlerInterfaces)
            {
                if (handler != null)
                {
                    handler.OnPanning(eventData);
                }
            }
        }

        #endregion Fire Events to Listening Objects

        #region IMixedRealityTouchHandler implementation

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (touchType == TouchType.IndexFinger)
            {
                if (currentPointer == null)
                {
                    currentPointer = eventData.InputSource.Pointers[0];

                    TryGetPointerPosition(currentPointer, out touchingPoint);
                    touchingInitialPt = touchingPoint;

                    touchingUV = GetUVFromPoint(touchingPoint);
                    touchingInitialUV = touchingUV;
                    touchingUVOffset = touchingUVTotalOffset;

                    StartTouch();

                    eventData.Use();
                }
            }
        }
        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            if (touchType == TouchType.IndexFinger)
            {
                if (currentPointer == eventData.InputSource.Pointers[0])
                {
                    DisconnectTouch();
                    eventData.Use();
                }
            }
        }
        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData) { }
        #endregion IMixedRealityHandTrackHandler

        #region BaseFocusHandler Methods

        public override void OnFocusEnter(FocusEventData eventData) { }

        public override void OnFocusExit(FocusEventData eventData)
        {
            DisconnectTouch();
        }

        #endregion

        #region IMixedRealityPointerHandler implementation

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (touchType == TouchType.HandRay)
            {
                if (currentPointer == null)
                {
                    currentPointer = eventData.InputSource.Pointers[0];

                    TryGetPointerPosition(currentPointer, out touchingPoint);
                    touchingInitialPt = touchingPoint;

                    touchingUV = GetUVFromPoint(touchingPoint);
                    touchingInitialUV = touchingUV;
                    touchingUVOffset = touchingUVTotalOffset;

                    StartTouch();

                    // Pointer cursor needs to be able to move on focus lock when panning.
                    currentPointer.IsTargetPositionLockedOnFocusLock = false;

                    eventData.Use();
                }
            }
        }

    void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

    void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (touchType == TouchType.HandRay)
        {
            if (currentPointer == eventData.Pointer)
            {
                DisconnectTouch();
                eventData.Use();
            }
        }
    }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        #endregion
    }
}