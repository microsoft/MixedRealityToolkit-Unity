// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The SpatialMappingManager class allows applications to use a SurfaceObserver or a stored 
    /// Spatial Mapping mesh (loaded from a file).
    /// When an application loads a mesh file, the SurfaceObserver is stopped.
    /// Calling StartObserver() clears the stored mesh and enables real-time SpatialMapping updates.
    /// </summary>
    [RequireComponent(typeof(SpatialMappingObserver))]
    public class SpatialMappingManager : Singleton<SpatialMappingManager>
    {
        [Tooltip("The physics layer for spatial mapping objects to be set to.")]
        public int PhysicsLayer = 31;

        [Tooltip("The material to use for rendering spatial mapping data.")]
        public Material surfaceMaterial;

        [Tooltip("Determines if spatial mapping data will be rendered.")]
        public bool drawVisualMeshes = false;

        [Tooltip("Determines if spatial mapping data will cast shadows.")]
        public bool castShadows = false;

        /// <summary>
        /// Used for gathering real-time Spatial Mapping data on the HoloLens.
        /// </summary>
        private SpatialMappingObserver surfaceObserver;

        /// <summary>
        /// Used for loading or saving spatial mapping data to disk.
        /// </summary>
        private FileSurfaceObserver fileSurfaceObserver;

        /// <summary>
        /// Used for sending meshes over the network and saving them to disk.
        /// </summary>
        private RemoteMeshTarget remoteMeshTarget;

        /// <summary>
        /// Time when StartObserver() was called.
        /// </summary>
        [HideInInspector]
        public float StartTime { get; private set; }

        /// <summary>
        /// The current source of spatial mapping data.
        /// </summary>
        public SpatialMappingSource Source { get; private set; }

        // Called when the GameObject is first created.
        private void Awake()
        {
            surfaceObserver = gameObject.GetComponent<SpatialMappingObserver>();
            Source = surfaceObserver;
        }

        // Use for initialization.
        private void Start()
        {
            remoteMeshTarget = FindObjectOfType<RemoteMeshTarget>();

#if !UNITY_EDITOR
            StartObserver();
#endif

#if UNITY_EDITOR
            fileSurfaceObserver = GetComponent<FileSurfaceObserver>();

            if (fileSurfaceObserver != null)
            {
                // In the Unity editor, try loading a saved mesh.
                fileSurfaceObserver.Load(fileSurfaceObserver.MeshFileName);

                if (fileSurfaceObserver.GetMeshFilters().Count > 0)
                {
                    SetSpatialMappingSource(fileSurfaceObserver);
                }
                else if(remoteMeshTarget != null)
                {
                    SetSpatialMappingSource(remoteMeshTarget);
                }
            }
#endif
        }

        // Called every frame.
        private void Update()
        {
            // There are a few keyboard commands we will add when in the editor.
#if UNITY_EDITOR
            // F - to use the 'file' sourced mesh.
            if (Input.GetKeyUp(KeyCode.F))
            {
                SpatialMappingManager.Instance.SetSpatialMappingSource(fileSurfaceObserver);
            }

            // S - saves the active mesh
            if (Input.GetKeyUp(KeyCode.S))
            {
                MeshSaver.Save(fileSurfaceObserver.MeshFileName, SpatialMappingManager.Instance.GetMeshes());
            }

            // L - loads the previously saved mesh into the file source.
            if (Input.GetKeyUp(KeyCode.L))
            {
                fileSurfaceObserver.Load(fileSurfaceObserver.MeshFileName);
            }
#endif
        }

        /// <summary>
        /// Returns the layer as a bit mask.
        /// </summary>
        public int LayerMask
        {
            get { return (1 << PhysicsLayer); }
        }

        /// <summary>
        /// The material to use when rendering surfaces.
        /// </summary>
        public Material SurfaceMaterial
        {
            get
            {
                return surfaceMaterial;
            }
            set
            {
                if(value != surfaceMaterial)
                {
                    surfaceMaterial = value;
                    SetSurfaceMaterial(surfaceMaterial);
                }
            }
        }

        /// <summary>
        /// Specifies whether or not the SpatialMapping meshes are to be rendered.
        /// </summary>
        public bool DrawVisualMeshes
        {
            get
            {
                return drawVisualMeshes;
            }
            set
            {
                if (value != drawVisualMeshes)
                {
                    drawVisualMeshes = value;
                    UpdateRendering(drawVisualMeshes);
                }
            }
        }

        /// <summary>
        /// Specifies whether or not the SpatialMapping meshes can cast shadows.
        /// </summary>
        public bool CastShadows
        {
            get
            {
                return castShadows;
            }
            set
            {
                if (value != castShadows)
                {
                    castShadows = value;
                    SetShadowCasting(castShadows);
                }
            }
        }

        /// <summary>
        /// Sets the source of surface information.
        /// </summary>
        /// <param name="mappingSource">The source to switch to. Null means return to the live stream if possible.</param>
        public void SetSpatialMappingSource(SpatialMappingSource mappingSource)
        {
            UpdateRendering(false);

            if (mappingSource == null)
            {
                Source = surfaceObserver;
            }
            else
            {
                Source = mappingSource;
            }

            UpdateRendering(DrawVisualMeshes);
        }

        /// <summary>
        /// Sets the material used by all Spatial Mapping meshes.
        /// </summary>
        /// <param name="surfaceMaterial">New material to apply.</param>
        public void SetSurfaceMaterial(Material surfaceMaterial)
        {
            SurfaceMaterial = surfaceMaterial;
            if (DrawVisualMeshes)
            {
                foreach (Renderer renderer in Source.GetMeshRenderers())
                {
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = surfaceMaterial;
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if the SurfaceObserver is currently running.
        /// </summary>
        /// <returns>True, if the observer state is running.</returns>
        public bool IsObserverRunning()
        {
            return surfaceObserver.ObserverState == ObserverStates.Running;
        }

        /// <summary>
        /// Instructs the SurfaceObserver to start updating the SpatialMapping mesh.
        /// </summary>
        public void StartObserver()
        {
            if (!IsObserverRunning())
            {
                surfaceObserver.StartObserving();
                StartTime = Time.time;
            }
        }

        /// <summary>
        /// Instructs the SurfacesurfaceObserver to stop updating the SpatialMapping mesh.
        /// </summary>
        public void StopObserver()
        {
            if (IsObserverRunning())
            {
                surfaceObserver.StopObserving();
            }
        }

        /// <summary>
        /// Gets all meshes that are associated with the SpatialMapping mesh.
        /// </summary>
        /// <returns>
        /// Collection of Mesh objects representing the SpatialMapping mesh.
        /// </returns>
        public List<Mesh> GetMeshes()
        {
            List<Mesh> meshes = new List<Mesh>();
            List<MeshFilter> meshFilters = GetMeshFilters();

            // Get all valid mesh filters for observed surfaces.
            foreach (MeshFilter filter in meshFilters)
            {
                // GetMeshFilters ensures that both filter and filter.sharedMesh are not null.
                meshes.Add(filter.sharedMesh);
            }

            return meshes;
        }

        /// <summary>
        /// Gets all Mesh Filter objects associated with the Spatial Mapping mesh.
        /// </summary>
        /// <returns>Collection of Mesh Filter objects.</returns>
        public List<MeshFilter> GetMeshFilters()
        {
            return Source.GetMeshFilters();
        }

        /// <summary>
        /// Sets the Cast Shadows property for each Spatial Mapping mesh renderer.
        /// </summary>
        private void SetShadowCasting(bool castShadows)
        {
            CastShadows = castShadows;
            foreach(Renderer renderer in Source.GetMeshRenderers())
            {
                if (renderer != null)
                {
                    if (castShadows)
                    {
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                    else
                    {
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the rendering state on the currently enabled surfaces.
        /// Updates the material and shadow casting mode for each renderer.
        /// </summary>
        /// <param name="Enable">True, if meshes should be rendered.</param>
        private void UpdateRendering(bool Enable)
        {
            List<MeshRenderer> renderers = Source.GetMeshRenderers();
            for (int index = 0; index < renderers.Count; index++)
            {
                renderers[index].enabled = Enable;
                if (Enable)
                {
                    renderers[index].sharedMaterial = SurfaceMaterial;
                }
            }
        }
    }
}