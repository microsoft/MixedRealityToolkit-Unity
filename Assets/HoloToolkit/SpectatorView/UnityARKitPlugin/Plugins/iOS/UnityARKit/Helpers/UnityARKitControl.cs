using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
    public class UnityARKitControl : MonoBehaviour {
#if UNITY_IOS || UNITY_EDITOR
        UnityARSessionRunOption [] runOptions = new UnityARSessionRunOption[4];
        UnityARAlignment [] alignmentOptions = new UnityARAlignment[3];
        UnityARPlaneDetection [] planeOptions = new UnityARPlaneDetection[4];
        int currentOptionIndex = 0;
        int currentAlignmentIndex = 0;
        int currentPlaneIndex = 0;

        // Use this for initialization
        void Start () {
            runOptions [0] = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;
            runOptions [1] = UnityARSessionRunOption.ARSessionRunOptionResetTracking;
            runOptions [2] = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors;
            runOptions [3] = 0;

            alignmentOptions [0] = UnityARAlignment.UnityARAlignmentCamera;
            alignmentOptions [1] = UnityARAlignment.UnityARAlignmentGravity;
            alignmentOptions [2] = UnityARAlignment.UnityARAlignmentGravityAndHeading;

            planeOptions [0] = UnityARPlaneDetection.Horizontal;
            planeOptions [1] = UnityARPlaneDetection.None;

        }
        
        // Update is called once per frame
        void Update () {
            
        }

        void OnGUI()
        {
            if (GUI.Button (new Rect (100, 100, 200, 50), "Stop")) {
                UnityARSessionNativeInterface.GetARSessionNativeInterface ().Pause ();
            }

            if (GUI.Button (new Rect (300, 100, 200, 50), "Start")) {
                ARKitWorldTrackingSessionConfiguration sessionConfig = new ARKitWorldTrackingSessionConfiguration (alignmentOptions [currentAlignmentIndex], planeOptions[currentPlaneIndex]);
                UnityARSessionNativeInterface.GetARSessionNativeInterface ().RunWithConfigAndOptions (sessionConfig, runOptions[currentOptionIndex]);
            }


            if (GUI.Button (new Rect (100, 300, 200, 100), "Start Non-WT Session")) {
                ARKitSessionConfiguration sessionConfig = new ARKitSessionConfiguration (alignmentOptions [currentAlignmentIndex], true, true);
                UnityARSessionNativeInterface.GetARSessionNativeInterface ().RunWithConfig (sessionConfig);
            }


            string runOptionStr = (currentOptionIndex == 0 ? "Full" : (currentOptionIndex == 1 ? "Tracking" : (currentOptionIndex == 2 ? "Anchors" : "None")));
            if (GUI.Button (new Rect (100, 200, 150, 50), "RunOption:" + runOptionStr)) {
                currentOptionIndex = (currentOptionIndex + 1) % 4;
            }

            string alignmentOptionStr = (currentAlignmentIndex == 0 ? "Camera" : (currentAlignmentIndex == 1 ? "Gravity" :  "GravityAndHeading"));
            if (GUI.Button (new Rect (300, 200, 150, 50), "AlignOption:" + alignmentOptionStr)) {
                currentAlignmentIndex = (currentAlignmentIndex + 1) % 3;
            }

            string planeOptionStr = currentPlaneIndex == 0 ? "Horizontal":  "None";
            if (GUI.Button (new Rect (500, 200, 150, 50), "PlaneOption:" + planeOptionStr)) {
                currentPlaneIndex = (currentPlaneIndex + 1) % 2;
            }
        }
#endif
    }
}
