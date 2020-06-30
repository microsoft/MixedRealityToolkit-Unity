// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The Billboard class implements the behaviors needed to keep a GameObject oriented towards the user.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/Billboard")]
    public class Billboard : MonoBehaviour
    {
        /// <summary>
        /// The axis about which the object will rotate.
        /// </summary>
        public PivotAxis PivotAxis
        {
            get { return pivotAxis; }
            set { pivotAxis = value; }
        }

        [Tooltip("Specifies the axis about which the object will rotate.")]
        [SerializeField]
        private PivotAxis pivotAxis = PivotAxis.XY;

        /// <summary>
        /// The target we will orient to. If no target is specified, the main camera will be used.
        /// </summary>
        public Transform TargetTransform
        {
            get { return targetTransform; }
            set { targetTransform = value; }
        }

        [Tooltip("Specifies the target we will orient to. If no target is specified, the main camera will be used.")]
        [SerializeField]
        private Transform targetTransform;

        private void OnEnable()
        {
            if (targetTransform == null)
            {
                targetTransform = CameraCache.Main.transform;
            }
        }

        /// <summary>
        /// Keeps the object facing the camera.
        /// </summary>
        private void Update()
        {
            if (targetTransform == null)
            {
                return;
            }

            // Get a Vector that points from the target to the main camera.
            Vector3 directionToTarget = targetTransform.position - transform.position;

            bool useCameraAsUpVector = true;

            // Adjust for the pivot axis.
            switch (pivotAxis)
            {
                case PivotAxis.X:
                    directionToTarget.x = 0.0f;
                    useCameraAsUpVector = false;
                    break;

                case PivotAxis.Y:
                    directionToTarget.y = 0.0f;
                    useCameraAsUpVector = false;
                    break;

                case PivotAxis.Z:
                    directionToTarget.x = 0.0f;
                    directionToTarget.y = 0.0f;
                    break;

                case PivotAxis.XY:
                    useCameraAsUpVector = false;
                    break;

                case PivotAxis.XZ:
                    directionToTarget.x = 0.0f;
                    break;

                case PivotAxis.YZ:
                    directionToTarget.y = 0.0f;
                    break;

                case PivotAxis.Free:
                default:
                    // No changes needed.
                    break;
            }

            // If we are right next to the camera the rotation is undefined. 
            if (directionToTarget.sqrMagnitude < 0.001f)
            {
                return;
            }

            // Calculate and apply the rotation required to reorient the object
            if (useCameraAsUpVector)
            {
                transform.rotation = Quaternion.LookRotation(-directionToTarget, CameraCache.Main.transform.up);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(-directionToTarget);
            }
        }
    }
}