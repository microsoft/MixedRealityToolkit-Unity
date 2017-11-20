// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace HoloToolkit.Unity.Boundary
{
    public class SceneContentAdjuster : MonoBehaviour
    {
        private int frameWaitHack = 0;

        [SerializeField]
        [Tooltip("Optional container object reference. If null, this script will move the object it's attached to.")]
        private Transform containerObject;

        [SerializeField]
        [Tooltip("Select this if the container should be placed in front of the head on app launch in a room scale app.")]
        private bool alignWithHeadHeight = true;

        [SerializeField]
        [Tooltip("Use this to set the desired position of the container in a stationary app.")]
        private Vector3 stationarySpaceTypePosition = Vector3.zero;

        [SerializeField]
        [Tooltip("Use this to set the desired position of the container in a room scale app, if alignWithInitialHeadHeight has not been set.")]
        private Vector3 roomScaleSpaceTypePosition = Vector3.zero;

        private Vector3 contentPosition = Vector3.zero;

        private void Awake()
        {
            if (containerObject == null)
            {
                containerObject = transform;
            }

#if UNITY_2017_2_OR_NEWER
            // If no XR device is present, the editor will default to (0, 0, 0) and no adjustment is needed.
            // This script runs on both opaque and transparent display devices, since the floor offset is based on
            // TrackingSpaceType and not display type.
            if (!XRDevice.isPresent)
#else
            if (true)
#endif
            {
                Destroy(this);
            }
            else
            {
                StartCoroutine(SetContentHeight());
            }
        }

        private IEnumerator SetContentHeight()
        {
            if (frameWaitHack < 1)
            {
                // Not waiting a frame often caused the camera's position to be incorrect at this point. This seems like a Unity bug.
                frameWaitHack++;
                yield return null;
            }

            if (XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale)
            {
                if (alignWithHeadHeight)
                {
                    contentPosition.x = containerObject.position.x;
                    contentPosition.y = containerObject.position.y + CameraCache.Main.transform.position.y;
                    contentPosition.z = containerObject.position.z;

                    containerObject.position = contentPosition;
                }
                else
                {
                    containerObject.position = roomScaleSpaceTypePosition;
                }
            }
            else if (XRDevice.GetTrackingSpaceType() == TrackingSpaceType.Stationary)
            {
                containerObject.position = stationarySpaceTypePosition;
            }
        }
    }
}
