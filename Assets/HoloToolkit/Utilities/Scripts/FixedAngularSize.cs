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
        /// <summary>
        /// Off sets the scale ratio so that text does not scale down so much. (Set to zero for linear scaling)
        /// </summary>
        public float OverrideSizeRatio = 0;
        // The ratio between the transform's local scale and its starting
        // distance from the camera.
        private float startingDistance;
        private Vector3 startingScale;

        void Start()
        {
            // Calculate the XYZ ratios for the transform's localScale over its
            // initial distance from the camera.
            startingDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
            startingScale = transform.localScale;

            if (startingDistance > 0.0f)
            {
                if (OverrideSizeRatio == 0)
                {
                    // set to a linear scale ratio
                    OverrideSizeRatio = 1 / startingDistance;
                }
            }
            else
            {
                // If the transform and the camera are both in the same
                // position (that is, the distance between them is zero),
                // disable this Behaviour so we don't get a DivideByZero
                // error later on.
                enabled = false;
#if UNITY_EDITOR
                Debug.LogWarning("The object and the camera are in the same position at Start(). The attached FixedAngularSize Behaviour is now disabled.");
#endif // UNITY_EDITOR
            }
        }

        public void SetSizeRatio(float ratio)
        {
            if (ratio == 0)
            {
                OverrideSizeRatio = 1 / startingDistance;
            }
            else
            {
                OverrideSizeRatio = ratio;
            }
        }

        void Update()
        {
            float distanceToHologram = Vector3.Distance(Camera.main.transform.position, transform.position);
            // create an offset ratio based on the starting position. This value creates a new angle that pivots
            // on the starting position that is more or less drastic than the normal scale ratio.
            float CurvedRatio = 1 - startingDistance * OverrideSizeRatio;
            transform.localScale = startingScale * (distanceToHologram * OverrideSizeRatio + CurvedRatio);
        }
    }
}