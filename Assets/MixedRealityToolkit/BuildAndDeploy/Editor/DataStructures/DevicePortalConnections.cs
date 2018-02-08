// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace MixedRealityToolkit.Build.DataStructures
{
    [Serializable]
    public class DevicePortalConnections
    {
        public List<ConnectInfo> Connections = new List<ConnectInfo>(0);

        public DevicePortalConnections(ConnectInfo connectInfo)
        {
            Connections.Add(connectInfo);
        }
    }
}
