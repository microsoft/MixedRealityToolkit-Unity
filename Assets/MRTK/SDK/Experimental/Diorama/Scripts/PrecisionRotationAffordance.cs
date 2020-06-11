using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Utilities.Experimental
{

    [ExecuteInEditMode]
    public class PrecisionRotationAffordance : MonoBehaviour
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

        [Range(-180.0f,180.0f)]
        public float startDegrees, endDegrees;

        [Range(0.0f, 2.0f)]
        public float manipulationScale;

        [Range(-0.1f, 0.1f)]
        public float markerRadiusOffset;

        [Range(-0.1f, 0.1f)]
        public float progressLineRadiusOffset;

        [Range(0.0f, 0.1f)]
        public float textLabelRadiusOffset;

        private Transform trackingTarget;
        private Vector3 originalVector;

        public void SetTrackingTarget(Transform target){
            trackingTarget = target;
            originalVector = target.position - transform.position;
            startDegrees = endDegrees = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //if(ProgressLineDataProvider != null)
            //{

            if(trackingTarget != null){
                endDegrees = Vector3.SignedAngle(originalVector, trackingTarget.position - transform.position, transform.up);
            }

            var startNormalized = (-startDegrees / 360.0f) + 0.5f;
            var endNormalized = (-endDegrees / 360.0f) + 0.5f;


            TickmarksDataProvider.Radius = new Vector2(manipulationScale, manipulationScale);
            ProgressLineDataProvider.Radius = new Vector2(manipulationScale, manipulationScale) + Vector2.one * progressLineRadiusOffset;
            ProgressBackgroundLineDataProvider.Radius = new Vector2(manipulationScale, manipulationScale) + Vector2.one * progressLineRadiusOffset;
            ProgressLineDataProvider.LineStartClamp = startNormalized;
            ProgressLineDataProvider.LineEndClamp = endNormalized;
            //}
            //if(markerPivot != null)
            //{
            markerPivot.localRotation = Quaternion.Euler(0, endDegrees, 0);
            ProgressLine.CustomPointDistributionLength = 0.01f/Mathf.Abs(startNormalized - endNormalized);
            valueDisplay.text = (startDegrees - endDegrees).ToString("F2") + "°";
            valueDisplay.transform.rotation = Quaternion.LookRotation(-transform.up, transform.forward);
            valueDisplay.transform.localPosition = new Vector3(0, 0, manipulationScale + markerRadiusOffset + textLabelRadiusOffset);
            marker.localPosition = new Vector3(0,0, manipulationScale + markerRadiusOffset);
            TetherLine.EndPoint = new MixedRealityPose(Quaternion.Euler(0, endDegrees, 0) * (Vector3.forward * (manipulationScale + progressLineRadiusOffset)));
            //}
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
