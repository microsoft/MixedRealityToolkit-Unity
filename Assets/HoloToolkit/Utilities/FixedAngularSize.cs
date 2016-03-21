// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Causes a Hologram to maintain a fixed angular size, which is to say it
    /// occupies the same pixels in the view regardless of its distance from
    /// the camera.
    /// </summary>
    public class FixedAngularSize : MonoBehaviour
    {
        // The ratio between the transform's local scale and its starting
        // distance from the camera.
        private Vector3 defaultSizeRatios;

        void Start()
        {
            // Calculate the XYZ ratios for the transform's localScale over its
            // initial distance from the camera.
            float startingDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (startingDistance > 0.0f)
            {
                defaultSizeRatios = transform.localScale / startingDistance;
            }
            else
            {
                // If the transform and the camera are both in the same
                // position (that is, the distance between them is zero),
                // disable this Behaviour so we don't get a DivdeByZero
                // error later on.
                enabled = false;
#if UNITY_EDITOR
                Debug.LogWarning("The object and the camera are in the same position at Start(). The attached FixedAngularSize Behaviour is now disabled.");
#endif // UNITY_EDITOR
            }
        }

        void Update()
        {
            float distanceToHologram = Vector3.Distance(Camera.main.transform.position, transform.position);
            transform.localScale = defaultSizeRatios * distanceToHologram;
        }
    }
}