// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Note: This logic does not currently support light flares.
    /// </summary>
    internal class LightBroadcaster : ComponentBroadcaster<LightService, LightBroadcaster.ChangeType>
    {
        [Flags]
        public enum ChangeType : byte
        {
            None = 0x0,
            Properties = 0x1,
        }

        private Light lightBroadcaster;
        private LightProperties previousProperties;

        protected override void Awake()
        {
            base.Awake();

            this.lightBroadcaster = GetComponent<Light>();
        }

        public static bool HasFlag(ChangeType changeType, ChangeType flag)
        {
            return (changeType & flag) == flag;
        }

        protected override bool HasChanges(ChangeType changeFlags)
        {
            return changeFlags != ChangeType.None;
        }

        protected override ChangeType CalculateDeltaChanges()
        {
            ChangeType changeType = ChangeType.None;
            LightProperties newProperties = new LightProperties(this.lightBroadcaster);
            if (newProperties != previousProperties)
            {
                previousProperties = newProperties;
                changeType |= ChangeType.Properties;
            }

            return changeType;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            SendDeltaChanges(endpoints, ChangeType.Properties);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, ChangeType changeFlags)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                ComponentBroadcasterService.WriteHeader(message, this);

                message.Write((byte)changeFlags);

                if (HasFlag(changeFlags, ChangeType.Properties))
                {
                    message.Write(previousProperties.enabled);
                    message.Write((byte)previousProperties.type);
                    message.Write(previousProperties.bounceIntensity);
                    message.Write(previousProperties.color);
                    message.Write(previousProperties.colorTemperature);
                    AssetService.Instance.TrySerializeTexture(message, previousProperties.cookie);
                    message.Write(previousProperties.cookieSize);
                    message.Write(previousProperties.cullingMask);
                    message.Write(previousProperties.intensity);
                    message.Write(previousProperties.range);
                    message.Write(previousProperties.shadowBias);
                    message.Write(previousProperties.shadowCustomResolution);
                    message.Write(previousProperties.shadowNearPlane);
                    message.Write(previousProperties.shadowNormalBias);
                    message.Write((byte)previousProperties.shadows);
                    message.Write(previousProperties.spotAngle);
                }

                message.Flush();
                StateSynchronizationSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        private struct LightProperties
        {
            public LightProperties(Light light)
            {
                enabled = light.enabled;
                type = light.type;
                bounceIntensity = light.bounceIntensity;
                color = light.color;
                colorTemperature = light.colorTemperature;
                cookie = light.cookie;
                cookieSize = light.cookieSize;
                cullingMask = light.cullingMask;
                intensity = light.intensity;
                range = light.range;
                shadowBias = light.shadowBias;
                shadowCustomResolution = light.shadowCustomResolution;
                shadowNearPlane = light.shadowNearPlane;
                shadowNormalBias = light.shadowNormalBias;
                shadows = light.shadows;
                spotAngle = light.spotAngle;
            }

            public bool enabled;
            public LightType type;
            public float bounceIntensity;
            public Color color;
            public float colorTemperature;
            public Texture cookie;
            public float cookieSize;
            public int cullingMask;
            public float intensity;
            public float range;
            public float shadowBias;
            public int shadowCustomResolution;
            public float shadowNearPlane;
            public float shadowNormalBias;
            public LightShadows shadows;
            public float spotAngle;

            public static bool operator ==(LightProperties first, LightProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(LightProperties first, LightProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is LightProperties))
                {
                    return false;
                }

                LightProperties other = (LightProperties)obj;
                return
                    other.enabled == enabled &&
                    other.type == type &&
                    other.bounceIntensity == bounceIntensity &&
                    other.color == color &&
                    other.colorTemperature == colorTemperature &&
                    other.cookie == cookie &&
                    other.cookieSize == cookieSize &&
                    other.cullingMask == cullingMask &&
                    other.intensity == intensity &&
                    other.range == range &&
                    other.shadowBias == shadowBias &&
                    other.shadowCustomResolution == shadowCustomResolution &&
                    other.shadowNearPlane == shadowNearPlane &&
                    other.shadowNormalBias == shadowNormalBias &&
                    other.shadows == shadows &&
                    other.spotAngle == spotAngle;
            }

            public override int GetHashCode()
            {
                return type.GetHashCode();
            }
        }
    }
}