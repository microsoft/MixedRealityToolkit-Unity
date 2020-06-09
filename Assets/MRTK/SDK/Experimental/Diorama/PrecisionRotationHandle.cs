using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Utilities.Experimental
{

    [ExecuteInEditMode]
    public class PrecisionRotationHandle : MonoBehaviour
    {
        public MultiMeshLineRenderer Tickmarks;
        public SimpleLineDataProvider TetherLine;
        public BaseMixedRealityLineRenderer ProgressLine;

        public EllipseLineDataProvider TickmarksDataProvider;

        public EllipseLineDataProvider ProgressLineDataProvider;
        public EllipseLineDataProvider ProgressBackgroundLineDataProvider;

        public Transform marker;
        public Transform markerPivot;
        public TextMeshPro valueDisplay;

        [Range(0.0f,1.0f)]
        public float startAmount, endAmount;

        [Range(0.0f, 2.0f)]
        public float manipulationScale;

        [Range(-0.1f, 0.1f)]
        public float markerRadiusOffset;

        [Range(-0.1f, 0.1f)]
        public float progressLineRadiusOffset;

        [Range(0.0f, 0.1f)]
        public float textLabelRadiusOffset;


        // Update is called once per frame
        void Update()
        {
            //if(ProgressLineDataProvider != null)
            //{
            TickmarksDataProvider.Radius = new Vector2(manipulationScale, manipulationScale);
            ProgressLineDataProvider.Radius = new Vector2(manipulationScale, manipulationScale) + Vector2.one * progressLineRadiusOffset;
            ProgressBackgroundLineDataProvider.Radius = new Vector2(manipulationScale, manipulationScale) + Vector2.one * progressLineRadiusOffset;
            ProgressLineDataProvider.LineStartClamp = startAmount;
            ProgressLineDataProvider.LineEndClamp = endAmount;
            //}
            //if(markerPivot != null)
            //{
            markerPivot.localRotation = Quaternion.Euler(0, 0, endAmount * 360.0f);
            ProgressLine.CustomPointDistributionLength = 0.01f/Mathf.Abs(startAmount - endAmount);
            valueDisplay.text = ((startAmount - endAmount) * 360.0f).ToString("F2") + "°";
            valueDisplay.transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            valueDisplay.transform.localPosition = new Vector3(manipulationScale + markerRadiusOffset + textLabelRadiusOffset, 0, 0);
            marker.localPosition = new Vector3(manipulationScale + markerRadiusOffset, 0,0);
            TetherLine.EndPoint = new MixedRealityPose(Quaternion.Euler(0, 0, endAmount * 360.0f) * (Vector3.right * (manipulationScale + progressLineRadiusOffset)));
            //}
        }
    }
}
