// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Logic for the gizmo handles in Bounding Box
    /// </summary>
    public class BoundingBoxGizmoHandle : MonoBehaviour, IMixedRealityInputHandler, IMixedRealitySourceStateHandler
    {
        private BoundingBoxRig rig;
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

        private Transform transformToAffect;
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

        private BoundingBoxGizmoHandleTransformType affineType;
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

        private BoundingBoxGizmoHandleAxisToAffect axis;
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

        private float maxScale = 10.0f;
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

        private Vector3 initialScale;

        private Vector3 initialHandPosition;

        private InputEventData inputDownEventData;

        private Renderer cachedRenderer;

        private static IMixedRealityInputSystem inputSystem = null;
        protected static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());


        private void Start()
        {
            cachedRenderer = gameObject.GetComponent<Renderer>();
        }

        private void Update()
        {
            if (inputDownEventData != null)
            {
                rig.RecalculateCount = 1;
                Vector3 currentHandPosition = GetHandPosition(inputDownEventData.SourceId);
             
                if (this.AffineType == BoundingBoxGizmoHandleTransformType.Scale)
                {
                    ApplyScale(currentHandPosition);
                }
                else if (this.AffineType == BoundingBoxGizmoHandleTransformType.Rotation)
                {
                   ApplyRotation(currentHandPosition);
                }
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
            float startMag = (initialHandPosition - rigCentroid).magnitude * 1.0f;
            float newMag = (currentHandPosition - rigCentroid).magnitude * 1.0f;
            float ratio = (newMag / startMag);
            Vector3 newScale = initialScale * ratio;
            newScale = GetBoundedScaleChange(newScale);

            transformToAffect.localScale = newScale;
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

        private Vector3 GetWorldAxisFromRig(BoundingBoxGizmoHandleAxisToAffect fromAxisDesc)
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

        #region Event Handlers
        public void OnInputDown(InputEventData eventData)
        {
            inputDownEventData = eventData;

            initialHandPosition     = GetHandPosition(eventData.SourceId);
            initialScale            = transformToAffect.localScale;

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
        #endregion
    }
}