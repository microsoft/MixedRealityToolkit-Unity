// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedLineRenderer : SynchronizedRenderer<LineRenderer, LineRendererService>
    {
        public static class LineRendererChangeType
        {
            public const byte StaticProperties = 0x8;
            public const byte DynamicProperties = 0x10;
        }

        private LineRendererDynamicData previousData = new LineRendererDynamicData();

        protected override byte InitialChangeType
        {
            get
            {
                return ChangeType.Enabled | LineRendererChangeType.StaticProperties | LineRendererChangeType.DynamicProperties | ChangeType.Materials;
            }
        }

        protected override byte CalculateDeltaChanges()
        {
            byte changeType = base.CalculateDeltaChanges();

            if (!previousData.Equals(Renderer))
            {
                previousData.Copy(Renderer);
                changeType |= LineRendererChangeType.DynamicProperties;
            }

            return changeType;
        }

        protected override void WriteRenderer(BinaryWriter message, byte changeType)
        {
            base.WriteRenderer(message, changeType);

            if (HasFlag(changeType, LineRendererChangeType.StaticProperties))
            {
                message.Write(Renderer.generateLightingData);
                message.Write(Renderer.loop);
                message.Write(Renderer.useWorldSpace);
                message.Write(Renderer.numCapVertices);
                message.Write((byte)Renderer.textureMode);
                message.Write((byte)Renderer.alignment);
                WriteColorGradient(message, Renderer.colorGradient);
                WriteAnimationCurve(message, Renderer.widthCurve);
            }
            if (HasFlag(changeType, LineRendererChangeType.DynamicProperties))
            {
                int numPosition = Renderer.positionCount;
                bool isNull = false;
                message.Write(isNull);
                message.Write(numPosition);
                for (int i = 0; i < numPosition; i++)
                {
                    message.Write(Renderer.GetPosition(i));
                }
                message.Write(Renderer.endColor);
                message.Write(Renderer.startColor);
                message.Write(Renderer.widthMultiplier);
                message.Write(Renderer.endWidth);
                message.Write(Renderer.startWidth);
            }
        }

        private void WriteColorGradient(BinaryWriter message, Gradient colorGradient)
        {
            message.Write((byte)colorGradient.mode);

            message.WriteArray(colorGradient.colorKeys, (msg, elem) =>
            {
                msg.Write(elem.color);
                msg.Write(elem.time);
            });

            message.WriteArray(colorGradient.alphaKeys, (msg, elem) =>
            {
                msg.Write(elem.alpha);
                msg.Write(elem.time);
            });
        }

        private void WriteAnimationCurve(BinaryWriter message, AnimationCurve animationCurve)
        {
            message.Write((byte)animationCurve.preWrapMode);
            message.Write((byte)animationCurve.postWrapMode);

            message.WriteArray(animationCurve.keys, (msg, elem) =>
            {
                msg.Write(elem.time);
                msg.Write(elem.value);
                msg.Write(elem.inTangent);
                msg.Write(elem.outTangent);
#if UNITY_2018_1_OR_NEWER
                msg.Write(elem.inWeight);
                msg.Write(elem.outWeight);
                msg.Write((byte)elem.weightedMode);
#else
                msg.Write(elem.tangentMode);

#endif
            });
        }

        class LineRendererDynamicData
        {
            public Color endColor;
            public Color startColor;
            public float widthMultiplier;
            public float endWidth;
            public float startWidth;
            public Vector3[] positions;

            public bool Equals(LineRenderer lineRenderer)
            {
                if (lineRenderer.endColor != endColor ||
                    lineRenderer.startColor != startColor ||
                    lineRenderer.widthMultiplier != widthMultiplier ||
                    lineRenderer.endWidth != endWidth ||
                    lineRenderer.startWidth != startWidth)
                    return false;

                int posCount = lineRenderer.positionCount;

                if (positions == null)
                {
                    return posCount == 0;
                }

                if (posCount != positions.Length)
                {
                    return false;
                }

                for (int i = 0; i < posCount; i++)
                {
                    if (lineRenderer.GetPosition(i) != positions[i])
                        return false;
                }

                return true;
            }

            public void Copy(LineRenderer lineRenderer)
            {
                endColor = lineRenderer.endColor;
                startColor = lineRenderer.startColor;
                widthMultiplier = lineRenderer.widthMultiplier;
                endWidth = lineRenderer.endWidth;
                startWidth = lineRenderer.startWidth;

                positions = null;

                int posCount = lineRenderer.positionCount;
                if (posCount > 0)
                {
                    positions = new Vector3[posCount];
                    lineRenderer.GetPositions(positions);
                }
            }
        }
    }
}