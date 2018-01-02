// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;

namespace HoloToolkit.Unity.SpatialMapping.Tests
{
    /// <summary>
    /// Attach this component to a GameObject that contains some meshes (i.e.: the FakeSpatialMappingMesh.fbx).
    /// When running in the Unity editor, the planes are then visualized via editor gizmos.  You can then
    /// play with the API parameters in real-time to see how the impact the plane finding algorithm.
    /// </summary>
    public class PlaneFindingTest : MonoBehaviour
    {
        [Range(0, 45)]
        public float SnapToGravityThreshold = 0.0f;

        [Range(0, 10)]
        public float MinArea = 1.0f;

        public bool VisualizeSubPlanes = false;

        private List<PlaneFinding.MeshData> meshData = new List<PlaneFinding.MeshData>();
        private BoundedPlane[] planes;

        private void Update()
        {
            // Grab the necessary mesh data from the current set of surfaces that we want to run
            // PlaneFinding against.  This must be done on the main UI thread.
            meshData.Clear();
            foreach (MeshFilter mesh in GetComponentsInChildren<MeshFilter>())
            {
                meshData.Add(new PlaneFinding.MeshData(mesh));
            }

            // Now call FindPlanes().  NOTE: In a real application, this MUST be executed on a
            // background thread (i.e.: via ThreadPool.QueueUserWorkItem) so that it doesn't stall the
            // rendering thread while running plane finding.  Maintaining a solid 60fps is crucial
            // to a good user experience.
            planes = (VisualizeSubPlanes) ?
                PlaneFinding.FindSubPlanes(meshData, SnapToGravityThreshold) :
                PlaneFinding.FindPlanes(meshData, SnapToGravityThreshold, MinArea);
        }

        private static Color[] colors = new Color[] { Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
        private void OnDrawGizmos()
        {
            if (planes != null)
            {
                for (int i = 0; i < planes.Length; ++i)
                {
                    Vector3 center = planes[i].Bounds.Center;
                    Quaternion rotation = planes[i].Bounds.Rotation;
                    Vector3 extents = planes[i].Bounds.Extents;
                    Vector3 normal = planes[i].Plane.normal;
                    center -= planes[i].Plane.GetDistanceToPoint(center) * normal;

                    Vector3[] corners = new Vector3[4] {
                    center + rotation * new Vector3(+extents.x, +extents.y, 0),
                    center + rotation * new Vector3(-extents.x, +extents.y, 0),
                    center + rotation * new Vector3(-extents.x, -extents.y, 0),
                    center + rotation * new Vector3(+extents.x, -extents.y, 0)
                };

                    Color color = colors[i % colors.Length];

                    Gizmos.color = color;
                    Gizmos.DrawLine(corners[0], corners[1]);
                    Gizmos.DrawLine(corners[0], corners[2]);
                    Gizmos.DrawLine(corners[0], corners[3]);
                    Gizmos.DrawLine(corners[1], corners[2]);
                    Gizmos.DrawLine(corners[1], corners[3]);
                    Gizmos.DrawLine(corners[2], corners[3]);
                    Gizmos.DrawLine(center, center + normal * 0.4f);
                }
            }
        }

#if UNITY_EDITOR
        // This relies on helper functionality from the UnityEditor.Handles class, so make it UNITY_EDITOR only
        private void OnDrawGizmosSelected()
        {
            if (planes != null)
            {
                Ray cameraForward = new Ray(Camera.current.transform.position, Camera.current.transform.forward);

                // Draw planes
                for (int i = 0; i < planes.Length; ++i)
                {
                    Vector3 center = planes[i].Bounds.Center;
                    Quaternion rotation = planes[i].Bounds.Rotation;
                    Vector3 extents = planes[i].Bounds.Extents;
                    Vector3 normal = planes[i].Plane.normal;
                    center -= planes[i].Plane.GetDistanceToPoint(center) * normal;

                    Vector3[] corners = new Vector3[4] {
                    center + rotation * new Vector3(+extents.x, +extents.y, 0),
                    center + rotation * new Vector3(-extents.x, +extents.y, 0),
                    center + rotation * new Vector3(-extents.x, -extents.y, 0),
                    center + rotation * new Vector3(+extents.x, -extents.y, 0)
                };

                    Color color = colors[i % colors.Length];

                    // Draw the same plane lines using the Handles class which ignores the depth buffer
                    UnityEditor.Handles.color = color;
                    UnityEditor.Handles.DrawLine(corners[0], corners[1]);
                    UnityEditor.Handles.DrawLine(corners[0], corners[2]);
                    UnityEditor.Handles.DrawLine(corners[0], corners[3]);
                    UnityEditor.Handles.DrawLine(corners[1], corners[2]);
                    UnityEditor.Handles.DrawLine(corners[1], corners[3]);
                    UnityEditor.Handles.DrawLine(corners[2], corners[3]);
                    UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.FromToRotation(Vector3.forward, normal), 0.4f, EventType.Ignore);

                    // If this plane is currently in the center of the camera's field of view, highlight it by drawing a
                    // solid rectangle, and display the important details about this plane.
                    float planeHitDistance;
                    if (planes[i].Plane.Raycast(cameraForward, out planeHitDistance))
                    {
                        Vector3 hitPoint = Quaternion.Inverse(rotation) * (cameraForward.GetPoint(planeHitDistance) - center);
                        if (Mathf.Abs(hitPoint.x) <= extents.x && Mathf.Abs(hitPoint.y) <= extents.y)
                        {
                            color.a = 0.1f;
                            UnityEditor.Handles.DrawSolidRectangleWithOutline(corners, color, Color.clear);

                            string text = string.Format("Area: {0} Bounds: {1}\nPlane: N{2}, D({3})",
                                planes[i].Area.ToString("F1"),
                                ((Vector2)extents).ToString("F2"),
                                normal.ToString("F3"),
                                planes[i].Plane.distance.ToString("F3"));

                            UnityEditor.Handles.Label(center, text, GUI.skin.textField);
                        }
                    }
                }
            }
        }
#endif
    }
}
