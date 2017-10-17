// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class ObjectSurfaceObserver : SpatialMappingSource
    {
        [Tooltip("The room model to use when loading meshes in Unity.")]
        public GameObject RoomModel;

        [Tooltip("If greater than or equal to zero, surface objects will claim to be updated at this period. This is useful when working with libraries that respond to updates (such as the SpatialUnderstanding library). If negative, surfaces will not claim to be updated.")]
        public float SimulatedUpdatePeriodInSeconds = -1;

        private void Start()
        {
#if UNITY_2017_2_OR_NEWER
            if (!UnityEngine.XR.XRDevice.isPresent && Application.isEditor)
#else
            if (!UnityEngine.VR.VRDevice.isPresent && Application.isEditor)
#endif
            {
                // When in the Unity editor and not remoting, try loading saved meshes from a model.
                Load(RoomModel);

                if (GetMeshFilters().Count > 0)
                {
                    SpatialMappingManager.Instance.SetSpatialMappingSource(this);
                }
            }
        }

        /// <summary>
        /// Loads the SpatialMapping mesh from the specified room object.
        /// </summary>
        /// <param name="roomModel">The room model to load meshes from.</param>
        public void Load(GameObject roomModel)
        {
            if (roomModel == null)
            {
                Debug.Log("No room model specified.");
                return;
            }

            GameObject roomObject = Instantiate(roomModel);
            Cleanup();

            try
            {
                MeshFilter[] roomFilters = roomObject.GetComponentsInChildren<MeshFilter>();

                for (int iMesh = 0; iMesh < roomFilters.Length; iMesh++)
                {
                    AddSurfaceObject(CreateSurfaceObject(
                        mesh: roomFilters[iMesh].sharedMesh,
                        objectName: "roomMesh-" + iMesh,
                        parentObject: transform,
                        meshID: iMesh
                        ));
                }
            }
            catch
            {
                Debug.Log("Failed to load object " + roomModel.name);
            }
            finally
            {
                if (roomObject != null)
                {
                    DestroyImmediate(roomObject);
                }
            }
        }

        private float lastUpdateUnscaledTimeInSeconds = 0;

        private void Update()
        {
            if (SimulatedUpdatePeriodInSeconds >= 0)
            {
                if ((Time.unscaledTime - lastUpdateUnscaledTimeInSeconds) >= SimulatedUpdatePeriodInSeconds)
                {
                    for (int iSurface = 0; iSurface < SurfaceObjects.Count; iSurface++)
                    {
                        UpdateOrAddSurfaceObject(SurfaceObjects[iSurface]);
                    }

                    lastUpdateUnscaledTimeInSeconds = Time.unscaledTime;
                }
            }
        }
    }
}