// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Example of how to use interactors to create a heatmap of eye tracking data.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/DrawOnTexture")]
    [RequireComponent(typeof(Renderer))]
    internal class DrawOnTexture : MRTKBaseInteractable
    {
        [Tooltip("A lookup texture that ramps from [0,1] in UV co-ordinates in attention values")]
        [SerializeField]
        private Texture2D heatmapLookUpTable;

        [Tooltip("The brush size or spread from the eye gaze point")]
        [SerializeField]
        private float drawBrushSize = 2000.0f;

        [Tooltip("The intensity or amplitude of the brush that falls off from the eye gaze point")]
        [SerializeField]
        private float drawIntensity = 15.0f;

        [Tooltip("The minimum threshold value to apply colors to the heat map")]
        [SerializeField]
        private float minThresholdDeltaHeatMap = 0.001f; // Mostly for performance to reduce spreading heatmap for small values.

        [Tooltip("Toggle to update the heat map in real-time or use recorded values")]
        [SerializeField]
        private bool useLiveInputStream = false;

        // The internal texture reference we will modify.
        // Bound to the renderer on this GameObject.
        private Texture2D texture;

        private void Start()
        {
            SetupTexture();
        }

        private void SetupTexture()
        {
            Renderer rendererComponent = GetComponent<Renderer>();

            // Create new texture and bind it to renderer/material.
            texture = new Texture2D(rendererComponent.material.mainTexture.width, rendererComponent.material.mainTexture.width, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            
            for (int ix = 0; ix < texture.width; ix++)
            {
                for (int iy = 0; iy < texture.height; iy++)
                {
                    texture.SetPixel(ix, iy, Color.clear);
                }
            }
            texture.Apply(false);

            rendererComponent.material.SetTexture("_MainTex", texture);
        }

        protected override void OnDestroy()
        {
            Destroy(texture);
            base.OnDestroy();
        }

        /// <summary>
        /// Updates the heat map with the current eye gaze position.
        /// </summary>
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && useLiveInputStream)
            {
                foreach (var interactor in interactorsHovering)
                {
                    if (interactor is FuzzyGazeInteractor gaze)
                    {
                        DrawAtThisHitPos(gaze.PreciseHitResult.raycastHit.point);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the heat map with a hit at <paramref name="hitPosition"/>.
        /// </summary>
        /// <param name="hitPosition">The hit position in world co-ordinates.</param>
        public void DrawAtThisHitPos(Vector3 hitPosition)
        {
            Vector2? hitPosUV = GetCursorPosInTexture(hitPosition);
            if (hitPosUV != null)
            {
                StartCoroutine(DrawAt(hitPosUV.Value));
            }
        }

        private IEnumerator DrawAt(Vector2 posUV)
        {
            // Assign colors
            yield return null;

            StartCoroutine(ComputeHeatmapAt(posUV, true, true));
            yield return null;

            StartCoroutine(ComputeHeatmapAt(posUV, true, false));
            yield return null;

            StartCoroutine(ComputeHeatmapAt(posUV, false, true));
            yield return null;

            StartCoroutine(ComputeHeatmapAt(posUV, false, false));
        }

        private IEnumerator ComputeHeatmapAt(Vector2 currPosUV, bool positiveX, bool positiveY)
        {
            yield return null;

            // Determine the center of our to be drawn 'blob'
            var center = new Vector2(currPosUV.x * texture.width, currPosUV.y * texture.height);
            int signX = positiveX ? 1 : -1;
            int signY = positiveY ? 1 : -1;
            int startX = positiveX ? 0 : 1;
            int startY = positiveY ? 0 : 1;

            for (int dx = startX; dx < texture.width; dx++)
            {
                float tx = currPosUV.x * texture.width + dx * signX;
                if ((tx < 0) || (tx >= texture.width))
                    break;

                for (int dy = startY; dy < texture.height; dy++)
                {
                    float ty = currPosUV.y * texture.height + dy * signY;
                    if ((ty < 0) || (ty >= texture.height))
                        break;

                    if (ComputeHeatmapColorAt(new Vector2(tx, ty), center, out Color? newColor))
                    {
                        if (newColor.HasValue)
                        {
                            texture.SetPixel((int)tx, (int)ty, newColor.Value);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            texture.Apply(false);
        }

        private bool ComputeHeatmapColorAt(Vector2 currentPoint, Vector2 originalPivot, out Color? col)
        {
            col = null;

            float spread = drawBrushSize;
            float amplitude = drawIntensity;
            float distCenterToCurrPnt = Vector2.Distance(originalPivot, currentPoint) / spread;

            float B = 2f;
            float scaledInterest = 1f / (1f + Mathf.Pow(Mathf.Epsilon, -(B * distCenterToCurrPnt)));
            float delta = scaledInterest / amplitude;
            if (delta < minThresholdDeltaHeatMap)
                return false;

            Color baseColor = texture.GetPixel((int)currentPoint.x, (int)currentPoint.y);
            float normalizedInterest = Mathf.Clamp01(baseColor.a + delta);

            // Get color from given heatmap ramp
            if (heatmapLookUpTable != null)
            {
                col = heatmapLookUpTable.GetPixel((int)(normalizedInterest * (heatmapLookUpTable.width - 1)), 0);
                col = new Color(col.Value.r, col.Value.g, col.Value.b, normalizedInterest);
            }
            else
            {
                col = Color.blue;
                col = new Color(col.Value.r, col.Value.g, col.Value.b, normalizedInterest);
            }

            return true;
        }

        /// <summary>
        /// Determine the position of the cursor within the texture in UV space.
        /// </summary>
        /// <returns>True if this GameObject is hit.</returns>
        private Vector2? GetCursorPosInTexture(Vector3 hitPosition)
        {
            Vector2? hitPointUV = null;

            try
            {
                Vector3 center = gameObject.transform.position;
                Vector3 halfSize = gameObject.transform.localScale * 0.5f;

                // Let's transform back to the origin: Translate & Rotate
                Vector3 transformHitPoint = hitPosition - center;

                // Rotate around the y axis
                transformHitPoint = Quaternion.AngleAxis(-(gameObject.transform.rotation.eulerAngles.y - 180f), Vector3.up) * transformHitPoint;

                // Rotate around the x axis
                transformHitPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.right) * transformHitPoint;

                // Normalize the transformed hit point to as UV coordinates are in [0,1].
                float uvx = 1f - (Mathf.Clamp(transformHitPoint.x, -halfSize.x, halfSize.x) + halfSize.x) / (2f * halfSize.x);
                float uvy = (Mathf.Clamp(transformHitPoint.y, -halfSize.y, halfSize.y) + halfSize.y) / (2f * halfSize.y);
                hitPointUV = new Vector2(uvx, uvy);
            }
            catch (UnityEngine.Assertions.AssertionException)
            {
                Debug.LogError(">> AssertionException");
            }

            return hitPointUV;
        }
    }
}
