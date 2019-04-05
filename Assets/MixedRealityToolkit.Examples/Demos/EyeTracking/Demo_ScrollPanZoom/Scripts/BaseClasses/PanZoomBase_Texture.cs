// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>
    public class PanZoomBase_Texture : PanZoomBase
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
        private float defaultAspectRatio = 1.0f;

        private float aspectRatio = -1;

        private bool IsValid => (textureRenderer != null) && textureRenderer.enabled;

        public override void Initialize()
        {
            if (aspectRatio == -1)
            {
                Initialize(defaultAspectRatio);
            }
            else
            {
                Initialize(aspectRatio);
            }
        }

        public void Initialize(float newAspectRatio)
        {
            if ((IsValid) && (aspectRatio != 0))
            {
                aspectRatio = newAspectRatio;

                //# Compute and set new scale  
                ZoomStop();
                scale = new Vector2(textureRenderer.transform.localScale.x / aspectRatio, 1f);
                textureRenderer.materials[0].SetTextureScale(textureTargetID, scale);

                //# Update new values for original ratio
                originalRatio = new Vector3(scale.x, scale.y);

                BoxCollider bcoll = textureRenderer.gameObject.GetComponent<BoxCollider>();
                if (bcoll != null)
                {
                    origColliderSize = bcoll.size;
                    MyCollider = bcoll;
                }
            }
        }

        /// <summary>
        /// Returns the pan speed.
        /// </summary>
        /// <param name="uvCursorVal">Normalized cursor position in the hit box. Center is assumed to be at [-0.5, 0.5].</param>
        /// <param name="maxSpeed"></param>
        /// <param name="minDistThresh"></param>
        /// <returns></returns>
        public override float ComputePanSpeed(float uvCursorPos, float maxSpeed, float minDistFromCenterForAutoPan)
        {
            // UV space from [0,1] -> Center: [-0.5, 0.5]
            float centeredVal = (uvCursorPos - 0.5f);

            // If the UV cursor is close to the center of the image, prevent continuous movements to limit distractions
            if (Mathf.Abs(centeredVal) < minDistFromCenterForAutoPan)
            {
                return 0;
            }
            else
            {
                float normalizedVal = (centeredVal - Mathf.Sign(centeredVal) * minDistFromCenterForAutoPan) / (Mathf.Sign(centeredVal) * (0.5f - minDistFromCenterForAutoPan));
                float speed = normalizedVal * normalizedVal * maxSpeed * Mathf.Sign(centeredVal);
                return speed;
            }
        }

        public override void UpdatePanZoom()
        {
            if (IsValid)
            {
                // Limit offsets to prevent the image to float out of the viewport
                float leftBounds = 0;
                float lowerBounds = 0;
                float rightBounds = 1 - scale.x;
                float upperBounds = 1 - scale.y;

                // If the scale is smaller than the viewbox, just center the view
                if (scale.x >= 1)
                {
                    offset.x = (1 - scale.x) / 2;
                }
                else
                {
                    offset.x = Mathf.Clamp(offset.x, Mathf.Min(leftBounds, rightBounds), Mathf.Max(leftBounds, rightBounds));
                }

                // Same for y
                if (scale.y >= 1)
                {
                    offset.y = (1 - scale.y) / 2;
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

        public override int ZoomDir(bool zoomIn)
        {
            return (zoomIn ? -1 : 1);
        }

        public override void ZoomIn()
        {
            if (isZooming)
            {
                ZoomInOut(zoomDir * zoomSpeed, cursorPos);

                // Panning across entire target (-0.5, +0.5) to move target of interest towards center while zooming in
                PanHorizontally(ComputePanSpeed(cursorPos.x, panSpeedLeftRight, minDistFromCenterForAutoPan.x));
                PanVertically(ComputePanSpeed(cursorPos.y, panSpeedUpDown, minDistFromCenterForAutoPan.y));
            }
        }

        public override void ZoomOut()
        {
            if (isZooming)
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
            offsetRate_Zoom = (oldScale - newScale) * pivot;

            // Update the texture's scale to the computed value.
            scale = newScale;
        }

        /// <summary>
        /// Determine the position of the cursor within the texture in UV space.
        /// </summary>
        /// <returns>True if this GameObject is hit.</returns>
        public override bool UpdateCursorPosInHitBox()
        {
            bool objIsHit = false;

            try
            {
                if (myEyeTarget.HasFocus)
                {
                    objIsHit = true;

                    Vector3 center = gameObject.transform.position;
                    Vector3 halfsize = gameObject.transform.lossyScale / 2;

                    // Let's transform back to the origin: Translate & Rotate
                    Vector3 transfHitPnt = MixedRealityToolkit.InputSystem.EyeGazeProvider.HitPosition - center;

                    // Rotate around the y axis
                    transfHitPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.down) * transfHitPnt;

                    // Rotate around the x axis
                    transfHitPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.left) * transfHitPnt;

                    // Rotate around the z axis
                    transfHitPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.back) * transfHitPnt;

                    // Rotate around the z axis
                    transfHitPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * transfHitPnt;

                    // Normalize the transformed hit point to as UV coordinates are in [0,1].
                    float uvx = (Mathf.Clamp(transfHitPnt.x, -halfsize.x, halfsize.x) + halfsize.x) / (2 * halfsize.x);
                    float uvy = (Mathf.Clamp(transfHitPnt.y, -halfsize.y, halfsize.y) + halfsize.y) / (2 * halfsize.y);
                    cursorPos = new Vector2(uvx, uvy);
                }
                else
                {
                    objIsHit = false;
                }
            }
            catch (UnityEngine.Assertions.AssertionException)
            {
                Debug.LogError(">> AssertionException in PanZoomBase");
            }
            return objIsHit;
        }
    }
}