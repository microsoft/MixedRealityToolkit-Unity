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
        #region Serialized Fields

        [Header("Lines + Visualizations")]
        [SerializeField]
        [Tooltip("Line renderer controlling the tickmarks display.")]
        private MultiMeshLineRenderer tickmarks;

        /// <summary>
        /// Line renderer controlling the tickmarks display.
        /// </summary>
        public MultiMeshLineRenderer Tickmarks
        {
            get => tickmarks;
            set => tickmarks = value;
        }

        [SerializeField]
        [Tooltip("Line renderer controlling the tether line, connecting the widget to the center of the object.")]
        private SimpleLineDataProvider tetherLine;

        /// <summary>
        /// Line renderer controlling the tether line, connecting the widget to the center of the object.
        /// </summary>
        public SimpleLineDataProvider TetherLine
        {
            get => tetherLine;
            set => tetherLine = value;
        }

        [SerializeField]
        [Tooltip("Line renderer controlling the progress line.")]
        private BaseMixedRealityLineRenderer progressLine;

        /// <summary>
        /// Line renderer controlling the tether line, connecting the widget to the center of the object.
        /// </summary>
        public BaseMixedRealityLineRenderer ProgressLine
        {
            get => progressLine;
            set => progressLine = value;
        }

        [SerializeField]
        [Tooltip("Data provider connected to the tickmarks display.")]
        private EllipseLineDataProvider tickmarksDataProvider;

        /// <summary>
        /// Data provider connected to the tickmarks display.
        /// </summary>
        public EllipseLineDataProvider TickmarksDataProvider
        {
            get => tickmarksDataProvider;
            set => tickmarksDataProvider = value;
        }

        [SerializeField]
        [Tooltip("Data provider connected to the progress line.")]
        private EllipseLineDataProvider progressLineDataProvider;

        /// <summary>
        /// Data provider connected to the progress line.
        /// </summary>
        public EllipseLineDataProvider ProgressLineDataProvider
        {
            get => progressLineDataProvider;
            set => progressLineDataProvider = value;
        }

        [SerializeField]
        [Tooltip("Data provider connected to the background of the progress line.")]
        private EllipseLineDataProvider progressBackgroundLineDataProvider;

        /// <summary>
        /// Data provider connected to the background of the progress line.
        /// </summary>
        public EllipseLineDataProvider ProgressBackgroundLineDataProvider
        {
            get => progressBackgroundLineDataProvider;
            set => progressBackgroundLineDataProvider = value;
        }

        [Header("")]
        [SerializeField]
        [Tooltip("The marker icon indicating the current rotation point.")]
        private Transform marker;

        /// <summary>
        /// The marker icon indicating the current rotation point.
        /// </summary>
        public Transform Marker
        {
            get => marker;
            set => marker = value;
        }

        [SerializeField]
        [Tooltip("Pivot transform for the marker container object.")]
        private Transform markerPivot;

        /// <summary>
        /// Pivot transform for the marker container object.
        /// </summary>
        public Transform MarkerPivot
        {
            get => markerPivot;
            set => markerPivot = value;
        }

        [SerializeField]
        [Tooltip("Display text indicating the current rotation angle.")]
        private TextMeshPro valueDisplay;

        /// <summary>
        /// Display text indicating the current rotation angle.
        /// </summary>
        public TextMeshPro ValueDisplay
        {
            get => valueDisplay;
            set => valueDisplay = value;
        }

        [Header("Visual Configuration")]
        [SerializeField]
        [Range(-180.0f, 180.0f)]
        [Tooltip("Beginning of the total range of the rotation widget, in degrees.")]
        private float startDegrees;

        /// <summary>
        /// Beginning of the total range of the rotation widget, in degrees.
        /// </summary>
        public float StartDegrees
        {
            get => startDegrees;
            set => startDegrees = value;
        }

        [SerializeField]
        [Range(-180.0f, 180.0f)]
        [Tooltip("End of the total range of the rotation widget, in degrees.")]
        private float endDegrees;

        /// <summary>
        /// End of the total range of the rotation widget, in degrees.
        /// </summary>
        public float EndDegrees
        {
            get => endDegrees;
            set => endDegrees = value;
        }

        [SerializeField]
        [Range(-0.1f, 0.1f)]
        [Tooltip("The radial offset of the marker from the edge of the rotation widget.")]
        private float markerRadiusOffset;

        /// <summary>
        /// The radial offset of the marker from the edge of the rotation widget.
        /// </summary>
        public float MarkerRadiusOffset
        {
            get => markerRadiusOffset;
            set => markerRadiusOffset = value;
        }

        [SerializeField]
        [Range(-0.1f, 0.1f)]
        [Tooltip("The radial offset of the progress line from the edge of the rotation widget.")]
        private float progressLineRadiusOffset;

        /// <summary>
        /// The radial offset of the progress line from the edge of the rotation widget.
        /// </summary>
        public float ProgressLineRadiusOffset
        {
            get => progressLineRadiusOffset;
            set => progressLineRadiusOffset = value;
        }

        [SerializeField]
        [Range(0.0f, 0.1f)]
        [Tooltip("The radial offset of the text label from the edge of the rotation widget.")]
        private float textLabelRadiusOffset;

        public float TextLabelRadiusOffset
        {
            get => textLabelRadiusOffset;
            set => textLabelRadiusOffset = value;
        }

        #endregion Serialized Fields

        #region Public Properties

        // The current manipulation scale of the widget; typically, the distance
        // of the manipulation point from the root/center of the manipulated object
        public float ManipulationScale = 0.2f;

        // Whether this widget is currently "deployed", i.e. inflated and active.
        public bool deployed = false;

        #endregion

        #region Private Properties

        // The pointer associated with this interaction.
        private PointerData? associatedPointer = null;

        // Transform of the object that is being manipulated.
        private Transform targetObject;

        // The handle of the BoundsControl that spawned this widget.
        private Transform targetHandle;

        private Quaternion rotationOffset;
        private Vector3 originalVector;

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

        #endregion

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
                    ManipulationScale = Smoothing.SmoothTo(ManipulationScale, (associatedPointer.Value.GrabPoint - transform.position).magnitude, 0.001f, Time.deltaTime);
                }
            }

            var startNormalized = (-startDegrees / 360.0f) + 0.5f;
            var endNormalized = (-endDegrees / 360.0f) + 0.5f;

            TickmarksDataProvider.LineStartClamp = Mathf.Lerp(TickmarksDataProvider.LineStartClamp, deployed ? 0.25f : endNormalized, 0.2f);
            TickmarksDataProvider.LineEndClamp = Mathf.Lerp(TickmarksDataProvider.LineEndClamp, deployed ? 0.75f : endNormalized, 0.2f);

            TickmarksDataProvider.Radius = new Vector2(ManipulationScale, ManipulationScale);
            ProgressLineDataProvider.Radius = new Vector2(ManipulationScale, ManipulationScale) + Vector2.one * progressLineRadiusOffset;
            ProgressBackgroundLineDataProvider.Radius = new Vector2(ManipulationScale, ManipulationScale) + Vector2.one * progressLineRadiusOffset;

            ProgressBackgroundLineDataProvider.LineStartClamp = Mathf.Lerp(TickmarksDataProvider.LineStartClamp, deployed ? 0.25f : endNormalized, 0.2f);
            ProgressBackgroundLineDataProvider.LineEndClamp = Mathf.Lerp(TickmarksDataProvider.LineEndClamp, deployed ? 0.75f : endNormalized, 0.2f);

            ProgressLineDataProvider.LineStartClamp = startNormalized;
            ProgressLineDataProvider.LineEndClamp = endNormalized;
            markerPivot.localRotation = Quaternion.Euler(0, endDegrees, 0);

            // We want to have a specified point distribution for a given unit of arclength.
            var arclength = (Mathf.Abs(startDegrees - endDegrees) / 360.0f) * 2.0f * Mathf.PI * (ManipulationScale + progressLineRadiusOffset) * transform.lossyScale.x;
            var degreesPerStep = Mathf.Abs(startDegrees - endDegrees) / 50.0f;
            var lengthPerDegree = arclength / Mathf.Abs(startDegrees - endDegrees);
            var lengthPerStep = lengthPerDegree * degreesPerStep;

            ProgressLine.CustomPointDistributionLength = (0.1f / Mathf.Abs(startNormalized - endNormalized)) * ManipulationScale;
            valueDisplay.text = (startDegrees - endDegrees).ToString("F2") + "°";
            valueDisplay.transform.rotation = Quaternion.LookRotation(valueDisplay.transform.position - Camera.main.transform.position, Vector3.up);
            valueDisplay.transform.localPosition = new Vector3(0, 0, ManipulationScale + markerRadiusOffset + textLabelRadiusOffset);
            marker.localPosition = new Vector3(0,0, ManipulationScale + markerRadiusOffset);
            marker.rotation = Quaternion.LookRotation(marker.position - Camera.main.transform.position, markerPivot.forward);
            TetherLine.EndPoint = new MixedRealityPose(Quaternion.Euler(0, endDegrees, 0) * (Vector3.forward * (ManipulationScale + progressLineRadiusOffset)));
        }

        public void DestroySelf()
        {
            if(gameObject != null)
                Destroy(gameObject, 0.5f);
            deployed = false;
        }
    }
}
