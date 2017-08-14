// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the tap gesture again to place.
    /// This script is used in conjunction with GazeManager, WorldAnchorManager, and SpatialMappingManager.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Interpolator))]
    public class TapToPlace : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("Distance from camera to keep the object while placing it.")]
        public float DefaultGazeDistance = 2.0f;

        [Tooltip("Supply a friendly name for the anchor as the key name for the WorldAnchorStore.")]
        public string SavedAnchorFriendlyName = "SavedAnchorFriendlyName";

        [Tooltip("Place parent on tap instead of current game object.")]
        public bool PlaceParentOnTap;

        [Tooltip("Specify the parent game object to be moved on tap, if the immediate parent is not desired.")]
        public GameObject ParentGameObjectToPlace;

        /// <summary>
        /// Keeps track of if the user is moving the object or not.
        /// Setting this to true will enable the user to move and place the object in the scene.
        /// Useful when you want to place an object immediately.
        /// </summary>
        [Tooltip("Setting this to true will enable the user to move and place the object in the scene without needing to tap on the object. Useful when you want to place an object immediately.")]
        public bool IsBeingPlaced;

        [Tooltip("Setting this to true will allow this behavior to control the DrawMesh property on the spatial mapping.")]
        public bool AllowMeshVisualizationControl = true;

        /// <summary>
        /// The default ignore raycast layer built into unity.
        /// </summary>
        private const int IgnoreRaycastLayer = 2;

        private Interpolator interpolator;

        private static Dictionary<GameObject, int> defaultLayersCache = new Dictionary<GameObject, int>();

        protected virtual void Start()
        {
            // Make sure we have all the components in the scene we need.
            if (WorldAnchorManager.Instance == null)
            {
                Debug.LogError("This script expects that you have a WorldAnchorManager component in your scene.");
            }

            if (WorldAnchorManager.Instance != null)
            {
                // If we are not starting out with actively placing the object, give it a World Anchor
                if (!IsBeingPlaced)
                {
                    WorldAnchorManager.Instance.AttachAnchor(gameObject, SavedAnchorFriendlyName);
                }
            }

            DetermineParent();

            interpolator = PlaceParentOnTap
                ? ParentGameObjectToPlace.EnsureComponent<Interpolator>()
                : gameObject.EnsureComponent<Interpolator>();

            if (IsBeingPlaced)
            {
                HandlePlacement();
            }
        }

        protected virtual void Update()
        {
            if (!IsBeingPlaced) { return; }

            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            Vector3 placementPosition = GetPlacementPosition(headPosition, gazeDirection, DefaultGazeDistance);

            // Here is where you might consider adding intelligence
            // to how the object is placed.  For example, consider
            // placing based on the bottom of the object's
            // collider so it sits properly on surfaces.

            if (PlaceParentOnTap)
            {
                placementPosition = ParentGameObjectToPlace.transform.position + (placementPosition - gameObject.transform.position);
            }

            // update the placement to match the user's gaze.
            interpolator.SetTargetPosition(placementPosition);

            // Rotate this object to face the user.
            interpolator.SetTargetRotation(Quaternion.Euler(0, Camera.main.transform.localEulerAngles.y, 0));
        }

        /// <summary>
        /// If we're using the spatial mapping, check to see if we got a hit, else use the gaze position.
        /// </summary>
        /// <returns></returns>
        private static Vector3 GetPlacementPosition(Vector3 headPosition, Vector3 gazeDirection, float defaultGazeDistance)
        {
            RaycastHit hitInfo;
            if (SpatialMappingRaycast(headPosition, gazeDirection, out hitInfo))
            {
                return hitInfo.point;
            }
            return GetGazePlacementPosition(headPosition, gazeDirection, defaultGazeDistance);
        }

        /// <summary>
        /// Does a raycast on the spatial mapping layer to try to find a hit.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="spatialMapHit"></param>
        /// <returns>Wheter it found a hit or not</returns>
        private static bool SpatialMappingRaycast(Vector3 origin, Vector3 direction, out RaycastHit spatialMapHit)
        {
            if (SpatialMappingManager.Instance != null)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction, out hitInfo, 30.0f, SpatialMappingManager.Instance.LayerMask))
                {
                    spatialMapHit = hitInfo;
                    return true;
                }
            }
            spatialMapHit = new RaycastHit();
            return false;
        }

        /// <summary>
        /// Get gaze position from the GazeManagers hit or just infront of the user
        /// </summary>
        /// <param name="headPosition"></param>
        /// <param name="gazeDirection"></param>
        /// <param name="defaultGazeDistance"></param>
        /// <returns></returns>
        private static Vector3 GetGazePlacementPosition(Vector3 headPosition, Vector3 gazeDirection, float defaultGazeDistance)
        {
            if (GazeManager.Instance.HitObject != null)
            {
                return GazeManager.Instance.HitPosition;
            }
            return headPosition + gazeDirection * defaultGazeDistance;
        }

        public virtual void OnInputClicked(InputClickedEventData eventData)
        {
            // On each tap gesture, toggle whether the user is in placing mode.
            IsBeingPlaced = !IsBeingPlaced;
            HandlePlacement();
        }

        private void HandlePlacement()
        {
            if (IsBeingPlaced)
            {
                SetLayerRecursively(transform, useDefaultLayer: false);
                InputManager.Instance.AddGlobalListener(gameObject);

                // If the user is in placing mode, display the spatial mapping mesh.
                if (AllowMeshVisualizationControl)
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;
                }
#if UNITY_WSA && !UNITY_EDITOR

                //Removes existing world anchor if any exist.
                WorldAnchorManager.Instance.RemoveAnchor(gameObject);
#endif
            } else
            {
                SetLayerRecursively(transform, useDefaultLayer: true);
                // Clear our cache in case we added or removed gameobjects between taps
                defaultLayersCache.Clear();
                InputManager.Instance.RemoveGlobalListener(gameObject);

                // If the user is not in placing mode, hide the spatial mapping mesh.
                if (AllowMeshVisualizationControl)
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = false;
                }
