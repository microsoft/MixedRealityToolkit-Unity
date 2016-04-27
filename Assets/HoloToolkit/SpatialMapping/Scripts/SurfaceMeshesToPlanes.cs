// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR
using System.Threading;
using System.Threading.Tasks;
#else
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// SurfaceMeshesToPlanes will find and create planes based on the meshes returned by the SpatialMappingManager's Observer.
    /// </summary>
    public class SurfaceMeshesToPlanes : Singleton<SurfaceMeshesToPlanes>
    {
        [Tooltip("Currently active planes found within the Spatial Mapping Mesh.")]
        public List<GameObject> ActivePlanes;

        [Tooltip("Object used for creating and rendering Surface Planes.")]
        public GameObject SurfacePlanePrefab;

        [Tooltip("Minimum area required for a plane to be created.")]
        public float MinArea = 0.025f;

        /// <summary>
        /// Determines which plane types should be rendered.
        /// </summary>
        [HideInInspector]
        public PlaneTypes drawPlanesMask =
            (PlaneTypes.Wall | PlaneTypes.Floor | PlaneTypes.Ceiling | PlaneTypes.Table);

        /// <summary>
        /// Determines which plane types should be discarded.
        /// Use this when the spatial mapping mesh is a better fit for the surface (ex: round tables).
        /// </summary>
        [HideInInspector]
        public PlaneTypes destroyPlanesMask = PlaneTypes.Unknown;

        /// <summary>
        /// Floor y value, which corresponds to the maximum horizontal area found below the user's head position.
        /// This value is reset by SurfaceMeshesToPlanes when the max floor plane has been found.
        /// </summary>
        public float FloorYPosition { get; private set; }

        /// <summary>
        /// Ceiling y value, which corresponds to the maximum horizontal area found above the user's head position.
        /// This value is reset by SurfaceMeshesToPlanes when the max ceiling plane has been found.
        /// </summary>
        public float CeilingYPosition { get; private set; }

        /// <summary>
        /// Delegate which is called when the MakePlanesCompleted event is triggered.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public delegate void EventHandler(object source, EventArgs args);

        /// <summary>
        /// EventHandler which is triggered when the MakePlanesRoutine is finished.
        /// </summary>
        public event EventHandler MakePlanesComplete;

        /// <summary>
        /// Empty game object used to contain all planes created by the SurfaceToPlanes class.
        /// </summary>
        private GameObject planesParent;

        /// <summary>
        /// Used to align planes with gravity so that they appear more level.
        /// </summary>
        private float snapToGravityThreshold = 5.0f;

        /// <summary>
        /// Indicates if SurfaceToPlanes is currently creating planes based on the Spatial Mapping Mesh.
        /// </summary>
        private bool makingPlanes = false;

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
            makingPlanes = false;
            ActivePlanes = new List<GameObject>();
            planesParent = new GameObject("SurfacePlanes");
            planesParent.transform.position = Vector3.zero;
            planesParent.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Creates planes based on meshes gathered by the SpatialMappingManager's SurfaceObserver.
        /// </summary>
        public void MakePlanes()
        {
            if (!makingPlanes)
            {
                makingPlanes = true;
                // Processing the mesh can be expensive...
                // We use Coroutine to split the work across multiple frames and avoid impacting the frame rate too much.
                StartCoroutine(MakePlanesRoutine());
            }
        }

        /// <summary>
        /// Gets all active planes of the specified type(s).
        /// </summary>
        /// <param name="planeTypes">A flag which includes all plane type(s) that should be returned.</param>
        /// <returns>A collection of planes that match the expected type(s).</returns>
        public List<GameObject> GetActivePlanes(PlaneTypes planeTypes)
        {
            List<GameObject> typePlanes = new List<GameObject>();

            foreach (GameObject plane in ActivePlanes)
            {
                SurfacePlane surfacePlane = plane.GetComponent<SurfacePlane>();

                if (surfacePlane != null)
                {
                    if ((planeTypes & surfacePlane.PlaneType) == surfacePlane.PlaneType)
                    {
                        typePlanes.Add(plane);
                    }
                }
            }

            return typePlanes;
        }

        /// <summary>
        /// Iterator block, analyzes surface meshes to find planes and create new 3D cubes to represent each plane.
        /// </summary>
        /// <returns>Yield result.</returns>
        private IEnumerator MakePlanesRoutine()
        {
            // Remove any previously existing planes, as they may no longer be valid.
            for (int index = 0; index < ActivePlanes.Count; index++)
            {
                Destroy(ActivePlanes[index]);
            }

            // Pause our work, and continue on the next frame.
            yield return null;
            float start = Time.realtimeSinceStartup;

            ActivePlanes.Clear();

            // Get the latest Mesh data from the Spatial Mapping Manager.
            List<PlaneFinding.MeshData> meshData = new List<PlaneFinding.MeshData>();
            List<MeshFilter> filters = SpatialMappingManager.Instance.GetMeshFilters();

            for (int index = 0; index < filters.Count; index++)
            {
                MeshFilter filter = filters[index];
                if (filter != null && filter.sharedMesh != null)
                {
                    // fix surface mesh normals so we can get correct plane orientation.
                    filter.mesh.RecalculateNormals();
                    meshData.Add(new PlaneFinding.MeshData(filter));
                }

                if ((Time.realtimeSinceStartup - start) > FrameTime)
                {
                    // Pause our work, and continue to make more PlaneFinding objects on the next frame.
                    yield return null;
                    start = Time.realtimeSinceStartup;
                }
            }

            // Pause our work, and continue on the next frame.
            yield return null;

#if !UNITY_EDITOR
            // When not in the unity editor we can use a cool background task to help manage FindPlanes().
            Task<BoundedPlane[]> planeTask = Task.Run(() => PlaneFinding.FindPlanes(meshData, snapToGravityThreshold, MinArea));
        
            while (planeTask.IsCompleted == false)
            {
                yield return null;
            }

            BoundedPlane[] planes = planeTask.Result;
#else
            // In the unity editor, the task class isn't available, but perf is usually good, so we'll just wait for FindPlanes to complete.
            BoundedPlane[] planes = PlaneFinding.FindPlanes(meshData, snapToGravityThreshold, MinArea);
#endif

            // Pause our work here, and continue on the next frame.
            yield return null;
            start = Time.realtimeSinceStartup;

            float maxFloorArea = 0.0f;
            float maxCeilingArea = 0.0f;
            FloorYPosition = 0.0f;
            CeilingYPosition = 0.0f;
            float upNormalThreshold = 0.9f;

            if (SurfacePlanePrefab != null && SurfacePlanePrefab.GetComponent<SurfacePlane>() != null)
            {
                upNormalThreshold = SurfacePlanePrefab.GetComponent<SurfacePlane>().UpNormalThreshold;
            }

            // Find the floor and ceiling.
            // We classify the floor as the maximum horizontal surface below the user's head.
            // We classify the ceiling as the maximum horizontal surface above the user's head.
            for (int i = 0; i < planes.Length; i++)
            {
                BoundedPlane boundedPlane = planes[i];
                if (boundedPlane.Bounds.Center.y < 0 && boundedPlane.Plane.normal.y >= upNormalThreshold)
                {
                    maxFloorArea = Mathf.Max(maxFloorArea, boundedPlane.Area);
                    if (maxFloorArea == boundedPlane.Area)
                    {
                        FloorYPosition = boundedPlane.Bounds.Center.y;
                    }
                }
                else if (boundedPlane.Bounds.Center.y > 0 && boundedPlane.Plane.normal.y <= -(upNormalThreshold))
                {
                    maxCeilingArea = Mathf.Max(maxCeilingArea, boundedPlane.Area);
                    if (maxCeilingArea == boundedPlane.Area)
                    {
                        CeilingYPosition = boundedPlane.Bounds.Center.y;
                    }
                }
            }

            // Create SurfacePlane objects to represent each plane found in the Spatial Mapping mesh.
            for (int index = 0; index < planes.Length; index++)
            {
                GameObject destPlane;
                BoundedPlane boundedPlane = planes[index];

                // Instantiate a SurfacePlane object, which will have the same bounds as our BoundedPlane object.
                if (SurfacePlanePrefab != null && SurfacePlanePrefab.GetComponent<SurfacePlane>() != null)
                {
                    destPlane = Instantiate(SurfacePlanePrefab);
                }
                else
                {
                    destPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    destPlane.AddComponent<SurfacePlane>();
                    destPlane.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                destPlane.transform.parent = planesParent.transform;
                SurfacePlane surfacePlane = destPlane.GetComponent<SurfacePlane>();

                // Set the Plane property to adjust transform position/scale/rotation and determine plane type.
                surfacePlane.Plane = boundedPlane;

                SetPlaneVisibility(surfacePlane);

                if ((destroyPlanesMask & surfacePlane.PlaneType) == surfacePlane.PlaneType)
                {
                    DestroyImmediate(destPlane);
                }
                else
                {
                    // Set the plane to use the same layer as the SpatialMapping mesh.
                    destPlane.layer = SpatialMappingManager.Instance.PhysicsLayer;
                    ActivePlanes.Add(destPlane);
                }

                // If too much time has passed, we need to return control to the main game loop.
                if ((Time.realtimeSinceStartup - start) > FrameTime)
                {
                    // Pause our work here, and continue making additional planes on the next frame.
                    yield return null;
                    start = Time.realtimeSinceStartup;
                }
            }

            Debug.Log("Finished making planes.");

            // We are done creating planes, trigger an event.
            EventHandler handler = MakePlanesComplete;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            makingPlanes = false;
        }

        /// <summary>
        /// Sets visibility of planes based on their type.
        /// </summary>
        /// <param name="surfacePlane"></param>
        private void SetPlaneVisibility(SurfacePlane surfacePlane)
        {
            surfacePlane.IsVisible = ((drawPlanesMask & surfacePlane.PlaneType) == surfacePlane.PlaneType);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor extension class to enable multi-selection of the 'Draw Planes' and 'Destroy Planes' options in the Inspector.
    /// </summary>
    [CustomEditor(typeof(SurfaceMeshesToPlanes))]
    public class PlaneTypesEnumEditor : Editor
    {
        public SerializedProperty drawPlanesMask;
        public SerializedProperty destroyPlanesMask;

        void OnEnable()
        {
            drawPlanesMask = serializedObject.FindProperty("drawPlanesMask");
            destroyPlanesMask = serializedObject.FindProperty("destroyPlanesMask");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            drawPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField
                    ("Draw Planes", (PlaneTypes)drawPlanesMask.intValue));

            destroyPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField
                    ("Destroy Planes", (PlaneTypes)destroyPlanesMask.intValue));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}