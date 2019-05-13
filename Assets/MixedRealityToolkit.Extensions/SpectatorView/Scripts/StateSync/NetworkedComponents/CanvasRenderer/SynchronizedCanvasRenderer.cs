// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedCanvasRenderer : SynchronizedComponent<CanvasRendererService, SynchronizedCanvasRenderer.ChangeType>
    {
        [Flags]
        public enum ChangeType : byte
        {
            None = 0x0,
            Properties = 0x1,
        }

        private CanvasRenderer synchronizedCanvasRenderer;
        private CanvasRendererProperties previousValues;

        protected override void Awake()
        {
            base.Awake();

            this.synchronizedCanvasRenderer = GetComponent<CanvasRenderer>();
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

            CanvasRendererProperties newValues = new CanvasRendererProperties(synchronizedCanvasRenderer);
            if (previousValues != newValues)
            {
                changeType |= ChangeType.Properties;
                previousValues = newValues;
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
                SynchronizedComponentService.WriteHeader(message, this);

                message.Write((byte)changeFlags);

                if (HasFlag(changeFlags, ChangeType.Properties))
                {
                    message.Write(previousValues.alpha);
                    message.Write(previousValues.color);
                }

                message.Flush();
                SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        private struct CanvasRendererProperties
        {
            public CanvasRendererProperties(CanvasRenderer canvasRenderer)
            {
                alpha = canvasRenderer.GetAlpha();
                color = canvasRenderer.GetColor();
            }

            public float alpha;
            public Color color;

            public static bool operator ==(CanvasRendererProperties first, CanvasRendererProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(CanvasRendererProperties first, CanvasRendererProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is CanvasRendererProperties))
                {
                    return false;
                }

                CanvasRendererProperties other = (CanvasRendererProperties)obj;
                return
                    other.alpha == alpha &&
                    other.color == color;
            }

            public override int GetHashCode()
            {
                return alpha.GetHashCode();
            }
        }
    }
}
