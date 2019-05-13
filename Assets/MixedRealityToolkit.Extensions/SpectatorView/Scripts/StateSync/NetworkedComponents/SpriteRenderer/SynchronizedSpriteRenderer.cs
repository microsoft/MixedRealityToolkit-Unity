// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedSpriteRenderer : SynchronizedRenderer<SpriteRenderer, SpriteRendererService>
    {
        private Sprite previousSprite;
        private SpriteRendererProperties previousData;

        public static class SpriteRendererChangeType
        {
            public const byte Sprite = 0x8;
            public const byte Properties = 0x10;
        }

        protected override byte InitialChangeType
        {
            get
            {
                return ChangeType.Enabled | ChangeType.Materials | SpriteRendererChangeType.Sprite | SpriteRendererChangeType.Properties;
            }
        }

        protected override byte CalculateDeltaChanges()
        {
            byte changeType = base.CalculateDeltaChanges();

            SpriteRendererProperties newData = new SpriteRendererProperties(Renderer);
            Sprite newSprite = Renderer.sprite;

            if (previousSprite != newSprite)
            {
                previousSprite = newSprite;
                changeType |= SpriteRendererChangeType.Sprite;
            }

            if (previousData != newData)
            {
                previousData = newData;
                changeType |= SpriteRendererChangeType.Properties;
            }

            return changeType;
        }

        protected override void WriteRenderer(BinaryWriter message, byte changeType)
        {
            base.WriteRenderer(message, changeType);

            if (HasFlag(changeType, SpriteRendererChangeType.Sprite))
            {
                message.Write(ImageService.Instance.GetSpriteId(previousSprite));
            }
            if (HasFlag(changeType, SpriteRendererChangeType.Properties))
            {
                message.Write(previousData.adaptiveModeThreshold);
                message.Write(previousData.color);
                message.Write((byte)previousData.drawMode);
                message.Write(previousData.flipX);
                message.Write(previousData.flipY);
                message.Write((byte)previousData.maskInteraction);
                message.Write(previousData.size);
                message.Write((byte)previousData.tileMode);
            }
        }

        private struct SpriteRendererProperties
        {
            public float adaptiveModeThreshold;
            public Color color;
            public SpriteDrawMode drawMode;
            public bool flipX;
            public bool flipY;
            public SpriteMaskInteraction maskInteraction;
            public Vector2 size;
            public SpriteTileMode tileMode;

            public SpriteRendererProperties(SpriteRenderer renderer)
            {
                adaptiveModeThreshold = renderer.adaptiveModeThreshold;
                color = renderer.color;
                drawMode = renderer.drawMode;
                flipX = renderer.flipX;
                flipY = renderer.flipY;
                maskInteraction = renderer.maskInteraction;
                size = renderer.size;
                tileMode = renderer.tileMode;
            }

            public static bool operator ==(SpriteRendererProperties first, SpriteRendererProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(SpriteRendererProperties first, SpriteRendererProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is SpriteRendererProperties))
                {
                    return false;
                }

                SpriteRendererProperties other = (SpriteRendererProperties)obj;
                return
                    other.adaptiveModeThreshold == adaptiveModeThreshold &&
                    other.color == color &&
                    other.drawMode == drawMode &&
                    other.flipX == flipX &&
                    other.flipY == flipY &&
                    other.maskInteraction == maskInteraction &&
                    other.size == size &&
                    other.tileMode == tileMode;
            }

            public override int GetHashCode()
            {
                return color.GetHashCode() | size.GetHashCode();
            }
        }
    }
}
