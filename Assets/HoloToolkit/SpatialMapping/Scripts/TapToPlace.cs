// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// This script is used in conjunction with GazeManager, GestureManager,
    /// and SpatialMappingManager.
    /// TapToPlace also adds a WorldAnchor component to enable persistence.
    /// </summary>

    public partial class TapToPlace : MonoBehaviour
    {
        [Tooltip("Supply a friendly name for the anchor as the key name for the WorldAnchorStore.")]
        public string SavedAnchorFriendlyName = "SavedAnchorFriendlyName";

        /// <summary>
        /// Keeps track of anchors stored on local device.
        /// </summary>
        private WorldAnchorStore anchorStore;

        /// <summary>
        /// Locally saved wold anchor.
        /// </summary>
        private WorldAnchor savedAnchor;

        private bool placing;

        private void Start()
        {
            WorldAnchorStore.GetAsync(AnchorStoreReady);
        }

        private void OnEnable()
        {
            GestureManager.Instance.OnTap += OnTap;
        }

        private void OnDisable()
        {
            GestureManager.Instance.OnTap -= OnTap;
        }

        /// <summary>
        /// Called when the local anchor store is ready.
        /// </summary>
        /// <param name="store">Anchor Store.</param>
        private void AnchorStoreReady(WorldAnchorStore store)
        {
            anchorStore = store;

            // Try to load a previously saved world anchor.
            savedAnchor = anchorStore.Load(SavedAnchorFriendlyName, gameObject);
            if (savedAnchor == null)
            {
                // Either world anchor was not saved / does not exist or has a different name.
                Debug.LogFormat("{0} : World anchor could not be loaded for this game object. Creating a new anchor.", gameObject.name);

                // Create anchor since one does not exist.
                CreateAnchor();
            }
            else
            {
                Debug.LogFormat("{0} : World anchor loaded from anchor store and updated for this game object.", gameObject.name);
            }
        }

        // Called by GazeGestureManager when the user performs a tap gesture.
        private void OnTap(GameObject go)
        {
            if (SpatialMappingManager.Instance != null)
            {
                // On each tap gesture, toggle whether the user is in placing mode.
                placing = !placing;

                // If the user is in placing mode, display the spatial mapping mesh.
                if (placing)
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;

                    Debug.LogFormat("{0} : Removing existing world anchor if any.", gameObject.name);

                    // Remove existing world anchor when moving an object.
                    DestroyImmediate(gameObject.GetComponent<WorldAnchor>());

                    // Delete existing world anchor from anchor store when moving an object.
                    if (anchorStore != null)
                    {
                        anchorStore.Delete(SavedAnchorFriendlyName);
                    }
                }
                // If the user is not in placing mode, hide the spatial mapping mesh.
                else
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = false;

                    // Add world anchor when object placement is done.
                    CreateAnchor();
                }
            }
            else
            {
                Debug.Log("TapToPlace requires spatial mapping.  Try adding SpatialMapping prefab to project.");
            }
        }

        private void CreateAnchor()
        {
            // NOTE: It's good practice to ensure your parent hierarchy or root game object does not already have a World Anchor.
            // You can handle this in a way that works best for your application.
            // For example: gameObject.transform.root.GetComponent<WorldAnchor>();

            // Add the world anchor component when done moving an object.
            WorldAnchor anchor = gameObject.AddComponent<WorldAnchor>();
            if (anchor.isLocated)
            {
                SaveAnchor(anchor);
            }
            else
            {
                anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
            }
        }

        private void SaveAnchor(WorldAnchor anchor)
        {
            // Save the anchor to persist holograms across sessions.
            if (anchorStore.Save(SavedAnchorFriendlyName, anchor))
            {
                Debug.LogFormat("{0} : World anchor saved successfully.", gameObject.name);
            }
            else
            {
                Debug.LogErrorFormat("{0} : World anchor save failed.", gameObject.name);
            }
        }

        private void Anchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                Debug.LogFormat("{0} : World anchor located successfully.", gameObject.name);

                SaveAnchor(self);
                self.OnTrackingChanged -= Anchor_OnTrackingChanged;
            }
            else
            {
                Debug.LogErrorFormat("{0} : World anchor failed to be located.", gameObject.name);
            }
        }

        private void Update()
        {
            // If the user is in placing mode,
            // update the placement to match the user's gaze.
            if (placing)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                    30.0f, SpatialMappingManager.Instance.LayerMask))
                {
                    // Move this object to where the raycast
                    // hit the Spatial Mapping mesh.
                    // Here is where you might consider adding intelligence
                    // to how the object is placed.  For example, consider
                    // placing based on the bottom of the object's
                    // collider so it sits properly on surfaces.
                    transform.position = hitInfo.point;

                    // Rotate this object to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    transform.rotation = toQuat;
                }
            }
        }
    }
}