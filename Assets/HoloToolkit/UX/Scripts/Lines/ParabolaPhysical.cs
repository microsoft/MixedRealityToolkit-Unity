// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class ParabolaPhysical : Parabola
    {
        [Header("Physical Parabola Settings")]
        public Vector3 Direction = Vector3.forward;
        public float Velocity = 2f;
        public float TimeMultiplier = 1f;
        public bool UseCustomGravity = false;
        public Vector3 Gravity = Vector3.down * 9.8f;

        public override int NumPoints
        {
            get
            {
                return 1;
            }
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return Start;

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

                default:
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtils.GetPointAlongPhysicalParabola(Start, Direction, Velocity, UseCustomGravity ? Gravity : Physics.gravity, normalizedDistance * TimeMultiplier);
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ParabolaPhysical))]
        public class CustomEditor : LineBaseEditor
        {
            protected override void DrawCustomSceneGUI()
            {
                base.DrawCustomSceneGUI();

                ParabolaPhysical line = (ParabolaPhysical)target;

                line.FirstPoint = SquareMoveHandle(line.FirstPoint);
                // Draw a handle for the parabola height
                //line.Height = AxisMoveHandle(line.FirstPoint, line.transform.up, line.Height);
            }
        }
#endif
    }
}