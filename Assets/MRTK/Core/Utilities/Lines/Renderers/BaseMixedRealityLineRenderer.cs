// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Base class for Mixed Reality Line Renderers.
    /// </summary>
    [ExecuteAlways]
    public abstract class BaseMixedRealityLineRenderer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The line data this component will render")]
        protected BaseMixedRealityLineDataProvider lineDataSource;

        /// <summary>
        /// The data provider component that provides the positioning source information for the LineRenderer.
        /// </summary>
        public BaseMixedRealityLineDataProvider LineDataSource
        {
            get
            {
                if (lineDataSource == null)
                {
                    lineDataSource = GetComponent<BaseMixedRealityLineDataProvider>();

                    if (lineDataSource != null)
                    {
                        var lineDataType = lineDataSource.GetType();

                        if (lineDataType == typeof(RectangleLineDataProvider))
                        {
                            StepMode = StepMode.FromSource;
                        }
                    }
                }

                if (lineDataSource == null)
                {
                    Debug.LogError($"Missing a Line Data Provider on {gameObject.name}");
                    enabled = false;
                }

                return lineDataSource;
            }
            set
            {
                lineDataSource = value;
                enabled = lineDataSource != null;
            }
        }

        [Header("Visual Settings")]

        [SerializeField]
        [Tooltip("Color gradient applied to line's normalized length")]
        private Gradient lineColor = new Gradient();

        /// <summary>
        /// Color gradient applied to line's normalized length
        /// </summary>
        public Gradient LineColor
        {
            get => lineColor;
            set => lineColor = value;
        }

        [SerializeField]
        private AnimationCurve lineWidth = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public AnimationCurve LineWidth
        {
            get => lineWidth;
            set => lineWidth = value;
        }

        [Range(0.01f, 10f)]
        [SerializeField]
        private float widthMultiplier = 0.01f;

        public float WidthMultiplier
        {
            get => widthMultiplier;
            set => widthMultiplier = Mathf.Clamp(value, 0f, 10f);
        }

        [Header("Offsets")]

        [Range(0f, 10f)]
        [SerializeField]
        [Tooltip("Normalized offset for color gradient")]
        private float colorOffset = 0f;

        /// <summary>
        /// Normalized offset for color gradient
        /// </summary>
        public float ColorOffset
        {
            get => colorOffset;
            set => colorOffset = Mathf.Clamp(value, 0f, 10f);
        }

        [Range(0f, 10f)]
        [SerializeField]
        [Tooltip("Normalized offset for width curve")]
        private float widthOffset = 0f;

        /// <summary>
        /// Normalized offset for width curve
        /// </summary>
        public float WidthOffset
        {
            get => widthOffset;
            set => widthOffset = Mathf.Clamp(value, 0f, 10f);
        }

        [Header("Point Placement")]

        [SerializeField]
        [Tooltip("Method for gathering points along line. Interpolated uses normalized length. FromSource uses line's base points. (FromSource may not look right for all LineDataProvider types.)")]
        private StepMode stepMode = StepMode.Interpolated;

        /// <summary>
        /// Method for gathering points along line. Interpolated uses normalized length. FromSource uses line's base points. (FromSource may not look right for all LineDataProvider types.)
        /// </summary>
        public StepMode StepMode
        {
            get => stepMode;
            set => stepMode = value;
        }

        [Range(2, 2048)]
        [SerializeField]
        [Tooltip("Number of steps to interpolate along line in Interpolated step mode")]
        private int lineStepCount = 16;

        [SerializeField]
        [Tooltip("Method for distributing rendered points along line. Auto lets the implementation decide. None means normalized distribution. DistanceSingleValue ensures uniform distribution. DistanceCurveValue enables custom distribution.")]
        private PointDistributionMode pointDistributionMode = PointDistributionMode.Auto;

        /// <summary>
        /// Method for distributing rendered points along line.
        /// </summary>
        public PointDistributionMode PointDistributionMode
        {
            get => pointDistributionMode;
            set => pointDistributionMode = value;
        }

        [SerializeField]
        [Tooltip("Minimum distance between points distributed along curve. Used when PointDistributionMode is set to DistanceSingleValue. Total points capped by LineStepCount.")]
        private float customPointDistributionLength = 0.1f;

        [SerializeField]
        [Tooltip("Custom function for distributing points along curve.Used when DistanceCurveValue is set to Distance. Total points set by LineStepCount.")]
        private AnimationCurve customPointDistributionCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Number of steps to interpolate along line in Interpolated step mode
        /// </summary>
        public int LineStepCount
        {
            get => lineStepCount;
            set => lineStepCount = Mathf.Clamp(value, 2, 2048);
        }

        /// <summary>
        /// Get the <see href="https://docs.unity3d.com/ScriptReference/Color.html">Color</see> along the normalized length of the line.
        /// </summary>
        protected virtual Color GetColor(float normalizedLength)
        {
            if (LineColor == null)
            {
                LineColor = new Gradient();
            }

            return LineColor.Evaluate(Mathf.Repeat(normalizedLength + colorOffset, 1f));
        }

        /// <summary>
        /// Get the width of the line along the normalized length of the line.
        /// </summary>
        protected virtual float GetWidth(float normalizedLength)
        {
            if (LineWidth == null)
            {
                LineWidth = AnimationCurve.Linear(0f, 1f, 1f, 1f);
            }

            return LineWidth.Evaluate(Mathf.Repeat(normalizedLength + widthOffset, 1f)) * widthMultiplier;
        }

        /// <summary>
        /// Gets the normalized distance along the line path (range 0 to 1) going the given number of steps provided
        /// </summary>
        /// <param name="stepNum">Number of steps to take "walking" along the curve </param>
        protected virtual float GetNormalizedPointAlongLine(int stepNum)
        {
            float normalizedDistance = 0;

            switch (pointDistributionMode)
            {
                case PointDistributionMode.None:
                case PointDistributionMode.Auto:
                    // Normalized length along line
                    normalizedDistance = (1f / (LineStepCount - 1)) * stepNum;
                    break;

                case PointDistributionMode.DistanceCurveValue:
                    // Use curve to interpret value
                    normalizedDistance = (1f / (LineStepCount - 1)) * stepNum;
                    normalizedDistance = customPointDistributionCurve.Evaluate(normalizedDistance);
                    break;

                case PointDistributionMode.DistanceSingleValue:
                    // Get the normalized distance along curve
                    float totalWorldLength = customPointDistributionLength * stepNum;
                    normalizedDistance = lineDataSource.GetNormalizedLengthFromWorldLength(totalWorldLength);
                    break;
            }

            return normalizedDistance;
        }

        private void LateUpdate()
        {
            UpdateLine();
        }

        /// <summary>
        /// Executes every Unity LateUpdate(). Any property updates or frame updates should occur here to update the line data source.
        /// </summary>
        protected abstract void UpdateLine();

        #region Gizmos

