// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.UX.BoundingBoxes
{
    public class BoundingBoxGizmoHandle : MonoBehaviour, IInputHandler
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
        private Color defaultColor = new Color(0, 0.486f, 0.796f, 1);
        private Color interactingColor = new Color(1.0f, 0.5f, 0.0f, 1);

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
                uint sourceId = inputDownEventData.SourceId;
                bool isPressed = false;
                double pressedValue = 0.0;
                inputDownEventData.InputSource.TryGetSelect(sourceId, out isPressed, out pressedValue);

                if (isPressed == false)
                {
                    inputDownEventData = null;
                    OnInputUp(null);
                    Rig.FocusOnHandle(null);
                }
                else
                {
                    Vector3 currentHandPosition = GetHandPosition(sourceId);

                    if (this.AffineType == TransformType.Scale)
                    {
                        float scalar = (currentHandPosition - objectToAffect.transform.position).magnitude / (objectToAffect.transform.position - initialHandPosition).magnitude;
                        Vector3 newScale = new Vector3(scalar, scalar, scalar);
                        newScale.Scale(initialScale);
                        objectToAffect.transform.localScale = newScale;
                    }
                    else if (this.AffineType == TransformType.Rotation)
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
                }
            }
        }

        private Vector3 GetHandPosition(uint sourceId)
        {
            Vector3 handPosition = new Vector3(0, 0, 0);
            inputDownEventData.InputSource.TryGetGripPosition(sourceId, out handPosition);
            return handPosition;
        }

        public void OnInputDown(InputEventData eventData)
        {
            inputDownEventData  = eventData;
            initialHandPosition = GetHandPosition(eventData.SourceId);
            initialScale        = objectToAffect.transform.localScale;
            initialOrientation  = objectToAffect.transform.rotation.eulerAngles;

            this.gameObject.GetComponent<Renderer>().material.color = interactingColor;
            Rig.FocusOnHandle(this.gameObject);

            eventData.Use();
        }
        public void OnInputUp(InputEventData eventData)
        {
            this.gameObject.GetComponent<Renderer>().material.color = Rig.UnselectedColor;
            inputDownEventData = null;
            Rig.FocusOnHandle(null);

            if (eventData != null)
            {
                eventData.Use();
            }
        }
    }
}
