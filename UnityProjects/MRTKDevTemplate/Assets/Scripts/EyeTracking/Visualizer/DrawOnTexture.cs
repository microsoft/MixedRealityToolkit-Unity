// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Basic example of how to use interactors to create a simple whiteboard-like drawing system.
    /// Uses MRTKBaseInteractable, but not StatefulInteractable.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/DrawOnTexture")]
    [RequireComponent(typeof(Renderer))]
    internal class DrawOnTexture : MRTKBaseInteractable
    {
        [SerializeField]
        private Texture2D _heatmapLookUpTable;

        [SerializeField]
        private float _drawBrushSize = 2000.0f; // aka spread

        [SerializeField]
        private float _drawIntensity = 15.0f; // aka amplitude

        [SerializeField]
        private float _minThreshDeltaHeatMap = 0.001f; // Mostly for performance to reduce spreading heatmap for small values.

        [SerializeField]
        private bool _useLiveInputStream = false;

        private Renderer _renderer;

        // The internal texture reference we will modify.
        // Bound to the renderer on this GameObject.
        private Texture2D _texture;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();

            SetupTexture();
        }

        private void SetupTexture()
        {
            // Create new texture and bind it to renderer/material.
            _texture = new Texture2D(_renderer.material.mainTexture.width, _renderer.material.mainTexture.width, TextureFormat.RGBA32, false);
            _texture.hideFlags = HideFlags.HideAndDontSave;
            
            for (int ix = 0; ix < _texture.width; ix++)
            {
                for (int iy = 0; iy < _texture.height; iy++)
                {
                    _texture.SetPixel(ix, iy, Color.clear);
                }
            }
            _texture.Apply(false);

            _renderer.material.SetTexture("_MainTex", _texture);
        }

        protected override void OnDestroy()
        {
            Destroy(_texture);
            base.OnDestroy();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {   
            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && _useLiveInputStream)
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
            yield return null;
        }

        private IEnumerator ComputeHeatmapAt(Vector2 currPosUV, bool positiveX, bool positiveY)
        {
            yield return null;

            // Determine the center of our to be drawn 'blob'
            var center = new Vector2(currPosUV.x * _texture.width, currPosUV.y * _texture.height);
            int signX = (positiveX) ? 1 : -1;
            int signY = (positiveY) ? 1 : -1;
            int startX = (positiveX) ? 0 : 1;
            int startY = (positiveY) ? 0 : 1;

            for (int dx = startX; dx < _texture.width; dx++)
            {
                float tx = currPosUV.x * _texture.width + dx * signX;
                if ((tx < 0) || (tx >= _texture.width))
                    break;

                for (int dy = startY; dy < _texture.height; dy++)
                {
                    float ty = currPosUV.y * _texture.height + dy * signY;
                    if ((ty < 0) || (ty >= _texture.height))
                        break;

                    if (ComputeHeatmapColorAt(new Vector2(tx, ty), center, out Color? newColor))
                    {
                        if (newColor.HasValue)
                        {
                            _texture.SetPixel((int)(tx), (int)(ty), newColor.Value);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            _texture.Apply(false);
        }

        private bool ComputeHeatmapColorAt(Vector2 currPnt, Vector2 origPivot, out Color? col)
        {
            col = null;

            float spread = _drawBrushSize;
            float amplitude = _drawIntensity;
            float distCenterToCurrPnt = Vector2.Distance(origPivot, currPnt) / spread;

            float B = 2f;
            float scaledInterest = 1f / (1f + Mathf.Pow(Mathf.Epsilon, -(B * distCenterToCurrPnt)));
            float delta = scaledInterest / amplitude;
            if (delta < _minThreshDeltaHeatMap)
                return false;

            Color baseColor = _texture.GetPixel((int)currPnt.x, (int)currPnt.y);
            float normalizedInterest = Mathf.Clamp01(baseColor.a + delta);

            // Get color from given heatmap ramp
            if (_heatmapLookUpTable != null)
            {
                col = _heatmapLookUpTable.GetPixel((int)(normalizedInterest * (_heatmapLookUpTable.width - 1)), 0);
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
                Vector3 transfHitPnt = hitPosition - center;

                // Rotate around the y axis
                transfHitPnt = Quaternion.AngleAxis(-(gameObject.transform.rotation.eulerAngles.y - 180f), Vector3.up) * transfHitPnt;

                // Rotate around the x axis
                transfHitPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.right) * transfHitPnt;

                // Normalize the transformed hit point to as UV coordinates are in [0,1].
                float uvx = 1f - (Mathf.Clamp(transfHitPnt.x, -halfSize.x, halfSize.x) + halfSize.x) / (2f * halfSize.x);
                float uvy = (Mathf.Clamp(transfHitPnt.y, -halfSize.y, halfSize.y) + halfSize.y) / (2f * halfSize.y);
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