#if UNITY_EDITOR

        protected virtual void OnDrawGizmos()
        {
            if (UnityEditor.Selection.activeGameObject == gameObject || Application.isPlaying) { return; }

            if (lineDataSource == null)
            {
                lineDataSource = gameObject.GetComponent<BaseMixedRealityLineDataProvider>();
            }

            if (lineDataSource == null || !lineDataSource.enabled)
            {
                return;
            }

            GizmosDrawLineRenderer();
        }

        private void GizmosDrawLineRenderer()
        {
            if (stepMode == StepMode.FromSource)
            {
                GizmosDrawLineFromSource();
            }
            else
            {
                GizmosDrawLineInterpolated();
            }
        }

        private void GizmosDrawLineFromSource()
        {
            Vector3 firstPos = lineDataSource.GetPoint(0);
            Vector3 lastPos = firstPos;

            Color gColor = GetColor(0);
            gColor.a = 0.15f;
            Gizmos.color = gColor;
            Gizmos.DrawSphere(firstPos, GetWidth(0) * 0.5f);

            for (int i = 1; i < lineDataSource.PointCount; i++)
            {
                float normalizedLength = (1f / lineDataSource.PointCount) * i;
                Vector3 currentPos = lineDataSource.GetPoint(i);

                gColor = GetColor(normalizedLength);
                Gizmos.color = gColor.Invert();
                Gizmos.DrawLine(lastPos, currentPos);

                gColor.a = 0.15f;
                Gizmos.color = gColor;
                Gizmos.DrawSphere(currentPos, GetWidth(normalizedLength) * 0.5f);

                lastPos = currentPos;
            }

            if (lineDataSource.Loops)
            {
                Gizmos.color = gColor.Invert();
                Gizmos.DrawLine(lastPos, firstPos);
            }
        }

        private void GizmosDrawLineInterpolated()
        {
            Vector3 firstPos = lineDataSource.GetPoint(0f);
            Vector3 lastPos = firstPos;
            Color gColor = GetColor(0f);

            gColor.a = 0.15f;
            Gizmos.color = gColor;
            Gizmos.DrawSphere(firstPos, GetWidth(0f) * 0.5f);

            for (int i = 1; i <= lineStepCount; i++)
            {
                float normalizedLength = (1f / lineStepCount) * i;
                Vector3 currentPos = lineDataSource.GetPoint(normalizedLength);

                gColor = GetColor(normalizedLength);
                Gizmos.color = gColor.Invert();
                Gizmos.DrawLine(lastPos, currentPos);

                gColor.a = 0.15f;
                Gizmos.color = gColor;
                Gizmos.DrawSphere(currentPos, GetWidth(normalizedLength) * 0.5f);
                lastPos = currentPos;
            }

            if (lineDataSource.Loops)
            {
                Gizmos.color = gColor.Invert();
                Gizmos.DrawLine(lastPos, firstPos);
            }
        }

#endif
        #endregion
    }
}