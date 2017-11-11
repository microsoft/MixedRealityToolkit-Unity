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
        private float contentHeightOffset = 1f;
        private int frameWaitHack = 0;

        [SerializeField]
        [Tooltip("Optional container object reference.  If null, this script will move the object it's attached to.")]
        private Transform containerObject;


        private void Awake()
        {
            if (containerObject == null)
            {
                containerObject = transform;
            }

#if UNITY_2017_2_OR_NEWER
            // A Stationary TrackingSpaceType doesn't need any changes for an object at height 0.
            if (XRDevice.GetTrackingSpaceType() == TrackingSpaceType.Stationary || !XRDevice.isPresent)
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

            containerObject.position = new Vector3(containerObject.position.x, CameraCache.Main.transform.position.y, containerObject.position.z);
        }
    }
}
