// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class CameraObserver : ComponentObserver<Camera>
    {
        private bool isMainCamera;
        private int cullingMask;

        protected override void Awake()
        {
            // Don't attach a camera component
        }

        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            CameraBroadcaster.ChangeType changeType = (CameraBroadcaster.ChangeType)message.ReadByte();

            if (CameraBroadcaster.HasFlag(changeType, CameraBroadcaster.ChangeType.Properties))
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