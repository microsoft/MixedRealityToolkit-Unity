// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    public abstract class LineRendererBase : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The source Line this component will render")]
        protected LineBase source;

        [Header("Visual Settings")]
        [Tooltip("Color gradient applied to line's normalized length")]
        public Gradient LineColor;

        public AnimationCurve LineWidth = AnimationCurve.Linear(0f, 0.05f, 1f, 0.05f);
        [Range(0f, 10f)]
        public float WidthMultiplier = 0.25f;

        [Header("Offsets")]
        [Range(0f, 10f)]
        [Tooltip("Normalized offset for color gradient")]
        public float ColorOffset = 0f;
        [Range(0f, 10f)]
        [Tooltip("Normalized offset for width curve")]
        public float WidthOffset = 0f;
        [Range(0f, 10f)]
        [Tooltip("Normalized offset for rotation offset")]
        public float RotationOffset = 0f;

        [Header("Point Placement")]
        [Tooltip("Method for gathering points along line. Interpolated uses normalized length. FromSource uses line's base points. (FromSource may not look right for all Line types.)")]
        public StepModeEnum StepMode = StepModeEnum.Interpolated;
        [Range(0, 2048)]
        [Tooltip("Number of steps to interpolate along line in Interpolated step mode")]
        [ShowIfEnumValue("StepMode", StepModeEnum.Interpolated)]
        public int NumLineSteps = 10;

        [FeatureInProgress]
        public InterpolationModeEnum InterpolationMode = InterpolationModeEnum.FromLength;
        [FeatureInProgress]
        [Range(0.001f, 1f)]
        public float StepLength = 0.05f;
        [FeatureInProgress]
        [Range(1, 2048)]
        public int MaxLineSteps = 2048;
        [FeatureInProgress]
        public AnimationCurve StepLengthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.5f);

        public virtual LineBase Source
        {
            get
            {
                if (source == null)
                {
                    source = GetComponent<LineBase>();
                }
                return source;
            }
            set
            {
                source = value;
                if (source == null)
                {
                    enabled = false;
                }
            }
        }

        protected virtual Color GetColor(float normalizedLength)
        {
            if (LineColor == null)
            {
                LineColor = new Gradient();
            }

            return LineColor.Evaluate(Mathf.Repeat(normalizedLength + ColorOffset, 1f));
        }

        protected virtual float GetWidth(float normalizedLength)
        {
            if (LineWidth == null)
            {
                LineWidth = AnimationCurve.Linear(0f, 1f, 1f, 1f);
            }

            return LineWidth.Evaluate(Mathf.Repeat(normalizedLength + WidthOffset, 1f)) * WidthMultiplier;
        }

        private float[] normalizedLengths;

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            if (source == null)
                source = gameObject.GetComponent<LineBase>();
            if (source == null || !source.enabled)
                return;

            GizmosDrawLineRenderer(source, this);
        }

        public static void GizmosDrawLineRenderer(LineBase source, LineRendererBase renderer)
        {
            switch (renderer.StepMode)
            {
                case StepModeEnum.FromSource:
                    GizmosDrawLineFromSource(source, renderer);
                    break;

                case StepModeEnum.Interpolated:
                    GizmosDrawLineInterpolated(source, renderer);
                    break;
            }
        }

        public static void GizmosDrawLineFromSource(LineBase source, LineRendererBase renderer)
        {
            Vector3 firstPos = source.GetPoint(0);
            Vector3 lastPos = firstPos;
            Color gColor = renderer.GetColor(0);

            gColor.a = 0.5f;
            Gizmos.color = gColor;
            Gizmos.DrawSphere(firstPos, renderer.GetWidth(0) / 2);
            for (int i = 1; i < source.NumPoints; i++)
            {
                float normalizedLength = (1f / (source.NumPoints)) * i;
                Vector3 currentPos = source.GetPoint(i);
                gColor = renderer.GetColor(normalizedLength);
                gColor.a = gColor.a * 0.5f;
                Gizmos.color = gColor;
                Gizmos.DrawLine(lastPos, currentPos);
                Gizmos.DrawSphere(currentPos, renderer.GetWidth(normalizedLength) / 2);
                lastPos = currentPos;
            }

            if (source.Loops)
            {
                Gizmos.DrawLine(lastPos, firstPos);
            }
        }

        public static void GizmosDrawLineInterpolated(LineBase source, LineRendererBase renderer)
        {
            Vector3 firstPos = source.GetPoint(0f);
            Vector3 lastPos = firstPos;
            Color gColor = renderer.GetColor(0);

            gColor.a = 0.5f;
            Gizmos.color = gColor;
            Gizmos.DrawSphere(firstPos, renderer.GetWidth(0) / 2);
            for (int i = 1; i <= renderer.NumLineSteps; i++)
            {
                float normalizedLength = (1f / (renderer.NumLineSteps)) * i;
                Vector3 currentPos = source.GetPoint(normalizedLength);
                gColor = renderer.GetColor(normalizedLength);
                gColor.a = gColor.a * 0.5f;
                Gizmos.color = gColor;
                Gizmos.DrawLine(lastPos, currentPos);
                Gizmos.DrawSphere(currentPos, renderer.GetWidth(normalizedLength) / 2);
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