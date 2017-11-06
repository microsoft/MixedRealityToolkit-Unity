// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MRTK.UX
{
    public class Bezeir : LineBase
    {
        [Serializable]
        public struct PointSet
        {
            public PointSet (float spread)
            {
                Point1 = Vector3.right * spread * 0.5f;
                Point2 = Vector3.right * spread * 0.25f;
                Point3 = Vector3.left * spread * 0.25f;
                Point4 = Vector3.left * spread * 0.5f;
            }

            public Vector3 Point1;
            public Vector3 Point2;
            public Vector3 Point3;
            public Vector3 Point4;
        }

        [Header("Bezeir Settings")]
        public PointSet Points = new PointSet(0.5f);

        public override int NumPoints
        {
            get
            {
                return 4;
            }
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return Points.Point1;

                case 1:
                    return Points.Point2;

                case 2:
                    return Points.Point3;

                case 3:
                    return Points.Point4;

                default:
                    return Vector3.zero;
            }
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    Points.Point1 = point;
                    break;

                case 1:
                    Points.Point2 = point;
                    break;

                case 2:
                    Points.Point3 = point;
                    break;

                case 3:
                    Points.Point4 = point;
                    break;

                default:
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtils.InterpolateBezeirPoints(Points.Point1, Points.Point2, Points.Point3, Points.Point4, normalizedDistance);
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
            // Bezeir up vectors just use transform up
            return transform.up;
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Bezeir))]
        public class CustomEditor : LineBaseEditor
        {
            protected override void DrawCustomSceneGUI()
            {
                base.DrawCustomSceneGUI();

                Bezeir line = (Bezeir)target;

                line.SetPoint(0, SphereMoveHandle(line.GetPoint(0)));
                line.SetPoint(1, SquareMoveHandle(line.GetPoint(1)));
                line.SetPoint(2, SquareMoveHandle(line.GetPoint(2)));
                line.SetPoint(3, SphereMoveHandle(line.GetPoint(3)));

                UnityEditor.Handles.color = handleColorTangent;
                UnityEditor.Handles.DrawLine(line.GetPoint(0), line.GetPoint(1));
                UnityEditor.Handles.DrawLine(line.GetPoint(2), line.GetPoint(3));
            }
        }
#endif
    }
}