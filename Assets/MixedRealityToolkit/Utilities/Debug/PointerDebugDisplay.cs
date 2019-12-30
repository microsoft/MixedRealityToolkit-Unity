using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit
{

    public class PointerDebugDisplay : MonoBehaviour
    {
        /// <summary>
        /// Text output field for articulated hand debug information
        /// </summary>
        public UnityEngine.UI.Text outputField = null;

        private string outputText;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (outputField == null)
            {
                return;
            }

            var allPointers = GetAllPointers();
            //if (allPointers == 0)
            //{
            //    outputText = "No active MRTK pointer";
            //}
            //else
            {
                outputText = "<b><size=18>Registered Pointers</size></b> \n\n";


                foreach (var pointer in allPointers)
                {
                    if (pointer.IsActive)
                    {
                        outputText += "<b>";
                    }
                    else
                    {
                        outputText += "(Inactive) ";
                    }
                    outputText += pointer.PointerName;
                    outputText += "\nPosition: " + GetVec3PrintPrintableVersion(pointer.Position);
                    outputText += "\nRotation: " + pointer.Rotation;
                    if (pointer.Result != null)
                    {
                        outputText += "\nPointer hit: " + GetVec3PrintPrintableVersion(pointer.Result.Details.Point);
                    }

                    outputText += "\n\n";
                    if (pointer.IsActive)
                    {
                        outputText += "</b>";
                    }
                }

                outputField.supportRichText = true;
                
                outputField.text = outputText;
            }
        }

        string GetVec3PrintPrintableVersion(Vector3 vec3)
        {
            const int numDigits = 3;
            string printvec = "(" + System.Math.Round(vec3.x, numDigits) + ", " 
                + System.Math.Round(vec3.y, numDigits) + ", " 
                + System.Math.Round(vec3.z, numDigits) + ")";

            return printvec;
        }


        private List<IMixedRealityPointer> GetAllPointers()
        {
            var allPointers = new List<IMixedRealityPointer>();
            foreach (var i in CoreServices.InputSystem.DetectedInputSources)
            {
                allPointers.AddRange(i.Pointers);
            }
            return allPointers;
        }
    }
}