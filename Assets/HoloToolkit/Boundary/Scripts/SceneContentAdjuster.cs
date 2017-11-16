// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        private Vector3 lastFloorHeight;
        private float floorHeightOffset = 1f;

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
            if (Application.isEditor && XRDevice.isPresent)
            {
                lastFloorHeight.y = floorHeightOffset;
                containerObject.position = lastFloorHeight;
            }
#else
        if (VRDevice.isPresent)
        {
            Destroy(this);
        }
#endif
        }

        private void Update()
        {
#if UNITY_2017_2_OR_NEWER
            if (!Application.isEditor && XRDevice.isPresent)
            {
                floorHeightOffset = BoundaryManager.Instance.CurrentFloorHeightOffset;

                if (lastFloorHeight.y != floorHeightOffset)
                {
                    lastFloorHeight.y = floorHeightOffset;
                    containerObject.position = lastFloorHeight;
                }
            }
#endif
        }
    }
}
