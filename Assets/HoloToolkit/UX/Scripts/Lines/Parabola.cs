// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class Parabola : LineBase
    {
        [Header("Parabola Settings")]
        public Vector3 Start = Vector3.zero;
        public Vector3 End = Vector3.forward;
        public Vector3 UpDirection = Vector3.up;
        [Range(0.01f, 10f)]
        public float Height = 1f;

        public override int NumPoints
        {
            get
            {
                return 2;
            }
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return Start;

                case 1:
                    return End;

                default:
                    return Vector3.zero;
            }
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    Start = point;
                    break;

                case 1:
                    End = point;
                    break;

                default:
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtils.GetPointAlongParabola(Start, End, UpDirection, Height, normalizedDistance);
        }

        protected override float GetUnclampedWorldLengthInternal()
        {
            // Crude approximation
            // TODO optimize
            float distance = 0f;
            Vector3 last = GetUnclampedPoint(0f);
            for (int i = 1; i < 10; i++)
            {
                Vector3 current = GetUnclampedPoint((float)i / 10);
                distance += Vector3.Distance(last, current);
            }
            return distance;
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return transform.up;
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Parabola))]
        public class CustomEditor : LineBaseEditor
        {
            protected override void DrawCustomSceneGUI()
            {
                base.DrawCustomSceneGUI();

                Parabola line = (Parabola)target;

                line.FirstPoint = SquareMoveHandle(line.FirstPoint);
                line.LastPoint = SquareMoveHandle(line.LastPoint);
                // Draw a handle for the parabola height
                line.Height = AxisMoveHandle(line.FirstPoint, line.transform.up, line.Height);
            }
        }
#endif
    }
}