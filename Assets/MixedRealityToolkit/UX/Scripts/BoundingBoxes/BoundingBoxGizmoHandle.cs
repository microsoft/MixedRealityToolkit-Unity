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
        private bool isDragging = false;
        private Vector3 initialVector;
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
           // if (isDragging)
         //   {
            //    Vector3 newScale = this.gameObject.transform.position - ObjectToAffect.transform.position;
            //    float mag = newScale.magnitude / initialVector.magnitude;
            //    ObjectToAffect.transform.localScale.Scale(new Vector3(mag, mag, mag));


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
            }




          //      GameObject textMesh = GameObject.Find("textOut");
          //      textMesh.GetComponent<TextMesh>().text = this.name + " len:" + mag.ToString();
           // }
        }
        public void OnInputDown(InputEventData eventData)
        {
            isDragging = true;
            initialVector = this.gameObject.transform.position - ObjectToAffect.transform.position;
            inputDownEventData = eventData;
            Rig.FocusOnHandle(this.gameObject);
        }
        public void OnInputUp(InputEventData eventData)
        {
            isDragging = false;
            inputDownEventData = null;
            Rig.FocusOnHandle(null);
        }
    }
}
