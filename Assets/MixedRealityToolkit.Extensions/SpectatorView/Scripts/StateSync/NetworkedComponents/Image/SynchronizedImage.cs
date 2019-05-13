// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedImage : SynchronizedComponent<ImageService, SynchronizedImage.ChangeType>
    {
        private Image imageComp;
        private ImageProperties previousProperties;
        private SynchronizedMaterials synchronizedMaterials = new SynchronizedMaterials();

        [Flags]
        public enum ChangeType : byte
        {
            None = 0x0,
            Data = 0x1,
            Materials = 0x2,
            MaterialProperty = 0x4
        }

        protected override void Awake()
        {
            base.Awake();

            imageComp = GetComponent<Image>();
        }

        protected override bool HasChanges(ChangeType changeFlags)
        {
            return changeFlags != ChangeType.None;
        }

        protected override ChangeType CalculateDeltaChanges()
        {
            ChangeType change = ChangeType.None;
            ImageProperties newProperties = new ImageProperties(imageComp);
            if (previousProperties != newProperties)
            {
                change |= ChangeType.Data;
                previousProperties = newProperties;
            }

            bool areMaterialsDifferent;
            synchronizedMaterials.UpdateMaterials(null, SynchronizedTransform.PerformanceParameters, new Material[] { imageComp.materialForRendering }, out areMaterialsDifferent);
            if (areMaterialsDifferent)
            {
                change |= ChangeType.Materials;
            }

            if (!HasFlag(change, ChangeType.Materials))
            {
                change |= ChangeType.MaterialProperty;
            }

            return change;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            SendDeltaChanges(endpoints, ChangeType.Data | ChangeType.Materials);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, ChangeType changeFlags)
        {
            if (changeFlags != ChangeType.MaterialProperty)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter message = new BinaryWriter(memoryStream))
                {
                    SynchronizedComponentService.WriteHeader(message, this);

                    message.Write((byte)(changeFlags & ~ChangeType.MaterialProperty));

                    if (HasFlag(changeFlags, ChangeType.Data))
                    {
                        message.Write(ImageService.Instance.GetSpriteId(imageComp.overrideSprite));
                        message.Write(ImageService.Instance.GetSpriteId(imageComp.sprite));
                        message.Write(imageComp.fillAmount);
                        message.Write(imageComp.color);

                        message.Write(imageComp.alphaHitTestMinimumThreshold);
                        message.Write(imageComp.fillOrigin);
                        message.Write(imageComp.fillClockwise);
                        message.Write((byte)imageComp.fillMethod);
                        message.Write(imageComp.fillCenter);
                        message.Write(imageComp.preserveAspect);
                        message.Write((byte)imageComp.type);
                        message.Write(imageComp.enabled);
                    }

                    if (HasFlag(changeFlags, ChangeType.Materials))
                    {
                        synchronizedMaterials.SendMaterials(message, null, ShouldSynchronizeMaterialProperty);
                    }

                    message.Flush();
                    SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
                }
            }

            if (HasFlag(changeFlags, ChangeType.MaterialProperty))
            {
                synchronizedMaterials.SendMaterialPropertyChanges(endpoints, null, SynchronizedTransform.PerformanceParameters, m =>
                {
                    ImageService.Instance.WriteHeader(m, this);
                    m.Write((byte)ChangeType.MaterialProperty);
                }, ShouldSynchronizeMaterialProperty);
            }
        }

        private bool ShouldSynchronizeMaterialProperty(MaterialPropertyAsset materialProperty)
        {
            return true;
        }

        public static bool HasFlag(ChangeType changeType, ChangeType flag)
        {
            return (changeType & flag) == flag;
        }

        private struct ImageProperties
        {
            public ImageProperties(Image image)
            {
                enabled = image.enabled;
                alphaHitTestMinimumThreshold = image.alphaHitTestMinimumThreshold;
                fillOrigin = image.fillOrigin;
                fillClockwise = image.fillClockwise;
                fillMethod = image.fillMethod;
                fillCenter = image.fillCenter;
                preserveAspect = image.preserveAspect;
                type = image.type;
                overrideSprite = image.overrideSprite;
                sprite = image.sprite;
                fillAmount = image.fillAmount;
                color = image.color;
                canvasColor = image.canvasRenderer.GetColor();
                canvasAlpha = image.canvasRenderer.GetAlpha();
            }

            bool enabled;
            float alphaHitTestMinimumThreshold;
            int fillOrigin;
            bool fillClockwise;
            Image.FillMethod fillMethod;
            bool fillCenter;
            bool preserveAspect;
            Image.Type type;
            Sprite overrideSprite;
            Sprite sprite;
            float fillAmount;
            Color color;
            Color canvasColor;
            float canvasAlpha;
            //            Material material;

            public static bool operator ==(ImageProperties first, ImageProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(ImageProperties first, ImageProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ImageProperties))
                {
                    return false;
                }

                ImageProperties other = (ImageProperties)obj;
                return
                    enabled == other.enabled &&
                    alphaHitTestMinimumThreshold == other.alphaHitTestMinimumThreshold &&
                    fillOrigin == other.fillOrigin &&
                    fillClockwise == other.fillClockwise &&
                    fillMethod == other.fillMethod &&
                    fillCenter == other.fillCenter &&
                    preserveAspect == other.preserveAspect &&
                    type == other.type &&
                    overrideSprite == other.overrideSprite &&
                    sprite == other.sprite &&
                    fillAmount == other.fillAmount &&
                    color == other.color &&
                    canvasColor == other.canvasColor &&
                    canvasAlpha == other.canvasAlpha;
            }

            public override int GetHashCode()
            {
                return (int)(alphaHitTestMinimumThreshold + fillOrigin + fillAmount + canvasAlpha);
            }
        }
    }
}