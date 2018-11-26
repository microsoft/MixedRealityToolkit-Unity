// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class Line : LineBase
    {
        [Header("Line Settings")]
        public Vector3 Start = Vector3.zero;
        public Vector3 End = Vector3.one;

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
                default:
                    return Start;

                case 1:
                    return End;
            }
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                default:
                    Start = point;
                    break;

                case 1:
                    End = point;
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return Vector3.Lerp(Start, End, normalizedDistance);
        }

        protected override float GetUnclampedWorldLengthInternal()
        {
            return Vector3.Distance(Start, End);
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return transform.up;
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Line))]
        public class CustomEditor : LineBaseEditor {
            protected override void DrawCustomSceneGUI()
            {
                base.DrawCustomSceneGUI();

                LineBase line = (LineBase)target;
                line.FirstPoint = SquareMoveHandle(line.FirstPoint);
                line.LastPoint = SquareMoveHandle(line.LastPoint);
            }
        }
#endif
    }
}