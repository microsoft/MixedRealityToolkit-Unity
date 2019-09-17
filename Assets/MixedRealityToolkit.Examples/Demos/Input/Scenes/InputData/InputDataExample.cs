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
        public Text debugText;
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
            debugText.text = sb.ToString();
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
