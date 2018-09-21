// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.InputSystem.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices.WindowsMixedReality;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Logic for the gizmo handles in Bounding Box
    /// </summary>
    public class BoundingBoxGizmoHandle : MonoBehaviour, IMixedRealityInputHandler, IMixedRealitySourceStateHandler
    {
        private BoundingBoxRig rig;
        private Transform transformToAffect;
        private BoundingBoxGizmoHandleTransformType affineType;
        private BoundingBoxGizmoHandleAxisToAffect axis;
        private Vector3 initialHandPosition;
        private Vector3 initialScale;
        private Vector3 initialPosition;
        private Vector3 initialScaleOrigin;
        private Quaternion initialRotation;
        private InputEventData inputDownEventData;
        private bool isHandRotationAvailable;
        private bool isLeftHandedRotation = false;
        private Vector3 rotationFromPositionScale = new Vector3(-300.0f, -300.0f, -300.0f);
        private float minimumScaleNav = 0.001f;
        private float scaleRate = 1.0f;
        private float maxScale = 10.0f;
        private BoundingBoxGizmoHandleRotationType rotationCoordinateSystem;
        private BoundingBoxGizmoHandleHandMotionType handMotionForRotation;
        private Vector3 lastHandWorldPos = Vector3.zero;

        private static IMixedRealityInputSystem inputSystem = null;
        protected static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

        public BoundingBoxGizmoHandleTransformType AffineType
        {
            get
            {
                return affineType;
            }

            set
            {
                affineType = value;
            }
        }
        public BoundingBoxGizmoHandleAxisToAffect Axis
        {
            get
            {
                return axis;
            }

            set
            {
                axis = value;
            }
        }
        public bool IsLeftHandedRotation
        {
            get
            {
                return isLeftHandedRotation;
            }

            set
            {
                isLeftHandedRotation = value;
            }
        }
        public Transform TransformToAffect
        {
            get
            {
                return transformToAffect;
            }

            set
            {
                transformToAffect = value;
            }
        }
        public BoundingBoxGizmoHandleRotationType RotationCoordinateSystem
        {
            get
            {
                return rotationCoordinateSystem;
            }
            set
            {
                rotationCoordinateSystem = value;
            }
        }
        public BoundingBoxGizmoHandleHandMotionType HandMotionForRotation
        {
            get
            {
                return handMotionForRotation;
            }
            set
            {
                handMotionForRotation = value;
            }
        }
        public float ScaleRate
        {
            get
            {
                return scaleRate;
            }
            set
            {
                scaleRate = value;
            }
        }
        public float MaxScale
        {
            get
            {
                return maxScale;
            }
            set
            {
                maxScale = value;
            }
        }
        public BoundingBoxRig Rig
        {
            get
            {
                return rig;
            }

            set
            {
                rig = value;
            }
        }
        public bool RotateAroundPivot { get; set; }

        private void Start()
        {
            isHandRotationAvailable = true;

            if (MixedReality.Toolkit.Core.Utilities.CameraCache.Main.clearFlags != CameraClearFlags.Skybox)
            {
                isHandRotationAvailable = false;
            }

            cachedRenderer = gameObject.GetComponent<Renderer>();
        }
        private void Update()
        {
            if (inputDownEventData != null)
            {
                Vector3 currentHandPosition = GetHandPosition(inputDownEventData.SourceId);
             
                //calculate affines
                if (this.AffineType == BoundingBoxGizmoHandleTransformType.Scale)
                {
                    ApplyScale(currentHandPosition);
                }
                else if (this.AffineType == BoundingBoxGizmoHandleTransformType.Rotation)
                {
                   ApplyRotation(currentHandPosition);
                }

                lastHandWorldPos = currentHandPosition;
            }
        }

        private Vector3 GetHandPosition(uint sourceId)
        {
            Vector3 handPosition = this.transform.position;

            Ray pointerRay = new Ray();
            if (inputDownEventData.InputSource.Pointers.Length > 0 &&
                true == inputDownEventData.InputSource.Pointers[0].TryGetPointingRay(out pointerRay))
            {
                float mag = (pointerRay.origin - this.transform.position).magnitude;
                handPosition = (pointerRay.origin + (pointerRay.direction * mag));
            }

            return handPosition;
        }

        private void ApplyScale(Vector3 currentHandPosition)
        {
            Vector3 rigCentroid = rig.RigCentroid;
            Vector3 cornerFrom = this.transform.position - rigCentroid;
            float startMag = (initialHandPosition - rigCentroid).magnitude;
            float newMag = (currentHandPosition - rigCentroid).magnitude;
            float ratio = newMag / startMag;

            Vector3 deltaScale = Vector3.one * ratio;
            Vector3 newScale = initialScale * ratio;
            newScale = GetBoundedScaleChange(newScale);

            //scale from object center
            transformToAffect.localScale = newScale;

            ////now handle offset
            //translate so that scale is effectively from opposite corner
           // Vector3 cornerTo = cornerFrom;
            //cornerTo *= ratio;
            //transformToAffect.position = initialPosition + (currentHandPosition - initialHandPosition);
        }

        private void ApplyRotation(Vector3 currentHandPosition)
        {
            Vector3 planeOrigin = rig.RigCentroid;
            Vector3 planeNormal = GetWorldAxisFromRig(this.axis);
            Vector3 initialGrabVector = (initialHandPosition - planeOrigin).normalized;
            Vector3 currentHandProj = PointToPlane(currentHandPosition, planeOrigin, planeNormal);
            Vector3 initialGrabProj = PointToPlane(initialHandPosition, planeOrigin, planeNormal);

            Vector3 newGrabVector = (currentHandPosition - planeOrigin).normalized;
            Vector3 newGrabVectorProj = (currentHandProj - planeOrigin).normalized;
            Vector3 initialGrabVectorProj = (initialGrabProj - planeOrigin).normalized;
            Quaternion toRotation = Quaternion.FromToRotation(initialGrabVectorProj, newGrabVectorProj);

            transformToAffect.rotation = toRotation * transformToAffect.rotation;
            initialHandPosition = currentHandPosition;
        }

        private Vector3 GetBoundedScaleChange(Vector3 scale)
        {
            Vector3 maximumScale = new Vector3(initialScale.x * maxScale, initialScale.y * maxScale, initialScale.z * maxScale);
            Vector3 intendedFinalScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
            intendedFinalScale.Scale(scale);
            if (intendedFinalScale.x > maximumScale.x || intendedFinalScale.y > maximumScale.y || intendedFinalScale.z > maximumScale.z)
            {
                return new Vector3(maximumScale.x / initialScale.x, maximumScale.y / initialScale.y, maximumScale.z / initialScale.z);
            }

            return scale;
        }

        private Vector3 PointToPlane(Vector3 pointPosition, Vector3 planePosition, Vector3 planeNormal)
        {
            Vector3 doubleCross = Vector3.Cross(planeNormal, pointPosition - planePosition);
            doubleCross = Vector3.Cross(planeNormal, doubleCross);
            doubleCross.Normalize();
            float mag = Vector3.Dot(pointPosition - planePosition, doubleCross);
            doubleCross *= mag;
            return doubleCross + planePosition;
        }

        public Vector3 GetWorldAxisFromRig(BoundingBoxGizmoHandleAxisToAffect fromAxisDesc)
        {
            if (fromAxisDesc == BoundingBoxGizmoHandleAxisToAffect.X)
            {
                return rig.transform.right;
            }
            else if (fromAxisDesc == BoundingBoxGizmoHandleAxisToAffect.Y)
            {
                return rig.transform.up;
            }
            else if (fromAxisDesc == BoundingBoxGizmoHandleAxisToAffect.Z)
            {
                return rig.transform.forward;
            }

            return new Vector3(1, 0, 0);
        }

        private Renderer cachedRenderer;

        private void ResetRigHandles()
        {
            inputDownEventData = null;

            if (this.AffineType == BoundingBoxGizmoHandleTransformType.Scale)
            {
                cachedRenderer.sharedMaterial = Rig.ScaleHandleMaterial;
            }
            else
            {
                cachedRenderer.sharedMaterial = Rig.RotateHandleMaterial;
            }

            InputSystem.PopModalInputHandler();
            Rig.FocusOnHandle(null);
        }

        public void OnInputDown(InputEventData eventData)
        {
            inputDownEventData = eventData;

            initialHandPosition     = GetHandPosition(eventData.SourceId);
            lastHandWorldPos        = initialHandPosition;
            initialScale            = transformToAffect.localScale;
            initialPosition         = transformToAffect.position;
            initialRotation         = transformToAffect.rotation;
            initialScaleOrigin      = initialPosition - this.transform.position;

            InputSystem.PushModalInputHandler(gameObject);

            cachedRenderer.sharedMaterial = Rig.InteractingMaterial;
            Rig.FocusOnHandle(this.gameObject);
            eventData.Use();
        }
        public void OnInputUp(InputEventData eventData)
        {
            inputDownEventData = null;
            ResetRigHandles();

            if (eventData != null)
            {
                eventData.Use();
            }
        }
        public void OnSourceDetected(SourceStateEventData eventData)
        {
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if ((inputDownEventData != null) &&
                (eventData.SourceId == inputDownEventData.SourceId))
            {
                inputDownEventData = null;
                ResetRigHandles();

                eventData.Use();
            }
        }

        public void OnInputPressed(InputEventData<float> eventData)
        {
        }

        public void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {

        }
    }
}