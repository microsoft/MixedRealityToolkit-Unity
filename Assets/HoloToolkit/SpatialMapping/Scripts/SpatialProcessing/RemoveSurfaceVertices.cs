// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// RemoveSurfaceVertices will remove any vertices from the Spatial Mapping Mesh that fall within the bounding volume.
    /// This can be used to create holes in the environment, or to help reduce triangle count after finding planes.
    /// </summary>
    public class RemoveSurfaceVertices : Singleton<RemoveSurfaceVertices>
    {
        [Tooltip("The amount, if any, to expand each bounding volume by.")]
        public float BoundsExpansion = 0.0f;

        /// <summary>
        /// Delegate which is called when the RemoveVerticesComplete event is triggered.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public delegate void EventHandler(object source, EventArgs args);

        /// <summary>
        /// EventHandler which is triggered when the RemoveSurfaceVertices is finished.
        /// </summary>
        public event EventHandler RemoveVerticesComplete;

        /// <summary>
        /// Indicates if RemoveSurfaceVertices is currently removing vertices from the Spatial Mapping Mesh.
        /// </summary>
        private bool removingVerts = false;

        /// <summary>
        /// Queue of bounding objects to remove surface vertices from.
        /// Bounding objects are queued so that RemoveSurfaceVerticesWithinBounds can be called even when the previous task has not finished.
        /// </summary>
        private Queue<Bounds> boundingObjectsQueue;

#if UNITY_EDITOR
        /// <summary>
        /// How much time (in sec), while running in the Unity Editor, to allow RemoveSurfaceVertices to consume before returning control to the main program.
        /// </summary>
        private static readonly float FrameTime = .016f;
#else
        /// <summary>
        /// How much time (in sec) to allow RemoveSurfaceVertices to consume before returning control to the main program.
        /// </summary>
        private static readonly float FrameTime = .008f;
#endif

        // GameObject initialization.
        private void Start()
        {
            boundingObjectsQueue = new Queue<Bounds>();
            removingVerts = false;
        }

        /// <summary>
        /// Removes portions of the surface mesh that exist within the bounds of the boundingObjects.
        /// </summary>
        /// <param name="boundingObjects">Collection of GameObjects that define the bounds where spatial mesh vertices should be removed.</param>
        public void RemoveSurfaceVerticesWithinBounds(IEnumerable<GameObject> boundingObjects)
        {
            if (boundingObjects == null)
            {
                return;
            }

            if (!removingVerts)
            {
                removingVerts = true;
                AddBoundingObjectsToQueue(boundingObjects);

                // We use Coroutine to split the work across multiple frames and avoid impacting the frame rate too much.
                StartCoroutine(RemoveSurfaceVerticesWithinBoundsRoutine());
            }
            else
            {
                // Add new boundingObjects to end of queue.
                AddBoundingObjectsToQueue(boundingObjects);
            }
        }

        /// <summary>
        /// Adds new bounding objects to the end of the Queue.
        /// </summary>
        /// <param name="boundingObjects">Collection of GameObjects which define the bounds where spatial mesh vertices should be removed.</param>
        private void AddBoundingObjectsToQueue(IEnumerable<GameObject> boundingObjects)
        {
            foreach (GameObject item in boundingObjects)
            {
                Bounds bounds = new Bounds();

                Collider boundingCollider = item.GetComponent<Collider>();
                if (boundingCollider != null)
                {
                    bounds = boundingCollider.bounds;

                    // Expand the bounds, if requested.
                    if (BoundsExpansion > 0.0f)
                    {
                        bounds.Expand(BoundsExpansion);
                    }

                    boundingObjectsQueue.Enqueue(bounds);
                }
            }
        }

        /// <summary>
        /// Iterator block, analyzes surface meshes to find vertices existing within the bounds of any boundingObject and removes them.
        /// </summary>
        /// <returns>Yield result.</returns>
        private IEnumerator RemoveSurfaceVerticesWithinBoundsRoutine()
        {
            List<MeshFilter> meshFilters = SpatialMappingManager.Instance.GetMeshFilters();
            float start = Time.realtimeSinceStartup;

            while (boundingObjectsQueue.Count > 0)
            {
                // Get the current boundingObject.
                Bounds bounds = boundingObjectsQueue.Dequeue();

                foreach (MeshFilter filter in meshFilters)
                {
                    // Since this is amortized across frames, the filter can be destroyed by the time
                    // we get here.
                    if (filter == null)
                    {
                        continue;
                    }

                    Mesh mesh = filter.sharedMesh;

                    if (mesh != null && !mesh.bounds.Intersects(bounds))
                    {
                        // We don't need to do anything to this mesh, move to the next one.
                        continue;
                    }

                    // Remove vertices from any mesh that intersects with the bounds.
                    Vector3[] verts = mesh.vertices;
                    List<int> vertsToRemove = new List<int>();

                    // Find which mesh vertices are within the bounds.
                    for (int i = 0; i < verts.Length; ++i)
                    {
                        if (bounds.Contains(verts[i]))
                        {
                            // These vertices are within bounds, so mark them for removal.
                            vertsToRemove.Add(i);
                        }

                        // If too much time has passed, we need to return control to the main game loop.
                        if ((Time.realtimeSinceStartup - start) > FrameTime)
                        {
                            // Pause our work here, and continue finding vertices to remove on the next frame.
                            yield return null;
                            start = Time.realtimeSinceStartup;
                        }
                    }

                    if (vertsToRemove.Count == 0)
                    {
                        // We did not find any vertices to remove, so move to the next mesh.
                        continue;
                    }

                    // We found vertices to remove, so now we need to remove any triangles that reference these vertices.
                    int[] indices = mesh.GetTriangles(0);
                    List<int> updatedIndices = new List<int>();

                    for (int index = 0; index < indices.Length; index += 3)
                    {
                        // Each triangle utilizes three slots in the index buffer, check to see if any of the
                        // triangle indices contain a vertex that should be removed.
                        if (vertsToRemove.Contains(indices[index]) ||
                            vertsToRemove.Contains(indices[index + 1]) ||
                            vertsToRemove.Contains(indices[index + 2]))
                        {
                            // Do nothing, we don't want to save this triangle...
                        }
                        else
                        {
                            // Every vertex in this triangle is good, so let's save it.
                            updatedIndices.Add(indices[index]);
                            updatedIndices.Add(indices[index + 1]);
                            updatedIndices.Add(indices[index + 2]);
                        }

                        // If too much time has passed, we need to return control to the main game loop.
                        if ((Time.realtimeSinceStartup - start) > FrameTime)
                        {
                            // Pause our work, and continue making additional planes on the next frame.
                            yield return null;
                            start = Time.realtimeSinceStartup;
                        }
                    }

                    if (indices.Length == updatedIndices.Count)
                    {
                        // None of the verts to remove were being referenced in the triangle list.
                        continue;
                    }

                    // Update mesh to use the new triangles.
                    mesh.SetTriangles(updatedIndices.ToArray(), 0);
                    mesh.RecalculateBounds();
                    yield return null;
                    start = Time.realtimeSinceStartup;

                    // Reset the mesh collider to fit the new mesh.
                    MeshCollider collider = filter.gameObject.GetComponent<MeshCollider>();
                    if (collider != null)
                    {
                        collider.sharedMesh = null;
                        collider.sharedMesh = mesh;
                    }
                }
            }

            Debug.Log("Finished removing vertices.");

            // We are done removing vertices, trigger an event.
            EventHandler handler = RemoveVerticesComplete;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            removingVerts = false;
        }
    }
}