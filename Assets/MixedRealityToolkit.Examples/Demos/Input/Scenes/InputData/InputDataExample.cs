using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class InputDataExample : MonoBehaviour
    {
        public Text inputUtilsText;
        public Text rawDataText;

        private Tuple<InputSourceType, Handedness> [] inputSources = new Tuple<InputSourceType, Handedness>[]
        {
            new Tuple<InputSourceType, Handedness>(InputSourceType.Controller, Handedness.Right) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Controller, Handedness.Left) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Eyes, Handedness.Any) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Head, Handedness.Any) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Hand, Handedness.Left) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Hand, Handedness.Right)
        };

        // Update is called once per frame
        void Update()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tuple in inputSources)
            {
                var sourceType = tuple.Item1;
                var handedness = tuple.Item2;
                sb.Append(sourceType.ToString() + " ");
                if (handedness != Handedness.Any)
                {
                    sb.Append(handedness.ToString());
                }
                sb.Append(": ");
                Ray myRay;
                if (InputUtils.TryGetRay(sourceType, handedness, out myRay))
                {
                    sb.Append($"pos: ({myRay.origin.x:F2}, {myRay.origin.y:F2}, {myRay.origin.z:F2}");
                    sb.Append($" forward: ({myRay.direction.x:F2}, {myRay.direction.y:F2}, {myRay.direction.z:F2}");
                }
                else
                {
                    sb.Append(" not available");
                }
                sb.AppendLine();
            }
            inputUtilsText.text = sb.ToString();

            // Iterate through all controllers and print the position of each input
            sb.Clear();
            foreach(var controller in CoreServices.InputSystem.DetectedControllers)
            {
                sb.AppendLine("Inputs for " + controller.InputSource.SourceName);
                sb.AppendLine();
                // Interactions for a controller is the list of inputs that this controller exposes
                foreach(MixedRealityInteractionMapping inputMapping in controller.Interactions)
                {
                    // 6DOF controllers support the "SpatialPointer" type (pointing direction)
                    // or "GripPointer" type (direction of the 6DOF controller)
                    if (inputMapping.InputType == DeviceInputType.SpatialPointer)
                    {
                        Debug.Log("spatial pointer PositionData: " + inputMapping.PositionData);
                        Debug.Log("spatial pointer RotationData: " + inputMapping.RotationData);
                    }
                    sb.AppendLine("\tDescription: " + inputMapping.Description);
                    sb.Append("\tAxisType: " + inputMapping.AxisType);
                    sb.Append("\tInputType: " + inputMapping.InputType);
                    sb.Append("\tPositionData: " + inputMapping.PositionData);
                    sb.Append("\tRotationData: " + inputMapping.RotationData);
                    sb.Append("\tBoolData: " + inputMapping.BoolData);
                    sb.Append("\tFloatData: " + inputMapping.FloatData);
                    sb.AppendLine();
                    sb.AppendLine();
                }
                sb.AppendLine();
            }
            rawDataText.text = sb.ToString();
        }

        public void SetPointersEnabled(bool isEnabled)
        {
            PointerBehavior value = isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff;
            PointerUtils.SetGazePointerBehavior(value);
            PointerUtils.SetHandRayPointerBehavior(value);
            PointerUtils.SetHandPokePointerBehavior(value);
            PointerUtils.SetMotionControllerRayPointerBehavior(value);
        }

    } 
}
