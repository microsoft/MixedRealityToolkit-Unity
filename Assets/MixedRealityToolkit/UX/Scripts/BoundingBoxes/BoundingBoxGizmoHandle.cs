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

                if (this.AffineType == TransformType.Scale)
                {
                    CalculateScale(currentHandPosition);
                }
                else if (this.AffineType == TransformType.Rotation)
                {
                    CalculateRotation(currentHandPosition);
                }
            }
        }

        private Vector3 GetHandPosition(uint sourceId)
        {
            Vector3 handPosition = new Vector3(0, 0, 0);

            inputDownEventData.InputSource.TryGetGripPosition(sourceId, out handPosition);

            return handPosition;
        }
        private void CalculateScale(Vector3 currentHandPosition)
        {
            float scalar = (currentHandPosition - objectToAffect.transform.position).magnitude / (objectToAffect.transform.position - initialHandPosition).magnitude;
            Vector3 newScale = new Vector3(scalar, scalar, scalar);
            newScale.Scale(initialScale);
            objectToAffect.transform.localScale = newScale;
        }
        private void CalculateRotation(Vector3 currentHandPosition)
        {
            float scalar = (currentHandPosition - objectToAffect.transform.position).magnitude - (objectToAffect.transform.position - initialHandPosition).magnitude;
            scalar *= (4.0f * Mathf.Rad2Deg);
            Vector3 newRotation = new Vector3(initialOrientation.x, initialOrientation.y, initialOrientation.z);
            if (Axis == AxisToAffect.X)
            {
                newRotation += new Vector3(scalar, 0, 0);
            }
            else if (Axis == AxisToAffect.Y)
            {
                newRotation += new Vector3(0, scalar, 0);
            }
            else if (Axis == AxisToAffect.Z)
            {
                newRotation += new Vector3(0, 0, scalar);
            }
            objectToAffect.transform.localEulerAngles = newRotation;
        }

        public void OnInputDown(InputEventData eventData)
        {
            MixedRealityToolkit.InputModule.InputManager.Instance.PushModalInputHandler(gameObject);
            inputDownEventData = eventData;
            initialHandPosition = GetHandPosition(eventData.SourceId);
            initialScale        = objectToAffect.transform.localScale;
            initialOrientation  = objectToAffect.transform.rotation.eulerAngles;

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
