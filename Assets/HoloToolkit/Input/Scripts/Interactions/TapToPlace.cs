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

            // If we're using the spatial mapping, check to see if we got a hit, else use the gaze position.
            RaycastHit hitInfo;
            Vector3 placementPosition = SpatialMappingManager.Instance != null &&
                Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMappingManager.Instance.LayerMask)
                    ? hitInfo.point
                    : (GazeManager.Instance.HitObject == null
                        ? GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * DefaultGazeDistance
                        : GazeManager.Instance.HitPosition);

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
            }
            else
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
                }
                else
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
            }
            else
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
