// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Distorters;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    public abstract class LineBase : MonoBehaviour
    {
        private const float MinRotationMagnitude = 0.0001f;

        public float UnClampedWorldLength => GetUnClampedWorldLengthInternal();

        [Header("Basic Settings")]

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Clamps the line's normalized start point. This setting will affect line renderers.")]
        private float lineStartClamp = 0f;

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Clamps the line's normalized end point. This setting will affect line renderers.")]
        private float lineEndClamp = 1f;

        [SerializeField]
        [Tooltip("Transform to use when translating points from local to world space. If null, this object's transform is used.")]
        private Transform customLineTransform;

        [SerializeField]
        [Tooltip("Controls whether this line loops (Note: some classes override this setting)")]
        private bool loops = false;

        public virtual bool Loops
        {
            get { return loops; }
            protected set { loops = value; }
        }

        [Header("Rotation")]

        [SerializeField]
        [Tooltip("The rotation mode used in the GetRotation function. You can visualize rotations by checking Draw Rotations under Editor Settings.")]
        private LineRotationType rotationType = LineRotationType.Velocity;

        [SerializeField]
        [Tooltip("Reverses up vector when determining rotation along line")]
        private bool flipUpVector = false;

        [SerializeField]
        [Tooltip("Local space offset to transform position. Used to determine rotation along line in RelativeToOrigin rotation mode")]
        private Vector3 originOffset = Vector3.zero;

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("The weight of manual up vectors in Velocity rotation mode")]
        private float manualUpVectorBlend = 0f;

        [SerializeField]
        [Tooltip("These vectors are used with ManualUpVectorBlend to determine rotation along the line in Velocity rotation mode. Vectors are distributed along the normalized length of the line.")]
        private Vector3[] manualUpVectors = { Vector3.up, Vector3.up, Vector3.up };

        [SerializeField]
        [Range(0.0001f, 0.1f)]
        [Tooltip("Used in Velocity rotation mode. Smaller values are more accurate but more expensive")]
        private float velocitySearchRange = 0.02f;

        [Header("Distortion")]

        [SerializeField]
        [Tooltip("NormalizedLength mode uses the DistortionStrength curve for distortion strength, Uniform uses UniformDistortionStrength along entire line")]
        private DistortionType distortionType = DistortionType.NormalizedLength;

        [SerializeField]
        private AnimationCurve distortionStrength = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [Range(0f, 1f)]
        [SerializeField]
        private float uniformDistortionStrength = 1f;

        [SerializeField]
        [Tooltip("A list of distorters that apply to this line")]
        private List<Distorter> distorters = new List<Distorter>();

        /// <summary>
        /// The number of points this line has.
        /// </summary>
        public abstract int PointCount { get; }

        /// <summary>
        /// Sets the point at index.
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <param name="point"></param>
        protected abstract void SetPointInternal(int pointIndex, Vector3 point);

        /// <summary>
        /// Get a point based on normalized distance along line
        /// Normalized distance will be pre-clamped
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        protected abstract Vector3 GetPointInternal(float normalizedLength);

        /// <summary>
        /// Get a point based on point index
        /// Point index will be pre-clamped
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <returns></returns>
        protected abstract Vector3 GetPointInternal(int pointIndex);

        /// <summary>
        /// Gets the up vector at a normalized length along line (used for rotation)
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        protected virtual Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return LineTransform.forward;
        }

        /// <summary>
        /// Get the UnClamped world length of the line
        /// </summary>
        /// <returns></returns>
        protected abstract float GetUnClampedWorldLengthInternal();

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

        public Transform LineTransform => customLineTransform != null ? customLineTransform : transform;

        public void AddDistorter(Distorter newDistorter)
        {
            if (!distorters.Contains(newDistorter))
            {
                distorters.Add(newDistorter);
            }
        }

        /// <summary>
        /// Places all points between the first and last point in a straight line
        /// </summary>
        public virtual void MakeStraightLine()
        {
            if (PointCount > 2)
            {
                Vector3 startPosition = GetPoint(0);
                Vector3 endPosition = GetPoint(PointCount - 1);

                for (int i = 1; i < PointCount - 2; i++)
                {
                    SetPoint(i, Vector3.Lerp(startPosition, endPosition, (1f / PointCount * 1)));
                }
            }
        }

        /// <summary>
        /// Returns a normalized length corresponding to a world length
        /// Useful for determining LineStartClamp / LineEndClamp values
        /// </summary>
        /// <param name="worldLength"></param>
        /// <param name="searchResolution"></param>
        /// <returns></returns>
        public float GetNormalizedLengthFromWorldLength(float worldLength, int searchResolution = 10)
        {
            Vector3 lastPoint = GetUnClampedPoint(0f);
            float normalizedLength = 0f;
            float distanceSoFar = 0f;

            for (int i = 1; i < searchResolution; i++)
            {
                // Get the normalized length of this position along the line
                normalizedLength = (1f / searchResolution) * i;
                Vector3 currentPoint = GetUnClampedPoint(normalizedLength);
                distanceSoFar += Vector3.Distance(lastPoint, currentPoint);
                lastPoint = currentPoint;

                if (distanceSoFar >= worldLength)
                {
                    // We've reached the world length
                    break;
                }
            }

            return Mathf.Clamp01(normalizedLength);
        }

        /// <summary>
        /// Gets the velocity along the line
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
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
        /// <param name="normalizedLength"></param>
        /// <param name="lineRotationType"></param>
        /// <returns></returns>
        public Quaternion GetRotation(float normalizedLength, LineRotationType lineRotationType = LineRotationType.None)
        {
            lineRotationType = (lineRotationType != LineRotationType.None) ? lineRotationType : rotationType;
            Vector3 rotationVector = Vector3.zero;

            switch (lineRotationType)
            {
                case LineRotationType.Velocity:
                    rotationVector = GetVelocity(normalizedLength);
                    break;
                case LineRotationType.RelativeToOrigin:
                    Vector3 point = GetPoint(normalizedLength);
                    Vector3 origin = LineTransform.TransformPoint(originOffset);
                    rotationVector = (point - origin).normalized;
                    break;
                case LineRotationType.None:
                    break;
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
        /// <param name="pointIndex"></param>
        /// <param name="lineRotationType"></param>
        /// <returns></returns>
        public Quaternion GetRotation(int pointIndex, LineRotationType lineRotationType = LineRotationType.None)
        {
            return GetRotation((float)pointIndex / PointCount, lineRotationType != LineRotationType.None ? lineRotationType : rotationType);
        }

        /// <summary>
        /// Gets a point along the line at the specified normalized length.
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        public Vector3 GetPoint(float normalizedLength)
        {
            normalizedLength = ClampedLength(normalizedLength);
            return DistortPoint(LineTransform.TransformPoint(GetPointInternal(normalizedLength)), normalizedLength);
        }

        /// <summary>
        /// Gets a point along the line at the specified length without using LineStartClamp or LineEndClamp
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        public Vector3 GetUnClampedPoint(float normalizedLength)
        {
            normalizedLength = Mathf.Clamp01(normalizedLength);
            return DistortPoint(LineTransform.TransformPoint(GetPointInternal(normalizedLength)), normalizedLength);
        }

        /// <summary>
        /// Gets a point along the line at the specified index
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <returns></returns>
        public Vector3 GetPoint(int pointIndex)
        {
            if (pointIndex < 0 || pointIndex >= PointCount)
            {
                Debug.LogError("Invalid point index");
                return Vector3.zero;
            }

            return LineTransform.TransformPoint(GetPointInternal(pointIndex));
        }

        /// <summary>
        /// Sets a point in the line
        /// This function is not guaranteed to have an effect
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <param name="point"></param>
        public void SetPoint(int pointIndex, Vector3 point)
        {
            if (pointIndex < 0 || pointIndex >= PointCount)
            {
                Debug.LogError("Invalid point index");
                return;
            }

            SetPointInternal(pointIndex, LineTransform.InverseTransformPoint(point));
        }

        public virtual void AppendPoint(Vector3 point)
        {
            // Does nothing by default
        }

        protected virtual void OnEnable()
        {
            distorters.Sort();
        }

        private Vector3 DistortPoint(Vector3 point, float normalizedLength)
        {
            float strength = uniformDistortionStrength;

            if (distortionType == DistortionType.NormalizedLength)
            {
                strength = distortionStrength.Evaluate(normalizedLength);
            }

            for (int i = 0; i < distorters.Count; i++)
            {
                // Components may be added or removed
                if (distorters[i] != null)
                {
                    point = distorters[i].DistortPoint(point, strength);
                }
            }

            return point;
        }

        private float ClampedLength(float normalizedLength)
        {
            return Mathf.Lerp(Mathf.Max(lineStartClamp, 0.0001f), Mathf.Min(lineEndClamp, 0.9999f), Mathf.Clamp01(normalizedLength));
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            // Show gizmos if this object is not selected
            // (SceneGUI will display it otherwise)

            if (Application.isPlaying || UnityEditor.Selection.activeGameObject == gameObject)
            {
                return;
            }

            // Only draw a gizmo if we don't have a line renderer
            var lineRenderer = gameObject.GetComponent<LineRendererBase>();

            if (lineRenderer != null)
            {
                return;
            }

            Vector3 firstPos = GetPoint(0f);
            Vector3 lastPos = firstPos;
            Gizmos.color = Color.Lerp(Color.white, Color.clear, 0.25f);
            const int numSteps = 16;

            for (int i = 1; i < numSteps; i++)
            {
                float normalizedLength = (1f / (numSteps - 1)) * i;
                Vector3 currentPos = GetPoint(normalizedLength);
                Gizmos.DrawLine(lastPos, currentPos);
                lastPos = currentPos;
            }

            if (Loops)
            {
                Gizmos.DrawLine(lastPos, firstPos);
            }
        }
#endif
    }
}