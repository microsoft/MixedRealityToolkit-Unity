// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#if PLANE_FINDING_PRESENT
using Microsoft.MixedReality.Toolkit.SpatialAwareness.Processing;
#endif

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// SurfaceMeshesToPlanes will find and create planes based on the meshes by a spatial awareness mesh observer.
    /// </summary>
    public class SurfaceMeshesToPlanes : MonoBehaviour
    {
        #region Public properties

        [SerializeField]
        [Tooltip("Empty game object used to contain all planes created by the SurfaceToPlanes class")]
        private GameObject planesParent;

        /// <summary>
        /// Empty game object used to contain all planes created by the SurfaceToPlanes class
        /// </summary>
        public GameObject PlanesParent
        {
            get => planesParent;
            set => planesParent = value;
        }

        [SerializeField]
        [Tooltip("The physics layer for planes to be set to")]
        private int physicsLayer;

        /// <summary>
        /// The physics layer for planes to be set to
        /// </summary>
        public int PhysicsLayer
        {
            get => physicsLayer;
            set => physicsLayer = value;
        }

        [SerializeField]
        [Tooltip("Material used to render planes")]
        private Material defaultMaterial;

        /// <summary>
        /// Material used to render planes
        /// </summary>
        public Material DefaultMaterial
        {
            get => defaultMaterial;
            set => defaultMaterial = value;
        }

        [SerializeField]
        [Tooltip("Minimum area required for a plane to be created.")]
        private float minArea = 0.025f;

        /// <summary>
        /// Minimum area required for a plane to be created.
        /// </summary>
        public float MinArea
        {
            get => minArea;
            set => minArea = value;
        }

        [SerializeField]
        [Tooltip("Threshold for acceptable normals (the closer to 1, the stricter the standard). Used when determining plane type.")]
        [Range(0.0f, 1.0f)]
        private float upNormalThreshold = 0.9f;

        /// <summary>
        /// Threshold for acceptable normals (the closer to 1, the stricter the standard). Used when determining plane type.
        /// </summary>
        public float UpNormalThreshold
        {
            get => upNormalThreshold;
            set => upNormalThreshold = value;
        }

        [SerializeField]
        [Tooltip("Buffer to use when determining if a horizontal plane near the floor should be considered part of the floor.")]
        [Range(0.0f, 1.0f)]
        private float floorBuffer = 0.1f;

        /// <summary>
        /// Buffer to use when determining if a horizontal plane near the floor should be considered part of the floor.
        /// </summary>
        public float FloorBuffer
        {
            get => floorBuffer;
            set => floorBuffer = value;
        }

        [SerializeField]
        [Tooltip("Buffer to use when determining if a horizontal plane near the ceiling should be considered part of the ceiling.")]
        [Range(0.0f, 1.0f)]
        private float ceilingBuffer = 0.1f;

        /// <summary>
        /// Buffer to use when determining if a horizontal plane near the ceiling should be considered part of the ceiling.
        /// </summary>
        public float CeilingBuffer
        {
            get => ceilingBuffer;
            set => ceilingBuffer = value;
        }

        [SerializeField]
        [Tooltip("Thickness of rendered plane objects")]
        [Range(0.0f, 1.0f)]
        private float planeThickness = 0.01f;

        /// <summary>
        /// Thickness of rendered plane objects
        /// </summary>
        public float PlaneThickness
        {
            get => planeThickness;
            set => planeThickness = value;
        }

        [HideInInspector]
        private SpatialAwarenessSurfaceTypes drawPlanesMask =
            (SpatialAwarenessSurfaceTypes.Wall | SpatialAwarenessSurfaceTypes.Floor | SpatialAwarenessSurfaceTypes.Ceiling | SpatialAwarenessSurfaceTypes.Platform);

        /// <summary>
        /// Determines which plane types should be rendered
        /// </summary>
        public SpatialAwarenessSurfaceTypes DrawPlanesMask
        {
            get => drawPlanesMask;
            set => drawPlanesMask = value;
        }

        [HideInInspector]
        private SpatialAwarenessSurfaceTypes destroyPlanesMask = SpatialAwarenessSurfaceTypes.Unknown;

        /// <summary>
        /// Determines which plane types should be discarded.
        /// Use this when the spatial mapping mesh is a better fit for the surface (ex: round tables).
        /// </summary>
        public SpatialAwarenessSurfaceTypes DestroyPlanesMask
        {
            get => destroyPlanesMask;
            set => destroyPlanesMask = value;
        }

#if PLANE_FINDING_PRESENT
        /// <summary>
        /// Delegate which is called when the MakePlanesCompleted event is triggered.
        /// </summary>
        public delegate void EventHandler(object source, EventArgs args);

        /// <summary>
        /// EventHandler which is triggered when the MakePlanesRoutine is finished.
        /// </summary>
        public event EventHandler MakePlanesComplete;
#endif // PLANE_FINDING_PRESENT

        /// <summary>
        /// Indicates whether or not the project contains the required components for SurfaceMeshesToPlanes
        /// to enable plane creation.
        /// </summary>
        /// <remarks>
        /// For SurfaceMeshesToPlanes to create planes, the Mixed Reality Toolkit Plane Finding package
        /// must be imported.
        /// </remarks>
        public static bool CanCreatePlanes =>
#if PLANE_FINDING_PRESENT
            true;
#else
            false;
#endif // PLANE_FINDING_PRESENT

        #endregion

        #region Private Properties

        private List<SpatialAwarenessPlanarObject> activePlanes;
        private bool makingPlanes = false;
        private CancellationTokenSource tokenSource;

#if PLANE_FINDING_PRESENT
        private float floorYPosition;
        private float ceilingYPosition;
        private const float SnapToGravityThreshold = 5.0f;
#endif // PLANE_FINDING_PRESENT

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all active planes of the specified type(s).
        /// </summary>
        /// <param name="planeTypes">A flag which includes all plane type(s) that should be returned.</param>
        /// <returns>A collection of planes that match the expected type(s).</returns>
        public List<GameObject> GetActivePlanes(SpatialAwarenessSurfaceTypes planeTypes)
        {
            List<GameObject> typePlanes = new List<GameObject>();

            foreach (SpatialAwarenessPlanarObject planes in activePlanes)
            {
                if ((planeTypes & planes.SurfaceType) == planes.SurfaceType)
                {
                    typePlanes.Add(planes.GameObject);
                }
            }

            return typePlanes;
        }

        /// <summary>
        /// Runs background task to create new planes based on data from mesh observers
        /// </summary>
        public void MakePlanes()
        {
            if (!makingPlanes)
            {
                makingPlanes = true;
                tokenSource = new CancellationTokenSource();
                _ = MakePlanes(tokenSource.Token).ConfigureAwait(true);
            }
        }

        #endregion

        #region Private Methods

        private void Start()
        {
            activePlanes = new List<SpatialAwarenessPlanarObject>();

            if (planesParent == null)
            {
                planesParent = new GameObject("SurfaceMeshesToPlanes");
            }
            planesParent.transform.position = Vector3.zero;
            planesParent.transform.rotation = Quaternion.identity;
        }

        private void OnDestroy()
        {
            Destroy(PlanesParent);
            tokenSource?.Cancel();
        }

        /// <summary>
        /// Task to analyze surface meshes to find planes and create new 3D cubes to represent each plane.
        /// </summary>
        /// <param name="cancellationToken">Token that allows cancellation of an asynchronous process.</param>
        /// <returns>Yield result.</returns>
        private async Task MakePlanes(CancellationToken cancellationToken)
        {
            await new WaitForUpdate();

#if PLANE_FINDING_PRESENT
            List<PlaneFinding.MeshData> meshData = new List<PlaneFinding.MeshData>();
            List<MeshFilter> filters = new List<MeshFilter>();

            var spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem;
            if (spatialAwarenessSystem != null)
            {
                GameObject parentObject = spatialAwarenessSystem.SpatialAwarenessObjectParent;

                // Get mesh filters from SpatialAwareness Mesh Observer
                foreach (MeshFilter filter in parentObject.GetComponentsInChildren<MeshFilter>())
                {
                    filters.Add(filter);
                }
            }

            for (int index = 0; index < filters.Count; index++)
            {
                MeshFilter filter = filters[index];
                if (filter != null && filter.sharedMesh != null)
                {
                    // fix surface mesh normals so we can get correct plane orientation.
                    filter.mesh.RecalculateNormals();
                    meshData.Add(new PlaneFinding.MeshData(filter));
                }
            }

            BoundedPlane[] planes;

            await new WaitForBackgroundThread();
            {
                planes = PlaneFinding.FindPlanes(meshData, SnapToGravityThreshold, MinArea);
            }

            await new WaitForUpdate();

            DestroyPreviousPlanes();
            activePlanes.Clear();
            ClassifyAndCreatePlanes(planes);

            // We are done creating planes, trigger an event.
            MakePlanesComplete?.Invoke(this, EventArgs.Empty);
#endif // PLANE_FINDING_PRESENT
            makingPlanes = false;
        }

#if PLANE_FINDING_PRESENT
        /// <summary>
        /// Create game objects to represent bounded planes in scene
        /// </summary>
        private void ClassifyAndCreatePlanes(BoundedPlane[] planes)
        {
            SetFloorAndCeilingPositions(planes);

            // Create SurfacePlane objects to represent each plane found in the Spatial Mapping mesh.
            for (int index = 0; index < planes.Length; index++)
            {
                BoundedPlane boundedPlane = planes[index];

                Vector3 size = boundedPlane.Bounds.Extents * 2;
                size.z = PlaneThickness;

                var planeObject = SpatialAwarenessPlanarObject.CreateSpatialObject(
                    boundedPlane.Bounds.Center,
                    size,
                    boundedPlane.Bounds.Rotation,
                    PhysicsLayer,
                    $"SurfacePlane {index}",
                    index,
                    GetPlaneType(boundedPlane));
                
                planeObject.GameObject.transform.parent = PlanesParent.transform;
                if (DefaultMaterial != null)
                {
                    planeObject.Renderer.material = DefaultMaterial;
                }
                SetPlaneVisibility(planeObject);

                if ((destroyPlanesMask & planeObject.SurfaceType) == planeObject.SurfaceType)
                {
                    DestroyImmediate(planeObject.GameObject);
                }
                else
                {
                    activePlanes.Add(planeObject);
                }
            }

            Debug.Log("Finished creating planes.");
        }

        /// <summary>
        /// Create game objects to represent bounded planes in scene
        /// </summary>
        private void SetFloorAndCeilingPositions(BoundedPlane[] planes)
        {
            floorYPosition = 0.0f;
            ceilingYPosition = 0.0f;
            float maxFloorArea = 0.0f;
            float maxCeilingArea = 0.0f;

            // Find the floor and ceiling.
            // We classify the floor as the maximum horizontal surface below the user's head.
            // We classify the ceiling as the maximum horizontal surface above the user's head.
            for (int i = 0; i < planes.Length; i++)
            {
                BoundedPlane boundedPlane = planes[i];
                if (boundedPlane.Bounds.Center.y < 0 && boundedPlane.Plane.normal.y >= UpNormalThreshold)
                {
                    maxFloorArea = Mathf.Max(maxFloorArea, boundedPlane.Area);
                    if (maxFloorArea == boundedPlane.Area)
                    {
                        floorYPosition = boundedPlane.Bounds.Center.y;
                    }
                }
                else if (boundedPlane.Bounds.Center.y > 0 && boundedPlane.Plane.normal.y <= -(UpNormalThreshold))
                {
                    maxCeilingArea = Mathf.Max(maxCeilingArea, boundedPlane.Area);
                    if (maxCeilingArea == boundedPlane.Area)
                    {
                        ceilingYPosition = boundedPlane.Bounds.Center.y;
                    }
                }
            }
        }

        /// <summary>
        /// Classifies the surface as a floor, wall, ceiling, table, etc.
        /// </summary>
        private SpatialAwarenessSurfaceTypes GetPlaneType(BoundedPlane plane)
        {
            SpatialAwarenessSurfaceTypes PlaneType;
            var surfaceNormal = plane.Plane.normal;

            // Determine what type of plane this is.
            // Use the upNormalThreshold to help determine if we have a horizontal or vertical surface.
            if (surfaceNormal.y >= UpNormalThreshold)
            {
                // If we have a horizontal surface with a normal pointing up, classify it as a floor.
                PlaneType = SpatialAwarenessSurfaceTypes.Floor;

                if (plane.Bounds.Center.y > (floorYPosition + FloorBuffer))
                {
                    // If the plane is too high to be considered part of the floor, classify it as a table.
                    PlaneType = SpatialAwarenessSurfaceTypes.Platform;
                }
            }
            else if (surfaceNormal.y <= -(UpNormalThreshold))
            {
                // If we have a horizontal surface with a normal pointing down, classify it as a ceiling.
                PlaneType = SpatialAwarenessSurfaceTypes.Ceiling;

                if (plane.Bounds.Center.y < (ceilingYPosition - CeilingBuffer))
                {
                    // If the plane is not high enough to be considered part of the ceiling, classify it as a table.
                    PlaneType = SpatialAwarenessSurfaceTypes.Platform;
                }
            }
            else if (Mathf.Abs(surfaceNormal.y) <= (1 - UpNormalThreshold))
            {
                // If the plane is vertical, then classify it as a wall.
                PlaneType = SpatialAwarenessSurfaceTypes.Wall;
            }
            else
            {
                // The plane has a strange angle, classify it as 'unknown'.
                PlaneType = SpatialAwarenessSurfaceTypes.Unknown;
            }

            return PlaneType;
        }

        /// <summary>
        /// Destroys all game objects under parent
        /// </summary>
        private void DestroyPreviousPlanes()
        {
            // Remove any previously existing planes, as they may no longer be valid.
            for (int index = 0; index < activePlanes.Count; index++)
            {
                Destroy(activePlanes[index].GameObject);
            }
        }

        /// <summary>
        /// Sets visibility of planes based on their type.
        /// </summary>
        private void SetPlaneVisibility(SpatialAwarenessPlanarObject plane)
        {
            plane.GameObject.SetActive((drawPlanesMask & plane.SurfaceType) == plane.SurfaceType);
        }
#endif // PLANE_FINDING_PRESENT

        #endregion
    }
}