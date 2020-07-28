using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
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

        private PointerData? associatedPointer = null;

        private Transform targetObject;
        private Transform targetHandle;
        private Quaternion rotationOffset;
        private Vector3 originalVector;

        public bool deployed = false;

        /// <summary>
        /// Holds the pointer and the initial intersection point of the pointer ray 
        /// with the object on pointer down in pointer space
        /// </summary>
        private struct PointerData
        {
            public IMixedRealityPointer pointer;
            private Vector3 initialGrabPointInPointer;

            public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint) : this()
            {
                this.pointer = pointer;
                this.initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
            }

            public bool IsNearPointer => pointer is IMixedRealityNearPointer;

            /// Returns the grab point on the manipulated object in world space
            public Vector3 GrabPoint => (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
        }

        public void SetAssociatedPointer(IMixedRealityPointer pointer)
        {
            associatedPointer = new PointerData(pointer, targetHandle.position);
        }

        public void SetTrackingTarget(Transform targetHandle, Transform targetObject, Quaternion rotationOffset){
            this.targetObject = targetObject;
            this.targetHandle = targetHandle;
            this.rotationOffset = rotationOffset;
            originalVector = targetHandle.position - transform.position;
            startDegrees = endDegrees = 0;
            deployed = true;
            TickmarksDataProvider.LineStartClamp = (-startDegrees / 360.0f) + 0.5f;
            TickmarksDataProvider.LineEndClamp = (-endDegrees / 360.0f) + 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            if(targetObject != null && targetHandle != null){
                endDegrees = Vector3.SignedAngle(originalVector, targetHandle.position - transform.position, transform.up);
                transform.position = targetObject.position;
                
                if(associatedPointer != null)
                {
                    manipulationScale = Smoothing.SmoothTo(manipulationScale, (associatedPointer.Value.GrabPoint - transform.position).magnitude, 0.001f, Time.deltaTime);
                }
            }

            var startNormalized = (-startDegrees / 360.0f) + 0.5f;
            var endNormalized = (-endDegrees / 360.0f) + 0.5f;

            TickmarksDataProvider.LineStartClamp = Mathf.Lerp(TickmarksDataProvider.LineStartClamp, deployed ? 0.25f : endNormalized, 0.2f);
            TickmarksDataProvider.LineEndClamp = Mathf.Lerp(TickmarksDataProvider.LineEndClamp, deployed ? 0.75f : endNormalized, 0.2f);

            TickmarksDataProvider.Radius = new Vector2(manipulationScale, manipulationScale);
            ProgressLineDataProvider.Radius = new Vector2(manipulationScale, manipulationScale) + Vector2.one * progressLineRadiusOffset;
            ProgressBackgroundLineDataProvider.Radius = new Vector2(manipulationScale, manipulationScale) + Vector2.one * progressLineRadiusOffset;

            ProgressBackgroundLineDataProvider.LineStartClamp = Mathf.Lerp(TickmarksDataProvider.LineStartClamp, deployed ? 0.25f : endNormalized, 0.2f);
            ProgressBackgroundLineDataProvider.LineEndClamp = Mathf.Lerp(TickmarksDataProvider.LineEndClamp, deployed ? 0.75f : endNormalized, 0.2f);

            ProgressLineDataProvider.LineStartClamp = startNormalized;
            ProgressLineDataProvider.LineEndClamp = endNormalized;
            //}
            //if(markerPivot != null)
            //{
            markerPivot.localRotation = Quaternion.Euler(0, endDegrees, 0);

            // We want to have a specified point distribution for a given unit of arclength.
            var arclength = (Mathf.Abs(startDegrees - endDegrees) / 360.0f) * 2.0f * Mathf.PI * (manipulationScale + progressLineRadiusOffset) * transform.lossyScale.x;
            var degreesPerStep = Mathf.Abs(startDegrees - endDegrees) / 50.0f;
            var lengthPerDegree = arclength / Mathf.Abs(startDegrees - endDegrees);
            var lengthPerStep = lengthPerDegree * degreesPerStep;

            ProgressLine.CustomPointDistributionLength = (0.1f / Mathf.Abs(startNormalized - endNormalized)) * manipulationScale;
            valueDisplay.text = (startDegrees - endDegrees).ToString("F2") + "°";
            valueDisplay.transform.rotation = Quaternion.LookRotation(valueDisplay.transform.position - Camera.main.transform.position, Vector3.up);
            valueDisplay.transform.localPosition = new Vector3(0, 0, manipulationScale + markerRadiusOffset + textLabelRadiusOffset);
            marker.localPosition = new Vector3(0,0, manipulationScale + markerRadiusOffset);
            marker.rotation = Quaternion.LookRotation(marker.position - Camera.main.transform.position, markerPivot.forward);
            TetherLine.EndPoint = new MixedRealityPose(Quaternion.Euler(0, endDegrees, 0) * (Vector3.forward * (manipulationScale + progressLineRadiusOffset)));
            //}
        }

        public void DestroySelf()
        {
            if(gameObject != null)
                Destroy(gameObject, 0.5f);
            deployed = false;
        }
    }
}
