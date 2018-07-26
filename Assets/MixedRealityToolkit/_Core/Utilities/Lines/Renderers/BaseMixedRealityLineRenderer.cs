// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.Renderers
{
    public abstract class BaseMixedRealityLineRenderer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The source LineDataProvider this component will render")]
        private BaseMixedRealityLineDataProvider source;

        public BaseMixedRealityLineDataProvider Source
        {
            get
            {
                if (source == null)
                {
                    source = GetComponent<BaseMixedRealityLineDataProvider>();
                }

                enabled = source != null;
                return source;
            }
            set
            {
                source = value;
                enabled = source != null;
            }
        }

        [Header("Visual Settings")]

        [SerializeField]
        [Tooltip("Color gradient applied to line's normalized length")]
        private Gradient lineColor;

        public Gradient LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }

        [SerializeField]
        private AnimationCurve lineWidth = AnimationCurve.Linear(0f, 0.05f, 1f, 0.05f);

        public AnimationCurve LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; }
        }

        [Range(0f, 10f)]
        [SerializeField]
        private float widthMultiplier = 0.25f;

        public float WidthMultiplier
        {
            get { return widthMultiplier; }
            set
            {
                if (value < 0f)
                {
                    widthMultiplier = 0f;
                }
                else if (value > 10f)
                {
                    widthMultiplier = 10f;
                }
                else
                {
                    widthMultiplier = value;
                }
            }
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
            get { return colorOffset; }
            set
            {
                if (value < 0f)
                {
                    colorOffset = 0f;
                }
                else if (value > 10f)
                {
                    colorOffset = 10f;
                }
                else
                {
                    colorOffset = value;
                }
            }
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
            get { return widthOffset; }
            set
            {
                if (value < 0f)
                {
                    widthOffset = 0f;
                }
                else if (value > 10f)
                {
                    widthOffset = 10f;
                }
                else
                {
                    widthOffset = value;
                }
            }
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
            get { return stepMode; }
            set { stepMode = value; }
        }

        [Range(0, 2048)]
        [SerializeField]
        [Tooltip("Number of steps to interpolate along line in Interpolated step mode")]
        private int lineStepCount = 10;

        /// <summary>
        /// Number of steps to interpolate along line in Interpolated step mode
        /// </summary>
        public int LineStepCount
        {
            get { return lineStepCount; }
            set
            {
                if (value < 0)
                {
                    lineStepCount = 0;
                }
                else if (value > 2048)
                {
                    lineStepCount = 2048;
                }
                else
                {
                    lineStepCount = value;
                }
            }
        }

        /// <summary>
        /// Get the <see cref="Color"/> along the normalized length of the line.
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        protected virtual Color GetColor(float normalizedLength)
        {
            if (lineColor == null)
            {
                lineColor = new Gradient();
            }

            return lineColor.Evaluate(Mathf.Repeat(normalizedLength + colorOffset, 1f));
        }

        /// <summary>
        /// Get the width of the line along the normalized length of the line.
        /// </summary>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        protected virtual float GetWidth(float normalizedLength)
        {
            if (lineWidth == null)
            {
                lineWidth = AnimationCurve.Linear(0f, 1f, 1f, 1f);
            }

            return lineWidth.Evaluate(Mathf.Repeat(normalizedLength + widthOffset, 1f)) * widthMultiplier;
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying) { return; }

            if (source == null)
            {
                source = gameObject.GetComponent<BaseMixedRealityLineDataProvider>();

            }

            if (source == null || !source.enabled)
            {
                return;
            }

            GizmosDrawLineRenderer(source, this);
        }

        private static void GizmosDrawLineRenderer(BaseMixedRealityLineDataProvider source, BaseMixedRealityLineRenderer renderer)
        {
            if (renderer.stepMode == StepMode.FromSource)
            {
                GizmosDrawLineFromSource(source, renderer);
            }
            else
            {
                GizmosDrawLineInterpolated(source, renderer);
            }
        }

        private static void GizmosDrawLineFromSource(BaseMixedRealityLineDataProvider source, BaseMixedRealityLineRenderer renderer)
        {
            Vector3 firstPos = source.GetPoint(0);
            Vector3 lastPos = firstPos;
            Color gColor = renderer.GetColor(0);

            gColor.a = 0.5f;
            Gizmos.color = gColor;
            Gizmos.DrawSphere(firstPos, renderer.GetWidth(0) * 0.5f);

            for (int i = 1; i < source.PointCount; i++)
            {
                float normalizedLength = (1f / source.PointCount) * i;
                Vector3 currentPos = source.GetPoint(i);
                gColor = renderer.GetColor(normalizedLength);
                gColor.a = gColor.a * 0.5f;
                Gizmos.color = gColor;
                Gizmos.DrawLine(lastPos, currentPos);
                Gizmos.DrawSphere(currentPos, renderer.GetWidth(normalizedLength) * 0.5f);
                lastPos = currentPos;
            }

            if (source.Loops)
            {
                Gizmos.DrawLine(lastPos, firstPos);
            }
        }

        private static void GizmosDrawLineInterpolated(BaseMixedRealityLineDataProvider source, BaseMixedRealityLineRenderer renderer)
        {
            Vector3 firstPos = source.GetPoint(0f);
            Vector3 lastPos = firstPos;
            Color gColor = renderer.GetColor(0f);

            gColor.a = 0.5f;
            Gizmos.color = gColor;
            Gizmos.DrawSphere(firstPos, renderer.GetWidth(0f) * 0.5f);

            for (int i = 1; i <= renderer.lineStepCount; i++)
            {
                float normalizedLength = (1f / renderer.lineStepCount) * i;
                Vector3 currentPos = source.GetPoint(normalizedLength);
                gColor = renderer.GetColor(normalizedLength);
                gColor.a = gColor.a * 0.5f;
                Gizmos.color = gColor;
                Gizmos.DrawLine(lastPos, currentPos);
                Gizmos.DrawSphere(currentPos, renderer.GetWidth(normalizedLength) * 0.5f);
                lastPos = currentPos;
            }

            if (source.Loops)
            {
                Gizmos.DrawLine(lastPos, firstPos);
            }
        }
#endif
    }
}