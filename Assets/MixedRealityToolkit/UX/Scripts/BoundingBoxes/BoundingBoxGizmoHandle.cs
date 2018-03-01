// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;

using UnityEngine;

namespace MixedRealityToolkit.UX.BoundingBoxes
{
    public class BoundingBoxGizmoHandle : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        public enum TransformType
        {
            Rotation,
            Scale
        };

        public enum AxisToAffect
        {
            X,
            Y,
            Z
        };

        public BoundingRig Rig;

        private GameObject objectToAffect;
        private TransformType affineType;
        private AxisToAffect axis;
        private Vector3 initialHandPosition;
        private Vector3 initialScale;
        private Vector3 initialOrientation;
        private Quaternion initialHandOrientation;
        private InputEventData inputDownEventData;

        public GameObject ObjectToAffect
        {
            get
            {
                return objectToAffect;
            }

            set
            {
                objectToAffect = value;
            }
        }
        public TransformType AffineType
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
        public AxisToAffect Axis
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

        private void Update()
        {
            if (inputDownEventData != null)
            {
                Vector3 currentHandPosition = GetHandPosition(inputDownEventData.SourceId);
                Quaternion currentHandOrientation = GetHandOrientation(inputDownEventData.SourceId);

                if (this.AffineType == TransformType.Scale)
                {
                    CalculateScale(currentHandPosition);
                }
                else if (this.AffineType == TransformType.Rotation)
                {
                    CalculateRotation(currentHandOrientation);
                }
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
        private void CalculateScale(Vector3 currentHandPosition)
        {
            float scalar = (currentHandPosition - objectToAffect.transform.position).magnitude / (objectToAffect.transform.position - initialHandPosition).magnitude;
            Vector3 newScale = new Vector3(scalar, scalar, scalar);
            newScale.Scale(initialScale);
            objectToAffect.transform.localScale = newScale;
        }
        private void CalculateRotation(Quaternion currentHandOrientation)
        {
            //{
            //    float scalar = (currentHandPosition - objectToAffect.transform.position).magnitude - (objectToAffect.transform.position - initialHandPosition).magnitude;
            //    scalar *= (4.0f * Mathf.Rad2Deg);
            //    Vector3 newRotation = new Vector3(initialOrientation.x, initialOrientation.y, initialOrientation.z);
            //    if (Axis == AxisToAffect.X)
            //    {
            //        newRotation += new Vector3(scalar, 0, 0);
            //    }
            //    else if (Axis == AxisToAffect.Y)
            //    {
            //        newRotation += new Vector3(0, scalar, 0);
            //    }
            //    else if (Axis == AxisToAffect.Z)
            //    {
            //        newRotation += new Vector3(0, 0, scalar);
            //    }
            //    objectToAffect.transform.localEulerAngles = newRotation;
            //}


            //
            //float angle = 0;
            //Vector3 axis = new Vector3(0, 0, 0);
            //newQ.ToAngleAxis(out angle, out axis);
            //if (Axis == AxisToAffect.X)
            //{
            //    axis += new Vector3(axis.x, 0, 0);
            //}
            //else if (Axis == AxisToAffect.Y)
            //{
            //    axis += new Vector3(0, axis.y, 0);
            //}
            //else if (Axis == AxisToAffect.Z)
            //{
            //    axis += new Vector3(0, 0, axis.z);
            //}
            //angle *= axis.magnitude;

            Matrix4x4 m = Matrix4x4.Rotate(initialHandOrientation);
            Vector3 initRay = new Vector3(0, 0, 1);
            initRay = m.MultiplyPoint(initRay);
            initRay.Normalize();

            m  = Matrix4x4.Rotate(currentHandOrientation);
            Vector3 currentRay = new Vector3(0, 0, 1);
            currentRay = m.MultiplyPoint(currentRay);
            currentRay.Normalize();

           

            float dot = Vector3.Dot(initRay, currentRay);
            dot = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (Mathf.Abs(initRay.y - currentRay.y) < Mathf.Abs(initRay.x - currentRay.x))
            {
                if (Vector3.Cross(initRay, currentRay).y > 0)
                {
                    dot = -dot;
                }
            }
            else
            {
                if (Vector3.Cross(initRay, currentRay).x > 0)
                {
                    dot = -dot;
                }
            }

            Vector3 newEulers = new Vector3(0,0,0);
            if (Axis == AxisToAffect.X)
            {
                newEulers = new Vector3(dot, 0, 0);
            }
            else if (Axis == AxisToAffect.Y)
            {
                newEulers = new Vector3(0, dot, 0);
            }
            else if (Axis == AxisToAffect.Z)
            {
                newEulers = new Vector3(0, 0, dot);
            }
            newEulers += initialOrientation;


            ObjectToAffect.transform.rotation = Quaternion.Euler(newEulers);
        }

        public void OnInputDown(InputEventData eventData)
        {
            MixedRealityToolkit.InputModule.InputManager.Instance.PushModalInputHandler(gameObject);
            inputDownEventData = eventData;
            initialHandPosition = GetHandPosition(eventData.SourceId);
            initialScale        = objectToAffect.transform.localScale;
            initialOrientation  = objectToAffect.transform.rotation.eulerAngles;
            initialHandOrientation = GetHandOrientation(eventData.SourceId);

            this.gameObject.GetComponent<Renderer>().material = Rig.InteractingMaterial;
            Rig.FocusOnHandle(this.gameObject);
            eventData.Use();
        }
        public void OnInputUp(InputEventData eventData)
        {
            inputDownEventData = null;

            if (this.AffineType == TransformType.Scale)
            {
                this.gameObject.GetComponent<Renderer>().material = Rig.ScaleHandleMaterial;
            }
            else
            {
                this.gameObject.GetComponent<Renderer>().material = Rig.RotateHandleMaterial;
            }

            MixedRealityToolkit.InputModule.InputManager.Instance.PopModalInputHandler();
            Rig.FocusOnHandle(null);

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
            if (eventData.SourceId == inputDownEventData.SourceId)
            {
                OnInputUp(null);
            }
            eventData.Use();
        }
    }
}
