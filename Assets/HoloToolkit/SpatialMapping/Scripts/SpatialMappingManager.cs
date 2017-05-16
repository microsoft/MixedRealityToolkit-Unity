// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
{
    /// <summary>
    /// The SpatialMappingManager class allows applications to use a SurfaceObserver or a stored 
    /// Spatial Mapping mesh (loaded from a file).
    /// When an application loads a mesh file, the SurfaceObserver is stopped.
    /// Calling StartObserver() clears the stored mesh and enables real-time SpatialMapping updates.
    /// </summary>
    [RequireComponent(typeof(SpatialMappingObserver))]
    public partial class SpatialMappingManager : Singleton<SpatialMappingManager>
    {
        /// <summary>
        /// Time when StartObserver() was called.
        /// </summary>
        public float StartTime { get; private set; }

        /// <summary>
        /// SurfaceMappingObserver GET
        /// </summary>
        public SpatialMappingObserver SurfaceObserver { get { return surfaceObserver; } }

        /// <summary>
        /// The current source of spatial mapping data.
        /// </summary>
        public SpatialMappingSource Source
        {
            get { return source; }

            private set
            {
                if (source != value)
                {
                    UpdateRendering(false);

                    var oldSource = source;
                    source = value;

                    UpdateRendering(DrawVisualMeshes);

                    var handlers = SourceChanged;
                    if (handlers != null)
                    {
                        handlers(this, PropertyChangedEventArgsEx.Create(() => Source, oldSource, source));
                    }
                }
            }
        }
        private SpatialMappingSource source;

        /// <summary>
        /// Occurs when <see cref="Source" /> changes.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgsEx<SpatialMappingSource>> SourceChanged;

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
            get { return surfaceMaterial; }
            set
            {
                if (value != surfaceMaterial)
                {
                    surfaceMaterial = value;
                    SetSurfaceMaterial(surfaceMaterial);
                }
            }
        }
        [SerializeField]
        private Material surfaceMaterial;

        /// <summary>
        /// Specifies whether or not the SpatialMapping meshes are to be rendered.
        /// </summary>
        public bool DrawVisualMeshes
        {
            get { return drawVisualMeshes; }
            set
            {
                if (value != drawVisualMeshes)
                {
                    drawVisualMeshes = value;
                    UpdateRendering(drawVisualMeshes);
                }
            }
        }
        [SerializeField]
        private bool drawVisualMeshes;

        /// <summary>
        /// Specifies whether or not the SpatialMapping meshes can cast shadows.
        /// </summary>
        public bool CastShadows
        {
            get { return castShadows; }
            set
            {
                if (value != castShadows)
                {
                    castShadows = value;
                    SetShadowCasting(castShadows);
                }
            }
        }
        [SerializeField]
        private bool castShadows;

        /// <summary>
        /// The Physics layer assigned to handle Spatial Mapping.
        /// </summary>
        public int PhysicsLayer
        {
            get { return physicsLayer; }
        }
        private int physicsLayer = 31;

        /// <summary>
        /// Auto Starts the Observer in Start()
        /// </summary>
        [SerializeField]
        public bool AutoStartObserver = true;

        /// <summary>
        /// The Maximum radius from the tap, that the pulse will expand from.
        /// </summary>
        [SerializeField]
        [Range(0.01f, 100f)]
        public float PulseMaximum = 10f;

        /// <summary>
        /// Used for gathering real-time Spatial Mapping data on the HoloLens.
        /// </summary>
        private SpatialMappingObserver surfaceObserver;

        // Called when the GameObject is first created.
        protected override void Awake()
        {
            base.Awake();

            surfaceObserver = gameObject.GetComponent<SpatialMappingObserver>();
            Source = surfaceObserver;
        }

        // Use for initialization.
        private void Start()
        {
            if (AutoStartObserver)
            {
                StartObserver();
            }
        }

        /// <summary>
        /// Sets the source of surface information.
        /// </summary>
        /// <param name="mappingSource">The source to switch to. Null means return to the live stream if possible.</param>
        public void SetSpatialMappingSource(SpatialMappingSource mappingSource)
        {
            UpdateRendering(false);

            Source = mappingSource ?? surfaceObserver;

            UpdateRendering(DrawVisualMeshes);
        }

        /// <summary>
        /// Sets the material used by all Spatial Mapping meshes.
        /// </summary>
        /// <param name="setSurfaceMaterial">New material to apply.</param>
        public void SetSurfaceMaterial(Material setSurfaceMaterial)
        {
            SurfaceMaterial = setSurfaceMaterial;
            if (DrawVisualMeshes)
            {
                foreach (MeshRenderer sourceRenderer in Source.GetMeshRenderers())
                {
                    if (sourceRenderer != null)
                    {
                        sourceRenderer.sharedMaterial = setSurfaceMaterial;
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
#if UNITY_EDITOR || UNITY_UWP
            // Allow observering if a device is present (Holographic Remoting)
            if (!UnityEngine.VR.VRDevice.isPresent) return;
#endif
            if (!IsObserverRunning())
            {
                surfaceObserver.StartObserving();
                StartTime = Time.unscaledTime;
            }
        }

        /// <summary>
        /// Instructs the SurfaceObserver to stop updating the SpatialMapping mesh.
        /// </summary>
        public void StopObserver()
        {
#if UNITY_EDITOR || UNITY_UWP
            // Allow observering if a device is present (Holographic Remoting)
            if (!UnityEngine.VR.VRDevice.isPresent) return;
#endif
            if (IsObserverRunning())
            {
                surfaceObserver.StopObserving();
            }
        }

        /// <summary>
        /// Instructs the SurfaceObserver to stop and cleanup all meshes.
        /// </summary>
        public void CleanupObserver()
        {
            surfaceObserver.CleanupObserver();
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
            for (int i = 0; i < meshFilters.Count; i++)
            {
                // GetMeshFilters ensures that both filter and filter.sharedMesh are not null.
                meshes.Add(meshFilters[i].sharedMesh);
            }

            return meshes;
        }

        /// <summary>
        /// Gets all the surface objects associated with the Spatial Mapping mesh.
        /// </summary>
        /// <returns>Collection of SurfaceObjects.</returns>
        public ReadOnlyCollection<SpatialMappingSource.SurfaceObject> GetSurfaceObjects()
        {
            return Source.SurfaceObjects;
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
        private void SetShadowCasting(bool canCastShadows)
        {
            CastShadows = canCastShadows;
            foreach (MeshRenderer sourceRenderer in Source.GetMeshRenderers())
            {
                if (sourceRenderer != null)
                {
                    if (canCastShadows)
                    {
                        sourceRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                    else
                    {
                        sourceRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the rendering state on the currently enabled surfaces.
        /// Updates the material and shadow casting mode for each renderer.
        /// </summary>
        /// <param name="enable">True, if meshes should be rendered.</param>
        private void UpdateRendering(bool enable)
        {
            if (Source != null)
            {
                List<MeshRenderer> renderers = Source.GetMeshRenderers();
                for (int index = 0; index < renderers.Count; index++)
                {
                    if (renderers[index] != null)
                    {
                        renderers[index].enabled = enable;
                        if (enable)
                        {
                            renderers[index].sharedMaterial = SurfaceMaterial;
                        }
                    }
                }
            }
        }
    }
}