#if UNITY_WSA && !UNITY_EDITOR

                // Add world anchor when object placement is done.
                WorldAnchorManager.Instance.AttachAnchor(gameObject, SavedAnchorFriendlyName);
#endif
            }
        }

        private void DetermineParent()
        {
            if (!PlaceParentOnTap) { return; }

            if (ParentGameObjectToPlace == null)
            {
                if (gameObject.transform.parent == null)
                {
                    Debug.LogWarning("The selected GameObject has no parent.");
                    PlaceParentOnTap = false;
                } else
                {
                    Debug.LogWarning("No parent specified. Using immediate parent instead: " + gameObject.transform.parent.gameObject.name);
                    ParentGameObjectToPlace = gameObject.transform.parent.gameObject;
                }
            }

            if (ParentGameObjectToPlace != null && !gameObject.transform.IsChildOf(ParentGameObjectToPlace.transform))
            {
                Debug.LogWarning("The specified parent object is not a parent of this object.");
            }
        }

        private static void SetLayerRecursively(Transform objectToSet, bool useDefaultLayer)
        {
            if (useDefaultLayer)
            {
                int defaultLayerId;
                if (defaultLayersCache.TryGetValue(objectToSet.gameObject, out defaultLayerId))
                {
                    objectToSet.gameObject.layer = defaultLayerId;
                    defaultLayersCache.Remove(objectToSet.gameObject);
                }
            } else
            {
                defaultLayersCache.Add(objectToSet.gameObject, objectToSet.gameObject.layer);

                objectToSet.gameObject.layer = IgnoreRaycastLayer;
            }

            for (int i = 0; i < objectToSet.childCount; i++)
            {
                SetLayerRecursively(objectToSet.GetChild(i), useDefaultLayer);
            }
        }
    }
}
