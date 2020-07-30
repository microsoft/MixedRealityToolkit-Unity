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
        [Tooltip("Line renderer controlling the tether line, connecting the affordance to the center of the object.")]
        private SimpleLineDataProvider tetherLine;

        /// <summary>
        /// Line renderer controlling the tether line, connecting the affordance to the center of the object.
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
        /// Line renderer controlling the tether line, connecting the affordance to the center of the object.
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
        [Tooltip("Beginning of the total range of the rotation affordance, in degrees.")]
        private float startDegrees;

        /// <summary>
        /// Beginning of the total range of the rotation affordance, in degrees.
        /// </summary>
        public float StartDegrees
        {
            get => startDegrees;
            set => startDegrees = value;
        }

        [SerializeField]
        [Range(-180.0f, 180.0f)]
        [Tooltip("End of the total range of the rotation affordance, in degrees.")]
        private float endDegrees;

        /// <summary>
        /// End of the total range of the rotation affordance, in degrees.
        /// </summary>
        public float EndDegrees
        {
            get => endDegrees;
            set => endDegrees = value;
        }

        [SerializeField]
        [Range(-0.1f, 0.1f)]
        [Tooltip("The radial offset of the marker from the edge of the rotation affordance.")]
        private float markerRadiusOffset;

        /// <summary>
        /// The radial offset of the marker from the edge of the rotation affordance.
        /// </summary>
        public float MarkerRadiusOffset
        {
            get => markerRadiusOffset;
            set => markerRadiusOffset = value;
        }

        [SerializeField]
        [Range(-0.1f, 0.1f)]
        [Tooltip("The radial offset of the progress line from the edge of the rotation affordance.")]
        private float progressLineRadiusOffset;

        /// <summary>
        /// The radial offset of the progress line from the edge of the rotation affordance.
        /// </summary>
        public float ProgressLineRadiusOffset
        {
            get => progressLineRadiusOffset;
            set => progressLineRadiusOffset = value;
        }

        [SerializeField]
        [Range(0.0f, 0.1f)]
        [Tooltip("The radial offset of the text label from the edge of the rotation affordance.")]
        private float textLabelRadiusOffset;

        public float TextLabelRadiusOffset
        {
            get => textLabelRadiusOffset;
            set => textLabelRadiusOffset = value;
        }

        [SerializeField]
        [Range(100, 1000)]
        [Tooltip("Number of vertices per normalized unit for the progress line.")]
        private int progressLineVerticesPerUnit;

        public int ProgressLineVerticesPerUnit
        {
            get => progressLineVerticesPerUnit;
            set => progressLineVerticesPerUnit = value;
        }

        #endregion Serialized Fields

        #region Public Properties

        // The current manipulation scale of the affordance; typically, the distance
        // of the manipulation point from the root/center of the manipulated object.
        public float ManipulationScale = 0.2f;

        // Whether this affordance is currently "deployed", i.e. inflated and active.
        public bool deployed = false;

        #endregion

        #region Private Properties

        // The pointer associated with this interaction.
        private PointerData? associatedPointer = null;

        // Transform of the object that is being manipulated.
        private Transform targetObject;

        // The handle of the BoundsControl that spawned this affordance.
        private Transform targetHandle;

        private Vector3 originalVector;

        private Material markerMaterial;
        private Color markerColor;

        private Material tickmarkMaterial;
        private Color tickmarkColor;

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

        internal void SetAssociatedPointer(IMixedRealityPointer pointer)
        {
            // Store the associated pointer. Stored in a PointerData struct
            // so that we can get the precise GrabPoint in the future.
            associatedPointer = new PointerData(pointer, targetHandle.position);

            // Initializes ManipulationScale to the calculated value.
            ManipulationScale = (associatedPointer.Value.GrabPoint - transform.position).magnitude;

            // Run a single frame update ahead of time.
            Update();
        }

        internal void SetTrackingTarget(Transform targetHandle, Transform targetObject, Quaternion rotationOffset){
            this.targetObject = targetObject;
            this.targetHandle = targetHandle;

            // Store the original handle-to-object vector.
            // This is the basis on which the rotation amount
            // is calculated.
            originalVector = targetHandle.position - transform.position;

            // The affordance starts out with no "progress".
            startDegrees = endDegrees = 0;

            // Deploy the affordance.
            deployed = true;

            // Display text fades in.
            valueDisplay.alpha = 0.0f;

            // Cache material reference to marker material.
            markerMaterial = marker.GetComponent<MeshRenderer>().material;

            // Set marker color alpha to begin at zero.
            markerColor = markerMaterial.color;
            markerColor.a = 0.0f;
            markerMaterial.color = markerColor;

            // Cache material reference to tickmark material.
            tickmarkMaterial = tickmarks.GetComponent<MultiMeshLineRenderer>().LineMaterial;

            // Set tickmark color alpha to begin at zero.
            tickmarkColor = tickmarkMaterial.color;
            tickmarkColor.a = 0.0f;
            tickmarkMaterial.color = tickmarkColor;

            // Initialize the tickmarks start/end clamp values.
            TickmarksDataProvider.LineStartClamp = (-startDegrees / 360.0f) + 0.5f;
            TickmarksDataProvider.LineEndClamp = (-endDegrees / 360.0f) + 0.5f;
        }

        private void Update()
        {
            if(targetObject != null && targetHandle != null){

                // If we have an assigned target object and handle, we calculate
                // the endpoint of the affordance lines (endDegrees)
                endDegrees = Vector3.SignedAngle(originalVector, targetHandle.position - transform.position, transform.up);

                // The affordance is always rigidly attached to the target object's position.
                transform.position = targetObject.position;
                
                // If we have an assigned pointer, and the affordance is actively deployed,
                // we calculate the manipulation scale of the affordance based on the grab point.
                if(associatedPointer != null && deployed)
                {
                    ManipulationScale = Smoothing.SmoothTo(ManipulationScale, (associatedPointer.Value.GrabPoint - transform.position).magnitude, 0.001f, Time.deltaTime);
                }
            }
            
            // Controls the value display fade.
            valueDisplay.alpha = Mathf.Lerp(valueDisplay.alpha, deployed ? 1.0f : 0.0f, Time.deltaTime / 0.1f);

            if (markerMaterial != null && tickmarkMaterial != null)
            {
                // Controls the marker color fade.
                markerColor.a = valueDisplay.alpha;
                markerMaterial.color = markerColor;

                // Controls the marker color fade.
                tickmarkColor.a = valueDisplay.alpha;
                tickmarkMaterial.color = tickmarkColor;
            }

            // Calculate the normalized start and end positions for the progress line/arc.
            float startNormalized = (-startDegrees / 360.0f) + 0.5f;
            float endNormalized = (-endDegrees / 360.0f) + 0.5f;

            // Apply the start/end clamp to the tickmarks; this inflates/deflates
            // the tickmarks in a pleasing animation.
            TickmarksDataProvider.LineStartClamp = Mathf.Lerp(TickmarksDataProvider.LineStartClamp, deployed ? 0.25f : endNormalized, Time.deltaTime / 0.1f);
            TickmarksDataProvider.LineEndClamp = Mathf.Lerp(TickmarksDataProvider.LineEndClamp, deployed ? 0.75f : endNormalized, Time.deltaTime / 0.1f);

            // Apply the proper radius to the tickmarks.
            TickmarksDataProvider.Radius = new Vector2(ManipulationScale, ManipulationScale);

            // Apply the radius + the radius offset to the progress line and its background line.
            ProgressLineDataProvider.Radius = new Vector2(ManipulationScale, ManipulationScale) + Vector2.one * progressLineRadiusOffset;
            ProgressBackgroundLineDataProvider.Radius = new Vector2(ManipulationScale, ManipulationScale) + Vector2.one * progressLineRadiusOffset;

            // Apply the start/end clamp to the progress line and the progress background line.
            ProgressBackgroundLineDataProvider.LineStartClamp = Mathf.Lerp(TickmarksDataProvider.LineStartClamp, deployed ? 0.25f : endNormalized, Time.deltaTime / 0.1f);
            ProgressBackgroundLineDataProvider.LineEndClamp = Mathf.Lerp(TickmarksDataProvider.LineEndClamp, deployed ? 0.75f : endNormalized, Time.deltaTime / 0.1f);
            ProgressLineDataProvider.LineStartClamp = Mathf.Lerp(ProgressLineDataProvider.LineStartClamp, deployed ? startNormalized : endNormalized, Time.deltaTime / 0.1f);
            ProgressLineDataProvider.LineEndClamp = endNormalized;

            // Rotate the entire marker assembly around the pivot.
            markerPivot.localRotation = Quaternion.Euler(0, endDegrees, 0);

            // We want fewer points as the arclength decreases, and more points as the arclength increases.
            ProgressLine.LineStepCount = Mathf.RoundToInt(Mathf.Abs(startNormalized - endNormalized) * ManipulationScale * progressLineVerticesPerUnit);

            // Display text.
            valueDisplay.text = (startDegrees - endDegrees).ToString("F1") + "°";

            // Rotate the display text to billboard to the user's head.
            valueDisplay.transform.rotation = Quaternion.LookRotation(valueDisplay.transform.position - Camera.main.transform.position, Vector3.up);

            // Apply the proper radius/offset to the value display text
            valueDisplay.transform.localPosition = new Vector3(0, 0, ManipulationScale + markerRadiusOffset + textLabelRadiusOffset);

            // Apply the proper radius/offset to the marker icon.
            marker.localPosition = new Vector3(0,0, ManipulationScale + markerRadiusOffset);

            // The marker icon is semi-billboarded; it remains aligned along the radial axis, but tilts to face the user.
            marker.rotation = Quaternion.LookRotation(marker.position - Camera.main.transform.position, markerPivot.forward);

            // Set the tether line's endpoint to the tip of the progress line.
            TetherLine.EndPoint = new MixedRealityPose(Quaternion.Euler(0, endDegrees, 0) * (Vector3.forward * (ManipulationScale + progressLineRadiusOffset)));

            // If the manipulation scale is large enough,
            // increase the frequency of the small/minor tick marks.
            if (ManipulationScale > 0.2f)
            {
                Tickmarks.minorLineStepSkip = 1;
            }
            else
            {
                Tickmarks.minorLineStepSkip = 2;
            }
        }

        /// <summary>
        /// Destroys the affordance after a set time, as well as marking the affordance as non-deployed,
        /// which triggers the deflation/fadeout effect.
        /// </summary>
        internal void DestroySelf()
        {
            if(gameObject != null)
                Destroy(gameObject, 0.5f);
            deployed = false;
        }
    }
}
