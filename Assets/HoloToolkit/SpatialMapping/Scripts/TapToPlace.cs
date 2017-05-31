// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
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
    [RequireComponent(typeof(Collider))]
    public class TapToPlace : MonoBehaviour, IInputClickHandler
    {
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

        /// <summary>
        /// Keeps track of the layer the game object was initially on.
        /// During placement the layer is switched to Ignore Raycast while placing the object.
        /// </summary>
        private int defaultLayer;

        /// <summary>
        /// The default ignore raycast layer built into unity.
        /// </summary>
        private const int DefaultIgnoreRaycastLayer = 2;

        protected virtual void Start()
        {
            defaultLayer = gameObject.layer;

#if UNITY_WSA && !UNITY_EDITOR
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
#endif

            if (PlaceParentOnTap)
            {
                if (ParentGameObjectToPlace != null && !gameObject.transform.IsChildOf(ParentGameObjectToPlace.transform))
                {
                    Debug.LogError("The specified parent object is not a parent of this object.");
                }

                DetermineParent();
            }
        }

        protected virtual void Update()
        {
            // If the user is in placing mode,
            // update the placement to match the user's gaze.
            if (!IsBeingPlaced) { return; }

            // Rotate this object to face the user.
            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;

#if UNITY_WSA && !UNITY_EDITOR
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 gazeDirection = Camera.main.transform.forward;

                // If we're using the spatial mapping, check to see if we got a hit, else use the gaze position.
                RaycastHit hitInfo;
                Vector3 placementPosition = SpatialMappingManager.Instance != null &&
                    Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMappingManager.LayerMask)
                        ? hitInfo.point
                        : GazeManager.Instance.HitPosition;
#else
            Vector3 placementPosition = GazeManager.Instance.HitPosition;
#endif

            // Here is where you might consider adding intelligence
            // to how the object is placed.  For example, consider
            // placing based on the bottom of the object's
            // collider so it sits properly on surfaces.

            if (PlaceParentOnTap)
            {
                ParentGameObjectToPlace.transform.position += placementPosition - gameObject.transform.position;
                ParentGameObjectToPlace.transform.rotation = toQuat;
            }
            else
            {
                gameObject.transform.position = placementPosition;
                gameObject.transform.rotation = toQuat;
            }
        }

        public virtual void OnInputClicked(InputClickedEventData eventData)
        {
            // On each tap gesture, toggle whether the user is in placing mode.
            IsBeingPlaced = !IsBeingPlaced;

            if (IsBeingPlaced)
            {
                gameObject.layer = DefaultIgnoreRaycastLayer;
                InputManager.Instance.AddGlobalListener(gameObject);
#if UNITY_WSA && !UNITY_EDITOR

                // If the user is in placing mode, display the spatial mapping mesh.
                SpatialMappingManager.Instance.DrawVisualMeshes = true;

                //Removes existing world anchor if any exist.
                WorldAnchorManager.Instance.RemoveAnchor(gameObject);
#endif
            }
            else
            {
                gameObject.layer = defaultLayer;
                InputManager.Instance.RemoveGlobalListener(gameObject);
#if UNITY_WSA && !UNITY_EDITOR

                // If the user is not in placing mode, hide the spatial mapping mesh.
                SpatialMappingManager.Instance.DrawVisualMeshes = false;

                // Add world anchor when object placement is done.
                WorldAnchorManager.Instance.AttachAnchor(gameObject, SavedAnchorFriendlyName);
#endif
            }
        }

        private void DetermineParent()
        {
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
        }
    }
}
