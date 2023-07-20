// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for sample. While nice to have, this documentation is not required for samples.
#pragma warning disable CS1591

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/PanZoomBaseTexture")]
    public class PanZoomBaseTexture : PanZoomBase
    {
        protected Renderer textureRenderer = null;

        private const string DefaultTextureShaderProperty = "_MainTex";
        private int textureTargetID = Shader.PropertyToID(DefaultTextureShaderProperty);
        private string textureShaderProperty = DefaultTextureShaderProperty;
        public string TextureShaderProperty
        {
            get { return textureShaderProperty; }
            set
            {
                textureShaderProperty = value;
                textureTargetID = Shader.PropertyToID(textureShaderProperty);
            }
        }

        [Tooltip("Underlying aspect ratio of the loaded texture to correctly determine scaling.")]
        [SerializeField]
        private float defaultAspectRatio = 1f;

        private float aspectRatio = -1f;

        private bool IsValid => (textureRenderer != null) && textureRenderer.enabled;

        /// <inheritdoc />
        protected override void Initialize()
        {
            if (aspectRatio == -1f)
            {
                Initialize(defaultAspectRatio);
            }
            else
            {
                Initialize(aspectRatio);
            }
        }

        private void Initialize(float newAspectRatio)
        {
            if (IsValid && aspectRatio != 0f)
            {
                aspectRatio = newAspectRatio;

                // Compute and set new scale  
                ZoomStop();
                scale = new Vector2(textureRenderer.transform.localScale.x / aspectRatio, 1f);
                textureRenderer.materials[0].SetTextureScale(textureTargetID, scale);

                // Update new values for original ratio
                originalRatio = new Vector3(scale.x, scale.y);

                if (textureRenderer.gameObject.TryGetComponent<BoxCollider>(out var boxCollider))
                {
                    originalColliderSize = boxCollider.size;
                    MyCollider = boxCollider;
                }
            }
        }

        /// <summary>
        /// Returns the pan speed.
        /// </summary>
        /// <param name="uvCursorPosition">Normalized cursor position in the hit box. Center is assumed to be at (-0.5, 0.5).</param>        
        /// <param name="maxSpeed">The maximum speed that can be returned by this function.</param>
        /// <param name="minDistanceFromCenterForAutoPan">The minium distances from the center at which to start panning.</param>
        protected override float ComputePanSpeed(float uvCursorPosition, float maxSpeed, float minDistanceFromCenterForAutoPan)
        {
            // UV space from [0,1] -> Center: [-0.5, 0.5]
            float centeredVal = uvCursorPosition - 0.5f;

            // If the UV cursor is close to the center of the image, prevent continuous movements to limit distractions
            if (Mathf.Abs(centeredVal) < minDistanceFromCenterForAutoPan)
            {
                return 0f;
            }
            else
            {
                float normalizedVal = (centeredVal - Mathf.Sign(centeredVal) * minDistanceFromCenterForAutoPan) / (Mathf.Sign(centeredVal) * (0.5f - minDistanceFromCenterForAutoPan));
                float speed = normalizedVal * normalizedVal * maxSpeed * Mathf.Sign(centeredVal);
                return speed;
            }
        }

        /// <inheritdoc />
        protected override void UpdatePanZoom()
        {
            if (IsValid)
            {
                // Limit offsets to prevent the image to float out of the viewport
                float leftBounds = 0f;
                float lowerBounds = 0f;
                float rightBounds = 1f - scale.x;
                float upperBounds = 1f - scale.y;

                // If the scale is smaller than the viewbox, just center the view
                if (scale.x >= 1f)
                {
                    offset.x = (1f - scale.x) / 2f;
                }
                else
                {
                    offset.x = Mathf.Clamp(offset.x, Mathf.Min(leftBounds, rightBounds), Mathf.Max(leftBounds, rightBounds));
                }

                // Same for y
                if (scale.y >= 1f)
                {
                    offset.y = (1f - scale.y) / 2f;
                }
                else
                {
                    offset.y = Mathf.Clamp(offset.y, Mathf.Min(lowerBounds, upperBounds), Mathf.Max(lowerBounds, upperBounds));
                }

                // Assign new values
                textureRenderer.materials[0].SetTextureOffset(textureTargetID, offset); // Pan
                textureRenderer.materials[0].SetTextureScale(textureTargetID, scale); // Zoom                 
            }
        }

        /// <inheritdoc />
        protected override int ZoomDir(bool zoomIn)
        {
            return zoomIn ? -1 : 1;
        }

        /// <inheritdoc />
        protected override void ZoomIn()
        {
            if (IsZooming)
            {
                ZoomInOut(zoomDirection * zoomSpeed, cursorPosition);

                // Panning across entire target (-0.5, +0.5) to move target of interest towards center while zooming in
                PanHorizontally(ComputePanSpeed(cursorPosition.x, panSpeedLeftRight, minDistFromCenterForAutoPan.x));
                PanVertically(ComputePanSpeed(cursorPosition.y, panSpeedUpDown, minDistFromCenterForAutoPan.y));
            }
        }

        /// <inheritdoc />
        protected override void ZoomOut()
        {
            if (IsZooming)
            {
                ZoomInOut(zoomSpeed, new Vector2(0.5f, 0.5f));
            }
        }


        /// <summary>
        /// Update the UV scale and UV offset for zooming.
        /// </summary>
        /// <param name="speed">Zoom speed</param>
        /// <param name="pivot">Zoom pivot</param>
        private void ZoomInOut(float speed, Vector2 pivot)
        {
            Vector2 oldScale = new Vector2(scale.x, scale.y); ;
            Vector2 newScale = new Vector2(scale.x + speed * originalRatio.x, scale.y + speed * originalRatio.y);
            newScale = LimitScaling(newScale);

            // Update the offset based on the scale diff and the zoom pivot.
            offsetRateZoom = (oldScale - newScale) * pivot;

            // Update the texture's scale to the computed value.
            scale = newScale;
        }

        /// <summary>
        /// Determine the position of the cursor within the texture in UV space.
        /// </summary>
        /// <returns>True if this GameObject is hit.</returns>
        protected override bool UpdateCursorPosInHitBox(Vector3 hitPosition)
        {
            Vector3 center = gameObject.transform.position;
            Vector3 halfsize = gameObject.transform.lossyScale * 0.5f;

            // Let's transform back to the origin: Translate & Rotate
            Vector3 transformHitPoint = hitPosition - center;

            // Rotate around the y axis
            transformHitPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.down) * transformHitPoint;

            // Rotate around the x axis
            transformHitPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.left) * transformHitPoint;

            // Rotate around the z axis
            transformHitPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.back) * transformHitPoint;

            // Rotate around the z axis
            transformHitPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * transformHitPoint;

            // Normalize the transformed hit point to as UV coordinates are in [0,1].
            float uvx = (Mathf.Clamp(transformHitPoint.x, -halfsize.x, halfsize.x) + halfsize.x) / (2f * halfsize.x);
            float uvy = (Mathf.Clamp(transformHitPoint.y, -halfsize.y, halfsize.y) + halfsize.y) / (2f * halfsize.y);
            cursorPosition = new Vector2(uvx, uvy);

            return true;
        }
    }
}

#pragma warning restore CS1591
