// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data consumer that will fetch any supported image type from a URL
    /// and associated with a Quad MeshRenderer component  being managed by this
    /// object.
    /// </summary>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Image Sprite From Url")]
    public class DataConsumerImageSpriteFromUrl : DataConsumerImageTextureFromUrl
    {
        [Tooltip("(Optional) Specific sprite renderer to populate with a retrieved image. If not specified, first SpriteRenderer found in this or children will be used.")]
        [SerializeField]
        private SpriteRenderer imageSpriteRenderer;

        /// </inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(SpriteRenderer) };
            return types;
        }

        /// </inheritdoc/>
        protected override void InitializeForComponent(Component component)
        {
            if (imageSpriteRenderer == null)
            {
                // No specific SpriteRenderer specified, so assign the one that was found for us.

                imageSpriteRenderer = component as SpriteRenderer;
            }
        }

        /// </inheritdoc/>
        protected override void PlaceImageTexture(Texture2D imageTexture)
        {
            float pixelsPerUnit = Math.Max(imageTexture.width, imageTexture.height);
            Sprite newSprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);

            imageSpriteRenderer.sprite = newSprite;
        }
    }
}
