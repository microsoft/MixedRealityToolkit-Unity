// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteLineRenderer : RemoteRenderer<LineRenderer, LineRendererService>
    {
        protected override void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            base.Read(sendingEndpoint, message, changeType);

            if (SynchronizedLineRenderer.HasFlag(changeType, SynchronizedLineRenderer.LineRendererChangeType.StaticProperties))
            {
                Renderer.generateLightingData = message.ReadBoolean();
                Renderer.loop = message.ReadBoolean();
                Renderer.useWorldSpace = message.ReadBoolean();
                Renderer.numCapVertices = message.ReadInt32();
                Renderer.textureMode = (LineTextureMode)message.ReadByte();
                Renderer.alignment = (LineAlignment)message.ReadByte();
                Renderer.colorGradient = ReadColorGradient(message);
                Renderer.widthCurve = ReadAnimationCurve(message);
            }
            if (SynchronizedLineRenderer.HasFlag(changeType, SynchronizedLineRenderer.LineRendererChangeType.DynamicProperties))
            {
                Vector3[] positions = message.ReadVector3Array();
                Renderer.positionCount = positions?.Length ?? 0;
                Renderer.SetPositions(positions);
                Renderer.endColor = message.ReadColor();
                Renderer.startColor = message.ReadColor();
                Renderer.widthMultiplier = message.ReadSingle();
                Renderer.endWidth = message.ReadSingle();
                Renderer.startWidth = message.ReadSingle();
            }
        }

        private Gradient ReadColorGradient(BinaryReader message)
        {
            Gradient colorGradient = new Gradient();
            colorGradient.mode = (GradientMode)message.ReadByte();

            GradientColorKey[] colorKeys = message.ReadArray((BinaryReader msg) =>
            {
                return new GradientColorKey(msg.ReadColor(), msg.ReadSingle());
            });

            GradientAlphaKey[] alphaKeys = message.ReadArray((BinaryReader msg) =>
            {
                return new GradientAlphaKey(msg.ReadSingle(), msg.ReadSingle());
            });

            colorGradient.SetKeys(colorKeys, alphaKeys);
            return colorGradient;
        }

        private AnimationCurve ReadAnimationCurve(BinaryReader message)
        {
            AnimationCurve animationCurve = new AnimationCurve();
            animationCurve.preWrapMode = (WrapMode)message.ReadByte();
            animationCurve.postWrapMode = (WrapMode)message.ReadByte();

            animationCurve.keys = message.ReadArray((BinaryReader msg) =>
            {
                Keyframe elem = new Keyframe();
                elem.time = msg.ReadSingle();
                elem.value = msg.ReadSingle();
                elem.inTangent = msg.ReadSingle();
                elem.outTangent = msg.ReadSingle();
#if UNITY_2018_1_OR_NEWER
                elem.inWeight = msg.ReadSingle();
                elem.outWeight = msg.ReadSingle();
                elem.weightedMode = (WeightedMode)msg.ReadByte();
#else
                elem.tangentMode = msg.ReadInt32();
#endif
                return elem;
            });

            return animationCurve;
        }

        public void LerpRead(BinaryReader message, float lerpVal)
        {
            if (Renderer == null)
                return;

            byte changeType = (byte)message.ReadByte();

            //Only lerp messages with dynamic property changes on its own
            if (changeType == SynchronizedLineRenderer.LineRendererChangeType.DynamicProperties)
            {
                Vector3[] positions = message.ReadVector3Array();

                //No lerping when number of positions are changed
                if (Renderer.positionCount != positions.Length)
                    return;

                for (int i = 0; i < positions.Length; i++)
                {
                    Renderer.SetPosition(i, Vector3.Lerp(Renderer.GetPosition(i), positions[i], lerpVal));
                }

                //Dont read and lerp the rest of the data
                return;
            }
        }
    }
}