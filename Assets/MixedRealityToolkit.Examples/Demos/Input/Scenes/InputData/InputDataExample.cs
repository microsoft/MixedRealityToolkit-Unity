using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class demonstrates how to query input data either by using InputUtils or
    /// by directly accessing InteractionMappings from all active controllers.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/InputDataExample")]
    public class InputDataExample : MonoBehaviour
    {
        public TextMesh inputUtilsText;
        public TextMesh rawDataText;

        private Tuple<InputSourceType, Handedness> [] inputSources = new Tuple<InputSourceType, Handedness>[]
        {
            new Tuple<InputSourceType, Handedness>(InputSourceType.Controller, Handedness.Right) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Controller, Handedness.Left) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Eyes, Handedness.Any) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Head, Handedness.Any) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Hand, Handedness.Left) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Hand, Handedness.Right)
        };

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
                if (InputRayUtils.TryGetRay(sourceType, handedness, out myRay))
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

            // Iterate through all controllers output position, rotation, and other data from input 
            // mappings on a controller.
            sb.Clear();
            foreach(var controller in CoreServices.InputSystem.DetectedControllers)
            {
                sb.AppendLine("Inputs for " + controller.InputSource.SourceName);
                sb.AppendLine();
                // Interactions for a controller is the list of inputs that this controller exposes
                foreach(MixedRealityInteractionMapping inputMapping in controller.Interactions)
                {
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

        public void Start()
        {
            // Disable the hand and gaze ray, we don't want then for this demo and the conflict
            // with the visuals
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
        }

    } 
}
