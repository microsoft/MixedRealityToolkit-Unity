// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class that provides data about a line.
    /// </summary>
    /// <remarks>Data to be consumed by other classes like the <see cref="BaseMixedRealityLineRenderer"/></remarks>
    [ExecuteAlways]
    internal abstract class BaseMixedRealityLineDataProvider : MonoBehaviour
    {
        #region Properties

        [Range(MinLineStartClamp, MaxLineEndClamp)]
        [SerializeField]
        [Tooltip("Clamps the line's normalized start point. This setting will affect line renderers.")]
        private float lineStartClamp = MinLineStartClamp;

        /// <summary>
        /// Clamps the line's normalized start point. This setting will affect line renderers.
        /// </summary>
        public float LineStartClamp
        {
            get => lineStartClamp;
            set => lineStartClamp = Mathf.Clamp(value, MinLineStartClamp, MaxLineEndClamp);
        }

        [Range(MinLineStartClamp, MaxLineEndClamp)]
        [SerializeField]
        [Tooltip("Clamps the line's normalized end point. This setting will affect line renderers.")]
        private float lineEndClamp = MaxLineEndClamp;

        /// <summary>
        /// Clamps the line's normalized end point. This setting will affect line renderers.
        /// </summary>
        public float LineEndClamp
        {
            get => lineEndClamp;
            set => lineEndClamp = Mathf.Clamp(value, MinLineStartClamp, MaxLineEndClamp);
        }

        [SerializeField]
        [Tooltip("Transform to use when translating points from local to world space. If null, this object's transform is used.")]
        private Transform customLineTransform;

        /// <summary>
        /// Transform to use when translating points from local to world space. If null, this object's transform is used.
        /// </summary>
        public Transform LineTransform
        {
            get => customLineTransform != null ? customLineTransform : transform;
            set => customLineTransform = value;
        }

        /// <summary>
        /// Returns world position of first point along line as defined by this data provider
        /// </summary>
        public Vector3 FirstPoint
        {
            get => GetPoint(0);
            set => SetPoint(0, value);
        }

        /// <summary>
        /// Returns world position of last point along line as defined by this data provider
        /// </summary>
        public Vector3 LastPoint
        {
            get => GetPoint(PointCount - 1);
            set => SetPoint(PointCount - 1, value);
        }

        public float UnClampedWorldLength => GetUnClampedWorldLengthInternal();

        #endregion

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

        private Matrix4x4 localToWorldMatrix;
        private Matrix4x4 worldToLocalMatrix;

        protected const int UnclampedWorldLengthSearchSteps = 10;
        private const float MinRotationMagnitude = 0.0001f;
        private const float MinLineStartClamp = 0.0001f;
        private const float MaxLineEndClamp = 0.9999f;

        #endregion BaseMixedRealityLineDataProvider Abstract Declarations

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

                float currDistance = Vector3.Distance(lastPoint, currentPoint);
                distanceSoFar += currDistance;

                if (distanceSoFar >= worldLength)
                {
                    // We've reached the world length, so subtract the amount we overshot
                    normalizedLength -= (distanceSoFar - worldLength) / currDistance * normalizedSegmentLength;
                    break;
                }

                lastPoint = currentPoint;
            }

            return Mathf.Clamp01(normalizedLength);
        }

        /// <summary>
        /// Gets a point along the line at the specified normalized length.
        /// </summary>
        public Vector3 GetPoint(float normalizedLength)
        {
            normalizedLength = Mathf.Lerp(lineStartClamp, lineEndClamp, Mathf.Clamp01(normalizedLength));
            Vector3 point = GetPointInternal(normalizedLength);
            TransformPoint(ref point);
            return point;
        }

        /// <summary>
        /// Gets a point along the line at the specified length without using LineStartClamp or LineEndClamp
        /// </summary>
        public Vector3 GetUnClampedPoint(float normalizedLength)
        {
            normalizedLength = Mathf.Clamp01(normalizedLength);
            Vector3 point = GetPointInternal(normalizedLength);
            TransformPoint(ref point);
            return point;
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

            Vector3 point = GetPointInternal(pointIndex);
            TransformPoint(ref point);
            return point;
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

            InverseTransformPoint(ref point);
            SetPointInternal(pointIndex, point);
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
            return GetNormalizedLengthFromWorldPosInternal(worldPosition, 0f, ref iteration, resolution, maxIterations, 0f, 1f);
        }

        private void InverseTransformPoint(ref Vector3 point)
        {
            point = LineTransform.InverseTransformPoint(point);
        }

        private void TransformPoint(ref Vector3 point)
        {
            point = LineTransform.TransformPoint(point);
        }

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
        }
    }
}
