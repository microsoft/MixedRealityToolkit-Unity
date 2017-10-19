// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Sample GestureInteractiveControl for scrubbing through ICylce components
    /// </summary>
    public class GestureControlCycler : GestureInteractiveControl
    {
        public struct CycleData
        {
            public int Index;
            public ICycle Controller;
        }
        
        [Tooltip("An array of GameObjects that contain an ICycle component")]
        public GameObject[] CycleControllers;
        
        // the new list of ICycle components so we can track the last index before gesture starts
        private List<CycleData> mCycleList;

        /// <summary>
        /// Setup the CycleData list
        /// </summary>
        private void Start()
        {
            mCycleList = new List<CycleData>();

            for (int i = 0;  i < CycleControllers.Length; ++ i)
            {
                ICycle cycle = CycleControllers[i].GetComponent<ICycle>();
                if (cycle != null)
                {
                    CycleData data = new CycleData();
                    data.Controller = cycle;
                    data.Index = cycle.Index;
                    mCycleList.Add(data);
                }
            }
        }

        public override void ManipulationUpdate(Vector3 startGesturePosition, Vector3 currentGesturePosition, Vector3 startHeadOrigin, Vector3 startHeadRay, GestureInteractive.GestureManipulationState gestureState)
        {
            base.ManipulationUpdate(startGesturePosition, currentGesturePosition, startHeadOrigin, startHeadRay, gestureState);
            
            // get gesture data for gesturing along the horizontal axis
            GestureInteractiveData gestureData = GetGestureData(new Vector3(1, 0, 0), MaxGestureDistance, FlipDirectionOnCameraForward);
            
            for (int i = 0; i < mCycleList.Count; ++i)
            {

                CycleData data = mCycleList[i];
                // get the length of ICycle values to loop through
                float length = data.Controller.GetLastIndex() + 1;
                float incrament = MaxGestureDistance / length;

                // get the delta of the current gesture
                int offset = Mathf.RoundToInt(gestureData.Distance / incrament);
                if (gestureState == GestureInteractive.GestureManipulationState.Start)
                {
                    // set the starting index so we can add the delta to it
                    data.Index = data.Controller.Index;

                    mCycleList[i] = data;
                }

                // update the ICycle component
                data.Controller.SetIndex(Mathf.Clamp(offset + data.Index, 0, data.Controller.GetLastIndex()));
            }
        }
    }
}
