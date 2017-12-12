// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public abstract class LineBase : MonoBehaviour
    {
        protected const float MinRotationMagnitude = 0.0001f;

        #region fields & properties

        public float UnclampedWorldLength
        {
            get
            {
                return GetUnclampedWorldLengthInternal();
            }
        }

        [Header("Basic Settings")]
        [Tooltip("Clamps the line's normalized start point. This setting will affect line renderers.")]
        [Range(0f, 1f)]
        public float LineStartClamp = 0f;
        [Range(0f, 1f)]
        [Tooltip("Clamps the line's normalized end point. This setting will affect line renderers.")]
        public float LineEndClamp = 1f;
        
        public virtual bool Loops
        {
            get
            {
                return loops;
            }
        }

        [Header("Rotation")]
        [Tooltip("The rotation mode used in the GetRotation function. You can visualize rotations by checking Draw Rotations under Editor Settings.")]
        public RotationTypeEnum RotationType = RotationTypeEnum.Velocity;

        [Tooltip("Reverses up vector when determining rotation along line")]
        public bool FlipUpVector = false;

        [Tooltip("Local space offset to transform position. Used to determine rotation along line in RelativeToOrigin rotation mode")]
        public Vector3 OriginOffset = Vector3.zero;

        [Tooltip("The weight of manual up vectors in Velocity rotation mode")]
        [Range(0f,1f)]
        public float ManualUpVectorBlend = 0f;
         
        [Tooltip("These vectors are used with ManualUpVectorBlend to determine rotation along the line in Velocity rotation mode. Vectors are distributed along the normalized length of the line.")]
        public Vector3[] ManualUpVectors = new Vector3[] { Vector3.up, Vector3.up, Vector3.up };

        [Tooltip("Used in Velocity rotation mode. Smaller values are more accurate but more expensive")]
        [Range(0.0001f, 0.1f)]
        public float VelocitySearchRange = 0.02f;
        [Range(0f, 1f)]
        public float VelocityBlend = 0.5f;

        [Header ("Distortion")]
        [Tooltip("NormalizedLength mode uses the DistortionStrength curve for distortion strength, Uniform uses UniformDistortionStrength along entire line")]
        public DistortionTypeEnum DistortionType = DistortionTypeEnum.NormalizedLength;
        public AnimationCurve DistortionStrength = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [Range(0f, 1f)]
        public float UniformDistortionStrength = 1f;

        [SerializeField]
        [Tooltip("A list of distorters that apply to this line")]
        protected List<Distorter> distorters = new List<Distorter>();

        [Tooltip("Controls whether this line loops (Note: some classes override this setting)")]
        [SerializeField]
        protected bool loops = false;

        #endregion

        #region abstract

        public abstract int NumPoints { get; }

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
            return transform.forward;
        }

        /// <summary>
        /// Get the UNCLAMPED world length of the line
        /// </summary>
        /// <returns></returns>
        protected abstract float GetUnclampedWorldLengthInternal();

        #endregion

        #region public

        // Convenience
        public Vector3 FirstPoint
        {
            get
            {
                return GetPoint(0);
            }
            set
            {
                SetPoint(0, value);
            }
        }

        public Vector3 LastPoint
        {
            get
            {
                return GetPoint(NumPoints - 1);
            }
            set
            {
                SetPoint(NumPoints - 1, value);
            }
        }

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
            if (NumPoints > 2)
            {
                Vector3 startPosition = GetPoint(0);
                Vector3 endPosition = GetPoint(NumPoints - 1);
                for (int i = 1; i < NumPoints - 2; i++)
                {
                    SetPoint(i, Vector3.Lerp(startPosition, endPosition, (1f / NumPoints * 1)));
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
        public float GetNormalizedLengthFromWorldLength (float worldLength, int searchResolution = 10)
        {
            Vector3 lastPoint = GetUnclampedPoint(0f);
            Vector3 currentPoint = Vector3.zero;
            float normalizedLength = 0f;
            float distanceSoFar = 0f;

            for (int i = 1; i < searchResolution; i++)
            {
                // Get the normalized length of this position along the line
                normalizedLength = (1f / searchResolution) * i;
                currentPoint = GetUnclampedPoint(normalizedLength);
                distanceSoFar += Vector3.Distance(lastPoint, currentPoint);
                lastPoint = currentPoint;

                if (distanceSoFar >= worldLength)
                {
                    // We've reached the world length
                    break;
                };
            }

            return Mathf.Clamp01 (normalizedLength);
        }

        /// <summary>
        /// Gets the velocity along the line
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        public Vector3 GetVelocity(float normalizedLength)
        {
            Vector3 velocity = Vector3.zero;
            if (normalizedLength < VelocitySearchRange)
            {
                Vector3 currentPos = GetPoint(normalizedLength);
                Vector3 nextPos = GetPoint(normalizedLength + VelocitySearchRange);
                velocity = (nextPos - currentPos).normalized;
            }
            else
            {
                Vector3 currentPos = GetPoint(normalizedLength);
                Vector3 prevPos = GetPoint(normalizedLength - VelocitySearchRange);
                velocity = (currentPos - prevPos).normalized;
            }
            return velocity;
        }

        /// <summary>
        /// Gets the rotation of a point along the line at the specified length
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <param name="rotationType"></param>
        /// <returns></returns>
        public Quaternion GetRotation(float normalizedLength, RotationTypeEnum rotationType = RotationTypeEnum.None)
        {
            rotationType = (rotationType != RotationTypeEnum.None) ? rotationType : RotationType;
            Vector3 rotationVector = Vector3.zero;

            switch (rotationType)
            {
                case RotationTypeEnum.None:
                default:
                    break;

                case RotationTypeEnum.Velocity:
                    rotationVector = GetVelocity(normalizedLength);
                    break;

                case RotationTypeEnum.RelativeToOrigin:
                    Vector3 point = GetPoint(normalizedLength);
                    Vector3 origin = transform.TransformPoint(OriginOffset);
                    rotationVector = (point - origin).normalized;
                    break;
            }

            if (rotationVector.magnitude < MinRotationMagnitude)
            {
                return transform.rotation;
            }

            Vector3 upVector = GetUpVectorInternal(normalizedLength);

            if (ManualUpVectorBlend > 0f)
            {
                Vector3 manualUpVector = LineUtils.GetVectorCollectionBlend(ManualUpVectors, normalizedLength, Loops);
                upVector = Vector3.Lerp(upVector, manualUpVector, manualUpVector.magnitude);
            }

            if (FlipUpVector)
            {
                upVector = -upVector;
            }

            return Quaternion.LookRotation(rotationVector, upVector);
        }

        /// <summary>
        /// Gets the rotation of a point along the line at the specified index
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <param name="rotationType"></param>
        /// <returns></returns>
        public Quaternion GetRotation (int pointIndex, RotationTypeEnum rotationType = RotationTypeEnum.None)
        {
            return GetRotation((float)pointIndex / NumPoints, (rotationType != RotationTypeEnum.None) ? rotationType : RotationType);
        }

        /// <summary>
        /// Gets a point along the line at the specified length
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        public Vector3 GetPoint(float normalizedLength)
        {
            normalizedLength = ClampedLength(normalizedLength);
            return DistortPoint (transform.TransformPoint(GetPointInternal(normalizedLength)), normalizedLength);
        }

        /// <summary>
        /// Gets a point along the line at the specified length without using LineStartClamp or LineEndClamp
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        public Vector3 GetUnclampedPoint(float normalizedLength)
        {
            normalizedLength = Mathf.Clamp01(normalizedLength);
            return DistortPoint(transform.TransformPoint(GetPointInternal(normalizedLength)), normalizedLength);
        }

        /// <summary>
        /// Gets a point along the line at the specified index
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <returns></returns>
        public Vector3 GetPoint (int pointIndex)
        {
            if (pointIndex < 0 || pointIndex >= NumPoints)
            {
                throw new System.IndexOutOfRangeException();
            }

            return transform.TransformPoint(GetPointInternal(pointIndex));
        }

        /// <summary>
        /// Sets a point in the line
        /// This function is not guaranteed to have an effect
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <param name="point"></param>
        public void SetPoint (int pointIndex, Vector3 point)
        {
            if (pointIndex < 0 || pointIndex >= NumPoints)
            {
                throw new System.IndexOutOfRangeException();
            }

            SetPointInternal(pointIndex, transform.InverseTransformPoint(point));
        }

        public virtual void AppendPoint(Vector3 point)
        {
            // Does nothing by default
        }

        #endregion

        #region private & protected

        protected virtual void OnEnable()
        {
            // Sort our distorters
            distorters.Sort();
        }

        private Vector3 DistortPoint (Vector3 point, float normalizedLength)
        {
            float strength = UniformDistortionStrength;
            switch (DistortionType)
            {
                case DistortionTypeEnum.Uniform:
                default:
                    break;

                case DistortionTypeEnum.NormalizedLength:
                    strength = DistortionStrength.Evaluate(normalizedLength);
                    break;
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
            return Mathf.Lerp(Mathf.Max (LineStartClamp, 0.0001f), Mathf.Min (LineEndClamp, 0.9999f), Mathf.Clamp01(normalizedLength));
        }

        #endregion

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            // Show gizmos if this object is not selected
            // (SceneGUI will display it otherwise)

            if (Application.isPlaying)
            {
                return;
            }

            if (UnityEditor.Selection.activeGameObject == this.gameObject)
            {
                return;
            }

            // Only draw a gizmo if we don't have a line renderer
            LineRendererBase lineRenderer = gameObject.GetComponent<LineRendererBase>();
            if (lineRenderer != null)
            {
                return;
            }

            Vector3 firstPos = GetPoint(0f);
            Vector3 lastPos = firstPos;
            Gizmos.color = Color.Lerp (LineBaseEditor.DefaultDisplayLineColor, Color.clear, 0.25f);
            int numSteps = 16;

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