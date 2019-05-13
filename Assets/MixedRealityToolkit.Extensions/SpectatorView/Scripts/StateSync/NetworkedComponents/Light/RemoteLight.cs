// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Note: This logic does not currently support light flares.
    /// </summary>
    internal class RemoteLight : RemoteComponent<Light>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedLight.ChangeType changeType = (SynchronizedLight.ChangeType)message.ReadByte();

            if (SynchronizedLight.HasFlag(changeType, SynchronizedLight.ChangeType.Properties))
            {
                if (attachedComponent == null)
                {
                    attachedComponent = gameObject.AddComponent<Light>();
                }

                attachedComponent.enabled = message.ReadBoolean();
                attachedComponent.type = (LightType)message.ReadByte();
                attachedComponent.bounceIntensity = message.ReadSingle();
                attachedComponent.color = message.ReadColor();
                attachedComponent.colorTemperature = message.ReadSingle();
                Texture cookieTexture;
                if (AssetService.Instance.TryDeserializeTexture(message, out cookieTexture))
                {
                    attachedComponent.cookie = cookieTexture;
                }
                attachedComponent.cookieSize = message.ReadSingle();
                attachedComponent.cullingMask = message.ReadInt32();
                attachedComponent.intensity = message.ReadSingle();
                attachedComponent.range = message.ReadSingle();
                attachedComponent.shadowBias = message.ReadSingle();
                attachedComponent.shadowCustomResolution = message.ReadInt32();
                attachedComponent.shadowNearPlane = message.ReadSingle();
                attachedComponent.shadowNormalBias = message.ReadSingle();
                attachedComponent.shadows = (LightShadows)message.ReadByte();
                attachedComponent.spotAngle = message.ReadSingle();
            }
        }
    }
}