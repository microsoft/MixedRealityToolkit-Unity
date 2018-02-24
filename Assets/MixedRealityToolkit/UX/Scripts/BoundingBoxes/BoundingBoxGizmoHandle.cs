using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;

namespace MixedRealityToolkit.InputModule.Utilities.Interations
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
                        objectToAffect.transform.rotation = Quaternion.Euler(newRotation);
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

            //GameObject lightGameObject = new GameObject("HandleLight");
            //Light lightComp = lightGameObject.AddComponent<Light>();
            //lightComp.color = Color.white;
            //lightComp.intensity = 10.0f;
            //lightComp.type = LightType.Point;
            //lightGameObject.transform.position = GameObject.Find("DefaultCursor").transform.position;
            //lightGameObject.transform.parent = this.gameObject.transform;

            this.gameObject.GetComponent<Renderer>().material.color = new Color(1, 0.5f, 0);

            Rig.FocusOnHandle(this.gameObject);

            eventData.Use();
        }
        public void OnInputUp(InputEventData eventData)
        {
            this.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
           // GameObject.Destroy(GameObject.Find("HandleLight"));
            inputDownEventData = null;
            Rig.FocusOnHandle(null);
            eventData.Use();
        }
    }
}
