// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.SpatialUnderstanding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.Examples.SpatialUnderstanding
{
    public class LineDrawer : MonoBehaviour
    {
        // Consts
        public const float DefaultLineWidth = 0.001f;
        public const float DefaultBasisLength = 0.2f;

        // Structs
        public class Line
        {
            // Functions
            public Line()
            {
            }
            public Line(Vector3 _p0, Vector3 _p1, Color _c0, Color _c1, float _lineWidth = DefaultLineWidth)
            {
                p0 = _p0;
                p1 = _p1;
                c0 = _c0;
                c1 = _c1;
                lineWidth = _lineWidth;
                isValid = true;
            }
            public bool Set_IfDifferent(Vector3 _p0, Vector3 _p1, Color _c0, Color _c1, float _lineWidth)
            {
                isValid = true;
                if ((p0 != _p0) || (p1 != _p1) || (c0 != _c0) || (c1 != _c1) || (lineWidth != _lineWidth))
                {
                    p0 = _p0;
                    p1 = _p1;
                    c0 = _c0;
                    c1 = _c1;
                    lineWidth = _lineWidth;
                    return true;
                }
                return false;
            }

            // Data
            public Vector3 p0;
            public Vector3 p1;
            public Color c0;
            public Color c1;
            public float lineWidth;
            public bool isValid;
        }
        public class LineData
        {
            public int LineIndex;
            public List<Line> Lines = new List<Line>();
            public MeshRenderer Renderer;
            public MeshFilter Filter;
        }
        public class AnimationCurve3
        {
            public void AddKey(float time, Vector3 pos)
            {
                CurveX.AddKey(time, pos.x);
                CurveY.AddKey(time, pos.y);
                CurveZ.AddKey(time, pos.z);
            }
            public Vector3 Evaluate(float time)
            {
                return new Vector3(CurveX.Evaluate(time), CurveY.Evaluate(time), CurveZ.Evaluate(time));
            }

            public AnimationCurve CurveX = new AnimationCurve();
            public AnimationCurve CurveY = new AnimationCurve();
            public AnimationCurve CurveZ = new AnimationCurve();
        }
        public class AnimatedBox
        {
            public const float InitialPositionForwardMaxDistance = 2.0f;
            public const float AnimationTime = 2.5f;
            public const float DelayPerItem = 0.35f;

            public AnimatedBox(
                float timeDelay,
                Vector3 center,
                Quaternion rotation,
                Color color,
                Vector3 halfSize,
                float lineWidth = DefaultLineWidth * 3.0f)
            {
                TimeDelay = timeDelay;
                Center = center;
                Rotation = rotation;
                Color = color;
                HalfSize = halfSize;
                LineWidth = lineWidth;

                // If no time delay, go ahead and lock the animation now
                if (TimeDelay <= 0.0f)
                {
                    SetupAnimation();
                }
            }

            public bool Update(float deltaTime)
            {
                Time += deltaTime;

                // Delay animation setup until after the time delay
                if (!IsAnimationSetup &&
                    (Time >= TimeDelay))
                {
                    SetupAnimation();
                }

                return (Time >= TimeDelay);
            }

            private void SetupAnimation()
            {
                if (!SpatialUnderstandingManager.Instance.AllowSpatialUnderstanding)
                {
                    return;
                }

                // Calculate the forward distance for the animation start point
                Vector3 rayPos = CameraCache.Main.transform.position;
                Vector3 rayVec = CameraCache.Main.transform.forward * InitialPositionForwardMaxDistance;
                IntPtr raycastResultPtr = SpatialUnderstandingManager.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();
                SpatialUnderstandingDll.Imports.PlayspaceRaycast(
                    rayPos.x, rayPos.y, rayPos.z, rayVec.x, rayVec.y, rayVec.z,
                    raycastResultPtr);
                SpatialUnderstandingDll.Imports.RaycastResult rayCastResult = SpatialUnderstandingManager.Instance.UnderstandingDLL.GetStaticRaycastResult();
                Vector3 animOrigin = (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid) ?
                    rayPos + rayVec.normalized * Mathf.Max((rayCastResult.IntersectPoint - rayPos).magnitude - 0.3f, 0.0f) :
                    rayPos + rayVec * InitialPositionForwardMaxDistance;

                // Create the animation (starting it on the ground in front of the camera
                SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstandingManager.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
                SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstandingManager.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();
                AnimPosition.AddKey(TimeDelay + 0.0f, new Vector3(animOrigin.x, alignment.FloorYValue, animOrigin.z));
                AnimPosition.AddKey(TimeDelay + AnimationTime * 0.5f, new Vector3(animOrigin.x, alignment.FloorYValue + 1.25f, animOrigin.z));
                AnimPosition.AddKey(TimeDelay + AnimationTime * 0.6f, new Vector3(animOrigin.x, alignment.FloorYValue + 1.0f, animOrigin.z));
                AnimPosition.AddKey(TimeDelay + AnimationTime * 0.95f, Center);
                AnimPosition.AddKey(TimeDelay + AnimationTime * 1.0f, Center);

                AnimScale.AddKey(TimeDelay + 0.0f, 0.0f);
                AnimScale.AddKey(TimeDelay + AnimationTime * 0.5f, 0.5f);
                AnimScale.AddKey(TimeDelay + AnimationTime * 0.8f, 1.0f);
                AnimScale.AddKey(TimeDelay + AnimationTime * 1.0f, 1.0f);

                AnimRotation.AddKey(TimeDelay + 0.0f, -1.5f);
                AnimRotation.AddKey(TimeDelay + AnimationTime * 0.2f, -0.5f);
                AnimRotation.AddKey(TimeDelay + AnimationTime * 0.9f, 0.0f);
                AnimRotation.AddKey(TimeDelay + AnimationTime * 1.0f, 0.0f);

                IsAnimationSetup = true;
            }
            public bool IsAnimationComplete { get { return IsAnimationSetup && (Time >= (AnimatedBox.AnimationTime + TimeDelay)); } }

            public Vector3 Center;
            public Quaternion Rotation;
            public Color Color;
            public Vector3 HalfSize;
            public float LineWidth;

            public bool IsAnimationSetup;
            public float Time;
            public float TimeDelay;
            public AnimationCurve AnimScale = new AnimationCurve();
            public AnimationCurve3 AnimPosition = new AnimationCurve3();
            public AnimationCurve AnimRotation = new AnimationCurve();

        }

        // Config
        public Material MaterialLine;

        // Privates
        private LineData lineData = new LineData();

        // Functions
        protected virtual void OnDestroy()
        {
            // Line renderer
            if (lineData != null)
            {
                if (lineData.Renderer != null)
                {
                    Destroy(lineData.Renderer);
                }
                if (lineData.Filter != null)
                {
                    Destroy(lineData.Filter);
                }
            }
            lineData = null;
        }

        protected const int PointsOnCircle = 50;
        protected bool Draw_Circle(Vector3 center, Vector3 normal, Color color, float radius = 0.25f, float lineWidth = DefaultLineWidth)
        {
            bool returnValue = false;
            float theta = 0;
            float radPerPoint = (2.0f * Mathf.PI) / (float)PointsOnCircle;
            Quaternion q = Quaternion.FromToRotation(Vector3.up, normal);
            Vector3 start = q * new Vector3(Mathf.Cos(theta) * radius, 0.0f, Mathf.Sin(theta) * radius) + center;

            for (int i = 1; i <= PointsOnCircle; i++)
            {
                theta = (i % PointsOnCircle) * radPerPoint;

                Vector3 end = q * new Vector3(Mathf.Cos(theta) * radius, 0.0f, Mathf.Sin(theta) * radius) + center;
                returnValue |= Draw_Line(start, end, color, color, lineWidth);

                start = end;
            }

            return returnValue;
        }

        protected bool Draw_Circle_Partial(Vector3 center, Vector3 normal, Color color, float radius = 0.25f, float lineWidth = DefaultLineWidth, float circleAngleArc = 360.0f)
        {
            bool returnValue = false;
            float theta = 0;
            float radPerPoint = (circleAngleArc * Mathf.Deg2Rad * 0.5f) / (float)PointsOnCircle;
            Quaternion q = Quaternion.FromToRotation(Vector3.up, normal);
            Vector3 start0 = q * new Vector3(Mathf.Cos(theta) * radius, 0.0f, Mathf.Sin(theta) * radius) + center;
            Vector3 start1 = q * new Vector3(Mathf.Cos(theta) * -radius, 0.0f, Mathf.Sin(theta) * -radius) + center;

            int maxPointCount = (circleAngleArc < 360.0f) ? (PointsOnCircle - 1) : PointsOnCircle;
            for (int i = 1; i <= maxPointCount; i++)
            {
                theta = (i % PointsOnCircle) * radPerPoint;

                Vector3 end0 = q * new Vector3(Mathf.Cos(theta) * radius, 0.0f, Mathf.Sin(theta) * radius) + center;
                Vector3 end1 = q * new Vector3(Mathf.Cos(theta) * -radius, 0.0f, Mathf.Sin(theta) * -radius) + center;
                returnValue |= Draw_Line(start0, end0, color, color, lineWidth);
                returnValue |= Draw_Line(start1, end1, color, color, lineWidth);

                start0 = end0;
                start1 = end1;
            }

            return returnValue;
        }

        protected bool Draw_Cube(Vector3 point, Color color, float halfSize = DefaultLineWidth)
        {
            return Draw_Line(point - Vector3.right * halfSize, point + Vector3.right * halfSize, color, color, halfSize);
        }

        protected bool Draw_AnimatedBox(AnimatedBox box)
        {
            // Update the time
            if (!box.Update(Time.deltaTime))
            {
                return false;
            }
            if (box.IsAnimationComplete)
            {
                // Animation is done, just pass through
                return Draw_Box(box.Center, box.Rotation, box.Color, box.HalfSize, box.LineWidth);
            }

            // Draw it using the current animation state
            return Draw_Box(
                box.AnimPosition.Evaluate(box.Time),
                box.Rotation * Quaternion.AngleAxis(360.0f * box.AnimRotation.Evaluate(box.Time), Vector3.up),
                box.Color,
                box.HalfSize * box.AnimScale.Evaluate(box.Time),
                box.LineWidth);
        }

        protected bool Draw_Box(Vector3 center, Quaternion rotation, Color color, Vector3 halfSize, float lineWidth = DefaultLineWidth)
        {
            bool needsUpdate = false;

            Vector3 basisX = rotation * Vector3.right;
            Vector3 basisY = rotation * Vector3.up;
            Vector3 basisZ = rotation * Vector3.forward;
            Vector3[] pts =
            {
            center + basisX * halfSize.x + basisY * halfSize.y + basisZ * halfSize.z,
            center + basisX * halfSize.x + basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x + basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x + basisY * halfSize.y + basisZ * halfSize.z,

            center + basisX * halfSize.x - basisY * halfSize.y + basisZ * halfSize.z,
            center + basisX * halfSize.x - basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x - basisY * halfSize.y - basisZ * halfSize.z,
            center - basisX * halfSize.x - basisY * halfSize.y + basisZ * halfSize.z
        };

            // Bottom
            needsUpdate |= Draw_Line(pts[0], pts[1], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[1], pts[2], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[2], pts[3], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[3], pts[0], color, color, lineWidth);

            // Top
            needsUpdate |= Draw_Line(pts[4], pts[5], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[5], pts[6], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[6], pts[7], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[7], pts[4], color, color, lineWidth);

            // Vertical lines
            needsUpdate |= Draw_Line(pts[0], pts[4], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[1], pts[5], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[2], pts[6], color, color, lineWidth);
            needsUpdate |= Draw_Line(pts[3], pts[7], color, color, lineWidth);

            return needsUpdate;
        }

        protected bool Draw_Line(Vector3 start, Vector3 end, Color colorStart, Color colorEnd, float lineWidth = DefaultLineWidth)
        {
            // Create up a new line (unless it's already created)
            while (lineData.LineIndex >= lineData.Lines.Count)
            {
                lineData.Lines.Add(new Line());
            }

            // Set it
            bool needsUpdate = lineData.Lines[lineData.LineIndex].Set_IfDifferent(transform.InverseTransformPoint(start), transform.InverseTransformPoint(end), colorStart, colorEnd, lineWidth);

            // Inc out count
            ++lineData.LineIndex;

            return needsUpdate;
        }

        protected bool Draw_TransformBasis(Transform transformToDraw, float basisLength = DefaultBasisLength, float lineWidth = DefaultLineWidth * 2.0f)
        {
            // Basis
            bool needsUpdate = false;
            needsUpdate |= Draw_Line(transformToDraw.transform.position, transformToDraw.transform.position + transformToDraw.transform.right * basisLength, Color.red, Color.red, lineWidth);
            needsUpdate |= Draw_Line(transformToDraw.transform.position, transformToDraw.transform.position + transformToDraw.transform.up * basisLength, Color.green, Color.green, lineWidth);
            needsUpdate |= Draw_Line(transformToDraw.transform.position, transformToDraw.transform.position + transformToDraw.transform.forward * basisLength, Color.blue, Color.blue, lineWidth);

            return needsUpdate;
        }

        private void Lines_LineDataToMesh()
        {
            // Allocate them up
            Vector3[] verts = new Vector3[lineData.Lines.Count * 8];
            int[] tris = new int[lineData.Lines.Count * 12 * 3];
            Color[] colors = new Color[verts.Length];

            // Build the data
            for (int i = 0; i < lineData.Lines.Count; ++i)
            {
                // Base index calculations
                int vert = i * 8;
                int v0 = vert;
                int tri = i * 12 * 3;

                // Setup
                Vector3 dirUnit = (lineData.Lines[i].p1 - lineData.Lines[i].p0).normalized;
                Vector3 normX = Vector3.Cross((Mathf.Abs(dirUnit.y) >= 0.99f) ? Vector3.right : Vector3.up, dirUnit).normalized;
                Vector3 normy = Vector3.Cross(normX, dirUnit);

                // Vertices
                verts[vert] = lineData.Lines[i].p0 + normX * lineData.Lines[i].lineWidth + normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c0; ++vert;
                verts[vert] = lineData.Lines[i].p0 - normX * lineData.Lines[i].lineWidth + normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c0; ++vert;
                verts[vert] = lineData.Lines[i].p0 - normX * lineData.Lines[i].lineWidth - normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c0; ++vert;
                verts[vert] = lineData.Lines[i].p0 + normX * lineData.Lines[i].lineWidth - normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c0; ++vert;

                verts[vert] = lineData.Lines[i].p1 + normX * lineData.Lines[i].lineWidth + normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c1; ++vert;
                verts[vert] = lineData.Lines[i].p1 - normX * lineData.Lines[i].lineWidth + normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c1; ++vert;
                verts[vert] = lineData.Lines[i].p1 - normX * lineData.Lines[i].lineWidth - normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c1; ++vert;
                verts[vert] = lineData.Lines[i].p1 + normX * lineData.Lines[i].lineWidth - normy * lineData.Lines[i].lineWidth; colors[vert] = lineData.Lines[i].c1; ++vert;

                // Indices
                tris[tri + 0] = (v0 + 0); tris[tri + 1] = (v0 + 5); tris[tri + 2] = (v0 + 4); tri += 3;
                tris[tri + 0] = (v0 + 1); tris[tri + 1] = (v0 + 5); tris[tri + 2] = (v0 + 0); tri += 3;

                tris[tri + 0] = (v0 + 1); tris[tri + 1] = (v0 + 6); tris[tri + 2] = (v0 + 5); tri += 3;
                tris[tri + 0] = (v0 + 2); tris[tri + 1] = (v0 + 6); tris[tri + 2] = (v0 + 1); tri += 3;

                tris[tri + 0] = (v0 + 2); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 6); tri += 3;
                tris[tri + 0] = (v0 + 3); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 2); tri += 3;

                tris[tri + 0] = (v0 + 3); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 4); tri += 3;
                tris[tri + 0] = (v0 + 3); tris[tri + 1] = (v0 + 4); tris[tri + 2] = (v0 + 0); tri += 3;

                tris[tri + 0] = (v0 + 0); tris[tri + 1] = (v0 + 3); tris[tri + 2] = (v0 + 2); tri += 3;
                tris[tri + 0] = (v0 + 0); tris[tri + 1] = (v0 + 2); tris[tri + 2] = (v0 + 1); tri += 3;

                tris[tri + 0] = (v0 + 5); tris[tri + 1] = (v0 + 6); tris[tri + 2] = (v0 + 7); tri += 3;
                tris[tri + 0] = (v0 + 5); tris[tri + 1] = (v0 + 7); tris[tri + 2] = (v0 + 4); tri += 3;
            }

            // Create up the components
            if (lineData.Filter == null)
            {
                lineData.Filter = gameObject.AddComponent<MeshFilter>();
            }
            if (lineData.Renderer == null)
            {
                lineData.Renderer = gameObject.AddComponent<MeshRenderer>();
                lineData.Renderer.material = MaterialLine;
            }

            // Create or clear the mesh
            Mesh mesh = null;
            if (lineData.Filter.mesh != null)
            {
                mesh = lineData.Filter.mesh;
                mesh.Clear();
            }
            else
            {
                mesh = new Mesh
                {
                    name = "LineDrawer.Lines_LineDataToMesh"
                };
            }

            // Set them into the mesh
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.colors = colors;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            lineData.Filter.mesh = mesh;

            // If no triangles, hide it
            lineData.Renderer.enabled = (lineData.Lines.Count == 0) ? false : true;

            // Line index reset
            lineData.LineIndex = 0;
        }

        protected void LineDraw_Begin()
        {
            lineData.LineIndex = 0;
            for (int i = 0; i < lineData.Lines.Count; ++i)
            {
                lineData.Lines[i].isValid = false;
            }
        }

        protected void LineDraw_End(bool needsUpdate)
        {
            if (lineData == null)
            {
                return;
            }

            // Check if we have any not dirty
            int i = 0;
            while (i < lineData.Lines.Count)
            {
                if (!lineData.Lines[i].isValid)
                {
                    needsUpdate = true;
                    lineData.Lines.RemoveAt(i);
                    continue;
                }
                ++i;
            }

            // Do the update (if needed)
            if (needsUpdate)
            {
                Lines_LineDataToMesh();
            }
        }
    }
}