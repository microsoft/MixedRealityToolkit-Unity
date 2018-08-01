//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HoloToolkit.UX.ToolTips
{
    /// <summary>
    /// A background with 'fake' inertia
    /// Useful for soft or liquid objects
    /// </summary>
    public class ToolTipBackgroundBlob : ToolTipBackground
    {
        /// <summary>
        /// Which transforms to use for each type of distortion
        /// See the ToolTipBalloon prefab for an example of which transforms to target
        /// </summary>
        [Header("Transform targets")]
        [SerializeField]
        private Transform positionTarget = null;

        [SerializeField]
        private Transform rotationTarget = null;

        [SerializeField]
        private Transform distortionTarget = null;

        [SerializeField]
        private Transform attachPointOffset = null;

        [Header("Blob settings")]
        [SerializeField]
        [Range(0f, 5f)]
        private float blobInertia = 0.5f;
        /// <summary>
        /// getter/setter for inertia setting
        /// </summary>
        public float BlobInertia
        {
            get
            {
                return blobInertia;
            }
            set
            {
                blobInertia = Mathf.Clamp(value, 0, maxInertia);
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        private float blobDistortion = 0.1f;
        /// <summary>
        /// getter/setter for distortion setting
        /// </summary>
        public float BlobDistortion
        {
            get
            {
                return blobDistortion;
            }
            set
            {
                blobDistortion = Mathf.Clamp(value, 0, maxDistortion);
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        private float blobRotation = 0.1f;
        /// <summary>
        /// getter/setter for rotation setting
        /// </summary>
        public float BlobRotation
        {
            get
            {
                return blobRotation;
            }
            set
            {
                blobRotation = Mathf.Clamp(value, 0, maxRotation);
            }
        }

        [SerializeField]
        [Range(0.1f, 5f)]
        private float positionCorrectionStrength = 1f;
        /// <summary>
        /// getter/setter for scalar on position correction.
        /// </summary>
        public float PositionCorrectionStrength
        {
            get
            {
                return positionCorrectionStrength;
            }
            set
            {
                positionCorrectionStrength = Mathf.Clamp(value, minPositionCorrection, maxPositionCorrection);
            }
        }

        [SerializeField]
        [Range(0.1f, 5f)]
        private float distortionCorrectionStrength = 1f;
        /// <summary>
        /// getter/setter for scalar on distortion correction.
        /// </summary>
        public float DistortionCorrectionStrength
        {
            get
            {
                return distortionCorrectionStrength;
            }
            set
            {
                distortionCorrectionStrength = Mathf.Clamp(value, minDistortionCorrection, maxDistortionCorrection);
            }
        }

        [SerializeField]
        [Range(0.1f, 5f)]
        private float rotationCorrectionStrength = 1f;
        /// <summary>
        /// getter/setter for scalar on rotation correction.
        /// </summary>
        public float RotationCorrectionStrength
        {
            get
            {
                return rotationCorrectionStrength;
            }
            set
            {
                rotationCorrectionStrength = Mathf.Clamp(value, minRotationCorrection, maxRotationCorrection);
            }
        }

        [SerializeField]
        private Vector3 blobOffset;
        /// <summary>
        /// getter/setter for offset value
        /// </summary>
        public Vector3 BlobOffset
        {
            get
            {
                return blobOffset;
            }
            set
            {
                blobOffset = value;
            }
        }

        private const float maxInertia = 5f;
        private const float maxDistortion = 1f;
        private const float maxRotation = 1f;
        private const float minPositionCorrection = 0.1f;
        private const float minDistortionCorrection = 0.1f;
        private const float minRotationCorrection = 0.1f;
        private const float maxPositionCorrection = 5f;
        private const float maxDistortionCorrection = 5f;
        private const float maxRotationCorrection = 5f;

        private Bounds defaultBounds = new Bounds(Vector3.zero, Vector3.one);
        private MeshFilter backgroundRendererMeshFilter;
        private Vector3 lastPosition;
        private Vector3 velocity;
        private Vector3 distortion;
        private Vector3 rotation;
        private Bounds localContentBounds;
        private Bounds inertialContentBounds;

        /// <summary>
        /// Mesh renderer for mesh background.
        /// </summary>
        public MeshRenderer BackgroundRenderer;

        protected override void OnEnable()
        {
            base.OnEnable();
            lastPosition = positionTarget.position;
            inertialContentBounds = defaultBounds;
            localContentBounds = defaultBounds;
            velocity = Vector3.zero;
            backgroundRendererMeshFilter = BackgroundRenderer.GetComponent<MeshFilter>();
        }

        protected override void ScaleToFitContent()
        {
            // Get the local size of the content - this is the scale of the text under the content parent
            Vector3 localContentSize = toolTip.LocalContentSize;
            Vector3 localContentOffset = toolTip.LocalContentOffset;

            // Get the size of the mesh and use this to adjust the local content size on the x / y axis
            // This will accomodate meshes that aren't built to 1,1 scale
            Bounds meshBounds = backgroundRendererMeshFilter.sharedMesh.bounds;
            localContentSize.x /= meshBounds.size.x;
            localContentSize.y /= meshBounds.size.y;
            localContentSize.z = 1;

            localContentBounds.size = Vector3.one; //localContentSize;
            localContentBounds.center = localContentOffset;
        }

        private void Update()
        {
            // Adjust center and size by velocity
            Vector3 currentPosition = positionTarget.position;
            velocity = Vector3.Lerp(velocity, lastPosition - currentPosition, 1f / blobInertia * Time.deltaTime);
            Vector3 currentDistortion = -velocity * blobDistortion;
            distortion = Vector3.Lerp(distortion, currentDistortion, 1f / blobDistortion * Time.deltaTime);

            inertialContentBounds.center = inertialContentBounds.center + velocity;
            Vector3 size = inertialContentBounds.size + distortion;
            inertialContentBounds.size = size;

            Vector3 currentRotation = Vector3.zero;
            currentRotation.x = velocity.x * 360;
            currentRotation.z = velocity.z * 360;
            currentRotation.y = velocity.y * 360;
            currentRotation = rotationTarget.TransformDirection(currentRotation);
            rotation = Vector3.Lerp (rotation, currentRotation, 1f / blobRotation * Time.deltaTime);

            // Correct the center and size
            inertialContentBounds.center = Vector3.Lerp(inertialContentBounds.center, localContentBounds.center, Time.deltaTime * positionCorrectionStrength);
            inertialContentBounds.size = Vector3.Lerp(inertialContentBounds.size, localContentBounds.size, Time.deltaTime * distortionCorrectionStrength);
            rotationTarget.localRotation = Quaternion.Lerp(positionTarget.localRotation, Quaternion.identity, Time.deltaTime * rotationCorrectionStrength);

            // Apply the center and size
            positionTarget.localPosition = inertialContentBounds.center + blobOffset;
            distortionTarget.localScale = inertialContentBounds.size;
            rotationTarget.Rotate(velocity * blobRotation * 360);

            // Adjust the tool tip attach position
            toolTip.AttachPointPosition = attachPointOffset.position;

            lastPosition = currentPosition;
        }
    }
}