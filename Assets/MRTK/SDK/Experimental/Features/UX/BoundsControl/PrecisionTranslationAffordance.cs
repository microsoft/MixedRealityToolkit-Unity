using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine.SocialPlatforms;

namespace Microsoft.MixedReality.Toolkit.Utilities.Experimental
{

    [ExecuteInEditMode]
    public class PrecisionTranslationAffordance : MonoBehaviour
    {
        public SimpleLineDataProvider TetherLine;
        public BaseMixedRealityLineRenderer ProgressLine;


        public SimpleLineDataProvider ProgressLineDataProvider;
        public SimpleLineDataProvider ProgressBackgroundLineDataProvider;

        public EllipseLineDataProvider CircleLineProvider;
        public SimpleLineDataProvider TetherLineProvider;

        public AnimationCurve tolerance;

        public Transform rulerContainer;
        public SimpleLineDataProvider TickmarksProvider;
        public MultiMeshLineRenderer Tickmarks;

        public Transform marker;
        public Transform markerPivot;
        public Transform TickmarkPivot;
        public TextMeshPro valueDisplay;
        public TextMeshPro scaleDisplay;

        public float start, end;

        [Range(1.0f, 100.0f)]
        public float logisticSlope;

        [Range(0.0f, 2.0f)]
        public float manipulationScale;

        [Range(-0.1f, 0.1f)]
        public float markerOffset;

        [Range(-0.1f, 0.1f)]
        public float upperTicksOffset;

        [Range(0.0f, 0.1f)]
        public float textLabelOffset;

        [Range(-0.1f, 0.1f)]
        public float manipAmount;
        [Range(-0.1f, 0.1f)]
        public float handleAmount;
        [Range(0.0f, 0.1f)]
        public float distanceFromAxis;

        private PointerData? associatedPointer = null;

        private Transform targetObject;
        private Transform targetHandle;
        private Quaternion rotationOffset;
        private Vector3 translationAxis;
        private Vector3 initialGrabPoint;
        private Vector3 initialHandlePosition;

        private Vector3 smoothedGrabPoint;

        private float initialGrabOffset;

        private Material rulerMaterial;

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
            associatedPointer = new PointerData(pointer, pointer.Result.Details.Point);
            this.initialGrabPoint = associatedPointer.Value.GrabPoint;
            this.smoothedGrabPoint = this.initialGrabPoint;

            // Nudge the widget over to line up with the handle's position along the translation axis.
            var inLocalSpace = transform.InverseTransformPoint(associatedPointer.Value.GrabPoint);
            transform.Translate(Vector3.right * inLocalSpace.x, Space.Self);
        }

        public void SetTrackingTarget(Transform targetHandle, Transform targetObject, Quaternion rotationOffset){
            this.targetObject = targetObject;
            this.targetHandle = targetHandle;
            this.rotationOffset = rotationOffset;
            initialHandlePosition = targetHandle.position;
            translationAxis = (targetHandle.position - targetObject.position).normalized;
            deployed = true;

            //transform.right = translationAxis;
            //transform.forward = Vector3.ProjectOnPlane((transform.position - Camera.main.transform.position), translationAxis);
            Vector3 eyeVector = Vector3.ProjectOnPlane((transform.position - Camera.main.transform.position), translationAxis);

            Vector3 cameraAxis = Camera.main.transform.InverseTransformVector(translationAxis);
            bool isNegative = cameraAxis.x < 0;

            Vector3 crossAxis = Vector3.Cross(eyeVector, isNegative ? -translationAxis : translationAxis);
            transform.rotation = Quaternion.LookRotation(eyeVector, crossAxis);
            
        }

        // Update is called once per frame
        void Update()
        {
            if (targetObject != null && targetHandle != null && associatedPointer != null)
            {
                smoothedGrabPoint = Smoothing.SmoothTo(smoothedGrabPoint, associatedPointer.Value.GrabPoint, 0.001f, Time.deltaTime);
                Vector3 translateVectorAlongAxis = Vector3.Project(smoothedGrabPoint - initialGrabPoint, translationAxis);
                distanceFromAxis = ((smoothedGrabPoint - initialGrabPoint) - translateVectorAlongAxis).magnitude;
                Vector3 handleVectorAlongAxis = Vector3.Project(targetHandle.position - initialHandlePosition, translationAxis);

                manipAmount = Vector3.Dot(translateVectorAlongAxis, translationAxis);
                handleAmount = Vector3.Dot(handleVectorAlongAxis, translationAxis);

                // Align widget with the YZ coords of the grab point, but leave the X alone.
                var localGrabPoint = transform.InverseTransformPoint(smoothedGrabPoint);
                rulerContainer.localPosition = new Vector3(0, localGrabPoint.y, localGrabPoint.z);
            }
            else
            {
                rulerContainer.localPosition = new Vector3(0, distanceFromAxis, 0);
            }
            Vector3 localTranslationAxis = transform.InverseTransformDirection(translationAxis);

            if(localTranslationAxis == Vector3.zero) { localTranslationAxis = Vector3.right; }

            
            // Sigmoid logistic function
            float damperFactor = (2 / (1 + Mathf.Exp(logisticSlope * -distanceFromAxis))) - 1;

            float scale = 1.0f - damperFactor;
            scale = Mathf.Clamp(scale, 0.001f, 1.0f);

            if (scale < 0.5f)
            {
                Tickmarks.minorLineStepSkip = 1;
            }
            else
            {
                Tickmarks.minorLineStepSkip = 2;
            }

            TickmarkPivot.localPosition = localTranslationAxis * (manipAmount);

            TickmarksProvider.transform.localPosition = (Vector3.right * -0.5f) - (localTranslationAxis * handleAmount) + Vector3.up * upperTicksOffset;
            TickmarkPivot.localScale = new Vector3(1.0f/scale, 1.0f, 1.0f);

            markerPivot.localPosition = localTranslationAxis * manipAmount + Vector3.up * (markerOffset+ upperTicksOffset);

            ProgressLineDataProvider.LineStartClamp = 0.5f;
            ProgressLineDataProvider.LineEndClamp = 0.5f + localTranslationAxis.x * handleAmount;

            CircleLineProvider.Radius = Vector2.one * distanceFromAxis;
            CircleLineProvider.transform.localPosition = localTranslationAxis * manipAmount;
            TetherLineProvider.EndPoint = new MixedRealityPose(localTranslationAxis * manipAmount);

            valueDisplay.text = (handleAmount * 100.0f).ToString("F2") + " cm";
            scaleDisplay.text = (scale).ToString("F2") + "x";
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(TickmarkPivot.position, 0.01f);
        }

        public void DestroySelf()
        {
            if(gameObject != null)
                Destroy(gameObject, 0.5f);
            deployed = false;
        }
    }
}
