// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedCanvas : SynchronizedComponent<CanvasService, SynchronizedCanvas.ChangeType>
    {
        [Flags]
        public enum ChangeType : byte
        {
            None = 0x0,
            Enabled = 0x1,
            Properties = 0x2,
        }

        private Canvas canvas;

        protected override void Awake()
        {
            base.Awake();

            canvas = GetComponent<Canvas>();
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
            // Warning: SynchronizedCanvas does not currently report changes
            return ChangeType.None;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            SendDeltaChanges(endpoints, ChangeType.Enabled | ChangeType.Properties);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, ChangeType changeType)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                SynchronizedComponentService.WriteHeader(message, this);

                message.Write((byte)changeType);

                if (HasFlag(changeType, ChangeType.Enabled))
                {
                    message.Write(canvas.enabled);
                }

                if (HasFlag(changeType, ChangeType.Properties))
                {
                    message.Write((byte)canvas.renderMode);
                    message.Write(canvas.sortingLayerID);
                    message.Write(canvas.sortingOrder);
                    message.Write(canvas.overrideSorting);
                }

                message.Flush();
                SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }
    }
}
