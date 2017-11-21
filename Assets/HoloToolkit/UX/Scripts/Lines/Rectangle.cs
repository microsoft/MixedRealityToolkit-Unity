// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class Rectangle : LineBase
    {
        public override int NumPoints
        {
            get
            {
                return 8;
            }
        }

        public override bool Loops
        {
            get
            {
                // Force to loop
                loops = true;
                return loops;
            }
        }

        [Header("Rectangle Settings")]
        [SerializeField]
        private Vector3[] points;

        [EditableProp]
        public float XSize
        {
            get { return xSize; }
            set
            {
                if (xSize != value)
                {
                    xSize = value;
                    BuildPoints();
                }
            }
        }
        [EditableProp]
        public float YSize
        {
            get { return ySize; }
            set
            {
                if (ySize != value)
                {
                    ySize = value;
                    BuildPoints();
                }
            }
        }
        [EditableProp]
        public float ZOffset
        {
            get
            {
                return zOffset;
            }
            set
            {
                if (zOffset != value)
                {
                    zOffset = value;
                    BuildPoints();
                }
            }
        }

        [SerializeField]
        [HideInMRTKInspector]
        private float xSize = 1f;
        [SerializeField]
        [HideInMRTKInspector]
        private float ySize = 1f;
        [SerializeField]
        [HideInMRTKInspector]
        private float zOffset = 0f;

        /// <summary>
        /// When we get interpolated points we subdivide the square so our sampling has more to work with
        /// </summary>
        /// <param name="normalizedDistance"></param>
        /// <returns></returns>
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            if (points == null || points.Length != 8)
            {
                points = new Vector3[8];
            }

            BuildPoints();

            return LineUtils.InterpolateVectorArray(points, normalizedDistance);
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (points == null || points.Length != 8)
            {
                points = new Vector3[8];
            }

            if (pointIndex <= 7 && pointIndex >= 0)
            {
                points[pointIndex] = point;
            }
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (points == null || points.Length != 8)
            {
                points = new Vector3[8];
            }

            if (pointIndex <= 7 && pointIndex >= 0)
            {
                return points[pointIndex];
            }

            return Vector3.zero;
        }

        protected override float GetUnclampedWorldLengthInternal()
        {
            BuildPoints();

            Vector3 last = points[0];
            float distance = 0f;
            for (int i = 1; i < points.Length; i++)
            {
                distance += Vector3.Distance(last, points[i]);
                last = points[i];
            }
            return distance;
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            // Rectangle 'up' vector always points out from center
            return (GetPoint(normalizedLength) - transform.position).normalized;
        }

        private void BuildPoints()
        {
            Vector3 offset = Vector3.forward * ZOffset;
            Vector3 top = (Vector3.up * YSize * 0.5f);
            Vector3 bot = (Vector3.down * YSize * 0.5f);
            Vector3 left = (Vector3.left * XSize * 0.5f);
            Vector3 right = (Vector3.right * XSize * 0.5f);

            SetPointInternal(0, top + left + offset);
            SetPointInternal(1, top + offset);
            SetPointInternal(2, top + right + offset);
            SetPointInternal(3, right + offset);
            SetPointInternal(4, bot + right + offset);
            SetPointInternal(5, bot + offset);
            SetPointInternal(6, bot + left + offset);
            SetPointInternal(7, left + offset);
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Rectangle))]
        public class CustomEditor : LineBaseEditor
        {
            // Use FromSource step mode for rectangles since interpolated looks weird
            protected override StepModeEnum EditorStepMode { get { return StepModeEnum.FromSource; } }
        }

        protected override void OnDrawGizmos()
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
            LineRendererBase lr = gameObject.GetComponent<LineRendererBase>();
            if (lr != null)
            {
                return;
            }

            Vector3 firstPos = GetPoint(0);
            Vector3 lastPos = firstPos;
            Gizmos.color = Color.Lerp(LineBaseEditor.DefaultDisplayLineColor, Color.clear, 0.25f);

            for (int i = 1; i < NumPoints; i++)
            {
                Vector3 currentPos = GetPoint(i);
                Gizmos.DrawLine(lastPos, currentPos);
                lastPos = currentPos;
            }

            Gizmos.DrawLine(lastPos, firstPos);
        }
#endif
    }
}