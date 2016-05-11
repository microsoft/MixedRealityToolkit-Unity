// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

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
    /// </summary>

    public partial class TapToPlace : MonoBehaviour
    {
        bool placing = false;

        // Called by GazeGestureManager when the user performs a tap gesture.
        void OnSelect()
        {
            if (SpatialMappingManager.Instance != null)
            {
                // On each tap gesture, toggle whether the user is in placing mode.
                placing = !placing;

                // If the user is in placing mode, display the spatial mapping mesh.
                if (placing)
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;
                }
                // If the user is not in placing mode, hide the spatial mapping mesh.
                else
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = false;
                }
            }
            else
            {
                Debug.Log("TapToPlace requires spatial mapping.  Try adding SpatialMapping prefab to project.");
            }
        }

        // Update is called once per frame.
        void Update()
        {
                // If the user is in placing mode,
                // update the placement to match the user's gaze.
                if (placing)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

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
                    this.transform.position = hitInfo.point;

                    // Rotate this object to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    this.transform.rotation = toQuat;
                }
            }
        }
    }
}
