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
        /// Offsets the scale ratio so that text does not scale down too much. (Set to zero for linear scaling)
        /// </summary>
        [Tooltip("Offsets the scale ratio so that text does not scale down too much. (Set to zero for linear scaling)")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float sizeRatio = 0.0f;
        public float SizeRatio
        {
            get { return sizeRatio; }
            set
            {
                // Set the value of sizeRatio, while maintaining the supported range.
                if (value < 0.0f)
                {
                    sizeRatio = 0.0f;
                }
                else if (value > 1.0f)
                {
                    sizeRatio = 1.0f;
                }
                else
                {
                    sizeRatio = value;
                }
            }
        }

        /// <summary>
        /// The ratio between the transform's local scale and its starting distance from the camera.
        /// </summary>
        private Vector3 startingScale;

        /// <summary>
        /// The starting distance from the camera.
        /// </summary>
        private float startingDistance;

        private void Start()
        {
            // Calculate the XYZ ratios for the transform's localScale over its
            // initial distance from the camera.
            startingDistance = Vector3.Distance(CameraCache.Main.transform.position, transform.position);
            startingScale = transform.localScale;

            SetSizeRatio(SizeRatio);
        }

        /// <summary>
        /// Manually update the OverrideSizeRatio during runtime or through UnityEvents in the editor
        /// </summary>
        /// <param name="ratio"> 0 - 1 : Use 0 for linear scaling</param>
        public void SetSizeRatio(float ratio)
        {
            if (ratio == 0)
            {
                if (startingDistance > 0.0f)
                {
                    // set to a linear scale ratio
                    SizeRatio = 1 / startingDistance;
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
            else
            {
                SizeRatio = ratio;
            }
        }

        /// <summary>
        /// LateUpdate is used to ensure that the scaling occurs after any other processing (ex: cursor placement).
        /// </summary>
        private void LateUpdate()
        {
            float distanceToHologram = Vector3.Distance(CameraCache.Main.transform.position, transform.position);
            // create an offset ratio based on the starting position. This value creates a new angle that pivots
            // on the starting position that is more or less drastic than the normal scale ratio.
            float curvedRatio = 1 - startingDistance * SizeRatio;
            transform.localScale = startingScale * (distanceToHologram * SizeRatio + curvedRatio);
        }
    }
}