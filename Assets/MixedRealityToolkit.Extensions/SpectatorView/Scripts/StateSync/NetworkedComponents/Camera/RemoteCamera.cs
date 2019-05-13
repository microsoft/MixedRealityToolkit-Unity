// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteCamera : RemoteComponent<Camera>
    {
        private bool isMainCamera;
        private int cullingMask;

        protected override void Awake()
        {
            // Don't attach a camera component
        }

        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedCamera.ChangeType changeType = (SynchronizedCamera.ChangeType)message.ReadByte();

            if (SynchronizedCamera.HasFlag(changeType, SynchronizedCamera.ChangeType.Properties))
            {
                isMainCamera = message.ReadBoolean();
                cullingMask = message.ReadInt32();

                if (isMainCamera)
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera)
                    {
                        mainCamera.cullingMask = cullingMask;
                    }
                }
            }
        }
    }
}