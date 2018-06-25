// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Logic for the gizmo handles in Bounding Box
    /// </summary>
    public class BoundingBoxGizmoHandle : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        private BoundingBoxRig rig;
        private Transform transformToAffect;
        private BoundingBoxGizmoHandleTransformType affineType;
        private BoundingBoxGizmoHandleAxisToAffect axis;
        private Vector3 initialHandPosition;
        private Vector3 initialScale;
        private Vector3 initialPosition;
        private Vector3 initialOrientation;
        private Quaternion initialRotation;
        private Quaternion initialHandOrientation;
        private Vector3 initialScaleOrigin;
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

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque == false)
            {
                isHandRotationAvailable = false;
            }
#endif
            cachedRenderer = gameObject.GetComponent<Renderer>();
        }
        private void Update()
        {
            if (inputDownEventData != null)
            {
                Vector3 currentHandPosition = Vector3.zero;
                Quaternion currentHandOrientation = Quaternion.identity;

                //set values from hand
                currentHandPosition = GetHandPosition(inputDownEventData.SourceId);
                if (isHandRotationAvailable)
                {
                    currentHandOrientation = GetHandOrientation(inputDownEventData.SourceId);
                }

                //calculate affines
                if (this.AffineType == BoundingBoxGizmoHandleTransformType.Scale)
                {
                    ApplyScale(currentHandPosition);
                }
                else if (this.AffineType == BoundingBoxGizmoHandleTransformType.Rotation)
                {
                    if (isHandRotationAvailable && handMotionForRotation == BoundingBoxGizmoHandleHandMotionType.handRotatesToRotateObject)
                    {
                        ApplyRotation(currentHandOrientation);
                    }
                    else
                    {
                        ApplyRotation(currentHandPosition);
                    }
                }

                lastHandWorldPos = currentHandPosition;
            }
        }

        private Vector3 GetHandPosition(uint sourceId)
        {
            Vector3 handPosition = new Vector3(0, 0, 0);
            inputDownEventData.InputSource.TryGetGripPosition(sourceId, out handPosition);
            return handPosition;
        }
        private Quaternion GetHandOrientation(uint sourceId)
        {
            Quaternion handOrientation = Quaternion.identity;
            inputDownEventData.InputSource.TryGetGripRotation(sourceId, out handOrientation);
            return handOrientation;
        }
        private void ApplyScale(Vector3 currentHandPosition)
        {
            if ((transformToAffect.position - initialHandPosition).magnitude > minimumScaleNav)
            {
                float scaleScalar = (currentHandPosition - transformToAffect.position).magnitude / (transformToAffect.position - initialHandPosition).magnitude;
                scaleScalar = Mathf.Pow(scaleScalar, scaleRate);
                Vector3 changeScale = new Vector3(scaleScalar, scaleScalar, scaleScalar);
                changeScale = GetBoundedScaleChange(changeScale);

                Vector3 newScale = changeScale;
                newScale.Scale(initialScale);

                //scale from object center
                transformToAffect.localScale = newScale;

                //now handle offset
                Vector3 currentScaleOrigin = initialScaleOrigin;
                currentScaleOrigin.Scale(changeScale);
                Vector3 postScaleOffset = currentScaleOrigin - initialScaleOrigin;

                //translate so that scale is effectively from opposite corner
                transformToAffect.position = initialPosition - postScaleOffset;
            }
        }
        private void ApplyRotation(Quaternion currentHandOrientation)
        {
#if UNITY_2017_1_OR_NEWER
            Matrix4x4 m = Matrix4x4.Rotate(initialHandOrientation);
            Vector3 initRay = new Vector3(0, 0, 1);
            initRay = m.MultiplyPoint(initRay);
            initRay.Normalize();

            m = Matrix4x4.Rotate(currentHandOrientation);
            Vector3 currentRay = new Vector3(0, 0, 1);
            currentRay = m.MultiplyPoint(currentRay);
            currentRay.Normalize();

            float angle = Vector3.Dot(initRay, currentRay);
            angle = Mathf.Acos(angle) * Mathf.Rad2Deg;
            if (Mathf.Abs(initRay.y - currentRay.y) < Mathf.Abs(initRay.x - currentRay.x))
            {
                if (Vector3.Cross(initRay, currentRay).y > 0)
                {
                    angle = -angle;
                }
            }
            else if (Vector3.Cross(initRay, currentRay).x > 0)
            {
                angle = -angle;
            }

            if (rotationCoordinateSystem == BoundingBoxGizmoHandleRotationType.globalCoordinates)
            {
                Vector3 newEulers = (Axis == BoundingBoxGizmoHandleAxisToAffect.X ? new Vector3(angle, 0, 0) : Axis == BoundingBoxGizmoHandleAxisToAffect.Y ? new Vector3(0, angle, 0) : new Vector3(0, 0, angle));
                newEulers += initialOrientation;
                transformToAffect.rotation = Quaternion.Euler(newEulers);
            }
            else
            {
                Vector3 axis = (Axis == BoundingBoxGizmoHandleAxisToAffect.X ? new Vector3(1, 0, 0) : Axis == BoundingBoxGizmoHandleAxisToAffect.Y ? new Vector3(0, 1, 0) : new Vector3(0, 0, 1));
                transformToAffect.localRotation = initialRotation;
                transformToAffect.Rotate(axis, angle * 5.0f);
            }
#endif // UNITY_2017_1_OR_NEWER
        }
        private void ApplyRotation(Vector3 currentHandPosition)
        {
            if (RotateAroundPivot)
            {
                ApplyRotationPivot(currentHandPosition);
            }
            else
            {
                ApplyRotationContinuous(currentHandPosition);
            }
        }

        private void ApplyRotationContinuous(Vector3 currentHandPosition)
        {
            Vector3 initialRay = initialHandPosition - transformToAffect.position;
            initialRay.Normalize();

            Vector3 currentRay = currentHandPosition - transformToAffect.position;
            currentRay.Normalize();

            Vector3 delta = currentRay - initialRay;
            delta.Scale(rotationFromPositionScale);

            Vector3 newEulers = new Vector3(0, 0, 0);
            if (Axis == BoundingBoxGizmoHandleAxisToAffect.X)
            {
                newEulers = new Vector3(-delta.y, 0, 0);
            }
            else if (Axis == BoundingBoxGizmoHandleAxisToAffect.Y)
            {
                newEulers = new Vector3(0, delta.x, 0);
            }
            else if (Axis == BoundingBoxGizmoHandleAxisToAffect.Z)
            {
                newEulers = new Vector3(0, 0, delta.y);
            }
            if (IsLeftHandedRotation)
            {
                newEulers.Scale(new Vector3(-1.0f, -1.0f, -1.0f));
            }

            if (rotationCoordinateSystem == BoundingBoxGizmoHandleRotationType.globalCoordinates)
            {
                newEulers += initialOrientation;
                transformToAffect.rotation = Quaternion.Euler(newEulers);
            }
            else
            {
                Vector3 axis = (Axis == BoundingBoxGizmoHandleAxisToAffect.X ? new Vector3(1, 0, 0) : Axis == BoundingBoxGizmoHandleAxisToAffect.Y ? new Vector3(0, 1, 0) : new Vector3(0, 0, 1));
                transformToAffect.localRotation = initialRotation;
                float angle = newEulers.x != 0 ? newEulers.x : newEulers.y != 0 ? newEulers.y : newEulers.z;
                transformToAffect.Rotate(axis, angle * 2.0f);
            }
        }

        private void ApplyRotationPivot(Vector3 currentHandPosition)
        {
            Vector3 delta = currentHandPosition - lastHandWorldPos;

            if (delta.sqrMagnitude == 0) { return; }

            delta.Scale(rotationFromPositionScale);

            var pivotToHandleDir = (transform.position - transformToAffect.position).normalized;
            switch (Axis)
            {
                default:
                case BoundingBoxGizmoHandleAxisToAffect.X:
                    transformToAffect.Rotate(Vector3.right, Vector3.Dot(delta, Vector3.Cross(pivotToHandleDir, transformToAffect.right)), Space.Self);
                    break;
                case BoundingBoxGizmoHandleAxisToAffect.Y:
                    transformToAffect.Rotate(Vector3.up, Vector3.Dot(delta, Vector3.Cross(pivotToHandleDir, transformToAffect.up)), Space.Self);
                    break;
                case BoundingBoxGizmoHandleAxisToAffect.Z:
                    transformToAffect.Rotate(Vector3.forward, Vector3.Dot(delta, Vector3.Cross(pivotToHandleDir, transformToAffect.forward)), Space.Self);
                    break;
            }
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

            HoloToolkit.Unity.InputModule.InputManager.Instance.PopModalInputHandler();
            Rig.FocusOnHandle(null);
        }

        public void OnInputDown(InputEventData eventData)
        {
            inputDownEventData = eventData;

            initialHandPosition     = GetHandPosition(eventData.SourceId);
            lastHandWorldPos        = initialHandPosition;
            initialScale            = transformToAffect.localScale;
            initialPosition         = transformToAffect.position;
            initialOrientation      = transformToAffect.rotation.eulerAngles;
            initialRotation         = transformToAffect.rotation;
            initialHandOrientation  = GetHandOrientation(eventData.SourceId);
            initialScaleOrigin      = transformToAffect.position - this.transform.position;

            HoloToolkit.Unity.InputModule.InputManager.Instance.PushModalInputHandler(gameObject);

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
    }
}