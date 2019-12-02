// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Base class that provides data about a line.
    /// </summary>
    /// <remarks>Data to be consumed by other classes like the <see cref="BaseMixedRealityLineRenderer"/></remarks>
    [ExecuteAlways] 
    public abstract class BaseMixedRealityLineDataProvider : MonoBehaviour
    {
        protected const int UnclampedWorldLengthSearchSteps = 10;
        private const float MinRotationMagnitude = 0.0001f;
        private const float MinLineStartClamp = 0.0001f;
        private const float MaxLineEndClamp = 0.9999f;

        public float UnClampedWorldLength => GetUnClampedWorldLengthInternal();

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Clamps the line's normalized start point. This setting will affect line renderers.")]
        private float lineStartClamp = 0f;

        /// <summary>
        /// Clamps the line's normalized start point. This setting will affect line renderers.
        /// </summary>
        public float LineStartClamp
        {
            get { return lineStartClamp; }
            set { lineStartClamp = Mathf.Clamp01(value); }
        }

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Clamps the line's normalized end point. This setting will affect line renderers.")]
        private float lineEndClamp = 1f;

        /// <summary>
        /// Clamps the line's normalized end point. This setting will affect line renderers.
        /// </summary>
        public float LineEndClamp
        {
            get { return lineEndClamp; }
            set { lineEndClamp = Mathf.Clamp01(value); }
        }

        [SerializeField]
        [Tooltip("Transform to use when translating points from local to world space. If null, this object's transform is used.")]
        private Transform customLineTransform;

        /// <summary>
        /// Transform to use when translating points from local to world space. If null, this object's transform is used.
        /// </summary>
        public Transform LineTransform
        {
            get { return customLineTransform != null ? customLineTransform : transform; }
            set { customLineTransform = value; }
        }

        [SerializeField]
        [Tooltip("Controls whether this line loops \nNote: some classes override this setting")]
        private bool loops = false;

        /// <summary>
        /// Controls whether this line loops
        /// </summary>
        /// <remarks>Some classes override this setting.</remarks>
        public virtual bool Loops
        {
            get { return loops; }
            set { loops = value; }
        }

        [SerializeField]
        [Tooltip("The transform mode used by the line. UseTransform will work when line is disabled, but at a performance cost. UseMatrix requires that the line be active and enabled to return accurate points.")]
        private LinePointTransformMode transformMode = LinePointTransformMode.UseTransform;

        /// <summary>
        /// Defines how a base line data provider will transform its points
        /// </summary>
        public LinePointTransformMode TransformMode
        {
            get { return transformMode; }
            set { transformMode = value; }
        }

        [SerializeField]
        [Tooltip("The rotation mode used in the GetRotation function. You can visualize rotations by checking Draw Rotations under Editor Settings.")]
        private LineRotationMode rotationMode = LineRotationMode.Velocity;

        /// <summary>
        /// The rotation mode used in the GetRotation function. You can visualize rotations by checking Draw Rotations under Editor Settings.
        /// </summary>
        public LineRotationMode RotationMode
        {
            get { return rotationMode; }
            set { rotationMode = value; }
        }

        [SerializeField]
        [Tooltip("Reverses up vector when determining rotation along line")]
        private bool flipUpVector = false;

        /// <summary>
        /// Reverses up vector when determining rotation along line
        /// </summary>
        public bool FlipUpVector
        {
            get { return flipUpVector; }
            set { flipUpVector = value; }
        }

        [SerializeField]
        [Tooltip("Local space offset to transform position. Used to determine rotation along line in RelativeToOrigin rotation mode")]
        private Vector3 originOffset = Vector3.zero;

        /// <summary>
        /// Local space offset to transform position. Used to determine rotation along line in RelativeToOrigin rotation mode
        /// </summary>
        public Vector3 OriginOffset
        {
            get { return originOffset; }
            set { originOffset = value; }
        }

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("The weight of manual up vectors in Velocity rotation mode")]
        private float manualUpVectorBlend = 0f;

        /// <summary>
        /// The weight of manual up vectors in Velocity rotation mode
        /// </summary>
        public float ManualUpVectorBlend
        {
            get { return manualUpVectorBlend; }
            set { manualUpVectorBlend = Mathf.Clamp01(value); }
        }

        [SerializeField]
        [Tooltip("These vectors are used with ManualUpVectorBlend to determine rotation along the line in Velocity rotation mode. Vectors are distributed along the normalized length of the line.")]
        private Vector3[] manualUpVectors = { Vector3.up, Vector3.up, Vector3.up };

        /// <summary>
        /// These vectors are used with ManualUpVectorBlend to determine rotation along the line in Velocity rotation mode. Vectors are distributed along the normalized length of the line.
        /// </summary>
        public Vector3[] ManualUpVectors
        {
            get { return manualUpVectors; }
            set { manualUpVectors = value; }
        }

        [SerializeField]
        [Range(0.0001f, 0.1f)]
        [Tooltip("Used in Velocity rotation mode. Smaller values are more accurate but more expensive")]
        private float velocitySearchRange = 0.02f;

        /// <summary>
        /// Used in Velocity rotation mode. 
        /// </summary>
        /// <remarks>
        /// Smaller values are more accurate but more expensive
        /// </remarks>
        public float VelocitySearchRange
        {
            get { return velocitySearchRange; }
            set { velocitySearchRange = Mathf.Clamp(value, 0.001f, 0.1f); }
        }

        [SerializeField]
        private List<Distorter> distorters = new List<Distorter>();

        /// <summary>
        /// A list of distorters that apply to this line
        /// </summary>
        public List<Distorter> Distorters
        {
            get
            {
                if (distorters.Count == 0)
                {
                    var newDistorters = GetComponents<Distorter>();

                    for (int i = 0; i < newDistorters.Length; i++)
                    {
                        distorters.Add(newDistorters[i]);
                    }
                }

                distorters.Sort();
                return distorters;
            }
        }

        [SerializeField]
        [Tooltip("Enables / disables all distorters used by line")]
        private bool distortionEnabled = true;

        /// <summary>
        /// Enabled / disables all distorters used by line.
        /// </summary>
        public bool DistortionEnabled
        {
            get { return distortionEnabled; }
            set { distortionEnabled = value; }
        }

        [SerializeField]
        [Tooltip("NormalizedLength mode uses the DistortionStrength curve for distortion strength, Uniform uses UniformDistortionStrength along entire line")]
        private DistortionMode distortionMode = DistortionMode.NormalizedLength;

        /// <summary>
        /// NormalizedLength mode uses the DistortionStrength curve for distortion strength, Uniform uses UniformDistortionStrength along entire line
        /// </summary>
        public DistortionMode DistortionMode
        {
            get { return distortionMode; }
            set { distortionMode = value; }
        }

        [SerializeField]
        private AnimationCurve distortionStrength = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public AnimationCurve DistortionStrength
        {
            get { return distortionStrength; }
            set { distortionStrength = value; }
        }

        [Range(0f, 1f)]
        [SerializeField]
        private float uniformDistortionStrength = 1f;

        public float UniformDistortionStrength
        {
            get { return uniformDistortionStrength; }
            set { uniformDistortionStrength = Mathf.Clamp01(value); }
        }

        public Vector3 FirstPoint
        {
            get { return GetPoint(0); }
            set { SetPoint(0, value); }
        }

        public Vector3 LastPoint
        {
            get { return GetPoint(PointCount - 1); }
            set { SetPoint(PointCount - 1, value); }
        }

        #region BaseMixedRealityLineDataProvider Abstract Declarations

        /// <summary>
        /// The number of points this line has.
        /// </summary>
        public abstract int PointCount { get; }

        /// <summary>
        /// Sets the point at index.
        /// </summary>
        protected abstract void SetPointInternal(int pointIndex, Vector3 point);

        /// <summary>
        /// Get a point based on normalized distance along line
        /// Normalized distance will be pre-clamped
        /// </summary>
        protected abstract Vector3 GetPointInternal(float normalizedLength);

        /// <summary>
        /// Get a point based on point index
        /// Point index will be pre-clamped
        /// </summary>
        protected abstract Vector3 GetPointInternal(int pointIndex);

        /// <summary>
        /// Gets the up vector at a normalized length along line (used for rotation)
        /// </summary>
        protected virtual Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return LineTransform.forward;
        }

        /// <summary>
        /// Get the UnClamped world length of the line
        /// </summary>
        protected abstract float GetUnClampedWorldLengthInternal();

        #endregion BaseMixedRealityLineDataProvider Abstract Declarations

        #region MonoBehaviour Implementation

        protected virtual void OnEnable()
        {
            UpdateMatrix();
            distorters.Sort();
        }

        protected virtual void LateUpdate()
        {
            UpdateMatrix();
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Returns a normalized length corresponding to a world length
        /// Useful for determining LineStartClamp / LineEndClamp values
        /// </summary>
        public float GetNormalizedLengthFromWorldLength(float worldLength, int searchResolution = 10)
        {
            if (searchResolution < 1)
            {
                return 0;
            }

            Vector3 lastPoint = GetUnClampedPoint(0f);
            float normalizedLength = 0f;
            float distanceSoFar = 0f;
            float normalizedSegmentLength = 1f / searchResolution;

            for (int i = 1; i <= searchResolution; i++)
            {
                // Get the normalized length of this position along the line
                normalizedLength = normalizedSegmentLength * i;
                Vector3 currentPoint = GetUnClampedPoint(normalizedLength);
                distanceSoFar += Vector3.Distance(lastPoint, currentPoint);

                if (distanceSoFar >= worldLength)
                {
                    // We've reached the world length, so subtract the amount we overshot
                    normalizedLength -= (distanceSoFar - worldLength) / Vector3.Distance(lastPoint, currentPoint) * normalizedSegmentLength;
                    break;
                }

                lastPoint = currentPoint;
            }

            return Mathf.Clamp01(normalizedLength);
        }

        /// <summary>
        /// Gets the velocity along the line
        /// </summary>
        public Vector3 GetVelocity(float normalizedLength)
        {
            Vector3 velocity;

            if (normalizedLength < velocitySearchRange)
            {
                Vector3 currentPos = GetPoint(normalizedLength);
                Vector3 nextPos = GetPoint(normalizedLength + velocitySearchRange);
                velocity = (nextPos - currentPos).normalized;
            }
            else
            {
                Vector3 currentPos = GetPoint(normalizedLength);
                Vector3 prevPos = GetPoint(normalizedLength - velocitySearchRange);
                velocity = (currentPos - prevPos).normalized;
            }

            return velocity;
        }

        /// <summary>
        /// Gets the rotation of a point along the line at the specified length
        /// </summary>
        public Quaternion GetRotation(float normalizedLength, LineRotationMode lineRotationMode = LineRotationMode.None)
        {
            lineRotationMode = (lineRotationMode != LineRotationMode.None) ? lineRotationMode : rotationMode;
            Vector3 rotationVector = Vector3.zero;

            switch (lineRotationMode)
            {
                case LineRotationMode.Velocity:
                    rotationVector = GetVelocity(normalizedLength);
                    break;
                case LineRotationMode.RelativeToOrigin:
                    Vector3 point = GetPoint(normalizedLength);
                    Vector3 origin = TransformPoint(originOffset);
                    rotationVector = (point - origin).normalized;
                    break;
                case LineRotationMode.None:
                    return LineTransform.rotation;
            }

            if (rotationVector.magnitude < MinRotationMagnitude)
            {
                return LineTransform.rotation;
            }

            Vector3 upVector = GetUpVectorInternal(normalizedLength);

            if (manualUpVectorBlend > 0f)
            {
                Vector3 manualUpVector = LineUtility.GetVectorCollectionBlend(manualUpVectors, normalizedLength, Loops);
                upVector = Vector3.Lerp(upVector, manualUpVector, manualUpVector.magnitude);
            }

            if (flipUpVector)
            {
                upVector = -upVector;
            }

            return Quaternion.LookRotation(rotationVector, upVector);
        }

        /// <summary>
        /// Gets the rotation of a point along the line at the specified index
        /// </summary>
        public Quaternion GetRotation(int pointIndex, LineRotationMode lineRotationMode = LineRotationMode.None)
        {
            return GetRotation((float)pointIndex / PointCount, lineRotationMode != LineRotationMode.None ? lineRotationMode : rotationMode);
        }

        /// <summary>
        /// Gets a point along the line at the specified normalized length.
        /// </summary>
        public Vector3 GetPoint(float normalizedLength)
        {
            normalizedLength = ClampedLength(normalizedLength);
            return DistortPoint(TransformPoint(GetPointInternal(normalizedLength)), normalizedLength);
        }

        /// <summary>
        /// Gets a point along the line at the specified length without using LineStartClamp or LineEndClamp
        /// </summary>
        public Vector3 GetUnClampedPoint(float normalizedLength)
        {
            normalizedLength = Mathf.Clamp01(normalizedLength);
            return DistortPoint(TransformPoint(GetPointInternal(normalizedLength)), normalizedLength);
        }

        /// <summary>
        /// Gets a point along the line at the specified index
        /// </summary>
        public Vector3 GetPoint(int pointIndex)
        {
            if (pointIndex < 0 || pointIndex >= PointCount)
            {
                Debug.LogError("Invalid point index");
                return Vector3.zero;
            }

            return TransformPoint(GetPointInternal(pointIndex));
        }

        /// <summary>
        /// Sets a point in the line
        /// This function is not guaranteed to have an effect
        /// </summary>
        public void SetPoint(int pointIndex, Vector3 point)
        {
            if (pointIndex < 0 || pointIndex >= PointCount)
            {
                Debug.LogError("Invalid point index");
                return;
            }

            SetPointInternal(pointIndex, InverseTransformPoint(point));
        }

        /// <summary>
        /// Iterates along line until it finds the point closest to worldPosition
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 worldPosition, int resolution = 5, int maxIterations = 5)
        {
            float length = GetNormalizedLengthFromWorldPos(worldPosition, resolution, maxIterations);
            return GetPoint(length);
        }

        /// <summary>
        /// Iterates along line until it finds the length closest to worldposition.
        /// </summary>
        public float GetNormalizedLengthFromWorldPos(Vector3 worldPosition, int resolution = 5, int maxIterations = 5)
        {
            int iteration = 0;
            float length = GetNormalizedLengthFromWorldPosInternal(worldPosition, 0f, ref iteration, resolution, maxIterations, 0f, 1f);
            return length;
        }

        private Vector3 InverseTransformPoint(Vector3 point)
        {
            switch (transformMode)
            {
                case LinePointTransformMode.UseTransform:
                default:
                    return LineTransform.InverseTransformPoint(point);
                case LinePointTransformMode.UseMatrix:
                    return worldToLocalMatrix.MultiplyPoint3x4(point);
            }
        }

        private Vector3 TransformPoint(Vector3 point)
        {
            switch (transformMode)
            {
                case LinePointTransformMode.UseTransform:
                default:
                    return LineTransform.TransformPoint(point);
                case LinePointTransformMode.UseMatrix:
                    return localToWorldMatrix.MultiplyPoint3x4(point);
            }
        }

        public void UpdateMatrix()
        {
            switch (transformMode)
            {
                case LinePointTransformMode.UseTransform:
                default:
                    return;
                case LinePointTransformMode.UseMatrix:
                    break;
            }

            Transform t = LineTransform;
            if (t.hasChanged)
            {
                t.hasChanged = false;
                localToWorldMatrix = LineTransform.localToWorldMatrix;
                worldToLocalMatrix = LineTransform.worldToLocalMatrix;
            }
        }

        private Matrix4x4 localToWorldMatrix;
        private Matrix4x4 worldToLocalMatrix;

        private float GetNormalizedLengthFromWorldPosInternal(Vector3 worldPosition, float currentLength, ref int iteration, int resolution, int maxIterations, float start, float end)
        {
            iteration++;

            // If we've maxed out our iterations, don't go any further
            if (iteration > maxIterations)
            {
                return currentLength;
            }

            float searchLengthStep = (end - start) / resolution;
            float closestDistanceSoFar = Mathf.Infinity;
            float currentSearchLength = start;

            for (int i = 0; i < resolution; i++)
            {
                Vector3 currentPoint = GetUnClampedPoint(currentSearchLength);

                float distSquared = (currentPoint - worldPosition).sqrMagnitude;
                if (distSquared < closestDistanceSoFar)
                {
                    currentLength = currentSearchLength;
                    closestDistanceSoFar = distSquared;
                }
                currentSearchLength += searchLengthStep;
            }

            // Our start and end lengths will now be 1 resolution to the left and right
            float newStart = currentLength - searchLengthStep;
            float newEnd = currentLength + searchLengthStep;

            if (newStart < 0)
            {
                newEnd -= newStart;
                newStart = 0;
            }

            if (newEnd > 1)
            {
                newEnd = 1;
            }

            return GetNormalizedLengthFromWorldPosInternal(worldPosition, currentLength, ref iteration, resolution, maxIterations, newStart, newEnd);
        }

        private Vector3 DistortPoint(Vector3 point, float normalizedLength)
        {
            if (!distortionEnabled || distorters.Count == 0)
                return point;

            float strength = uniformDistortionStrength;

            if (distortionMode == DistortionMode.NormalizedLength)
            {
                strength = distortionStrength.Evaluate(normalizedLength);
            }
            
            for (int i = 0; i <distorters.Count; i++)
            {
                Distorter distorter = distorters[i];

                if (!distorter.DistortionEnabled)
                    continue;

                point = distorter.DistortPoint(point, strength);
            }

            return point;
        }

        private float ClampedLength(float normalizedLength)
        {
            if (lineStartClamp < MinLineStartClamp)
                lineStartClamp = MinLineStartClamp;
            else if (lineStartClamp > MaxLineEndClamp)
                lineStartClamp = MaxLineEndClamp;

            if (lineEndClamp < MinLineStartClamp)
                lineEndClamp = MinLineStartClamp;
            else if (lineEndClamp > MaxLineEndClamp)
                lineEndClamp = MaxLineEndClamp;

            if (normalizedLength > 1)
                return 1;
            else if (normalizedLength < 0)
                return 0;

            return Mathf.Lerp(lineStartClamp, lineEndClamp, normalizedLength);
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            // Draw a crude, performant gizmo for lines that are unselected
            if (Application.isPlaying || UnityEditor.Selection.activeGameObject == gameObject)
            {
                return;
            }
#endif
            DrawUnselectedGizmosPreview();
        }

        protected virtual void DrawUnselectedGizmosPreview()
        {
            int linePreviewResolution = Mathf.Max(16, PointCount / 4);
            Vector3 firstPosition = FirstPoint;
            Vector3 lastPosition = firstPosition;

            for (int i = 1; i < linePreviewResolution; i++)
            {
                Vector3 currentPosition;

                if (i == linePreviewResolution - 1)
                {
                    currentPosition = LastPoint;
                }
                else
                {
                    float normalizedLength = (1f / (linePreviewResolution - 1)) * i;
                    currentPosition = GetPoint(normalizedLength);
                }

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(lastPosition, currentPosition);

                lastPosition = currentPosition;
            }

            if (Loops)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(lastPosition, firstPosition);
            }
        }
    }
}