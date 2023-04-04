// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Basic example of how to use interactors to create a simple whiteboard-like drawing system.
    /// Uses MRTKBaseInteractable, but not StatefulInteractable.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Whiteboard")]
    internal class Whiteboard : MRTKBaseInteractable
    {
        // Preferably power of two!
        public int TextureSize;

        // Color used to draw on texture.
        public Color32 drawingColor = new Color32(1, 0, 0, 0);

        // The internal texture reference we will modify.
        // Bound to the renderer on this GameObject.
        private Texture2D texture;

        // Used draw a full line between current frame + last frame's "paintbrush" position.
        private Dictionary<IXRInteractor, Vector2> lastPositions = new Dictionary<IXRInteractor, Vector2>();

        void Start()
        {
            // Create new texture and bind it to renderer/material.
            texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            Renderer rend = GetComponent<Renderer>();
            rend.material.SetTexture("_MainTex", texture);
        }

        public void ClearDrawing()
        {
            // Destroys texture and re-inits.
            Object.Destroy(texture);
            Start();
        }

        public void ChangeColorYellow()
        {
            drawingColor = new Color(1.0f, 0.7f, 0.0f, 1.0f);
        }

        public void ChangeColorGreen()
        {
            drawingColor = new Color(0.0f, 1.0f, 0.7f, 1.0f);
        }
        public void ChangeColorRed()
        {
            drawingColor = new Color(1.0f, 0.0f, 0.2f, 1.0f);
        }

        protected override void OnDestroy()
        {
            Object.Destroy(texture);
            base.OnDestroy();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {   
            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                NativeArray<Color32> data = texture.GetRawTextureData<Color32>();

                foreach (var interactor in interactorsSelecting)
                {
                    // attachTransform will be the actual point of the touch interaction (e.g. index tip)
                    // Most applications will probably just end up using this local touch position.
                    Vector3 localTouchPosition = transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);

                    // For whiteboard drawing: compute UV coordinates on texture by flattening Vector3 against the plane and adding 0.5f.
                    Vector2 uvTouchPosition = new Vector2(localTouchPosition.x + 0.5f, localTouchPosition.y + 0.5f);

                    // Compute pixel coords as a fraction of the texture dimension
                    Vector2 pixelCoordinate = Vector2.Scale(new Vector2(TextureSize, TextureSize), uvTouchPosition);

                    // Have we seen this interactor before? If not, last position = current position.
                    if (!lastPositions.TryGetValue(interactor, out Vector2 lastPosition))
                    {
                        lastPosition = pixelCoordinate;
                    }

                    // Very simple "line drawing algorithm".
                    for (int i = 0; i < Vector2.Distance(pixelCoordinate, lastPosition); i++)
                    {
                        DrawSplat(Vector2.Lerp(lastPosition, pixelCoordinate, i / Vector2.Distance(pixelCoordinate, lastPosition)), data);
                    }
                    
                    // Write/update the last-position.
                    if (lastPositions.ContainsKey(interactor))
                    {
                        lastPositions[interactor] = pixelCoordinate;
                    }
                    else
                    {
                        lastPositions.Add(interactor, pixelCoordinate);
                    }
                }

                texture.Apply(false);
            }
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            // Remove the interactor from our last-position collection when it leaves.
            lastPositions.Remove(args.interactorObject);
        }

        // Draws a 3x3 splat onto the texture at the specified pixel coordinates.
        private void DrawSplat(Vector2 pixelCoordinate, NativeArray<Color32> data)
        {
            // Compute index of pixel in NativeArray.
            int pixelIndex = Mathf.RoundToInt(pixelCoordinate.x) + TextureSize * Mathf.RoundToInt(pixelCoordinate.y);

            // Draw a 3x3 splat, centered on pixelIndex.
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    data[Mathf.Clamp(pixelIndex + x + (TextureSize * y), 0, data.Length - 1)] = drawingColor;
                }
            }
        }
    }
}
